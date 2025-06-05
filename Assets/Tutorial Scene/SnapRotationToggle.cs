using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SnapRotationToggle : MonoBehaviour
{
    [Header("Rotation Settings")]
    public Vector3 offRotation = new Vector3(0f, 0f, 0f);
    public Vector3 onRotation = new Vector3(15f, 0f, 0f);

    [Header("Position Offset Settings")]
    public Vector3 offPosition = new Vector3(0f, 0f, 0f);
    public Vector3 onPosition = new Vector3(0f, 0.01f, -0.01f);

    [Header("XR Interaction")]
    public XRBaseInteractable interactable;

    [Header("Audio Settings")]
    public AudioSource machineAudio;    // e.g., humming/motor sound
    public AudioSource voiceoverAudio;  // e.g., explanation/voice instructions

    private bool isOn = false;

    private void Start()
    {
        transform.localEulerAngles = offRotation;
        transform.localPosition = offPosition;

        if (interactable == null)
            interactable = GetComponent<XRBaseInteractable>();

        if (interactable != null)
            interactable.selectEntered.AddListener(OnToggle);
    }

    private void OnDestroy()
    {
        if (interactable != null)
            interactable.selectEntered.RemoveListener(OnToggle);
    }

    private void OnToggle(SelectEnterEventArgs args)
    {
        isOn = !isOn;

        transform.localEulerAngles = isOn ? onRotation : offRotation;
        transform.localPosition = isOn ? onPosition : offPosition;

        if (isOn)
        {
            // Play both audio sources at the same time
            if (machineAudio != null && !machineAudio.isPlaying)
                machineAudio.Play();

            if (voiceoverAudio != null && !voiceoverAudio.isPlaying)
                voiceoverAudio.Play();
        }
        else
        {
            // Stop both audio sources
            if (machineAudio != null)
            {
                machineAudio.Stop();
                machineAudio.time = 0f;
            }

            if (voiceoverAudio != null)
            {
                voiceoverAudio.Stop();
                voiceoverAudio.time = 0f;
            }
        }
    }
}
