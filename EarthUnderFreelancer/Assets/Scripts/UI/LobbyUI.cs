using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using EarthUnderFreelancer.Core;
using EarthUnderFreelancer.Networking;

namespace EarthUnderFreelancer.UI
{
    /// <summary>
    /// Lobby UI controller
    /// </summary>
    public class LobbyUI : MonoBehaviour
    {
        [Header("Room List")]
        [SerializeField] private GameObject roomListPanel;
        [SerializeField] private Transform roomListContainer;
        [SerializeField] private GameObject roomItemPrefab;
        [SerializeField] private Button refreshButton;
        [SerializeField] private Button createRoomButton;
        [SerializeField] private Button quickMatchButton;

        [Header("Room Creation")]
        [SerializeField] private GameObject createRoomPanel;
        [SerializeField] private InputField roomNameInput;
        [SerializeField] private Dropdown gameModeDropdown;
        [SerializeField] private Dropdown mapDropdown;
        [SerializeField] private Slider maxPlayersSlider;
        [SerializeField] private Text maxPlayersText;
        [SerializeField] private Button confirmCreateButton;
        [SerializeField] private Button cancelCreateButton;

        [Header("In-Room")]
        [SerializeField] private GameObject inRoomPanel;
        [SerializeField] private Text roomTitleText;
        [SerializeField] private Transform playerListContainer;
        [SerializeField] private GameObject playerItemPrefab;
        [SerializeField] private Button readyButton;
        [SerializeField] private Button startButton;
        [SerializeField] private Button leaveButton;
        [SerializeField] private Text countdownText;

        [Header("Team Selection")]
        [SerializeField] private Button team1Button;
        [SerializeField] private Button team2Button;

        private List<GameObject> roomItems = new List<GameObject>();
        private List<GameObject> playerItems = new List<GameObject>();

        private void Start()
        {
            SetupButtons();
            SubscribeToEvents();
            ShowRoomList();
        }

        private void SetupButtons()
        {
            refreshButton?.onClick.AddListener(RefreshRoomList);
            createRoomButton?.onClick.AddListener(ShowCreateRoomPanel);
            quickMatchButton?.onClick.AddListener(QuickMatch);
            confirmCreateButton?.onClick.AddListener(CreateRoom);
            cancelCreateButton?.onClick.AddListener(HideCreateRoomPanel);
            readyButton?.onClick.AddListener(ToggleReady);
            startButton?.onClick.AddListener(StartMatch);
            leaveButton?.onClick.AddListener(LeaveRoom);
            team1Button?.onClick.AddListener(() => SelectTeam(1));
            team2Button?.onClick.AddListener(() => SelectTeam(2));

            if (maxPlayersSlider != null)
            {
                maxPlayersSlider.onValueChanged.AddListener(value =>
                {
                    if (maxPlayersText != null)
                        maxPlayersText.text = $"Max Players: {(int)value}";
                });
            }
        }

        private void SubscribeToEvents()
        {
            if (NetworkManager.Instance != null)
            {
                NetworkManager.Instance.OnRoomListUpdated += UpdateRoomList;
                NetworkManager.Instance.OnJoinedRoom += OnJoinedRoom;
                NetworkManager.Instance.OnLeftRoom += OnLeftRoom;
                NetworkManager.Instance.OnPlayerJoined += OnPlayerChanged;
                NetworkManager.Instance.OnPlayerLeft += OnPlayerChanged;
            }

            if (LobbyManager.Instance != null)
            {
                LobbyManager.Instance.OnCountdownTick += OnCountdownTick;
                LobbyManager.Instance.OnCountdownStarted += OnCountdownStarted;
                LobbyManager.Instance.OnCountdownCancelled += OnCountdownCancelled;
            }
        }

        private void OnDestroy()
        {
            if (NetworkManager.Instance != null)
            {
                NetworkManager.Instance.OnRoomListUpdated -= UpdateRoomList;
                NetworkManager.Instance.OnJoinedRoom -= OnJoinedRoom;
                NetworkManager.Instance.OnLeftRoom -= OnLeftRoom;
                NetworkManager.Instance.OnPlayerJoined -= OnPlayerChanged;
                NetworkManager.Instance.OnPlayerLeft -= OnPlayerChanged;
            }

            if (LobbyManager.Instance != null)
            {
                LobbyManager.Instance.OnCountdownTick -= OnCountdownTick;
                LobbyManager.Instance.OnCountdownStarted -= OnCountdownStarted;
                LobbyManager.Instance.OnCountdownCancelled -= OnCountdownCancelled;
            }
        }

        private void ShowRoomList()
        {
            roomListPanel?.SetActive(true);
            createRoomPanel?.SetActive(false);
            inRoomPanel?.SetActive(false);
            RefreshRoomList();
        }

        private void ShowCreateRoomPanel()
        {
            AudioManager.Instance?.PlayButtonClick();
            createRoomPanel?.SetActive(true);
        }

        private void HideCreateRoomPanel()
        {
            AudioManager.Instance?.PlayButtonClick();
            createRoomPanel?.SetActive(false);
        }

        private void RefreshRoomList()
        {
            AudioManager.Instance?.PlayButtonClick();
            NetworkManager.Instance?.RefreshRoomList();
        }

        private void UpdateRoomList(List<RoomInfo> rooms)
        {
            // Clear existing items
            foreach (var item in roomItems) Destroy(item);
            roomItems.Clear();

            // Create new items
            foreach (var room in rooms)
            {
                if (roomItemPrefab != null && roomListContainer != null)
                {
                    GameObject item = Instantiate(roomItemPrefab, roomListContainer);
                    
                    // Setup room item
                    Text[] texts = item.GetComponentsInChildren<Text>();
                    if (texts.Length > 0) texts[0].text = room.roomName;
                    if (texts.Length > 1) texts[1].text = $"{room.playerCount}/{room.maxPlayers}";
                    if (texts.Length > 2) texts[2].text = room.gameMode.ToString();

                    Button joinButton = item.GetComponentInChildren<Button>();
                    if (joinButton != null)
                    {
                        string roomId = room.roomId;
                        joinButton.onClick.AddListener(() => JoinRoom(roomId));
                    }

                    roomItems.Add(item);
                }
            }
        }

        private void JoinRoom(string roomId)
        {
            AudioManager.Instance?.PlayButtonClick();
            NetworkManager.Instance?.JoinRoom(roomId);
        }

        private void CreateRoom()
        {
            AudioManager.Instance?.PlayButtonClick();
            
            string roomName = roomNameInput?.text ?? $"Room_{Random.Range(1000, 9999)}";
            GameMode gameMode = gameModeDropdown != null ? (GameMode)gameModeDropdown.value : GameMode.TeamDeathmatch;
            string mapName = mapDropdown != null ? mapDropdown.options[mapDropdown.value].text : "Arena_01";
            int maxPlayers = maxPlayersSlider != null ? (int)maxPlayersSlider.value : 8;

            NetworkManager.Instance?.CreateRoom(roomName, gameMode, mapName, maxPlayers);
        }

        private void QuickMatch()
        {
            AudioManager.Instance?.PlayButtonClick();
            NetworkManager.Instance?.JoinRandomRoom();
        }

        private void OnJoinedRoom(RoomInfo room)
        {
            roomListPanel?.SetActive(false);
            createRoomPanel?.SetActive(false);
            inRoomPanel?.SetActive(true);

            if (roomTitleText != null)
                roomTitleText.text = room.roomName;

            UpdatePlayerList();
            UpdateStartButton();
        }

        private void OnLeftRoom()
        {
            ShowRoomList();
        }

        private void OnPlayerChanged(NetworkPlayer player)
        {
            UpdatePlayerList();
            UpdateStartButton();
        }

        private void UpdatePlayerList()
        {
            foreach (var item in playerItems) Destroy(item);
            playerItems.Clear();

            List<NetworkPlayer> players = NetworkManager.Instance?.GetAllPlayers();
            if (players == null) return;

            foreach (var player in players)
            {
                if (playerItemPrefab != null && playerListContainer != null)
                {
                    GameObject item = Instantiate(playerItemPrefab, playerListContainer);
                    
                    Text[] texts = item.GetComponentsInChildren<Text>();
                    if (texts.Length > 0) texts[0].text = player.playerName;
                    if (texts.Length > 1) texts[1].text = player.isReady ? "Ready" : "Not Ready";
                    if (texts.Length > 2) texts[2].text = $"Team {player.team}";

                    playerItems.Add(item);
                }
            }
        }

        private void UpdateStartButton()
        {
            if (startButton != null)
            {
                startButton.gameObject.SetActive(NetworkManager.Instance?.IsHost == true);
                startButton.interactable = LobbyManager.Instance?.CanStartMatch() == true;
            }
        }

        private void ToggleReady()
        {
            AudioManager.Instance?.PlayButtonClick();
            NetworkPlayer local = NetworkManager.Instance?.LocalPlayer;
            if (local != null)
            {
                LobbyManager.Instance?.SetPlayerReady(!local.isReady);
                UpdatePlayerList();
            }
        }

        private void StartMatch()
        {
            AudioManager.Instance?.PlayButtonClick();
            LobbyManager.Instance?.StartCountdown();
        }

        private void LeaveRoom()
        {
            AudioManager.Instance?.PlayButtonClick();
            LobbyManager.Instance?.LeaveLobby();
        }

        private void SelectTeam(int team)
        {
            AudioManager.Instance?.PlayButtonClick();
            LobbyManager.Instance?.SelectTeam(team);
            UpdatePlayerList();
        }

        private void OnCountdownTick(float time)
        {
            if (countdownText != null)
                countdownText.text = $"Starting in {Mathf.CeilToInt(time)}...";
        }

        private void OnCountdownStarted()
        {
            if (countdownText != null)
                countdownText.gameObject.SetActive(true);
        }

        private void OnCountdownCancelled()
        {
            if (countdownText != null)
                countdownText.gameObject.SetActive(false);
        }

        public void BackToMainMenu()
        {
            AudioManager.Instance?.PlayButtonClick();
            NetworkManager.Instance?.Disconnect();
            SceneLoader.Instance?.LoadMainMenu();
        }
    }
}
