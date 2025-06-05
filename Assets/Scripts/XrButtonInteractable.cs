using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;
using System; // Needed for Action

public class XrButtonInteractable : XRSimpleInteractable
{
    [SerializeField] private Image buttonImage;
    [SerializeField] private Color[] buttonColors = new Color[4];

    public Action onButtonPressed; // Event for button press

    private Color normalColor;
    private Color highlightedColor;
    private Color pressedColor;
    private Color selectedColor;
    private bool isPressed = false;

    private void Start()
    {
        if (buttonColors.Length >= 4)
        {
            normalColor = buttonColors[0];
            highlightedColor = buttonColors[1];
            pressedColor = buttonColors[2];
            selectedColor = buttonColors[3];
        }
        else
        {
            Debug.LogError("Button Colors array is not properly set up!");
        }

        if (buttonImage != null)
        {
            buttonImage.color = normalColor;
        }
    }

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);
        if (!isPressed && buttonImage != null)
        {
            buttonImage.color = highlightedColor;
        }
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);
        if (!isPressed && buttonImage != null)
        {
            buttonImage.color = normalColor;
        }
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        isPressed = true;
        if (buttonImage != null)
        {
            buttonImage.color = pressedColor;
        }

        // ✅ Trigger the button action when pressed
        onButtonPressed?.Invoke(); 
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        isPressed = false; // Reset press state when released
        if (buttonImage != null)
        {
            buttonImage.color = selectedColor;
        }
    }
}
