using UnityEngine;
using System;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Networking
{
    /// <summary>
    /// Enhanced network message system with reliable delivery and packet management
    /// Implements UDP-style fast transmission with TCP-style reliability options
    /// </summary>
    public class NetworkMessageSystem : MonoBehaviour
    {
        public static NetworkMessageSystem Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private int maxPacketsPerFrame = 30;
        [SerializeField] private float resendInterval = 0.3f;
        [SerializeField] private int maxResendAttempts = 5;
        [SerializeField] private int maxMessageQueueSize = 100;

        [Header("Stats")]
        [SerializeField] private int totalMessagesSent;
        [SerializeField] private int totalMessagesReceived;
        [SerializeField] private int packetsLost;
        [SerializeField] private float averageLatency;

        private Dictionary<int, NetworkMessage> pendingMessages = new Dictionary<int, NetworkMessage>();
        private Queue<NetworkMessage> outgoingMessages = new Queue<NetworkMessage>();
        private Queue<NetworkMessage> incomingMessages = new Queue<NetworkMessage>();
        private int nextMessageId = 1;

        public event Action<NetworkMessageType, object> OnMessageReceived;

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
            ProcessOutgoingMessages();
            ProcessIncomingMessages();
            CheckForTimeouts();
        }

        /// <summary>
        /// Send a network message with optional reliability
        /// </summary>
        public void SendMessage(NetworkMessageType messageType, object data, bool reliable = false, int targetPlayerId = -1)
        {
            if (outgoingMessages.Count >= maxMessageQueueSize)
            {
                Debug.LogWarning("[Network] Message queue full, dropping message");
                return;
            }

            NetworkMessage message = new NetworkMessage
            {
                messageId = nextMessageId++,
                messageType = messageType,
                data = data,
                reliable = reliable,
                targetPlayerId = targetPlayerId,
                timestamp = Time.time,
                sendAttempts = 0
            };

            outgoingMessages.Enqueue(message);
            totalMessagesSent++;

            if (reliable)
            {
                pendingMessages[message.messageId] = message;
            }
        }

        /// <summary>
        /// Send position update (optimized, unreliable)
        /// </summary>
        public void SendPositionUpdate(Vector3 position, Quaternion rotation, Vector3 velocity)
        {
            PositionUpdateData data = new PositionUpdateData
            {
                position = position,
                rotation = rotation,
                velocity = velocity,
                timestamp = Time.time
            };

            SendMessage(NetworkMessageType.PositionUpdate, data, reliable: false);
        }

        /// <summary>
        /// Send damage event (reliable)
        /// </summary>
        public void SendDamageEvent(int targetPlayerId, float damage, string damageSource)
        {
            DamageEventData data = new DamageEventData
            {
                targetPlayerId = targetPlayerId,
                damage = damage,
                damageSource = damageSource,
                timestamp = Time.time
            };

            SendMessage(NetworkMessageType.DamageEvent, data, reliable: true, targetPlayerId);
        }

        /// <summary>
        /// Send player action (reliable or unreliable based on action type)
        /// </summary>
        public void SendPlayerAction(PlayerAction action, object actionData)
        {
            PlayerActionData data = new PlayerActionData
            {
                action = action,
                data = actionData,
                timestamp = Time.time
            };

            // Fire weapon = unreliable, Use item = reliable
            bool reliable = action == PlayerAction.UseItem || action == PlayerAction.Respawn;
            SendMessage(NetworkMessageType.PlayerAction, data, reliable);
        }

        private void ProcessOutgoingMessages()
        {
            int processedThisFrame = 0;

            while (outgoingMessages.Count > 0 && processedThisFrame < maxPacketsPerFrame)
            {
                NetworkMessage message = outgoingMessages.Dequeue();
                TransmitMessage(message);
                processedThisFrame++;
            }
        }

        private void TransmitMessage(NetworkMessage message)
        {
            // In real implementation, this would serialize and send over network
            // For now, we simulate transmission
            
            message.sendAttempts++;
            message.lastSendTime = Time.time;

            // Simulate 5% packet loss for unreliable messages
            if (!message.reliable && UnityEngine.Random.value < 0.05f)
            {
                packetsLost++;
                return;
            }

            // Simulate successful transmission with small delay
            float simulatedLatency = UnityEngine.Random.Range(0.02f, 0.05f);
            averageLatency = Mathf.Lerp(averageLatency, simulatedLatency, 0.1f);

            // For demo purposes, immediately "receive" the message
            // In real implementation, this would come from network
            ReceiveMessage(message);
        }

        private void ReceiveMessage(NetworkMessage message)
        {
            incomingMessages.Enqueue(message);
        }

        private void ProcessIncomingMessages()
        {
            while (incomingMessages.Count > 0)
            {
                NetworkMessage message = incomingMessages.Dequeue();
                
                totalMessagesReceived++;

                // Remove from pending if it was reliable
                if (message.reliable && pendingMessages.ContainsKey(message.messageId))
                {
                    pendingMessages.Remove(message.messageId);
                }

                // Dispatch to handlers
                OnMessageReceived?.Invoke(message.messageType, message.data);
                HandleMessage(message);
            }
        }

        private void HandleMessage(NetworkMessage message)
        {
            switch (message.messageType)
            {
                case NetworkMessageType.PositionUpdate:
                    if (message.data is PositionUpdateData posData)
                    {
                        // Update player position in game
                        UpdatePlayerPosition(posData);
                    }
                    break;

                case NetworkMessageType.DamageEvent:
                    if (message.data is DamageEventData damageData)
                    {
                        // Apply damage to target
                        ApplyDamage(damageData);
                    }
                    break;

                case NetworkMessageType.PlayerAction:
                    if (message.data is PlayerActionData actionData)
                    {
                        // Execute player action
                        ExecutePlayerAction(actionData);
                    }
                    break;

                case NetworkMessageType.ChatMessage:
                    if (message.data is ChatMessageData chatData)
                    {
                        DisplayChatMessage(chatData);
                    }
                    break;
            }
        }

        private void CheckForTimeouts()
        {
            if (pendingMessages.Count == 0) return;

            List<int> toResend = new List<int>();
            List<int> toRemove = new List<int>();

            foreach (var kvp in pendingMessages)
            {
                NetworkMessage message = kvp.Value;
                
                if (Time.time - message.lastSendTime >= resendInterval)
                {
                    if (message.sendAttempts >= maxResendAttempts)
                    {
                        Debug.LogWarning($"[Network] Message {message.messageId} failed after {maxResendAttempts} attempts");
                        toRemove.Add(message.messageId);
                        packetsLost++;
                    }
                    else
                    {
                        toResend.Add(message.messageId);
                    }
                }
            }

            // Resend failed messages
            foreach (int messageId in toResend)
            {
                outgoingMessages.Enqueue(pendingMessages[messageId]);
            }

            // Remove permanently failed messages
            foreach (int messageId in toRemove)
            {
                pendingMessages.Remove(messageId);
            }
        }

        // Message handlers (would interact with game systems)
        private void UpdatePlayerPosition(PositionUpdateData data)
        {
            // Update via multiplayer sync system
        }

        private void ApplyDamage(DamageEventData data)
        {
            // Apply damage via game manager
            Debug.Log($"[Network] Player {data.targetPlayerId} took {data.damage} damage from {data.damageSource}");
        }

        private void ExecutePlayerAction(PlayerActionData data)
        {
            Debug.Log($"[Network] Player action: {data.action}");
        }

        private void DisplayChatMessage(ChatMessageData data)
        {
            Debug.Log($"[Chat] {data.senderName}: {data.message}");
        }

        public NetworkStats GetStats()
        {
            return new NetworkStats
            {
                totalSent = totalMessagesSent,
                totalReceived = totalMessagesReceived,
                packetsLost = packetsLost,
                averageLatencyMs = averageLatency * 1000f,
                pendingReliableMessages = pendingMessages.Count,
                queuedOutgoingMessages = outgoingMessages.Count
            };
        }

        public void Reset()
        {
            outgoingMessages.Clear();
            incomingMessages.Clear();
            pendingMessages.Clear();
            totalMessagesSent = 0;
            totalMessagesReceived = 0;
            packetsLost = 0;
            averageLatency = 0;
        }
    }

    public enum NetworkMessageType
    {
        PositionUpdate,
        DamageEvent,
        PlayerAction,
        ChatMessage,
        PlayerJoined,
        PlayerLeft,
        MatchStateUpdate,
        ObjectSpawn,
        ObjectDestroy
    }

    public enum PlayerAction
    {
        FireWeapon,
        UseItem,
        Respawn,
        ChangeLoadout,
        EmotePlay
    }

    [Serializable]
    public class NetworkMessage
    {
        public int messageId;
        public NetworkMessageType messageType;
        public object data;
        public bool reliable;
        public int targetPlayerId;
        public float timestamp;
        public float lastSendTime;
        public int sendAttempts;
    }

    [Serializable]
    public class PositionUpdateData
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 velocity;
        public float timestamp;
    }

    [Serializable]
    public class DamageEventData
    {
        public int targetPlayerId;
        public float damage;
        public string damageSource;
        public float timestamp;
    }

    [Serializable]
    public class PlayerActionData
    {
        public PlayerAction action;
        public object data;
        public float timestamp;
    }

    [Serializable]
    public class ChatMessageData
    {
        public string senderName;
        public string message;
        public bool isTeamChat;
        public float timestamp;
    }

    [Serializable]
    public struct NetworkStats
    {
        public int totalSent;
        public int totalReceived;
        public int packetsLost;
        public float averageLatencyMs;
        public int pendingReliableMessages;
        public int queuedOutgoingMessages;
    }
}
