using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class AssemblySequenceManager : MonoBehaviour
{
    public List<AssemblyStep> steps;
    private int currentStepIndex = 0;
    private int currentScrewIndex = 0;

    public ProjectorTaskManager projectorTaskManager;

    [Header("Highlight Settings")]
    public Material highlightMaterial;
    private Material originalPartMaterial;
    private Material originalScrewMaterial;

    [Header("Timer Check (Optional)")]
    public AssemblyVideoTimerCheck timerCheck;

    [Header("Completion Popup (Optional)")]
    public GameObject congratsPopup;

    [Header("Incorrect Grab UI")]
    public GameObject incorrectGrabUI;
    private AudioSource warningAudio;

    void Start()
    {
        if (incorrectGrabUI != null)
        {
            incorrectGrabUI.SetActive(false); // Hide warning UI on start
            warningAudio = incorrectGrabUI.GetComponent<AudioSource>();

            if (warningAudio != null)
            {
                warningAudio.Stop();            // Ensure audio isn't playing
                warningAudio.playOnAwake = false; // Disable auto-play
            }
        }

        foreach (var step in steps)
        {
            if (step.partToGrab != null)
            {
                step.partToGrab.selectEntered.AddListener(OnAnyPartGrabbed);
                step.partToGrab.selectExited.AddListener(OnAnyPartReleased);
            }

            foreach (var screw in step.screwsToEnable)
            {
                if (screw != null)
                    screw.SetActive(false);
            }
        }

        EnableStep(currentStepIndex);
    }

    void EnableStep(int index)
    {
        if (index >= steps.Count)
        {
            Debug.Log("✅ All assembly steps completed.");
            projectorTaskManager?.ShowStep("Assembly Complete", "You've completed the repair!");

            if (timerCheck != null)
                timerCheck.OnProcessComplete();

            if (congratsPopup != null)
                congratsPopup.SetActive(true);

            return;
        }

        AssemblyStep step = steps[index];

        if (step.partToGrab != null)
        {
            var rend = step.partToGrab.GetComponent<Renderer>();
            if (rend != null)
            {
                originalPartMaterial = rend.material;
                rend.material = highlightMaterial;
            }

            StartCoroutine(ForceEnableNextFrame(step.partToGrab));

            if (step.targetSocket == null)
            {
                step.partToGrab.selectExited.AddListener(OnGrabReleased);
            }
        }

        if (step.targetSocket != null)
        {
            step.targetSocket.selectEntered.AddListener(OnPartSocketed);
        }
    }

    void OnPartSocketed(SelectEnterEventArgs args)
    {
        var step = steps[currentStepIndex];
        if (args.interactableObject.transform != step.partToGrab.transform)
            return;

        step.targetSocket.selectEntered.RemoveListener(OnPartSocketed);

        Renderer rend = step.partToGrab.GetComponent<Renderer>();
        if (rend != null && originalPartMaterial != null)
            rend.material = originalPartMaterial;

        StartCoroutine(LockSocketedObject(step.partToGrab));

        if (step.screwsToEnable == null || step.screwsToEnable.Length == 0)
        {
            projectorTaskManager?.MarkTaskComplete(currentStepIndex);
            currentStepIndex++;
            EnableStep(currentStepIndex);
        }
        else
        {
            currentScrewIndex = 0;
            ShowNextScrewInStep(step);
        }
    }

    void ShowNextScrewInStep(AssemblyStep step)
    {
        if (currentScrewIndex >= step.screwsToEnable.Length)
        {
            projectorTaskManager?.MarkTaskComplete(currentStepIndex);
            currentStepIndex++;
            EnableStep(currentStepIndex);
            return;
        }

        GameObject screwObj = step.screwsToEnable[currentScrewIndex];
        screwObj.SetActive(true);

        ScrewUnscrew screwScript = screwObj.GetComponent<ScrewUnscrew>();
        if (screwScript != null)
        {
            screwScript.isStepActive = true;
            screwScript.onScrewCompleted += () =>
            {
                screwScript.isStepActive = false;
                RestoreScrewMaterial(screwObj);
                screwScript.onScrewCompleted = null;
                currentScrewIndex++;
                ShowNextScrewInStep(step);
            };

            HighlightScrew(screwObj);
        }
    }

    void HighlightScrew(GameObject screw)
    {
        Renderer rend = screw.GetComponent<Renderer>();
        if (rend != null && highlightMaterial != null)
        {
            originalScrewMaterial = rend.material;
            rend.material = highlightMaterial;
        }
    }

    void RestoreScrewMaterial(GameObject screw)
    {
        Renderer rend = screw.GetComponent<Renderer>();
        if (rend != null && originalScrewMaterial != null)
        {
            rend.material = originalScrewMaterial;
        }
    }

    IEnumerator LockSocketedObject(XRGrabInteractable grab)
    {
        yield return new WaitForSeconds(0.1f);

        if (grab != null)
        {
            grab.enabled = false;

            var rb = grab.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }
        }
    }

    IEnumerator ForceEnableNextFrame(XRGrabInteractable part)
    {
        yield return null;

        part.enabled = false;
        yield return null;
        part.enabled = true;

        var rb = part.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    void OnGrabReleased(SelectExitEventArgs args)
    {
        var step = steps[currentStepIndex];
        if (args.interactableObject.transform != step.partToGrab.transform)
            return;

        step.partToGrab.selectExited.RemoveListener(OnGrabReleased);

        Renderer rend = step.partToGrab.GetComponent<Renderer>();
        if (rend != null && originalPartMaterial != null)
            rend.material = originalPartMaterial;

        projectorTaskManager?.MarkTaskComplete(currentStepIndex);
        currentStepIndex++;
        EnableStep(currentStepIndex);
    }

    void OnAnyPartGrabbed(SelectEnterEventArgs args)
    {
        Transform grabbed = args.interactableObject.transform;
        Transform correct = steps[currentStepIndex].partToGrab?.transform;

        if (grabbed != correct)
        {
            if (incorrectGrabUI != null && !incorrectGrabUI.activeSelf)
                incorrectGrabUI.SetActive(true);

            if (warningAudio != null && !warningAudio.isPlaying)
                warningAudio.Play();
        }
    }

    void OnAnyPartReleased(SelectExitEventArgs args)
    {
        if (incorrectGrabUI != null && incorrectGrabUI.activeSelf)
            incorrectGrabUI.SetActive(false);

        if (warningAudio != null && warningAudio.isPlaying)
            warningAudio.Stop();
    }
}
