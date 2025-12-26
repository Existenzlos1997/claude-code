using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.MMO
{
    [System.Serializable]
    public class TerritoryData
    {
        public string territoryId;
        public string controllingFaction;
        public float controlPercentage = 0f;
        public Vector3 position;
    }
    
    [System.Serializable]
    public class WorldEventData
    {
        public string eventId;
        public string eventType;
        public Vector3 location;
        public float startTime;
        public float duration;
        public bool isActive;
    }
    
    public class PersistentWorldManager : MonoBehaviour
    {
        private static PersistentWorldManager instance;
        public static PersistentWorldManager Instance => instance;
        
        [Header("Territory Control")]
        public Dictionary<string, TerritoryData> territories = new Dictionary<string, TerritoryData>();
        
        [Header("World Events")]
        public List<WorldEventData> activeEvents = new List<WorldEventData>();
        public float worldBossSpawnInterval = 3600f; // 1 hour
        private float nextBossSpawn = 0f;
        
        [Header("Player Structures")]
        public Dictionary<string, GameObject> playerBases = new Dictionary<string, GameObject>();
        
        private void Awake() { if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); } else { Destroy(gameObject); } }
        
        private void Update()
        {
            UpdateWorldEvents();
            UpdateTerritories();
        }
        
        private void UpdateWorldEvents()
        {
            // Remove expired events
            activeEvents.RemoveAll(e => Time.time > e.startTime + e.duration);
            
            // Spawn world boss
            if (Time.time >= nextBossSpawn)
            {
                SpawnWorldBoss();
                nextBossSpawn = Time.time + worldBossSpawnInterval;
            }
        }
        
        private void UpdateTerritories()
        {
            foreach (var territory in territories.Values)
            {
                // Territory control decays over time without player presence
                territory.controlPercentage -= Time.deltaTime * 0.001f;
                territory.controlPercentage = Mathf.Clamp01(territory.controlPercentage);
            }
        }
        
        public void SpawnWorldBoss()
        {
            WorldEventData bossEvent = new WorldEventData
            {
                eventId = "WorldBoss_" + Time.time,
                eventType = "WorldBoss",
                location = new Vector3(Random.Range(-10000f, 10000f), 1000f, Random.Range(-10000f, 10000f)),
                startTime = Time.time,
                duration = 1800f, // 30 minutes
                isActive = true
            };
            activeEvents.Add(bossEvent);
            Debug.Log($"World Boss spawned at {bossEvent.location}");
        }
        
        public void CaptureTerritory(string territoryId, string faction)
        {
            if (territories.TryGetValue(territoryId, out TerritoryData territory))
            {
                territory.controllingFaction = faction;
                territory.controlPercentage = 1f;
            }
        }
        
        public void RegisterPlayerBase(string playerId, GameObject baseObject)
        {
            playerBases[playerId] = baseObject;
        }
    }
}
