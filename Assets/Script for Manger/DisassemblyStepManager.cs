using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DisassemblyStepManager : MonoBehaviour
{
    [System.Serializable]
    public class DisassemblyStep
    {
        public string stepName;
        [TextArea] public string stepDescription;

        public XRGrabInteractable partToUnlock;
        public List<ScrewUnscrew> orderedScrews = new List<ScrewUnscrew>(); // Empty = grab-only step
        public List<XRGrabInteractable> prerequisites = new List<XRGrabInteractable>();
    }

    [Header("Disassembly Steps")]
    public List<DisassemblyStep> steps = new List<DisassemblyStep>();

    [Header("Highlight Settings")]
    public Material highlightMaterial;

    [Header("Projector / Objective UI")]
    public ProjectorTaskManager projectorTaskManager;

    [Header("Completion UI")]
    public GameObject completionPopup;

    [Header("Timer Check")]
    public AssemblyVideoTimerCheck timerCheck;

    private int currentStepIndex = 0;
    private int currentScrewIndex = 0;

    private Dictionary<ScrewUnscrew, Material[]> originalScrewMaterials = new();
    private Dictionary<XRGrabInteractable, Material[]> originalPartMaterials = new();
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
            projectorTaskManager?.ShowStep("🎉 Disassembly Complete!", "All steps completed!");

            if (timerCheck != null)
                timerCheck.OnProcessComplete();

            if (completionPopup != null)
                completionPopup.SetActive(true);
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
            HighlightAndPreparePart(step.partToUnlock);
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
                HighlightAndPreparePart(step.partToUnlock);
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

        Renderer rend = screw.GetComponent<Renderer>();
        if (rend != null)
        {
            originalScrewMaterials[screw] = rend.materials;
            ApplyHighlightToRenderer(rend);
        }

        screw.onScrewCompleted += () =>
        {
            screw.isStepActive = false;
            screw.onScrewCompleted = null;

            if (originalScrewMaterials.TryGetValue(screw, out var originalMats))
                rend.materials = originalMats;

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

            Renderer rend = prereq.GetComponent<Renderer>();
            if (rend != null)
            {
                originalPartMaterials[prereq] = rend.materials;
                ApplyHighlightToRenderer(rend);
            }

            prereq.enabled = true;
            prereq.selectEntered.AddListener(OnPrerequisiteGrabbed);
        }
    }

    void OnPrerequisiteGrabbed(SelectEnterEventArgs args)
    {
        var interactor = args.interactableObject as XRGrabInteractable;
        if (interactor == null) return;

        interactor.selectEntered.RemoveListener(OnPrerequisiteGrabbed);

        Renderer rend = interactor.GetComponent<Renderer>();
        if (rend != null && originalPartMaterials.TryGetValue(interactor, out var originalMats))
            rend.materials = originalMats;

        grabbedPrerequisites.Add(interactor);

        var step = steps[currentStepIndex];
        if (grabbedPrerequisites.Count >= step.prerequisites.Count)
        {
            HighlightAndPreparePart(step.partToUnlock);
        }
    }

    void HighlightAndPreparePart(XRGrabInteractable part)
    {
        if (part == null) return;

        Renderer rend = part.GetComponent<Renderer>();
        if (rend != null)
        {
            originalPartMaterials[part] = rend.materials;
            ApplyHighlightToRenderer(rend);
        }

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

        Renderer rend = part.GetComponent<Renderer>();
        if (rend != null && originalPartMaterials.TryGetValue(part, out var originalMats))
            rend.materials = originalMats;

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

    void ApplyHighlightToRenderer(Renderer rend)
    {
        if (rend == null || highlightMaterial == null) return;

        Material[] mats = new Material[rend.materials.Length];
        for (int i = 0; i < mats.Length; i++)
            mats[i] = highlightMaterial;

        rend.materials = mats;
    }
}
