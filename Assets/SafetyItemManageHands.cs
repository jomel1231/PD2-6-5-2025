using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SafetyItemManageHands: MonoBehaviour
{
    [Header("Grabbables")]
    public XRGrabInteractable gloves;
    public XRGrabInteractable wristStrap;

    [Header("Hand Appearance")]
    public GameObject leftHandModel;
    public GameObject rightHandModel;
    public Material glovesMaterial;

    [Header("Next Button")]
    public GameObject nextButton;

    private bool glovesGrabbed = false;
    private bool strapGrabbed = false;

    void Start()
    {
        if (nextButton != null)
            nextButton.SetActive(false);
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

        // Immediately change hand material on glove grab
        ChangeHandMaterial(leftHandModel);
        ChangeHandMaterial(rightHandModel);

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

    void ChangeHandMaterial(GameObject hand)
    {
        if (hand != null)
        {
            Renderer rend = hand.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material = glovesMaterial;
            }
            else
            {
                SkinnedMeshRenderer skinnedRend = hand.GetComponent<SkinnedMeshRenderer>();
                if (skinnedRend != null)
                    skinnedRend.material = glovesMaterial;
            }
        }
    }
}