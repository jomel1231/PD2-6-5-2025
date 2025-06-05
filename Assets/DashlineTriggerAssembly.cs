using UnityEngine;

public class DashLineTriggerAssembly : MonoBehaviour
{
    public DashlineGroupManagerAssembly manager;
    private bool isCleared = false;

    public void ResetDash()
    {
        isCleared = false;
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCleared || manager == null)
            return;

        // Only allow trigger if correct tag (e.g., Glue) and validated by manager
        if (manager.IsValidTrigger(other))
        {
            isCleared = true;
            gameObject.SetActive(false); // Hide dash guideline
            manager.RegisterDashCleared(this);
        }
    }
}
