using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class StartSafetyGrab : MonoBehaviour
{
    public GameObject glovesObject;
    public GameObject wristStrapObject;

    public void EnableSafetyItems()
    {
        glovesObject.SetActive(true);
        wristStrapObject.SetActive(true);
    }
}
