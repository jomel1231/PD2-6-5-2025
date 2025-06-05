using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;
using TMPro;

[RequireComponent(typeof(XRBaseInteractable))]
public class BootloopButton : MonoBehaviour
{
    public enum Axis { X, Y, Z }

    [Header("Movement Settings")]
    public Axis moveAxis = Axis.Y;
    public float moveDistance = 0.02f;
    public float returnDelay = 0.5f;

    [Header("UI Images (Assign 4 RawImages)")]
    public RawImage[] uiImages = new RawImage[4];

    [Header("UI Buttons")]
    public Button uiButton1;
    public Button uiButton2;

    [Header("Enable These On Steps")]
    public GameObject objectOnTask4;
    public GameObject objectOnTask5;

    [Header("Slider Hold")]
    public Slider holdProgressSlider;

    [Header("External Buttons")]
    public VolumeUpButton volumeUpButton;
    public VolumeDownButton volumeDownButton;

    [Header("Lightning Socket Reference")]
    public XRSocketInteractor lightningSocket;

    [Header("Projector Task Integration")]
    public ProjectorTaskManager projectorTaskManager;
    public int task1Index;
    public int task2Index;
    public int task3Index;
    public int task4Index;
    public int task5Index;

    [Header("Assessment Uploader")]
    public AssessmentUploader assessmentUploader;

    [Header("Completion UI & Sound")]
    public GameObject completionUIPanel;
    public AudioSource completionSound;

    [Header("Fail Sound")]
    public AudioSource failSound;

    [Header("Popup After Task 1 (Diagnosis)")]
    public GameObject popupAfterTask1;

    [Header("Popup If Task 5 Is Late")]
    public GameObject lateWarningPopup;
    public float bootloopTimeLimit = 10f;

    [Header("Late Popup Text Elements")]
    public TextMeshProUGUI lateTimeText;
    public TextMeshProUGUI lateStepsText;
    public TextMeshProUGUI lateResultText;
    public TextMeshProUGUI lateSummaryText;

    [Header("Countdown Display")]
    public TextMeshProUGUI countdownText;

    [HideInInspector] public bool allowTask2 = false;

    private Vector3 originalPosition;
    private XRBaseInteractable interactable;
    private bool isAnimating = false;

    private float holdTimer = 0f;
    private bool isLongHolding = false;

    private bool task1Done = false;
    private bool task2Done = false;
    private bool task3Done = false;
    private bool task4Done = false;
    private bool task5Done = false;

    private bool volumeUpTriggered = false;
    private bool volumeDownTriggered = false;

    private bool bootloopTimerStarted = false;
    private float bootloopTimer = 0f;
    private bool latePopupShown = false;

    private Coroutine imageLoopCoroutine;

    void Start()
    {
        originalPosition = transform.localPosition;
        interactable = GetComponent<XRBaseInteractable>();
        interactable.selectEntered.AddListener(OnGrabbed);
        interactable.selectExited.AddListener(OnReleased);

        if (uiButton2 != null)
            uiButton2.onClick.AddListener(OnUIButton2Clicked);
        if (uiButton1 != null)
            uiButton1.onClick.AddListener(OnUIButton1Clicked);

        DisableAllUI();
        if (uiImages.Length > 0 && uiImages[0] != null)
            imageLoopCoroutine = StartCoroutine(LoopImageElement0());

        if (objectOnTask4 != null) objectOnTask4.SetActive(false);
        if (objectOnTask5 != null) objectOnTask5.SetActive(false);

        if (holdProgressSlider != null)
        {
            holdProgressSlider.minValue = 0f;
            holdProgressSlider.maxValue = 5f;
            holdProgressSlider.value = 0f;
            holdProgressSlider.gameObject.SetActive(false);
        }

        if (completionUIPanel) completionUIPanel.SetActive(false);
        if (popupAfterTask1) popupAfterTask1.SetActive(false);
        if (lateWarningPopup) lateWarningPopup.SetActive(false);

        if (lateTimeText) lateTimeText.gameObject.SetActive(false);
        if (lateStepsText) lateStepsText.gameObject.SetActive(false);
        if (lateSummaryText) lateSummaryText.gameObject.SetActive(false);
        if (lateResultText) lateResultText.gameObject.SetActive(false);
        if (countdownText) countdownText.gameObject.SetActive(false);

        if (lightningSocket != null)
        {
            lightningSocket.selectEntered.AddListener(args =>
            {
                if (!allowTask2) return;
                task2Done = true;
                projectorTaskManager.MarkTaskComplete(task2Index);
            });

            lightningSocket.selectExited.AddListener(args =>
            {
                if (!allowTask2) return;
                task2Done = false;
                projectorTaskManager.MarkTaskIncomplete(task2Index);
            });
        }

        if (volumeUpButton != null)
            volumeUpButton.GetComponent<XRBaseInteractable>().selectEntered.AddListener((_) => volumeUpTriggered = true);
        if (volumeDownButton != null)
            volumeDownButton.GetComponent<XRBaseInteractable>().selectEntered.AddListener((_) => volumeDownTriggered = true);
    }

    void Update()
    {
        if (isLongHolding)
        {
            holdTimer += Time.deltaTime;
            if (holdProgressSlider && holdTimer >= 1f)
            {
                holdProgressSlider.gameObject.SetActive(true);
                holdProgressSlider.value = Mathf.Min(holdTimer, 5f);
            }

            if (!task1Done && volumeUpTriggered && volumeDownTriggered && holdTimer >= 3f)
                CompleteTask1();

            if (!task3Done && task2Done && volumeDownButton.IsPressed && holdTimer >= 5f)
                CompleteTask3();
        }

        if (bootloopTimerStarted && !latePopupShown)
        {
            bootloopTimer += Time.deltaTime;

            if (countdownText && bootloopTimer <= bootloopTimeLimit)
            {
                float remaining = Mathf.Clamp(bootloopTimeLimit - bootloopTimer, 0, bootloopTimeLimit);
                int minutes = Mathf.FloorToInt(remaining / 60);
                int seconds = Mathf.FloorToInt(remaining % 60);
                countdownText.text = $"{minutes:00}:{seconds:00}";
            }

            if (bootloopTimer >= bootloopTimeLimit && GetCompletedCount() < 5)
            {
                latePopupShown = true;
                if (lateWarningPopup) lateWarningPopup.SetActive(true);
                if (failSound) failSound.Play();
                if (countdownText) countdownText.gameObject.SetActive(false);

                if (lateTimeText)
                {
                    lateTimeText.gameObject.SetActive(true);
                    lateTimeText.text = $"{Mathf.FloorToInt(bootloopTimer / 60):00}:{Mathf.FloorToInt(bootloopTimer % 60):00}";
                }

                if (lateStepsText)
                {
                    lateStepsText.gameObject.SetActive(true);
                    lateStepsText.text = GetCompletedCount().ToString();
                }

                if (lateSummaryText)
                {
                    lateSummaryText.gameObject.SetActive(true);
                    lateSummaryText.text = GetCompletedCount() + "/5";
                }

                if (lateResultText)
                {
                    lateResultText.gameObject.SetActive(true);
                    lateResultText.text = "FAILED";
                }

                // 🔴 Upload failure result
                if (assessmentUploader)
                {
                    string timeUsed = $"{Mathf.FloorToInt(bootloopTimer / 60):00}:{Mathf.FloorToInt(bootloopTimer % 60):00}";
                    string taskCompleted = $"P1: {(task1Done ? "✔" : "✘")}, {(task2Done ? "✔" : "✘")}, {(task3Done ? "✔" : "✘")}, {(task4Done ? "✔" : "✘")}, {(task5Done ? "✔" : "✘")}";
                    string score = $"{GetCompletedCount()}/5";
                    assessmentUploader.UploadResult(timeUsed, taskCompleted, score, "FAILED");
                }
            }
        }
    }

    private void CompleteTask1()
    {
        task1Done = true;
        projectorTaskManager.MarkTaskComplete(task1Index);
        if (popupAfterTask1) popupAfterTask1.SetActive(true);
    }

    private void CompleteTask3()
    {
        task3Done = true;
        projectorTaskManager.MarkTaskComplete(task3Index);

        if (uiImages.Length > 1)
        {
            if (uiImages[0]) uiImages[0].gameObject.SetActive(false);
            if (uiImages[1]) uiImages[1].gameObject.SetActive(true);
        }

        if (uiButton2) uiButton2.gameObject.SetActive(true);
    }

    private void OnUIButton2Clicked()
    {
        if (!task3Done || task4Done) return;
        task4Done = true;
        projectorTaskManager.MarkTaskComplete(task4Index);
        if (objectOnTask4) objectOnTask4.SetActive(true);
        if (uiButton2) uiButton2.interactable = false;
    }

    private void OnUIButton1Clicked()
    {
        if (!task4Done || task5Done) return;

        task5Done = true;
        projectorTaskManager.MarkTaskComplete(task5Index);

        if (uiImages.Length > 2)
        {
            if (uiImages[2]) uiImages[2].gameObject.SetActive(false);
            if (uiImages[3]) uiImages[3].gameObject.SetActive(true);
        }

        if (objectOnTask5) objectOnTask5.SetActive(true);
        if (uiButton1) uiButton1.interactable = false;

        if (bootloopTimerStarted && bootloopTimer <= bootloopTimeLimit && GetCompletedCount() == 5)
        {
            if (completionUIPanel) completionUIPanel.SetActive(true);
            if (completionSound) completionSound.Play();
            if (countdownText) countdownText.gameObject.SetActive(false);

            if (lateTimeText)
            {
                lateTimeText.gameObject.SetActive(true);
                lateTimeText.text = $"{Mathf.FloorToInt(bootloopTimer / 60):00}:{Mathf.FloorToInt(bootloopTimer % 60):00}";
            }

            if (lateStepsText)
            {
                lateStepsText.gameObject.SetActive(true);
                lateStepsText.text = GetCompletedCount().ToString();
            }

            if (lateSummaryText)
            {
                lateSummaryText.gameObject.SetActive(true);
                lateSummaryText.text = GetCompletedCount() + "/5";
            }

            if (lateResultText)
            {
                lateResultText.gameObject.SetActive(true);
                lateResultText.text = "PASSED";
            }

            // ✅ Upload success result
            if (assessmentUploader)
            {
                string timeUsed = $"{Mathf.FloorToInt(bootloopTimer / 60):00}:{Mathf.FloorToInt(bootloopTimer % 60):00}";
                string taskCompleted = $"P1: ✔, ✔, ✔, ✔, ✔";
                string score = "5/5";
                assessmentUploader.UploadResult(timeUsed, taskCompleted, score, "PASSED");
            }
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (isAnimating) return;
        StartCoroutine(AnimatePress());
        isLongHolding = true;
        holdTimer = 0f;
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        isLongHolding = false;
        holdTimer = 0f;
        if (holdProgressSlider)
        {
            holdProgressSlider.value = 0f;
            holdProgressSlider.gameObject.SetActive(false);
        }
    }

    private IEnumerator AnimatePress()
    {
        isAnimating = true;
        transform.localPosition = originalPosition + GetOffset();
        yield return new WaitForSeconds(returnDelay);
        transform.localPosition = originalPosition;
        isAnimating = false;
    }

    private IEnumerator LoopImageElement0()
    {
        while (!task3Done)
        {
            if (uiImages.Length > 0 && uiImages[0])
                uiImages[0].gameObject.SetActive(!uiImages[0].gameObject.activeSelf);
            yield return new WaitForSeconds(2f);
        }
        if (uiImages.Length > 0 && uiImages[0]) uiImages[0].gameObject.SetActive(false);
    }

    private void DisableAllUI()
    {
        foreach (var img in uiImages)
        {
            if (img) img.gameObject.SetActive(false);
        }
    }

    private Vector3 GetOffset()
    {
        return moveAxis switch
        {
            Axis.X => new Vector3(-moveDistance, 0, 0),
            Axis.Y => new Vector3(0, -moveDistance, 0),
            Axis.Z => new Vector3(0, 0, -moveDistance),
            _ => Vector3.zero,
        };
    }

    public void StartBootloopTimer()
    {
        bootloopTimerStarted = true;
        bootloopTimer = 0f;
        latePopupShown = false;

        if (countdownText)
        {
            countdownText.gameObject.SetActive(true);
            countdownText.text = $"{(int)(bootloopTimeLimit / 60):00}:{(int)(bootloopTimeLimit % 60):00}";
        }
    }

    private int GetCompletedCount()
    {
        int count = 0;
        if (task1Done) count++;
        if (task2Done) count++;
        if (task3Done) count++;
        if (task4Done) count++;
        if (task5Done) count++;
        return count;
    }
}
