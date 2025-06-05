using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class PhoneScreenInteractionInstaller : MonoBehaviour
{
    [SerializeField] private GameObject phoneObject;
    [SerializeField] private float diagnoseWaitTime = 3f;

    [Header("Task System")]
    public ProjectorTaskManager projectorTaskManager;

    private bool hasStartedDiagnosis = false;

    private void Awake()
    {
        if (phoneObject == null)
        {
            Debug.LogWarning("[Installer] Phone object not assigned.");
            return;
        }

        // Optional: ensure the phoneObject has an invisible image for UI click detection
        Image img = phoneObject.GetComponent<Image>();
        if (img == null)
        {
            img = phoneObject.AddComponent<Image>();
            img.color = new Color(0, 0, 0, 0); // Fully transparent
        }
    }

    // Call this from a Unity Button or EventTrigger (e.g. when user taps or swipes screen)
    public void TriggerDiagnosis()
    {
        if (!hasStartedDiagnosis)
        {
            hasStartedDiagnosis = true;
            StartCoroutine(RunDiagnosisSequence());
        }
    }

    IEnumerator RunDiagnosisSequence()
    {
        Debug.Log("🧪 Starting unresponsive diagnosis...");

        yield return new WaitForSeconds(diagnoseWaitTime);

        // ✅ Mark projector step complete (Step 0 = Check Responsiveness)
        if (projectorTaskManager != null)
            projectorTaskManager.MarkTaskComplete(0);

        // ✅ Show the diagnosis popup
        if (projectorTaskManager != null)
            projectorTaskManager.ShowDiagnosePopout();
    }
}
