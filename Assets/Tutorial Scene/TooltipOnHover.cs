using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class TooltipOnHoverManual : MonoBehaviour
{
    [SerializeField] private Canvas tooltipCanvas;

    private XRGrabInteractable grabInteractable;

    private void OnEnable()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (tooltipCanvas != null)
            tooltipCanvas.gameObject.SetActive(false); // Initially hide tooltip

        // Register listeners for hover and select events
        grabInteractable.hoverEntered.AddListener(ShowTooltip);
        grabInteractable.hoverExited.AddListener(HideTooltip);
        grabInteractable.selectEntered.AddListener(HideTooltip); // Hide tooltip on selection (grabbing)
    }

    private void OnDisable()
    {
        // Unregister listeners to avoid memory leaks
        grabInteractable.hoverEntered.RemoveListener(ShowTooltip);
        grabInteractable.hoverExited.RemoveListener(HideTooltip);
        grabInteractable.selectEntered.RemoveListener(HideTooltip);
    }

    // Show tooltip when hovered over
    private void ShowTooltip(HoverEnterEventArgs args)
    {
        if (tooltipCanvas != null)
            tooltipCanvas.gameObject.SetActive(true); // Show tooltip when hovered
    }

    // Hide tooltip when hover is exited or object is grabbed
    private void HideTooltip(BaseInteractionEventArgs args)
    {
        if (tooltipCanvas != null)
            tooltipCanvas.gameObject.SetActive(false); // Hide tooltip
    }
}
