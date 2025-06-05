using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets
{
    /// <summary>
    /// Component that reads input values and animates the thumbstick, trigger, and grip transforms
    /// to mimic real controller movements.
    /// </summary>
    public class ControllerAnimator : MonoBehaviour
    {
        [Header("Thumbstick")]
        [SerializeField] private Transform m_ThumbstickTransform;
        [SerializeField] private Vector2 m_StickRotationRange = new Vector2(30f, 30f);
        [SerializeField] private InputActionReference m_StickInput; // Replaced XRInputValueReader

        [Header("Trigger")]
        [SerializeField] private Transform m_TriggerTransform;
        [SerializeField] private Vector2 m_TriggerXAxisRotationRange = new Vector2(0f, -15f);
        [SerializeField] private InputActionReference m_TriggerInput; // Replaced XRInputValueReader

        [Header("Grip")]
        [SerializeField] private Transform m_GripTransform;
        [SerializeField] private Vector2 m_GripRightRange = new Vector2(-0.0125f, -0.011f);
        [SerializeField] private InputActionReference m_GripInput; // Replaced XRInputValueReader

        private void OnEnable()
        {
            if (m_ThumbstickTransform == null || m_GripTransform == null || m_TriggerTransform == null)
            {
                Debug.LogWarning($"Controller Animator component is missing references on {gameObject.name}", this);
                enabled = false;
                return;
            }

            // Enable input actions
            m_StickInput?.action.Enable();
            m_TriggerInput?.action.Enable();
            m_GripInput?.action.Enable();
        }

        private void OnDisable()
        {
            // Disable input actions
            m_StickInput?.action.Disable();
            m_TriggerInput?.action.Disable();
            m_GripInput?.action.Disable();
        }

        private void Update()
        {
            if (m_StickInput != null)
            {
                Vector2 stickVal = m_StickInput.action.ReadValue<Vector2>();
                m_ThumbstickTransform.localRotation = Quaternion.Euler(-stickVal.y * m_StickRotationRange.x, 0f, -stickVal.x * m_StickRotationRange.y);
            }

            if (m_TriggerInput != null)
            {
                float triggerVal = m_TriggerInput.action.ReadValue<float>();
                m_TriggerTransform.localRotation = Quaternion.Euler(Mathf.Lerp(m_TriggerXAxisRotationRange.x, m_TriggerXAxisRotationRange.y, triggerVal), 0f, 0f);
            }

            if (m_GripInput != null)
            {
                float gripVal = m_GripInput.action.ReadValue<float>();
                Vector3 currentPos = m_GripTransform.localPosition;
                m_GripTransform.localPosition = new Vector3(Mathf.Lerp(m_GripRightRange.x, m_GripRightRange.y, gripVal), currentPos.y, currentPos.z);
            }
        }
    }
}
