using UnityEngine;
using UnityEngine.InputSystem;

public class GlueGuidelineRemover : MonoBehaviour
{
    [Header("Guideline Object")]
    public GameObject guidelineObject;

    [Header("Reference to SocketStepManager")]
    public SocketStepManager stepManager;

    [Header("Trigger Input")]
    public InputActionProperty triggerAction;

    private bool hasTouched = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTouched) return;

        // Only trigger if user is pressing the glue tool's trigger
        float triggerValue = triggerAction.action.ReadValue<float>();
        if (triggerValue > 0.1f && other.CompareTag("Guideline"))
        {
            if (guidelineObject != null)
                guidelineObject.SetActive(false);

            if (stepManager != null)
                stepManager.MarkStep7FromGlue();

            hasTouched = true;
        }
    }

    private void OnEnable()
    {
        triggerAction.action.Enable();
    }

    private void OnDisable()
    {
        triggerAction.action.Disable();
    }
}
