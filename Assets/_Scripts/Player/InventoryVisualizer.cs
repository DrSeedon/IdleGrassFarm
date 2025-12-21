using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class InventoryVisualizer : MonoBehaviour
{
    public GrassInventory inventory;
    public Transform backpackRoot;
    public GameObject stackCubePrefab;
    public float stackSize = 0.3f;
    public float stackSpacing = 0.05f;
    public int stacksPerRow = 5;

    List<GameObject> visualStacks = new List<GameObject>();

    void Start()
    {
        if (inventory != null)
        {
            inventory.onInventoryChanged.AddListener(UpdateVisuals);
            UpdateVisuals();
        }
    }

    void OnDestroy()
    {
        if (inventory != null)
        {
            inventory.onInventoryChanged.RemoveListener(UpdateVisuals);
        }
    }

    void UpdateVisuals()
    {
        foreach (var obj in visualStacks)
        {
            if (obj != null) Destroy(obj);
        }
        visualStacks.Clear();

        if (inventory == null) return;

        var allStacks = inventory.GetAllStacks();
        var fullStacks = allStacks.Where(s => s.IsFull).ToList();

        for (int i = 0; i < fullStacks.Count; i++)
        {
            var stack = fullStacks[i];
            SpawnCubeVisual(i, stack);
        }
    }

    void SpawnCubeVisual(int index, InventoryStack stack)
    {
        if (backpackRoot == null) return;

        int row = index / stacksPerRow;
        int col = index % stacksPerRow;

        float xOffset = col * (stackSize + stackSpacing);
        float yOffset = row * (stackSize + stackSpacing);

        Vector3 localPos = new Vector3(xOffset, yOffset, 0);
        Vector3 worldPos = backpackRoot.TransformPoint(localPos);

        GameObject cube;
        
        if (stackCubePrefab != null)
        {
            cube = Instantiate(stackCubePrefab, worldPos, Quaternion.identity, backpackRoot);
            cube.transform.localScale = Vector3.one * stackSize;
            
            var stackCube = cube.GetComponent<GrassStackCube>();
            if (stackCube != null)
            {
                stackCube.Initialize(stack.type, stack.color, 0);
            }
        }
        else
        {
            cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.SetParent(backpackRoot);
            cube.transform.position = worldPos;
            cube.transform.localScale = Vector3.one * stackSize;
            cube.transform.localRotation = Quaternion.identity;

            var renderer = cube.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = new Material(Shader.Find("Standard"));
                renderer.material.color = stack.color;
            }

            var collider = cube.GetComponent<Collider>();
            if (collider != null) Destroy(collider);
        }

        visualStacks.Add(cube);
    }

    void OnDrawGizmosSelected()
    {
        if (backpackRoot == null) return;

        int maxStacks = inventory != null ? inventory.maxFullStacks : 10;
        Gizmos.color = new Color(1f, 1f, 0f, 0.3f);

        for (int i = 0; i < maxStacks; i++)
        {
            int row = i / stacksPerRow;
            int col = i % stacksPerRow;

            float xOffset = col * (stackSize + stackSpacing);
            float yOffset = row * (stackSize + stackSpacing);

            Vector3 worldPos = backpackRoot.TransformPoint(new Vector3(xOffset, yOffset, 0));
            Gizmos.DrawCube(worldPos, Vector3.one * stackSize);
            Gizmos.DrawWireCube(worldPos, Vector3.one * stackSize);
        }
    }
}
