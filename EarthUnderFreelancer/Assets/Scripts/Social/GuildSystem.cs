using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EarthUnderFreelancer.Social
{
    /// <summary>
    /// Complete Guild System for MMORPG functionality
    /// Similar to WoW's guild system with ranks, permissions, bank, and progression
    /// </summary>
    public class GuildSystem : MonoBehaviour
    {
        public static GuildSystem Instance { get; private set; }

        [Header("Configuration")]
        [SerializeField] private int maxGuildMembers = 500;
        [SerializeField] private int guildCreationCost = 100000;
        [SerializeField] private int maxGuildRanks = 10;
        [SerializeField] private int maxGuildBankTabs = 8;

        // Current player's guild
        public GuildData CurrentGuild { get; private set; }
        public GuildRank CurrentPlayerRank { get; private set; }
        public bool IsInGuild => CurrentGuild != null;
        public bool IsGuildMaster => CurrentPlayerRank?.rankIndex == 0;

        // Events
        public event Action<GuildData> OnGuildJoined;
        public event Action OnGuildLeft;
        public event Action<GuildData> OnGuildUpdated;
        public event Action<GuildMember> OnMemberJoined;
        public event Action<GuildMember> OnMemberLeft;
        public event Action<GuildMember> OnMemberPromoted;
        public event Action<GuildMember> OnMemberDemoted;
        public event Action<GuildMessage> OnGuildMessageReceived;
        public event Action<int> OnGuildLevelUp;

        #region Data Structures

        [Serializable]
        public class GuildData
        {
            public string guildId;
            public string guildName;
            public string guildTag;
            public string description;
            public string motd;
            public DateTime createdAt;
            public string leaderPlayerId;
            public string leaderName;
            public int guildLevel;
            public long guildExperience;
            public long experienceToNextLevel;
            public List<GuildMember> members;
            public List<GuildRank> ranks;
            public List<GuildBankTab> bankTabs;
            public long bankCredits;
            public GuildSettings settings;
        }

        [Serializable]
        public class GuildMember
        {
            public string playerId;
            public string playerName;
            public int rankIndex;
            public DateTime joinedAt;
            public DateTime lastOnline;
            public bool isOnline;
            public int level;
            public long contributedExperience;
            public long contributedCredits;
        }

        [Serializable]
        public class GuildRank
        {
            public int rankIndex;
            public string rankName;
            public GuildPermissions permissions;
            public bool isDefault;
        }

        [Serializable]
        public class GuildPermissions
        {
            public bool canInvite;
            public bool canKick;
            public bool canPromote;
            public bool canDemote;
            public bool canEditMotd;
            public bool canAccessBankTab1;
            public bool canWithdrawCredits;
            public bool canDepositCredits;
        }

        [Serializable]
        public class GuildBankTab
        {
            public int tabIndex;
            public string tabName;
            public List<GuildBankSlot> slots;
            public int maxSlots = 98;
        }

        [Serializable]
        public class GuildBankSlot
        {
            public int slotIndex;
            public string itemId;
            public int quantity;
        }

        [Serializable]
        public class GuildSettings
        {
            public bool isRecruiting;
            public int minLevelToApply;
            public bool requiresApplication;
        }

        [Serializable]
        public class GuildMessage
        {
            public string senderId;
            public string senderName;
            public string content;
            public DateTime timestamp;
        }

        #endregion

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

        #region Guild Management

        public async Task<bool> CreateGuild(string guildName, string guildTag)
        {
            if (IsInGuild) return false;
            if (string.IsNullOrWhiteSpace(guildName) || guildName.Length < 3) return false;
            if (string.IsNullOrWhiteSpace(guildTag) || guildTag.Length < 2 || guildTag.Length > 4) return false;

            var guild = new GuildData
            {
                guildId = Guid.NewGuid().ToString(),
                guildName = guildName,
                guildTag = guildTag.ToUpper(),
                motd = "Welcome to " + guildName + "!",
                createdAt = DateTime.UtcNow,
                guildLevel = 1,
                guildExperience = 0,
                experienceToNextLevel = 10000,
                members = new List<GuildMember>(),
                ranks = CreateDefaultRanks(),
                bankTabs = new List<GuildBankTab>(),
                bankCredits = 0,
                settings = new GuildSettings { isRecruiting = true, minLevelToApply = 1 }
            };

            guild.members.Add(new GuildMember
            {
                playerId = GetCurrentPlayerId(),
                playerName = GetCurrentPlayerName(),
                rankIndex = 0,
                joinedAt = DateTime.UtcNow,
                isOnline = true
            });

            await Task.Delay(300);

            CurrentGuild = guild;
            CurrentPlayerRank = guild.ranks[0];
            OnGuildJoined?.Invoke(guild);

            Debug.Log($"[Guild] Created: {guildName} [{guildTag}]");
            return true;
        }

        private List<GuildRank> CreateDefaultRanks()
        {
            return new List<GuildRank>
            {
                new GuildRank { rankIndex = 0, rankName = "Guild Master", permissions = new GuildPermissions { canInvite = true, canKick = true, canPromote = true, canDemote = true, canEditMotd = true, canAccessBankTab1 = true, canWithdrawCredits = true, canDepositCredits = true } },
                new GuildRank { rankIndex = 1, rankName = "Officer", permissions = new GuildPermissions { canInvite = true, canKick = true, canPromote = true, canAccessBankTab1 = true, canDepositCredits = true } },
                new GuildRank { rankIndex = 2, rankName = "Veteran", permissions = new GuildPermissions { canInvite = true, canAccessBankTab1 = true, canDepositCredits = true } },
                new GuildRank { rankIndex = 3, rankName = "Member", isDefault = true, permissions = new GuildPermissions { canAccessBankTab1 = true, canDepositCredits = true } },
                new GuildRank { rankIndex = 4, rankName = "Initiate", permissions = new GuildPermissions { canDepositCredits = true } }
            };
        }

        public async Task<bool> LeaveGuild()
        {
            if (!IsInGuild) return false;
            if (IsGuildMaster) return false;

            await Task.Delay(200);
            CurrentGuild = null;
            CurrentPlayerRank = null;
            OnGuildLeft?.Invoke();
            return true;
        }

        public async Task<bool> InvitePlayer(string playerId, string playerName)
        {
            if (!IsInGuild || !CurrentPlayerRank.permissions.canInvite) return false;
            if (CurrentGuild.members.Count >= maxGuildMembers) return false;

            await Task.Delay(100);
            Debug.Log($"[Guild] Invited {playerName}");
            return true;
        }

        public async Task<bool> KickMember(string playerId)
        {
            if (!IsInGuild || !CurrentPlayerRank.permissions.canKick) return false;

            var member = CurrentGuild.members.Find(m => m.playerId == playerId);
            if (member == null || member.rankIndex <= CurrentPlayerRank.rankIndex) return false;

            CurrentGuild.members.Remove(member);
            OnMemberLeft?.Invoke(member);
            await Task.Delay(100);
            return true;
        }

        public async Task<bool> PromoteMember(string playerId)
        {
            if (!IsInGuild || !CurrentPlayerRank.permissions.canPromote) return false;

            var member = CurrentGuild.members.Find(m => m.playerId == playerId);
            if (member == null || member.rankIndex <= 0) return false;

            member.rankIndex--;
            OnMemberPromoted?.Invoke(member);
            await Task.Delay(100);
            return true;
        }

        public void AddGuildExperience(long amount)
        {
            if (!IsInGuild) return;

            CurrentGuild.guildExperience += amount;
            while (CurrentGuild.guildExperience >= CurrentGuild.experienceToNextLevel && CurrentGuild.guildLevel < 25)
            {
                CurrentGuild.guildExperience -= CurrentGuild.experienceToNextLevel;
                CurrentGuild.guildLevel++;
                CurrentGuild.experienceToNextLevel = (long)(10000 * Mathf.Pow(CurrentGuild.guildLevel, 1.5f));
                OnGuildLevelUp?.Invoke(CurrentGuild.guildLevel);
            }
        }

        public void SendGuildMessage(string content)
        {
            if (!IsInGuild) return;
            OnGuildMessageReceived?.Invoke(new GuildMessage
            {
                senderId = GetCurrentPlayerId(),
                senderName = GetCurrentPlayerName(),
                content = content,
                timestamp = DateTime.UtcNow
            });
        }

        private string GetCurrentPlayerId() => "player_local";
        private string GetCurrentPlayerName() => "Player";

        #endregion
    }
}
