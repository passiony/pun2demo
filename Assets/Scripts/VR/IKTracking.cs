using System;
using Photon.Pun;
using RootMotion.FinalIK;
using UnityEngine;

public class IKTracking : MonoBehaviourPun
{
    [SerializeField] private Transform centerEyeAnchor;
    [SerializeField] private Transform leftHandAnchor;
    [SerializeField] private Transform rightHandAnchor;

    [SerializeField] private VRAwatar m_Avatar;

    [SerializeField] private bool m_IkEnable;
    public bool IkEnable => m_IkEnable;
    public VRAwatar Avatar=> m_Avatar;

    public void UpdateRoot(Vector3 pos)
    {
        transform.position = pos;
    }

    public void UpdateHead(Vector3 pos, Vector3 euler)
    {
        centerEyeAnchor.position = pos;
        centerEyeAnchor.eulerAngles = euler;
    }

    public void UpdateLeftHand(Vector3 pos, Vector3 euler)
    {
        leftHandAnchor.position = pos;
        leftHandAnchor.eulerAngles = euler;
    }

    public void UpdateRightHand(Vector3 pos, Vector3 euler)
    {
        rightHandAnchor.position = pos;
        rightHandAnchor.eulerAngles = euler;
    }

    public void SetHeadVisible(bool value)
    {
        m_Avatar.SetHeadVisible(value);
    }

    public void UseRagdoll(bool enable)
    {
        m_IkEnable = enable;
        if (enable)
        {
            m_Avatar.GetComponent<VRIK>().enabled = true;
            m_Avatar.GetComponent<Animator>().enabled = true;

            var rigidbodys = this.GetComponentsInChildren<Rigidbody>();
            foreach (var body in rigidbodys)
            {
                body.useGravity = false;
                body.isKinematic = true;
            }
        }
        else
        {
            m_Avatar.GetComponent<VRIK>().enabled = false;
            m_Avatar.GetComponent<Animator>().enabled = false;

            var rigidbodys = this.GetComponentsInChildren<Rigidbody>();
            foreach (var body in rigidbodys)
            {
                body.useGravity = true;
                body.isKinematic = false;
            }
        }
    }
}