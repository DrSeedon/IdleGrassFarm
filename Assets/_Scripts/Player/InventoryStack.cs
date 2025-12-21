using UnityEngine;

[System.Serializable]
public class InventoryStack
{
    public GrassType type;
    public Color color;
    public int count;
    public const int MaxCount = 100;

    public bool IsFull => count >= MaxCount;
    public bool IsEmpty => count <= 0;

    public InventoryStack(GrassType type, Color color)
    {
        this.type = type;
        this.color = color;
        this.count = 0;
    }

    public int AddGrass(int amount)
    {
        int canAdd = Mathf.Min(amount, MaxCount - count);
        count += canAdd;
        return amount - canAdd;
    }
}
