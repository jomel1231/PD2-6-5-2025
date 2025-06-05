using UnityEngine;

public class RespawnOnFall : MonoBehaviour
{
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 originalScale; // ✅ Add this
    private Rigidbody rb;

    [Header("Respawn Settings")]
    public string groundTag = "Floor"; // The tag of your ground
    public float delayBeforeRespawn = 0f;

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalScale = transform.localScale; // ✅ Capture original scale
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(groundTag))
        {
            Invoke(nameof(Respawn), delayBeforeRespawn);
        }
    }

    void Respawn()
    {
        // Stop momentum
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // ✅ Reset everything
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        transform.localScale = originalScale; // ✅ Restore scale

        Debug.Log($"{gameObject.name} respawned.");
    }
}
