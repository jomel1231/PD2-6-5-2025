using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class ProgressTracker : MonoBehaviour
{
    [Header("Time Limit (in seconds)")]
    public float timeLimit = 300f;

    [Header("Primary Projector Task Manager")]
    public ProjectorTaskManager projectorTaskManager1;

    [Header("Secondary Projector Task Manager")]
    public ProjectorTaskManager projectorTaskManager2;

    [Header("UI Panels")]
    public GameObject completionUIPanel;
    public GameObject passPanel;
    public GameObject failPanel;

    [Header("Result Text Elements")]
    public TextMeshProUGUI timeUsedText;
    public TextMeshProUGUI tasksCompletedText;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI scoreText;

    [Header("Countdown Texts (hh:mm)")]
    public TextMeshProUGUI countdownText1;
    public TextMeshProUGUI countdownText2;

    [Header("UI Button to Enable When Primary Tasks Done")]
    public Button primaryTasksDoneButton;

    private float timer = 0f;
    private bool resultShown = false;

    void Start()
    {
        timer = 0f;
        resultShown = false;
        enabled = false; // Only runs when StartAssessmentTimer() is called

        if (completionUIPanel) completionUIPanel.SetActive(false);
        if (passPanel) passPanel.SetActive(false);
        if (failPanel) failPanel.SetActive(false);

        if (countdownText1) countdownText1.gameObject.SetActive(false);
        if (countdownText2) countdownText2.gameObject.SetActive(false);

        if (primaryTasksDoneButton) primaryTasksDoneButton.interactable = false;
    }

    void Update()
    {
        if (resultShown || projectorTaskManager1 == null || projectorTaskManager2 == null) return;

        timer += Time.deltaTime;

        // Update countdown timers
        float remaining = Mathf.Clamp(timeLimit - timer, 0f, timeLimit);
        int minutes = Mathf.FloorToInt(remaining / 60f);
        int seconds = Mathf.FloorToInt(remaining % 60f);
        string formattedTime = $"{minutes:00}:{seconds:00}";

        if (countdownText1)
        {
            countdownText1.gameObject.SetActive(true);
            countdownText1.text = formattedTime;
        }

        if (countdownText2)
        {
            countdownText2.gameObject.SetActive(true);
            countdownText2.text = formattedTime;
        }

        // Count completed tasks
        int completedTasks = 0;
        foreach (var task in projectorTaskManager1.taskList)
        {
            if (task.isComplete) completedTasks++;
        }
        foreach (var task in projectorTaskManager2.taskList)
        {
            if (task.isComplete) completedTasks++;
        }

        int totalTasks = projectorTaskManager1.taskList.Count + projectorTaskManager2.taskList.Count;

        // Enable button only when primary projector's tasks are all complete
        if (primaryTasksDoneButton != null && !primaryTasksDoneButton.interactable)
        {
            bool allPrimaryDone = true;
            foreach (var task in projectorTaskManager1.taskList)
            {
                if (!task.isComplete)
                {
                    allPrimaryDone = false;
                    break;
                }
            }

            if (allPrimaryDone)
                primaryTasksDoneButton.interactable = true;
        }

        // Show result if completed or time expired
        if (completedTasks == totalTasks)
        {
            ShowResultPanel(true, completedTasks, totalTasks);
        }

        if (timer >= timeLimit)
        {
            ShowResultPanel(false, completedTasks, totalTasks);
        }
    }

    public void StartAssessmentTimer()
    {
        timer = 0f;
        resultShown = false;
        enabled = true;

        if (countdownText1) countdownText1.gameObject.SetActive(true);
        if (countdownText2) countdownText2.gameObject.SetActive(true);
    }

    private void ShowResultPanel(bool passed, int completedTasks, int totalTasks)
    {
        resultShown = true;
        enabled = false;

        if (completionUIPanel) completionUIPanel.SetActive(true);
        if (passPanel) passPanel.SetActive(passed);
        if (failPanel) failPanel.SetActive(!passed);

        int usedMin = Mathf.FloorToInt(timer / 60);
        int usedSec = Mathf.FloorToInt(timer % 60);
        if (timeUsedText) timeUsedText.text = $"Time Used: {usedMin:00}:{usedSec:00}";

        if (passed)
        {
            if (tasksCompletedText) tasksCompletedText.text = $"{completedTasks}";
        }
        else
        {
            string incompleteText = "";

            List<int> p1Missing = new List<int>();
            List<int> p2Missing = new List<int>();

            for (int i = 0; i < projectorTaskManager1.taskList.Count; i++)
            {
                if (!projectorTaskManager1.taskList[i].isComplete)
                    p1Missing.Add(i + 1);
            }

            for (int i = 0; i < projectorTaskManager2.taskList.Count; i++)
            {
                if (!projectorTaskManager2.taskList[i].isComplete)
                    p2Missing.Add(i + 1);
            }

            if (p1Missing.Count > 0)
                incompleteText += $"P1 - #{string.Join(", #", p1Missing)}";

            if (p2Missing.Count > 0)
            {
                if (!string.IsNullOrEmpty(incompleteText)) incompleteText += "\n";
                incompleteText += $"P2 - #{string.Join(", #", p2Missing)}";
            }

            if (tasksCompletedText) tasksCompletedText.text = incompleteText;
        }

        if (statusText) statusText.text = passed ? "PASSED" : "FAILED";
        if (scoreText) scoreText.text = $"{completedTasks}/{totalTasks}";

        for (int i = 0; i < completedTasks && i < projectorTaskManager2.taskList.Count; i++)
        {
            projectorTaskManager2.MarkTaskComplete(i);
        }

        if (countdownText1) countdownText1.gameObject.SetActive(false);
        if (countdownText2) countdownText2.gameObject.SetActive(false);
    }
}
