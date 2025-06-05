using UnityEngine;

public class EnableAnimationOnSpudgerBatt : MonoBehaviour
{
    private Animator animator;
    private bool hasTriggered = false;

    [Header("Animation Settings")]
    public string animationName = "CablePop";

    [Header("Manager Reference")]
    public BatteryUnlockManager unlockManager;

    [Header("Trigger Tag")]
    public string triggerTag = "Spudger";

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
            hasTriggered = true;
            animator.enabled = true;
            animator.Play(animationName, 0, 0f);

            float duration = animator.GetCurrentAnimatorStateInfo(0).length;
            if (duration <= 0f) duration = 1f;

            Invoke(nameof(NotifyManager), duration); // Call after animation is done
        }
    }

    private void NotifyManager()
    {
        if (unlockManager != null)
            unlockManager.MarkCableDone(); // ✅ Correct call with no arguments
    }
}
