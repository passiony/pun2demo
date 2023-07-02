using UnityEngine;
using UnityEngine.Serialization;

namespace MFPS
{
    public class XRPlayer : MonoBehaviour
    {
        public static XRPlayer Instance;

        private IKTracking m_IKTracking;

        [FormerlySerializedAs("Head")] [SerializeField]
        private Transform m_Head;

        [FormerlySerializedAs("LeftHand")] [SerializeField]
        private Transform m_LeftHand;

        [FormerlySerializedAs("RightHand")] [SerializeField]
        private Transform m_RightHand;

        public Transform Head => m_Head;
        private VRPlayerController m_Player;
        private bool m_RightTrigger;
        private bool m_LeftTrigger;

        public VRPlayerController Player => m_Player;

        private void Awake()
        {
            Instance = this;
            m_RightHand.GetComponent<XRInputEvent>().OnTriggerButton.AddListener((x) => { m_RightTrigger = x; });
            m_LeftHand.GetComponent<XRInputEvent>().OnTriggerButton.AddListener((x) => { m_LeftTrigger = x; });
        }

        public void SetTracking(IKTracking tracking)
        {
            m_IKTracking = tracking;
            tracking.SetHeadVisible(false);
        }

        public void SetBorn(Transform born)
        {
            transform.position = born.position;
            transform.rotation = born.rotation;
        }
        void Update()
        {
            if (m_IKTracking && m_IKTracking.IkEnable)
            {
                var foot = m_Head.position;
                foot.y = m_IKTracking.transform.position.y;
                m_IKTracking.UpdateRoot(foot);
                m_IKTracking.UpdateHead(m_Head.position, m_Head.eulerAngles);
                m_IKTracking.UpdateLeftHand(m_LeftHand.position, m_LeftHand.eulerAngles);
                m_IKTracking.UpdateRightHand(m_RightHand.position, m_RightHand.eulerAngles);

                if (m_Player)
                {
                    if (Input.GetMouseButtonDown(0) || m_RightTrigger)
                    {
                        m_Player.Fire();
                    }

                    if (Input.GetKeyUp(KeyCode.Q) || m_LeftTrigger)
                    {
                        m_Player.SwitchGun();
                    }
                }
            }
        }

        public void SetPlayer(VRPlayerController player)
        {
            m_Player = player;
        }
    }
}