using System.Collections;
using Photon.Pun;
using UnityEngine;

public class BulletSupply : MonoBehaviourPunCallbacks, ISupply
{
    public float m_Duration=20;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(m_Duration);
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    public void Supply(PlayerController _target)
    {
        _target.LoadBulletSupply();
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}