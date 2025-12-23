using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Zenject;
using UnityEngine.Events;

public class SalesTable : MonoBehaviour
{
    public GrassInventory tableInventory;
    public GrassInventory playerInventory;
    public QueueManager queueManager;
    public Transform exitPoint;
    public float customerCheckInterval = 1f;

    public UnityEvent onGrassSold;

    MoneyManager moneyManager;
    Dictionary<GrassType, GrassData> grassDataCache = new Dictionary<GrassType, GrassData>();

    float customerCheckTimer;
    string cachedDebugText = "";
    bool needsTextUpdate = true;

    [Inject]
    public void Construct(MoneyManager money)
    {
        moneyManager = money;
    }

    void Start()
    {
        CacheGrassData();
        customerCheckTimer = customerCheckInterval;
        
        if (tableInventory != null)
        {
            tableInventory.onInventoryChanged.AddListener(MarkTextDirty);
        }
    }

    void OnDestroy()
    {
        if (tableInventory != null)
        {
            tableInventory.onInventoryChanged.RemoveListener(MarkTextDirty);
        }
    }

    void MarkTextDirty()
    {
        needsTextUpdate = true;
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

                if (grassDataCache.TryGetValue(stack.type, out GrassData data))
                {
                if (moneyManager != null)
                {
                        moneyManager.AddRedCurrency(data.redCurrencyPerStack);
                        moneyManager.AddYellowCurrency(data.yellowCurrencyPerStack);
                    }
                }

                tableInventory.GetAllStacks().Remove(stack);
            }
        }

        onGrassSold?.Invoke();
        tableInventory.onInventoryChanged?.Invoke();
    }

    void UpdateDebugText()
    {
            string text = "--- SALES TABLE ---\n";

            if (tableInventory != null)
            {
                var fullStacks = tableInventory.GetAllStacks().Where(s => s.IsFull).ToList();
                text += $"Total stacks: {fullStacks.Count}\n";

                if (fullStacks.Count > 0)
                {
                    var grouped = fullStacks.GroupBy(s => s.type);
                    foreach (var group in grouped)
                    {
                        text += $"  {group.Key}: {group.Count()}\n";
                    }
                }
                else
                {
                    text += "  Empty\n";
                }
            }

            if (queueManager != null)
            {
                text += $"\nQueue: {queueManager.GetQueuePosition(null) + 1} customers\n";

                Customer first = queueManager.GetFirstCustomer();
                if (first != null && first.order != null)
                {
                    text += "First wants:\n";
                    foreach (var req in first.order.requirements)
                    {
                        text += $"  {req.Key}x{req.Value}\n";
                    }
                }
            }

        cachedDebugText = text;
        needsTextUpdate = false;
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

    void OnGUI()
    {
        if (Camera.main == null) return;

        if (needsTextUpdate)
        {
            UpdateDebugText();
        }

        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 2f);
        
        if (screenPos.z > 0)
        {
            screenPos.y = Screen.height - screenPos.y;

            GUIStyle style = new GUIStyle(GUI.skin.box);
            style.fontSize = 14;
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.UpperLeft;
            style.padding = new RectOffset(10, 10, 10, 10);

            Vector2 size = style.CalcSize(new GUIContent(cachedDebugText));
            size.x += 20;
            size.y += 20;

            GUI.Box(new Rect(screenPos.x - size.x * 0.5f, screenPos.y - size.y * 0.5f, size.x, size.y), cachedDebugText, style);
        }
    }
}
