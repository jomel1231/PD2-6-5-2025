using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroTextManager : MonoBehaviour
{
    public TextMeshProUGUI introText;
    public Button nextButton;
    public string sceneToLoad;

    public AudioSource audioSource;                // Drag your AudioSource here
    public AudioClip[] pageAudioClips;             // Assign 4 audio clips here

    private int currentPage = 0;

    private string[] pages = new string[]
    {
        "Welcome to VR Mobile Phone Training!\n\nLearn to repair phones in a safe, interactive 3D environment.",

        "What You'll Learn:\nYou’ll learn how to use common repair tools like a screwdriver, suction cup, and spudger to safely open and fix mobile phones.",

        "How It Works:\n- Follow each step carefully.\n- Only interact with highlighted parts.\n- Progress at your own pace.",

        "Your Goal:\n- Successfully repair a phone.\n- Use tools correctly and safely.\n- Understand each step in the repair process."
    };

    void Start()
    {
        ShowPage();
        nextButton.onClick.AddListener(NextPage);
        nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Next";
    }

    void ShowPage()
    {
        // Update the UI text
        introText.text = pages[currentPage];

        // Update the button label
        if (currentPage == pages.Length - 1)
        {
            nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Start";
        }
        else
        {
            nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Next";
        }

        // Play corresponding audio
        PlayPageAudio(currentPage);
    }

    void PlayPageAudio(int index)
    {
        if (pageAudioClips != null && index < pageAudioClips.Length && pageAudioClips[index] != null)
        {
            audioSource.Stop();
            audioSource.clip = pageAudioClips[index];
            audioSource.Play();
        }
    }

    void NextPage()
    {
        if (currentPage < pages.Length - 1)
        {
            currentPage++;
            ShowPage();
        }
        else
        {
            LoadNextScene();
        }
    }

    void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogError("Scene name is not set! Assign it in the Inspector.");
        }
    }
}
