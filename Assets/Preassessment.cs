using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.Video;

public class PreAssessmentManager : MonoBehaviour
{
    [Header("Timer Settings")]
    public float totalAssessmentTime = 900f;
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
    [SerializeField] private string webAppUrl = "https://script.google.com/macros/s/AKfycbzqIiP6Bc_asxxJVOESkdEx7OfN_UMSqsBSXHF1Fjyc0EpsYsArPnHnvpzj1HRXft-m6Q/exec";

    private bool assessmentEnded = false;
    private string userName = "Anonymous";
    private string mode = "PreAssessment"; // 🔥

    void Start()
    {
        currentTime = totalAssessmentTime;

        if (UserNameManager.Instance != null)
        {
            userName = UserNameManager.Instance.GetUserName();
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
            EndAssessment(false);
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

    void EndAssessment(bool passed)
    {
        assessmentEnded = true;

        VidController vidController = FindObjectOfType<VidController>();
        if (vidController != null)
            vidController.StopCountdownVideo();

        if (assemblyVideoTimerCheck != null)
        {
            if (passed)
                assemblyVideoTimerCheck.OnProcessComplete();
            else
                assemblyVideoTimerCheck.OnProcessFail();
        }

        int totalSteps = disassemblyManager.steps.Count + assemblyManager.steps.Count;
        int completedSteps = disassemblyManager.GetCompletedSteps() + assemblyManager.GetCompletedSteps();
        int score = passed ? 100 : 0;
        int mistakes = totalSteps - completedSteps;
        float timeUsed = totalAssessmentTime - currentTime;

        ShowResultPanel(passed, completedSteps, totalSteps, timeUsed);

        UploadAssessment(userName, score, Mathf.RoundToInt(timeUsed), mistakes, passed ? "Pass" : "Fail", mode);
    }

    void ShowResultPanel(bool passed, int completedSteps, int totalSteps, float timeUsed)
    {
        string timeFormatted = $"{Mathf.FloorToInt(timeUsed / 60f):00}:{Mathf.FloorToInt(timeUsed % 60f):00}";

        if (passed && passPanel != null)
        {
            passPanel.SetActive(true);
            passTimeUsedText.text = timeFormatted;
            passTasksCompletedText.text = $"{completedSteps}/{totalSteps}";
            passResultText.text = "PASS";
            passFinalScoreText.text = "100%";
        }
        else if (failPanel != null)
        {
            failPanel.SetActive(true);
            failTimeUsedText.text = timeFormatted;
            failTasksCompletedText.text = $"{completedSteps}/{totalSteps}";
            failResultText.text = "FAIL";
            failFinalScoreText.text = "0%";
        }
    }

    void UploadAssessment(string userName, int score, int timeUsed, int mistakes, string result, string mode)
    {
        StartCoroutine(Upload(userName, score, timeUsed, mistakes, result, mode));
    }

    IEnumerator Upload(string userName, int score, int timeUsed, int mistakes, string result, string mode)
    {
        AssessmentData data = new AssessmentData
        {
            userName = userName,
            score = score,
            timeUsed = timeUsed,
            mistakes = mistakes,
            result = result,
            mode = mode
        };

        string jsonData = JsonUtility.ToJson(data);

        UnityWebRequest request = new UnityWebRequest(webAppUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
            Debug.Log("✅ Pre-Assessment Upload Success!");
        else
            Debug.LogError("❌ Pre-Assessment Upload Failed: " + request.error);
    }

    [System.Serializable]
    public class AssessmentData
    {
        public string userName;
        public int score;
        public int timeUsed;
        public int mistakes;
        public string result;
        public string mode;
    }
}
