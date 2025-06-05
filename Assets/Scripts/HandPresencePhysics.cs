using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPresencePhysics : MonoBehaviour
{
    public Transform target; // The VR controller or hand tracking target
    private Rigidbody rb;

    // Rotation adjustment in case of wrong hand orientation
    private Quaternion rotationOffset = Quaternion.Euler(0, 0, -90);

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate; // Smooth movement
    
    }

    void FixedUpdate()
    {
        // Position Update: Move Rigidbody towards target smoothly
        rb.velocity = (target.position - transform.position) / Time.fixedDeltaTime;

        // Correct rotation using offset if needed
        Quaternion correctedTargetRotation = target.rotation * rotationOffset;

        // Calculate rotation difference
        Quaternion rotationDifference = correctedTargetRotation * Quaternion.Inverse(transform.rotation);
        rotationDifference.ToAngleAxis(out float angleInDegree, out Vector3 rotationAxis);

        if (angleInDegree > 180) angleInDegree -= 360; // Normalize the angle

        // Convert to angular velocity
        Vector3 targetAngularVelocity = (angleInDegree * rotationAxis) * Mathf.Deg2Rad / Time.fixedDeltaTime;

        // Smooth transition to prevent laggy rotations
        rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, targetAngularVelocity, 0.5f);

        // Apply final smooth rotation
        rb.MoveRotation(correctedTargetRotation);
    }
}
