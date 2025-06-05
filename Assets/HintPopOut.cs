using UnityEngine;
using UnityEngine.UI;

public class HintPopup : MonoBehaviour
{
    [Header("UI References")]
    // The hint panel or label that will pop out
    public GameObject hintPanel;
    // The button that opens the hint
    public Button hintButton;
    // The button that closes the hint
    public Button closeButton;

    private void Start()
    {
        // Ensure the hint panel is hidden at the start
        if (hintPanel != null)
            hintPanel.SetActive(false);

        // Add event listeners to the buttons, if they're assigned
        if (hintButton != null)
            hintButton.onClick.AddListener(ShowHint);

        if (closeButton != null)
            closeButton.onClick.AddListener(HideHint);
    }

    // Show the hint panel
    private void ShowHint()
    {
        if (hintPanel != null)
            hintPanel.SetActive(true);
    }

    // Hide the hint panel
    private void HideHint()
    {
        if (hintPanel != null)
            hintPanel.SetActive(false);
    }
}
