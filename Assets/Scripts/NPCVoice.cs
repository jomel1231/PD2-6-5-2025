using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class NPCVoice : MonoBehaviour
{
    private AudioSource audioSource;
    public XRBaseInteractable interactable;

    void Start()
    {
        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();

        // Ensure it plays once on wake
        if (audioSource && !audioSource.isPlaying)
            audioSource.Play();
        
        // Add click event listener
        interactable.selectEntered.AddListener(PlayVoice);
    }

    private void PlayVoice(SelectEnterEventArgs args)
    {
        if (audioSource)
            audioSource.Play();
    }
}
