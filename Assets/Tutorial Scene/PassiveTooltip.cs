using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PassiveTooltip : MonoBehaviour
{
    public GameObject tooltipUI; // Assign your tooltip UI object (can be world space canvas or 3D text)

    private void Awake()
    {
        if (tooltipUI != null)
            tooltipUI.SetActive(false); // Hide tooltip initially
    }

    private void OnEnable()
    {
        var interactable = GetComponent<XRBaseInteractable>();
        if (interactable != null)
        {
            interactable.hoverEntered.AddListener(OnHoverEnter);
            interactable.hoverExited.AddListener(OnHoverExit);
        }
    }

    private void OnDisable()
    {
        var interactable = GetComponent<XRBaseInteractable>();
        if (interactable != null)
        {
            interactable.hoverEntered.RemoveListener(OnHoverEnter);
            interactable.hoverExited.RemoveListener(OnHoverExit);
        }
    }

    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        if (tooltipUI != null)
            tooltipUI.SetActive(true);
    }

    private void OnHoverExit(HoverExitEventArgs args)
    {
        if (tooltipUI != null)
            tooltipUI.SetActive(false);
    }
}
