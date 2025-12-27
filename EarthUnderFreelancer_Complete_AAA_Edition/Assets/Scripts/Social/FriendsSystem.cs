using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EarthUnderFreelancer.Social
{
    /// <summary>
    /// Friends and Social System for MMO
    /// Manages friend lists, online status, and social interactions
    /// </summary>
    public class FriendsSystem : MonoBehaviour
    {
        public static FriendsSystem Instance { get; private set; }

        [Header("Configuration")]
        [SerializeField] private int maxFriends = 200;
        [SerializeField] private int maxIgnored = 100;
        [SerializeField] private float onlineCheckInterval = 30f;

        public List<FriendData> Friends { get; private set; }
        public List<FriendData> PendingRequests { get; private set; }
        public List<FriendData> SentRequests { get; private set; }
        public List<string> IgnoredPlayers { get; private set; }

        public event Action<FriendData> OnFriendAdded;
        public event Action<FriendData> OnFriendRemoved;
        public event Action<FriendData> OnFriendRequestReceived;
        public event Action<FriendData> OnFriendOnline;
        public event Action<FriendData> OnFriendOffline;
        public event Action<string, string> OnFriendStatusChanged;

        #region Data Structures

        [Serializable]
        public class FriendData
        {
            public string playerId;
            public string playerName;
            public bool isOnline;
            public DateTime lastOnline;
            public string status;
            public string currentLocation;
            public int level;
            public string guildName;
            public string currentAircraft;
            public FriendNote note;
            public DateTime friendSince;
        }

        [Serializable]
        public class FriendNote
        {
            public string text;
            public DateTime lastUpdated;
        }

        #endregion

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Initialize()
        {
            Friends = new List<FriendData>();
            PendingRequests = new List<FriendData>();
            SentRequests = new List<FriendData>();
            IgnoredPlayers = new List<string>();
            
            InvokeRepeating(nameof(CheckOnlineStatus), onlineCheckInterval, onlineCheckInterval);
        }

        #region Friend Management

        public async Task<bool> SendFriendRequest(string playerId, string playerName)
        {
            if (Friends.Count >= maxFriends)
            {
                Debug.LogWarning("[Friends] Friend list is full!");
                return false;
            }

            if (Friends.Exists(f => f.playerId == playerId))
            {
                Debug.LogWarning("[Friends] Already friends!");
                return false;
            }

            if (SentRequests.Exists(f => f.playerId == playerId))
            {
                Debug.LogWarning("[Friends] Request already sent!");
                return false;
            }

            if (IgnoredPlayers.Contains(playerId))
            {
                Debug.LogWarning("[Friends] Player is ignored!");
                return false;
            }

            var request = new FriendData
            {
                playerId = playerId,
                playerName = playerName
            };

            SentRequests.Add(request);
            await Task.Delay(100);

            Debug.Log($"[Friends] Sent friend request to {playerName}");
            return true;
        }

        public void ReceiveFriendRequest(FriendData fromPlayer)
        {
            if (IgnoredPlayers.Contains(fromPlayer.playerId)) return;
            if (PendingRequests.Exists(f => f.playerId == fromPlayer.playerId)) return;

            PendingRequests.Add(fromPlayer);
            OnFriendRequestReceived?.Invoke(fromPlayer);
        }

        public async Task<bool> AcceptFriendRequest(string playerId)
        {
            var request = PendingRequests.Find(f => f.playerId == playerId);
            if (request == null) return false;

            request.friendSince = DateTime.UtcNow;
            Friends.Add(request);
            PendingRequests.Remove(request);

            OnFriendAdded?.Invoke(request);
            await Task.Delay(50);

            Debug.Log($"[Friends] Accepted friend request from {request.playerName}");
            return true;
        }

        public bool DeclineFriendRequest(string playerId)
        {
            var request = PendingRequests.Find(f => f.playerId == playerId);
            if (request == null) return false;

            PendingRequests.Remove(request);
            Debug.Log($"[Friends] Declined friend request from {request.playerName}");
            return true;
        }

        public async Task<bool> RemoveFriend(string playerId)
        {
            var friend = Friends.Find(f => f.playerId == playerId);
            if (friend == null) return false;

            Friends.Remove(friend);
            OnFriendRemoved?.Invoke(friend);
            await Task.Delay(50);

            Debug.Log($"[Friends] Removed {friend.playerName} from friends");
            return true;
        }

        public bool IgnorePlayer(string playerId)
        {
            if (IgnoredPlayers.Count >= maxIgnored) return false;
            if (IgnoredPlayers.Contains(playerId)) return false;

            IgnoredPlayers.Add(playerId);
            
            // Also remove from friends if present
            var friend = Friends.Find(f => f.playerId == playerId);
            if (friend != null)
            {
                Friends.Remove(friend);
                OnFriendRemoved?.Invoke(friend);
            }

            return true;
        }

        public bool UnignorePlayer(string playerId)
        {
            return IgnoredPlayers.Remove(playerId);
        }

        public void SetFriendNote(string playerId, string noteText)
        {
            var friend = Friends.Find(f => f.playerId == playerId);
            if (friend != null)
            {
                friend.note = new FriendNote
                {
                    text = noteText,
                    lastUpdated = DateTime.UtcNow
                };
            }
        }

        #endregion

        #region Online Status

        private void CheckOnlineStatus()
        {
            // Would query server for friend online status
            foreach (var friend in Friends)
            {
                bool wasOnline = friend.isOnline;
                // Simulate online check
                friend.isOnline = UnityEngine.Random.value > 0.5f;

                if (friend.isOnline && !wasOnline)
                {
                    OnFriendOnline?.Invoke(friend);
                }
                else if (!friend.isOnline && wasOnline)
                {
                    friend.lastOnline = DateTime.UtcNow;
                    OnFriendOffline?.Invoke(friend);
                }
            }
        }

        public void UpdateFriendStatus(string playerId, string status, string location)
        {
            var friend = Friends.Find(f => f.playerId == playerId);
            if (friend != null)
            {
                friend.status = status;
                friend.currentLocation = location;
                OnFriendStatusChanged?.Invoke(playerId, status);
            }
        }

        #endregion

        #region Queries

        public List<FriendData> GetOnlineFriends()
        {
            return Friends.FindAll(f => f.isOnline);
        }

        public List<FriendData> GetOfflineFriends()
        {
            return Friends.FindAll(f => !f.isOnline);
        }

        public FriendData GetFriend(string playerId)
        {
            return Friends.Find(f => f.playerId == playerId);
        }

        public bool IsFriend(string playerId)
        {
            return Friends.Exists(f => f.playerId == playerId);
        }

        public bool IsIgnored(string playerId)
        {
            return IgnoredPlayers.Contains(playerId);
        }

        #endregion
    }
}
