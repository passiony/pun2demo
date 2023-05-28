using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class GunSupply : MonoBehaviourPunCallbacks, ISupply
{
    public GameObject[] m_Guns;
    public EGunType m_GunType;

    IEnumerator Start()
    {
        var random = Random.Range(0, 2);
        m_GunType = (EGunType)random;
        m_Guns[random].SetActive(true);

        yield return new WaitForSeconds(10);
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTrigger:" + other.name);
    }

    public void Supply(PlayerController _target)
    {
        _target.LoadGun(m_GunType);
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}