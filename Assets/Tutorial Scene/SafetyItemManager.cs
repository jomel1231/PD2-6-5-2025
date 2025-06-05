using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SafetyItemGrabManagerTwoHands : MonoBehaviour
{
    [Header("Grabbables")]
    public XRGrabInteractable gloves;
    public XRGrabInteractable wristStrap;

    [Header("Hand Models")]
    public GameObject leftHandDefault;
    public GameObject rightHandDefault;
    public GameObject leftHandGloved;
    public GameObject rightHandGloved;

    [Header("Next Button")]
    public GameObject nextButton;

    private bool glovesGrabbed = false;
    private bool strapGrabbed = false;

    void Start()
    {
        if (nextButton != null)
            nextButton.SetActive(false);

        // Ensure only default hands are active initially
        if (leftHandGloved != null) leftHandGloved.SetActive(false);
        if (rightHandGloved != null) rightHandGloved.SetActive(false);
        if (leftHandDefault != null) leftHandDefault.SetActive(true);
        if (rightHandDefault != null) rightHandDefault.SetActive(true);
    }

    void OnEnable()
    {
        gloves.selectEntered.AddListener(OnGlovesGrabbed);
        wristStrap.selectEntered.AddListener(OnStrapGrabbed);
    }

    void OnDisable()
    {
        gloves.selectEntered.RemoveListener(OnGlovesGrabbed);
        wristStrap.selectEntered.RemoveListener(OnStrapGrabbed);
    }

    void OnGlovesGrabbed(SelectEnterEventArgs args)
    {
        glovesGrabbed = true;
        gloves.gameObject.SetActive(false);

        // Swap hand models
        if (leftHandDefault != null) leftHandDefault.SetActive(false);
        if (rightHandDefault != null) rightHandDefault.SetActive(false);
        if (leftHandGloved != null) leftHandGloved.SetActive(true);
        if (rightHandGloved != null) rightHandGloved.SetActive(true);

        CheckBothGrabbed();
    }

    void OnStrapGrabbed(SelectEnterEventArgs args)
    {
        strapGrabbed = true;
        wristStrap.gameObject.SetActive(false);

        CheckBothGrabbed();
    }

    void CheckBothGrabbed()
    {
        if (glovesGrabbed && strapGrabbed)
        {
            if (nextButton != null)
                nextButton.SetActive(true);

            Debug.Log("✅ Safety gear equipped. Next button is now visible.");
        }
    }
}
