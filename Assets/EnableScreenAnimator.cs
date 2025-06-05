using UnityEngine;
using UnityEngine.UI;

public class ScreenAnimatorEnabler : MonoBehaviour
{
    [Header("Screen With Animator")]
    public GameObject screenObject;

    [Header("UI Button")]
    public Button triggerButton;

    [Header("Optional: Animator Trigger Name")]
    public string animationTrigger = "ActivateScreen";

    private Animator screenAnimator;

    void Start()
    {
        if (screenObject != null)
        {
            screenAnimator = screenObject.GetComponent<Animator>();
        }

        if (triggerButton != null)
        {
            triggerButton.onClick.AddListener(EnableAnimator);
        }
    }

    public void EnableAnimator()
    {
        if (screenAnimator != null)
        {
            screenAnimator.enabled = true;

            if (!string.IsNullOrEmpty(animationTrigger))
            {
                screenAnimator.SetTrigger(animationTrigger);
            }
        }
    }
}
