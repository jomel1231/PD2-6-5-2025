using UnityEngine;

public class ScrewManager : MonoBehaviour
{
    public static ScrewManager Instance;
    public ProjectorTaskManager projectorTaskManager;
    public int chargingPortTaskIndex = 0;
    public int screwsRequired = 2;

    private int screwsUnscrewed = 0;

    void Awake()
    {
        Instance = this;
        Debug.Log("ScrewManager Awake called");
    }

    public void ScrewUnscrewed()
    {
        screwsUnscrewed++;
        Debug.Log("Screw Unscrewed: total now " + screwsUnscrewed);

        if (screwsUnscrewed >= screwsRequired)
        {
            Debug.Log("Screws required reached. Now marking task complete explicitly.");
            projectorTaskManager.MarkTaskComplete(chargingPortTaskIndex);
        }
    }
}
