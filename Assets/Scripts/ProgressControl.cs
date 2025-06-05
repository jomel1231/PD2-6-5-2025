using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;
using TMPro;

public class ProgressControl : MonoBehaviour
{
    public UnityEvent<string> OnStartGame;
    public UnityEvent<string> OnChallengeComplete;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI challengeText;

    [Header("Game Progress")]
    [SerializeField] private XrButtonInteractable startButton;
    [SerializeField] private GameObject keyIndicatorLight;
    [SerializeField] private string startGameString = "Press Start Button";
    [SerializeField] private string[] challengeStrings;

    private bool startGameBool = false;
    private int challengeNumber = 0;
    public int GetChallengeCount()
    {
        return challengeStrings.Length;
    }

    private void Start()
    {
        if (startButton != null)
        {
            startButton.selectEntered.AddListener(StartButtonPressed);
        }

        // Display the start game text initially
        if (challengeText != null)
        {
            challengeText.text = startGameString;
        }
    }

    private void StartButtonPressed(SelectEnterEventArgs arg0)
    {
        if (!startGameBool)
        {
            startGameBool = true;

            if (keyIndicatorLight != null)
            {
                keyIndicatorLight.SetActive(true);
            }

            if (OnStartGame != null && challengeNumber < challengeStrings.Length)
            {
                OnStartGame.Invoke(challengeStrings[challengeNumber]);
            }

            // Update challenge text to the first challenge
            UpdateChallengeText(challengeNumber);
        }
    }

    public void UpdateChallengeText(int challengeIndex)
    {
        if (challengeText != null && challengeStrings != null && challengeIndex < challengeStrings.Length)
        {
            challengeText.text = challengeStrings[challengeIndex];
            challengeNumber = challengeIndex;
        }
    }
}
