using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[System.Serializable]
public class AssemblyStep
{
    public string stepName;

    public XRGrabInteractable partToGrab;
    public XRSocketInteractor targetSocket;

    public GameObject[] screwsToEnable; // Screw GameObjects (disabled initially)
}
