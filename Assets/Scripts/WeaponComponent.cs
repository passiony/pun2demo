using System.Collections.Generic;
using UnityEngine;

public class WeaponComponent : MonoBehaviour
{
    private GunWeapon[] m_Weapons;
    private Dictionary<EGunID, GunWeapon> m_WeaponDic;
    private GunWeapon m_Weapon;
    [SerializeField] private EGunID m_WeaponId;

    private void Awake()
    {
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
        m_Weapon.Equip(transform);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_Weapon.UseItem();
        }
    }

    public void LoadWeapon(EGunID gunId)
    {
        m_WeaponId = gunId;
        m_Weapon.gameObject.SetActive(false);
        m_Weapon = m_WeaponDic[m_WeaponId];
        m_Weapon.gameObject.SetActive(true);
    }
}