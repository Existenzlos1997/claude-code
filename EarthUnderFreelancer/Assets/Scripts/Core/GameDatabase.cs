using UnityEngine;
using System;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Core
{
    /// <summary>
    /// Complete database/persistence layer for the MMORPG
    /// Handles saving/loading all player and world data
    /// </summary>
    
    [Serializable]
    public class PlayerSaveData
    {
        public string playerId;
        public string playerName;
        public string factionId;
        public int level;
        public int xp;
        public long credits;
        public int gems;
        public Vector3 lastPosition;
        public string lastAircraftId;
        public List<string> ownedAircraft;
        public List<string> inventory;
        public Dictionary<string, int> factionReputation;
        public Dictionary<string, int> skills;
        public List<string> completedQuests;
        public List<string> achievements;
        public float playTime;
        public DateTime lastLogin;
        public DateTime created;
    }
    
    [Serializable]
    public class WorldSaveData
    {
        public Dictionary<string, string> territoryOwnership;
        public Dictionary<string, float> territoryInfluence;
        public List<string> activeEvents;
        public Dictionary<string, float> marketPrices;
        public float worldTime;
        public int worldDay;
        public string currentSeason;
    }
    
    public class GameDatabase : MonoBehaviour
    {
        public static GameDatabase Instance { get; private set; }
        
        private Dictionary<string, PlayerSaveData> playerDatabase = new Dictionary<string, PlayerSaveData>();
        private WorldSaveData worldData = new WorldSaveData();
        
        public event Action<string> OnPlayerDataLoaded;
        public event Action<string> OnPlayerDataSaved;
        public event Action OnWorldDataLoaded;
        
        private void Awake()
        {
            if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
            else { Destroy(gameObject); }
        }
        
        public PlayerSaveData CreateNewPlayer(string playerId, string playerName, string factionId)
        {
            var data = new PlayerSaveData
            {
                playerId = playerId,
                playerName = playerName,
                factionId = factionId,
                level = 1,
                xp = 0,
                credits = 10000,
                gems = 100,
                lastPosition = Vector3.zero,
                ownedAircraft = new List<string> { "p51d_mustang" },
                inventory = new List<string>(),
                factionReputation = new Dictionary<string, int>(),
                skills = new Dictionary<string, int>(),
                completedQuests = new List<string>(),
                achievements = new List<string>(),
                playTime = 0f,
                lastLogin = DateTime.UtcNow,
                created = DateTime.UtcNow
            };
            
            playerDatabase[playerId] = data;
            SavePlayerData(playerId);
            return data;
        }
        
        public PlayerSaveData LoadPlayerData(string playerId)
        {
            string key = $"player_{playerId}";
            if (PlayerPrefs.HasKey(key))
            {
                string json = PlayerPrefs.GetString(key);
                var data = JsonUtility.FromJson<PlayerSaveData>(json);
                playerDatabase[playerId] = data;
                OnPlayerDataLoaded?.Invoke(playerId);
                return data;
            }
            return null;
        }
        
        public void SavePlayerData(string playerId)
        {
            if (!playerDatabase.ContainsKey(playerId)) return;
            
            var data = playerDatabase[playerId];
            data.lastLogin = DateTime.UtcNow;
            
            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString($"player_{playerId}", json);
            PlayerPrefs.Save();
            
            OnPlayerDataSaved?.Invoke(playerId);
        }
        
        public PlayerSaveData GetPlayerData(string playerId)
        {
            if (playerDatabase.ContainsKey(playerId))
                return playerDatabase[playerId];
            return LoadPlayerData(playerId);
        }
        
        public void UpdatePlayerPosition(string playerId, Vector3 position)
        {
            if (playerDatabase.ContainsKey(playerId))
                playerDatabase[playerId].lastPosition = position;
        }
        
        public void AddCredits(string playerId, long amount)
        {
            if (playerDatabase.ContainsKey(playerId))
                playerDatabase[playerId].credits += amount;
        }
        
        public bool SpendCredits(string playerId, long amount)
        {
            if (!playerDatabase.ContainsKey(playerId)) return false;
            if (playerDatabase[playerId].credits < amount) return false;
            playerDatabase[playerId].credits -= amount;
            return true;
        }
        
        public void AddXP(string playerId, int amount)
        {
            if (!playerDatabase.ContainsKey(playerId)) return;
            var data = playerDatabase[playerId];
            data.xp += amount;
            
            int xpForNextLevel = data.level * 1000;
            while (data.xp >= xpForNextLevel)
            {
                data.xp -= xpForNextLevel;
                data.level++;
                xpForNextLevel = data.level * 1000;
            }
        }
        
        public void AddAircraft(string playerId, string aircraftId)
        {
            if (!playerDatabase.ContainsKey(playerId)) return;
            if (!playerDatabase[playerId].ownedAircraft.Contains(aircraftId))
                playerDatabase[playerId].ownedAircraft.Add(aircraftId);
        }
        
        public void AddItem(string playerId, string itemId)
        {
            if (playerDatabase.ContainsKey(playerId))
                playerDatabase[playerId].inventory.Add(itemId);
        }
        
        public void RemoveItem(string playerId, string itemId)
        {
            if (playerDatabase.ContainsKey(playerId))
                playerDatabase[playerId].inventory.Remove(itemId);
        }
        
        public void SetFactionReputation(string playerId, string factionId, int reputation)
        {
            if (playerDatabase.ContainsKey(playerId))
                playerDatabase[playerId].factionReputation[factionId] = reputation;
        }
        
        public void CompleteQuest(string playerId, string questId)
        {
            if (!playerDatabase.ContainsKey(playerId)) return;
            if (!playerDatabase[playerId].completedQuests.Contains(questId))
                playerDatabase[playerId].completedQuests.Add(questId);
        }
        
        public void UnlockAchievement(string playerId, string achievementId)
        {
            if (!playerDatabase.ContainsKey(playerId)) return;
            if (!playerDatabase[playerId].achievements.Contains(achievementId))
                playerDatabase[playerId].achievements.Add(achievementId);
        }
        
        public void SaveWorldData()
        {
            string json = JsonUtility.ToJson(worldData);
            PlayerPrefs.SetString("world_data", json);
            PlayerPrefs.Save();
        }
        
        public void LoadWorldData()
        {
            if (PlayerPrefs.HasKey("world_data"))
            {
                string json = PlayerPrefs.GetString("world_data");
                worldData = JsonUtility.FromJson<WorldSaveData>(json);
                OnWorldDataLoaded?.Invoke();
            }
            else
            {
                InitializeWorldData();
            }
        }
        
        private void InitializeWorldData()
        {
            worldData = new WorldSaveData
            {
                territoryOwnership = new Dictionary<string, string>(),
                territoryInfluence = new Dictionary<string, float>(),
                activeEvents = new List<string>(),
                marketPrices = new Dictionary<string, float>(),
                worldTime = 12f,
                worldDay = 1,
                currentSeason = "Summer"
            };
        }
        
        public WorldSaveData GetWorldData() => worldData;
        
        public void SetTerritoryOwner(string territoryId, string factionId)
        {
            worldData.territoryOwnership[territoryId] = factionId;
        }
        
        public string GetTerritoryOwner(string territoryId)
        {
            return worldData.territoryOwnership.ContainsKey(territoryId) ? 
                   worldData.territoryOwnership[territoryId] : "neutral";
        }
        
        public void UpdatePlayTime(string playerId, float deltaTime)
        {
            if (playerDatabase.ContainsKey(playerId))
                playerDatabase[playerId].playTime += deltaTime;
        }
        
        public void SaveAll()
        {
            foreach (var playerId in playerDatabase.Keys)
                SavePlayerData(playerId);
            SaveWorldData();
        }
        
        private void OnApplicationQuit()
        {
            SaveAll();
        }
        
        private void OnApplicationPause(bool pause)
        {
            if (pause) SaveAll();
        }
    }
}
