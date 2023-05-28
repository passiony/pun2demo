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

    public IGun m_CurrentGun;
    public IGun CurrentGun => m_CurrentGun;
    private bool IsFiring;
    private bool IsDead;

    private EGunType m_GunType;
    private Dictionary<EGunType, IGun> gunDic;

    void Awake()
    {
        var guns = transform.GetComponentsInChildren<IGun>();
        m_GunType = EGunType.Rifle;
        m_CurrentGun = guns[(int)m_GunType];

        m_CurrentGun.SetTarget(this);
        m_Hp = MaxHP;

        gunDic = new Dictionary<EGunType, IGun>();
        foreach (var gun in guns)
        {
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
        gunDic[gunType].Load();
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

        m_CurrentGun.SetFire(IsFiring);
    }

    public void AddBullet()
    {
        CurrentGun.AddBullet(30);
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