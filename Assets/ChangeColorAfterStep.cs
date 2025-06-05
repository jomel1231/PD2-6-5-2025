using UnityEngine;

public class ChangeColorAfterStep : MonoBehaviour
{
    [Header("Projector Task Manager")]
    public ProjectorTaskManager projectorTaskManager;

    [Header("Task Index to Watch (e.g., 0 for Step 1)")]
    public int taskIndexToWatch = 0;

    [Header("Target Objects (2 objects with MeshRenderer)")]
    public GameObject object1;
    public GameObject object2;

    [Header("New Color After Step Complete")]
    public Color newColorForObject1 = Color.green;
    public Color newColorForObject2 = Color.green;

    private bool hasChanged = false;

    void Update()
    {
        if (hasChanged || projectorTaskManager == null) return;

        if (projectorTaskManager.IsTaskComplete(taskIndexToWatch))
        {
            ApplyNewColor(object1, newColorForObject1);
            ApplyNewColor(object2, newColorForObject2);
            hasChanged = true;
        }
    }

    void ApplyNewColor(GameObject obj, Color color)
    {
        if (obj == null) return;

        MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            foreach (var mat in meshRenderer.materials)
                mat.color = color;
        }
    }
}
