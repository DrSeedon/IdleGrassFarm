using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

public class GrassInventory : MonoBehaviour
{
    public int maxFullStacks = 10;
    public UnityEvent onInventoryChanged;

    List<InventoryStack> stacks = new List<InventoryStack>();
    Dictionary<GrassType, int> grassMaxCountCache = new Dictionary<GrassType, int>();

    void Start()
    {
        CacheGrassMaxCounts();
    }

    void CacheGrassMaxCounts()
    {
        GrassFieldRenderer[] fields = FindObjectsOfType<GrassFieldRenderer>();
        foreach (var field in fields)
        {
            if (field.grassData != null && !grassMaxCountCache.ContainsKey(field.grassData.type))
            {
                grassMaxCountCache[field.grassData.type] = field.grassData.grassPerStack;
            }
        }
    }

    public bool IsFull()
    {
        int fullStackCount = stacks.Count(s => s.IsFull);
        return fullStackCount >= maxFullStacks;
    }

    public bool CanAddGrass(GrassType type)
    {
        var existingStack = stacks.FirstOrDefault(s => s.type == type && !s.IsFull);
        if (existingStack != null) return true;

        int fullStackCount = stacks.Count(s => s.IsFull);
        return fullStackCount < maxFullStacks;
    }

    public void AddGrass(GrassType type, Color color, int amount)
    {
        int remaining = amount;

        while (remaining > 0)
        {
            var existingStack = stacks.FirstOrDefault(s => s.type == type && !s.IsFull);

            if (existingStack != null)
            {
                remaining = existingStack.AddGrass(remaining);
            }
            else
            {
                if (IsFull()) break;

                int maxCount = grassMaxCountCache.ContainsKey(type) ? grassMaxCountCache[type] : 100;
                var newStack = new InventoryStack(type, color, maxCount);
                stacks.Add(newStack);
                remaining = newStack.AddGrass(remaining);
            }
        }

        onInventoryChanged?.Invoke();
    }

    public List<InventoryStack> GetAllStacks()
    {
        return stacks;
    }

    public int GetFullStackCount()
    {
        return stacks.Count(s => s.IsFull);
    }
}
