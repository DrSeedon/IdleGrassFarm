using UnityEngine;
using Zenject;
using System.Linq;

public class PlayerStatsUI : MonoBehaviour
{
    public GrassInventory inventory;
    public GrassCutter cutter;
    
    MoneyManager moneyManager;

    [Inject]
    public void Construct(MoneyManager money)
    {
        moneyManager = money;
    }

    void OnGUI()
    {
        GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
        boxStyle.fontSize = 14;
        boxStyle.alignment = TextAnchor.UpperLeft;
        boxStyle.padding = new RectOffset(10, 10, 10, 10);
        boxStyle.normal.textColor = Color.white;

        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.fontSize = 14;
        labelStyle.normal.textColor = Color.white;

        string statsText = BuildStatsText();
        
        Vector2 size = boxStyle.CalcSize(new GUIContent(statsText));
        size.x += 20;
        size.y += 20;

        GUI.Box(new Rect(10, Screen.height - size.y - 10, size.x, size.y), statsText, boxStyle);
    }

    string BuildStatsText()
    {
        string text = "";

        if (moneyManager != null)
        {
            text += $"Red Currency: {moneyManager.GetRedCurrency()}\n";
            text += $"Yellow Currency: {moneyManager.GetYellowCurrency()}\n\n";
        }

        if (cutter != null)
        {
            text += $"Cutter Radius: {cutter.cutRadius:F1}\n";
        }

        if (inventory != null)
        {
            text += $"Max Inventory: {inventory.maxFullStacks}\n\n";

            text += "--- INVENTORY ---\n";

            var allStacks = inventory.GetAllStacks();
            var fullStacks = allStacks.Where(s => s.IsFull).ToList();
            var partialStacks = allStacks.Where(s => !s.IsFull).ToList();

            text += $"Full stacks: {fullStacks.Count}/{inventory.maxFullStacks}\n";

            if (fullStacks.Count > 0)
            {
                var grouped = fullStacks.GroupBy(s => s.type);
                foreach (var group in grouped)
                {
                    text += $"  {group.Key}: {group.Count()} stacks\n";
                }
            }

            if (partialStacks.Count > 0)
            {
                text += "\nPartial stacks:\n";
                foreach (var stack in partialStacks)
                {
                    text += $"  {stack.type}: {stack.count}/{stack.maxCount}\n";
                }
            }
        }

        return text;
    }
}
