using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using static UnityEngine.XR.OpenXR.Features.Interactions.HTCViveControllerProfile;

public class AssessmentManager : MonoBehaviour
{
    [Header("Timer Settings")]
    public float totalAssessmentTime = 900f; // 15 minutes = 900 seconds
    private float currentTime;

    [Header("Task Managers")]
    public DisassemblyStepManager_NoHighlight disassemblyManager;
    public FreeformAssemblyManager assemblyManager;

    [Header("Video Timer (Optional)")]
    public AssemblyVideoTimerCheck assemblyVideoTimerCheck;

    [Header("UI Elements")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI taskProgressText;
    public GameObject passPanel;
    public GameObject failPanel;

    [Header("Pass/Fail Panel Texts")]
    public TextMeshProUGUI passTimeUsedText;
    public TextMeshProUGUI passTasksCompletedText;
    public TextMeshProUGUI passResultText;
    public TextMeshProUGUI passFinalScoreText;

    public TextMeshProUGUI failTimeUsedText;
    public TextMeshProUGUI failTasksCompletedText;
    public TextMeshProUGUI failResultText;
    public TextMeshProUGUI failFinalScoreText;

    [Header("Google Sheet Upload")]
    [SerializeField] private string webAppUrl = "https://script.google.com/macros/s/AKfycby15WeoxODMV-zMujUPy-TbT_ZrjF5yRnbQxnXaMOHdQLpEQww5RRS2jhdhaTdzd-5g/exec";

    private bool assessmentEnded = false;
    private string userName = "Anonymous"; // Default, will be updated

    private string mode = "Assessment"; // ✅ Added this

    void Start()
    {
        currentTime = totalAssessmentTime;

        if (UserNameManager.Instance != null)
        {
            userName = UserNameManager.Instance.GetUserName();
        }
        else
        {
            userName = "Anonymous";
        }

        if (passPanel != null) passPanel.SetActive(false);
        if (failPanel != null) failPanel.SetActive(false);
    }

    void Update()
    {
        if (assessmentEnded) return;

        currentTime -= Time.deltaTime;
        UpdateUI();

        if (currentTime <= 0)
        {
            currentTime = 0;
            EndAssessment(false); // Time ran out → Fail immediately
        }
    }

    void UpdateUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60f);
            int seconds = Mathf.FloorToInt(currentTime % 60f);
            timerText.text = $"Time Left: {minutes:00}:{seconds:00}";
        }

        if (taskProgressText != null)
        {
            int totalSteps = disassemblyManager.steps.Count + assemblyManager.steps.Count;
            int completedSteps = disassemblyManager.GetCompletedSteps() + assemblyManager.GetCompletedSteps();
            taskProgressText.text = $"Tasks: {completedSteps}/{totalSteps}";
        }
    }

    bool AllTasksCompleted()
    {
        return disassemblyManager.IsAllStepsCompleted() && assemblyManager.IsAllStepsCompleted();
    }

    void EndAssessment(bool passed)
    {
        assessmentEnded = true;

        int totalSteps = disassemblyManager.steps.Count + assemblyManager.steps.Count;
        int completedSteps = disassemblyManager.GetCompletedSteps() + assemblyManager.GetCompletedSteps();
        int score = passed ? 100 : 0;
        int mistakes = totalSteps - completedSteps;
        float timeUsed = totalAssessmentTime - currentTime;

        int usedMinutes = Mathf.FloorToInt(timeUsed / 60f);
        int usedSeconds = Mathf.FloorToInt(timeUsed % 60f);
        string timeUsedFormatted = $"{usedMinutes:00}:{usedSeconds:00}";

        VidController vidController = FindObjectOfType<VidController>();
        if (vidController != null)
        {
            vidController.StopCountdownVideo();
        }

        if (assemblyVideoTimerCheck != null)
        {
            if (passed)
                assemblyVideoTimerCheck.OnProcessComplete();
            else
                assemblyVideoTimerCheck.OnProcessFail();
        }

        if (passed)
        {
            if (passPanel != null)
            {
                passPanel.SetActive(true);

                if (passTimeUsedText != null) passTimeUsedText.text = $"{timeUsedFormatted}";
                if (passTasksCompletedText != null) passTasksCompletedText.text = $"{completedSteps}/{totalSteps}";
                if (passResultText != null) passResultText.text = $"PASS";
                if (passFinalScoreText != null) passFinalScoreText.text = $"100%";
            }
        }
        else
        {
            if (failPanel != null)
            {
                failPanel.SetActive(true);

                if (failTimeUsedText != null) failTimeUsedText.text = $"{timeUsedFormatted}";
                if (failTasksCompletedText != null) failTasksCompletedText.text = $"{completedSteps}/{totalSteps}";
                if (failResultText != null) failResultText.text = $"FAIL";
                if (failFinalScoreText != null) failFinalScoreText.text = $"0%";
            }
        }

        UploadAssessment(userName, score, Mathf.RoundToInt(timeUsed), mistakes, passed ? "Pass" : "Fail");
    }

    public void ForcePass()
    {
        if (assessmentEnded) return;

        EndAssessment(true); // Manually end the assessment as PASS
    }

    void UploadAssessment(string userName, int score, int timeUsed, int mistakes, string result)
    {
        StartCoroutine(Upload(userName, score, timeUsed, mistakes, result));
    }

    IEnumerator Upload(string userName, int score, int timeUsed, int mistakes, string result)
    {
        AssessmentData data = new AssessmentData
        {
            userName = userName,
            score = score,
            timeUsed = timeUsed,
            mistakes = mistakes,
            result = result,
            mode = mode // ✅ Add mode field!
        };

        string jsonData = JsonUtility.ToJson(data);

        UnityWebRequest request = new UnityWebRequest(webAppUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ Upload successful to Google Sheet!");
        }
        else
        {
            Debug.LogError("❌ Upload failed: " + request.error);
        }
    }

    [System.Serializable]
    public class AssessmentData
    {
        public string userName;
        public int score;
        public int timeUsed;
        public int mistakes;
        public string result;
        public string mode; // ✅ Added mode here
    }

    public void UpdateUserName()
    {
        if (UserNameManager.Instance != null)
        {
            userName = UserNameManager.Instance.GetUserName();
            Debug.Log("✅ AssessmentManager updated userName to: " + userName);
        }
        else
        {
            userName = "Anonymous";
            Debug.LogWarning("⚠️ UserNameManager not found during UpdateUserName!");
        }
    }
}
