using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Zenject;

public class SalesTable : MonoBehaviour
{
    public GrassInventory tableInventory;
    public GrassInventory playerInventory;
    public QueueManager queueManager;
    public Transform exitPoint;
    public float customerCheckInterval = 1f;

    MoneyManager moneyManager;
    Dictionary<GrassType, GrassData> grassDataCache = new Dictionary<GrassType, GrassData>();

    float customerCheckTimer;

    [Inject]
    public void Construct(MoneyManager money)
    {
        moneyManager = money;
    }

    void Start()
    {
        CacheGrassData();
        customerCheckTimer = customerCheckInterval;
    }

    void Update()
    {
        customerCheckTimer -= Time.deltaTime;
        if (customerCheckTimer <= 0f)
        {
            CheckCustomerPurchase();
            customerCheckTimer = customerCheckInterval;
        }
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

    void CheckCustomerPurchase()
    {
        if (queueManager == null) return;

        Customer first = queueManager.GetFirstCustomer();
        if (first == null || first.state != CustomerState.InQueue) return;

        if (CanFulfillOrder(first.order))
        {
            TransferCubesToCustomer(first);
            first.StartLeaving(exitPoint);
            queueManager.RemoveCustomer(first);
            queueManager.ShiftQueue();
        }
    }

    bool CanFulfillOrder(CustomerOrder order)
    {
        if (order == null || order.requirements == null) return false;

        foreach (var requirement in order.requirements)
        {
            int available = tableInventory.GetAllStacks()
                .Where(s => s.IsFull && s.type == requirement.Key)
                .Count();
            
            if (available < requirement.Value) return false;
        }
        return true;
    }

    void TransferCubesToCustomer(Customer customer)
    {
        if (customer == null || customer.order == null) return;

        foreach (var requirement in customer.order.requirements)
        {
            GrassType type = requirement.Key;
            int needed = requirement.Value;

            var matchingStacks = tableInventory.GetAllStacks()
                .Where(s => s.IsFull && s.type == type)
                .Take(needed)
                .ToList();

            foreach (var stack in matchingStacks)
            {
                customer.ReceiveGrass(stack.type, stack.color, stack.count);

                int price = 100;
                if (grassDataCache.TryGetValue(stack.type, out GrassData data))
                {
                    price = data.pricePerStack;
                }

                if (moneyManager != null)
                {
                    moneyManager.AddMoney(price);
                }

                tableInventory.GetAllStacks().Remove(stack);
            }
        }

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

        if (exitPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(exitPoint.position, 0.5f);
        }
    }
}
