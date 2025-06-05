using UnityEngine;
using System.Collections.Generic;

public class StepObjectHighlighter : MonoBehaviour
{
    [Header("Projector Manager")]
    public ProjectorTaskManager projectorTaskManager;

    [Header("Highlight Color")]
    public Color highlightColor = Color.yellow;

    [Header("Step 1 Objects (3)")]
    public GameObject[] step1Objects = new GameObject[3];

    [Header("Step 2 Object")]
    public GameObject step2Object;

    [Header("Step 3 Objects (2)")]
    public GameObject[] step3Objects = new GameObject[2];

    [Header("Step 4 Object")]
    public GameObject step4Object;

    [Header("Step 5 Object")]
    public GameObject step5Object;

    private Dictionary<Renderer, Color[]> originalColors = new Dictionary<Renderer, Color[]>();
    private Dictionary<Renderer, Material[]> originalMats = new Dictionary<Renderer, Material[]>();

    void Start()
    {
        CacheOriginalColors(step1Objects);
        CacheOriginalColors(new GameObject[] { step2Object });
        CacheOriginalColors(step3Objects);
        CacheOriginalColors(new GameObject[] { step4Object });
        CacheOriginalColors(new GameObject[] { step5Object });
    }

    void Update()
    {
        int currentStep = GetCurrentStepIndex();

        HandleStepHighlight(step1Objects, 0, currentStep);
        HandleStepHighlight(new GameObject[] { step2Object }, 1, currentStep);
        HandleStepHighlight(step3Objects, 2, currentStep);
        HandleStepHighlight(new GameObject[] { step4Object }, 3, currentStep);
        HandleStepHighlight(new GameObject[] { step5Object }, 4, currentStep);
    }

    int GetCurrentStepIndex()
    {
        if (projectorTaskManager == null || projectorTaskManager.taskList == null)
            return -1;

        for (int i = 0; i < projectorTaskManager.taskList.Count; i++)
        {
            if (!projectorTaskManager.taskList[i].isComplete)
                return i;
        }

        return -1; // All steps complete
    }

    void CacheOriginalColors(GameObject[] objects)
    {
        if (objects == null) return;

        foreach (var obj in objects)
        {
            if (obj == null) continue;

            Renderer rend = obj.GetComponent<Renderer>();
            if (rend == null || originalColors.ContainsKey(rend)) continue;

            Material[] matsCopy = rend.materials;
            if (matsCopy == null) continue;

            rend.materials = matsCopy; // Ensure instance
            Color[] baseColors = new Color[matsCopy.Length];
            for (int i = 0; i < baseColors.Length; i++)
                baseColors[i] = matsCopy[i].color;

            originalColors[rend] = baseColors;
            originalMats[rend] = matsCopy;
        }
    }

    void HandleStepHighlight(GameObject[] targets, int taskIndex, int currentStep)
    {
        if (targets == null) return;

        foreach (var obj in targets)
        {
            if (obj == null) continue;

            Renderer rend = obj.GetComponent<Renderer>();
            if (rend == null) continue;

            if (taskIndex == currentStep)
            {
                foreach (var mat in rend.materials)
                    mat.color = highlightColor;
            }
            else
            {
                if (originalColors.TryGetValue(rend, out var baseColors))
                {
                    for (int i = 0; i < rend.materials.Length; i++)
                        rend.materials[i].color = baseColors[i];
                }
            }
        }
    }
}
