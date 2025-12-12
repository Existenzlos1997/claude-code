using UnityEngine;
using System;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Social
{
    /// <summary>
    /// Party/Group System for cooperative gameplay
    /// Allows players to form groups for missions, raids, and PvP
    /// </summary>
    public class PartySystem : MonoBehaviour
    {
        public static PartySystem Instance { get; private set; }

        [Header("Configuration")]
        [SerializeField] private int maxPartySize = 5;
        [SerializeField] private int maxRaidSize = 40;

        public PartyData CurrentParty { get; private set; }
        public bool IsInParty => CurrentParty != null;
        public bool IsPartyLeader => CurrentParty?.leaderId == GetCurrentPlayerId();
        public bool IsInRaid => CurrentParty?.isRaid ?? false;

        public event Action<PartyData> OnPartyJoined;
        public event Action OnPartyLeft;
        public event Action<PartyMember> OnMemberJoined;
        public event Action<PartyMember> OnMemberLeft;
        public event Action<string> OnLeaderChanged;
        public event Action<LootRule> OnLootRuleChanged;

        [Serializable]
        public class PartyData
        {
            public string partyId;
            public string leaderId;
            public string leaderName;
            public List<PartyMember> members;
            public bool isRaid;
            public LootRule lootRule;
            public int lootThreshold;
            public bool isPublic;
            public string dungeonId;
        }

        [Serializable]
        public class PartyMember
        {
            public string playerId;
            public string playerName;
            public int level;
            public string aircraftType;
            public float health;
            public float maxHealth;
            public bool isOnline;
            public bool isReady;
            public PartyRole role;
            public Vector3 position;
        }

        public enum PartyRole
        {
            DPS,
            Tank,
            Support,
            Scout
        }

        public enum LootRule
        {
            FreeForAll,
            RoundRobin,
            MasterLooter,
            NeedBeforeGreed,
            GroupLoot
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

        #region Party Management

        public bool CreateParty()
        {
            if (IsInParty) return false;

            CurrentParty = new PartyData
            {
                partyId = Guid.NewGuid().ToString(),
                leaderId = GetCurrentPlayerId(),
                leaderName = GetCurrentPlayerName(),
                members = new List<PartyMember>
                {
                    new PartyMember
                    {
                        playerId = GetCurrentPlayerId(),
                        playerName = GetCurrentPlayerName(),
                        level = 1,
                        health = 100,
                        maxHealth = 100,
                        isOnline = true,
                        isReady = true,
                        role = PartyRole.DPS
                    }
                },
                isRaid = false,
                lootRule = LootRule.RoundRobin,
                lootThreshold = 2,
                isPublic = false
            };

            OnPartyJoined?.Invoke(CurrentParty);
            Debug.Log("[Party] Created party");
            return true;
        }

        public bool InviteToParty(string playerId, string playerName)
        {
            if (!IsInParty) CreateParty();
            if (!IsPartyLeader) return false;

            int maxSize = CurrentParty.isRaid ? maxRaidSize : maxPartySize;
            if (CurrentParty.members.Count >= maxSize) return false;

            Debug.Log($"[Party] Invited {playerName}");
            return true;
        }

        public bool JoinParty(string partyId)
        {
            if (IsInParty) return false;
            // Would join via network
            return true;
        }

        public bool LeaveParty()
        {
            if (!IsInParty) return false;

            if (IsPartyLeader && CurrentParty.members.Count > 1)
            {
                // Transfer leadership
                var newLeader = CurrentParty.members.Find(m => m.playerId != GetCurrentPlayerId());
                if (newLeader != null)
                {
                    CurrentParty.leaderId = newLeader.playerId;
                    CurrentParty.leaderName = newLeader.playerName;
                }
            }

            CurrentParty = null;
            OnPartyLeft?.Invoke();
            Debug.Log("[Party] Left party");
            return true;
        }

        public bool KickMember(string playerId)
        {
            if (!IsPartyLeader) return false;
            if (playerId == GetCurrentPlayerId()) return false;

            var member = CurrentParty.members.Find(m => m.playerId == playerId);
            if (member != null)
            {
                CurrentParty.members.Remove(member);
                OnMemberLeft?.Invoke(member);
                return true;
            }
            return false;
        }

        public bool PromoteToLeader(string playerId)
        {
            if (!IsPartyLeader) return false;

            var member = CurrentParty.members.Find(m => m.playerId == playerId);
            if (member != null)
            {
                CurrentParty.leaderId = playerId;
                CurrentParty.leaderName = member.playerName;
                OnLeaderChanged?.Invoke(playerId);
                return true;
            }
            return false;
        }

        public bool ConvertToRaid()
        {
            if (!IsPartyLeader) return false;
            if (CurrentParty.isRaid) return false;

            CurrentParty.isRaid = true;
            Debug.Log("[Party] Converted to raid");
            return true;
        }

        public bool SetLootRule(LootRule rule)
        {
            if (!IsPartyLeader) return false;

            CurrentParty.lootRule = rule;
            OnLootRuleChanged?.Invoke(rule);
            return true;
        }

        public void SetRole(PartyRole role)
        {
            if (!IsInParty) return;

            var member = CurrentParty.members.Find(m => m.playerId == GetCurrentPlayerId());
            if (member != null)
            {
                member.role = role;
            }
        }

        public void SetReady(bool ready)
        {
            if (!IsInParty) return;

            var member = CurrentParty.members.Find(m => m.playerId == GetCurrentPlayerId());
            if (member != null)
            {
                member.isReady = ready;
            }
        }

        public bool AreAllMembersReady()
        {
            if (!IsInParty) return false;
            return CurrentParty.members.TrueForAll(m => m.isReady);
        }

        public void UpdateMemberPosition(string playerId, Vector3 position)
        {
            if (!IsInParty) return;

            var member = CurrentParty.members.Find(m => m.playerId == playerId);
            if (member != null)
            {
                member.position = position;
            }
        }

        public void UpdateMemberHealth(string playerId, float health, float maxHealth)
        {
            if (!IsInParty) return;

            var member = CurrentParty.members.Find(m => m.playerId == playerId);
            if (member != null)
            {
                member.health = health;
                member.maxHealth = maxHealth;
            }
        }

        #endregion

        #region Helpers

        private string GetCurrentPlayerId() => "player_local";
        private string GetCurrentPlayerName() => "Player";

        public List<PartyMember> GetPartyMembers()
        {
            return CurrentParty?.members ?? new List<PartyMember>();
        }

        public PartyMember GetMember(string playerId)
        {
            return CurrentParty?.members.Find(m => m.playerId == playerId);
        }

        #endregion
    }
}
