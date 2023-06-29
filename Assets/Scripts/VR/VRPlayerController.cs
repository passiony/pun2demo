using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.MFPS;
using UnityEngine;

public class VRPlayerController : MonoBehaviourPunCallbacks, IPunObservable, IDamageOriginator, IDamageTarget
{
    public GameObject Owner { get; set; }
    public GameObject OriginatingGameObject { get; set; }
    public GameObject HitGameObject { get; set; }
    public int MaxHP => 100;
    private int m_Hp;
    private int m_KillCount;
    
    private WeaponComponent m_WeaponComponent;
    private void Awake()
    {
        m_Hp = MaxHP;
        Owner = gameObject;
        OriginatingGameObject = gameObject;
        HitGameObject = gameObject;

        m_WeaponComponent = this.GetComponentInChildren<WeaponComponent>();
        if (photonView.IsMine)
        {
            XRPlayer.Instance.SetPlayer(this);
        }
    }

    public void Fire()
    {
        photonView.RPC("SyncFire",RpcTarget.All);
    }

    public void SwitchGun()
    {
        photonView.RPC("SyncSwtichGun",RpcTarget.All);
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
        m_Hp -= (int)damageData.Amount;
        var sourceViewId = damageData.DamageOriginator.OriginatingGameObject.GetPhotonView().ViewID;
        photonView.RPC("OnApplyDamage", RpcTarget.All, damageData.Amount, sourceViewId);
    }

    [PunRPC]
    void OnApplyDamage(int damageAmount, int sourceViewId, PhotonMessageInfo info)
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
    }
    
    public bool IsAlive()
    {
        return m_Hp > 0;
    }

    void OnDead()
    {
        Debug.Log("死亡动画");
        GameUI.Instance.ShowDeadPanel();
        
        if (photonView.IsMine)
        {
            Debug.Log("重生");
        }
    }
}