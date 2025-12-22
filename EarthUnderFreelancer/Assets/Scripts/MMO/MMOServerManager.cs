using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EarthUnderFreelancer.MMO
{
    /// <summary>
    /// Core MMO Server Manager - Handles server connections, authentication, and world state
    /// Similar to WoW's realm/shard system
    /// </summary>
    public class MMOServerManager : MonoBehaviour
    {
        public static MMOServerManager Instance { get; private set; }

        [Header("Server Configuration")]
        [SerializeField] private string masterServerURL = "https://earthunderfreelancer.com/api";
        [SerializeField] private int maxPlayersPerShard = 1000;
        [SerializeField] private float heartbeatInterval = 30f;
        [SerializeField] private float positionSyncInterval = 0.1f;

        [Header("Connection Settings")]
        [SerializeField] private int connectionTimeout = 30;
        [SerializeField] private int maxReconnectAttempts = 5;
        [SerializeField] private float reconnectDelay = 5f;

        // Server State
        public bool IsConnected { get; private set; }
        public bool IsAuthenticated { get; private set; }
        public string CurrentShardId { get; private set; }
        public int CurrentPlayerCount { get; private set; }
        public float Latency { get; private set; }

        // Player Info
        public string PlayerId { get; private set; }
        public string PlayerName { get; private set; }
        public string AccountId { get; private set; }

        // Events
        public event Action OnConnected;
        public event Action OnDisconnected;
        public event Action<string> OnConnectionError;
        public event Action OnAuthenticated;
        public event Action<PlayerData> OnPlayerDataReceived;
        public event Action<List<ShardInfo>> OnShardListReceived;
        public event Action<string, Vector3> OnPlayerPositionUpdate;
        public event Action<string> OnPlayerJoinedShard;
        public event Action<string> OnPlayerLeftShard;
        public event Action<ChatMessage> OnChatMessageReceived;
        public event Action<WorldEvent> OnWorldEventReceived;

        // Internal Data
        private Dictionary<string, RemotePlayer> remotePlayers = new Dictionary<string, RemotePlayer>();
        private Queue<NetworkMessage> messageQueue = new Queue<NetworkMessage>();
        private float lastHeartbeat;
        private float lastPositionSync;
        private int reconnectAttempts;
        private bool isReconnecting;

        [Serializable]
        public class ShardInfo
        {
            public string shardId;
            public string shardName;
            public string region;
            public int currentPlayers;
            public int maxPlayers;
            public ShardStatus status;
            public float latency;
        }

        public enum ShardStatus
        {
            Online,
            Full,
            Maintenance,
            Offline
        }

        [Serializable]
        public class RemotePlayer
        {
            public string playerId;
            public string playerName;
            public string guildName;
            public int level;
            public string shipType;
            public Vector3 position;
            public Quaternion rotation;
            public Vector3 velocity;
            public PlayerStatus status;
            public float lastUpdate;
        }

        public enum PlayerStatus
        {
            Online,
            InCombat,
            Docked,
            AFK,
            Invisible
        }

        [Serializable]
        public class ChatMessage
        {
            public string senderId;
            public string senderName;
            public ChatChannel channel;
            public string message;
            public DateTime timestamp;
            public string targetId; // For whispers
        }

        public enum ChatChannel
        {
            Global,
            Local,
            Party,
            Guild,
            Whisper,
            Trade,
            System,
            Combat,
            Announcement
        }

        [Serializable]
        public class WorldEvent
        {
            public string eventId;
            public WorldEventType eventType;
            public string eventName;
            public string description;
            public Vector3 location;
            public float radius;
            public DateTime startTime;
            public DateTime endTime;
            public Dictionary<string, object> eventData;
        }

        public enum WorldEventType
        {
            WorldBoss,
            Invasion,
            DoubleXP,
            SpecialDrop,
            PvPEvent,
            StationSiege,
            AsteroidStorm,
            AlienContact
        }

        [Serializable]
        public class NetworkMessage
        {
            public MessageType type;
            public string data;
            public float timestamp;
        }

        public enum MessageType
        {
            Position,
            Combat,
            Chat,
            Trade,
            Guild,
            Party,
            Quest,
            WorldEvent,
            System
        }

        [Serializable]
        public class PlayerData
        {
            public string playerId;
            public string playerName;
            public int level;
            public long experience;
            public long credits;
            public int gems;
            public string currentShipId;
            public List<string> ownedShips;
            public List<string> inventory;
            public Dictionary<string, int> factionReputation;
            public string guildId;
            public PlayerStats stats;
            public DateTime lastLogin;
            public DateTime createdAt;
            public int playTime; // in minutes
        }

        [Serializable]
        public class PlayerStats
        {
            public int kills;
            public int deaths;
            public int pvpKills;
            public int pvpDeaths;
            public int missionsCompleted;
            public int dungeonsCleared;
            public int raidsCompleted;
            public int achievementsUnlocked;
            public long totalDamageDealt;
            public long totalDamageTaken;
            public long totalCreditsEarned;
            public float totalDistanceTraveled;
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            if (!IsConnected) return;

            // Send heartbeat
            if (Time.time - lastHeartbeat > heartbeatInterval)
            {
                SendHeartbeat();
                lastHeartbeat = Time.time;
            }

            // Sync position
            if (Time.time - lastPositionSync > positionSyncInterval)
            {
                SyncPlayerPosition();
                lastPositionSync = Time.time;
            }

            // Process message queue
            ProcessMessageQueue();

            // Update remote players
            UpdateRemotePlayers();
        }

        #region Connection Management

        public async Task<bool> ConnectToMasterServer()
        {
            Debug.Log("[MMO] Connecting to master server...");
            
            try
            {
                // Simulate connection (replace with actual networking)
                await Task.Delay(1000);
                
                IsConnected = true;
                OnConnected?.Invoke();
                Debug.Log("[MMO] Connected to master server");
                return true;
            }
            catch (Exception e)
            {
                OnConnectionError?.Invoke(e.Message);
                Debug.LogError($"[MMO] Connection failed: {e.Message}");
                return false;
            }
        }

        public async Task<bool> Authenticate(string username, string password)
        {
            if (!IsConnected)
            {
                Debug.LogError("[MMO] Not connected to server");
                return false;
            }

            try
            {
                // Simulate authentication (replace with actual auth)
                await Task.Delay(500);
                
                // Generate IDs (would come from server)
                AccountId = Guid.NewGuid().ToString();
                PlayerId = Guid.NewGuid().ToString();
                PlayerName = username;
                
                IsAuthenticated = true;
                OnAuthenticated?.Invoke();
                Debug.Log($"[MMO] Authenticated as {username}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[MMO] Authentication failed: {e.Message}");
                return false;
            }
        }

        public async Task<bool> AuthenticateWithToken(string token)
        {
            if (!IsConnected)
            {
                await ConnectToMasterServer();
            }

            try
            {
                await Task.Delay(300);
                IsAuthenticated = true;
                OnAuthenticated?.Invoke();
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[MMO] Token auth failed: {e.Message}");
                return false;
            }
        }

        public async Task<List<ShardInfo>> GetShardList()
        {
            var shards = new List<ShardInfo>
            {
                new ShardInfo { shardId = "us-west-1", shardName = "Sol System", region = "US West", currentPlayers = 847, maxPlayers = 1000, status = ShardStatus.Online, latency = 45 },
                new ShardInfo { shardId = "us-east-1", shardName = "Alpha Centauri", region = "US East", currentPlayers = 923, maxPlayers = 1000, status = ShardStatus.Online, latency = 32 },
                new ShardInfo { shardId = "eu-central-1", shardName = "Orion Cluster", region = "EU Central", currentPlayers = 756, maxPlayers = 1000, status = ShardStatus.Online, latency = 85 },
                new ShardInfo { shardId = "eu-west-1", shardName = "Andromeda Gate", region = "EU West", currentPlayers = 634, maxPlayers = 1000, status = ShardStatus.Online, latency = 92 },
                new ShardInfo { shardId = "asia-1", shardName = "Dragon Nebula", region = "Asia", currentPlayers = 1000, maxPlayers = 1000, status = ShardStatus.Full, latency = 150 },
                new ShardInfo { shardId = "oceania-1", shardName = "Southern Cross", region = "Oceania", currentPlayers = 312, maxPlayers = 1000, status = ShardStatus.Online, latency = 180 }
            };

            OnShardListReceived?.Invoke(shards);
            return shards;
        }

        public async Task<bool> JoinShard(string shardId)
        {
            Debug.Log($"[MMO] Joining shard: {shardId}");
            
            try
            {
                await Task.Delay(500);
                CurrentShardId = shardId;
                OnPlayerJoinedShard?.Invoke(shardId);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[MMO] Failed to join shard: {e.Message}");
                return false;
            }
        }

        public void Disconnect()
        {
            IsConnected = false;
            IsAuthenticated = false;
            CurrentShardId = null;
            remotePlayers.Clear();
            OnDisconnected?.Invoke();
        }

        private async void AttemptReconnect()
        {
            if (isReconnecting || reconnectAttempts >= maxReconnectAttempts) return;

            isReconnecting = true;
            reconnectAttempts++;

            await Task.Delay((int)(reconnectDelay * 1000));

            if (await ConnectToMasterServer())
            {
                reconnectAttempts = 0;
            }

            isReconnecting = false;
        }

        #endregion

        #region Position Sync

        private void SendHeartbeat()
        {
            var message = new NetworkMessage
            {
                type = MessageType.System,
                data = "heartbeat",
                timestamp = Time.time
            };
            SendMessage(message);
        }

        private void SyncPlayerPosition()
        {
            var player = FindObjectOfType<Player.PlayerController>();
            if (player == null) return;

            var posData = new
            {
                position = player.transform.position,
                rotation = player.transform.rotation,
                velocity = player.GetComponent<Rigidbody>()?.velocity ?? Vector3.zero
            };

            var message = new NetworkMessage
            {
                type = MessageType.Position,
                data = JsonUtility.ToJson(posData),
                timestamp = Time.time
            };
            SendMessage(message);
        }

        public void UpdatePlayerPosition(string playerId, Vector3 position, Quaternion rotation, Vector3 velocity)
        {
            if (remotePlayers.TryGetValue(playerId, out var player))
            {
                player.position = position;
                player.rotation = rotation;
                player.velocity = velocity;
                player.lastUpdate = Time.time;
            }
            OnPlayerPositionUpdate?.Invoke(playerId, position);
        }

        #endregion

        #region Remote Players

        public void AddRemotePlayer(RemotePlayer player)
        {
            if (!remotePlayers.ContainsKey(player.playerId))
            {
                remotePlayers[player.playerId] = player;
                SpawnRemotePlayerVisual(player);
            }
        }

        public void RemoveRemotePlayer(string playerId)
        {
            if (remotePlayers.TryGetValue(playerId, out var player))
            {
                remotePlayers.Remove(playerId);
                DestroyRemotePlayerVisual(playerId);
                OnPlayerLeftShard?.Invoke(playerId);
            }
        }

        private void SpawnRemotePlayerVisual(RemotePlayer player)
        {
            // Would instantiate a prefab for the remote player
            Debug.Log($"[MMO] Spawning remote player: {player.playerName}");
        }

        private void DestroyRemotePlayerVisual(string playerId)
        {
            // Would destroy the visual representation
            Debug.Log($"[MMO] Removing remote player visual: {playerId}");
        }

        private void UpdateRemotePlayers()
        {
            float timeout = 10f;
            var toRemove = new List<string>();

            foreach (var kvp in remotePlayers)
            {
                if (Time.time - kvp.Value.lastUpdate > timeout)
                {
                    toRemove.Add(kvp.Key);
                }
            }

            foreach (var id in toRemove)
            {
                RemoveRemotePlayer(id);
            }
        }

        public RemotePlayer GetRemotePlayer(string playerId)
        {
            return remotePlayers.TryGetValue(playerId, out var player) ? player : null;
        }

        public List<RemotePlayer> GetNearbyPlayers(Vector3 position, float radius)
        {
            var nearby = new List<RemotePlayer>();
            foreach (var player in remotePlayers.Values)
            {
                if (Vector3.Distance(position, player.position) <= radius)
                {
                    nearby.Add(player);
                }
            }
            return nearby;
        }

        #endregion

        #region Chat System

        public void SendChatMessage(ChatChannel channel, string message, string targetId = null)
        {
            var chatMessage = new ChatMessage
            {
                senderId = PlayerId,
                senderName = PlayerName,
                channel = channel,
                message = message,
                timestamp = DateTime.UtcNow,
                targetId = targetId
            };

            var networkMessage = new NetworkMessage
            {
                type = MessageType.Chat,
                data = JsonUtility.ToJson(chatMessage),
                timestamp = Time.time
            };

            SendMessage(networkMessage);

            // Also show locally
            OnChatMessageReceived?.Invoke(chatMessage);
        }

        public void ReceiveChatMessage(ChatMessage message)
        {
            OnChatMessageReceived?.Invoke(message);
        }

        #endregion

        #region World Events

        public void BroadcastWorldEvent(WorldEvent worldEvent)
        {
            OnWorldEventReceived?.Invoke(worldEvent);
        }

        public List<WorldEvent> GetActiveWorldEvents()
        {
            // Would fetch from server
            return new List<WorldEvent>();
        }

        #endregion

        #region Message Queue

        private void SendMessage(NetworkMessage message)
        {
            // Would actually send to server
            messageQueue.Enqueue(message);
        }

        private void ProcessMessageQueue()
        {
            while (messageQueue.Count > 0)
            {
                var message = messageQueue.Dequeue();
                // Process or send the message
            }
        }

        #endregion

        #region Player Data

        public async Task<PlayerData> LoadPlayerData()
        {
            await Task.Delay(200);

            var data = new PlayerData
            {
                playerId = PlayerId,
                playerName = PlayerName,
                level = 1,
                experience = 0,
                credits = 10000,
                gems = 100,
                currentShipId = "starter_ship",
                ownedShips = new List<string> { "starter_ship" },
                inventory = new List<string>(),
                factionReputation = new Dictionary<string, int>(),
                stats = new PlayerStats(),
                lastLogin = DateTime.UtcNow,
                createdAt = DateTime.UtcNow,
                playTime = 0
            };

            OnPlayerDataReceived?.Invoke(data);
            return data;
        }

        public async Task<bool> SavePlayerData(PlayerData data)
        {
            await Task.Delay(100);
            Debug.Log("[MMO] Player data saved");
            return true;
        }

        #endregion

        #region Utility

        public float GetServerTime()
        {
            return Time.time; // Would sync with server time
        }

        public void SetPlayerStatus(PlayerStatus status)
        {
            // Update status on server
        }

        #endregion
    }
}
