using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Events;

public class ScrewUnscrew : MonoBehaviour
{
    private Renderer cachedRenderer;
    private Material defaultMat;
    private Material highlightMat;

    public enum RotationAxis { X, Y, Z }

    public System.Action onScrewCompleted;
    public UnityEvent onScrewStateChanged;

    public bool isStepActive = true;
    public bool IsScrewedIn { get; set; } = false;

    [Header("Screw Settings")]
    public Transform screw;
    public Vector3 slideOffset = new Vector3(0f, 0.02f, 0f);
    public float slideSpeed = 0.01f;
    public float rotationSpeed = 360f;
    public int numberOfTurns = 3;
    public RotationAxis rotationAxis = RotationAxis.Y;

    [Header("Interaction Settings")]
    public string screwdriverTag = "Screwdriver";
    public InputAction triggerAction;

    [Header("Manager Reference")]
    public MonoBehaviour genericManagerToNotify; // ✅ Can be ScrewGroupManager or ScrewGroupManagerAssembly

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isUnscrewing = false;
    private bool screwdriverInside = false;
    private bool isCounted = false;

    private Collider myCollider;

    public Vector3 initialLocalPosition { get; private set; }
    public Quaternion initialLocalRotation { get; private set; }

    void Start()
    {
        if (screw == null)
        {
            Debug.LogWarning("Screw reference is missing!");
            return;
        }

        initialLocalPosition = screw.localPosition;
        initialLocalRotation = screw.localRotation;

        closedPosition = screw.localPosition;
        openPosition = closedPosition + slideOffset;

        triggerAction.Enable();
        triggerAction.performed += OnTriggerPressed;

        myCollider = GetComponent<Collider>();

        if (onScrewStateChanged == null)
            onScrewStateChanged = new UnityEvent();
    }

    private void OnDestroy()
    {
        triggerAction.performed -= OnTriggerPressed;
        triggerAction.Disable();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(screwdriverTag))
        {
            screwdriverInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(screwdriverTag))
        {
            screwdriverInside = false;
        }
    }

    private void OnTriggerPressed(InputAction.CallbackContext context)
    {
        if (!isStepActive || isUnscrewing)
            return;

        if (screwdriverInside)
        {
            StartCoroutine(UnscrewRoutine());
        }
    }

    IEnumerator UnscrewRoutine()
    {
        isUnscrewing = true;

        if (!isCounted)
        {
            isCounted = true;

            // ✅ Notify based on actual type
            if (genericManagerToNotify is ScrewGroupManager manager)
                manager.NotifyScrewUnscrewed(gameObject);
        }

        float totalRotation = 360f * numberOfTurns;
        float rotated = 0f;
        float elapsedTime = 0f;

        Vector3 startingPos = screw.localPosition;
        float journeyLength = Vector3.Distance(startingPos, openPosition);
        float slideDuration = journeyLength / slideSpeed;

        while (rotated < totalRotation || elapsedTime < slideDuration)
        {
            if (rotated < totalRotation)
            {
                float rotateStep = rotationSpeed * Time.deltaTime;
                Vector3 axis = GetRotationAxis();
                screw.Rotate(axis, rotateStep, Space.Self);
                rotated += rotateStep;
            }

            if (elapsedTime < slideDuration)
            {
                screw.localPosition = Vector3.Lerp(startingPos, openPosition, elapsedTime / slideDuration);
                elapsedTime += Time.deltaTime;
            }

            yield return null;
        }

        screw.localPosition = openPosition;
        IsScrewedIn = true;

        onScrewStateChanged?.Invoke();

        yield return null;

        if (myCollider != null)
            myCollider.enabled = false;

        onScrewCompleted?.Invoke();
    }

    private Vector3 GetRotationAxis()
    {
        return rotationAxis switch
        {
            RotationAxis.X => Vector3.right,
            RotationAxis.Y => Vector3.up,
            RotationAxis.Z => Vector3.forward,
            _ => Vector3.up,
        };
    }

    public void ResetScrew()
    {
        if (screw != null)
        {
            screw.localPosition = initialLocalPosition;
            screw.localRotation = initialLocalRotation;
        }

        IsScrewedIn = false;
        isUnscrewing = false;
        isCounted = false;

        if (myCollider != null)
            myCollider.enabled = true;
    }

    public void AssignMaterials(Material defaultMaterial, Material highlightMaterial)
    {
        defaultMat = defaultMaterial;
        highlightMat = highlightMaterial;

        if (cachedRenderer == null)
            cachedRenderer = GetComponentInChildren<Renderer>();
    }

    public void EnableHighlight()
    {
        if (cachedRenderer != null && highlightMat != null)
            cachedRenderer.material = highlightMat;
    }

    public void DisableHighlight()
    {
        if (cachedRenderer != null && defaultMat != null)
            cachedRenderer.material = defaultMat;
    }
}
