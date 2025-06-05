using UnityEngine;

public class BatteryDiagnosisManager : MonoBehaviour
{
    public GameObject correctPanel;
    public GameObject incorrectPanel;
    public GameObject diagnosisPopup;
    public XRPressableButtonWithUI pressableButton;

    private bool answered = false;

    public void SelectAnswer(bool isCorrect)
    {
        if (answered) return;
        answered = true;

        if (correctPanel != null)
            correctPanel.SetActive(isCorrect);

        if (incorrectPanel != null)
            incorrectPanel.SetActive(!isCorrect);
    }

    public void OnContinueClicked()
    {
        if (diagnosisPopup != null)
            diagnosisPopup.SetActive(false);

        if (pressableButton != null)
            pressableButton.allowNextStep = true;

        Debug.Log("Battery diagnosis completed — next step is now unlocked.");
    }
}
