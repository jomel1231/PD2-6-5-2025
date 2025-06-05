using UnityEngine;

public class SolderMachineController : MonoBehaviour
{
    public Transform button;            // Button visual
    public AudioSource audioSource;     // Audio to play when toggled on

    private Vector3 buttonInitialPosition;
    private Vector3 buttonPressedPosition;
    private bool audioActive = false;
    private bool isAnimating = false;

    public float buttonMoveDistance = 0.02f;
    public float animationDuration = 0.2f;

    private void Start()
    {
        buttonInitialPosition = button.localPosition;
        buttonPressedPosition = buttonInitialPosition - new Vector3(0, buttonMoveDistance, 0);

        if (audioSource != null)
            audioSource.Stop(); // make sure it's off initially
    }

    public void OnButtonPressed()
    {
        if (isAnimating)
            return;

        audioActive = !audioActive;

        if (audioSource != null)
        {
            if (audioActive && !audioSource.isPlaying)
                audioSource.Play();
            else if (!audioActive && audioSource.isPlaying)
                audioSource.Stop();
        }

        StopAllCoroutines();
        Vector3 targetPosition = audioActive ? buttonPressedPosition : buttonInitialPosition;
        StartCoroutine(AnimateButton(button.localPosition, targetPosition));
    }

    private System.Collections.IEnumerator AnimateButton(Vector3 from, Vector3 to)
    {
        isAnimating = true;
        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            button.localPosition = Vector3.Lerp(from, to, elapsed / animationDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        button.localPosition = to;
        isAnimating = false;
    }
}
