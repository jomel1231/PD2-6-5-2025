using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class AdhesiveStripManager : MonoBehaviour
{
    [Header("Strips to Track")]
    public AdhesiveStrip strip1;
    public AdhesiveStrip strip2;

    [Header("Task Tracking")]
    public ProjectorTaskManager projectorTaskManager;
    public int taskIndex = 0;

    [Header("Battery Unlock")]
    public BatteryUnlockManager batteryUnlockManager; // Assign in Inspector

    private bool taskMarked = false;

    // Allows external scripts to check if the adhesive task is done
    public bool IsAdhesiveTaskDone => taskMarked;

    void Start()
    {
        // Ensure battery is locked initially (optional safeguard)
        if (batteryUnlockManager != null)
            batteryUnlockManager.CheckIfCanUnlockBattery();
    }

    public void NotifyStripReleased(AdhesiveStrip strip)
    {
        if (taskMarked) return;

        if (strip1 != null && strip2 != null && strip1.WasGrabbed() && strip2.WasGrabbed())
        {
            taskMarked = true;

            if (projectorTaskManager != null)
                projectorTaskManager.MarkTaskComplete(taskIndex);

            // Check battery unlock condition after task is marked done
            if (batteryUnlockManager != null)
                batteryUnlockManager.CheckIfCanUnlockBattery();
        }
    }
}
