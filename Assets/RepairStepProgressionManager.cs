using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;
using System.Collections.Generic;

public class RepairStepProgressionManager : MonoBehaviour
{
    [System.Serializable]
    public class Step
    {
        public string stepName;
        public List<GameObject> highlightTargets;
        public int linkedTaskIndex = -1;

        [HideInInspector] public bool initialized = false;
        [HideInInspector] public Dictionary<Renderer, Material[]> originalMats = new Dictionary<Renderer, Material[]>();
        [HideInInspector] public Dictionary<GameObject, bool> grabbed = new Dictionary<GameObject, bool>();
    }

    [Header("Steps")]
    public List<Step> steps = new List<Step>();
    public Color highlightColor = Color.yellow;

    [Header("Highlight Material Template")]
    public Material highlightMaterialTemplate;

    [Header("Projector Task Manager")]
    public ProjectorTaskManager projectorTaskManager;

    [Header("UI & Audio Trigger (When All Steps Complete)")]
    public GameObject completionUI;
    public AudioSource completionSound;
    public Button finalActionButton; // 🔹 New field for button to enable on completion

    private int currentStepIndex = 0;
    private Material baseHighlightMaterial;
    private bool completionHandled = false;

    void Start()
    {
        baseHighlightMaterial = new Material(highlightMaterialTemplate);
        baseHighlightMaterial.EnableKeyword("_EMISSION");
        baseHighlightMaterial.SetColor("_EmissionColor", highlightColor);

        InitializeSteps();
        ApplyHighlightToCurrentStep();

        if (completionUI != null)
            completionUI.SetActive(false);

        if (finalActionButton != null)
            finalActionButton.interactable = false; // 🔹 Button disabled by default
    }

    void Update()
    {
        if (currentStepIndex >= steps.Count)
        {
            if (!completionHandled)
            {
                HandleCompletion();
            }
            return;
        }

        Step currentStep = steps[currentStepIndex];

        if (currentStep.linkedTaskIndex >= 0 &&
            projectorTaskManager != null &&
            projectorTaskManager.IsTaskComplete(currentStep.linkedTaskIndex))
        {
            MarkStepComplete();
        }
    }

    void InitializeSteps()
    {
        foreach (var step in steps)
        {
            if (step.initialized) continue;

            step.originalMats.Clear();
            step.grabbed.Clear();

            foreach (var obj in step.highlightTargets)
            {
                if (obj == null) continue;

                Renderer rend = obj.GetComponent<Renderer>();
                if (rend == null) rend = obj.GetComponentInChildren<Renderer>();
                if (rend == null) continue;

                step.originalMats[rend] = rend.materials;
                step.grabbed[obj] = false;

                XRGrabInteractable grab = obj.GetComponent<XRGrabInteractable>();
                if (grab != null)
                {
                    grab.selectEntered.AddListener((args) => OnObjectGrabbed(obj));
                }
            }

            step.initialized = true;
        }
    }

    void ApplyHighlightToCurrentStep()
    {
        if (currentStepIndex >= steps.Count) return;

        Step step = steps[currentStepIndex];
        foreach (var obj in step.highlightTargets)
        {
            Renderer rend = obj.GetComponent<Renderer>();
            if (rend == null) rend = obj.GetComponentInChildren<Renderer>();
            if (rend == null) continue;

            Material[] highlightMats = new Material[rend.materials.Length];
            for (int i = 0; i < highlightMats.Length; i++)
            {
                Material mat = new Material(baseHighlightMaterial);
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", highlightColor);
                highlightMats[i] = mat;
            }

            rend.materials = highlightMats;
        }
    }

    void RevertHighlight(Step step)
    {
        foreach (var entry in step.originalMats)
        {
            if (entry.Key != null)
                entry.Key.materials = entry.Value;
        }
    }

    public void OnObjectGrabbed(GameObject obj)
    {
        if (currentStepIndex >= steps.Count) return;

        Step step = steps[currentStepIndex];
        if (!step.grabbed.ContainsKey(obj) || step.grabbed[obj]) return;

        step.grabbed[obj] = true;

        Renderer rend = obj.GetComponent<Renderer>();
        if (rend == null) rend = obj.GetComponentInChildren<Renderer>();
        if (rend == null || !step.originalMats.ContainsKey(rend)) return;

        rend.materials = step.originalMats[rend];
    }

    public void MarkStepComplete()
    {
        if (currentStepIndex >= steps.Count) return;

        RevertHighlight(steps[currentStepIndex]);
        currentStepIndex++;

        if (currentStepIndex < steps.Count)
        {
            ApplyHighlightToCurrentStep();
        }
        else
        {
            Debug.Log("✅ All repair steps completed.");
            HandleCompletion();
        }
    }

    void HandleCompletion()
    {
        completionHandled = true;

        if (completionUI != null)
            completionUI.SetActive(true);

        if (completionSound != null)
            completionSound.Play();

        if (finalActionButton != null)
            finalActionButton.interactable = true; // 🔹 Enable the button now
    }

    public string GetCurrentStepName()
    {
        return currentStepIndex < steps.Count ? steps[currentStepIndex].stepName : "Completed";
    }
}
