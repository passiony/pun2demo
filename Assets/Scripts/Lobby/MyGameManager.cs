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
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace MFPS
{
#pragma warning disable 649
    public class MyGameManager : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private class TeamPoint
        {
            public string TeamCode;
            public Transform[] Points;
        }
        
        static public MyGameManager Instance;

        private GameObject instance;

        public bool StartGame;

        [SerializeField] private TeamPoint[] bornPoints;

        [SerializeField] private GameObject playerPrefab;

        #region MonoBehaviour CallBacks

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        void Start()
        {
            Instance = this;

            // in case we started this demo with the wrong scene being active, simply load the menu scene
            if (!PhotonNetwork.IsConnected)
            {
                SceneManager.LoadScene("LobbyScene");
                return;
            }

            if (playerPrefab == null)
            {
                // #Tip Never assume public properties of Components are filled up properly, always check and inform the developer of it.

                Debug.LogError(
                    "<Color=Red><b>Missing</b></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'",
                    this);
            }
            else
            {
                if (MyPlayerManager.LocalPlayerInstance == null)
                {
                    Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                    
                    var team = PhotonNetwork.LocalPlayer.GetPhotonTeamCode();
                    var born = GetBornPoit(team);
                    PhotonNetwork.Instantiate(this.playerPrefab.name, born.position, Quaternion.identity, 0);
                }
                else
                {
                    Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
                }
            }
        }

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity on every frame.
        /// </summary>
        void Update()
        {
            // "back" button of phone equals "Escape". quit app if that's pressed
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                QuitApplication();
            }
        }

        #endregion

        #region Photon Callbacks

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
                // LoadArena();
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
                // LoadArena(); 
            }
        }

        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene("LobbyScene");
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            object props;
            if (changedProps.TryGetValue(MyPunTeamScores.PlayerScoreProp, out props))
            {
                var scores = (int[])props;
                CheckEndOfGame(scores);
            }
        }

        #endregion

        private void CheckEndOfGame(int[] scores)
        {
            bool gameOver = false;
            int winTeam = 0;
            for (int i = 0; i < scores.Length; i++)
            {
                if (scores[i] >= 30)
                {
                    winTeam = i;
                    gameOver = true;
                    break;
                }
            }

            if (gameOver)
            {
                var win = PhotonNetwork.LocalPlayer.GetPhotonTeamCode() == winTeam;
                if (win)
                {
                    GameUI.Instance.ShowWinPanel();
                }
                else
                {
                    GameUI.Instance.ShowLosePanel();
                }
            }
        }

        public Transform GetBornPoit(int teamCode)
        {
            var teamPoint = bornPoints[teamCode - 1];
            var born = teamPoint.Points[Random.Range(0, teamPoint.Points.Length)];
            return born;
        }
        public bool LeaveRoom()
        {
            return PhotonNetwork.LeaveRoom();
        }

        public void QuitApplication()
        {
            Application.Quit();
        }
    }
}