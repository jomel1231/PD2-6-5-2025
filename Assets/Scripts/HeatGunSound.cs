using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HeatGunSound : MonoBehaviour
{
    private AudioSource heatGunAudio;
    private XRGrabInteractable grabInteractable;
    public InputActionProperty triggerPressAction;

    private bool isHeld = false;
    private float heatHeldTime = 0f;
    public float requiredHoldTime = 5f;

    public int heatGunTaskIndex = 0;
    public ProjectorTaskManager projectorTaskManager;
    private bool hasCompleted = false;

    [Header("UI")]
    public Slider heatProgressSlider;

    [Header("Dashline Guide Activation")]
    public GameObject dashLineGroupObject; // 👈 Drag the DashLineGroupManager GameObject here

    void Start()
    {
        heatGunAudio = GetComponent<AudioSource>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);

        if (heatProgressSlider != null)
        {
            heatProgressSlider.minValue = 0f;
            heatProgressSlider.maxValue = requiredHoldTime;
            heatProgressSlider.value = 0f;
            heatProgressSlider.gameObject.SetActive(false);
        }

        if (dashLineGroupObject != null)
            dashLineGroupObject.SetActive(false); // Make sure it's off at start
    }

    void Update()
    {
        float triggerValue = triggerPressAction.action.ReadValue<float>();

        if (isHeld && triggerValue > 0.1f)
        {
            if (heatProgressSlider != null)
            {
                heatProgressSlider.gameObject.SetActive(true);
                heatProgressSlider.value = Mathf.Min(heatHeldTime, requiredHoldTime);
            }

            heatHeldTime += Time.deltaTime;

            if (!heatGunAudio.isPlaying)
                heatGunAudio.Play();

            if (!hasCompleted && heatHeldTime >= requiredHoldTime)
            {
                hasCompleted = true;

                if (projectorTaskManager != null)
                    projectorTaskManager.MarkTaskComplete(heatGunTaskIndex);

                if (dashLineGroupObject != null)
                    dashLineGroupObject.SetActive(true); // 👈 Enable guideline
            }
        }
        else
        {
            if (heatGunAudio.isPlaying)
                heatGunAudio.Stop();

            if (isHeld)
                heatHeldTime = 0f;

            if (heatProgressSlider != null)
            {
                heatProgressSlider.value = 0f;
                heatProgressSlider.gameObject.SetActive(false);
            }
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        isHeld = true;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        isHeld = false;
        heatHeldTime = 0f;

        heatGunAudio.Stop();

        if (heatProgressSlider != null)
        {
            heatProgressSlider.value = 0f;
            heatProgressSlider.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }
}
