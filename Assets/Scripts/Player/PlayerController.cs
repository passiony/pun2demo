using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    public GameObject HealthUIPrefab;
    public int MaxHP => 100;
    private int m_Hp;
    public int HP => m_Hp;

    private BaseGun m_CurrentBaseGun;
    public BaseGun CurrentBaseGun => m_CurrentBaseGun;
    private bool IsFiring;
    private bool IsDead;

    private EGunType m_GunType;
    private Dictionary<EGunType, BaseGun> gunDic;

    void Awake()
    {
        m_Hp = MaxHP;
        InitGun();
        LoadGun(EGunType.AK47);
    }

    void InitGun()
    {
        var guns = transform.GetComponentsInChildren<BaseGun>(true);
        gunDic = new Dictionary<EGunType, BaseGun>();
        foreach (var gun in guns)
        {
            gun.SetTarget(this);
            gunDic.Add(gun.GunType, gun);
        }
    }
    
    public void LoadGun(EGunType gunType)
    {
        m_GunType = gunType;
        foreach (var gun in gunDic)
        {
            gun.Value.gameObject.SetActive(false);
        }

        m_CurrentBaseGun = gunDic[gunType];
        m_CurrentBaseGun.Load();
    }

    private void Start()
    {
        if (photonView.IsMine)
        {
            GameUI.Instance.SetTarget(this);
        }
        else
        {
            GameObject uiGo = Instantiate(this.HealthUIPrefab);
            uiGo.GetComponent<PlayerUI>().SetTarget(this);
        }
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            IsFiring = Input.GetMouseButton(0);
            if (IsFiring)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        m_CurrentBaseGun.SetFire(IsFiring);
    }

    public void AddBullet()
    {
        CurrentBaseGun.AddBullet(30);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Supply"))
        {
            var supply = hit.collider.GetComponent<ISupply>();
            supply.Supply(this);
        }
    }

    public void OnDamage()
    {
        if (photonView.IsMine)
        {
            return;
        }

        if (IsDead)
        {
            return;
        }

        Debug.Log("hp:" + m_Hp);
        this.m_Hp -= 10;
        if (m_Hp <= 0)
        {
            IsDead = true;
            this.GetComponent<PlayerMovement>().SetDead(true);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(this.m_Hp);
            stream.SendNext(this.IsDead);
            stream.SendNext(this.IsFiring);
            stream.SendNext(m_GunType);
        }
        else
        {
            // Network player, receive data
            this.m_Hp = (int)stream.ReceiveNext();
            this.IsDead = (bool)stream.ReceiveNext();
            this.IsFiring = (bool)stream.ReceiveNext();
            var gunType = (EGunType)stream.ReceiveNext();

            if (gunType != m_GunType)
            {
                this.m_GunType = gunType;
                LoadGun(gunType);
            }
        }
    }
}