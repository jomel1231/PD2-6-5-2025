using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabHandPose : MonoBehaviour
{
    public HandData rightHandPose; // Reference hand pose for grabbing

    private Vector3 startingHandPosition;
    private Vector3 finalHandPosition;
    private Quaternion startingHandRotation;
    private Quaternion finalHandRotation;

    private Quaternion[] startingFingerRotations;
    private Quaternion[] finalFingerRotations;

    void Start()
    {
        XRGrabInteractable grabInteractable = GetComponent<XRGrabInteractable>();

        grabInteractable.selectEntered.AddListener(SetupPose);
        grabInteractable.selectExited.AddListener(UnSetPose);

        rightHandPose.gameObject.SetActive(false); // Hide reference hand at start
    }

    public void SetupPose(BaseInteractionEventArgs arg)
    {
        if (arg.interactorObject is XRBaseControllerInteractor controllerInteractor && controllerInteractor != null)
        {
            HandData handData = controllerInteractor.xrController.transform.GetComponentInChildren<HandData>();

            if (handData != null)
            {
                handData.animator.enabled = false; // Disable animator to allow manual positioning

                // Move the hand to match the exact position of the reference pose
                handData.root.position = rightHandPose.root.position;
                handData.root.rotation = rightHandPose.root.rotation;

                // Set hand data values and apply pose
                SetHandDataValues(handData, rightHandPose, controllerInteractor.transform);
                SendHandData(handData, finalHandPosition, finalHandRotation, finalFingerRotations);
            }
        }
    }

    public void UnSetPose(BaseInteractionEventArgs arg)
    {
        if (arg.interactorObject is XRBaseControllerInteractor controllerInteractor && controllerInteractor != null)
        {
            HandData handData = controllerInteractor.xrController.transform.GetComponentInChildren<HandData>();

            if (handData != null)
            {
                handData.animator.enabled = true; // Re-enable hand animator

                // Reset hand to original pose
                SendHandData(handData, startingHandPosition, startingHandRotation, startingFingerRotations);
            }
        }
    }

    public void SetHandDataValues(HandData h1, HandData h2, Transform interactorTransform)
    {
        // Adjust for interactor transform to avoid incorrect positioning
        startingHandPosition = interactorTransform.InverseTransformPoint(h1.root.position);
        finalHandPosition = interactorTransform.InverseTransformPoint(h2.root.position);
       
        startingHandRotation = h1.root.localRotation;
        finalHandRotation = h2.root.localRotation;

        startingFingerRotations = new Quaternion[h1.fingerBones.Length];
        finalFingerRotations = new Quaternion[h1.fingerBones.Length];

        for (int i = 0; i < h1.fingerBones.Length; i++)
        {
            startingFingerRotations[i] = h1.fingerBones[i].localRotation;
            finalFingerRotations[i] = h2.fingerBones[i].localRotation;
        }
    }

    public void SendHandData(HandData h, Vector3 newPosition, Quaternion newRotation, Quaternion[] newBonesRotation)
    {
        h.root.localPosition = newPosition;
        h.root.localRotation = newRotation;

        for (int i = 0; i < newBonesRotation.Length; i++)
        {
            h.fingerBones[i].localRotation = newBonesRotation[i];
        }
    }
}
