using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class AssessmentAssemblyManager : MonoBehaviour
{
    public List<AssemblyStep> steps;
    public ProjectorTaskManager projectorTaskManager;

    private int currentCorrectStepIndex = 0;
    private HashSet<GameObject> lockedParts = new HashSet<GameObject>();

    void Start()
    {
        foreach (var step in steps)
        {
            if (step.partToGrab != null)
            {
                step.partToGrab.enabled = true;
            }

            foreach (var screw in step.screwsToEnable)
            {
                if (screw != null)
                    screw.SetActive(true);
            }

            if (step.targetSocket != null)
            {
                step.targetSocket.selectEntered.AddListener(OnPartSocketed);
            }
        }
    }

    void OnPartSocketed(SelectEnterEventArgs args)
    {
        for (int i = 0; i < steps.Count; i++)
        {
            var step = steps[i];

            if (args.interactableObject.transform == step.partToGrab.transform)
            {
                if (AreAllScrewsDone(step))
                {
                    if (i == currentCorrectStepIndex)
                    {
                        LockPart(step.partToGrab);
                        projectorTaskManager?.MarkTaskComplete(currentCorrectStepIndex);
                        currentCorrectStepIndex++;
                    }
                    else
                    {
                        // Wrong order, but allow placement and screwing
                        Debug.Log("🔄 Part placed in wrong step, but allowed.");
                    }
                }
                break;
            }
        }
    }

    void LockPart(XRGrabInteractable part)
    {
        if (!lockedParts.Contains(part.gameObject))
        {
            StartCoroutine(LockAfterFrame(part));
        }
    }

    IEnumerator LockAfterFrame(XRGrabInteractable part)
    {
        yield return new WaitForSeconds(0.1f);

        part.enabled = false;
        var rb = part.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        lockedParts.Add(part.gameObject);
    }

    bool AreAllScrewsDone(AssemblyStep step)
    {
        foreach (var screw in step.screwsToEnable)
        {
            ScrewUnscrew screwScript = screw.GetComponent<ScrewUnscrew>();
            if (screwScript != null && !screwScript.IsScrewedIn)
                return false;
        }
        return true;
    }
}
