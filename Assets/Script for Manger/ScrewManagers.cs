using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;
using System.Collections.Generic;

public class ScrewManagers : MonoBehaviour
{
    [Header("Screws Required To Unlock")]
    public List<GameObject> requiredScrews;

    private HashSet<GameObject> unscrewedScrews = new HashSet<GameObject>();

    [Header("Target Part To Unlock")]
    public XRGrabInteractable partToUnlock;

    [Header("Highlight Settings")]
    public Material highlightMaterial;
    private Material originalMaterial;

    private Renderer targetRenderer;

    void Start()
    {
        if (partToUnlock != null)
        {
            partToUnlock.enabled = false;

            // Save original material
            targetRenderer = partToUnlock.GetComponent<Renderer>();
            if (targetRenderer != null)
            {
                originalMaterial = targetRenderer.material;
            }

            // Subscribe to grab event for resetting highlight
            partToUnlock.selectEntered.AddListener(OnPartGrabbed);
        }
    }

    public void NotifyScrewUnscrewed(GameObject screw)
    {
        if (!requiredScrews.Contains(screw))
            return;

        unscrewedScrews.Add(screw);

        // ✅ Start coroutine to disable screw after unscrewing duration
        StartCoroutine(HideScrewAfterUnscrew(screw));

        if (unscrewedScrews.Count >= requiredScrews.Count)
        {
            if (partToUnlock != null)
            {
                partToUnlock.enabled = true;

                // Highlight it
                if (targetRenderer != null && highlightMaterial != null)
                {
                    targetRenderer.material = highlightMaterial;
                }
            }
        }
    }

    private IEnumerator HideScrewAfterUnscrew(GameObject screw)
    {
        var unscrewScript = screw.GetComponent<ScrewUnscrew>();
        if (unscrewScript == null)
            yield break;

        // Calculate delay based on number of turns and rotation speed
        float rotationDuration = unscrewScript.numberOfTurns / unscrewScript.rotationSpeed;
        float slideDuration = Vector3.Distance(
            unscrewScript.screw.localPosition,
            unscrewScript.screw.localPosition + unscrewScript.slideOffset
        ) / unscrewScript.slideSpeed;

        float totalDelay = Mathf.Max(rotationDuration, slideDuration) + 0.1f;

        yield return new WaitForSeconds(totalDelay);

        screw.SetActive(false); // 🔥 Hide the screw after animation finishes
    }

    private void OnPartGrabbed(SelectEnterEventArgs args)
    {
        if (targetRenderer != null && originalMaterial != null)
        {
            targetRenderer.material = originalMaterial;
        }

        partToUnlock.selectEntered.RemoveListener(OnPartGrabbed);
    }

    private void OnDestroy()
    {
        if (partToUnlock != null)
        {
            partToUnlock.selectEntered.RemoveListener(OnPartGrabbed);
        }
    }
}
