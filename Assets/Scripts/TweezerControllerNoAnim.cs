using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class TweezerControllerNoAnim : MonoBehaviour
{
    [Header("Input")]
    public InputActionProperty triggerAction;  // Assign your 'TriggerForTweezer' action in the Inspector

    [Header("Tweezers Behavior")]
    [Tooltip("Scale factor when tweezers are 'closed'. 1.0 = normal scale, < 1.0 = narrower.")]
    public float closedScale = 0.7f;

    private XRGrabInteractable grabInteractable;
    private bool isHeld = false;

    // Screw references
    private Transform screwInRange = null;
    private bool isGrabbingScrew = false;

    void Start()
    {
        // Get XRGrabInteractable so we know when the tweezers are grabbed/dropped
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);

        // Disable the input action until the tweezers are actually held
        triggerAction.action.Disable();
    }

    void Update()
    {
        // Only respond to input if the tweezers are being held
        if (!isHeld) return;

        // Read the trigger's float value (0 to 1)
        float triggerValue = triggerAction.action.ReadValue<float>();

        // If trigger is pressed, "close" the tweezers
        if (triggerValue > 0.1f)
        {
            // Simulate closing by scaling the tweezers slightly
            transform.localScale = new Vector3(closedScale, 1f, 1f);

            // If we have a screw in range and haven't grabbed it yet, grab it
            if (!isGrabbingScrew && screwInRange != null)
            {
                GrabScrew(screwInRange);
            }
        }
        else
        {
            // Trigger not pressed => "open" the tweezers
            transform.localScale = Vector3.one;

            // If we're currently grabbing a screw, release it
            if (isGrabbingScrew && screwInRange != null)
            {
                ReleaseScrew();
            }
        }
    }

    private void GrabScrew(Transform screw)
    {
        isGrabbingScrew = true;

        // Make the screw a child of the tweezers
        screw.SetParent(transform);

        // Disable the screw's physics so it doesn't fall away
        Rigidbody rb = screw.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    private void ReleaseScrew()
    {
        isGrabbingScrew = false;

        // Return the screw to normal physics
        Rigidbody rb = screwInRange.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        // Unparent the screw so it can be dropped
        screwInRange.SetParent(null);
    }

    // Called when the tweezers are grabbed by the player
    private void OnGrab(SelectEnterEventArgs args)
    {
        isHeld = true;
        triggerAction.action.Enable();
    }

    // Called when the tweezers are released
    private void OnRelease(SelectExitEventArgs args)
    {
        isHeld = false;
        triggerAction.action.Disable();

        // Reset scale to open
        transform.localScale = Vector3.one;

        // If we were holding a screw, release it
        if (isGrabbingScrew && screwInRange != null)
        {
            ReleaseScrew();
        }
    }

    // Detect when a screw enters the tweezers' collider
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Screw"))
        {
            screwInRange = other.transform;
        }
    }

    // Detect when a screw leaves the tweezers' collider
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Screw") && screwInRange == other.transform)
        {
            screwInRange = null;
        }
    }
}
