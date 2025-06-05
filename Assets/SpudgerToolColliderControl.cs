using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;

public class SpudgerToolColliderControl : MonoBehaviour
{
    [Header("Cable Triggers")]
    public List<Collider> cableTriggers = new List<Collider>();

    [Header("Input Settings")]
    public InputActionProperty triggerAction;

    private XRGrabInteractable grabInteractable;
    private bool isHeld = false;

    void Start()
    {
        // Prevent this logic from running if script was disabled at start
        if (!enabled)
            return;

        grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            grabInteractable.selectExited.AddListener(OnReleased);
        }

        triggerAction.action.performed += OnTriggerPressed;

        SetCableTriggersEnabled(false);
    }

    private void OnEnable()
    {
        // Re-bind when script is enabled
        if (triggerAction != null)
            triggerAction.action.performed += OnTriggerPressed;
    }

    private void OnDisable()
    {
        if (triggerAction != null)
            triggerAction.action.performed -= OnTriggerPressed;

        SetCableTriggersEnabled(false);
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
        isHeld = true;
        SetCableTriggersEnabled(false);
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        isHeld = false;
        SetCableTriggersEnabled(false);
    }

    private void OnTriggerPressed(InputAction.CallbackContext ctx)
    {
        if (isHeld)
        {
            SetCableTriggersEnabled(true);
        }
    }

    private void SetCableTriggersEnabled(bool enabled)
    {
        foreach (var trigger in cableTriggers)
        {
            if (trigger != null)
                trigger.enabled = enabled;
        }
    }
}
