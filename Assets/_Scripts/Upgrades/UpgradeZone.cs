using UnityEngine;
using Zenject;
using UnityEngine.Events;

public class UpgradeZone : MonoBehaviour
{
    public UpgradeType upgradeType;
    public int baseRedCost = 50;
    public int baseYellowCost = 50;
    public float costMultiplier = 1.5f;
    public float upgradeAmount = 0.5f;
    public int maxLevel = 10;

    public UnityEvent onUpgrade;
    
    MoneyManager moneyManager;
    int currentLevel = 0;

    [Inject]
    public void Construct(MoneyManager money)
    {
        moneyManager = money;
    }

    int GetCurrentRedCost()
    {
        return Mathf.RoundToInt(baseRedCost * Mathf.Pow(costMultiplier, currentLevel));
    }

    int GetCurrentYellowCost()
    {
        return Mathf.RoundToInt(baseYellowCost * Mathf.Pow(costMultiplier, currentLevel));
    }

    bool IsMaxLevel()
    {
        return currentLevel >= maxLevel;
    }

    void OnTriggerEnter(Collider other)
    {
        if (IsMaxLevel()) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        int redCost = GetCurrentRedCost();
        int yellowCost = GetCurrentYellowCost();

        if (moneyManager == null || 
            moneyManager.GetRedCurrency() < redCost || 
            moneyManager.GetYellowCurrency() < yellowCost)
        {
            return;
        }

        if (moneyManager.SpendCurrency(redCost, yellowCost))
        {
            ApplyUpgrade(player.gameObject);
            currentLevel++;
            onUpgrade?.Invoke();
        }
    }

    void ApplyUpgrade(GameObject playerObj)
    {
        switch (upgradeType)
        {
            case UpgradeType.CutterRadius:
                GrassCutter cutter = playerObj.GetComponentInChildren<GrassCutter>();
                if (cutter != null)
                {
                    cutter.UpgradeRadius(upgradeAmount);
                }
                break;

            case UpgradeType.InventoryCapacity:
                GrassInventory inventory = playerObj.GetComponent<GrassInventory>();
                if (inventory != null)
                {
                    inventory.maxFullStacks += Mathf.RoundToInt(upgradeAmount);
                }
                break;
        }
    }

    void OnGUI()
    {
        if (Camera.main == null) return;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 2f);
        
        if (screenPos.z > 0)
        {
            screenPos.y = Screen.height - screenPos.y;

            GUIStyle style = new GUIStyle(GUI.skin.box);
            style.fontSize = 14;
            style.normal.textColor = IsMaxLevel() ? Color.gray : Color.yellow;
            style.alignment = TextAnchor.MiddleCenter;

            string typeText = upgradeType == UpgradeType.CutterRadius ? "Cutter Radius" : "Inventory Capacity";
            string levelText = $"Level: {currentLevel}/{maxLevel}";
            string costText = IsMaxLevel() ? "MAX LEVEL" : $"Cost: R{GetCurrentRedCost()} Y{GetCurrentYellowCost()}";
            string amountText = $"+{upgradeAmount} per level";

            string fullText = $"{typeText}\n{levelText}\n{costText}\n{amountText}";
            
            Vector2 size = style.CalcSize(new GUIContent(fullText));
            GUI.Box(new Rect(screenPos.x - size.x * 0.5f, screenPos.y - size.y * 0.5f, size.x, size.y), fullText, style);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = IsMaxLevel() ? Color.gray : Color.yellow;
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
        }
    }
}
