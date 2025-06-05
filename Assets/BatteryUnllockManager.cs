using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;

public class BatteryUnlockManager : MonoBehaviour
{
    [Header("Adhesive Dependency")]
    public AdhesiveStripManager adhesiveManager;

    [Header("Battery References")]
    public XRGrabInteractable batteryGrab;
    public Rigidbody batteryRb;
    public Collider batteryCollider;

    [Header("Cable References")]
    public List<Rigidbody> cableRigidbodies = new List<Rigidbody>();
    public List<Collider> cableColliders = new List<Collider>();
    public List<Transform> cableObjects = new List<Transform>();
    public List<Animator> cableAnimators = new List<Animator>();

    [Header("Projector Task Tracking")]
    public ProjectorTaskManager projectorTaskManager;
    public int cableTaskIndex = 0;

    private bool cablesDone = false;
    private bool batteryUnlocked = false;

    public bool IsCableDone => cablesDone;

    void Start()
    {
        if (batteryGrab != null) batteryGrab.enabled = false;

        if (batteryRb != null)
        {
            batteryRb.isKinematic = true;
            batteryRb.useGravity = false;
        }

        if (batteryCollider != null)
            batteryCollider.enabled = false;

        foreach (var rb in cableRigidbodies)
        {
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }
        }

        foreach (var col in cableColliders)
        {
            if (col != null)
                col.enabled = false;
        }

        foreach (var anim in cableAnimators)
        {
            if (anim != null)
                anim.enabled = false;
        }
    }

    public void MarkCableDone()
    {
        if (cablesDone) return;

        cablesDone = true;

        // ✅ Mark cable task complete
        if (projectorTaskManager != null)
            projectorTaskManager.MarkTaskComplete(cableTaskIndex);

        foreach (var anim in cableAnimators)
        {
            if (anim != null)
                anim.enabled = false;
        }

        foreach (var cable in cableObjects)
        {
            if (cable != null && batteryRb != null)
                cable.SetParent(batteryRb.transform, true);
        }

        foreach (var rb in cableRigidbodies)
        {
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
                rb.constraints = RigidbodyConstraints.FreezeRotation;
            }
        }

        foreach (var col in cableColliders)
        {
            if (col != null)
                col.enabled = true;
        }

        TryUnlockBattery();
    }

    private void TryUnlockBattery()
    {
        if (!cablesDone || adhesiveManager == null || !adhesiveManager.IsAdhesiveTaskDone)
            return;

        if (batteryUnlocked) return;

        batteryUnlocked = true;

        if (batteryGrab != null) batteryGrab.enabled = true;
        if (batteryCollider != null) batteryCollider.enabled = true;
        if (batteryRb != null)
        {
            batteryRb.isKinematic = false;
            batteryRb.useGravity = true;
            batteryRb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    public void CheckIfCanUnlockBattery()
    {
        TryUnlockBattery();
    }
}
