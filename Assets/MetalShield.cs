using UnityEngine;

public class MetalShield : MonoBehaviour
{
    private bool isAttached = false;
    private bool wasGrabbed = false;
    private Transform attachPoint;
    private Rigidbody rb;
    private TweezerGrabController currentTweezer;

    public MetalShieldGrabManager manager;
    public ScrewGroupManager screwGroupManager; // 👈 Add this

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    void OnTriggerStay(Collider other)
    {
        if (isAttached) return;
        if (screwGroupManager == null || !screwGroupManager.IsTriPointGroupDone()) return; // 👈 Block early

        if (other.CompareTag("Tweezers"))
        {
            TweezerGrabController tweezer = other.GetComponent<TweezerGrabController>();
            if (tweezer == null) return;

            if (tweezer.IsTriggerHeld())
            {
                attachPoint = tweezer.GetAttachPoint();
                transform.SetParent(attachPoint);
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;

                rb.isKinematic = true;
                rb.useGravity = false;

                isAttached = true;
                currentTweezer = tweezer;
            }
        }
    }

    void Update()
    {
        if (isAttached && currentTweezer != null && !currentTweezer.IsTriggerHeld())
        {
            transform.SetParent(null);
            rb.isKinematic = false;
            rb.useGravity = true;

            isAttached = false;

            if (!wasGrabbed)
            {
                wasGrabbed = true;
                manager?.NotifyShieldReleased(this);
            }

            currentTweezer = null;
        }
    }

    public bool WasGrabbed() => wasGrabbed;
}
