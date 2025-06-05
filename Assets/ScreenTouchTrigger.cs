using UnityEngine;
using UnityEngine.UI;

public class ScreenTouchTrigger : MonoBehaviour
{
    [Header("Animator Control")]
    public Animator screenAnimator;
    public string animationTriggerName = "ActivateScreen";

    [Header("Step Manager Reference")]
    public SocketStepManager stepManager;

    [Tooltip("Steps that must be completed before animation is allowed")]
    public int[] requiredStepIndices;

    [Header("Optional UI Elements")]
    public Button uiButtonToEnable;

    [Header("Optional Object to Enable")]
    public GameObject objectToActivate;

    private bool visualsActivated = false;

    void Start()
    {
        // Start with UI and object disabled
        if (uiButtonToEnable != null)
        {
            uiButtonToEnable.interactable = false;
            uiButtonToEnable.onClick.AddListener(TriggerAnimation);
        }

        if (objectToActivate != null)
            objectToActivate.SetActive(false);
    }

    void Update()
    {
        if (!visualsActivated && AllStepsComplete())
        {
            if (uiButtonToEnable != null)
                uiButtonToEnable.interactable = true;

            if (objectToActivate != null)
                objectToActivate.SetActive(true);

            visualsActivated = true;
        }
    }

    public void TriggerAnimation()
    {
        if (screenAnimator != null)
        {
            screenAnimator.SetTrigger(animationTriggerName);
        }
    }

    private bool AllStepsComplete()
    {
        foreach (int index in requiredStepIndices)
        {
            if (!stepManager.IsStepDone(index))
                return false;
        }
        return true;
    }
}
