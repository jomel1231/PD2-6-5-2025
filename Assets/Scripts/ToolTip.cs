using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ToolTip : MonoBehaviour
{
    public string message;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the interacting object has a DirectInteractor or RayInteractor
        if (other.GetComponent<XRDirectInteractor>() || other.GetComponent<XRRayInteractor>())
        {
            TooltipManager._instance.SetAndShowTooltip(message, transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<XRDirectInteractor>() || other.GetComponent<XRRayInteractor>())
        {
            TooltipManager._instance.HideTooltip();
        }
    }
}
