using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SequentialSocketControl : MonoBehaviour
{
    public XRSocketInteractor motherboardSocket;
    public XRSocketInteractor screenSocket;

    void Start()
    {
        // Initially disable the motherboard socket
        motherboardSocket.enabled = false;

        // Listen for screen detachment
        screenSocket.selectExited.AddListener(OnScreenRemoved);
    }

    void OnScreenRemoved(SelectExitEventArgs args)
    {
        // Enable the motherboard socket when the screen is removed
        motherboardSocket.enabled = true;
    }

    void OnDestroy()
    {
        // Clean up event listener
        screenSocket.selectExited.RemoveListener(OnScreenRemoved);
    }
}
