using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PentalobeGroupAssembly : MonoBehaviour
{
    [Header("Pentalobe Screws")]
    public List<ScrewUnscrewAssembly> pentalobeScrews;
    public int projectorTaskIndex;

    [Header("Projector Reference")]
    public ProjectorTaskManager projectorTaskManager;

    private bool groupComplete = false;
    private bool cooldown = false;
    private int screwsTriggered = 0;

    public void OnScrewTriggered(ScrewUnscrewAssembly screw)
    {
        if (!pentalobeScrews.Contains(screw)) return;

        if (cooldown)
        {
            // Reverse all screws
            foreach (var s in pentalobeScrews)
            {
                s.Animate(false);
                s.HasBeenTriggered = false;
                s.IsScrewedIn = false;
            }

            screwsTriggered = 0;
            groupComplete = false;
            cooldown = false;
            projectorTaskManager?.MarkTaskIncomplete(projectorTaskIndex);
            return;
        }

        if (!screw.HasBeenTriggered)
        {
            screw.Animate(true);
            screw.HasBeenTriggered = true;
            screw.IsScrewedIn = true;
            screwsTriggered++;

            if (screwsTriggered >= pentalobeScrews.Count && !groupComplete)
            {
                groupComplete = true;
                projectorTaskManager?.MarkTaskComplete(projectorTaskIndex);
                StartCoroutine(SetCooldownAfterDelay());
            }
        }
    }

    private IEnumerator SetCooldownAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        cooldown = true;
    }
    public bool IsGroupDone() => groupComplete;

}
