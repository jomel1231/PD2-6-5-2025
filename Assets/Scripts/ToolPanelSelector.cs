using UnityEngine;

public class ToolPanelSelector : MonoBehaviour
{
    [Header("Tools Menu Panel (with the 3 category buttons)")]
    public GameObject toolsMenuPanel;

    [Header("Tool Detail Panels (Tools - 1, Tools - 2, Tools - 3)")]
    public GameObject[] toolDetailPanels;

    private int currentIndex = 0;

    public void ShowPanel(int index)
    {
        if (index < 0 || index >= toolDetailPanels.Length) return;

        if (toolsMenuPanel != null)
            toolsMenuPanel.SetActive(false);

        for (int i = 0; i < toolDetailPanels.Length; i++)
        {
            toolDetailPanels[i].SetActive(i == index);
        }

        currentIndex = index;
    }

    public void BackToMenu()
    {
        foreach (GameObject panel in toolDetailPanels)
        {
            if (panel != null)
                panel.SetActive(false);
        }

        if (toolsMenuPanel != null)
            toolsMenuPanel.SetActive(true);
    }
}
