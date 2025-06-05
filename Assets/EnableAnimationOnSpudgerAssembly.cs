using UnityEngine;

public class EnableAnimationOnSpudgerAssembly : MonoBehaviour
{
    private Animator animator;
    private bool hasTriggered = false;

    [Header("Animation Settings")]
    public string animationName = "CablePop";

    [Header("Trigger Tag")]
    public string triggerTag = "Spudger";

    [Header("Dependency")]
    public SocketStepManager socketStepManager; // ✅ Assign in Inspector

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

        // ✅ Block if step 2 is not done or any shield is socketed
        if (!IsStep2Done() || IsShieldSocketed())
        {
            Debug.Log("⛔ Cannot trigger animation: Step 2 not done or shield already socketed.");
            return;
        }

        if (other.CompareTag(triggerTag))
        {
            hasTriggered = true;
            animator.enabled = true;
            animator.Play(animationName, 0, 0f);

            float duration = animator.GetCurrentAnimatorStateInfo(0).length;
            if (duration <= 0f) duration = 1f;

            // Optional follow-up call after animation if needed
        }
    }

    private bool IsStep2Done()
    {
        return socketStepManager != null && socketStepManager.IsStepDone(1);
    }

    private bool IsShieldSocketed()
    {
        if (socketStepManager == null || socketStepManager.shieldSockets == null)
            return false;

        foreach (var socket in socketStepManager.shieldSockets)
        {
            if (socket != null && socket.hasSelection)
                return true; // ❌ At least one shield is socketed
        }
        return false; // ✅ No shields are socketed
    }
}
