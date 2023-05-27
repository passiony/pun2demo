using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    public GameObject HealthUIPrefab;
    public int MaxHP => 100;
    private int m_Hp;
    public int HP => m_Hp;

    private IGun m_Gun;
    public IGun Gun => m_Gun;
    bool IsFiring;

    void Awake()
    {
        m_Gun = transform.GetComponentInChildren<IGun>();
        m_Gun.SetTarget(this);
        m_Hp = MaxHP;
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

        m_Gun.SetFire(IsFiring);
    }

    public void OnDamage()
    {
        if (photonView.IsMine)
        {
            return;
        }

        Debug.Log("hp:" + m_Hp);
        this.m_Hp -= 10;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(this.IsFiring);
            stream.SendNext(this.m_Hp);
        }
        else
        {
            // Network player, receive data
            this.IsFiring = (bool)stream.ReceiveNext();
            this.m_Hp = (int)stream.ReceiveNext();
        }
    }
}