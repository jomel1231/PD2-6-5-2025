using UnityEngine;
using UnityEngine.UI;

public class InvisibleButtonTrigger : MonoBehaviour
{
    [SerializeField] private ProjectorTaskManager projectorTaskManager;

    // This function will be called when the invisible button is clicked
    public void OnInvisibleButtonClicked()
    {
        if (projectorTaskManager != null)
        {
            // This shows the diagnose pop-out and marks the first step as complete
            projectorTaskManager.ShowDiagnosePopout();
        }
        else
        {
            Debug.LogWarning("ProjectorTaskManager is not assigned in the inspector.");
        }
    }
}
