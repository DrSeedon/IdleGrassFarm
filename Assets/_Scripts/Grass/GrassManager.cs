using UnityEngine;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;

public class GrassManager : MonoBehaviour
{
    public Material grassMaterialTemplate;

    public SerializedDictionary<GrassData, Material> grassMaterials = new SerializedDictionary<GrassData, Material>();
    
    int tramplePosID;
    int trampleRadiusID;
    int baseColorID;
    int topColorID;
    
    Vector3 currentTramplePos;
    float currentTrampleRadius;

    void Awake()
    {
        tramplePosID = Shader.PropertyToID("_TramplePos");
        trampleRadiusID = Shader.PropertyToID("_TrampleRadius");
        baseColorID = Shader.PropertyToID("_BaseColor");
        topColorID = Shader.PropertyToID("_TopColor");
    }

    void OnDestroy()
    {
        foreach (var mat in grassMaterials.Values)
        {
            if (mat != null)
            {
                Destroy(mat);
            }
        }
        grassMaterials.Clear();
    }

    public Material GetGrassMaterial(GrassData grassData)
    {
        if (grassData == null || grassMaterialTemplate == null) return null;

        if (!grassMaterials.ContainsKey(grassData))
        {
            Material instance = new Material(grassMaterialTemplate);
            instance.name = $"Grass_{grassData.type}_{grassData.GetInstanceID()}";
            instance.enableInstancing = true;
            
            instance.SetColor(baseColorID, grassData.color * 0.7f);
            instance.SetColor(topColorID, grassData.color);
            
            grassMaterials[grassData] = instance;
            
            Debug.Log($"Created material for {grassData.type}: {instance.name}");
        }

        return grassMaterials[grassData];
    }

    public void SetTramplePosition(Vector3 position, float radius)
    {
        currentTramplePos = position;
        currentTrampleRadius = radius;
    }
    
    public void ApplyTrampleToPropertyBlock(MaterialPropertyBlock block)
    {
        if (block == null) return;
        
        block.SetVector(tramplePosID, new Vector4(currentTramplePos.x, currentTramplePos.y, currentTramplePos.z, 0));
        block.SetFloat(trampleRadiusID, currentTrampleRadius);
    }
    
    public int GetTramplePosID() => tramplePosID;
    public int GetTrampleRadiusID() => trampleRadiusID;
}
