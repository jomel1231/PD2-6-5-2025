using UnityEngine;
using UnityEngine.InputSystem;

public class PhoneScreenTriggerUnlock : MonoBehaviour
{
    public PhoneUIStateManager uiManager;
    public InputActionReference triggerAction;

    private bool isHandHovering = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isHandHovering = true;
            triggerAction.action.performed += OnTriggerPressed;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isHandHovering = false;
            triggerAction.action.performed -= OnTriggerPressed;
        }
    }

    private void OnTriggerPressed(InputAction.CallbackContext context)
    {
        if (isHandHovering)
        {
            Debug.Log("🟢 Trigger pressed on screen — unlocking");
            uiManager.SwipeToUnlock();
        }
    }

    private void OnDisable()
    {
        triggerAction.action.performed -= OnTriggerPressed;
    }
}
