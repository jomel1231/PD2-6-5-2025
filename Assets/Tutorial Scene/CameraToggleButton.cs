using UnityEngine;

public class CameraToggleButton : MonoBehaviour
{
    public Transform button;                  // Button child
    public Camera magnifyCamera;              // Magnify camera child
    public Renderer ledRenderer;              // LED Renderer
    public RenderTexture magnifyRenderTexture; // The RenderTexture you're using
    public Material blackMaterial;            // Assign a simple unlit black material here

    private Vector3 buttonInitialPosition;
    private Vector3 buttonPressedPosition;
    private bool cameraActive = false;
    private bool isAnimating = false;

    public float buttonMoveDistance = 0.02f;
    public float animationDuration = 0.2f;

    private Color originalLEDColor;

    private void Start()
    {
        buttonInitialPosition = button.localPosition;
        buttonPressedPosition = buttonInitialPosition - new Vector3(0, buttonMoveDistance, 0);

        originalLEDColor = ledRenderer.material.GetColor("_EmissionColor");

        SetButtonOutlineEmission(false);
        SetLEDColor(false);

        magnifyCamera.gameObject.SetActive(false);
        ClearRenderTexture(); // Black screen initially
    }

    public void OnButtonPressed()
    {
        if (isAnimating)
            return;

        cameraActive = !cameraActive;
        magnifyCamera.gameObject.SetActive(cameraActive);

        if (!cameraActive)
            ClearRenderTexture(); // Immediately black out when off

        SetLEDColor(cameraActive);
        SetButtonOutlineEmission(cameraActive);

        StopAllCoroutines();
        Vector3 targetPosition = cameraActive ? buttonPressedPosition : buttonInitialPosition;
        StartCoroutine(AnimateButton(button.localPosition, targetPosition));
    }

    private System.Collections.IEnumerator AnimateButton(Vector3 from, Vector3 to)
    {
        isAnimating = true;
        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            button.localPosition = Vector3.Lerp(from, to, elapsed / animationDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        button.localPosition = to;
        isAnimating = false;
    }

    private void SetButtonOutlineEmission(bool isOn)
    {
        Renderer btnRenderer = button.GetComponent<Renderer>();
        if (btnRenderer != null)
        {
            Material mat = btnRenderer.material;
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", isOn ? Color.green * 0.5f : Color.red * 0.5f);
        }
    }

    private void SetLEDColor(bool isOn)
    {
        if (ledRenderer != null)
        {
            Material mat = ledRenderer.material;
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", isOn ? Color.green * 1.0f : originalLEDColor);
        }
    }

    private void ClearRenderTexture()
    {
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = magnifyRenderTexture;

        GL.Clear(true, true, Color.black);

        // Optionally draw black quad if above is insufficient:
        Graphics.Blit(null, magnifyRenderTexture, blackMaterial);

        RenderTexture.active = currentRT;
    }
}
