using UnityEngine;

public class DualStickTriggerManager : MonoBehaviour
{
    [Header("Trigger Spots")]
    public Collider stick1Target; // Assign the designated collider for Stick 1
    public Collider stick2Target; // Assign the designated collider for Stick 2

    [Header("Sticks")]
    public string stick1Tag = "Stick1"; // Tag your stick GameObjects with "Stick1"
    public string stick2Tag = "Stick2"; // Tag your stick GameObjects with "Stick2"

    [Header("Canvas Switch")]
    public GameObject canvasToHide;   // The one currently shown
    public GameObject canvasToShow;   // The one to enable when both are triggered

    private bool stick1InPlace = false;
    private bool stick2InPlace = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(stick1Tag) && stick1Target.bounds.Contains(other.transform.position))
        {
            stick1InPlace = true;
        }

        if (other.CompareTag(stick2Tag) && stick2Target.bounds.Contains(other.transform.position))
        {
            stick2InPlace = true;
        }

        CheckBothSticks();
    }

    private void CheckBothSticks()
    {
        if (stick1InPlace && stick2InPlace)
        {
            if (canvasToHide != null)
                canvasToHide.SetActive(false);

            if (canvasToShow != null)
                canvasToShow.SetActive(true);

            Debug.Log("🎯 Both sticks in position. Canvas swapped.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(stick1Tag))
            stick1InPlace = false;

        if (other.CompareTag(stick2Tag))
            stick2InPlace = false;
    }
}
