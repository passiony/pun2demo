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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;

namespace Photon.Pun.Demo.PunBasics
{
    /// <summary>
    /// Game manager.
    /// Connects and watch Photon Status, Instantiate Player
    /// Deals with quiting the room and the game
    /// Deals with level loading (outside the in room synchronization)
    /// </summary>
    public class MyGameManager : MonoBehaviourPunCallbacks
    {
        static public MyGameManager Instance;
        private GameObject instance;

        [SerializeField] private GameObject gameUI;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject gunPrefab;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform[] bornPoints;
        private Dictionary<string, int> scoreBoard;

        public Dictionary<string, int> ScoreBoard => scoreBoard;
        public int Interval = 20;
        private float timer;

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        void Start()
        {
            Instance = this;
            // in case we started this demo with the wrong scene being active, simply load the menu scene
            if (!PhotonNetwork.IsConnected)
            {
                SceneManager.LoadScene("Launcher");
                return;
            }

            if (playerPrefab ==
                null) // #Tip Never assume public properties of Components are filled up properly, always check and inform the developer of it.
            {
                Debug.LogError(
                    "<Color=Red><b>Missing</b></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'",
                    this);
            }
            else
            {
                int index = PhotonNetwork.IsMasterClient ? 0 : 1;
                var born = bornPoints[index];
                Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                PhotonNetwork.Instantiate(this.playerPrefab.name, born.position, born.rotation, 0);
                gameUI.SetActive(true);
            }

            //创建计分板
            if (PhotonNetwork.IsMasterClient)
            {
                scoreBoard = new Dictionary<string, int>();
                PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "ScoreBoard", scoreBoard } });
            }
            else //房主创建，非房主获取
            {
                scoreBoard = (Dictionary<string, int>)PhotonNetwork.CurrentRoom.CustomProperties["ScoreBoard"];
            }
        }

        public void Reboarn(PlayerController player)
        {
            int index = PhotonNetwork.IsMasterClient ? 0 : 1;
            var born = bornPoints[index];

            player.SetPosition(born);
        }
        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity on every frame.
        /// </summary>
        void Update()
        {
            // "back" button of phone equals "Escape". quit app if that's pressed
            // if (Input.GetKeyDown(KeyCode.Escape))
            // {
            // 	QuitApplication();
            // }

            if (PhotonNetwork.IsMasterClient)
            {
                timer += Time.deltaTime;
                if (timer > Interval)
                {
                    timer = 0;
                    LoadSupplies();
                }
            }
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
                scoreBoard = (Dictionary<string, int>)propertiesThatChanged["ScoreBoard"];
            }
        }

        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene("Launcher");
        }

        public void SetScore(string name, int score)
        {
            if (!scoreBoard.ContainsKey(name))
            {
                scoreBoard.Add(name, score);
            }
            else
            {
                scoreBoard[name] = score;
            }

            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "ScoreBoard", scoreBoard } });
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
            Vector3 point = new Vector3(Random.Range(-10, 10), 10, Random.Range(-10, 10));
            PhotonNetwork.InstantiateRoomObject(gunPrefab.name, point, Quaternion.identity);
        }
        
    }
}