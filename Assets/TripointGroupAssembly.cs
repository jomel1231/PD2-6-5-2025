using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriPointGroupAssembly : MonoBehaviour
{
    [Header("Tri-point Screws")]
    public List<ScrewUnscrewAssembly> triPointScrews;
    public int projectorTaskIndex;

    [Header("Projector Reference")]
    public ProjectorTaskManager projectorTaskManager;

    [Header("Assessment Monitor")]
    public AssessmentResultMonitor assessmentResultMonitor;

    private bool groupComplete = false;
    private bool cooldown = false;
    private int screwsTriggered = 0;

    public void OnScrewTriggered(ScrewUnscrewAssembly screw)
    {
        if (!triPointScrews.Contains(screw)) return;

        // ✅ If in cooldown, revert all screws and mark incomplete
        if (cooldown)
        {
            foreach (var s in triPointScrews)
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

        // ✅ Mark screw as triggered only once per forward action
        if (!screw.HasBeenTriggered)
        {
            screw.Animate(true);
            screw.HasBeenTriggered = true;
            screw.IsScrewedIn = true;
            screwsTriggered++;

            // ✅ Once all screws are triggered
            if (screwsTriggered >= triPointScrews.Count)
            {
                groupComplete = true;
                projectorTaskManager?.MarkTaskComplete(projectorTaskIndex);
                StartCoroutine(SetCooldownAfterDelay());

                // ✅ Trigger result UI immediately if in assessment
                assessmentResultMonitor?.ForceImmediateResult();
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
