using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class NPCDialogue : MonoBehaviour
{
    [Header("Dialogue UI")]
    public TextMeshProUGUI dialogueText;
    public AudioSource audioSource;
    public AudioClip[] dialogueClips;
    public string[] dialogueLines;
    public float textSpeed = 0.05f;

    [System.Serializable]
    public class ObjectWithAudio
    {
        public Sprite imageToShow;
        public Image uiImage;
        public GameObject imageObject;
        public AudioSource objectAudioSource;
        public AudioClip objectAppearClip;
        public AudioClip popSound;
        public string objectSubtitle;
        public float displayTime = 5f;
    }

    [System.Serializable]
    public class TabletUI
    {
        public GameObject tabletObject;
        public TextMeshProUGUI tabletSubtitle;
        public AudioSource tabletAudioSource;
        public AudioClip tabletVoiceover;
        public AudioClip popSound;
        public string tabletSubtitleText;
        public float tabletDisplayTime = 5f;
    }

    [Header("Object With Audio")]
    public ObjectWithAudio objectWithAudio;

    [Header("Tablets UI")]
    public TabletUI firstTablet;
    public TabletUI secondTablet;

    [Header("Final UI Before Scene Change")]
    public GameObject finalUIPanel; // UI for last scene
    public AudioSource finalVoiceover;
    public string nextSceneName;

    private int currentLine = 0;

    void Start()
    {
        // Hide all UI elements at the start
        if (objectWithAudio.imageObject != null)
            objectWithAudio.imageObject.SetActive(false);

        if (firstTablet.tabletObject != null)
            firstTablet.tabletObject.SetActive(false);

        if (secondTablet.tabletObject != null)
            secondTablet.tabletObject.SetActive(false);

        if (finalUIPanel != null)
            finalUIPanel.SetActive(false); // Hide final UI initially

        // Start Dialogue Sequence
        StartCoroutine(PlayDialogueSequence());
    }

    IEnumerator PlayDialogueSequence()
    {
        // Step 1: Play First Two Dialogues
        yield return StartCoroutine(PlayDialogueLine(0));
        yield return StartCoroutine(PlayDialogueLine(1));

        // Step 2: Show Object with Audio
        yield return StartCoroutine(ShowObjectWithAudio());

        // Step 3: Play Dialogue 3
        yield return StartCoroutine(PlayDialogueLine(2));

        // Step 4: Play Dialogue 4
        yield return StartCoroutine(PlayDialogueLine(3));

        // Step 5: Show First Tablet
        yield return StartCoroutine(ShowTabletUI(firstTablet));

        // Step 6: Play Dialogue 5
        yield return StartCoroutine(PlayDialogueLine(4));

        // Step 7: Show Second Tablet
        yield return StartCoroutine(ShowTabletUI(secondTablet));

        // Step 8: Show Final UI Before Scene Change
        yield return StartCoroutine(ShowFinalUI());

        // Hide text at the end
        dialogueText.text = "";
    }

    IEnumerator PlayDialogueLine(int lineIndex)
    {
        if (lineIndex >= dialogueLines.Length || lineIndex >= dialogueClips.Length)
            yield break;

        dialogueText.text = "";
        audioSource.clip = dialogueClips[lineIndex];
        audioSource.Play();
        yield return StartCoroutine(TypeText(dialogueLines[lineIndex], audioSource.clip.length));
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator TypeText(string line, float duration)
    {
        dialogueText.text = "";
        float timePerChar = duration / line.Length;

        for (int i = 0; i < line.Length; i++)
        {
            dialogueText.text += line[i];
            yield return new WaitForSeconds(Mathf.Min(timePerChar, textSpeed));
        }

        yield return new WaitForSeconds(0.5f);
        dialogueText.text = "";
    }

    IEnumerator ShowObjectWithAudio()
    {
        if (objectWithAudio.imageObject == null) yield break;

        objectWithAudio.imageObject.SetActive(true); // Show Object

        // Play Pop Sound
        if (objectWithAudio.popSound != null)
            objectWithAudio.objectAudioSource.PlayOneShot(objectWithAudio.popSound);

        // Play Object Voiceover
        if (objectWithAudio.objectAppearClip != null)
        {
            objectWithAudio.objectAudioSource.clip = objectWithAudio.objectAppearClip;
            objectWithAudio.objectAudioSource.Play();
            yield return StartCoroutine(TypeText(objectWithAudio.objectSubtitle, objectWithAudio.objectAudioSource.clip.length));
        }

        yield return new WaitForSeconds(objectWithAudio.displayTime);

        objectWithAudio.imageObject.SetActive(false); // Hide Object
    }

    IEnumerator ShowTabletUI(TabletUI tablet)
    {
        if (tablet.tabletObject == null) yield break;

        tablet.tabletObject.SetActive(true); // Show Tablet UI

        // Play Pop Sound
        if (tablet.popSound != null)
            tablet.tabletAudioSource.PlayOneShot(tablet.popSound);

        // Play Tablet Voiceover
        if (tablet.tabletAudioSource != null && tablet.tabletVoiceover != null)
        {
            tablet.tabletAudioSource.clip = tablet.tabletVoiceover;
            tablet.tabletAudioSource.Play();
            yield return StartCoroutine(TypeText(tablet.tabletSubtitleText, tablet.tabletAudioSource.clip.length));
        }

        yield return new WaitForSeconds(tablet.tabletDisplayTime);

        tablet.tabletObject.SetActive(false); // Hide Tablet UI
    }

    IEnumerator ShowFinalUI()
    {
        if (finalUIPanel == null) yield break;

        finalUIPanel.SetActive(true); // Show final UI

        // Play final voiceover
        if (finalVoiceover != null)
        {
            finalVoiceover.Play();
            yield return new WaitForSeconds(finalVoiceover.clip.length);
        }
    }

    // This function is triggered when the UI button is clicked
    public void ProceedToNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
