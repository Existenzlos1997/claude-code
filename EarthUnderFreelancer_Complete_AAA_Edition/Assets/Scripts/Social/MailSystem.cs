using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EarthUnderFreelancer.Social
{
    /// <summary>
    /// In-game Mail System for MMO
    /// Allows players to send messages, items, and credits to each other
    /// </summary>
    public class MailSystem : MonoBehaviour
    {
        public static MailSystem Instance { get; private set; }

        [Header("Configuration")]
        [SerializeField] private int maxInboxSize = 100;
        [SerializeField] private int maxSentSize = 50;
        [SerializeField] private float mailExpirationDays = 30f;
        [SerializeField] private int sendCostCredits = 30;
        [SerializeField] private int itemAttachmentCost = 50;

        public List<MailMessage> Inbox { get; private set; }
        public List<MailMessage> Sent { get; private set; }
        public int UnreadCount => Inbox.FindAll(m => !m.isRead).Count;

        public event Action<MailMessage> OnMailReceived;
        public event Action<MailMessage> OnMailRead;
        public event Action<int> OnUnreadCountChanged;
        public event Action<MailMessage> OnAttachmentsCollected;

        #region Data Structures

        [Serializable]
        public class MailMessage
        {
            public string mailId;
            public string senderId;
            public string senderName;
            public string recipientId;
            public string recipientName;
            public string subject;
            public string body;
            public DateTime sentAt;
            public DateTime expiresAt;
            public bool isRead;
            public bool isSystem;
            public MailType type;
            
            // Attachments
            public List<MailAttachment> attachments;
            public long creditsAttached;
            public bool attachmentsCollected;
        }

        [Serializable]
        public class MailAttachment
        {
            public string itemId;
            public string itemName;
            public int quantity;
            public string itemIcon;
        }

        public enum MailType
        {
            Player,
            System,
            Guild,
            AuctionHouse,
            Reward,
            Newsletter
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
            Inbox = new List<MailMessage>();
            Sent = new List<MailMessage>();
            
            // Check for expired mail periodically
            InvokeRepeating(nameof(CleanExpiredMail), 60f, 300f);
        }

        #region Sending Mail

        public async Task<bool> SendMail(string recipientId, string recipientName, string subject, string body,
            List<MailAttachment> attachments = null, long credits = 0)
        {
            if (string.IsNullOrWhiteSpace(recipientId) || string.IsNullOrWhiteSpace(subject))
            {
                Debug.LogError("[Mail] Invalid recipient or subject!");
                return false;
            }

            if (Sent.Count >= maxSentSize)
            {
                // Remove oldest sent mail
                Sent.RemoveAt(0);
            }

            // Calculate cost
            int totalCost = sendCostCredits;
            if (attachments != null && attachments.Count > 0)
            {
                totalCost += itemAttachmentCost * attachments.Count;
            }
            // Would deduct credits here

            var mail = new MailMessage
            {
                mailId = Guid.NewGuid().ToString(),
                senderId = GetCurrentPlayerId(),
                senderName = GetCurrentPlayerName(),
                recipientId = recipientId,
                recipientName = recipientName,
                subject = subject,
                body = body,
                sentAt = DateTime.UtcNow,
                expiresAt = DateTime.UtcNow.AddDays(mailExpirationDays),
                isRead = false,
                isSystem = false,
                type = MailType.Player,
                attachments = attachments ?? new List<MailAttachment>(),
                creditsAttached = credits,
                attachmentsCollected = false
            };

            Sent.Add(mail);

            // Would send to server
            await Task.Delay(100);

            Debug.Log($"[Mail] Sent mail to {recipientName}: {subject}");
            return true;
        }

        public void SendSystemMail(string recipientId, string subject, string body,
            List<MailAttachment> attachments = null, long credits = 0, MailType type = MailType.System)
        {
            var mail = new MailMessage
            {
                mailId = Guid.NewGuid().ToString(),
                senderId = "SYSTEM",
                senderName = "System",
                recipientId = recipientId,
                subject = subject,
                body = body,
                sentAt = DateTime.UtcNow,
                expiresAt = DateTime.UtcNow.AddDays(mailExpirationDays),
                isRead = false,
                isSystem = true,
                type = type,
                attachments = attachments ?? new List<MailAttachment>(),
                creditsAttached = credits,
                attachmentsCollected = false
            };

            // If recipient is current player, add to inbox
            if (recipientId == GetCurrentPlayerId())
            {
                ReceiveMail(mail);
            }
        }

        #endregion

        #region Receiving Mail

        public void ReceiveMail(MailMessage mail)
        {
            if (Inbox.Count >= maxInboxSize)
            {
                Debug.LogWarning("[Mail] Inbox is full! Deleting oldest mail.");
                DeleteMail(Inbox[0].mailId);
            }

            Inbox.Insert(0, mail);
            OnMailReceived?.Invoke(mail);
            OnUnreadCountChanged?.Invoke(UnreadCount);

            Debug.Log($"[Mail] Received mail from {mail.senderName}: {mail.subject}");
        }

        public void ReadMail(string mailId)
        {
            var mail = Inbox.Find(m => m.mailId == mailId);
            if (mail != null && !mail.isRead)
            {
                mail.isRead = true;
                OnMailRead?.Invoke(mail);
                OnUnreadCountChanged?.Invoke(UnreadCount);
            }
        }

        public bool CollectAttachments(string mailId)
        {
            var mail = Inbox.Find(m => m.mailId == mailId);
            if (mail == null || mail.attachmentsCollected) return false;

            // Would add items to player inventory
            foreach (var attachment in mail.attachments)
            {
                Debug.Log($"[Mail] Collected {attachment.quantity}x {attachment.itemName}");
            }

            // Would add credits
            if (mail.creditsAttached > 0)
            {
                Debug.Log($"[Mail] Collected {mail.creditsAttached} credits");
            }

            mail.attachmentsCollected = true;
            OnAttachmentsCollected?.Invoke(mail);

            return true;
        }

        public bool DeleteMail(string mailId)
        {
            var mail = Inbox.Find(m => m.mailId == mailId);
            if (mail == null) return false;

            // Can't delete mail with uncollected attachments
            if (!mail.attachmentsCollected && (mail.attachments.Count > 0 || mail.creditsAttached > 0))
            {
                Debug.LogWarning("[Mail] Collect attachments before deleting!");
                return false;
            }

            Inbox.Remove(mail);
            OnUnreadCountChanged?.Invoke(UnreadCount);
            return true;
        }

        public void DeleteAllRead()
        {
            Inbox.RemoveAll(m => m.isRead && m.attachmentsCollected);
            OnUnreadCountChanged?.Invoke(UnreadCount);
        }

        #endregion

        #region Queries

        public List<MailMessage> GetUnreadMail()
        {
            return Inbox.FindAll(m => !m.isRead);
        }

        public List<MailMessage> GetMailWithAttachments()
        {
            return Inbox.FindAll(m => !m.attachmentsCollected && 
                (m.attachments.Count > 0 || m.creditsAttached > 0));
        }

        public MailMessage GetMail(string mailId)
        {
            return Inbox.Find(m => m.mailId == mailId);
        }

        #endregion

        #region Maintenance

        private void CleanExpiredMail()
        {
            var now = DateTime.UtcNow;
            int removed = Inbox.RemoveAll(m => m.expiresAt < now && m.attachmentsCollected);
            if (removed > 0)
            {
                Debug.Log($"[Mail] Cleaned {removed} expired messages");
                OnUnreadCountChanged?.Invoke(UnreadCount);
            }
        }

        #endregion

        private string GetCurrentPlayerId() => "player_local";
        private string GetCurrentPlayerName() => "Player";
    }
}
