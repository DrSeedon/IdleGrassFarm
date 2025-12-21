using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Zenject;
using Cysharp.Threading.Tasks;

public class SalesTable : MonoBehaviour
{
    public GrassInventory tableInventory;
    public GrassInventory playerInventory;
    public float sellInterval = 1f;

    MoneyManager moneyManager;
    Dictionary<GrassType, GrassData> grassDataCache = new Dictionary<GrassType, GrassData>();

    bool isSelling = false;

    [Inject]
    public void Construct(MoneyManager money)
    {
        moneyManager = money;
    }

    void Start()
    {
        CacheGrassData();
    }

    void CacheGrassData()
    {
        GrassFieldRenderer[] fields = FindObjectsOfType<GrassFieldRenderer>();
        foreach (var field in fields)
        {
            if (field.grassData != null && !grassDataCache.ContainsKey(field.grassData.type))
            {
                grassDataCache[field.grassData.type] = field.grassData;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            TransferInventoryToTable();
            if (!isSelling && tableInventory.GetAllStacks().Any(s => s.IsFull))
            {
                StartSellingProcess().Forget();
            }
        }
    }

    void TransferInventoryToTable()
    {
        if (playerInventory == null || tableInventory == null) return;

        var fullStacks = playerInventory.GetAllStacks().Where(s => s.IsFull).ToList();

        foreach (var stack in fullStacks)
        {
            tableInventory.AddGrass(stack.type, stack.color, stack.count);
        }

        playerInventory.GetAllStacks().RemoveAll(s => s.IsFull);
        playerInventory.onInventoryChanged?.Invoke();
    }

    async UniTaskVoid StartSellingProcess()
    {
        isSelling = true;

        while (tableInventory.GetAllStacks().Any(s => s.IsFull))
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(sellInterval));

            if (tableInventory.GetAllStacks().Any(s => s.IsFull))
            {
                SellOneCube();
            }
        }

        isSelling = false;
    }

    void SellOneCube()
    {
        if (tableInventory == null) return;

        var fullStacks = tableInventory.GetAllStacks().Where(s => s.IsFull).ToList();
        if (fullStacks.Count == 0) return;

        var stackToSell = fullStacks[0];
        
        int price = 100;
        if (grassDataCache.TryGetValue(stackToSell.type, out GrassData data))
        {
            price = data.pricePerStack;
        }

        if (moneyManager != null)
        {
            moneyManager.AddMoney(price);
        }

        tableInventory.GetAllStacks().Remove(stackToSell);
        tableInventory.onInventoryChanged?.Invoke();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
        }
    }
}
