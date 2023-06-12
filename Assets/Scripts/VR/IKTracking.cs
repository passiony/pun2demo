using Photon.Pun;
using UnityEngine;

public class IKTracking : MonoBehaviourPun
{
    private Transform trackingRoot;
    [SerializeField]
    private Transform centerEyeAnchor;
    [SerializeField]
    private Transform leftHandAnchor;
    [SerializeField]
    private Transform rightHandAnchor;

    [SerializeField]
    private VRAwatar m_Avatar;
    
    public void UpdateHead(Vector3 pos,Vector3 euler)
    {
        centerEyeAnchor.position = pos;
        centerEyeAnchor.eulerAngles = euler;
    }

    public void UpdateLeftHand(Vector3 pos,Vector3 euler)
    {
        leftHandAnchor.position = pos;
        leftHandAnchor.eulerAngles = euler;
    }

    public void UpdateRightHand(Vector3 pos,Vector3 euler)
    {
        rightHandAnchor.position = pos;
        rightHandAnchor.eulerAngles = euler;
    }

    public void SetHeadVisible(bool value)
    {
        m_Avatar.SetHeadVisible(value);
    }
}