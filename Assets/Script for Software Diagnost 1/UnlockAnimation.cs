using UnityEngine;

public class UnlockAnimation : MonoBehaviour
{
    public float duration = 0.5f;               // Slide duration
    public float slideDistance = 0.5f;          // Distance to move upward
    public GameObject appScreen;                // Assign UI with application
    public PhoneUIStateManager uiManager;

    private Vector3 startPos;
    private Vector3 endPos;
    private float timer;
    private bool isAnimating = false;

    void Start()
    {
        startPos = transform.localPosition;
        endPos = startPos + Vector3.up * slideDistance;
    }

    public void PlayUnlockAnimation()
    {
        if (!isAnimating)
        {
            timer = 0f;
            isAnimating = true;
        }
    }

    void Update()
    {
        if (!isAnimating) return;

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / duration);
        transform.localPosition = Vector3.Lerp(startPos, endPos, t);

        if (t >= 1f)
        {
            isAnimating = false;

            // After animation, switch to app screen
            gameObject.SetActive(false); // Hide lock screen
            if (appScreen != null) appScreen.SetActive(true);
            if (uiManager != null) uiManager.ForceStateToApp();
        }
    }
}
