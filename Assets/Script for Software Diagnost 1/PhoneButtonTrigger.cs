using UnityEngine;
using UnityEngine.InputSystem;

public class PhoneButtonTriggerInput : MonoBehaviour
{
    public enum ButtonType { VolumeUp, VolumeDown, Power }
    public ButtonType buttonType;

    public PhoneUIStateManager uiManager;
    public InputActionReference triggerAction;

    private bool handInside = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            handInside = true;
            triggerAction.action.performed += OnTriggerPressed;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            handInside = false;
            triggerAction.action.performed -= OnTriggerPressed;
        }
    }

    private void OnTriggerPressed(InputAction.CallbackContext context)
    {
        if (!handInside) return;

        Debug.Log("🔘 Trigger pressed on: " + buttonType);

        switch (buttonType)
        {
            case ButtonType.VolumeUp:
                uiManager.PressVolumeUp();
                break;
            case ButtonType.VolumeDown:
                uiManager.PressVolumeDown();
                break;
            case ButtonType.Power:
                uiManager.PressPowerButton();
                break;
        }
    }

    private void OnDisable()
    {
        triggerAction.action.performed -= OnTriggerPressed;
    }
}
