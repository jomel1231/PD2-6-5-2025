using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class SolderingIronSound : MonoBehaviour
{
    private AudioSource solderingAudio;
    private XRGrabInteractable grabInteractable;
    public InputActionProperty triggerPressAction;

    private bool isHeld = false;
    public int solderingTaskIndex = 0;

    public ProjectorTaskManager projectorTaskManager;
    private bool hasPlayed = false;

    void Start()
    {
        solderingAudio = GetComponent<AudioSource>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    void Update()
    {
        if (!isHeld) return;

        float triggerValue = triggerPressAction.action.ReadValue<float>();
        if (triggerValue > 0.1f)
        {
            if (!solderingAudio.isPlaying)
            {
                solderingAudio.Play();

                if (!hasPlayed && projectorTaskManager != null)
                {
                    hasPlayed = true;
                    projectorTaskManager.MarkTaskComplete(solderingTaskIndex);
                }
            }
        }
        else
        {
            if (solderingAudio.isPlaying)
                solderingAudio.Stop();
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        isHeld = true;
        triggerPressAction.action.Enable();
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        isHeld = false;
        triggerPressAction.action.Disable();
        solderingAudio.Stop();
    }

    private void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrab);
            grabInteractable.selectExited.RemoveListener(OnRelease);
        }
    }
}
