using UnityEngine;

public class GrassMeshGenerator : MonoBehaviour
{
    public float width = 0.1f;
    public float height = 0.5f;

    [ContextMenu("Generate Grass Mesh")]
    void GenerateGrassMesh()
    {
        Mesh mesh = new Mesh();
        mesh.name = "GrassBlade";

        Vector3[] vertices = new Vector3[]
        {
            new Vector3(-width * 0.5f, 0, 0),
            new Vector3(width * 0.5f, 0, 0),
            new Vector3(-width * 0.3f, height * 0.5f, 0),
            new Vector3(width * 0.3f, height * 0.5f, 0),
            new Vector3(0, height, 0)
        };

        int[] triangles = new int[]
        {
            0, 2, 1,
            1, 2, 3,
            2, 4, 3
        };

        Vector2[] uvs = new Vector2[]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 0.5f),
            new Vector2(1, 0.5f),
            new Vector2(0.5f, 1)
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        string path = "Assets/Prefabs/GrassBlade.asset";
        
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.CreateAsset(mesh, path);
        UnityEditor.AssetDatabase.SaveAssets();
        Debug.Log($"Grass mesh created at {path}");
#endif
    }
}

