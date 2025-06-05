using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class SuctionToolColliderControl : MonoBehaviour
{
    [Header("Trigger Control")]
    public Collider screenTrigger;
    public InputActionProperty triggerAction;

    [Header("Dependency")]
    public DashLineGroupManager dashLineManager; // 👈 Drag your DashLineGroupManager here

    private XRGrabInteractable grabInteractable;
    private bool isHeld = false;

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            grabInteractable.selectExited.AddListener(OnReleased);
        }

        triggerAction.action.performed += OnTriggerPressed;

        // Disable screenTrigger just in case
        if (screenTrigger != null)
            screenTrigger.enabled = false;
    }

    private void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }

        triggerAction.action.performed -= OnTriggerPressed;
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (!CanUseSuction()) return;

        isHeld = true;
        if (screenTrigger != null)
            screenTrigger.enabled = false;
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        isHeld = false;
        if (screenTrigger != null)
            screenTrigger.enabled = false;
    }

    private void OnTriggerPressed(InputAction.CallbackContext ctx)
    {
        if (!CanUseSuction()) return;

        if (isHeld && screenTrigger != null)
        {
            screenTrigger.enabled = true;
        }
    }

    private bool CanUseSuction()
    {
        return dashLineManager != null && dashLineManager.IsDashlineTaskDone();
    }
}
