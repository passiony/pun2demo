// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Launcher.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking Demos
// </copyright>
// <summary>
//  Used in "PUN Basic tutorial" to handle typical game management requirements
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;

/// <summary>
/// Game manager.
/// Connects and watch Photon Status, Instantiate Player
/// Deals with quiting the room and the game
/// Deals with level loading (outside the in room synchronization)
/// </summary>
public class VRGameManager : MonoBehaviourPunCallbacks
{
    public static VRGameManager Instance;

    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject[] supplies;
    [SerializeField] private Transform[] bornPoints;

    [SerializeField] public int m_Interval = 20;
    [SerializeField] private int m_WinCount = 10;

    private Dictionary<string, int> m_ScoreBoard;
    public Dictionary<string, int> ScoreBoard => m_ScoreBoard;

    private bool m_GameOver;
    public bool GameOver => m_GameOver;

    private float timer;

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during initialization phase.
    /// </summary>
    void Start()
    {
        // in case we started this demo with the wrong scene being active, simply load the menu scene
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene("Launcher");
            return;
        }

        // #Tip Never assume public properties of Components are filled up properly, always check and inform the developer of it.
        if (playerPrefab == null)
        {
            Debug.LogError("<Missing playerPrefab Reference. Please set it up in GameObject 'Game Manager'",
                this);
        }
        else
        {
            int index = Random.Range(0, bornPoints.Length);
            var born = bornPoints[index];
            Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
            // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
            var go = PhotonNetwork.Instantiate(this.playerPrefab.name, born.position, born.rotation, 0);
            var tracking = go.GetComponent<IKTracking>();
            tracking.SetHeadVisible(false);
            XRPlayer.Instance.SetTracking(tracking);
            gameUI.SetActive(true);
        }

        //创建计分板
        if (PhotonNetwork.IsMasterClient)
        {
            m_ScoreBoard = new Dictionary<string, int>();
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "ScoreBoard", m_ScoreBoard } });
        }
        else //房主创建，非房主获取
        {
            m_ScoreBoard = (Dictionary<string, int>)PhotonNetwork.CurrentRoom.CustomProperties["ScoreBoard"];
        }
    }

    public Transform GetRandomBornPoint()
    {
        int index = Random.Range(0, bornPoints.Length);
        var born = bornPoints[index];

        return born;
    }

    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity on every frame.
    /// </summary>
    void Update()
    {
        // if (PhotonNetwork.IsMasterClient)
        // {
        //     timer += Time.deltaTime;
        //     if (timer > m_Interval)
        //     {
        //         timer = 0;
        //         LoadSupplies();
        //     }
        // }
    }

    /// <summary>
    /// Called when a Photon Player got connected. We need to then load a bigger scene.
    /// </summary>
    /// <param name="other">Other.</param>
    public override void OnPlayerEnteredRoom(Player other)
    {
        Debug.Log("OnPlayerEnteredRoom() " + other.NickName); // not seen if you're the player connecting

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}",
                PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
            LoadArena();
        }
    }

    /// <summary>
    /// Called when a Photon Player got disconnected. We need to load a smaller scene.
    /// </summary>
    /// <param name="other">Other.</param>
    public override void OnPlayerLeftRoom(Player other)
    {
        Debug.Log("OnPlayerLeftRoom() " + other.NickName); // seen when other disconnects

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}",
                PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
            LoadArena();
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("ScoreBoard"))
        {
            m_ScoreBoard = (Dictionary<string, int>)propertiesThatChanged["ScoreBoard"];

            CheckGameOver();
        }
    }

    /// <summary>
    /// Called when the local player left the room. We need to load the launcher scene.
    /// </summary>
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Launcher");
    }

    public void AddScore(string name, int score)
    {
        if (!m_ScoreBoard.ContainsKey(name))
        {
            m_ScoreBoard.Add(name, score);
        }
        else
        {
            m_ScoreBoard[name] += score;
        }

        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "ScoreBoard", m_ScoreBoard } });
    }

    public bool LeaveRoom()
    {
        return PhotonNetwork.LeaveRoom();
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

    void LoadArena()
    {
        // if ( ! PhotonNetwork.IsMasterClient )
        // {
        // 	Debug.LogError( "PhotonNetwork : Trying to Load a level but we are not the master Client" );
        // 	return;
        // }
        //
        // Debug.LogFormat( "PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount );
        //
        // PhotonNetwork.LoadLevel("PunBasics-Room for "+PhotonNetwork.CurrentRoom.PlayerCount);
    }

    void LoadSupplies()
    {
        int randon = Random.Range(0, 2);
        var prefab = supplies[randon];

        Vector3 point = new Vector3(Random.Range(-15, 15), 10, Random.Range(-20, 20));
        PhotonNetwork.InstantiateRoomObject(prefab.name, point, Quaternion.identity);
    }

    void CheckGameOver()
    {
        foreach (var player in m_ScoreBoard)
        {
            if (player.Value >= m_WinCount)
            {
                m_GameOver = true;
                if (PlayerController.LocalPlayer.photonView.Owner.NickName == player.Key)
                {
                    GameUI.Instance.ShowWinPanel();
                }
                else
                {
                    GameUI.Instance.ShowLosePanel();
                }
            }
        }
    }
}