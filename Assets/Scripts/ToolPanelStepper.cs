using UnityEngine;

public class ToolPanelStepper : MonoBehaviour
{
    [Header("Assign your UI panels in order (Welcome → About → Tools → etc.)")]
    public GameObject[] toolPanels;

    private int currentIndex = 0;

    void Start()
    {
        HideAllPanels();
        ShowOnlyCurrent();
    }

    public void ShowNextPanel()
    {
        if (currentIndex < toolPanels.Length - 1)
        {
            currentIndex++;
            ShowOnlyCurrent();
        }
    }

    public void ShowPreviousPanel()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            ShowOnlyCurrent();
        }
    }

    private void ShowOnlyCurrent()
    {
        for (int i = 0; i < toolPanels.Length; i++)
        {
            toolPanels[i].SetActive(i == currentIndex);
        }
    }

    private void HideAllPanels()
    {
        foreach (GameObject panel in toolPanels)
        {
            panel.SetActive(false);
        }
    }
}
