using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.AI
{
    /// <summary>
    /// Manages AI spawning and population
    /// </summary>
    public class AISpawnManager : MonoBehaviour
    {
        public static AISpawnManager Instance { get; private set; }

        [Header("Spawn Settings")]
        [SerializeField] private int maxActiveAI = 20;
        [SerializeField] private float spawnRadius = 1000f;
        [SerializeField] private float minSpawnDistance = 200f;
        [SerializeField] private float spawnInterval = 30f;

        [Header("AI Prefabs")]
        [SerializeField] private GameObject[] enemyPrefabs;
        [SerializeField] private GameObject[] friendlyPrefabs;

        [Header("Spawn Zones")]
        [SerializeField] private List<SpawnZone> spawnZones = new List<SpawnZone>();

        private List<AIShipController> activeAI = new List<AIShipController>();
        private Transform playerTransform;
        private float lastSpawnTime;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            FindPlayer();
        }

        private void Update()
        {
            CleanupDeadAI();

            if (Time.time - lastSpawnTime > spawnInterval)
            {
                TrySpawnAI();
                lastSpawnTime = Time.time;
            }
        }

        private void FindPlayer()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
        }

        private void CleanupDeadAI()
        {
            activeAI.RemoveAll(ai => ai == null || ai.IsDead);
        }

        private void TrySpawnAI()
        {
            if (activeAI.Count >= maxActiveAI) return;
            if (playerTransform == null)
            {
                FindPlayer();
                if (playerTransform == null) return;
            }

            // Find valid spawn position
            Vector3 spawnPos = GetValidSpawnPosition();
            if (spawnPos == Vector3.zero) return;

            // Select prefab
            if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;
            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

            // Spawn
            GameObject aiObj = Instantiate(prefab, spawnPos, Random.rotation);
            AIShipController ai = aiObj.GetComponent<AIShipController>();
            
            if (ai != null)
            {
                activeAI.Add(ai);
                ai.OnDestroyed += OnAIDestroyed;
            }
        }

        private Vector3 GetValidSpawnPosition()
        {
            for (int i = 0; i < 10; i++)
            {
                Vector3 randomDir = Random.onUnitSphere;
                float distance = Random.Range(minSpawnDistance, spawnRadius);
                Vector3 spawnPos = playerTransform.position + randomDir * distance;

                // Check if position is valid (not too close to player)
                if (Vector3.Distance(spawnPos, playerTransform.position) >= minSpawnDistance)
                {
                    return spawnPos;
                }
            }

            return Vector3.zero;
        }

        private void OnAIDestroyed(AIShipController ai)
        {
            activeAI.Remove(ai);
            ai.OnDestroyed -= OnAIDestroyed;
        }

        public void SpawnAtPosition(Vector3 position, string factionId = "Pirates", int count = 1)
        {
            if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;

            for (int i = 0; i < count; i++)
            {
                Vector3 offset = Random.insideUnitSphere * 50f;
                Vector3 spawnPos = position + offset;

                GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
                GameObject aiObj = Instantiate(prefab, spawnPos, Random.rotation);
                
                AIShipController ai = aiObj.GetComponent<AIShipController>();
                if (ai != null)
                {
                    activeAI.Add(ai);
                    ai.OnDestroyed += OnAIDestroyed;
                }
            }
        }

        public void SpawnInZone(SpawnZone zone)
        {
            if (zone.prefabs == null || zone.prefabs.Length == 0) return;

            for (int i = 0; i < zone.spawnCount; i++)
            {
                Vector3 spawnPos = zone.center + Random.insideUnitSphere * zone.radius;
                GameObject prefab = zone.prefabs[Random.Range(0, zone.prefabs.Length)];
                
                GameObject aiObj = Instantiate(prefab, spawnPos, Random.rotation);
                AIShipController ai = aiObj.GetComponent<AIShipController>();
                
                if (ai != null)
                {
                    activeAI.Add(ai);
                    ai.OnDestroyed += OnAIDestroyed;
                    ai.SetPatrolCenter(zone.center);
                }
            }
        }

        public List<AIShipController> GetActiveAI() => new List<AIShipController>(activeAI);

        public int GetActiveAICount() => activeAI.Count;

        public void DespawnAll()
        {
            foreach (var ai in activeAI)
            {
                if (ai != null)
                {
                    Destroy(ai.gameObject);
                }
            }
            activeAI.Clear();
        }
    }

    [System.Serializable]
    public class SpawnZone
    {
        public string zoneName;
        public Vector3 center;
        public float radius = 500f;
        public int spawnCount = 5;
        public GameObject[] prefabs;
        public string factionId;
    }
}
