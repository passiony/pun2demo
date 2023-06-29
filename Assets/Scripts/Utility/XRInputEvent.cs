using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace GT.Hotfix
{
    /// <summary>
    /// VR手柄按键事件类
    /// </summary>
    public class XRInputEvent : MonoBehaviour
    {
        private ActionBasedController m_ActionController;

        public bool m_LastPressed;
        public bool m_CurrPressed;

        public UnityEvent OnTriggerButtonDown = new UnityEvent();
        public UnityEvent OnTriggerButtonUp = new UnityEvent();

        void Awake()
        {
            m_ActionController = GetComponent<ActionBasedController>();
        }

        void Update()
        {
            if (m_ActionController == null)
            {
                return;
            }

            m_CurrPressed = m_ActionController.activateAction.action.IsPressed();
            if (m_CurrPressed == m_LastPressed)
            {
                return;
            }

            m_LastPressed = m_CurrPressed;
            if (m_CurrPressed)
            {
                OnTriggerButtonDown?.Invoke();
            }
            else
            {
                OnTriggerButtonUp?.Invoke();
            }
        }
    }
}