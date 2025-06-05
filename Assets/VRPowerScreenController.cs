using UnityEngine;
using UnityEngine.UI;

public class VRPowerScreenController : MonoBehaviour
{
    [Header("Button Movement")]
    public Transform button;                 // Assign the 3D child object to animate
    public float buttonMoveDistance = 0.02f;
    public float animationDuration = 0.2f;

    [Header("UI Raw Images (Canvas Pages)")]
    public RawImage[] rawImages;             // Assign 4 RawImages here

    [Header("UI Button to Go to Next Page")]
    public Button nextImageButton;           // Assign the UI Button that switches images

    private Vector3 buttonInitialPosition;
    private Vector3 buttonPressedPosition;
    private bool isAnimating = false;
    private bool isActive = false;
    private int currentImageIndex = -1;

    private void Start()
    {
        buttonInitialPosition = button.localPosition;
        buttonPressedPosition = buttonInitialPosition - new Vector3(0, buttonMoveDistance, 0);

        // Disable all images at start
        DisableAllImages();

        // Assign listener to UI next button
        if (nextImageButton != null)
            nextImageButton.onClick.AddListener(NextImage);
    }

    public void OnButtonPressed()
    {
        if (isAnimating)
            return;

        isActive = !isActive;

        // Animate the button (3D press)
        StopAllCoroutines();
        Vector3 targetPos = isActive ? buttonPressedPosition : buttonInitialPosition;
        StartCoroutine(AnimateButton(button.localPosition, targetPos));

        if (!isActive)
        {
            DisableAllImages();
            currentImageIndex = -1;
        }
        else
        {
            currentImageIndex = 0;
            ShowImage(currentImageIndex);
        }
    }

    private void NextImage()
    {
        if (!isActive || rawImages.Length == 0)
            return;

        rawImages[currentImageIndex].gameObject.SetActive(false);
        currentImageIndex = (currentImageIndex + 1) % rawImages.Length;
        ShowImage(currentImageIndex);
    }

    private void ShowImage(int index)
    {
        if (index >= 0 && index < rawImages.Length)
            rawImages[index].gameObject.SetActive(true);
    }

    private void DisableAllImages()
    {
        foreach (var img in rawImages)
        {
            if (img != null)
                img.gameObject.SetActive(false);
        }
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
