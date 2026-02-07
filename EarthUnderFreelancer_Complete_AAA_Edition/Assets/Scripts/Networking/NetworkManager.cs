using UnityEngine;
using System;
using System.Collections.Generic;
using EarthUnderFreelancer.Core;

namespace EarthUnderFreelancer.Networking
{
    /// <summary>
    /// Manages all multiplayer networking functionality
    /// </summary>
    public class NetworkManager : MonoBehaviour
    {
        public static NetworkManager Instance { get; private set; }

        [Header("Network Settings")]
        [SerializeField] private int maxPlayers = 16;
        [SerializeField] private int defaultPort = 7777;
        [SerializeField] private string masterServerAddress = "localhost";
        [SerializeField] private float heartbeatInterval = 5f;

        [Header("Room Settings")]
        [SerializeField] private string currentRoomName;
        [SerializeField] private GameMode currentGameMode;
        [SerializeField] private string currentMap;

        [Header("State")]
        [SerializeField] private bool isConnected = false;
        [SerializeField] private bool isHost = false;
        [SerializeField] private bool isInRoom = false;

        private Dictionary<int, NetworkPlayer> connectedPlayers = new Dictionary<int, NetworkPlayer>();
        private NetworkPlayer localPlayer;
        private int localPlayerId = -1;
        private List<RoomInfo> availableRooms = new List<RoomInfo>();
        private float lastHeartbeat;

        public event Action OnConnectedToServer;
        public event Action OnDisconnectedFromServer;
        public event Action<NetworkPlayer> OnPlayerJoined;
        public event Action<NetworkPlayer> OnPlayerLeft;
        public event Action<RoomInfo> OnJoinedRoom;
        public event Action OnLeftRoom;
        public event Action<List<RoomInfo>> OnRoomListUpdated;
        public event Action OnMatchStarted;
        public event Action<MatchResult> OnMatchEnded;

        public bool IsConnected => isConnected;
        public bool IsHost => isHost;
        public bool IsInRoom => isInRoom;
        public NetworkPlayer LocalPlayer => localPlayer;
        public int PlayerCount => connectedPlayers.Count;
        public GameMode CurrentGameMode => currentGameMode;

        private void Awake()
        {
            if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
            else Destroy(gameObject);
        }

        private void Update()
        {
            if (isConnected && Time.time - lastHeartbeat >= heartbeatInterval)
            {
                SendHeartbeat();
                lastHeartbeat = Time.time;
            }
        }

        public void ConnectToMasterServer()
        {
            Debug.Log($"Connecting to master server: {masterServerAddress}");
            SimulateConnection();
        }

        private void SimulateConnection()
        {
            isConnected = true;
            localPlayerId = UnityEngine.Random.Range(1000, 9999);
            
            localPlayer = new NetworkPlayer
            {
                playerId = localPlayerId,
                playerName = GameManager.Instance?.playerData?.playerName ?? "Player",
                isLocal = true,
                isReady = false
            };

            OnConnectedToServer?.Invoke();
            EventManager.TriggerEvent(GameEvents.CONNECTED_TO_SERVER);
        }

        public void Disconnect()
        {
            if (isInRoom) LeaveRoom();

            isConnected = false;
            localPlayer = null;
            localPlayerId = -1;

            OnDisconnectedFromServer?.Invoke();
            EventManager.TriggerEvent(GameEvents.DISCONNECTED_FROM_SERVER);
        }

        private void SendHeartbeat() { /* Send heartbeat to keep connection alive */ }

        public void CreateRoom(string roomName, GameMode gameMode, string mapName, int maxPlayerCount = 8)
        {
            if (!isConnected) return;

            currentRoomName = roomName;
            currentGameMode = gameMode;
            currentMap = mapName;
            maxPlayers = maxPlayerCount;
            isHost = true;
            isInRoom = true;

            connectedPlayers.Clear();
            connectedPlayers.Add(localPlayerId, localPlayer);

            var roomInfo = new RoomInfo
            {
                roomId = Guid.NewGuid().ToString(),
                roomName = roomName,
                hostName = localPlayer.playerName,
                gameMode = gameMode,
                mapName = mapName,
                playerCount = 1,
                maxPlayers = maxPlayerCount,
                isOpen = true
            };

            OnJoinedRoom?.Invoke(roomInfo);
        }

        public void JoinRoom(string roomId)
        {
            if (!isConnected) return;

            RoomInfo room = null;
            foreach (var r in availableRooms) if (r.roomId == roomId) { room = r; break; }
            if (room == null) return;

            currentRoomName = room.roomName;
            currentGameMode = room.gameMode;
            currentMap = room.mapName;
            isHost = false;
            isInRoom = true;

            connectedPlayers.Clear();
            connectedPlayers.Add(localPlayerId, localPlayer);
            OnJoinedRoom?.Invoke(room);
        }

        public void JoinRandomRoom(GameMode preferredMode = GameMode.TeamDeathmatch)
        {
            foreach (var room in availableRooms)
            {
                if (room.isOpen && room.playerCount < room.maxPlayers && room.gameMode == preferredMode)
                {
                    JoinRoom(room.roomId);
                    return;
                }
            }
            CreateRoom($"Room_{UnityEngine.Random.Range(1000, 9999)}", preferredMode, "Arena_01");
        }

        public void LeaveRoom()
        {
            if (!isInRoom) return;
            connectedPlayers.Clear();
            isInRoom = false;
            isHost = false;
            currentRoomName = "";
            OnLeftRoom?.Invoke();
        }

        public void RefreshRoomList()
        {
            availableRooms.Clear();
            for (int i = 0; i < 5; i++)
            {
                availableRooms.Add(new RoomInfo
                {
                    roomId = Guid.NewGuid().ToString(),
                    roomName = $"Battle Room {i + 1}",
                    hostName = $"Player{UnityEngine.Random.Range(1, 100)}",
                    gameMode = (GameMode)UnityEngine.Random.Range(0, 4),
                    mapName = $"Arena_0{UnityEngine.Random.Range(1, 4)}",
                    playerCount = UnityEngine.Random.Range(1, 8),
                    maxPlayers = 8,
                    isOpen = true
                });
            }
            OnRoomListUpdated?.Invoke(availableRooms);
        }

        public List<RoomInfo> GetRoomList() => new List<RoomInfo>(availableRooms);

        public void StartMatch()
        {
            if (!isHost) return;

            bool allReady = true;
            foreach (var player in connectedPlayers.Values) if (!player.isReady && !player.isLocal) { allReady = false; break; }
            if (!allReady) return;

            OnMatchStarted?.Invoke();
            EventManager.TriggerEvent(GameEvents.MATCH_STARTED);
            SceneLoader.Instance?.LoadScene(currentMap);
        }

        public void EndMatch(MatchResult result)
        {
            OnMatchEnded?.Invoke(result);
            EventManager.TriggerEvent(GameEvents.MATCH_ENDED, result);

            if (GameManager.Instance != null)
            {
                foreach (var score in result.playerScores)
                {
                    if (score.playerId == localPlayerId)
                    {
                        GameManager.Instance.playerData.totalKills += score.kills;
                        GameManager.Instance.playerData.totalDeaths += score.deaths;
                        if (result.winningTeam == localPlayer.team) GameManager.Instance.playerData.pvpWins++;
                        else GameManager.Instance.playerData.pvpLosses++;
                        GameManager.Instance.playerData.credits += score.creditsEarned;
                        GameManager.Instance.playerData.AddExperience(score.experienceEarned);
                        break;
                    }
                }
                GameManager.Instance.SaveGame();
            }
        }

        public void SetReady(bool ready) { if (localPlayer != null) localPlayer.isReady = ready; }
        public void SetTeam(int team) { if (localPlayer != null) localPlayer.team = team; }
        public NetworkPlayer GetPlayer(int playerId) { connectedPlayers.TryGetValue(playerId, out NetworkPlayer player); return player; }
        public List<NetworkPlayer> GetAllPlayers() => new List<NetworkPlayer>(connectedPlayers.Values);
        public void SendChatMessage(string message) => Debug.Log($"[Chat] {localPlayer.playerName}: {message}");
        public void SendTeamMessage(string message) => Debug.Log($"[Team] {localPlayer.playerName}: {message}");
    }

    [Serializable]
    public class NetworkPlayer
    {
        public int playerId;
        public string playerName;
        public int team;
        public bool isLocal;
        public bool isHost;
        public bool isReady;
        public string vehicleId;
        public int ping;
        public int kills;
        public int deaths;
        public int score;
    }

    [Serializable]
    public class RoomInfo
    {
        public string roomId;
        public string roomName;
        public string hostName;
        public GameMode gameMode;
        public string mapName;
        public int playerCount;
        public int maxPlayers;
        public bool isOpen;
        public int ping;
    }

    [Serializable]
    public class MatchResult
    {
        public int winningTeam;
        public float matchDuration;
        public List<PlayerScore> playerScores = new List<PlayerScore>();
    }

    [Serializable]
    public class PlayerScore
    {
        public int playerId;
        public string playerName;
        public int team;
        public int kills;
        public int deaths;
        public int assists;
        public int score;
        public int creditsEarned;
        public int experienceEarned;
    }

    public enum GameMode { FreeForAll, TeamDeathmatch, Domination, CaptureTheFlag, Survival, Coop }
}
