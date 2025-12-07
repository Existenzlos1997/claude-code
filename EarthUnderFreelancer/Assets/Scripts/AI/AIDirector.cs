using UnityEngine;
using System;
using System.Collections.Generic;

namespace EarthUnderFreelancer.AI
{
    /// <summary>
    /// Advanced AI Director system that manages enemy spawning, difficulty scaling,
    /// and creates dynamic, engaging gameplay encounters
    /// </summary>
    public class AIDirector : MonoBehaviour
    {
        public static AIDirector Instance { get; private set; }

        [Header("Difficulty Settings")]
        [SerializeField] private float baseDifficulty = 1f;
        [SerializeField] private float difficultyScalePerLevel = 0.1f;
        [SerializeField] private float difficultyScalePerPlayer = 0.5f;
        [SerializeField] private float maxDifficulty = 10f;
        [SerializeField] private bool adaptiveDifficulty = true;

        [Header("Spawn Settings")]
        [SerializeField] private float minSpawnDistance = 500f;
        [SerializeField] private float maxSpawnDistance = 2000f;
        [SerializeField] private float spawnCooldown = 30f;
        [SerializeField] private int maxActiveEnemies = 50;
        [SerializeField] private int maxEnemiesPerPlayer = 5;

        [Header("Encounter Types")]
        [SerializeField] private float patrolEncounterChance = 0.4f;
        [SerializeField] private float ambushEncounterChance = 0.2f;
        [SerializeField] private float bossEncounterChance = 0.05f;
        [SerializeField] private float tradeInterceptChance = 0.3f;

        [Header("Adaptive Parameters")]
        [SerializeField] private float performanceWindow = 300f; // 5 minutes
        [SerializeField] private float targetKillDeathRatio = 1.5f;
        [SerializeField] private float difficultyAdjustRate = 0.1f;

        // State
        private float currentDifficulty;
        private float lastSpawnTime;
        private int activeEnemyCount;
        private List<AIEncounter> activeEncounters = new List<AIEncounter>();
        private Queue<PerformanceRecord> performanceHistory = new Queue<PerformanceRecord>();

        // Player performance tracking
        private Dictionary<string, PlayerPerformance> playerPerformance = new Dictionary<string, PlayerPerformance>();

        // Events
        public event Action<AIEncounter> OnEncounterStarted;
        public event Action<AIEncounter> OnEncounterCompleted;
        public event Action<float> OnDifficultyChanged;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                currentDifficulty = baseDifficulty;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            UpdateEncounters();
            UpdateAdaptiveDifficulty();
            CheckSpawnOpportunities();
        }

        private void UpdateEncounters()
        {
            for (int i = activeEncounters.Count - 1; i >= 0; i--)
            {
                var encounter = activeEncounters[i];
                encounter.Update();

                if (encounter.IsCompleted)
                {
                    OnEncounterCompleted?.Invoke(encounter);
                    activeEnemyCount -= encounter.EnemyCount;
                    activeEncounters.RemoveAt(i);
                }
            }
        }

        private void UpdateAdaptiveDifficulty()
        {
            if (!adaptiveDifficulty) return;

            // Clean old records
            float cutoffTime = Time.time - performanceWindow;
            while (performanceHistory.Count > 0 && performanceHistory.Peek().timestamp < cutoffTime)
            {
                performanceHistory.Dequeue();
            }

            // Calculate average K/D ratio
            if (performanceHistory.Count < 10) return;

            float totalKills = 0;
            float totalDeaths = 0;
            foreach (var record in performanceHistory)
            {
                totalKills += record.kills;
                totalDeaths += record.deaths;
            }

            float kdRatio = totalDeaths > 0 ? totalKills / totalDeaths : totalKills;

            // Adjust difficulty
            float targetDifficulty;
            if (kdRatio > targetKillDeathRatio * 1.5f)
            {
                // Player too strong - increase difficulty
                targetDifficulty = currentDifficulty + difficultyAdjustRate;
            }
            else if (kdRatio < targetKillDeathRatio * 0.5f)
            {
                // Player struggling - decrease difficulty
                targetDifficulty = currentDifficulty - difficultyAdjustRate;
            }
            else
            {
                targetDifficulty = currentDifficulty;
            }

            targetDifficulty = Mathf.Clamp(targetDifficulty, 0.5f, maxDifficulty);

            if (Math.Abs(targetDifficulty - currentDifficulty) > 0.01f)
            {
                currentDifficulty = Mathf.Lerp(currentDifficulty, targetDifficulty, Time.deltaTime * 0.1f);
                OnDifficultyChanged?.Invoke(currentDifficulty);
            }
        }

        private void CheckSpawnOpportunities()
        {
            if (Time.time - lastSpawnTime < spawnCooldown) return;
            if (activeEnemyCount >= maxActiveEnemies) return;

            // Get active players
            var players = GetActivePlayers();
            if (players.Count == 0) return;

            // Check if we should spawn
            foreach (var player in players)
            {
                if (ShouldSpawnForPlayer(player))
                {
                    SpawnEncounterForPlayer(player);
                    lastSpawnTime = Time.time;
                    break; // One spawn per check
                }
            }
        }

        private bool ShouldSpawnForPlayer(Transform player)
        {
            // Count enemies near this player
            int nearbyEnemies = CountNearbyEnemies(player.position, maxSpawnDistance);
            
            if (nearbyEnemies >= maxEnemiesPerPlayer) return false;

            // Random chance based on difficulty and time since last encounter
            float spawnChance = 0.1f * currentDifficulty;
            return UnityEngine.Random.value < spawnChance * Time.deltaTime;
        }

        private void SpawnEncounterForPlayer(Transform player)
        {
            // Determine encounter type
            float roll = UnityEngine.Random.value;
            EncounterType type;

            if (roll < bossEncounterChance && currentDifficulty > 5f)
            {
                type = EncounterType.Boss;
            }
            else if (roll < bossEncounterChance + ambushEncounterChance)
            {
                type = EncounterType.Ambush;
            }
            else if (roll < bossEncounterChance + ambushEncounterChance + tradeInterceptChance)
            {
                type = EncounterType.TradeIntercept;
            }
            else
            {
                type = EncounterType.Patrol;
            }

            // Create encounter
            var encounter = CreateEncounter(type, player);
            if (encounter != null)
            {
                activeEncounters.Add(encounter);
                activeEnemyCount += encounter.EnemyCount;
                OnEncounterStarted?.Invoke(encounter);
            }
        }

        private AIEncounter CreateEncounter(EncounterType type, Transform player)
        {
            // Calculate spawn position
            Vector3 spawnDirection = UnityEngine.Random.onUnitSphere;
            spawnDirection.y = Mathf.Clamp(spawnDirection.y, -0.3f, 0.3f); // Prefer horizontal
            float spawnDistance = UnityEngine.Random.Range(minSpawnDistance, maxSpawnDistance);
            Vector3 spawnPosition = player.position + spawnDirection * spawnDistance;

            // Determine enemy count based on difficulty and type
            int enemyCount;
            float enemyDifficulty;

            switch (type)
            {
                case EncounterType.Patrol:
                    enemyCount = Mathf.RoundToInt(2 + currentDifficulty * 0.5f);
                    enemyDifficulty = currentDifficulty * 0.8f;
                    break;

                case EncounterType.Ambush:
                    enemyCount = Mathf.RoundToInt(3 + currentDifficulty * 0.7f);
                    enemyDifficulty = currentDifficulty;
                    break;

                case EncounterType.TradeIntercept:
                    enemyCount = Mathf.RoundToInt(2 + currentDifficulty * 0.3f);
                    enemyDifficulty = currentDifficulty * 0.6f;
                    break;

                case EncounterType.Boss:
                    enemyCount = 1;
                    enemyDifficulty = currentDifficulty * 2f;
                    break;

                default:
                    enemyCount = 2;
                    enemyDifficulty = currentDifficulty;
                    break;
            }

            return new AIEncounter(type, spawnPosition, enemyCount, enemyDifficulty, player);
        }

        private List<Transform> GetActivePlayers()
        {
            var players = new List<Transform>();
            var playerObjects = GameObject.FindGameObjectsWithTag("Player");
            foreach (var obj in playerObjects)
            {
                players.Add(obj.transform);
            }
            return players;
        }

        private int CountNearbyEnemies(Vector3 position, float radius)
        {
            int count = 0;
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var enemy in enemies)
            {
                if (Vector3.Distance(enemy.transform.position, position) < radius)
                {
                    count++;
                }
            }
            return count;
        }

        public void RecordKill(string playerId)
        {
            performanceHistory.Enqueue(new PerformanceRecord { timestamp = Time.time, kills = 1, deaths = 0 });

            if (!playerPerformance.ContainsKey(playerId))
            {
                playerPerformance[playerId] = new PlayerPerformance();
            }
            playerPerformance[playerId].kills++;
        }

        public void RecordDeath(string playerId)
        {
            performanceHistory.Enqueue(new PerformanceRecord { timestamp = Time.time, kills = 0, deaths = 1 });

            if (!playerPerformance.ContainsKey(playerId))
            {
                playerPerformance[playerId] = new PlayerPerformance();
            }
            playerPerformance[playerId].deaths++;
        }

        public float GetCurrentDifficulty() => currentDifficulty;

        public void SetBaseDifficulty(float difficulty)
        {
            baseDifficulty = difficulty;
            currentDifficulty = Mathf.Max(currentDifficulty, baseDifficulty);
        }

        public void ForceSpawnEncounter(EncounterType type, Vector3 position, int enemyCount = -1)
        {
            if (enemyCount < 0)
            {
                enemyCount = Mathf.RoundToInt(2 + currentDifficulty * 0.5f);
            }

            var encounter = new AIEncounter(type, position, enemyCount, currentDifficulty, null);
            activeEncounters.Add(encounter);
            activeEnemyCount += encounter.EnemyCount;
            OnEncounterStarted?.Invoke(encounter);
        }

        public AIDirectorStats GetStats()
        {
            return new AIDirectorStats
            {
                CurrentDifficulty = currentDifficulty,
                ActiveEnemies = activeEnemyCount,
                ActiveEncounters = activeEncounters.Count,
                TotalKills = GetTotalKills(),
                TotalDeaths = GetTotalDeaths()
            };
        }

        private int GetTotalKills()
        {
            int total = 0;
            foreach (var perf in playerPerformance.Values)
            {
                total += perf.kills;
            }
            return total;
        }

        private int GetTotalDeaths()
        {
            int total = 0;
            foreach (var perf in playerPerformance.Values)
            {
                total += perf.deaths;
            }
            return total;
        }
    }

    public enum EncounterType
    {
        Patrol,
        Ambush,
        TradeIntercept,
        Boss,
        DefenseEvent,
        RaidEvent
    }

    public class AIEncounter
    {
        public EncounterType Type { get; private set; }
        public Vector3 Position { get; private set; }
        public int EnemyCount { get; private set; }
        public float Difficulty { get; private set; }
        public Transform Target { get; private set; }
        public bool IsCompleted { get; private set; }
        public float StartTime { get; private set; }

        private int remainingEnemies;
        private List<GameObject> spawnedEnemies = new List<GameObject>();

        public AIEncounter(EncounterType type, Vector3 position, int enemyCount, float difficulty, Transform target)
        {
            Type = type;
            Position = position;
            EnemyCount = enemyCount;
            Difficulty = difficulty;
            Target = target;
            StartTime = Time.time;
            remainingEnemies = enemyCount;

            SpawnEnemies();
        }

        private void SpawnEnemies()
        {
            // In a real implementation, this would instantiate enemy prefabs
            // For now, we simulate the spawning
            for (int i = 0; i < EnemyCount; i++)
            {
                Vector3 offset = UnityEngine.Random.insideUnitSphere * 50f;
                Vector3 spawnPos = Position + offset;

                // Would instantiate enemy prefab here
                // var enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
                // spawnedEnemies.Add(enemy);
            }
        }

        public void Update()
        {
            // Check if all enemies are defeated
            int aliveCount = 0;
            for (int i = spawnedEnemies.Count - 1; i >= 0; i--)
            {
                if (spawnedEnemies[i] == null)
                {
                    spawnedEnemies.RemoveAt(i);
                }
                else
                {
                    aliveCount++;
                }
            }

            remainingEnemies = aliveCount;

            // Timeout after 10 minutes
            if (Time.time - StartTime > 600f)
            {
                IsCompleted = true;
            }

            // Complete if all enemies defeated
            if (remainingEnemies <= 0)
            {
                IsCompleted = true;
            }
        }

        public void EnemyDefeated()
        {
            remainingEnemies--;
        }
    }

    public struct PerformanceRecord
    {
        public float timestamp;
        public int kills;
        public int deaths;
    }

    public class PlayerPerformance
    {
        public int kills;
        public int deaths;
        public float damageDealt;
        public float damageTaken;
    }

    public struct AIDirectorStats
    {
        public float CurrentDifficulty;
        public int ActiveEnemies;
        public int ActiveEncounters;
        public int TotalKills;
        public int TotalDeaths;
    }
}
