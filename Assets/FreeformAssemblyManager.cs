using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FreeformAssemblyManager : MonoBehaviour
{
    public List<AssemblyStep> steps;
    public ProjectorTaskManager projectorTaskManager;

    private int currentCorrectStepIndex = 0;
    private HashSet<GameObject> lockedParts = new HashSet<GameObject>();
    private Dictionary<GameObject, bool> screwSpawnedForPart = new Dictionary<GameObject, bool>();

    private int mistakeCount = 0;
    public List<string> mistakeSteps = new List<string>();

    public int GetMistakeCount() => mistakeCount;
    public List<string> GetMistakeSteps() => mistakeSteps;
    public int TotalSteps => steps.Count;

    void Start()
    {
        foreach (var step in steps)
        {
            if (step.partToGrab != null)
            {
                step.partToGrab.enabled = true;
                screwSpawnedForPart[step.partToGrab.gameObject] = false;

                if (step.targetSocket == null)
                    step.partToGrab.selectExited.AddListener(OnPartUngrabbed);
            }

            foreach (var screw in step.screwsToEnable)
            {
                if (screw != null)
                {
                    screw.SetActive(false);
                    var screwScript = screw.GetComponent<ScrewUnscrew>();
                    if (screwScript != null)
                        screwScript.onScrewStateChanged.AddListener(() => OnScrewStateChanged(step));
                }
            }

            if (step.targetSocket != null)
            {
                step.targetSocket.selectEntered.AddListener(OnPartSocketed);
                step.targetSocket.selectExited.AddListener(OnPartRemoved);
            }
        }
    }

    void OnPartSocketed(SelectEnterEventArgs args)
    {
        for (int i = 0; i < steps.Count; i++)
        {
            AssemblyStep step = steps[i];
            if (args.interactableObject.transform == step.partToGrab.transform)
            {
                if (!screwSpawnedForPart[step.partToGrab.gameObject])
                {
                    foreach (var screw in step.screwsToEnable)
                        if (screw != null) screw.SetActive(true);
                    screwSpawnedForPart[step.partToGrab.gameObject] = true;
                }

                if (AreAllScrewsDone(step))
                {
                    if (i == currentCorrectStepIndex && IsPartInSocket(step))
                    {
                        LockPart(step.partToGrab);
                        projectorTaskManager?.MarkTaskComplete(currentCorrectStepIndex);
                        currentCorrectStepIndex++;

                        TryCompleteAssessment();
                    }
                    else
                    {
                        mistakeCount++;
                        if (!mistakeSteps.Contains(step.stepName))
                            mistakeSteps.Add(step.stepName);
                    }
                }
                break;
            }
        }
    }

    void OnPartRemoved(SelectExitEventArgs args)
    {
        if (lockedParts.Contains(args.interactableObject.transform.gameObject)) return;

        for (int i = 0; i < steps.Count; i++)
        {
            AssemblyStep step = steps[i];
            if (args.interactableObject.transform == step.partToGrab.transform)
            {
                foreach (var screw in step.screwsToEnable)
                {
                    if (screw != null)
                    {
                        screw.SetActive(false);
                        var screwScript = screw.GetComponent<ScrewUnscrew>();
                        if (screwScript != null)
                            screwScript.ResetScrew();
                    }
                }
                screwSpawnedForPart[step.partToGrab.gameObject] = false;
                break;
            }
        }
    }

    void OnScrewStateChanged(AssemblyStep step)
    {
        if (IsPartInSocket(step) && AreAllScrewsDone(step))
        {
            int stepIndex = steps.IndexOf(step);
            if (stepIndex == currentCorrectStepIndex)
            {
                LockPart(step.partToGrab);
                projectorTaskManager?.MarkTaskComplete(currentCorrectStepIndex);
                currentCorrectStepIndex++;

                TryCompleteAssessment();
            }
            else
            {
                mistakeCount++;
                if (!mistakeSteps.Contains(step.stepName))
                    mistakeSteps.Add(step.stepName);
            }
        }
    }

    void OnPartUngrabbed(SelectExitEventArgs args)
    {
        for (int i = 0; i < steps.Count; i++)
        {
            AssemblyStep step = steps[i];
            if (step.targetSocket == null && args.interactableObject.transform == step.partToGrab.transform)
            {
                if (i == currentCorrectStepIndex)
                {
                    LockPart(step.partToGrab);
                    projectorTaskManager?.MarkTaskComplete(currentCorrectStepIndex);
                    currentCorrectStepIndex++;

                    TryCompleteAssessment();
                }
                else
                {
                    mistakeCount++;
                    if (!mistakeSteps.Contains(step.stepName))
                        mistakeSteps.Add(step.stepName);
                }
                break;
            }
        }
    }

    bool IsPartInSocket(AssemblyStep step)
    {
        return step.targetSocket != null &&
               step.targetSocket.selectTarget != null &&
               step.targetSocket.selectTarget.transform == step.partToGrab.transform;
    }

    void LockPart(XRGrabInteractable part)
    {
        if (!lockedParts.Contains(part.gameObject))
            StartCoroutine(LockAfterFrame(part));
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
            var screwScript = screw.GetComponent<ScrewUnscrew>();
            if (screwScript != null && !screwScript.IsScrewedIn)
                return false;
        }
        return true;
    }

    void TryCompleteAssessment()
    {
        if (currentCorrectStepIndex >= steps.Count)
        {
            Debug.Log("✅ Assembly Finished!");
            AssessmentManager assessmentManager = FindObjectOfType<AssessmentManager>();
            if (assessmentManager != null)
                assessmentManager.ForcePass();
        }
    }

    public int GetCompletedSteps()
    {
        return currentCorrectStepIndex;
    }

    public bool IsAllStepsCompleted()
    {
        return currentCorrectStepIndex >= steps.Count;
    }
}
