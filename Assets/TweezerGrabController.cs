using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class TweezerGrabController : MonoBehaviour
{
    [Header("Tweezer Behavior")]
    public Transform tweezerBody; // The object to scale (e.g. visual mesh)
    public Transform tipAttachPoint; // Where the adhesive strip should stick
    public Collider tipTriggerCollider; // The trigger collider at the tip

    [Header("Scale Settings")]
    public Vector3 normalScale = new Vector3(1, 1.5854f, 1); // Original Y scale
    public Vector3 pinchScale = new Vector3(1, 0.02f, 1);     // Pinched Y scale

    [Header("Input Settings")]
    public InputActionProperty triggerAction;

    private XRGrabInteractable grabInteractable;
    private int originalLayer = -1;

    private void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (tipTriggerCollider != null)
        {
            tipTriggerCollider.enabled = false;
            originalLayer = tipTriggerCollider.gameObject.layer;
        }

        triggerAction.action.Disable();

        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void Update()
    {
        if (!grabInteractable.isSelected) return;

        // When trigger is pressed → pinch + enable tip trigger
        if (triggerAction.action.WasPressedThisFrame())
        {
            if (tweezerBody != null)
                tweezerBody.localScale = pinchScale;

            if (tipTriggerCollider != null && !tipTriggerCollider.enabled)
            {
                tipTriggerCollider.enabled = true;
                tipTriggerCollider.gameObject.layer = originalLayer; // restore proper layer
            }
        }

        // When trigger is released → reset scale + disable tip trigger
        if (triggerAction.action.WasReleasedThisFrame())
        {
            if (tweezerBody != null)
                tweezerBody.localScale = normalScale;

            if (tipTriggerCollider != null && tipTriggerCollider.enabled)
            {
                tipTriggerCollider.enabled = false;
                tipTriggerCollider.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast"); // prevents interference
            }
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        triggerAction.action.Enable();

        // Restore proper trigger layer
        if (tipTriggerCollider != null)
            tipTriggerCollider.gameObject.layer = originalLayer;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        triggerAction.action.Disable();

        if (tweezerBody != null)
            tweezerBody.localScale = normalScale;

        if (tipTriggerCollider != null)
        {
            tipTriggerCollider.enabled = false;
            tipTriggerCollider.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }
    }

    // Called by AdhesiveStrip script
    public bool IsTriggerHeld()
    {
        return triggerAction.action.IsPressed();
    }

    // Called by AdhesiveStrip to get attach point
    public Transform GetAttachPoint()
    {
        return tipAttachPoint;
    }
}
