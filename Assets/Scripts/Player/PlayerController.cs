using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using UnityEngine;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    public int MaxHP => 100;
    public GameObject HealthUIPrefab;
    public static PlayerController LocalPlayer;
    
    private int m_Hp;
    private int m_Score;
    private bool IsFiring;
    private bool IsDead;
    private EGunType m_GunType;

    private PlayerMovement movement;
    private BaseGun m_CurrentBaseGun;
    public BaseGun CurrentBaseGun => m_CurrentBaseGun;
    public int HP => m_Hp;

    private Dictionary<EGunType, BaseGun> gunDic;

    void Awake()
    {
        // used in GameManager.cs: we keep track of the localPlayer instance to prevent instanciation when levels are synchronized
        if (photonView.IsMine)
        {
            LocalPlayer = this;
        }
        
        m_Hp = MaxHP;
        movement = this.GetComponent<PlayerMovement>();
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
            AddScore();
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

    public void AddScore(int add = 0)
    {
        m_Score += add;
        MyGameManager.Instance.SetScore(photonView.Owner.NickName, m_Score);
    }

    public void SetPosition(Transform point)
    {
        transform.position = point.position;
        transform.rotation = point.rotation;
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("player.OnTriggerEnter:" + other.name);
        if (other.CompareTag("Supply"))
        {
            var supply = other.GetComponent<ISupply>();
            supply.Supply(this);
        }
    }

    public bool OnDamage()
    {
        if (!photonView.IsMine)
        {
            return false;
        }

        if (IsDead)
        {
            return false;
        }

        Debug.Log("hp:" + m_Hp);
        this.m_Hp -= 10;
        if (m_Hp <= 0)
        {
            IsDead = true;
            Reboarn();
            return true;
        }

        return false;
    }

    private void Reboarn()
    {
        Debug.Log("重生");
        m_Hp = MaxHP;
        IsDead = false;
        this.movement.SetDead(false);
        MyGameManager.Instance.Reboarn(this);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(this.m_Hp);
            stream.SendNext(this.m_Score);
            stream.SendNext(this.IsDead);
            stream.SendNext(this.IsFiring);
            stream.SendNext((int)m_GunType);
        }
        else
        {
            // Network player, receive data
            this.m_Hp = (int)stream.ReceiveNext();
            this.m_Score = (int)stream.ReceiveNext();
            this.IsDead = (bool)stream.ReceiveNext();
            this.IsFiring = (bool)stream.ReceiveNext();
            var gunType = (EGunType)(int)stream.ReceiveNext();

            if (gunType != m_GunType)
            {
                this.m_GunType = gunType;
                LoadGun(gunType);
            }
        }
    }
}