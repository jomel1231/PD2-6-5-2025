using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class WrongInteractionWarning_StepBased : MonoBehaviour
{
    [Header("UI Panel")]
    public GameObject warningPanel;

    [Header("Warning Sound")]
    public AudioSource warningSound;

    [Header("Projector Task Reference")]
    public ProjectorTaskManager projectorTaskManager;

    [Header("Enable After All Steps")]
    public GameObject objectToEnableAfterAllStepsDone;

    [System.Serializable]
    public class StepInteractable
    {
        public int taskIndex;
        public XRBaseInteractable[] allowedObjects;
    }

    public StepInteractable[] stepInteractables;

    private bool panelVisible = false;
    private bool hasEnabledObject = false;

    private void Start()
    {
        if (warningPanel != null)
            warningPanel.SetActive(false);

        if (objectToEnableAfterAllStepsDone != null)
            objectToEnableAfterAllStepsDone.SetActive(false);

        foreach (var step in stepInteractables)
        {
            foreach (var obj in step.allowedObjects)
            {
                if (obj != null)
                {
                    obj.selectEntered.AddListener(OnGrabbed);
                    obj.selectExited.AddListener(OnReleased);
                }
            }
        }
    }

    private void Update()
    {
        if (!hasEnabledObject && AllStepInteractablesComplete())
        {
            hasEnabledObject = true;

            if (objectToEnableAfterAllStepsDone != null)
                objectToEnableAfterAllStepsDone.SetActive(true);

            gameObject.SetActive(false); // ✅ Disable this GameObject
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        // ✅ Skip warning check if object is being selected by a socket
        if (args.interactorObject is XRSocketInteractor)
            return;

        StartCoroutine(DelayedCheck(args));
    }

    private IEnumerator DelayedCheck(SelectEnterEventArgs args)
    {
        yield return new WaitForSeconds(0.05f); // Short delay helps avoid false triggers

        var interactable = args.interactable;
        int currentStep = GetCurrentTaskIndex();

        if (!IsAllowed(interactable, currentStep))
        {
            if (!panelVisible && warningPanel != null)
            {
                warningPanel.SetActive(true);
                panelVisible = true;

                if (warningSound != null && !warningSound.isPlaying)
                    warningSound.Play();
            }
        }
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        if (panelVisible && warningPanel != null)
        {
            warningPanel.SetActive(false);
            panelVisible = false;

            if (warningSound != null && warningSound.isPlaying)
                warningSound.Stop();
        }
    }

    private int GetCurrentTaskIndex()
    {
        if (projectorTaskManager == null || projectorTaskManager.taskList == null)
            return -1;

        for (int i = 0; i < projectorTaskManager.taskList.Count; i++)
        {
            if (!projectorTaskManager.taskList[i].isComplete)
                return i;
        }
        return -1;
    }

    private bool IsAllowed(XRBaseInteractable interactable, int step)
    {
        foreach (var stepDef in stepInteractables)
        {
            if (stepDef.taskIndex == step)
            {
                foreach (var allowed in stepDef.allowedObjects)
                {
                    if (interactable == allowed)
                        return true;
                }
            }
        }
        return false;
    }

    private bool AllStepInteractablesComplete()
    {
        if (projectorTaskManager == null) return false;

        foreach (var stepDef in stepInteractables)
        {
            if (!projectorTaskManager.IsTaskComplete(stepDef.taskIndex))
                return false;
        }

        return true;
    }
}
