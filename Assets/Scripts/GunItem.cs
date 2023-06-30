using System.Collections;
using UnityEngine;

public class GunItem : MonoBehaviour
{
    public EGunID m_GunType;

    void Start()
    {
        Destroy(gameObject,10);
    }
}