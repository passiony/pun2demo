using System.Collections;
using Photon.Pun;
using UnityEngine;

public class BulletSupply : MonoBehaviourPunCallbacks, ISupply
{
    IEnumerator Start()
    {
        yield return new WaitForSeconds(10);
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    public void Supply(PlayerController _target)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            _target.AddBullet();
            PhotonNetwork.Destroy(gameObject);
        }
    }
}