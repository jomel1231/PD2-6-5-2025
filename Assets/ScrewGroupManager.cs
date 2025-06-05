using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrewGroupManager : MonoBehaviour
{
    [Header("Pentalobe Group")]
    public List<ScrewUnscrew> pentalobeScrews = new List<ScrewUnscrew>();
    public int pentalobeTaskIndex = 0;

    [Header("Tri-point Group")]
    public List<ScrewUnscrew> triPointScrews = new List<ScrewUnscrew>();
    public int triPointTaskIndex = 1;

    [Header("Task Tracker")]
    public ProjectorTaskManager projectorTaskManager;

    private int pentalobeProgress = 0;
    private int triPointProgress = 0;

    private bool pentalobeComplete = false;
    private bool triPointComplete = false;

    void Start()
    {
        foreach (var screw in pentalobeScrews)
        {
            if (screw != null)
            {
                screw.genericManagerToNotify = this;
                screw.IsScrewedIn = false;
            }
        }

        foreach (var screw in triPointScrews)
        {
            if (screw != null)
            {
                screw.genericManagerToNotify = this;
                screw.IsScrewedIn = false;
            }
        }
    }

    public void NotifyScrewUnscrewed(GameObject screwObj)
    {
        foreach (var screw in pentalobeScrews)
        {
            if (screw.gameObject == screwObj)
            {
                pentalobeProgress++;
                if (!pentalobeComplete && pentalobeProgress >= pentalobeScrews.Count)
                {
                    pentalobeComplete = true;
                    projectorTaskManager?.MarkTaskComplete(pentalobeTaskIndex);
                    StartCoroutine(HideScrewGroupWithDelay(pentalobeScrews));
                }
                return;
            }
        }

        foreach (var screw in triPointScrews)
        {
            if (screw.gameObject == screwObj)
            {
                triPointProgress++;
                if (!triPointComplete && triPointProgress >= triPointScrews.Count)
                {
                    triPointComplete = true;
                    projectorTaskManager?.MarkTaskComplete(triPointTaskIndex);
                    StartCoroutine(HideScrewGroupWithDelay(triPointScrews));
                }
                return;
            }
        }
    }

    private IEnumerator HideScrewGroupWithDelay(List<ScrewUnscrew> screws)
    {
        yield return new WaitForSeconds(2f);

        foreach (var screw in screws)
        {
            if (screw != null)
                screw.gameObject.SetActive(false);
        }
    }

    public bool IsTriPointGroupDone() => triPointComplete;
    public bool IsPentalobeGroupDone() => pentalobeComplete;
}
