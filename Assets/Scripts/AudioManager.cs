using UnityEngine;
using UnityEngine.Audio;
using System;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0, 1)]
    public float volume = 1;
    [Range(-3, 3)]
    public float pitch = 1;
    public bool loop = false;
    [HideInInspector] public AudioSource source; // Hide in Inspector since it's assigned dynamically

    public Sound()
    {
        volume = 1;
        pitch = 1;
        loop = false;
    }
}

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public static AudioManager instance;

    void Awake()
    {
        // Ensure only one AudioManager exists across all scenes
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("Duplicate AudioManager detected. Destroying the new instance.");
            Destroy(gameObject);
            return;
        }

        // Initialize sounds
        InitializeSounds();
    }

    void Start()
    {
        // Assign scene change event to reset AudioSources when switching scenes
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Prevent memory leaks
    }

    // Called when a new scene is loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene Loaded: " + scene.name);
        ReassignAudioSources();
    }

    // Initialize audio sources
    private void InitializeSounds()
    {
        foreach (Sound s in sounds)
        {
            if (s.source == null)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.loop = s.loop;
            }
        }
    }

    // Reassign AudioSources if lost after scene load
    private void ReassignAudioSources()
    {
        foreach (Sound s in sounds)
        {
            if (s.source == null) // If missing, reinitialize
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.loop = s.loop;
            }
        }
        Debug.Log("Audio Sources Reassigned After Scene Load");
    }

    // Play sound
    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null || s.source == null)
        {
            Debug.LogWarning("Sound: " + name + " not found or AudioSource is null.");
            return;
        }

        Debug.Log("Playing Sound: " + name);
        s.source.Play();
    }

    // Stop sound
    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null || s.source == null)
        {
            Debug.LogWarning("Sound: " + name + " not found or AudioSource is null.");
            return;
        }

        s.source.Stop();
    }
}
