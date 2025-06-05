using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace UnityEngine.XR.Interaction.Toolkit.Feedback
{
    [Serializable]
    public class HapticImpulseData
    {
        [SerializeField, Range(0f, 1f)]
        private float m_Amplitude = 0.5f;

        public float amplitude { get => m_Amplitude; set => m_Amplitude = Mathf.Clamp01(value); }

        [SerializeField]
        private float m_Duration = 0.1f;

        public float duration { get => m_Duration; set => m_Duration = Mathf.Max(0, value); }

        [SerializeField]
        private float m_Frequency = 0f;

        public float frequency { get => m_Frequency; set => m_Frequency = Mathf.Max(0, value); }
    }

    [AddComponentMenu("XR/Feedback/Simple Haptic Feedback")]
    public class SimpleHapticFeedback : MonoBehaviour
    {
        [SerializeField]
        private XRBaseInteractor m_Interactor;

        [SerializeField]
        private HapticImpulseData m_SelectEnteredData = new HapticImpulseData();

        [SerializeField]
        private HapticImpulseData m_SelectExitedData = new HapticImpulseData();

        [SerializeField]
        private HapticImpulseData m_HoverEnteredData = new HapticImpulseData { amplitude = 0.25f, duration = 0.1f };

        [SerializeField]
        private HapticImpulseData m_HoverExitedData = new HapticImpulseData { amplitude = 0.25f, duration = 0.1f };

        private void OnEnable()
        {
            if (m_Interactor != null)
            {
                m_Interactor.selectEntered.AddListener(OnSelectEntered);
                m_Interactor.selectExited.AddListener(OnSelectExited);
                m_Interactor.hoverEntered.AddListener(OnHoverEntered);
                m_Interactor.hoverExited.AddListener(OnHoverExited);
            }
        }

        private void OnDisable()
        {
            if (m_Interactor != null)
            {
                m_Interactor.selectEntered.RemoveListener(OnSelectEntered);
                m_Interactor.selectExited.RemoveListener(OnSelectExited);
                m_Interactor.hoverEntered.RemoveListener(OnHoverEntered);
                m_Interactor.hoverExited.RemoveListener(OnHoverExited);
            }
        }

        private void OnSelectEntered(SelectEnterEventArgs args)
        {
            SendHapticImpulse(m_SelectEnteredData);
        }

        private void OnSelectExited(SelectExitEventArgs args)
        {
            SendHapticImpulse(m_SelectExitedData);
        }

        private void OnHoverEntered(HoverEnterEventArgs args)
        {
            SendHapticImpulse(m_HoverEnteredData);
        }

        private void OnHoverExited(HoverExitEventArgs args)
        {
            SendHapticImpulse(m_HoverExitedData);
        }

        private void SendHapticImpulse(HapticImpulseData data)
        {
            if (m_Interactor is XRBaseControllerInteractor controllerInteractor)
            {
                controllerInteractor.SendHapticImpulse(data.amplitude, data.duration);
            }
        }
    }

    public class HapticImpulsePlayer : MonoBehaviour
    {
        public void SendHapticImpulse(float amplitude, float duration, float frequency = 0f)
        {
            if (TryGetComponent(out XRBaseControllerInteractor controllerInteractor))
            {
                controllerInteractor.SendHapticImpulse(amplitude, duration);
            }
        }
    }
}
