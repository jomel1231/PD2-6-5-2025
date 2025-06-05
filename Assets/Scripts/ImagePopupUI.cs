using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImagePopupUI : MonoBehaviour
{
    public Canvas imageCanvas; // The separate UI Canvas
    public Image uiImage; // The UI Image inside the canvas
    public float fadeDuration = 1f; // Time to fade in/out

    void Start()
    {
        // Hide the entire UI Canvas at the start
        if (imageCanvas != null)
        {
            imageCanvas.gameObject.SetActive(false);
        }
        
        // Hide the image inside the canvas
        if (uiImage != null)
        {
            SetImageAlpha(0);
        }
    }

    public void ShowImage(Sprite image, float displayTime = 5f)
    {
        if (imageCanvas != null)
        {
            imageCanvas.gameObject.SetActive(true); // Show the Canvas
        }

        if (uiImage != null)
        {
            uiImage.sprite = image; // Set the image
            StartCoroutine(FadeImage(1)); // Fade in
        }

        StartCoroutine(HideAfterDelay(displayTime)); // Hide after set time
    }

    IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(FadeImage(0)); // Fade out

        yield return new WaitForSeconds(fadeDuration);

        // Hide canvas completely after fade out
        if (imageCanvas != null)
        {
            imageCanvas.gameObject.SetActive(false);
        }
    }

    IEnumerator FadeImage(float targetAlpha)
    {
        float startAlpha = uiImage.color.a;
        float elapsedTime = 0;
        Color color = uiImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            uiImage.color = color;
            yield return null;
        }
    }

    void SetImageAlpha(float alpha)
    {
        Color color = uiImage.color;
        color.a = alpha;
        uiImage.color = color;
    }
}
