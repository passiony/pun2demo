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
    private bool IsFiring;
    private bool IsDead;
    private EGunType m_GunType;

    private PlayerMovement m_Movement;
    private IKControl m_IkControl;
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
        m_Movement = this.GetComponent<PlayerMovement>();
        m_IkControl = this.GetComponent<IKControl>();
        InitGun();
        LoadGunSupply(EGunType.AK47);
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
        if (MyGameManager.Instance.GameOver) return;
        if (IsDead) return;

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

    /// <summary>
    /// 初始化枪支
    /// </summary>
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

    public void InitIK(Transform leftHandle, Transform rightHandle)
    {
        m_IkControl.ikActive = true;
        m_IkControl.lookObj = leftHandle;
        m_IkControl.leftHandObj = leftHandle;
        m_IkControl.rightHandObj = rightHandle;
    }

    /// <summary>
    /// 添加子弹补给
    /// </summary>
    public void LoadBulletSupply()
    {
        Debug.Log("子弹道具，添加子弹30发");
        CurrentBaseGun.AddBullet(30);
    }

    /// <summary>
    /// 加载枪支补给
    /// </summary>
    /// <param name="gunType"></param>
    public void LoadGunSupply(EGunType gunType)
    {
        m_GunType = gunType;
        foreach (var gun in gunDic)
        {
            gun.Value.gameObject.SetActive(false);
        }

        m_CurrentBaseGun = gunDic[gunType];
        m_CurrentBaseGun.Load();
    }

    /// <summary>
    /// 击杀角色+1分
    /// </summary>
    /// <param name="add"></param>
    public void AddScore(int add = 0)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        Debug.Log($"设置分数{add}分");
        MyGameManager.Instance.AddScore(photonView.Owner.NickName, add);
    }

    public void SetPosition(Transform point)
    {
        m_Movement.Reborn(point.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (photonView.IsMine)
        {
            if (other.CompareTag("Supply"))
            {
                var supply = other.GetComponent<ISupply>();
                supply.Supply(this);
            }
        }
    }

    //射击了他人
    public void AttachPlayer(PlayerController player)
    {
        if (photonView.IsMine) //他人执行Rpc
        {
            player.TakeDamage(photonView.ViewID);
        }
    }

    //他人被射击，转发给他的主机执行OnDamage
    public void TakeDamage(int sourceViewId)
    {
        photonView.RPC("OnDamege", RpcTarget.Others, sourceViewId);
    }

    [PunRPC]
    void OnDamege(int viewId)
    {
        if (!photonView.IsMine || IsDead)
        {
            return;
        }

        this.m_Hp -= 10;
        Debug.Log("OnDamege.HP:" + m_Hp);
        GameUI.Instance.ShowDamage();
        if (m_Hp <= 0)
        {
            photonView.RPC("OnDead", RpcTarget.All);

            //对方击杀了我，RPC通知他的主机
            PhotonView view = PhotonView.Find(viewId);
            view.RPC("OnKillPlayer", RpcTarget.Others);
        }
    }

    [PunRPC]
    void OnDead()
    {
        IsDead = true;
        m_Movement.Die();

        if (photonView.IsMine)
        {
            GameUI.Instance.ShowDeadPanel();
            StartCoroutine(CoDie());
        }
    }


    [PunRPC]
    void OnKillPlayer()
    {
        AddScore(1);
    }


    /// <summary>
    /// 死亡
    /// </summary>
    private IEnumerator CoDie()
    {
        yield return new WaitForSeconds(2);

        GameUI.Instance.ShowDeadPanel();
        photonView.RPC("Reboarn", RpcTarget.All);
    }

    /// <summary>
    /// 重生
    /// </summary>
    [PunRPC]
    private void Reboarn()
    {
        Debug.Log("重生");
        m_Hp = MaxHP;
        IsDead = false;
        LoadGunSupply(EGunType.Rifle);

        var born = MyGameManager.Instance.GetRandomBornPoint();
        this.m_Movement.Reborn(born.position);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(this.m_Hp);
            stream.SendNext(this.IsDead);
            stream.SendNext(this.IsFiring);
            stream.SendNext((int)m_GunType);
        }
        else
        {
            // Network player, receive data
            this.m_Hp = (int)stream.ReceiveNext();
            this.IsDead = (bool)stream.ReceiveNext();
            this.IsFiring = (bool)stream.ReceiveNext();
            var gunType = (EGunType)(int)stream.ReceiveNext();

            if (gunType != m_GunType)
            {
                this.m_GunType = gunType;
                LoadGunSupply(gunType);
            }
        }
    }
}