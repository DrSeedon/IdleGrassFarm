using UnityEngine;

public class CheatPanel : MonoBehaviour
{
    public GrassCutter cutter;
    public GrassInventory inventory;
    public PlayerController player;

    public float cutterRadiusBoost = 0.5f;
    public int inventoryBoost = 5;
    public float speedBoost = 1f;

    bool showPanel = true;

    void OnGUI()
    {
        if (!showPanel) return;

        float panelWidth = 200f;
        float panelHeight = 180f;
        float padding = 10f;
        
        Rect panelRect = new Rect(
            Screen.width - panelWidth - padding,
            Screen.height - panelHeight - padding,
            panelWidth,
            panelHeight
        );

        GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
        boxStyle.fontSize = 12;
        boxStyle.normal.textColor = Color.red;
        boxStyle.alignment = TextAnchor.UpperCenter;

        GUI.Box(panelRect, "CHEAT PANEL", boxStyle);

        GUILayout.BeginArea(new Rect(panelRect.x + 10, panelRect.y + 30, panelRect.width - 20, panelRect.height - 40));
        
        if (GUILayout.Button("+ Cutter Radius"))
        {
            if (cutter != null)
            {
                cutter.UpgradeRadius(cutterRadiusBoost);
            }
        }

        if (GUILayout.Button("+ Inventory Size"))
        {
            if (inventory != null)
            {
                inventory.maxFullStacks += inventoryBoost;
            }
        }

        if (GUILayout.Button("+ Speed"))
        {
            if (player != null)
            {
                var agent = player.GetComponent<UnityEngine.AI.NavMeshAgent>();
                if (agent != null)
                {
                    agent.speed += speedBoost;
                }
            }
        }

        if (GUILayout.Button("Hide Panel"))
        {
            showPanel = false;
        }

        GUILayout.EndArea();
    }
}
