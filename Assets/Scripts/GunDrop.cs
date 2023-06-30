using System.Collections.Generic;
using UnityEngine;

public class GunDrop : MonoSingleton<GunDrop>
{
    public GunItem[] m_GunModels;

    private Dictionary<EGunID, GunItem> m_GunDic = new Dictionary<EGunID, GunItem>();

    void Start()
    {
        foreach (var gun in m_GunModels)
        {
            m_GunDic.Add(gun.m_GunType, gun);
        }
    }

    public void CreateDrop(GunWeapon gun)
    {
        var prefab = m_GunDic[gun.ID];
        var go = GameObject.Instantiate(prefab);
        go.transform.position = gun.transform.position;
        go.transform.rotation = gun.transform.rotation;
    }
}