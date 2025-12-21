using UnityEngine;

[CreateAssetMenu(fileName = "GrassData", menuName = "Data/GrassData")]
public class GrassData : ScriptableObject
{
    public GrassType type;
    public Color color;
    public float respawnTime = 3f;
    public float spacing = 0.5f;
    public float randomOffsetRadius = 0.2f;
    public float randomScale = 0.3f;
    public int pricePerStack = 100;
}

