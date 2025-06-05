using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class GlueActivator : MonoBehaviour
{
    [Header("Input Action")]
    public InputActionProperty triggerAction;

    [Header("Audio")]
    public AudioSource glueSound;

    [Header("Glue Trigger Collider (child object)")]
    public Collider glueTriggerCollider; // Assign the trigger collider in the inspector

    [Header("Required Tag for Trigger")]
    public string glueTag = "Glue"; // Must match DashlineGroupManagerAssembly.validTriggerTag

    private XRGrabInteractable glueInteractable;
    private bool isHeld = false;

    void Start()
    {
        glueInteractable = GetComponent<XRGrabInteractable>();
        if (glueInteractable != null)
        {
            glueInteractable.selectEntered.AddListener(OnGrab);
            glueInteractable.selectExited.AddListener(OnRelease);
        }

        if (glueTriggerCollider != null)
        {
            glueTriggerCollider.enabled = false;
            glueTriggerCollider.isTrigger = true;
            glueTriggerCollider.gameObject.tag = glueTag; // Ensure correct tag
        }

        triggerAction.action.Enable();
    }

    void Update()
    {
        if (!isHeld || glueTriggerCollider == null)
            return;

        float triggerValue = triggerAction.action.ReadValue<float>();

        if (triggerValue > 0.1f)
        {
            if (!glueTriggerCollider.enabled)
                glueTriggerCollider.enabled = true;

            if (!glueSound.isPlaying && glueSound != null)
                glueSound.Play();
        }
        else
        {
            if (glueTriggerCollider.enabled)
                glueTriggerCollider.enabled = false;

            if (glueSound != null && glueSound.isPlaying)
                glueSound.Stop();
        }
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        isHeld = true;
    }

    void OnRelease(SelectExitEventArgs args)
    {
        isHeld = false;

        if (glueTriggerCollider != null)
            glueTriggerCollider.enabled = false;

        if (glueSound != null && glueSound.isPlaying)
            glueSound.Stop();
    }

    void OnDestroy()
    {
        if (glueInteractable != null)
        {
            glueInteractable.selectEntered.RemoveListener(OnGrab);
            glueInteractable.selectExited.RemoveListener(OnRelease);
        }

        triggerAction.action.Disable();
    }
}
