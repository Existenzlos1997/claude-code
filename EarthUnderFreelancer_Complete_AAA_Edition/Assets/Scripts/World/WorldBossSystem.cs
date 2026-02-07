using UnityEngine;
using System;
using System.Collections.Generic;

namespace EarthUnderFreelancer.World
{
    /// <summary>
    /// World Boss System for large-scale open world events
    /// </summary>
    public class WorldBossSystem : MonoBehaviour
    {
        public static WorldBossSystem Instance { get; private set; }

        [Header("Configuration")]
        [SerializeField] private List<WorldBossData> worldBosses;
        [SerializeField] private float minimumSpawnInterval = 3600f; // 1 hour
        [SerializeField] private float warningTimeSeconds = 300f; // 5 min warning

        public WorldBossData ActiveBoss { get; private set; }
        public bool IsBossActive => ActiveBoss != null && ActiveBoss.isActive;
        public float TimeUntilNextSpawn { get; private set; }

        public event Action<WorldBossData> OnBossSpawning;
        public event Action<WorldBossData> OnBossSpawned;
        public event Action<WorldBossData, float> OnBossHealthChanged;
        public event Action<WorldBossData> OnBossDefeated;
        public event Action<List<WorldBossReward>> OnRewardsDistributed;

        #region Data Structures

        [Serializable]
        public class WorldBossData
        {
            public string bossId;
            public string bossName;
            public string title;
            public string description;
            public BossType type;
            public int recommendedLevel;
            public int minimumPlayers;
            
            // Combat
            public float maxHealth;
            public float currentHealth;
            public float damage;
            public float armor;
            public List<BossAbility> abilities;
            
            // Spawn
            public Vector3 spawnLocation;
            public string zoneName;
            public float spawnRadius;
            public float enrageTimer;
            
            // Loot
            public List<string> guaranteedLoot;
            public List<string> rareLootTable;
            public int honorReward;
            public int experienceReward;
            
            // State
            public bool isActive;
            public DateTime spawnTime;
            public List<string> participantIds;
            public Dictionary<string, float> damageContribution;
        }

        public enum BossType
        {
            Titan,          // Massive ship/creature
            Fleet,          // Group of elite enemies
            Anomaly,        // Space phenomenon
            Invasion,       // Enemy faction attack
            Ancient         // Mysterious ancient being
        }

        [Serializable]
        public class BossAbility
        {
            public string abilityName;
            public string description;
            public float damage;
            public float cooldown;
            public float radius;
            public AbilityPhase phase;
            public bool isAreaDamage;
        }

        public enum AbilityPhase
        {
            All,
            Phase1,
            Phase2,
            Phase3,
            Enrage
        }

        [Serializable]
        public class WorldBossReward
        {
            public string playerId;
            public string playerName;
            public int rank;
            public float damageContribution;
            public List<string> items;
            public int honor;
            public int experience;
            public bool gotRareDrop;
        }

        #endregion

        private float nextSpawnTime;
        private float currentEnrageTimer;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeBosses();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeBosses()
        {
            worldBosses = new List<WorldBossData>
            {
                new WorldBossData
                {
                    bossId = "void_titan",
                    bossName = "Void Titan",
                    title = "The Endless Hunger",
                    description = "A massive entity from beyond known space. Consumes everything in its path.",
                    type = BossType.Titan,
                    recommendedLevel = 50,
                    minimumPlayers = 20,
                    maxHealth = 100000000,
                    damage = 5000,
                    armor = 500,
                    spawnLocation = new Vector3(10000, 0, 10000),
                    zoneName = "The Void Edge",
                    spawnRadius = 2000,
                    enrageTimer = 1800, // 30 min
                    abilities = new List<BossAbility>
                    {
                        new BossAbility { abilityName = "Void Beam", damage = 10000, cooldown = 30, radius = 100, isAreaDamage = true },
                        new BossAbility { abilityName = "Consume", damage = 50000, cooldown = 60, radius = 50 },
                        new BossAbility { abilityName = "Spawn Void Spawn", cooldown = 45 },
                        new BossAbility { abilityName = "Reality Tear", damage = 25000, cooldown = 90, radius = 500, phase = AbilityPhase.Phase2 }
                    },
                    honorReward = 1000,
                    experienceReward = 100000
                },
                new WorldBossData
                {
                    bossId = "pirate_armada",
                    bossName = "The Black Fleet",
                    title = "Scourge of the Sector",
                    description = "A massive pirate armada led by the infamous Admiral Darkstar.",
                    type = BossType.Fleet,
                    recommendedLevel = 35,
                    minimumPlayers = 15,
                    maxHealth = 50000000,
                    damage = 3000,
                    armor = 300,
                    spawnLocation = new Vector3(-5000, 0, 5000),
                    zoneName = "Pirate Nebula",
                    spawnRadius = 3000,
                    enrageTimer = 1200,
                    abilities = new List<BossAbility>
                    {
                        new BossAbility { abilityName = "Broadside Barrage", damage = 8000, cooldown = 20, radius = 200, isAreaDamage = true },
                        new BossAbility { abilityName = "Reinforce Fleet", cooldown = 60 },
                        new BossAbility { abilityName = "Boarding Party", damage = 15000, cooldown = 45 }
                    },
                    honorReward = 750,
                    experienceReward = 75000
                },
                new WorldBossData
                {
                    bossId = "quantum_storm",
                    bossName = "Quantum Storm",
                    title = "The Reality Breaker",
                    description = "An unstable quantum anomaly threatening to tear apart the sector.",
                    type = BossType.Anomaly,
                    recommendedLevel = 40,
                    minimumPlayers = 10,
                    maxHealth = 30000000,
                    damage = 4000,
                    spawnLocation = new Vector3(0, 0, -8000),
                    zoneName = "Quantum Rift",
                    spawnRadius = 1500,
                    enrageTimer = 900,
                    abilities = new List<BossAbility>
                    {
                        new BossAbility { abilityName = "Quantum Flux", damage = 5000, cooldown = 15, radius = 300, isAreaDamage = true },
                        new BossAbility { abilityName = "Reality Shift", cooldown = 30 },
                        new BossAbility { abilityName = "Time Dilation", cooldown = 60 }
                    },
                    honorReward = 600,
                    experienceReward = 60000
                }
            };

            ScheduleNextBoss();
        }

        private void Update()
        {
            if (!IsBossActive)
            {
                TimeUntilNextSpawn = nextSpawnTime - Time.time;
                
                // Check for spawn
                if (Time.time >= nextSpawnTime - warningTimeSeconds && Time.time < nextSpawnTime)
                {
                    // Warning phase
                    if (ActiveBoss == null)
                    {
                        SelectAndPrepareBoss();
                    }
                }
                else if (Time.time >= nextSpawnTime && ActiveBoss != null)
                {
                    SpawnBoss();
                }
            }
            else
            {
                // Update enrage timer
                currentEnrageTimer -= Time.deltaTime;
                if (currentEnrageTimer <= 0)
                {
                    EnrageBoss();
                }
            }
        }

        #region Boss Management

        private void ScheduleNextBoss()
        {
            nextSpawnTime = Time.time + minimumSpawnInterval + UnityEngine.Random.Range(0, 1800); // 0-30 min random
        }

        private void SelectAndPrepareBoss()
        {
            int index = UnityEngine.Random.Range(0, worldBosses.Count);
            ActiveBoss = worldBosses[index];
            ActiveBoss.participantIds = new List<string>();
            ActiveBoss.damageContribution = new Dictionary<string, float>();
            ActiveBoss.currentHealth = ActiveBoss.maxHealth;
            
            OnBossSpawning?.Invoke(ActiveBoss);
            Debug.Log($"[WorldBoss] {ActiveBoss.bossName} spawning in {warningTimeSeconds} seconds!");
        }

        private void SpawnBoss()
        {
            if (ActiveBoss == null) return;

            ActiveBoss.isActive = true;
            ActiveBoss.spawnTime = DateTime.UtcNow;
            currentEnrageTimer = ActiveBoss.enrageTimer;

            OnBossSpawned?.Invoke(ActiveBoss);
            Debug.Log($"[WorldBoss] {ActiveBoss.bossName} has spawned at {ActiveBoss.zoneName}!");
        }

        public void DealDamageToBoss(string playerId, float damage)
        {
            if (!IsBossActive) return;

            ActiveBoss.currentHealth -= damage;

            // Track contribution
            if (!ActiveBoss.participantIds.Contains(playerId))
            {
                ActiveBoss.participantIds.Add(playerId);
                ActiveBoss.damageContribution[playerId] = 0;
            }
            ActiveBoss.damageContribution[playerId] += damage;

            OnBossHealthChanged?.Invoke(ActiveBoss, ActiveBoss.currentHealth / ActiveBoss.maxHealth);

            if (ActiveBoss.currentHealth <= 0)
            {
                DefeatBoss();
            }
        }

        private void EnrageBoss()
        {
            if (!IsBossActive) return;

            // Increase damage, speed, etc.
            ActiveBoss.damage *= 2;
            Debug.Log($"[WorldBoss] {ActiveBoss.bossName} has enraged!");
        }

        private void DefeatBoss()
        {
            if (!IsBossActive) return;

            OnBossDefeated?.Invoke(ActiveBoss);
            
            // Distribute rewards
            var rewards = CalculateRewards();
            OnRewardsDistributed?.Invoke(rewards);

            Debug.Log($"[WorldBoss] {ActiveBoss.bossName} defeated! {ActiveBoss.participantIds.Count} participants rewarded.");

            ActiveBoss.isActive = false;
            ActiveBoss = null;
            ScheduleNextBoss();
        }

        private List<WorldBossReward> CalculateRewards()
        {
            var rewards = new List<WorldBossReward>();
            
            // Sort by damage contribution
            var sortedContributors = new List<KeyValuePair<string, float>>(ActiveBoss.damageContribution);
            sortedContributors.Sort((a, b) => b.Value.CompareTo(a.Value));

            float totalDamage = ActiveBoss.maxHealth;

            for (int i = 0; i < sortedContributors.Count; i++)
            {
                var contributor = sortedContributors[i];
                float contribution = contributor.Value / totalDamage * 100f;

                var reward = new WorldBossReward
                {
                    playerId = contributor.Key,
                    playerName = $"Player_{contributor.Key}",
                    rank = i + 1,
                    damageContribution = contribution,
                    items = new List<string>(),
                    honor = (int)(ActiveBoss.honorReward * (1f - i * 0.02f)),
                    experience = (int)(ActiveBoss.experienceReward * (1f - i * 0.01f))
                };

                // Top contributors get guaranteed loot
                if (i < 10 && ActiveBoss.guaranteedLoot != null)
                {
                    reward.items.Add(ActiveBoss.guaranteedLoot[UnityEngine.Random.Range(0, ActiveBoss.guaranteedLoot.Count)]);
                }

                // Rare loot chance
                float rareChance = 0.05f + (contribution * 0.01f);
                if (UnityEngine.Random.value < rareChance && ActiveBoss.rareLootTable != null && ActiveBoss.rareLootTable.Count > 0)
                {
                    reward.items.Add(ActiveBoss.rareLootTable[UnityEngine.Random.Range(0, ActiveBoss.rareLootTable.Count)]);
                    reward.gotRareDrop = true;
                }

                rewards.Add(reward);
            }

            return rewards;
        }

        #endregion

        #region Queries

        public List<WorldBossData> GetUpcomingBosses()
        {
            return worldBosses;
        }

        public WorldBossData GetBossData(string bossId)
        {
            return worldBosses.Find(b => b.bossId == bossId);
        }

        public float GetBossHealthPercent()
        {
            if (!IsBossActive) return 0;
            return ActiveBoss.currentHealth / ActiveBoss.maxHealth;
        }

        public int GetParticipantCount()
        {
            if (!IsBossActive) return 0;
            return ActiveBoss.participantIds.Count;
        }

        public float GetEnrageTimeRemaining()
        {
            return IsBossActive ? currentEnrageTimer : 0;
        }

        #endregion
    }
}
