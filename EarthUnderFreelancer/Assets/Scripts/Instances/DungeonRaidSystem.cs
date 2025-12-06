using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EarthUnderFreelancer.Instances
{
    /// <summary>
    /// Dungeon and Raid Instance System
    /// Manages instanced PvE content with bosses, loot, and progression
    /// </summary>
    public class DungeonRaidSystem : MonoBehaviour
    {
        public static DungeonRaidSystem Instance { get; private set; }

        [Header("Configuration")]
        [SerializeField] private List<DungeonData> availableDungeons;
        [SerializeField] private List<RaidData> availableRaids;
        [SerializeField] private int maxDungeonLockouts = 10;
        [SerializeField] private float weeklyResetDays = 7f;

        public InstanceData CurrentInstance { get; private set; }
        public bool IsInInstance => CurrentInstance != null;

        public event Action<InstanceData> OnInstanceEntered;
        public event Action OnInstanceLeft;
        public event Action<BossData> OnBossDefeated;
        public event Action<List<LootItem>> OnLootReceived;
        public event Action<InstanceProgress> OnProgressUpdated;

        #region Data Structures

        [Serializable]
        public class DungeonData
        {
            public string dungeonId;
            public string dungeonName;
            public string description;
            public DifficultyTier difficulty;
            public int minLevel;
            public int recommendedItemLevel;
            public int minPlayers;
            public int maxPlayers;
            public float estimatedTimeMinutes;
            public List<BossData> bosses;
            public List<string> lootTable;
            public string sceneName;
            public Sprite icon;
            public Nation faction;
        }

        [Serializable]
        public class RaidData : DungeonData
        {
            public bool isLegacy;
            public int tierLevel;
            public List<string> achievements;
            public RaidSize raidSize;
        }

        public enum RaidSize
        {
            Small_10,
            Normal_25,
            Large_40
        }

        public enum DifficultyTier
        {
            Normal,
            Hard,
            Expert,
            Nightmare,
            Mythic
        }

        [Serializable]
        public class BossData
        {
            public string bossId;
            public string bossName;
            public string description;
            public float health;
            public float damage;
            public List<BossAbility> abilities;
            public List<BossPhase> phases;
            public List<string> lootTable;
            public int bossIndex;
            public bool isOptional;
            public bool isDefeated;
            public string achievementId;
        }

        [Serializable]
        public class BossAbility
        {
            public string abilityName;
            public float damage;
            public float cooldown;
            public float castTime;
            public AbilityType type;
            public string warningMessage;
        }

        public enum AbilityType
        {
            Damage,
            DOT,
            Debuff,
            Summon,
            Enrage,
            AreaDenial,
            Knockback,
            Stun
        }

        [Serializable]
        public class BossPhase
        {
            public int phaseNumber;
            public float healthThreshold;
            public string phaseName;
            public List<BossAbility> phaseAbilities;
        }

        [Serializable]
        public class InstanceData
        {
            public string instanceId;
            public string dungeonId;
            public string dungeonName;
            public DifficultyTier difficulty;
            public DateTime startTime;
            public List<string> partyMemberIds;
            public InstanceProgress progress;
            public List<LootItem> pendingLoot;
            public bool isCompleted;
        }

        [Serializable]
        public class InstanceProgress
        {
            public int bossesDefeated;
            public int totalBosses;
            public List<string> defeatedBossIds;
            public float completionPercent;
            public int deathCount;
            public int wipeCount;
            public TimeSpan elapsedTime;
        }

        [Serializable]
        public class LootItem
        {
            public string itemId;
            public string itemName;
            public ItemRarity rarity;
            public string sourceId;
            public string winnerId;
            public bool isPersonal;
        }

        public enum ItemRarity
        {
            Common,
            Uncommon,
            Rare,
            Epic,
            Legendary,
            Artifact
        }

        [Serializable]
        public class InstanceLockout
        {
            public string dungeonId;
            public DifficultyTier difficulty;
            public DateTime expiresAt;
            public List<string> defeatedBossIds;
        }

        #endregion

        private List<InstanceLockout> playerLockouts = new List<InstanceLockout>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeDungeons();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeDungeons()
        {
            availableDungeons = new List<DungeonData>
            {
                new DungeonData
                {
                    dungeonId = "asteroid_mines",
                    dungeonName = "Abandoned Asteroid Mines",
                    description = "An old mining facility overrun by pirates and automated defenses.",
                    difficulty = DifficultyTier.Normal,
                    minLevel = 10,
                    recommendedItemLevel = 50,
                    minPlayers = 1,
                    maxPlayers = 5,
                    estimatedTimeMinutes = 30,
                    bosses = new List<BossData>
                    {
                        new BossData { bossId = "mining_droid", bossName = "Rogue Mining Droid", health = 50000, bossIndex = 0 },
                        new BossData { bossId = "pirate_captain", bossName = "Captain Blackstar", health = 100000, bossIndex = 1 }
                    }
                },
                new DungeonData
                {
                    dungeonId = "derelict_carrier",
                    dungeonName = "Derelict Carrier",
                    description = "A massive carrier drifting in space, home to hostile AI and unknown dangers.",
                    difficulty = DifficultyTier.Hard,
                    minLevel = 20,
                    recommendedItemLevel = 100,
                    minPlayers = 1,
                    maxPlayers = 5,
                    estimatedTimeMinutes = 45,
                    bosses = new List<BossData>
                    {
                        new BossData { bossId = "security_chief", bossName = "Security Chief Omega", health = 150000, bossIndex = 0 },
                        new BossData { bossId = "rogue_ai", bossName = "HAL-9001", health = 200000, bossIndex = 1 },
                        new BossData { bossId = "carrier_core", bossName = "Corrupted Ship Core", health = 300000, bossIndex = 2 }
                    }
                },
                new DungeonData
                {
                    dungeonId = "alien_hive",
                    dungeonName = "Xenomorph Hive",
                    description = "An alien infested station. High danger level.",
                    difficulty = DifficultyTier.Expert,
                    minLevel = 35,
                    recommendedItemLevel = 200,
                    minPlayers = 3,
                    maxPlayers = 5,
                    estimatedTimeMinutes = 60,
                    bosses = new List<BossData>
                    {
                        new BossData { bossId = "brood_mother", bossName = "Brood Mother", health = 400000, bossIndex = 0 },
                        new BossData { bossId = "hive_guardian", bossName = "Hive Guardian", health = 500000, bossIndex = 1 },
                        new BossData { bossId = "xeno_queen", bossName = "Xenomorph Queen", health = 800000, bossIndex = 2 }
                    }
                }
            };

            availableRaids = new List<RaidData>
            {
                new RaidData
                {
                    dungeonId = "station_siege",
                    dungeonName = "Station Siege: Command Center",
                    description = "Lead a massive assault on the enemy command station.",
                    difficulty = DifficultyTier.Normal,
                    minLevel = 50,
                    recommendedItemLevel = 300,
                    minPlayers = 10,
                    maxPlayers = 25,
                    estimatedTimeMinutes = 120,
                    raidSize = RaidSize.Normal_25,
                    tierLevel = 1,
                    bosses = new List<BossData>
                    {
                        new BossData { bossId = "gate_keeper", bossName = "Gate Keeper", health = 2000000, bossIndex = 0 },
                        new BossData { bossId = "fleet_admiral", bossName = "Fleet Admiral Zorn", health = 3000000, bossIndex = 1 },
                        new BossData { bossId = "war_council", bossName = "The War Council", health = 5000000, bossIndex = 2 },
                        new BossData { bossId = "supreme_commander", bossName = "Supreme Commander Vex", health = 10000000, bossIndex = 3 }
                    }
                }
            };
        }

        #region Instance Management

        public async Task<bool> EnterDungeon(string dungeonId, DifficultyTier difficulty)
        {
            if (IsInInstance)
            {
                Debug.LogError("[Dungeon] Already in an instance!");
                return false;
            }

            var dungeon = availableDungeons.Find(d => d.dungeonId == dungeonId);
            if (dungeon == null)
            {
                Debug.LogError("[Dungeon] Dungeon not found!");
                return false;
            }

            // Check lockout
            if (HasLockout(dungeonId, difficulty))
            {
                Debug.LogWarning("[Dungeon] Instance is locked!");
                // Still can enter, but some bosses may be unavailable
            }

            // Check party size
            var party = Social.PartySystem.Instance?.GetPartyMembers();
            int partySize = party?.Count ?? 1;
            if (partySize < dungeon.minPlayers)
            {
                Debug.LogError($"[Dungeon] Need at least {dungeon.minPlayers} players!");
                return false;
            }

            // Create instance
            CurrentInstance = new InstanceData
            {
                instanceId = Guid.NewGuid().ToString(),
                dungeonId = dungeonId,
                dungeonName = dungeon.dungeonName,
                difficulty = difficulty,
                startTime = DateTime.UtcNow,
                partyMemberIds = new List<string>(),
                progress = new InstanceProgress
                {
                    bossesDefeated = 0,
                    totalBosses = dungeon.bosses.Count,
                    defeatedBossIds = new List<string>(),
                    completionPercent = 0
                },
                pendingLoot = new List<LootItem>(),
                isCompleted = false
            };

            // Load instance scene
            await Task.Delay(1000); // Simulate loading

            OnInstanceEntered?.Invoke(CurrentInstance);
            Debug.Log($"[Dungeon] Entered {dungeon.dungeonName} ({difficulty})");
            return true;
        }

        public async Task<bool> EnterRaid(string raidId, DifficultyTier difficulty)
        {
            var raid = availableRaids.Find(r => r.dungeonId == raidId);
            if (raid == null)
            {
                Debug.LogError("[Raid] Raid not found!");
                return false;
            }

            // Similar to dungeon entry but with raid requirements
            return await EnterDungeon(raidId, difficulty);
        }

        public void LeaveInstance()
        {
            if (!IsInInstance) return;

            CurrentInstance = null;
            OnInstanceLeft?.Invoke();
            Debug.Log("[Dungeon] Left instance");
        }

        public void ResetInstance()
        {
            if (!IsInInstance) return;

            CurrentInstance.progress = new InstanceProgress
            {
                bossesDefeated = 0,
                totalBosses = CurrentInstance.progress.totalBosses,
                defeatedBossIds = new List<string>(),
                completionPercent = 0
            };

            Debug.Log("[Dungeon] Instance reset");
        }

        #endregion

        #region Boss Combat

        public void DefeatBoss(string bossId)
        {
            if (!IsInInstance) return;

            if (CurrentInstance.progress.defeatedBossIds.Contains(bossId))
            {
                Debug.Log("[Dungeon] Boss already defeated");
                return;
            }

            var dungeon = availableDungeons.Find(d => d.dungeonId == CurrentInstance.dungeonId) ??
                          (DungeonData)availableRaids.Find(r => r.dungeonId == CurrentInstance.dungeonId);

            var boss = dungeon?.bosses.Find(b => b.bossId == bossId);
            if (boss == null) return;

            boss.isDefeated = true;
            CurrentInstance.progress.defeatedBossIds.Add(bossId);
            CurrentInstance.progress.bossesDefeated++;
            CurrentInstance.progress.completionPercent = 
                (float)CurrentInstance.progress.bossesDefeated / CurrentInstance.progress.totalBosses * 100f;

            // Generate loot
            var loot = GenerateBossLoot(boss, CurrentInstance.difficulty);
            CurrentInstance.pendingLoot.AddRange(loot);

            OnBossDefeated?.Invoke(boss);
            OnLootReceived?.Invoke(loot);
            OnProgressUpdated?.Invoke(CurrentInstance.progress);

            // Check completion
            if (CurrentInstance.progress.bossesDefeated >= CurrentInstance.progress.totalBosses)
            {
                CompleteInstance();
            }

            // Create lockout
            UpdateLockout(CurrentInstance.dungeonId, CurrentInstance.difficulty, bossId);

            Debug.Log($"[Dungeon] Defeated {boss.bossName}!");
        }

        private List<LootItem> GenerateBossLoot(BossData boss, DifficultyTier difficulty)
        {
            var loot = new List<LootItem>();

            // Higher difficulty = better loot
            int numItems = difficulty switch
            {
                DifficultyTier.Normal => 2,
                DifficultyTier.Hard => 3,
                DifficultyTier.Expert => 4,
                DifficultyTier.Nightmare => 5,
                DifficultyTier.Mythic => 6,
                _ => 2
            };

            for (int i = 0; i < numItems; i++)
            {
                loot.Add(new LootItem
                {
                    itemId = $"loot_{boss.bossId}_{i}",
                    itemName = $"Epic Equipment from {boss.bossName}",
                    rarity = GetRandomRarity(difficulty),
                    sourceId = boss.bossId,
                    isPersonal = false
                });
            }

            return loot;
        }

        private ItemRarity GetRandomRarity(DifficultyTier difficulty)
        {
            float roll = UnityEngine.Random.value;
            float epicChance = 0.1f + ((int)difficulty * 0.05f);
            float legendaryChance = 0.02f + ((int)difficulty * 0.02f);

            if (roll < legendaryChance) return ItemRarity.Legendary;
            if (roll < epicChance) return ItemRarity.Epic;
            if (roll < 0.4f) return ItemRarity.Rare;
            return ItemRarity.Uncommon;
        }

        private void CompleteInstance()
        {
            CurrentInstance.isCompleted = true;
            CurrentInstance.progress.elapsedTime = DateTime.UtcNow - CurrentInstance.startTime;

            // Bonus loot for completion
            CurrentInstance.pendingLoot.Add(new LootItem
            {
                itemId = "completion_bonus",
                itemName = "Instance Completion Chest",
                rarity = ItemRarity.Epic,
                isPersonal = true
            });

            Debug.Log($"[Dungeon] Instance completed in {CurrentInstance.progress.elapsedTime.TotalMinutes:F1} minutes!");
        }

        public void RegisterWipe()
        {
            if (!IsInInstance) return;
            CurrentInstance.progress.wipeCount++;
            OnProgressUpdated?.Invoke(CurrentInstance.progress);
        }

        public void RegisterDeath()
        {
            if (!IsInInstance) return;
            CurrentInstance.progress.deathCount++;
            OnProgressUpdated?.Invoke(CurrentInstance.progress);
        }

        #endregion

        #region Lockouts

        public bool HasLockout(string dungeonId, DifficultyTier difficulty)
        {
            CleanExpiredLockouts();
            return playerLockouts.Exists(l => 
                l.dungeonId == dungeonId && 
                l.difficulty == difficulty &&
                l.expiresAt > DateTime.UtcNow);
        }

        public InstanceLockout GetLockout(string dungeonId, DifficultyTier difficulty)
        {
            return playerLockouts.Find(l => 
                l.dungeonId == dungeonId && 
                l.difficulty == difficulty);
        }

        private void UpdateLockout(string dungeonId, DifficultyTier difficulty, string bossId)
        {
            var lockout = GetLockout(dungeonId, difficulty);
            if (lockout == null)
            {
                lockout = new InstanceLockout
                {
                    dungeonId = dungeonId,
                    difficulty = difficulty,
                    expiresAt = GetNextWeeklyReset(),
                    defeatedBossIds = new List<string>()
                };
                playerLockouts.Add(lockout);
            }

            if (!lockout.defeatedBossIds.Contains(bossId))
            {
                lockout.defeatedBossIds.Add(bossId);
            }
        }

        private void CleanExpiredLockouts()
        {
            playerLockouts.RemoveAll(l => l.expiresAt <= DateTime.UtcNow);
        }

        private DateTime GetNextWeeklyReset()
        {
            // Reset every Tuesday at 10:00 UTC (like WoW)
            var now = DateTime.UtcNow;
            int daysUntilTuesday = ((int)DayOfWeek.Tuesday - (int)now.DayOfWeek + 7) % 7;
            if (daysUntilTuesday == 0 && now.Hour >= 10) daysUntilTuesday = 7;
            return now.Date.AddDays(daysUntilTuesday).AddHours(10);
        }

        #endregion

        #region Queries

        public List<DungeonData> GetAvailableDungeons(int playerLevel)
        {
            return availableDungeons.FindAll(d => d.minLevel <= playerLevel);
        }

        public List<RaidData> GetAvailableRaids(int playerLevel)
        {
            return availableRaids.FindAll(r => r.minLevel <= playerLevel);
        }

        public DungeonData GetDungeonData(string dungeonId)
        {
            return availableDungeons.Find(d => d.dungeonId == dungeonId);
        }

        public RaidData GetRaidData(string raidId)
        {
            return availableRaids.Find(r => r.dungeonId == raidId);
        }

        #endregion
    }
}
