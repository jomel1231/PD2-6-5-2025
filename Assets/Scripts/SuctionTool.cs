using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class SuctionTool : MonoBehaviour
{
    public InputActionProperty triggerAction;
    private XRGrabInteractable grabInteractable;

    private Animator screenAnimator;
    private bool hasPlayed = false;

    public int suctionCupTaskIndex = 0;
    public ProjectorTaskManager projectorTaskManager;

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);

        triggerAction.action.Disable();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Screen") && screenAnimator == null)
        {
            screenAnimator = other.GetComponent<Animator>();
        }
    }

    void Update()
    {
        if (!grabInteractable.isSelected) return;
        if (screenAnimator == null || hasPlayed) return;

        if (triggerAction.action.WasPressedThisFrame())
        {
            screenAnimator.SetTrigger("Play"); // Make sure the screen's Animator has a trigger named "Play"
            hasPlayed = true;

            if (projectorTaskManager != null)
            {
                projectorTaskManager.MarkTaskComplete(suctionCupTaskIndex);
            }
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        triggerAction.action.Enable();
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        triggerAction.action.Disable();
    }
}
