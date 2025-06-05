using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SocketMonitorStepManager : MonoBehaviour
{
    [System.Serializable]
    public class SocketStep
    {
        public string stepName;
        public XRSocketInteractor socket;
        public int projectorTaskIndex;
    }

    [Header("Socket Steps")]
    public List<SocketStep> steps = new List<SocketStep>();

    [Header("Projector Manager")]
    public ProjectorTaskManager projectorTaskManager;

    private Dictionary<XRSocketInteractor, IXRSelectInteractable> previousSelections = new();

    private void Start()
    {
        foreach (var step in steps)
        {
            if (step.socket != null)
            {
                step.socket.selectEntered.AddListener(OnObjectSocketed);
                step.socket.selectExited.AddListener(OnObjectRemoved);
                previousSelections[step.socket] = null;
            }
        }
    }

    private void OnDestroy()
    {
        foreach (var step in steps)
        {
            if (step.socket != null)
            {
                step.socket.selectEntered.RemoveListener(OnObjectSocketed);
                step.socket.selectExited.RemoveListener(OnObjectRemoved);
            }
        }
    }

    private void OnObjectSocketed(SelectEnterEventArgs args)
    {
        XRSocketInteractor socket = args.interactorObject as XRSocketInteractor;
        if (socket == null) return;

        foreach (var step in steps)
        {
            if (step.socket == socket)
            {
                IXRSelectInteractable interactable = args.interactableObject;
                if (interactable != null)
                {
                    GameObject obj = interactable.transform.gameObject;
                    projectorTaskManager.MarkTaskComplete(step.projectorTaskIndex);

                    // Disable XRGrabInteractable to lock it after correct socketing
                    XRGrabInteractable grab = obj.GetComponent<XRGrabInteractable>();
                    if (grab != null)
                        grab.enabled = false;

                    previousSelections[socket] = interactable;
                }
            }
        }
    }

    private void OnObjectRemoved(SelectExitEventArgs args)
    {
        XRSocketInteractor socket = args.interactorObject as XRSocketInteractor;
        if (socket == null) return;

        foreach (var step in steps)
        {
            if (step.socket == socket)
            {
                // Re-enable grab if it was disabled (optional)
                if (previousSelections.TryGetValue(socket, out IXRSelectInteractable prevObj) && prevObj != null)
                {
                    XRGrabInteractable grab = prevObj.transform.GetComponent<XRGrabInteractable>();
                    if (grab != null)
                        grab.enabled = true;
                }

                projectorTaskManager.MarkTaskIncomplete(step.projectorTaskIndex);
                previousSelections[socket] = null;
            }
        }
    }
}
