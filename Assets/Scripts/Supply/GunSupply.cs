using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MFPS
{
    public class GunSupply : MonoBehaviourPunCallbacks, ISupply
    {
        public GameObject[] m_Guns;

        // public EGunType m_GunType;
        public float m_Duration = 20;

        IEnumerator Start()
        {
            var random = Random.Range(0, 2);
            // m_GunType = (EGunType)random;
            // m_Guns[random].SetActive(true);

            yield return new WaitForSeconds(m_Duration);
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }


        public void Supply(VRPlayerController _target)
        {
            // _target.LoadGunSupply(m_GunType);
            // if (PhotonNetwork.IsMasterClient)
            // {
            //     PhotonNetwork.Destroy(gameObject);
            // }
        }
    }
}