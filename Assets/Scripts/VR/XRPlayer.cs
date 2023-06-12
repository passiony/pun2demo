using System;
using UnityEngine;

public class XRPlayer : MonoBehaviour
{
    public static XRPlayer Instance;
    
    private IKTracking m_IKTracking;

    [SerializeField] private Transform Head;
    [SerializeField] private Transform LeftHand;
    [SerializeField] private Transform RightHand;

    private void Awake()
    {
        Instance = this;
    }

    public void SetTracking(IKTracking tracking)
    {
        m_IKTracking = tracking;
    }
    void Update()
    {
        if (m_IKTracking)
        {
            m_IKTracking.UpdateHead(Head.position, Head.eulerAngles);
            m_IKTracking.UpdateLeftHand(LeftHand.position, LeftHand.eulerAngles);
            m_IKTracking.UpdateRightHand(RightHand.position, RightHand.eulerAngles);
        }
    }
}