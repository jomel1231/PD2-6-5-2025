using System.Collections.Generic;
using UnityEngine;

public class VRHighlightManager : MonoBehaviour
{
    [Header("Components in Removal Order")]
    public List<GameObject> components;

    [Header("Highlight Settings")]
    public Color highlightColor = Color.yellow;
    public float emissionIntensity = 2f;

    private int currentIndex = 0;
    private Material currentMaterial;
    private Dictionary<GameObject, Material> originalMaterials = new Dictionary<GameObject, Material>();

    void Start()
    {
        CacheOriginalMaterials();
        HighlightCurrent();
    }

    void CacheOriginalMaterials()
    {
        foreach (GameObject component in components)
        {
            Renderer renderer = component.GetComponent<Renderer>();
            if (renderer != null)
            {
                originalMaterials[component] = renderer.material;
                renderer.material.DisableKeyword("_EMISSION");
            }
            else
            {
                Debug.LogError("Component missing Renderer: " + component.name);
            }
        }
    }

    void HighlightCurrent()
    {
        ClearAllHighlights();

        if (currentIndex >= components.Count)
        {
            Debug.Log("All components removed.");
            return;
        }

        GameObject current = components[currentIndex];
        Renderer rend = current.GetComponent<Renderer>();

        if (rend != null)
        {
            currentMaterial = rend.material;
            currentMaterial.EnableKeyword("_EMISSION");
            currentMaterial.SetColor("_EmissionColor", highlightColor * emissionIntensity);
        }
    }

    void ClearAllHighlights()
    {
        foreach (var entry in originalMaterials)
        {
            if (entry.Key != null && entry.Value != null)
            {
                Renderer rend = entry.Key.GetComponent<Renderer>();
                rend.material.DisableKeyword("_EMISSION");
                rend.material.SetColor("_EmissionColor", Color.black);
            }
        }
    }

    // Call this method from your grab/remove interaction
    public void OnComponentRemoved(GameObject component)
    {
        if (component == components[currentIndex])
        {
            currentIndex++;
            HighlightCurrent();
        }
        else
        {
            Debug.LogWarning("Incorrect component removed. Expected: " + components[currentIndex].name);
        }
    }

    public void ResetSequence()
    {
        currentIndex = 0;
        HighlightCurrent();
    }
}
