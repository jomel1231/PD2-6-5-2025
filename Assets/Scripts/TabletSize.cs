using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MaintainScaleOnGrab : MonoBehaviour
{
    private Vector3 originalScale;
    private XRGrabInteractable grabInteractable;

    void Start()
    {
        originalScale = transform.localScale;
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Subscribe to grab and release events
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    void OnRelease(SelectExitEventArgs args)
    {
        transform.localScale = originalScale;
    }

    void OnDestroy()
    {
        // Unsubscribe from event to prevent memory leaks
        if (grabInteractable != null)
        {
            grabInteractable.selectExited.RemoveListener(OnRelease);
        }
    }
}
