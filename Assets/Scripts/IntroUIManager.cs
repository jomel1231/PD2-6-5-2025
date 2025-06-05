using UnityEngine;

public class IntroUIManager : MonoBehaviour
{
    [Header("Assign your intro UI panels in order (Start, then Intro, etc.)")]
    public GameObject[] panels;

    private int currentPanel = 0;

    void Start()
    {
        // Show only the first panel on start
        ShowOnlyPanel(currentPanel);
    }

    // Called by the Next button
    public void ShowNextPanel()
    {
        if (currentPanel < panels.Length - 1)
        {
            currentPanel++;
            ShowOnlyPanel(currentPanel);
        }
    }

    // Called by the Start button
    public void CloseAllPanels()
    {
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }
    }

    // Utility to show only one panel at a time
    private void ShowOnlyPanel(int index)
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(i == index);
        }
    }
}
