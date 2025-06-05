using System.Collections;
using UnityEngine;
using TMPro;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager _instance;
    public TextMeshProUGUI textComponent; // Ensure you assign this in the Inspector
    public Canvas tooltipCanvas; // Assign a world-space Canvas to this

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {
        gameObject.SetActive(false);
    }

    public void SetAndShowTooltip(string message, Transform targetTransform)
    {
        gameObject.SetActive(true);
        textComponent.text = message;

        // Position the tooltip slightly above the target
        Vector3 tooltipPosition = targetTransform.position + Vector3.up * 0.2f;
        tooltipCanvas.transform.position = tooltipPosition;
        tooltipCanvas.transform.LookAt(Camera.main.transform);
        tooltipCanvas.transform.Rotate(0, 180, 0); // Ensure the tooltip faces the camera
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
        textComponent.text = string.Empty;
    }
}
