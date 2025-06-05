using UnityEngine;
using System.Collections.Generic;

public class MetalShieldGrabManager : MonoBehaviour
{
    [Header("Metal Shields to Track")]
    public List<MetalShield> metalShields = new List<MetalShield>();

    [Header("Task Tracking")]
    public ProjectorTaskManager projectorTaskManager;
    public int shieldTaskIndex = 0;

    [Header("Screw Group Dependency")]
    public ScrewGroupManager screwGroupManager;

    private HashSet<MetalShield> releasedShields = new HashSet<MetalShield>();
    private bool shieldsUnlocked = false;
    private bool taskMarked = false;

    void Start()
    {
        foreach (var shield in metalShields)
        {
            if (shield != null)
                shield.enabled = false;
        }
    }

    void Update()
    {
        if (!shieldsUnlocked && screwGroupManager != null && screwGroupManager.IsTriPointGroupDone())
        {
            UnlockShields();
        }
    }

    private void UnlockShields()
    {
        shieldsUnlocked = true;

        foreach (var shield in metalShields)
        {
            if (shield != null)
                shield.enabled = true;
        }
    }

    public void NotifyShieldReleased(MetalShield shield)
    {
        if (taskMarked || !shieldsUnlocked || shield == null || releasedShields.Contains(shield))
            return;

        releasedShields.Add(shield);

        if (releasedShields.Count >= metalShields.Count)
        {
            taskMarked = true;
            projectorTaskManager?.MarkTaskComplete(shieldTaskIndex);
        }
    }

    // ✅ Called from other scripts to check if task is done
    public bool AreShieldsCompleted()
    {
        return taskMarked;
    }
}
