using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DashlineGroupManagerAssembly : MonoBehaviour
{
    public List<DashLineTriggerAssembly> dashLines = new List<DashLineTriggerAssembly>();
    public UnityEvent onAllDashesCleared;

    [Header("Projector Task Integration")]
    public ProjectorTaskManager projectorTaskManager;
    public int taskIndex = 0;

    [Header("Trigger Settings")]
    public string validTriggerTag = "Glue"; // Tag of glue's trigger collider

    private int clearedCount = 0;
    private bool taskMarked = false;

    void Start()
    {
        clearedCount = 0;
        taskMarked = false;

        foreach (var dash in dashLines)
        {
            dash.manager = this;
            dash.ResetDash();
        }
    }

    public void RegisterDashCleared(DashLineTriggerAssembly dash)
    {
        clearedCount++;
        if (clearedCount >= dashLines.Count)
        {
            if (!taskMarked && projectorTaskManager != null)
            {
                projectorTaskManager.MarkTaskComplete(taskIndex);
                taskMarked = true;
            }

            onAllDashesCleared?.Invoke();
        }
    }

    public bool IsDashlineTaskDone()
    {
        return taskMarked;
    }

    public bool IsValidTrigger(Collider other)
    {
        return other.CompareTag(validTriggerTag);
    }
}
