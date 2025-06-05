using UnityEngine;

public class EnableAnimatorOnSuction : MonoBehaviour
{
    private Animator animator;
    private bool hasTriggered = false;

    [Header("Animation Settings")]
    public string animationName = "ScreenLift";

    [Header("Manager Reference")]
    public ScreenUnlockManager unlockManager;

    [Header("Screw Group Requirement")]
    public ScrewGroupManager screwGroupManager; // ✅ Assign in Inspector

    [Header("Trigger Tag")]
    public string triggerTag = "SuctionTool";

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator != null)
            animator.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered || animator == null)
            return;

        if (other.CompareTag(triggerTag))
        {
            // ✅ Block suction if pentalobe screws are not yet removed
            if (screwGroupManager != null && !screwGroupManager.IsPentalobeGroupDone())
            {
                Debug.LogWarning("⛔ Cannot trigger suction animation. Pentalobe screws not yet completed.");
                return;
            }

            hasTriggered = true;
            animator.enabled = true;
            animator.Play(animationName, 0, 0f);

            float duration = animator.GetCurrentAnimatorStateInfo(0).length;
            if (duration <= 0f) duration = 1f;

            Invoke(nameof(NotifyManager), duration);
        }
    }

    private void NotifyManager()
    {
        if (unlockManager != null)
            unlockManager.MarkScreenDone();
    }
}
