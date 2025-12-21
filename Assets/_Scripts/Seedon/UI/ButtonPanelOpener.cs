using UnityEngine;
using UnityEngine.UI;
using Seedon;

public class ButtonPanelOpener : MonoBehaviour
{
    public UIPanel panel;
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => panel?.Show());
        }
    }
}