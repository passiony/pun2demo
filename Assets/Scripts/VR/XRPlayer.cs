using GT.Hotfix;
using UnityEngine;

public class XRPlayer : MonoBehaviour
{
    public static XRPlayer Instance;

    private IKTracking m_IKTracking;

    [SerializeField] private Transform Head;
    [SerializeField] private Transform LeftHand;
    [SerializeField] private Transform RightHand;

    private VRPlayerController m_Player;
    private bool m_RightTrigger;
    private bool m_LeftTrigger;

    private void Awake()
    {
        Instance = this;
        RightHand.GetComponent<XRInputEvent>().OnTriggerButton.AddListener((x) => { m_RightTrigger = x; });
        LeftHand.GetComponent<XRInputEvent>().OnTriggerButton.AddListener((x) => { m_LeftTrigger = x; });
    }

    public void SetTracking(IKTracking tracking)
    {
        m_IKTracking = tracking;
    }

    void Update()
    {
        if (m_IKTracking)
        {
            var foot = Head.position;
            foot.y = m_IKTracking.transform.position.y;
            m_IKTracking.UpdateRoot(foot);
            m_IKTracking.UpdateHead(Head.position, Head.eulerAngles);
            m_IKTracking.UpdateLeftHand(LeftHand.position, LeftHand.eulerAngles);
            m_IKTracking.UpdateRightHand(RightHand.position, RightHand.eulerAngles);
        }

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

    public void SetPlayer(VRPlayerController player)
    {
        m_Player = player;
    }
}