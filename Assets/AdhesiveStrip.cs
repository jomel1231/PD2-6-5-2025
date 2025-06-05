using UnityEngine;

public class AdhesiveStrip : MonoBehaviour
{
    private bool isAttached = false;
    private bool wasGrabbed = false;
    private Transform attachPoint;
    private Rigidbody rb;
    private TweezerGrabController currentTweezer;

    public AdhesiveStripManager manager; // Assign in Inspector
    public ScreenUnlockManager screenUnlockManager; // 🔹 Assign in Inspector

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    void OnTriggerStay(Collider other)
    {
        if (isAttached) return;

        if (other.CompareTag("Tweezers"))
        {
            TweezerGrabController tweezer = other.GetComponent<TweezerGrabController>();
            if (tweezer == null) return;

            // 🔸 Block grabbing if screen is not yet triggered
            if (screenUnlockManager != null && !screenUnlockManager.IsScreenTriggered())
                return;

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

                if (manager != null)
                    manager.NotifyStripReleased(this);
            }

            currentTweezer = null;
        }
    }

    public bool WasGrabbed() => wasGrabbed;
}
