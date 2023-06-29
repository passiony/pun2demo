using System.Collections.Generic;
using UnityEngine;

public class WeaponComponent : MonoBehaviour
{
    [SerializeField] private EGunID m_WeaponId;
    [SerializeField] private float m_SwithRate = 1f;

    private VRPlayerController m_Owner;
    private GunWeapon[] m_Weapons;
    private Dictionary<EGunID, GunWeapon> m_WeaponDic;
    private GunWeapon m_Weapon;
    private float m_LastUseTime;
    
    private void Awake()
    {
        m_Owner = this.GetComponentInParent<VRPlayerController>();
        m_Weapons = this.GetComponentsInChildren<GunWeapon>(true);
        m_WeaponDic = new Dictionary<EGunID, GunWeapon>();
        foreach (var weapon in m_Weapons)
        {
            weapon.gameObject.SetActive(false);
            m_WeaponDic.Add(weapon.ID, weapon);
        }
    }

    void Start()
    {
        m_Weapon = m_WeaponDic[m_WeaponId];
        m_Weapon.Equip(m_Owner);
    }

    public void LoadWeapon(EGunID gunId)
    {
        Debug.Log("LoadWeapon:" + gunId);
        m_WeaponId = gunId;
        m_Weapon.gameObject.SetActive(false);
        m_Weapon = m_WeaponDic[m_WeaponId];
        m_Weapon.gameObject.SetActive(true);
        m_Weapon.Equip(m_Owner);
    }

    public void UseWeapon()
    {
        m_Weapon.UseItem();
    }

    public void SwitchWeapon()
    {
        if (Time.time - m_LastUseTime > m_SwithRate)
        {
            m_LastUseTime = Time.time;
            int gunid = (int)m_WeaponId;
            if (++gunid >= m_Weapons.Length) gunid = 0;
            LoadWeapon((EGunID)gunid);
        }

    }
}