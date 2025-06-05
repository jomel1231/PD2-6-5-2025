using UnityEngine;

public class BootloopDiagnosisManager : MonoBehaviour
{
    [Header("Feedback Panels")]
    public GameObject correctPanel;        // Assign: Canvas > Correct
    public GameObject incorrectPanel;      // Assign: Canvas > Incorrect

    [Header("Diagnosis Popup Root")]
    public GameObject diagnosisPopup;      // Assign: Diagnosed Pop Up

    [Header("Bootloop Button Reference")]
    public BootloopButton bootloopButton;  // Assign: Your Power Button object

    private bool answered = false;

    // Called by answer buttons
    public void SelectAnswer(bool isCorrect)
    {
        if (answered) return; // Prevent multiple triggers
        answered = true;

        if (correctPanel != null)
            correctPanel.SetActive(isCorrect);

        if (incorrectPanel != null)
            incorrectPanel.SetActive(!isCorrect);
    }

    // Called by "Start Repair and Assembly" button
    public void OnContinueClicked()
    {
        // Hide popup
        if (diagnosisPopup != null)
            diagnosisPopup.SetActive(false);

        // Allow Task 2 in BootloopButton.cs
        if (bootloopButton != null)
            bootloopButton.allowTask2 = true;

        Debug.Log("Diagnosis complete — Task 2 unlocked.");
    }
}
