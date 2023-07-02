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
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;

namespace MFPS
{
    public class MyGameManager : MonoBehaviourPunCallbacks
    {
        [Serializable]
        private class TeamPoint
        {
            public string TeamCode;
            public Transform[] Points;
        }

        public static MyGameManager Instance;

        [SerializeField] private GameObject gameUI;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject[] supplies;
        [SerializeField] private TeamPoint[] bornPoints;

        [SerializeField] public int m_Interval = 20;
        [SerializeField] private int m_WinCount = 10;

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
                var team = PhotonNetwork.LocalPlayer.GetPhotonTeamCode();
                var born = GetBornPoit(team);

                Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                var go = PhotonNetwork.Instantiate(this.playerPrefab.name, born.position, born.rotation, 0);
                var tracking = go.GetComponent<IKTracking>();
                XRPlayer.Instance.SetTracking(tracking, born);
                
            }
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

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            if (propertiesThatChanged.ContainsKey(MyPunTeamScores.PlayerScoreProp))
            {
                var scores = ScoreExtensions.GetScores();

                GameUI.Instance.RefreshScoreBorad(scores);
                CheckGameOver(scores);
            }
        }

        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene("Launcher");
        }

        public bool LeaveRoom()
        {
            return PhotonNetwork.LeaveRoom();
        }

        public void QuitApplication()
        {
            Application.Quit();
        }

        void LoadSupplies()
        {
            int randon = Random.Range(0, 2);
            var prefab = supplies[randon];

            Vector3 point = new Vector3(Random.Range(-15, 15), 10, Random.Range(-20, 20));
            PhotonNetwork.InstantiateRoomObject(prefab.name, point, Quaternion.identity);
        }

        public Transform GetBornPoit(int teamCode)
        {
            var teamPoint = bornPoints[teamCode - 1];
            var born = teamPoint.Points[Random.Range(0, teamPoint.Points.Length)];
            return born;
        }

        void CheckGameOver(int[] scores)
        {
            for (int i = 1; i < scores.Length; i++)
            {
                if (scores[i] >= m_WinCount)
                {
                    m_GameOver = true;
                    if (PhotonNetwork.LocalPlayer.GetPhotonTeamCode() == i)
                    {
                        GameUI.Instance.ShowWinPanel();
                    }
                    else
                    {
                        GameUI.Instance.ShowLosePanel();
                    }
                    break;
                }
            }
        }
    }
}