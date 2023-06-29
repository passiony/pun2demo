using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace GT.Hotfix
{
    /// <summary>
    /// VR手柄控制类
    /// </summary>
    public class VRHandItem : MonoBehaviour
    {
        private XRInteractionGroup m_XRGroup;
        private XRRayInteractor m_XrRayInteractor;
        private XRDirectInteractor m_XrDirectInteractor;

        private XRInputEvent m_XrInputEvent;

         void Start()
        {
            m_XRGroup = GetComponent<XRInteractionGroup>();
            m_XrRayInteractor = GetComponentInChildren<XRRayInteractor>();
            m_XrDirectInteractor = GetComponentInChildren<XRDirectInteractor>();

            m_XrRayInteractor.selectEntered.AddListener(OnSelectEntered);
            m_XrDirectInteractor.selectEntered.AddListener(OnSelectEntered);

            m_XrInputEvent = gameObject.GetComponent<XRInputEvent>();
            m_XrInputEvent.OnTriggerButton.AddListener(OnTriggerButtonClick);
            m_XrInputEvent.OnGrabButton.AddListener(OnGrabButtonClick);
        }

         public void OnSelectEntered(SelectEnterEventArgs args)
        {
            // var attach = args.interactableObject.transform;
            // var item = attach.GetComponent<SceneObject>();
        }
         
         private void OnGrabButtonClick(bool arg0)
         {
             throw new System.NotImplementedException();
         }

         private void OnTriggerButtonClick(bool arg0)
         {
             throw new System.NotImplementedException();
         }
    }
}