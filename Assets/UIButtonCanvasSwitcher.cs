using UnityEngine;
using UnityEngine.UI;

public class UIButtonCanvasSwitcher : MonoBehaviour
{
    [Header("Current Canvas to Hide")]
    public GameObject currentCanvas;

    [Header("Target Canvas to Enable")]
    public GameObject targetCanvas;

    [Header("Button")]
    public Button uiButton;

    void Start()
    {
        if (uiButton != null)
        {
            uiButton.onClick.AddListener(SwitchCanvas);
        }
        else
        {
            Debug.LogWarning("UIButtonCanvasSwitcher: No UI Button assigned!");
        }

        if (targetCanvas != null)
        {
            targetCanvas.SetActive(false); // Optional: ensure it's hidden at start
        }
    }

    void SwitchCanvas()
    {
        if (currentCanvas != null)
            currentCanvas.SetActive(false);

        if (targetCanvas != null)
            targetCanvas.SetActive(true);
    }
}
