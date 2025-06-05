using UnityEngine;

public class AutoPopulateProjectorTasks : MonoBehaviour
{
    public Transform canvasRoot;
    public ProjectorTaskManager projectorTaskManager;

    void Awake()
    {
        if (canvasRoot == null || projectorTaskManager == null)
        {
            Debug.LogWarning("[AutoPopulateProjectorTasks] Missing references.");
            return;
        }

        projectorTaskManager.taskList.Clear();

        foreach (Transform child in canvasRoot)
        {
            if (child.name.ToLower().Contains("header"))
                continue;

            Transform incomplete = child.Find("Initial X-Mark");
            Transform complete = child.Find("Checkmark");

            if (incomplete == null || complete == null)
            {
                Debug.LogWarning($"Skipping '{child.name}' — missing icons.");
                continue;
            }

            projectorTaskManager.taskList.Add(new ProjectorTaskManager.Task
            {
                taskName = child.name,
                incompleteIcon = incomplete.gameObject,
                completeIcon = complete.gameObject
            });
        }

        Debug.Log($"✅ Auto-populated {projectorTaskManager.taskList.Count} valid tasks.");
    }
}
