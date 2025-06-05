using UnityEngine;

public class SafetyStageStarter : MonoBehaviour
{
    [Header("UI and Objects to Toggle")]
    public GameObject firstCanvas;       // Canvas 1 (initial instructions)
    public GameObject secondCanvas;      // Canvas 2 (gloves/wrist strap instructions)
    public GameObject glovesObject;      // Gloves object (disabled initially)
    public GameObject wristStrapObject;  // Wrist strap object (disabled initially)

    public void ProceedToNextStage()
    {
        // Hide Canvas 1
        if (firstCanvas != null)
            firstCanvas.SetActive(false);

        // Show Canvas 2 and activate the safety objects
        if (secondCanvas != null)
            secondCanvas.SetActive(true);

        if (glovesObject != null)
            glovesObject.SetActive(true);

        if (wristStrapObject != null)
            wristStrapObject.SetActive(true);

        Debug.Log("➡️ Proceeded to safety setup stage.");
    }
}
