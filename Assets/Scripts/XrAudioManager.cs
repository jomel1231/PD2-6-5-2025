using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class XrAudioManager : MonoBehaviour
{
    [Header("Progress Control")]
    [SerializeField] private ProgressControl progressControl;
    [SerializeField] private AudioSource progressSound;
    [SerializeField] private AudioClip challengeCompleteClip;

    [Header("Challenge Objects")]
    [SerializeField] private List<XRGrabInteractable> challengeObjects = new List<XRGrabInteractable>();
    [SerializeField] private List<GameObject> challengeSpotlights = new List<GameObject>();

    private bool gameStarted = false;
    private int currentChallengeIndex = 0; // Start at 0
    private bool challengeCompleted = false;

    [Header("Background Music")]
    [SerializeField] private AudioSource backgroundMusic;
    [SerializeField] private AudioClip backgroundMusicClip;

    [Header("Start Game Audio")]
    [SerializeField] private AudioSource startGameAudioSource;
    [SerializeField] private AudioClip startGameClip;

    [Header("UI Panels")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject challengePanel;
    [SerializeField] private TextMeshProUGUI challengeText;

    private void OnEnable()
    {
        if (progressControl != null)
        {
            progressControl.OnStartGame.AddListener(StartGame);
        }

        foreach (var obj in challengeObjects)
        {
            obj.selectEntered.AddListener(OnChallengeObjectUsed);
        }
    }

    private void OnDisable()
    {
        if (progressControl != null)
        {
            progressControl.OnStartGame.RemoveListener(StartGame);
        }

        foreach (var obj in challengeObjects)
        {
            obj.selectEntered.RemoveListener(OnChallengeObjectUsed);
        }
    }

    private void Start()
    {
        if (backgroundMusic != null && backgroundMusicClip != null)
        {
            backgroundMusic.clip = backgroundMusicClip;
            backgroundMusic.loop = true;
            backgroundMusic.Play();
        }

        if (startPanel != null) startPanel.SetActive(true);
        if (challengePanel != null) challengePanel.SetActive(false);

        // Ensure all spotlights are OFF at the beginning
        foreach (var light in challengeSpotlights)
        {
            if (light != null) light.SetActive(false);
        }
    }

    private void StartGame(string arg0)
    {
        if (gameStarted) return;

        gameStarted = true;

        if (startPanel != null) startPanel.SetActive(false);
        if (challengePanel != null) challengePanel.SetActive(true);

        // Play the start game audio
        if (startGameAudioSource != null && startGameClip != null)
        {
            startGameAudioSource.clip = startGameClip;
            startGameAudioSource.loop = false; // Ensure it doesn't loop
            startGameAudioSource.Play();
        }

        // Activate the first challenge
        ActivateCurrentChallenge();
    }

    private void OnChallengeObjectUsed(SelectEnterEventArgs args)
    {
        if (!gameStarted || challengeCompleted) return;

        // Check if the grabbed tool matches the current challenge tool using tags
        if (args.interactableObject.transform.CompareTag(GetTagForChallenge(currentChallengeIndex)))
        {
            Debug.Log($"{args.interactableObject.transform.name} picked up! Challenge complete.");
            ChallengeComplete();
        }
    }

    private void ChallengeComplete()
    {
        if (challengeCompleted) return;

        challengeCompleted = true;

        if (progressSound != null && challengeCompleteClip != null)
        {
            progressSound.PlayOneShot(challengeCompleteClip);
        }

        if (progressControl != null)
        {
            progressControl.OnChallengeComplete.Invoke("Challenge Complete!");
        }

        Debug.Log("Challenge Completed! Waiting before next challenge...");
        StartCoroutine(ProceedToNextChallenge());
    }

    private IEnumerator ProceedToNextChallenge()
    {
        yield return new WaitForSeconds(2f);

        // Turn off the previous spotlight
        if (currentChallengeIndex < challengeSpotlights.Count && challengeSpotlights[currentChallengeIndex] != null)
        {
            challengeSpotlights[currentChallengeIndex].SetActive(false);
        }

        currentChallengeIndex++;

        if (currentChallengeIndex >= challengeObjects.Count)
        {
            Debug.Log("No more challenges left!");
            yield break;
        }

        challengeCompleted = false;

        if (progressControl != null)
        {
            progressControl.UpdateChallengeText(currentChallengeIndex);
        }

        Debug.Log($"Starting Challenge {currentChallengeIndex}");

        // Activate the new challenge spotlight
        ActivateCurrentChallenge();
    }

    private void ActivateCurrentChallenge()
    {
        if (currentChallengeIndex < challengeSpotlights.Count && challengeSpotlights[currentChallengeIndex] != null)
        {
            challengeSpotlights[currentChallengeIndex].SetActive(true);
            Debug.Log($"Spotlight turned on for Challenge {currentChallengeIndex}");
        }
    }

    private string GetTagForChallenge(int challengeIndex)
    {
        switch (challengeIndex)
        {
            case 0: return "SuctionTool";
            case 1: return "Screwdriver";
            case 2: return "Scalpel";
            default: return "";
        }
    }
}
