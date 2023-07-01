// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerListEntry.cs" company="Exit Games GmbH">
//   Part of: Asteroid Demo,
// </copyright>
// <summary>
//  Player List Entry
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

namespace Photon.Pun.MFPS
{
    public class MyPlayerListEntry : MonoBehaviour
    {
        [Header("UI References")] public Text PlayerNameText;

        public Image PlayerColorImage;
        public Button TeamSwitchButton;
        public Button PlayerReadyButton;
        public Image PlayerReadyImage;

        private int ownerId;
        private bool isPlayerReady;

        #region UNITY

        public void OnEnable()
        {
            MyPlayerNumbering.OnPlayerNumberingChanged += OnPlayerNumberingChanged;
        }

        public void OnDisable()
        {
            MyPlayerNumbering.OnPlayerNumberingChanged -= OnPlayerNumberingChanged;
        }

        public void Start()
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber != ownerId)
            {
                PlayerReadyButton.gameObject.SetActive(false);
                TeamSwitchButton.gameObject.SetActive(false);
            }
            else
            {
                Hashtable initialProps = new Hashtable()
                {
                    { FPSGame.PLAYER_READY, isPlayerReady },
                    { FPSGame.PLAYER_LIVES, FPSGame.PLAYER_MAX_LIVES },
                };
                PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps);
                PhotonNetwork.LocalPlayer.SetScore(0);

                PlayerReadyButton.onClick.AddListener(() =>
                {
                    isPlayerReady = !isPlayerReady;
                    // SetPlayerReady(isPlayerReady);

                    Hashtable props = new Hashtable() { { FPSGame.PLAYER_READY, isPlayerReady } };
                    PhotonNetwork.LocalPlayer.SetCustomProperties(props);

                    if (PhotonNetwork.IsMasterClient)
                    {
                        FindObjectOfType<MyLobbyMainPanel>().LocalPlayerPropertiesUpdated();
                    }
                });
                TeamSwitchButton.onClick.AddListener(() =>
                {
                    var teamId = PhotonNetwork.LocalPlayer.GetPhotonTeamCode();
                    var newTeam = PhotonTeamsManager.Instance.GetOppositeTeam(teamId);
                    if (PhotonTeamsManager.Instance.GetTeamMembersCount(newTeam) < 3)
                    {
                        PhotonNetwork.LocalPlayer.SwitchTeam(newTeam);
                    }
                    else
                    {
                        Debug.Log("opposite team members count exceeds the maximum");
                    }
                });
            }
        }

        #endregion

        public void Initialize(int playerId, string playerName, int teamCode)
        {
            ownerId = playerId;
            PlayerNameText.text = playerName;
            SetPlayerTeam(teamCode);
        }

        private void OnPlayerNumberingChanged()
        {
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                if (p.ActorNumber == ownerId)
                {
                    SetPlayerTeam(p.GetPhotonTeamCode());
                }
            }
        }

        public void SetPlayerReady(bool playerReady)
        {
            PlayerReadyButton.GetComponentInChildren<Text>().text = playerReady ? "Ready!" : "Ready?";
            PlayerReadyImage.enabled = playerReady;
        }

        public void SetPlayerTeam(int team)
        {
            PlayerColorImage.color = FPSGame.GetTeamColor(team);
        }
    }
}