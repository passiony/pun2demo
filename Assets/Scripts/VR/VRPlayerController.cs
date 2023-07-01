using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;
using ScoreExtensions = Photon.Pun.MFPS.ScoreExtensions;

public class VRPlayerController : MonoBehaviourPunCallbacks, IPunObservable, IDamageOriginator, IDamageTarget
{
    public GameObject Owner { get; set; }
    public GameObject OriginatingGameObject { get; set; }
    public GameObject HitGameObject { get; set; }
    public int MaxHP => 100;
    private int m_Hp;
    private int m_KillCount;

    private WeaponComponent m_WeaponComponent;
    private IKTracking m_IkTracking;
    private MyPlayerUI m_PlayerUI;
    private VRAvatar m_Avatar;

    public int HP => m_Hp;
    public int KillCount => m_KillCount;
    public WeaponComponent WeaponComponent => m_WeaponComponent;
    public VRAvatar Avatar => m_Avatar;

    public bool IsAlive()
    {
        return m_Hp > 0;
    }

    private void Awake()
    {
        m_Hp = MaxHP;
        Owner = gameObject;
        OriginatingGameObject = gameObject;
        HitGameObject = gameObject;

        m_WeaponComponent = this.GetComponentInChildren<WeaponComponent>();
        m_IkTracking = this.GetComponent<IKTracking>();
        m_Avatar = m_IkTracking.Avatar;

        if (photonView.IsMine)
        {
            XRPlayer.Instance.SetPlayer(this);
        }

        InitPlayerUI();
        m_IkTracking.UseRagdoll(true);
    }

    void InitPlayerUI()
    {
        if (photonView.IsMine)
        {
            m_PlayerUI = transform.GetComponentInChildren<MyPlayerUI>(true);
            m_PlayerUI.gameObject.SetActive(true);
            m_PlayerUI.SetTarget(this, false);
        }
        else
        {
            var prefab = Resources.Load<GameObject>("PlayerUI");
            var go = GameObject.Instantiate(prefab);
            m_PlayerUI = go.GetComponent<MyPlayerUI>();
            m_PlayerUI.SetTarget(this, true);
        }
    }

    public bool IsSameTeam(VRPlayerController other)
    {
        return this.photonView.Owner.GetPhotonTeamCode() == other.photonView.Owner.GetPhotonTeamCode();
    }

    public void Fire()
    {
        photonView.RPC("SyncFire", RpcTarget.All);
    }

    public void SwitchGun()
    {
        photonView.RPC("SyncSwtichGun", RpcTarget.All);
    }

    [PunRPC]
    void SyncFire()
    {
        m_WeaponComponent.UseWeapon();
    }

    [PunRPC]
    void SyncSwtichGun()
    {
        m_WeaponComponent.SwitchWeapon();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(this.m_Hp);
            stream.SendNext(this.m_KillCount);
        }
        else
        {
            // Network player, receive data
            this.m_Hp = (int)stream.ReceiveNext();
            this.m_KillCount = (int)stream.ReceiveNext();
        }
    }

    public void Damage(DamageData damageData)
    {
        var damage = damageData.Amount;
        if (damageData.HitCollider.CompareTag("Player"))
        {
            var part = damageData.HitCollider.GetComponent<PlayerBody>();
            damage = part.GetRealDamage(damage);
        }

        m_Hp -= (int)damage;
        var sourceViewId = damageData.DamageOriginator.OriginatingGameObject.GetPhotonView().ViewID;
        photonView.RPC("OnApplyDamage", RpcTarget.All, (int)damage, sourceViewId);
    }

    [PunRPC]
    void OnApplyDamage(int damageAmount, int sourceViewId)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        Debug.Log("OnApplyDamage");
        if (!IsAlive())
        {
            return;
        }

        m_Hp -= damageAmount;
        GameUI.Instance.ShowDamage();

        if (IsAlive())
        {
            return;
        }

        photonView.RPC("OnDead", RpcTarget.All);

        //被对方击杀，RPC，通知对方主机
        PhotonView view = PhotonView.Find(sourceViewId);
        view.RPC("OnKillPlayer", RpcTarget.All);
    }

    [PunRPC]
    void OnKillPlayer()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        Debug.Log("OnKillPlayer");
        m_KillCount++;
        ScoreExtensions.AddScore(photonView.Owner, 1);
    }

    [PunRPC]
    void OnDead()
    {
        Debug.Log("死亡");
        // GameUI.Instance.ShowDeadPanel();
        if (m_PlayerUI)
            this.m_PlayerUI.gameObject.SetActive(false);
        this.m_IkTracking.UseRagdoll(false);
        m_WeaponComponent.DropWeapon();
        m_IkTracking.SetHeadVisible(true);

        if (photonView.IsMine)
        {
            Debug.Log("重生");
            GameUI.Instance.ShowDeadPanel(() => { photonView.RPC("OnReborn", RpcTarget.All); });
        }
    }

    [PunRPC]
    void OnReborn()
    {
        Debug.Log("重生");
        m_Hp = MaxHP;
        if (m_PlayerUI)
            m_PlayerUI.gameObject.SetActive(true);
        m_IkTracking.UseRagdoll(true);
        m_WeaponComponent.LoadWeapon(EGunID.Pistol);
        if (photonView.Owner.IsLocal)
            m_IkTracking.SetHeadVisible(false);
        else
            m_IkTracking.SetHeadVisible(true);
    }

    [SerializeField] private bool m_Dead = false;
    private bool m_LastDead = false;

    private void Update()
    {
        if (m_Dead == m_LastDead)
        {
            return;
        }

        m_LastDead = m_Dead;
        if (m_Dead)
        {
            OnDead();
        }
        else
        {
            OnReborn();
        }
    }
}