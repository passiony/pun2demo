using System;
using ExitGames.Client.Photon;
using Photon.Realtime;
using System.Collections.Generic;
using Photon.Pun.Demo.Asteroids;
using Photon.Pun.Demo.PunBasics;
using Photon.Pun.UtilityScripts;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Photon.Pun.MFPS
{
    public class MyLobbyMainPanel : MonoBehaviourPunCallbacks
    {
        [Header("Loading")] [SerializeField] private LoaderAnime loaderAnime;

        [Header("Login Panel")] [SerializeField]
        private GameObject LoginPanel;

        [SerializeField] private InputField PlayerNameInput;
        [SerializeField] private InputField AddressInput;

        [Header("Selection Panel")] [SerializeField]
        private GameObject SelectionPanel;

        [Header("Create Room Panel")] [SerializeField]
        private GameObject CreateRoomPanel;

        [SerializeField] private InputField RoomNameInputField;
        [SerializeField] private InputField MaxPlayersInputField;

        [Header("Join Random Room Panel")] [SerializeField]
        private GameObject JoinRandomRoomPanel;

        [Header("Room List Panel")] [SerializeField]
        private GameObject RoomListPanel;

        [SerializeField] private GameObject RoomListContent;
        [SerializeField] private GameObject RoomListEntryPrefab;

        [Header("Inside Room Panel")] [SerializeField]
        private GameObject InsideRoomPanel;

        [SerializeField] private Transform BlueTeamPanel;
        [SerializeField] private Transform RedTeamPanel;

        [SerializeField] private Button StartGameButton;
        [SerializeField] private GameObject PlayerListEntryPrefab;

        private Dictionary<string, RoomInfo> cachedRoomList;
        private Dictionary<string, GameObject> roomListEntries;
        private Dictionary<int, GameObject> playerListEntries;
        private string gameVersion = "0.0.1";
        private bool isConnecting;

        public void Awake()
        {
            if (loaderAnime == null)
            {
                Debug.LogError("<Color=Red><b>Missing</b></Color> loaderAnime Reference.", this);
            }

            PhotonNetwork.AutomaticallySyncScene = true;
            cachedRoomList = new Dictionary<string, RoomInfo>();
            roomListEntries = new Dictionary<string, GameObject>();
        }

        private void Start()
        {
            SetActivePanel(LoginPanel.name);
            PlayerNameInput.text = "Player" + Random.Range(1000, 10000);
        }

        #region PUN CALLBACKS

        public override void OnConnectedToMaster()
        {
            this.SetActivePanel(SelectionPanel.name);
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            ClearRoomListView();

            UpdateCachedRoomList(roomList);
            UpdateRoomListView();
        }

        public override void OnJoinedLobby()
        {
            // whenever this joins a new lobby, clear any previous room lists
            cachedRoomList.Clear();
            ClearRoomListView();
        }

        // note: when a client joins / creates a room, OnLeftLobby does not get called, even if the client was in a lobby before
        public override void OnLeftLobby()
        {
            cachedRoomList.Clear();
            ClearRoomListView();
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            SetActivePanel(SelectionPanel.name);
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            SetActivePanel(SelectionPanel.name);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            string roomName = "Room " + Random.Range(1000, 10000);

            RoomOptions options = new RoomOptions { MaxPlayers = 8 };

            PhotonNetwork.CreateRoom(roomName, options, null);
        }

        public override void OnJoinedRoom()
        {
            // joining (or entering) a room invalidates any cached lobby room list (even if LeaveLobby was not called due to just joining a room)
            cachedRoomList.Clear();

            SetActivePanel(InsideRoomPanel.name);

            if (playerListEntries == null)
            {
                playerListEntries = new Dictionary<int, GameObject>();
            }

            foreach (Player p in PhotonNetwork.PlayerList)
            {
                var team = p.GetPhotonTeam();
                if (team == null)
                {
                    team = PhotonTeamsManager.Instance.GetLessMemberCountTeam();
                    p.JoinTeam(team);
                }

                var teamParent = GetTeamParent(team.Code);
                GameObject entry = Instantiate(PlayerListEntryPrefab, teamParent, false);
                entry.SetActive(true);
                entry.transform.localScale = Vector3.one;
                entry.GetComponent<MyPlayerListEntry>().Initialize(p.ActorNumber, p.NickName, team.Code);

                object isPlayerReady;
                if (p.CustomProperties.TryGetValue(GameUtility.PLAYER_READY, out isPlayerReady))
                {
                    entry.GetComponent<MyPlayerListEntry>().SetPlayerReady((bool)isPlayerReady);
                }

                playerListEntries.Add(p.ActorNumber, entry);
            }

            StartGameButton.gameObject.SetActive(CheckPlayersReady());

            Hashtable props = new Hashtable
            {
                { GameUtility.PLAYER_LOADED_LEVEL, false }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        public override void OnLeftRoom()
        {
            SetActivePanel(SelectionPanel.name);

            foreach (GameObject entry in playerListEntries.Values)
            {
                Destroy(entry.gameObject);
            }

            playerListEntries.Clear();
            playerListEntries = null;
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            var team = newPlayer.GetPhotonTeam();
            var teamParent = GetTeamParent(team.Code);

            GameObject entry = Instantiate(PlayerListEntryPrefab, teamParent, false);
            entry.SetActive(true);
            entry.transform.localScale = Vector3.one;
            entry.GetComponent<MyPlayerListEntry>().Initialize(newPlayer.ActorNumber, newPlayer.NickName, team.Code);

            playerListEntries.Add(newPlayer.ActorNumber, entry);

            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
            playerListEntries.Remove(otherPlayer.ActorNumber);

            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
            {
                StartGameButton.gameObject.SetActive(CheckPlayersReady());
            }
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (playerListEntries == null)
            {
                playerListEntries = new Dictionary<int, GameObject>();
            }

            GameObject entry;
            if (playerListEntries.TryGetValue(targetPlayer.ActorNumber, out entry))
            {
                object isPlayerReady;
                if (changedProps.TryGetValue(GameUtility.PLAYER_READY, out isPlayerReady))
                {
                    entry.GetComponent<MyPlayerListEntry>().SetPlayerReady((bool)isPlayerReady);
                }

                object teamCode;
                if (changedProps.TryGetValue(PhotonTeamsManager.TeamPlayerProp, out teamCode))
                {
                    var teamParent = GetTeamParent((byte)teamCode);
                    entry.transform.SetParent(teamParent, false);
                    entry.GetComponent<MyPlayerListEntry>().SetPlayerTeam((byte)teamCode);
                }
            }

            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        #endregion

        #region UI CALLBACKS

        public void OnBackButtonClicked()
        {
            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.LeaveLobby();
            }

            SetActivePanel(SelectionPanel.name);
        }

        public void OnCreateRoomButtonClicked()
        {
            string roomName = RoomNameInputField.text;
            roomName = (roomName.Equals(string.Empty)) ? "Room " + Random.Range(1000, 10000) : roomName;

            byte maxPlayers;
            byte.TryParse(MaxPlayersInputField.text, out maxPlayers);
            maxPlayers = (byte)Mathf.Clamp(maxPlayers, 2, 8);

            RoomOptions options = new RoomOptions { MaxPlayers = maxPlayers, PlayerTtl = 10000 };

            PhotonNetwork.CreateRoom(roomName, options, null);
        }

        public void OnJoinRandomRoomButtonClicked()
        {
            SetActivePanel(JoinRandomRoomPanel.name);

            PhotonNetwork.JoinRandomRoom();
        }

        public void OnLeaveGameButtonClicked()
        {
            PhotonNetwork.LeaveRoom();
        }

        public void OnLoginButtonClicked()
        {
            isConnecting = true;
            loaderAnime?.StartLoaderAnimation();

            string playerName = PlayerNameInput.text;
            string address = AddressInput.text;

            if (!playerName.Equals(""))
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;
                PhotonNetwork.GameVersion = this.gameVersion;
                // PhotonNetwork.ConnectUsingSettings();

                PhotonNetwork.ConnectToMaster(address, PhotonNetwork.PhotonServerSettings.AppSettings.Port,
                    PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime);
            }
            else
            {
                Debug.LogError("Player Name is invalid.");
            }
        }

        public void OnRoomListButtonClicked()
        {
            if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby();
            }

            SetActivePanel(RoomListPanel.name);
        }

        public void OnStartGameButtonClicked()
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;

            PhotonNetwork.LoadLevel("GameScene");
        }

        #endregion

        private bool CheckPlayersReady()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return false;
            }

            foreach (Player p in PhotonNetwork.PlayerList)
            {
                object isPlayerReady;
                if (p.CustomProperties.TryGetValue(GameUtility.PLAYER_READY, out isPlayerReady))
                {
                    if (!(bool)isPlayerReady)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private void ClearRoomListView()
        {
            foreach (GameObject entry in roomListEntries.Values)
            {
                Destroy(entry.gameObject);
            }

            roomListEntries.Clear();
        }

        public void LocalPlayerPropertiesUpdated()
        {
            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        public void SetActivePanel(string activePanel)
        {
            LoginPanel.SetActive(activePanel.Equals(LoginPanel.name));
            SelectionPanel.SetActive(activePanel.Equals(SelectionPanel.name));
            CreateRoomPanel.SetActive(activePanel.Equals(CreateRoomPanel.name));
            JoinRandomRoomPanel.SetActive(activePanel.Equals(JoinRandomRoomPanel.name));
            RoomListPanel.SetActive(
                activePanel.Equals(RoomListPanel.name)); // UI should call OnRoomListButtonClicked() to activate this
            InsideRoomPanel.SetActive(activePanel.Equals(InsideRoomPanel.name));
        }

        private void UpdateCachedRoomList(List<RoomInfo> roomList)
        {
            foreach (RoomInfo info in roomList)
            {
                // Remove room from cached room list if it got closed, became invisible or was marked as removed
                if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
                {
                    if (cachedRoomList.ContainsKey(info.Name))
                    {
                        cachedRoomList.Remove(info.Name);
                    }

                    continue;
                }

                // Update cached room info
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList[info.Name] = info;
                }
                // Add new room info to cache
                else
                {
                    cachedRoomList.Add(info.Name, info);
                }
            }
        }

        private void UpdateRoomListView()
        {
            foreach (RoomInfo info in cachedRoomList.Values)
            {
                GameObject entry = Instantiate(RoomListEntryPrefab);
                entry.transform.SetParent(RoomListContent.transform);
                entry.transform.localScale = Vector3.one;
                entry.GetComponent<RoomListEntry>()
                    .Initialize(info.Name, (byte)info.PlayerCount, (byte)info.MaxPlayers);

                roomListEntries.Add(info.Name, entry);
            }
        }

        // new PhotonTeam { Name = "Blue", Code = 1 },
        // new PhotonTeam { Name = "Red", Code = 2 }
        private Transform GetTeamParent(int code)
        {
            if (code == 1)
            {
                return BlueTeamPanel;
            }
            else
            {
                return RedTeamPanel;
            }
        }
    }
}