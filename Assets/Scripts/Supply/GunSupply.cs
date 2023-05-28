using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

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

    public void Supply(PlayerController _target)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            _target.LoadGun(m_GunType);
            PhotonNetwork.Destroy(gameObject);
        }
    }
}