using UnityEngine;

public class RingPulseAnimation : MonoBehaviour
{
    [Header("Pulse Settings")]
    public float pulseSpeed = 2f;         // How fast the pulse animation happens
    public float shrinkFactor = 0.9f;      // How small it shrinks (1 = no shrink, 0.9 = 90% of size)
    
    private Vector3 originalScale;
    private float pulseTimer = 0f;

    void Start()
    {
        // Save the starting size
        originalScale = transform.localScale;
    }

    void Update()
    {
        // Make a pulsing value using sine wave
        pulseTimer += Time.deltaTime * pulseSpeed;
        float scaleMultiplier = Mathf.Lerp(shrinkFactor, 1f, (Mathf.Sin(pulseTimer) + 1f) / 2f);

        // Apply the scale
        transform.localScale = originalScale * scaleMultiplier;
    }
}
