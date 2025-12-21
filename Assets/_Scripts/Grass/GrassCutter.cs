using UnityEngine;
using System.Collections.Generic;

public class GrassCutter : MonoBehaviour
{
    public float cutRadius = 1f;
    public float cutInterval = 0.1f;
    public GrassInventory inventory;

    float cutTimer = 0f;
    Dictionary<GrassType, int> cutCounts = new Dictionary<GrassType, int>();

    void Update()
    {
        if (inventory != null && inventory.IsFull()) return;

        cutTimer -= Time.deltaTime;

        if (cutTimer <= 0f)
        {
            CutAllGrass();
            cutTimer = cutInterval;
        }
    }

    void CutAllGrass()
    {
        GrassFieldRenderer[] fields = FindObjectsOfType<GrassFieldRenderer>();
        
        foreach (var field in fields)
        {
            int cutCount = field.CutGrassInRadius(transform.position, cutRadius);
            
            if (cutCount > 0)
            {
                GrassType type = field.GetGrassType();
                
                if (!cutCounts.ContainsKey(type))
                {
                    cutCounts[type] = 0;
                }
                
                cutCounts[type] += cutCount;
                Debug.Log($"Cut {type} grass: +{cutCount} (Total: {cutCounts[type]})");

                if (inventory != null && field.grassData != null)
                {
                    inventory.AddGrass(type, field.grassData.color, cutCount);
                }
            }
        }
    }

    public int GetCutCount(GrassType type)
    {
        return cutCounts.ContainsKey(type) ? cutCounts[type] : 0;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, cutRadius);
    }
}

