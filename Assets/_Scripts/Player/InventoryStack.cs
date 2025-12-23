using UnityEngine;

[System.Serializable]
public class InventoryStack
{
    public GrassType type;
    public Color color;
    public int count;
    public int maxCount = 100;

    public bool IsFull => count >= maxCount;
    public bool IsEmpty => count <= 0;

    public InventoryStack(GrassType type, Color color, int maxCount = 100)
    {
        this.type = type;
        this.color = color;
        this.count = 0;
        this.maxCount = maxCount;
    }

    public int AddGrass(int amount)
    {
        int canAdd = Mathf.Min(amount, maxCount - count);
        count += canAdd;
        return amount - canAdd;
    }
}
