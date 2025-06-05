using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DisassemblyStepManager_NoHighlight : MonoBehaviour
{
    [System.Serializable]
    public class DisassemblyStep
    {
        public string stepName;
        [TextArea] public string stepDescription;

        public XRGrabInteractable partToUnlock;
        public List<ScrewUnscrew> orderedScrews = new List<ScrewUnscrew>();
        public List<XRGrabInteractable> prerequisites = new List<XRGrabInteractable>();
    }

    [Header("Disassembly Steps")]
    public List<DisassemblyStep> steps = new List<DisassemblyStep>();

    [Header("Projector / Objective UI")]
    public ProjectorTaskManager projectorTaskManager;

    private int currentStepIndex = 0;
    private int currentScrewIndex = 0;
    private HashSet<XRGrabInteractable> grabbedPrerequisites = new();

    void Start()
    {
        foreach (var step in steps)
        {
            if (step.partToUnlock != null)
                step.partToUnlock.enabled = false;

            foreach (var prereq in step.prerequisites)
                prereq.enabled = false;

            foreach (var screw in step.orderedScrews)
            {
                if (screw != null)
                {
                    screw.isStepActive = false;
                    screw.gameObject.SetActive(true);
                }
            }
        }

        StartStep(currentStepIndex);
    }

    void StartStep(int index)
    {
        if (index >= steps.Count)
        {
            projectorTaskManager?.ShowStep("🎉 Disassembly Complete!", "All steps completed.");
            return;
        }

        currentScrewIndex = 0;
        grabbedPrerequisites.Clear();

        var step = steps[index];
        projectorTaskManager?.ShowStep(step.stepName, step.stepDescription);

        if (step.orderedScrews.Count > 0)
            ShowNextScrew(step);
        else if (step.prerequisites.Count > 0)
            EnablePrerequisites(step);
        else if (step.partToUnlock != null)
            PreparePart(step.partToUnlock);
        else
            AdvanceStep();
    }

    void ShowNextScrew(DisassemblyStep step)
    {
        if (currentScrewIndex >= step.orderedScrews.Count)
        {
            if (step.prerequisites.Count > 0)
                EnablePrerequisites(step);
            else if (step.partToUnlock != null)
                PreparePart(step.partToUnlock);
            else
                AdvanceStep();
            return;
        }

        ScrewUnscrew screw = step.orderedScrews[currentScrewIndex];
        if (screw == null)
        {
            currentScrewIndex++;
            ShowNextScrew(step);
            return;
        }

        foreach (var s in step.orderedScrews)
            if (s != null) s.isStepActive = false;

        screw.isStepActive = true;

        screw.onScrewCompleted += () =>
        {
            screw.isStepActive = false;
            screw.onScrewCompleted = null;
            StartCoroutine(HideScrewAfterAnimation(screw));

            currentScrewIndex++;
            ShowNextScrew(step);
        };
    }

    IEnumerator HideScrewAfterAnimation(ScrewUnscrew screw)
    {
        float rotationDuration = screw.numberOfTurns / screw.rotationSpeed;
        float slideDuration = Vector3.Distance(
            screw.screw.localPosition,
            screw.screw.localPosition + screw.slideOffset
        ) / screw.slideSpeed;

        float delay = Mathf.Max(rotationDuration, slideDuration) + 0.1f;
        yield return new WaitForSeconds(delay);

        screw.gameObject.SetActive(false);
    }

    void EnablePrerequisites(DisassemblyStep step)
    {
        foreach (var prereq in step.prerequisites)
        {
            if (prereq == null) continue;
            prereq.enabled = true;
            prereq.selectEntered.AddListener(OnPrerequisiteGrabbed);
        }
    }

    void OnPrerequisiteGrabbed(SelectEnterEventArgs args)
    {
        var interactor = args.interactableObject as XRGrabInteractable;
        if (interactor == null) return;

        interactor.selectEntered.RemoveListener(OnPrerequisiteGrabbed);
        grabbedPrerequisites.Add(interactor);

        var step = steps[currentStepIndex];
        if (grabbedPrerequisites.Count >= step.prerequisites.Count)
        {
            PreparePart(step.partToUnlock);
        }
    }

    void PreparePart(XRGrabInteractable part)
    {
        if (part == null) return;

        part.enabled = true;

        Rigidbody rb = part.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = false;
        }

        XRSocketInteractor socket = GetSocketHolding(part);
        if (socket != null)
            StartCoroutine(ForceReselect(socket, part));

        StartCoroutine(WaitForGrab(part));
    }

    IEnumerator WaitForGrab(XRGrabInteractable part)
    {
        while (!part.isSelected || part.selectingInteractor is XRSocketInteractor)
            yield return null;

        projectorTaskManager?.MarkTaskComplete(currentStepIndex);
        AdvanceStep();
    }

    void AdvanceStep()
    {
        currentStepIndex++;
        StartStep(currentStepIndex);
    }

    XRSocketInteractor GetSocketHolding(XRGrabInteractable grab)
    {
        foreach (var socket in FindObjectsOfType<XRSocketInteractor>())
            if (socket.hasSelection && socket.selectTarget == grab)
                return socket;
        return null;
    }

    IEnumerator ForceReselect(XRSocketInteractor socket, XRGrabInteractable part)
    {
        socket.interactionManager.SelectExit(socket, part);
        yield return null;
        socket.interactionManager.SelectEnter(socket, part);
    }

    // ✅ FOR AssessmentManager:
    public int GetCompletedSteps()
    {
        return currentStepIndex;
    }

    public bool IsAllStepsCompleted()
    {
        return currentStepIndex >= steps.Count;
    }
}
