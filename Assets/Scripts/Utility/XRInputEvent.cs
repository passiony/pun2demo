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

        private bool m_LastActivePressed;
        private bool m_CurrActivePressed;

        private bool m_LastSelectPressed;
        private bool m_CurrSelectPressed;
        
        public UnityEvent<bool> OnTriggerButton = new();
        public UnityEvent<bool> OnGrabButton = new();
        
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
            UpdateTrigger();
            UpdateSelect();
        }

        void UpdateTrigger()
        {
            m_CurrActivePressed = m_ActionController.activateAction.action.IsPressed();
            if (m_CurrActivePressed == m_LastActivePressed)
            {
                return;
            }

            m_LastActivePressed = m_CurrActivePressed;
            OnTriggerButton?.Invoke(m_CurrActivePressed);
        }

        void UpdateSelect()
        {
            m_CurrSelectPressed = m_ActionController.selectAction.action.IsPressed();
            if (m_CurrSelectPressed == m_LastSelectPressed)
            {
                return;
            }
            
            m_LastSelectPressed = m_CurrSelectPressed;
            OnGrabButton?.Invoke(m_CurrSelectPressed);
        }
    }
}