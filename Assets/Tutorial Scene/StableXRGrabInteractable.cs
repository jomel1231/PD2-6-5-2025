using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class StableXRGrabInteractable : XRGrabInteractable
{
    protected override void Awake()
    {
        base.Awake();

        // Set grab to be stable and immediate
        movementType = MovementType.Instantaneous;
        useDynamicAttach = false;
        matchAttachPosition = false;
        matchAttachRotation = false;
        smoothPosition = false;
        smoothRotation = false;
        smoothScale = false;
        throwOnDetach = false;
    }
}
