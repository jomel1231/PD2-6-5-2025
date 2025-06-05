using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SocketCompletionManager : MonoBehaviour
{
    [Header("Socket to Track")]
    public XRSocketInteractor socket; // Assign the XR Socket Interactor in Inspector

    [Header("Task Tracking")]
    public ProjectorTaskManager projectorTaskManager;
    public int taskIndex = 0;

    private bool taskMarked = false;

    void OnEnable()
    {
        if (socket != null)
            socket.selectEntered.AddListener(OnObjectSocketed);
    }

    void OnDisable()
    {
        if (socket != null)
            socket.selectEntered.RemoveListener(OnObjectSocketed);
    }

    private void OnObjectSocketed(SelectEnterEventArgs args)
    {
        if (!taskMarked && projectorTaskManager != null)
        {
            taskMarked = true;
            projectorTaskManager.MarkTaskComplete(taskIndex);
        }
    }
}
