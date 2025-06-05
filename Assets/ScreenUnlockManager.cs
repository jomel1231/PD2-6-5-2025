using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;

public class ScreenUnlockManager : MonoBehaviour
{
    [Header("Screen References")]
    public XRGrabInteractable screenGrab;
    public Rigidbody screenRb;
    public Collider screenCollider;
    public Transform screenObject;
    public Animator screenAnimator;

    [Header("Cable References")]
    public List<Rigidbody> cableRigidbodies = new List<Rigidbody>();
    public List<Collider> cableColliders = new List<Collider>();
    public List<Collider> cableTriggerColliders = new List<Collider>();
    public List<Transform> cableObjects = new List<Transform>();
    public List<Animator> cableAnimators = new List<Animator>();

    [Header("Tool References")]
    public SpudgerToolColliderControl spudgerControl;

    [Header("Shield Dependency")]
    public MetalShieldGrabManager metalShieldManager; // ✅ Assigned in Inspector

    [Header("Projector Task Tracking")]
    public ProjectorTaskManager projectorTaskManager;
    public int screenTriggeredIndex = 0;
    public int screenFullyUnlockedIndex = 1;

    private bool screenDone = false;
    private bool cablesDone = false;
    private bool screenTriggeredMarked = false;
    private bool screenUnlockedMarked = false;

    void Start()
    {
        if (screenGrab != null)
            screenGrab.enabled = false;

        if (screenRb != null)
        {
            screenRb.isKinematic = true;
            screenRb.useGravity = false;
        }

        if (screenCollider != null)
            screenCollider.enabled = false;

        if (screenAnimator != null)
            screenAnimator.enabled = false;

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

        foreach (var triggerCol in cableTriggerColliders)
        {
            if (triggerCol != null)
                triggerCol.enabled = false;
        }

        foreach (var anim in cableAnimators)
        {
            if (anim != null)
                anim.enabled = false;
        }

        if (spudgerControl != null)
            spudgerControl.enabled = false;
    }

    public void MarkScreenDone()
    {
        screenDone = true;

        if (spudgerControl != null)
            spudgerControl.enabled = true;

        if (!screenTriggeredMarked && projectorTaskManager != null)
        {
            screenTriggeredMarked = true;
            projectorTaskManager.MarkTaskComplete(screenTriggeredIndex);
        }

        TryUnlock();
    }

    public void MarkCableDone()
    {
        // ✅ Don't allow cable to be marked until shields are done
        if (metalShieldManager != null && !metalShieldManager.AreShieldsCompleted())
            return;

        cablesDone = true;
        TryUnlock();
    }

    private void TryUnlock()
    {
        if (screenDone && cablesDone)
        {
            if (screenAnimator != null)
                screenAnimator.enabled = false;

            foreach (var anim in cableAnimators)
            {
                if (anim != null)
                    anim.enabled = false;
            }

            if (screenObject != null)
                screenObject.SetParent(transform, true);

            foreach (var cable in cableObjects)
            {
                if (cable != null)
                    cable.SetParent(transform, true);
            }

            if (screenRb != null)
            {
                screenRb.isKinematic = false;
                screenRb.useGravity = true;
                screenRb.constraints = RigidbodyConstraints.FreezeRotation;
            }

            if (screenCollider != null)
                screenCollider.enabled = true;

            if (screenGrab != null)
                screenGrab.enabled = true;

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

            if (!screenUnlockedMarked && projectorTaskManager != null)
            {
                screenUnlockedMarked = true;
                projectorTaskManager.MarkTaskComplete(screenFullyUnlockedIndex);
            }
        }
    }

    public bool IsScreenTriggered()
    {
        return screenDone;
    }
}
