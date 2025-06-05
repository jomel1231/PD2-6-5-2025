using UnityEngine;

public class PlayAudioOnEnable : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    private void OnEnable()
    {
        if (audioSource != null)
            audioSource.Play();
    }
}
