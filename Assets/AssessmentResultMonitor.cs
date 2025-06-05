using UnityEngine;
using TMPro;

public class AssessmentResultMonitor : MonoBehaviour
{
    [Header("Time Limit (seconds)")]
    public float totalTimeAllowed = 600f; // 10 minutes

    [Header("Projector Task Managers")]
    public ProjectorTaskManager disassemblyManager;
    public ProjectorTaskManager assemblyManager;

    [Header("UI Panels")]
    public GameObject passPanel;
    public GameObject failPanel;

    [Header("Text Elements (TextMeshProUGUI)")]
    public TextMeshProUGUI timeUsedText;
    public TextMeshProUGUI tasksCompletedText;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI finalScoreText;

    private float timer = 0f;
    private bool resultShown = false;

    void Start()
    {
        if (passPanel) passPanel.SetActive(false);
        if (failPanel) failPanel.SetActive(false);
    }

    void Update()
    {
        if (resultShown || disassemblyManager == null || assemblyManager == null) return;

        timer += Time.deltaTime;

        int completedTasks = 0;
        int totalTasks = 0;

        // Count disassembly tasks
        totalTasks += disassemblyManager.taskList.Count;
        foreach (var task in disassemblyManager.taskList)
            if (task.isComplete) completedTasks++;

        // Count assembly tasks
        totalTasks += assemblyManager.taskList.Count;
        foreach (var task in assemblyManager.taskList)
            if (task.isComplete) completedTasks++;

        // ✅ Finish immediately if all tasks are done
        if (completedTasks == totalTasks)
        {
            ShowResult(true, completedTasks, totalTasks);
        }
        // ❌ Fail if time runs out before finishing
        else if (timer >= totalTimeAllowed)
        {
            ShowResult(false, completedTasks, totalTasks);
        }
    }

    void ShowResult(bool passed, int completed, int total)
    {
        resultShown = true;

        string timeFormatted = $"{Mathf.FloorToInt(timer / 60)}m {Mathf.FloorToInt(timer % 60)}s";
        string scorePercent = $"{(completed * 100 / total)}%";

        // Activate the appropriate panel
        if (passed && passPanel != null)
            passPanel.SetActive(true);
        else if (!passed && failPanel != null)
            failPanel.SetActive(true);

        // Update all text fields
        if (timeUsedText) timeUsedText.text = $"Time Used: {timeFormatted}";
        if (tasksCompletedText) tasksCompletedText.text = $"Tasks Completed: {completed}/{total}";
        if (resultText) resultText.text = $"Result: {(passed ? "Passed" : "Failed")}";
        if (finalScoreText) finalScoreText.text = $"Final Score: {scorePercent}";
    }
    public void ForceImmediateResult()
    {
        if (resultShown) return;

        int completedTasks = 0;
        int totalTasks = 0;

        totalTasks += disassemblyManager.taskList.Count;
        foreach (var task in disassemblyManager.taskList)
            if (task.isComplete) completedTasks++;

        totalTasks += assemblyManager.taskList.Count;
        foreach (var task in assemblyManager.taskList)
            if (task.isComplete) completedTasks++;

        ShowResult(true, completedTasks, totalTasks);
    }

}
