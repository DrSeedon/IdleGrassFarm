using UnityEngine;
using System.Collections.Generic;
using Zenject;

public class GrassFieldRenderer : MonoBehaviour
{
    public Mesh grassMesh;
    public GrassData grassData;
    
    public int gridWidth = 50;
    public int gridHeight = 50;

    Matrix4x4[] matrices;
    MaterialPropertyBlock propertyBlock;
    
    Material grassMaterial;
    
    GrassState[] grassStates;
    List<int> activeGrassIndices = new List<int>();

    GrassManager grassManager;

    [Inject]
    public void Construct(GrassManager manager)
    {
        grassManager = manager;
    }

    struct GrassState
    {
        public Vector3 position;
        public bool isGrown;
        public float respawnTimer;
    }

    void Start()
    {
        if (grassManager != null && grassData != null)
        {
            grassMaterial = grassManager.GetGrassMaterial(grassData);
        }
        
        GenerateGrassField();
    }

    void OnDestroy()
    {
    }

    void GenerateGrassField()
    {
        if (grassData == null) return;
        
        int totalGrass = gridWidth * gridHeight;
        matrices = new Matrix4x4[totalGrass];
        grassStates = new GrassState[totalGrass];
        
        int index = 0;
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                Vector3 basePos = transform.position + new Vector3(x * grassData.spacing, 0, z * grassData.spacing);
                
                Vector2 randomOffset = Random.insideUnitCircle * grassData.randomOffsetRadius;
                Vector3 pos = basePos + new Vector3(randomOffset.x, 0, randomOffset.y);
                
                float scale = 1f + Random.Range(-grassData.randomScale, grassData.randomScale);
                float rotY = Random.Range(0f, 360f);
                
                matrices[index] = Matrix4x4.TRS(
                    pos,
                    Quaternion.Euler(0, rotY, 0),
                    new Vector3(scale, scale, scale)
                );
                
                grassStates[index] = new GrassState
                {
                    position = pos,
                    isGrown = true,
                    respawnTimer = 0
                };
                
                index++;
            }
        }

        propertyBlock = new MaterialPropertyBlock();
        UpdateActiveGrass();
    }

    void Update()
    {
        if (matrices == null || grassMesh == null || grassMaterial == null) return;

        UpdateRespawn();
        
        if (grassManager != null)
        {
            grassManager.ApplyTrampleToPropertyBlock(propertyBlock);
        }
        
        for (int i = 0; i < activeGrassIndices.Count; i += 1023)
        {
            int batchSize = Mathf.Min(1023, activeGrassIndices.Count - i);
            Matrix4x4[] batch = new Matrix4x4[batchSize];
            
            for (int j = 0; j < batchSize; j++)
            {
                batch[j] = matrices[activeGrassIndices[i + j]];
            }
            
            Graphics.DrawMeshInstanced(grassMesh, 0, grassMaterial, batch, batchSize, propertyBlock);
        }
    }

    void UpdateRespawn()
    {
        bool needsUpdate = false;
        
        for (int i = 0; i < grassStates.Length; i++)
        {
            if (!grassStates[i].isGrown)
            {
                grassStates[i].respawnTimer -= Time.deltaTime;
                
                if (grassStates[i].respawnTimer <= 0)
                {
                    grassStates[i].isGrown = true;
                    needsUpdate = true;
                }
            }
        }
        
        if (needsUpdate)
        {
            UpdateActiveGrass();
        }
    }

    void UpdateActiveGrass()
    {
        activeGrassIndices.Clear();
        
        for (int i = 0; i < grassStates.Length; i++)
        {
            if (grassStates[i].isGrown)
            {
                activeGrassIndices.Add(i);
            }
        }
    }

    public int CutGrassInRadius(Vector3 position, float radius)
    {
        if (grassData == null) return 0;
        
        bool anyCut = false;
        int cutCount = 0;
        
        for (int i = 0; i < grassStates.Length; i++)
        {
            if (grassStates[i].isGrown)
            {
                float distance = Vector3.Distance(grassStates[i].position, position);
                
                if (distance <= radius)
                {
                    grassStates[i].isGrown = false;
                    grassStates[i].respawnTimer = grassData.respawnTime;
                    anyCut = true;
                    cutCount++;
                }
            }
        }
        
        if (anyCut)
        {
            UpdateActiveGrass();
        }
        
        return cutCount;
    }
    
    public GrassType GetGrassType()
    {
        return grassData != null ? grassData.type : GrassType.Green;
    }

    void OnDrawGizmosSelected()
    {
        if (grassData == null) return;
        
        Gizmos.color = grassData.color;
        
        Vector3 fieldSize = new Vector3(gridWidth * grassData.spacing, 0.1f, gridHeight * grassData.spacing);
        Vector3 fieldCenter = transform.position + new Vector3(fieldSize.x * 0.5f - grassData.spacing * 0.5f, 0, fieldSize.z * 0.5f - grassData.spacing * 0.5f);
        Gizmos.DrawWireCube(fieldCenter, fieldSize);
        
        Gizmos.color = new Color(grassData.color.r, grassData.color.g, grassData.color.b, 0.3f);
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                Vector3 pos = transform.position + new Vector3(x * grassData.spacing, 0, z * grassData.spacing);
                Gizmos.DrawLine(pos, pos + Vector3.up * 0.3f);
            }
        }
    }
}

