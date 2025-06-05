using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ProjectorTaskManager : MonoBehaviour
{
    [System.Serializable]
    public class Task
    {
        public string taskName;                   // Optional label for identifying the task
        public GameObject incompleteIcon;         // Red X icon or similar
        public GameObject completeIcon;           // Green check icon
        public GameObject taskObject;             // ✅ Link to the actual UI object (e.g., Remove Battery)

        [HideInInspector] public bool isComplete; // Tracks if task is complete
    }

    [Header("Task List")]
    public List<Task> taskList = new List<Task>();

    [Header("Step UI Elements")]
    public Text stepTitleText;
    public Text stepDescriptionText;

    [Header("Diagnosis UI")]
    public GameObject diagnosePanel;
    public Button popoutTriggerButton;
    public Button nextButton;

    [Header("Optional Manager Link")]
    public PhoneUIStateManager phoneUIStateManager;

    /// <summary>
    /// Marks a specified task as complete by updating its icons.
    /// </summary>
    public void MarkTaskComplete(int index)
    {
        if (index < 0 || index >= taskList.Count) return;

        Task task = taskList[index];
        if (task.isComplete) return;

        task.isComplete = true;
        if (task.incompleteIcon != null)
            task.incompleteIcon.SetActive(false);
        if (task.completeIcon != null)
            task.completeIcon.SetActive(true);
    }

    /// <summary>
    /// Marks a specified task as incomplete (e.g. reversed action).
    /// </summary>
    public void MarkTaskIncomplete(int index)
    {
        if (index < 0 || index >= taskList.Count) return;

        Task task = taskList[index];
        if (!task.isComplete) return;

        task.isComplete = false;
        if (task.incompleteIcon != null)
            task.incompleteIcon.SetActive(true);
        if (task.completeIcon != null)
            task.completeIcon.SetActive(false);
    }

    /// <summary>
    /// Resets all tasks to their initial (incomplete) state.
    /// </summary>
    public void ResetAllTasks()
    {
        foreach (var task in taskList)
        {
            task.isComplete = false;
            if (task.incompleteIcon != null)
                task.incompleteIcon.SetActive(true);
            if (task.completeIcon != null)
                task.completeIcon.SetActive(false);
        }
    }

    /// <summary>
    /// Displays the step title and description on the UI.
    /// </summary>
    public void ShowStep(string title, string description)
    {
        if (stepTitleText != null)
            stepTitleText.text = title;
        if (stepDescriptionText != null)
            stepDescriptionText.text = description;
    }

    /// <summary>
    /// Called when the original pop-out trigger button is clicked.
    /// </summary>
    public void OnPopoutButtonClicked()
    {
        if (popoutTriggerButton != null)
        {
            popoutTriggerButton.interactable = false;
            Debug.Log("Pop-out trigger button has been disabled.");
        }
        else
        {
            Debug.LogWarning("Pop-out trigger button not assigned.");
        }

        ShowDiagnosePopout();
    }

    /// <summary>
    /// Activates the diagnose panel and updates the UI.
    /// Also marks the diagnose task (assumed to be at index 0) as complete.
    /// </summary>
    public void ShowDiagnosePopout()
    {
        if (diagnosePanel != null)
        {
            diagnosePanel.SetActive(true);
            ShowStep("Diagnosis", "Screen unresponsive. Press 'Next' to continue.");
            MarkTaskComplete(0);
        }
        else
        {
            Debug.LogWarning("Diagnose panel not assigned.");
        }
    }

    /// <summary>
    /// Called when the 'Next' button inside the diagnose panel is clicked.
    /// </summary>
    public void OnDiagnoseNextButtonClicked()
    {
        if (nextButton != null)
        {
            nextButton.interactable = false;
        }
        if (diagnosePanel != null)
        {
            diagnosePanel.SetActive(false);
        }
    }

    /// <summary>
    /// Check if a task at a given index is complete.
    /// </summary>
    public bool IsTaskComplete(int index)
    {
        if (index < 0 || index >= taskList.Count)
        {
            Debug.LogWarning($"[ProjectorTaskManager] Tried to query invalid task index: {index}");
            return false;
        }
        return taskList[index].isComplete;
    }
}
