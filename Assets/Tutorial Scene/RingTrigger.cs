using UnityEngine;

public class RingTrigger : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Ring Object (Optional Visual)")]
    public GameObject ringVisual;

    [Header("Next Ring (To Activate When All Tasks Complete)")]
    public GameObject nextRing;

    [Header("Object To Enable When Next Ring Is Triggered")]
    public GameObject objectToEnable;

    [Header("Projector To Monitor")]
    public ProjectorTaskManager projectorToMonitor;

    [Header("Assessment Timer Reference")]
    public ProgressTracker progressTracker;

    [Header("Bootloop Button Reference")]
    public BootloopButton bootloopButton; // 🔹 New field

    private bool triggered = false;
    private bool waitingForCompletion = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered || !other.CompareTag("Player")) return;
        triggered = true;

        // 🔊 Play sound immediately
        if (audioSource != null)
            audioSource.Play();

        // 🎯 Hide ring visual or object
        if (ringVisual != null)
            ringVisual.SetActive(false);
        else
            gameObject.SetActive(false);

        // 🕒 Start the assessment timer (if assigned)
        if (progressTracker != null)
            progressTracker.StartAssessmentTimer();

        // ⏱ Start the bootloop timer (if assigned)
        if (bootloopButton != null)
            bootloopButton.StartBootloopTimer();

        // ✅ Continue if tasks are complete or wait
        if (projectorToMonitor != null)
        {
            if (AreAllTasksComplete())
            {
                ActivateNextRing();
            }
            else
            {
                waitingForCompletion = true;
            }
        }
        else
        {
            ActivateNextRing();
        }
    }

    private void Update()
    {
        if (waitingForCompletion && AreAllTasksComplete())
        {
            ActivateNextRing();
            waitingForCompletion = false;
        }
    }

    private bool AreAllTasksComplete()
    {
        foreach (var task in projectorToMonitor.taskList)
        {
            if (!task.isComplete)
                return false;
        }
        return true;
    }

    private void ActivateNextRing()
    {
        if (nextRing != null)
            nextRing.SetActive(true);

        if (objectToEnable != null)
            objectToEnable.SetActive(true);
    }
}
