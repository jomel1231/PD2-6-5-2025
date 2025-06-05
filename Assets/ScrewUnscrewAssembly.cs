using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class ScrewUnscrewAssembly : MonoBehaviour
{
    public bool IsScrewedIn { get; set; } = false;
    public bool HasBeenTriggered { get; set; } = false;

    [Header("Screw Settings")]
    public Transform screw;
    public Vector3 slideOffset = new Vector3(0f, 0.02f, 0f);
    public float slideSpeed = 0.01f;
    public float rotationSpeed = 360f;
    public int numberOfTurns = 3;
    public enum RotationAxis { X, Y, Z }
    public RotationAxis rotationAxis = RotationAxis.Y;

    [Header("Interaction")]
    public string screwdriverTag = "Screwdriver";
    public InputAction triggerAction;

    [Header("Manager Reference")]
    public MonoBehaviour groupManagerBase; // Accept either group manager

    [Header("Step Check Manager")]
    public SocketStepManager stepManager; // 👈 NEW: Reference to SocketStepManager

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isAnimating = false;
    private bool screwdriverInside = false;

    void Start()
    {
        closedPosition = screw.localPosition;
        openPosition = closedPosition + slideOffset;

        triggerAction.Enable();
        triggerAction.performed += OnTriggerPressed;
    }

    void OnDestroy()
    {
        triggerAction.performed -= OnTriggerPressed;
        triggerAction.Disable();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(screwdriverTag))
            screwdriverInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(screwdriverTag))
            screwdriverInside = false;
    }

    private void OnTriggerPressed(InputAction.CallbackContext context)
    {
        if (!screwdriverInside || isAnimating || groupManagerBase == null)
            return;

        // 🚫 Block if any of steps 0 to 4 are not completed
        if (stepManager == null ||
            !stepManager.IsStepDone(0) ||
            !stepManager.IsStepDone(1) ||
            !stepManager.IsStepDone(2) ||
            !stepManager.IsStepDone(3) ||
            !stepManager.IsStepDone(4))
        {
            return;
        }

        // ✅ Proceed with toggle
        if (groupManagerBase is PentalobeGroupAssembly pentalobeManager)
            pentalobeManager.OnScrewTriggered(this);
        else if (groupManagerBase is TriPointGroupAssembly triPointManager)
            triPointManager.OnScrewTriggered(this);
    }

    public void Animate(bool unscrew)
    {
        StartCoroutine(ScrewRoutine(unscrew));
    }

    private IEnumerator ScrewRoutine(bool unscrew)
    {
        isAnimating = true;

        float totalRotation = 360f * numberOfTurns;
        float rotated = 0f;
        float elapsedTime = 0f;

        Vector3 from = unscrew ? closedPosition : openPosition;
        Vector3 to = unscrew ? openPosition : closedPosition;
        float duration = Vector3.Distance(from, to) / slideSpeed;

        while (rotated < totalRotation || elapsedTime < duration)
        {
            if (rotated < totalRotation)
            {
                float rotateStep = rotationSpeed * Time.deltaTime;
                screw.Rotate(GetRotationAxis(), unscrew ? rotateStep : -rotateStep, Space.Self);
                rotated += rotateStep;
            }

            if (elapsedTime < duration)
            {
                screw.localPosition = Vector3.Lerp(from, to, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
            }

            yield return null;
        }

        screw.localPosition = to;
        IsScrewedIn = !unscrew;
        isAnimating = false;
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
}
