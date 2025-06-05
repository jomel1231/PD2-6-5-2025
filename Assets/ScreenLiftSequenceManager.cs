using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class ScreenLiftSequenceManager : MonoBehaviour
{
    public Animator screenAnimator;
    public XRGrabInteractable screenInteractable;
    public Collider screenTriggerZone;
    public Collider cableTriggerZone;

    public InputActionProperty triggerAction; // For controller trigger
    public XRGrabInteractable suctionTool;
    public XRGrabInteractable spudgerTool;

    private bool suctionReady = false;
    private bool animationPlayed = false;
    private bool spudgerReady = false;

    void OnEnable()
    {
        triggerAction.action.performed += OnTriggerPressed;
    }

    void OnDisable()
    {
        triggerAction.action.performed -= OnTriggerPressed;
    }

    private void OnTriggerPressed(InputAction.CallbackContext ctx)
    {
        if (suctionReady && !animationPlayed)
        {
            screenAnimator.Play("ScreenLift");
            animationPlayed = true;
        }
        else if (spudgerReady && animationPlayed)
        {
            screenInteractable.enabled = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == screenTriggerZone && suctionTool.isSelected && !animationPlayed)
        {
            suctionReady = true;
        }

        if (other == cableTriggerZone && spudgerTool.isSelected && animationPlayed)
        {
            spudgerReady = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == screenTriggerZone) suctionReady = false;
        if (other == cableTriggerZone) spudgerReady = false;
    }
}
