using UnityEngine;
using UnityEngine.UI;

public class SkipAudioButton : MonoBehaviour
{
    [Header("Audio Source to Stop")]
    public AudioSource audioSource;

    [Header("Button Reference")]
    public Button stopButton;

    void Start()
    {
        if (stopButton != null)
            stopButton.onClick.AddListener(StopAudio);
        else
            Debug.LogWarning("Stop Button is not assigned.");
    }

    void StopAudio()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
            Debug.Log("🔇 Audio stopped.");
        }
    }
}
