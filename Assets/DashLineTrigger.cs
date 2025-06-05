using UnityEngine;

public class DashLineTrigger : MonoBehaviour
{
    public DashLineGroupManager manager;
    private bool cleared = false;

    void OnTriggerEnter(Collider other)
    {
        if (cleared) return;

        // Clear if touched by Spudger or Glue
        if (other.CompareTag("Spudger") || other.CompareTag("Glue"))
        {
            cleared = true;
            gameObject.SetActive(false); // Hide this dash line
            manager.RegisterDashCleared(this);
        }
    }

    public void ResetDash()
    {
        cleared = false;
        gameObject.SetActive(true);
    }
}
