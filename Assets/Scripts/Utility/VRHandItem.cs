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
            m_XrInputEvent.OnTriggerButtonDown.AddListener(OnTriggerButtonDown);
            m_XrInputEvent.OnTriggerButtonUp.AddListener(OnTriggerButtonUp);
        }

        public void OnSelectEntered(SelectEnterEventArgs args)
        {
            var attach = args.interactableObject.transform;
            // var item = attach.GetComponent<SceneObject>();
            // if (item)
            // {
            //     if (Hand == EHandPart.LeftHand)
            //     {
            //         item.OnTouchEnter(ITEM_ID.LeftHand);
            //         Debug.Log("左手触摸了：" + item.name);
            //     }
            //     else
            //     {
            //         item.OnTouchEnter(ITEM_ID.RightHand);
            //         Debug.Log("右手触摸了：" + item.name);
            //     }
            // }
        }

        protected virtual void OnTriggerButtonDown()
        {
        }

        protected virtual void OnTriggerButtonUp()
        {
        }
    }
}