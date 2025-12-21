using UnityEngine;

public class GrassStackCube : MonoBehaviour
{
    public GrassType type;
    public int price;
    public Material cubeMaterial;

    public void Initialize(GrassType grassType, Color color, int stackPrice)
    {
        type = grassType;
        price = stackPrice;

        var renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            if (cubeMaterial != null)
            {
                renderer.material = cubeMaterial;
            }
            renderer.material.color = color;
        }
    }

    public void SetColor(Color color)
    {
        var renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = color;
        }
    }
}
