using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LastSelectUIManager : MonoBehaviour
{
    // Reference to the additional UI Panel that should pop out after the last selection is released.
    [SerializeField]
    private GameObject additionalUIPanel;

    // New reference to the projector GameObject (assigned via Inspector)
    [SerializeField]
    private GameObject projectorGameObject;

    private XRBaseInteractable interactable;

    private void Awake()
    {
        interactable = GetComponent<XRBaseInteractable>();
        if (interactable == null)
        {
            Debug.LogError("XRBaseInteractable component not found on this GameObject.");
        }
    }

    private void OnEnable()
    {
        if (interactable != null)
        {
            interactable.selectExited.AddListener(OnSelectExited);
        }
    }

    private void OnDisable()
    {
        if (interactable != null)
        {
            interactable.selectExited.RemoveListener(OnSelectExited);
        }
    }

    // This callback fires whenever an interactor deselects the object.
    private void OnSelectExited(SelectExitEventArgs args)
    {
        // Check if no interactors remain selecting the object.
        if (interactable.interactorsSelecting.Count == 0)
        {
            Debug.Log("Last select exit detected.");
            PopOutAdditionalUI();
        }
    }

    // Activates the additional UI panel and references the projector game object.
    private void PopOutAdditionalUI()
    {
        if (additionalUIPanel != null)
        {
            additionalUIPanel.SetActive(true);
            Debug.Log("Additional UI panel activated.");

            // Optionally, use the projector game object reference.
            if (projectorGameObject != null)
            {
                Debug.Log("Projector GameObject is referenced: " + projectorGameObject.name);
                // Example: You might highlight the projector object or trigger a specific function on it.
                // projectorGameObject.GetComponent<YourProjectorScript>()?.DoSomething();
            }
            else
            {
                Debug.LogWarning("Projector GameObject has not been assigned in the Inspector.");
            }
        }
        else
        {
            Debug.LogWarning("Additional UI panel not assigned in the Inspector.");
        }
    }
}
