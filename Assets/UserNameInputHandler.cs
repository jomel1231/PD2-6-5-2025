using UnityEngine;
using TMPro;

public class UserNameInputHandler : MonoBehaviour
{
    [Header("References")]
    public TMP_InputField userNameInputField;
    public Microsoft.MixedReality.Toolkit.Experimental.UI.NonNativeKeyboard nonNativeKeyboard;
    public GameObject assessmentPanelToHide;
    public GameObject gameplayObjectsToEnable;
    public VidController vidController;

    private void Start()
    {
        if (userNameInputField != null)
        {
            userNameInputField.onSelect.AddListener(delegate { OnInputFieldSelected(); });
        }
    }

    private void OnInputFieldSelected()
    {
        if (nonNativeKeyboard != null && userNameInputField != null)
        {
            nonNativeKeyboard.InputField = userNameInputField;
            nonNativeKeyboard.PresentKeyboard(userNameInputField.text);
            Debug.Log("🔥 Keyboard opened and linked to InputField.");
        }
    }

    public void OnStartButtonClicked()
    {
        // Step 1: Check if user typed a name
        if (string.IsNullOrWhiteSpace(userNameInputField.text))
        {
            Debug.LogWarning("⚠️ Cannot start: Please type your name first!");
            return; // ❌ DO NOT PROCEED if empty
        }

        // Step 2: Save name to UserNameManager
        if (UserNameManager.Instance != null)
        {
            UserNameManager.Instance.SetUserName(userNameInputField.text);
            Debug.Log("✅ Saved User Name: " + userNameInputField.text);

            // Also update AssessmentManager's username
            AssessmentManager assessmentManager = FindObjectOfType<AssessmentManager>();
            if (assessmentManager != null)
            {
                assessmentManager.UpdateUserName();
                Debug.Log("✅ AssessmentManager updated with User Name.");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ UserNameManager not found!");
        }

        // Step 3: Hide Welcome Panel
        if (assessmentPanelToHide != null)
        {
            assessmentPanelToHide.SetActive(false);
            Debug.Log("✅ Welcome Panel hidden.");
        }

        // Step 4: Play Countdown Video
        if (vidController != null)
        {
            vidController.PlayCountdownVideo();
            Debug.Log("✅ Countdown Video started.");
        }
        else
        {
            Debug.LogWarning("⚠️ VidController is missing!");
        }

        // Step 5: Enable gameplay objects (optional)
        if (gameplayObjectsToEnable != null)
        {
            gameplayObjectsToEnable.SetActive(true);
            Debug.Log("✅ Gameplay Objects enabled.");
        }

        // Step 6: Close the virtual keyboard
        if (nonNativeKeyboard != null)
        {
            nonNativeKeyboard.gameObject.SetActive(false);
            Debug.Log("✅ Keyboard closed.");
        }
    }
}
