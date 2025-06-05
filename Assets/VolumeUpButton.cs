using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

[RequireComponent(typeof(XRBaseInteractable))]
public class VolumeUpButton : MonoBehaviour
{
    public enum Axis { X, Y, Z }

    [Header("Movement Settings")]
    public Axis moveAxis = Axis.Y;
    public float moveDistance = 0.02f;
    public float returnDelay = 0.5f;

    [HideInInspector] public bool IsPressed = false;

    private Vector3 originalPosition;
    private XRBaseInteractable interactable;
    private bool isAnimating = false;

    private void Start()
    {
        originalPosition = transform.localPosition;

        interactable = GetComponent<XRBaseInteractable>();
        interactable.selectEntered.AddListener(OnPressed);
        interactable.selectExited.AddListener(OnReleased);
    }

    private void OnPressed(SelectEnterEventArgs args)
    {
        if (!isAnimating)
            StartCoroutine(AnimatePress());
        IsPressed = true;
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        IsPressed = false;
    }

    private IEnumerator AnimatePress()
    {
        isAnimating = true;
        Vector3 pressedPosition = originalPosition + GetOffset();
        transform.localPosition = pressedPosition;

        yield return new WaitForSeconds(returnDelay);

        transform.localPosition = originalPosition;
        isAnimating = false;
    }

    private Vector3 GetOffset()
    {
        switch (moveAxis)
        {
            case Axis.X: return new Vector3(-moveDistance, 0, 0);
            case Axis.Y: return new Vector3(0, -moveDistance, 0);
            case Axis.Z: return new Vector3(0, 0, -moveDistance);
            default: return Vector3.zero;
        }
    }
}
