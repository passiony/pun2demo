using System;
using UnityEngine;

public enum EGunType
{
    Rifle,
    AK47,
}

public class BaseGun : MonoBehaviour
{
    [SerializeField] protected EGunType m_GunType;
    [SerializeField] protected int m_BulletCount = 30;
    [SerializeField] protected float m_FireInterval = 0.2f;
    [SerializeField] protected MyMuzzleFlash m_MuzzleFlash;
    [SerializeField] protected GameObject m_HitEffect;
    [SerializeField] protected Transform m_LeftHandle;
    [SerializeField] protected Transform m_RightHandle;
    [SerializeField] protected AudioClip m_FireSound;
    [SerializeField] protected AudioClip m_DryFireSound;

    protected bool m_Fire;
    protected PlayerController target;
    protected AudioSource m_FireAudio;

    public EGunType GunType => m_GunType;
    public int BulletCount => m_BulletCount;

    protected virtual void Awake()
    {
        m_FireAudio = this.GetComponent<AudioSource>();
    }

    public virtual void AddBullet(int bullet)
    {
        m_BulletCount += bullet;
    }

    public virtual void SetFire(bool fire)
    {
        m_Fire = fire;
    }

    public void SetTarget(PlayerController _target)
    {
        target = _target;
    }

    public virtual void Load()
    {
        this.gameObject.SetActive(true);
        m_BulletCount = 30;

        target.InitIK(m_LeftHandle, m_RightHandle);
    }

    protected virtual void Fire()
    {
        m_BulletCount--;
        PlayFire();
        m_MuzzleFlash.Show();
    }

    protected virtual void DryFire()
    {
        PlayDryFire();
    }
    protected void PlayDryFire()
    {
        m_FireAudio.clip = m_DryFireSound;
        m_FireAudio.Play();
    }
    protected void PlayFire()
    {
        m_FireAudio.clip = m_FireSound;
        m_FireAudio.Play();
    }
}