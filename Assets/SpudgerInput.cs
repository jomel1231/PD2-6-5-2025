using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class SpudgerInput : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    public bool IsTriggerPressed()
    {
        if (grabInteractable != null && grabInteractable.isSelected)
        {
            if (grabInteractable.selectingInteractor is XRBaseControllerInteractor controllerInteractor)
            {
                // ✅ Cast xrController to XRController to access inputDevice
                if (controllerInteractor.xrController is XRController xrCtrl)
                {
                    InputDevice device = xrCtrl.inputDevice;
                    if (device.TryGetFeatureValue(CommonUsages.triggerButton, out bool isPressed))
                    {
                        return isPressed;
                    }
                }
            }
        }

        return false;
    }
}
