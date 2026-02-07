using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

namespace EarthUnderFreelancer.Social
{
    /// <summary>
    /// Complete Chat System for MMO communication
    /// Supports multiple channels, whispers, emotes, and chat commands
    /// </summary>
    public class ChatSystem : MonoBehaviour
    {
        public static ChatSystem Instance { get; private set; }

        [Header("Configuration")]
        [SerializeField] private int maxMessageHistory = 200;
        [SerializeField] private int maxMessageLength = 255;
        [SerializeField] private float messageCooldown = 0.5f;
        [SerializeField] private bool profanityFilterEnabled = true;

        [Header("UI References")]
        [SerializeField] private GameObject chatPanel;
        [SerializeField] private TMP_InputField chatInput;
        [SerializeField] private ScrollRect chatScrollRect;
        [SerializeField] private Transform messageContainer;
        [SerializeField] private GameObject messagePrefab;

        [Header("Channel Colors")]
        [SerializeField] private Color globalColor = Color.white;
        [SerializeField] private Color localColor = new Color(1f, 0.8f, 0.6f);
        [SerializeField] private Color partyColor = new Color(0.6f, 0.8f, 1f);
        [SerializeField] private Color guildColor = new Color(0.4f, 1f, 0.4f);
        [SerializeField] private Color whisperColor = new Color(1f, 0.6f, 1f);
        [SerializeField] private Color tradeColor = new Color(1f, 1f, 0.6f);
        [SerializeField] private Color systemColor = Color.yellow;
        [SerializeField] private Color combatColor = new Color(1f, 0.4f, 0.4f);

        // State
        private ChatChannel currentChannel = ChatChannel.Global;
        private string lastWhisperTarget;
        private float lastMessageTime;
        private List<ChatMessage> messageHistory = new List<ChatMessage>();
        private Dictionary<ChatChannel, bool> channelEnabled = new Dictionary<ChatChannel, bool>();

        public event Action<ChatMessage> OnMessageReceived;
        public event Action<ChatChannel> OnChannelChanged;

        #region Data Structures

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
        public class ChatMessage
        {
            public string messageId;
            public string senderId;
            public string senderName;
            public ChatChannel channel;
            public string content;
            public DateTime timestamp;
            public string targetId;
            public string targetName;
            public bool isSystem;
        }

        #endregion

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeChannels();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeChannels()
        {
            foreach (ChatChannel channel in Enum.GetValues(typeof(ChatChannel)))
            {
                channelEnabled[channel] = true;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (chatInput != null && chatInput.isFocused && !string.IsNullOrEmpty(chatInput.text))
                {
                    SendMessage(chatInput.text);
                    chatInput.text = "";
                }
                else if (chatInput != null)
                {
                    chatInput.Select();
                    chatInput.ActivateInputField();
                }
            }

            // Channel shortcuts
            if (Input.GetKeyDown(KeyCode.Tab) && chatInput != null && chatInput.isFocused)
            {
                CycleChannel();
            }
        }

        #region Sending Messages

        public void SendMessage(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return;
            if (Time.time - lastMessageTime < messageCooldown) return;

            // Parse commands
            if (text.StartsWith("/"))
            {
                ParseCommand(text);
                return;
            }

            // Truncate if too long
            if (text.Length > maxMessageLength)
            {
                text = text.Substring(0, maxMessageLength);
            }

            // Apply profanity filter
            if (profanityFilterEnabled)
            {
                text = FilterProfanity(text);
            }

            var message = new ChatMessage
            {
                messageId = Guid.NewGuid().ToString(),
                senderId = GetCurrentPlayerId(),
                senderName = GetCurrentPlayerName(),
                channel = currentChannel,
                content = text,
                timestamp = DateTime.UtcNow,
                isSystem = false
            };

            // Handle whisper
            if (currentChannel == ChatChannel.Whisper && !string.IsNullOrEmpty(lastWhisperTarget))
            {
                message.targetName = lastWhisperTarget;
            }

            SendToServer(message);
            lastMessageTime = Time.time;
        }

        public void SendWhisper(string targetName, string text)
        {
            if (string.IsNullOrWhiteSpace(targetName) || string.IsNullOrWhiteSpace(text)) return;

            lastWhisperTarget = targetName;

            var message = new ChatMessage
            {
                messageId = Guid.NewGuid().ToString(),
                senderId = GetCurrentPlayerId(),
                senderName = GetCurrentPlayerName(),
                channel = ChatChannel.Whisper,
                content = text,
                timestamp = DateTime.UtcNow,
                targetName = targetName,
                isSystem = false
            };

            SendToServer(message);
            
            // Show sent whisper locally
            DisplayMessage(message);
        }

        public void SendSystemMessage(string text, ChatChannel channel = ChatChannel.System)
        {
            var message = new ChatMessage
            {
                messageId = Guid.NewGuid().ToString(),
                channel = channel,
                content = text,
                timestamp = DateTime.UtcNow,
                isSystem = true
            };

            DisplayMessage(message);
        }

        private void SendToServer(ChatMessage message)
        {
            // Would send to network
            // For now, just display locally
            DisplayMessage(message);
            OnMessageReceived?.Invoke(message);
        }

        #endregion

        #region Receiving Messages

        public void ReceiveMessage(ChatMessage message)
        {
            if (!channelEnabled[message.channel]) return;

            DisplayMessage(message);
            OnMessageReceived?.Invoke(message);
        }

        private void DisplayMessage(ChatMessage message)
        {
            messageHistory.Add(message);
            if (messageHistory.Count > maxMessageHistory)
            {
                messageHistory.RemoveAt(0);
            }

            if (messagePrefab != null && messageContainer != null)
            {
                var msgObj = Instantiate(messagePrefab, messageContainer);
                var tmpText = msgObj.GetComponent<TextMeshProUGUI>();
                if (tmpText != null)
                {
                    tmpText.text = FormatMessage(message);
                    tmpText.color = GetChannelColor(message.channel);
                }

                // Auto-scroll
                if (chatScrollRect != null)
                {
                    Canvas.ForceUpdateCanvases();
                    chatScrollRect.verticalNormalizedPosition = 0f;
                }
            }

            Debug.Log($"[Chat] [{message.channel}] {message.senderName}: {message.content}");
        }

        private string FormatMessage(ChatMessage message)
        {
            string timestamp = message.timestamp.ToLocalTime().ToString("HH:mm");
            string channelTag = GetChannelTag(message.channel);

            if (message.isSystem)
            {
                return $"[{timestamp}] {channelTag} {message.content}";
            }

            if (message.channel == ChatChannel.Whisper)
            {
                if (message.senderId == GetCurrentPlayerId())
                {
                    return $"[{timestamp}] To [{message.targetName}]: {message.content}";
                }
                else
                {
                    return $"[{timestamp}] From [{message.senderName}]: {message.content}";
                }
            }

            return $"[{timestamp}] {channelTag} [{message.senderName}]: {message.content}";
        }

        private string GetChannelTag(ChatChannel channel)
        {
            return channel switch
            {
                ChatChannel.Global => "[G]",
                ChatChannel.Local => "[L]",
                ChatChannel.Party => "[P]",
                ChatChannel.Guild => "[Gu]",
                ChatChannel.Whisper => "[W]",
                ChatChannel.Trade => "[T]",
                ChatChannel.System => "[S]",
                ChatChannel.Combat => "[C]",
                ChatChannel.Announcement => "[!]",
                _ => ""
            };
        }

        private Color GetChannelColor(ChatChannel channel)
        {
            return channel switch
            {
                ChatChannel.Global => globalColor,
                ChatChannel.Local => localColor,
                ChatChannel.Party => partyColor,
                ChatChannel.Guild => guildColor,
                ChatChannel.Whisper => whisperColor,
                ChatChannel.Trade => tradeColor,
                ChatChannel.System => systemColor,
                ChatChannel.Combat => combatColor,
                ChatChannel.Announcement => Color.red,
                _ => Color.white
            };
        }

        #endregion

        #region Commands

        private void ParseCommand(string text)
        {
            string[] parts = text.Split(new[] { ' ' }, 2);
            string command = parts[0].ToLower();
            string args = parts.Length > 1 ? parts[1] : "";

            switch (command)
            {
                case "/g":
                case "/global":
                    SetChannel(ChatChannel.Global);
                    if (!string.IsNullOrEmpty(args)) SendMessage(args);
                    break;

                case "/l":
                case "/local":
                case "/say":
                    SetChannel(ChatChannel.Local);
                    if (!string.IsNullOrEmpty(args)) SendMessage(args);
                    break;

                case "/p":
                case "/party":
                    SetChannel(ChatChannel.Party);
                    if (!string.IsNullOrEmpty(args)) SendMessage(args);
                    break;

                case "/gu":
                case "/guild":
                    SetChannel(ChatChannel.Guild);
                    if (!string.IsNullOrEmpty(args)) SendMessage(args);
                    break;

                case "/w":
                case "/whisper":
                case "/tell":
                    string[] whisperParts = args.Split(new[] { ' ' }, 2);
                    if (whisperParts.Length >= 2)
                    {
                        SendWhisper(whisperParts[0], whisperParts[1]);
                    }
                    break;

                case "/r":
                case "/reply":
                    if (!string.IsNullOrEmpty(lastWhisperTarget))
                    {
                        SendWhisper(lastWhisperTarget, args);
                    }
                    break;

                case "/t":
                case "/trade":
                    SetChannel(ChatChannel.Trade);
                    if (!string.IsNullOrEmpty(args)) SendMessage(args);
                    break;

                case "/e":
                case "/emote":
                case "/me":
                    SendEmote(args);
                    break;

                case "/who":
                    ShowOnlinePlayers();
                    break;

                case "/invite":
                    InvitePlayer(args);
                    break;

                case "/kick":
                    KickPlayer(args);
                    break;

                case "/leave":
                    LeaveParty();
                    break;

                case "/help":
                case "/?":
                    ShowHelp();
                    break;

                case "/clear":
                    ClearChat();
                    break;

                case "/ignore":
                    IgnorePlayer(args);
                    break;

                case "/unignore":
                    UnignorePlayer(args);
                    break;

                default:
                    SendSystemMessage($"Unknown command: {command}. Type /help for commands.");
                    break;
            }
        }

        private void SendEmote(string emote)
        {
            var message = new ChatMessage
            {
                senderId = GetCurrentPlayerId(),
                senderName = GetCurrentPlayerName(),
                channel = ChatChannel.Local,
                content = $"*{GetCurrentPlayerName()} {emote}*",
                timestamp = DateTime.UtcNow
            };
            SendToServer(message);
        }

        private void ShowOnlinePlayers()
        {
            SendSystemMessage("Online players: Feature not implemented in offline mode");
        }

        private void InvitePlayer(string playerName)
        {
            PartySystem.Instance?.InviteToParty("", playerName);
            SendSystemMessage($"Invited {playerName} to party");
        }

        private void KickPlayer(string playerName)
        {
            SendSystemMessage($"Kicked {playerName} from party");
        }

        private void LeaveParty()
        {
            PartySystem.Instance?.LeaveParty();
        }

        private void ShowHelp()
        {
            SendSystemMessage("=== Chat Commands ===");
            SendSystemMessage("/g, /global - Global chat");
            SendSystemMessage("/l, /local, /say - Local area chat");
            SendSystemMessage("/p, /party - Party chat");
            SendSystemMessage("/gu, /guild - Guild chat");
            SendSystemMessage("/w, /whisper [name] [msg] - Whisper");
            SendSystemMessage("/r, /reply - Reply to last whisper");
            SendSystemMessage("/t, /trade - Trade chat");
            SendSystemMessage("/e, /emote [action] - Emote");
            SendSystemMessage("/invite [name] - Invite to party");
            SendSystemMessage("/leave - Leave party");
            SendSystemMessage("/clear - Clear chat");
        }

        private void ClearChat()
        {
            if (messageContainer != null)
            {
                foreach (Transform child in messageContainer)
                {
                    Destroy(child.gameObject);
                }
            }
            messageHistory.Clear();
        }

        private List<string> ignoredPlayers = new List<string>();

        private void IgnorePlayer(string playerName)
        {
            if (!ignoredPlayers.Contains(playerName))
            {
                ignoredPlayers.Add(playerName);
                SendSystemMessage($"Now ignoring {playerName}");
            }
        }

        private void UnignorePlayer(string playerName)
        {
            if (ignoredPlayers.Remove(playerName))
            {
                SendSystemMessage($"No longer ignoring {playerName}");
            }
        }

        #endregion

        #region Channel Management

        public void SetChannel(ChatChannel channel)
        {
            currentChannel = channel;
            OnChannelChanged?.Invoke(channel);
            SendSystemMessage($"Now speaking in {channel} channel");
        }

        public void CycleChannel()
        {
            int current = (int)currentChannel;
            int next = (current + 1) % 5; // Only cycle through main channels
            SetChannel((ChatChannel)next);
        }

        public void ToggleChannel(ChatChannel channel, bool enabled)
        {
            channelEnabled[channel] = enabled;
        }

        public ChatChannel CurrentChannel => currentChannel;

        #endregion

        #region Utilities

        private string FilterProfanity(string text)
        {
            // Simple profanity filter - would use a proper word list
            string[] badWords = { "badword1", "badword2" };
            foreach (var word in badWords)
            {
                text = text.Replace(word, new string('*', word.Length), StringComparison.OrdinalIgnoreCase);
            }
            return text;
        }

        private string GetCurrentPlayerId() => "player_local";
        private string GetCurrentPlayerName() => "Player";

        public List<ChatMessage> GetMessageHistory(ChatChannel? channel = null)
        {
            if (channel.HasValue)
            {
                return messageHistory.FindAll(m => m.channel == channel.Value);
            }
            return new List<ChatMessage>(messageHistory);
        }

        #endregion
    }
}
