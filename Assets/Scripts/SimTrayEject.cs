using UnityEngine;
using System.Collections;

public class SimTrayEject : MonoBehaviour
{
    [Header("Tray Settings")]
    public Transform tray;
    public Vector3 slideOffset = new Vector3(0.1f, 0, 0);
    public float slideSpeed = 1f;

    [Header("Projector Task Manager")]
    public ProjectorTaskManager projectorTaskManager;

    [Tooltip("Index of the SIM tray eject task in the projector task list.")]
    public int simTrayTaskIndex = 0;
    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isEjecting = false;

    void Start()
    {
        closedPosition = tray.localPosition;
        openPosition = closedPosition + slideOffset;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("Pin") || other.CompareTag("Player")) && !isEjecting)
        {
            isEjecting = true;
            StartCoroutine(SlideTray());

            if (projectorTaskManager != null)
            {
                projectorTaskManager.MarkTaskComplete(simTrayTaskIndex);
            }
        }
    }

    IEnumerator SlideTray()
    {
        float elapsedTime = 0f;
        Vector3 startingPos = tray.localPosition;
        float journeyLength = Vector3.Distance(startingPos, openPosition);

        while (elapsedTime < journeyLength / slideSpeed)
        {
            tray.localPosition = Vector3.Lerp(startingPos, openPosition, (elapsedTime * slideSpeed) / journeyLength);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        tray.localPosition = openPosition;
    }
}
