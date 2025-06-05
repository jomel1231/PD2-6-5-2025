using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

[RequireComponent(typeof(XRBaseInteractable))]
public class XRPressableButtonWithUI : MonoBehaviour
{
    public enum Axis { X, Y, Z }

    [Header("Movement Settings")]
    public Axis moveAxis = Axis.Y;
    public float moveDistance = 0.02f;
    public float returnDelay = 0.5f;

    [Header("UI Images (Assign 4 RawImages)")]
    public RawImage[] uiImages = new RawImage[4];

    [Header("UI Button to Cycle Pages")]
    public Button nextPageButton;

    [Header("Final Step Object (Enabled After Page 4)")]
    public GameObject objectToEnableOnPage4;

    [Header("Projector Task Integration")]
    public ProjectorTaskManager projectorTaskManager;
    public int page4TaskIndex = 0;
    public int screenOffTaskIndex = 1;

    [Header("Screen Unlock Dependency")]
    public ScreenUnlockManager screenUnlockManager;

    [Header("Hold Slider UI")]
    public Slider holdProgressSlider;

    [Header("Battery Diagnosis Popup Integration")]
    public BatteryDiagnosisManager batteryDiagnosisManager;
    public bool allowNextStep = false;

    private Vector3 originalPosition;
    private bool isAnimating = false;
    private bool uiActive = false;
    private int currentImageIndex = -1;

    private bool page4Marked = false;
    private bool screenOffMarked = false;

    private bool isLongHolding = false;
    private float holdTimer = 0f;
    private float requiredHoldTime = 3f;

    private bool uiLocked = false;
    private bool screenOffMarkedByHold = false;

    private XRBaseInteractable interactable;

    private void Start()
    {
        originalPosition = transform.localPosition;

        DisableAllUI();
        if (objectToEnableOnPage4 != null)
            objectToEnableOnPage4.SetActive(false);

        interactable = GetComponent<XRBaseInteractable>();
        interactable.selectEntered.AddListener(OnGrabbed);
        interactable.selectExited.AddListener(OnReleased);

        if (nextPageButton != null)
            nextPageButton.onClick.AddListener(OnNextPageClicked);

        if (holdProgressSlider != null)
        {
            holdProgressSlider.minValue = 0f;
            holdProgressSlider.maxValue = requiredHoldTime;
            holdProgressSlider.value = 0f;
            holdProgressSlider.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (isLongHolding)
        {
            holdTimer += Time.deltaTime;

            if (holdProgressSlider != null && holdTimer >= 1f)
            {
                holdProgressSlider.gameObject.SetActive(true);
                holdProgressSlider.value = Mathf.Min(holdTimer, requiredHoldTime);
            }

            if (holdTimer >= requiredHoldTime)
            {
                isLongHolding = false;
                holdTimer = 0f;

                if (holdProgressSlider != null)
                {
                    holdProgressSlider.value = 0f;
                    holdProgressSlider.gameObject.SetActive(false);
                }

                ToggleUILock();

                if (!screenOffMarkedByHold && projectorTaskManager != null)
                {
                    projectorTaskManager.MarkTaskComplete(screenOffTaskIndex);
                    screenOffMarkedByHold = true;
                }
            }
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (isAnimating) return;

        StartCoroutine(AnimatePress());

        if (screenUnlockManager != null && screenUnlockManager.IsScreenTriggered())
            return;

        if (uiLocked) return;

        if (!uiActive)
        {
            currentImageIndex = 0;
            ShowUI(currentImageIndex);
            uiActive = true;
        }
        else
        {
            if (!screenOffMarked && projectorTaskManager != null)
            {
                projectorTaskManager.MarkTaskComplete(screenOffTaskIndex);
                screenOffMarked = true;
            }

            ResetUIState();
        }
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        isLongHolding = false;
        holdTimer = 0f;

        if (holdProgressSlider != null)
        {
            holdProgressSlider.value = 0f;
            holdProgressSlider.gameObject.SetActive(false);
        }
    }

    private void ToggleUILock()
    {
        uiLocked = !uiLocked;

        if (uiLocked)
        {
            ResetUIState();
        }
        else
        {
            currentImageIndex = 0;
            ShowUI(currentImageIndex);
            uiActive = true;
        }
    }

    public void OnNextPageClicked()
    {
        if (!uiActive || currentImageIndex == -1)
            return;

        if (!allowNextStep && currentImageIndex == 3)
        {
            Debug.Log("Battery diagnosis not answered yet. Cannot proceed.");
            return;
        }

        uiImages[currentImageIndex].gameObject.SetActive(false);
        currentImageIndex++;

        if (currentImageIndex < uiImages.Length)
        {
            ShowUI(currentImageIndex);

            if (currentImageIndex == 3 && objectToEnableOnPage4 != null)
            {
                objectToEnableOnPage4.SetActive(true);

                if (!page4Marked && projectorTaskManager != null)
                {
                    projectorTaskManager.MarkTaskComplete(page4TaskIndex);
                    page4Marked = true;
                }

                if (batteryDiagnosisManager != null && batteryDiagnosisManager.diagnosisPopup != null)
                {
                    batteryDiagnosisManager.diagnosisPopup.SetActive(true);
                    allowNextStep = false;
                }
            }
        }
        else
        {
            ResetUIState();
        }
    }

    private void ShowUI(int index)
    {
        if (index >= 0 && index < uiImages.Length)
            uiImages[index].gameObject.SetActive(true);
    }

    private void DisableAllUI()
    {
        foreach (var img in uiImages)
        {
            if (img != null)
                img.gameObject.SetActive(false);
        }
    }

    private void ResetUIState()
    {
        DisableAllUI();
        currentImageIndex = -1;
        uiActive = false;

        if (objectToEnableOnPage4 != null)
            objectToEnableOnPage4.SetActive(false);
    }

    private IEnumerator AnimatePress()
    {
        isAnimating = true;
        Vector3 pressedPosition = originalPosition + GetOffset();
        transform.localPosition = pressedPosition;

        isLongHolding = true;
        holdTimer = 0f;

        yield return new WaitForSeconds(returnDelay);

        transform.localPosition = originalPosition;
        isAnimating = false;
    }

    private Vector3 GetOffset()
    {
        switch (moveAxis)
        {
            case Axis.X: return new Vector3(-moveDistance, 0, 0);
            case Axis.Y: return new Vector3(0, -moveDistance, 0);
            case Axis.Z: return new Vector3(0, 0, -moveDistance);
            default: return Vector3.zero;
        }
    }
}
