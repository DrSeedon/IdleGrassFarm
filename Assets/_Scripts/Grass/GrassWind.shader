Shader "Custom/GrassWind"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.3, 0.8, 0.3, 1)
        _TopColor ("Top Color", Color) = (0.5, 1, 0.5, 1)
        
        _WindSpeed ("Wind Speed", Range(0, 5)) = 1
        _WindStrength ("Wind Strength", Range(0, 1)) = 0.3
        _WindScale ("Wind Scale", Range(0.1, 10)) = 2
        
        [Header(Trample)]
        _TramplePos ("Trample Position", Vector) = (0, 0, 0, 0)
        _TrampleRadius ("Trample Radius", Float) = 1
        _TrampleStrength ("Trample Strength", Range(0, 1)) = 0.5
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            float4 _BaseColor;
            float4 _TopColor;
            float _WindSpeed;
            float _WindStrength;
            float _WindScale;
            
            float4 _TramplePos;
            float _TrampleRadius;
            float _TrampleStrength;
            
            v2f vert(appdata v)
            {
                v2f o;
                
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                
                float windNoise = sin(worldPos.x * _WindScale + _Time.y * _WindSpeed) * 
                                  cos(worldPos.z * _WindScale * 0.7 + _Time.y * _WindSpeed * 0.8);
                
                float windEffect = v.uv.y * _WindStrength * windNoise;
                
                v.vertex.x += windEffect;
                v.vertex.z += windEffect * 0.5;
                
                float3 worldPosAfterWind = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 trampleDiff = worldPosAfterWind - _TramplePos.xyz;
                float dist = length(trampleDiff);
                float trampleEffect = 1.0 - saturate(dist / _TrampleRadius);
                
                if (trampleEffect > 0.01)
                {
                    float3 pushDir = normalize(float3(trampleDiff.x, 0, trampleDiff.z));
                    float3 offset = pushDir * trampleEffect * _TrampleStrength * v.uv.y;
                    v.vertex.xyz += mul((float3x3)unity_WorldToObject, offset);
                }
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                
                fixed4 col = lerp(_BaseColor, _TopColor, i.uv.y);
                return col;
            }
            ENDCG
        }
    }
}

