using System.Collections.Generic;
using UnityEngine;

public class WeaponComponent : MonoBehaviour
{
    [SerializeField] private EGunID m_WeaponId;
    [SerializeField] private float m_SwithRate = 1f;
    [SerializeField] private bool m_AutoReload = true;
    private VRPlayerController m_Owner;
    private GunWeapon[] m_AllWeapons;
    private Dictionary<EGunID, GunWeapon> m_WeaponDic;

    private GunWeapon m_CurrentWeapon;
    private float m_LastUseTime;

    private void Awake()
    {
        m_Owner = this.GetComponentInParent<VRPlayerController>();
        m_AllWeapons = this.GetComponentsInChildren<GunWeapon>(true);
        m_WeaponDic = new Dictionary<EGunID, GunWeapon>();
        foreach (var weapon in m_AllWeapons)
        {
            weapon.gameObject.SetActive(false);
            m_WeaponDic.Add(weapon.ID, weapon);
        }
    }

    void Start()
    {
        m_CurrentWeapon = m_WeaponDic[m_WeaponId];
        m_CurrentWeapon.Equip(m_Owner);
    }

    public void LoadWeapon(EGunID gunId)
    {
        Debug.Log("LoadWeapon:" + gunId);
        m_WeaponId = gunId;
        m_CurrentWeapon.gameObject.SetActive(false);
        m_CurrentWeapon = m_WeaponDic[m_WeaponId];
        m_CurrentWeapon.gameObject.SetActive(true);
        m_CurrentWeapon.Equip(m_Owner);
    }

    public void UseWeapon()
    {
        bool success = m_CurrentWeapon.UseItem();
        if (!success)
        {
            m_CurrentWeapon.CheckReload(m_AutoReload);
        }
    }

    public void SwitchWeapon()
    {
        if (Time.time - m_LastUseTime > m_SwithRate)
        {
            m_LastUseTime = Time.time;
            int gunid = (int)m_WeaponId;
            if (++gunid >= m_AllWeapons.Length) gunid = 0;
            LoadWeapon((EGunID)gunid);
        }
    }

    public GunWeapon GetCurrentWeapon()
    {
        return m_CurrentWeapon;
    }

    public void DropWeapon()
    {
        m_CurrentWeapon.gameObject.SetActive(false);
        GunDrop.Instance.CreateDrop(m_CurrentWeapon);
    }
}