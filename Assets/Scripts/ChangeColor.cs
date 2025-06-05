using UnityEngine;
using UnityEngine.UI;
using TMPro; // Import TextMeshPro namespace
using UnityEngine.EventSystems; // For event handling

public class ButtonColorChange : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Image buttonImage;
    private Text buttonText;
    private TextMeshProUGUI tmpText; // For TextMeshPro support
    private Color originalImageColor;
    private Color originalTextColor;
    private Color hoverColor = Color.green;

    private void Start()
    {
        // Get the button's Image component
        buttonImage = GetComponent<Image>();

        // Try to find either a standard Text or TextMeshProUGUI component
        buttonText = GetComponentInChildren<Text>();
        tmpText = GetComponentInChildren<TextMeshProUGUI>();

        // Store original colors
        originalImageColor = buttonImage.color;
        if (buttonText != null)
            originalTextColor = buttonText.color;
        else if (tmpText != null)
            originalTextColor = tmpText.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonImage.color = hoverColor; // Change background to green

        if (buttonText != null)
            buttonText.color = hoverColor; // Change standard Text color
        else if (tmpText != null)
            tmpText.color = hoverColor; // Change TextMeshPro text color
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonImage.color = originalImageColor; // Reset background

        if (buttonText != null)
            buttonText.color = originalTextColor; // Reset standard Text color
        else if (tmpText != null)
            tmpText.color = originalTextColor; // Reset TextMeshPro text color
    }
}
