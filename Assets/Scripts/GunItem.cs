using System.Collections;
using Photon.Pun.MFPS;
using UnityEngine;

public class GunItem : MonoBehaviour
{
    public EGunID m_GunType;

    void Start()
    {
        Destroy(gameObject, FPSGame.REBORN_TIME);
    }
}