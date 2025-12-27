using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace EarthUnderFreelancer.Core
{
    /// <summary>
    /// Complete save/load system for game state
    /// </summary>
    public class SaveLoadManager : MonoBehaviour
    {
        public static SaveLoadManager Instance { get; private set; }

        [Header("Save Settings")]
        [SerializeField] private string saveFileName = "EarthUnderFreelancer_Save";
        [SerializeField] private int maxSaveSlots = 5;

        private string SavePath => Path.Combine(Application.persistentDataPath, "Saves");

        public event System.Action OnSaveCompleted;
        public event System.Action OnLoadCompleted;
        public event System.Action<string> OnSaveError;
        public event System.Action<string> OnLoadError;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                EnsureSaveDirectoryExists();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void EnsureSaveDirectoryExists()
        {
            if (!Directory.Exists(SavePath))
            {
                Directory.CreateDirectory(SavePath);
            }
        }

        public void SaveGame(int slot = 0)
        {
            try
            {
                GameSaveData saveData = new GameSaveData();

                // Gather all save data
                SavePlayerData(saveData);
                SaveEconomyData(saveData);
                SaveInventoryData(saveData);
                SaveProgressionData(saveData);
                SaveFactionData(saveData);
                SaveMissionData(saveData);
                SaveSettingsData(saveData);

                saveData.saveTimestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                saveData.gameVersion = Application.version;

                // Serialize to JSON
                string json = JsonUtility.ToJson(saveData, true);
                string filePath = GetSaveFilePath(slot);

                File.WriteAllText(filePath, json);

                Debug.Log($"Game saved to slot {slot}");
                OnSaveCompleted?.Invoke();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Save failed: {e.Message}");
                OnSaveError?.Invoke(e.Message);
            }
        }

        public void LoadGame(int slot = 0)
        {
            try
            {
                string filePath = GetSaveFilePath(slot);

                if (!File.Exists(filePath))
                {
                    OnLoadError?.Invoke("Save file not found");
                    return;
                }

                string json = File.ReadAllText(filePath);
                GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);

                // Restore all data
                LoadPlayerData(saveData);
                LoadEconomyData(saveData);
                LoadInventoryData(saveData);
                LoadProgressionData(saveData);
                LoadFactionData(saveData);
                LoadMissionData(saveData);
                LoadSettingsData(saveData);

                Debug.Log($"Game loaded from slot {slot}");
                OnLoadCompleted?.Invoke();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Load failed: {e.Message}");
                OnLoadError?.Invoke(e.Message);
            }
        }

        private string GetSaveFilePath(int slot)
        {
            return Path.Combine(SavePath, $"{saveFileName}_{slot}.json");
        }

        public bool SaveExists(int slot)
        {
            return File.Exists(GetSaveFilePath(slot));
        }

        public SaveSlotInfo GetSaveInfo(int slot)
        {
            string filePath = GetSaveFilePath(slot);

            if (!File.Exists(filePath))
            {
                return new SaveSlotInfo { isEmpty = true, slot = slot };
            }

            try
            {
                string json = File.ReadAllText(filePath);
                GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);

                return new SaveSlotInfo
                {
                    isEmpty = false,
                    slot = slot,
                    timestamp = saveData.saveTimestamp,
                    playerLevel = saveData.playerLevel,
                    credits = saveData.credits,
                    playTime = saveData.totalPlayTime
                };
            }
            catch
            {
                return new SaveSlotInfo { isEmpty = true, slot = slot };
            }
        }

        public void DeleteSave(int slot)
        {
            string filePath = GetSaveFilePath(slot);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        private void SavePlayerData(GameSaveData data)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                data.playerPosition = player.transform.position;
                data.playerRotation = player.transform.rotation.eulerAngles;

                var health = player.GetComponent<Vehicles.HealthSystem>();
                if (health != null)
                {
                    data.playerHealth = health.CurrentHealth;
                    data.playerShield = health.CurrentShield;
                }
            }
        }

        private void LoadPlayerData(GameSaveData data)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = data.playerPosition;
                player.transform.rotation = Quaternion.Euler(data.playerRotation);

                var health = player.GetComponent<Vehicles.HealthSystem>();
                if (health != null)
                {
                    health.SetHealth(data.playerHealth);
                    health.SetShield(data.playerShield);
                }
            }
        }

        private void SaveEconomyData(GameSaveData data)
        {
            if (Systems.EconomyManager.Instance != null)
            {
                data.credits = Systems.EconomyManager.Instance.PlayerCredits;
            }
        }

        private void LoadEconomyData(GameSaveData data)
        {
            if (Systems.EconomyManager.Instance != null)
            {
                Systems.EconomyManager.Instance.SetCredits(data.credits);
            }
        }

        private void SaveInventoryData(GameSaveData data)
        {
            if (Systems.InventoryManager.Instance != null)
            {
                data.inventoryItems = new List<SerializedInventoryItem>();
                foreach (var item in Systems.InventoryManager.Instance.CargoItems)
                {
                    data.inventoryItems.Add(new SerializedInventoryItem
                    {
                        itemId = item.itemId,
                        quantity = item.quantity
                    });
                }

                var loadout = Systems.InventoryManager.Instance.CurrentLoadout;
                data.equippedPrimaryWeapon = loadout.primaryWeapon;
                data.equippedSecondaryWeapon = loadout.secondaryWeapon;
                data.equippedShield = loadout.shield;
                data.equippedEngine = loadout.engine;
            }
        }

        private void LoadInventoryData(GameSaveData data)
        {
            if (Systems.InventoryManager.Instance != null)
            {
                Systems.InventoryManager.Instance.ClearInventory();

                if (data.inventoryItems != null)
                {
                    foreach (var item in data.inventoryItems)
                    {
                        Systems.InventoryManager.Instance.AddItem(item.itemId, item.quantity);
                    }
                }
            }
        }

        private void SaveProgressionData(GameSaveData data)
        {
            if (Systems.ProgressionManager.Instance != null)
            {
                data.playerLevel = Systems.ProgressionManager.Instance.CurrentLevel;
                data.playerXP = Systems.ProgressionManager.Instance.CurrentXP;
                data.skillPoints = Systems.ProgressionManager.Instance.AvailableSkillPoints;

                var skills = Systems.ProgressionManager.Instance.GetAllSkills();
                data.skills = new List<SerializedSkill>();
                foreach (var skill in skills)
                {
                    data.skills.Add(new SerializedSkill { skillId = skill.Key, level = skill.Value });
                }
            }
        }

        private void LoadProgressionData(GameSaveData data)
        {
            if (Systems.ProgressionManager.Instance != null)
            {
                Systems.ProgressionManager.Instance.SetLevel(data.playerLevel, data.playerXP);
            }
        }

        private void SaveFactionData(GameSaveData data)
        {
            if (Systems.FactionManager.Instance != null)
            {
                var reputations = Systems.FactionManager.Instance.GetAllReputations();
                data.factionReputations = new List<SerializedReputation>();
                foreach (var rep in reputations)
                {
                    data.factionReputations.Add(new SerializedReputation
                    {
                        factionId = rep.Key,
                        reputation = rep.Value
                    });
                }
            }
        }

        private void LoadFactionData(GameSaveData data)
        {
            if (Systems.FactionManager.Instance != null && data.factionReputations != null)
            {
                foreach (var rep in data.factionReputations)
                {
                    Systems.FactionManager.Instance.SetReputation(rep.factionId, rep.reputation);
                }
            }
        }

        private void SaveMissionData(GameSaveData data)
        {
            if (Systems.MissionSystem.Instance != null)
            {
                data.activeMissions = new List<string>();
                foreach (var mission in Systems.MissionSystem.Instance.ActiveMissions)
                {
                    data.activeMissions.Add(mission.missionId);
                }
            }
        }

        private void LoadMissionData(GameSaveData data)
        {
            // Missions would need to be restored from templates
        }

        private void SaveSettingsData(GameSaveData data)
        {
            data.musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
            data.sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            data.mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1f);
        }

        private void LoadSettingsData(GameSaveData data)
        {
            PlayerPrefs.SetFloat("MusicVolume", data.musicVolume);
            PlayerPrefs.SetFloat("SFXVolume", data.sfxVolume);
            PlayerPrefs.SetFloat("MouseSensitivity", data.mouseSensitivity);
            PlayerPrefs.Save();
        }

        public void QuickSave()
        {
            SaveGame(0);
        }

        public void QuickLoad()
        {
            LoadGame(0);
        }
    }

    [System.Serializable]
    public class GameSaveData
    {
        public string saveTimestamp;
        public string gameVersion;
        public float totalPlayTime;

        // Player
        public Vector3 playerPosition;
        public Vector3 playerRotation;
        public float playerHealth;
        public float playerShield;

        // Economy
        public int credits;

        // Inventory
        public List<SerializedInventoryItem> inventoryItems;
        public string equippedPrimaryWeapon;
        public string equippedSecondaryWeapon;
        public string equippedShield;
        public string equippedEngine;

        // Progression
        public int playerLevel;
        public int playerXP;
        public int skillPoints;
        public List<SerializedSkill> skills;

        // Factions
        public List<SerializedReputation> factionReputations;

        // Missions
        public List<string> activeMissions;
        public List<string> completedMissions;

        // Settings
        public float musicVolume;
        public float sfxVolume;
        public float mouseSensitivity;
    }

    [System.Serializable]
    public class SerializedInventoryItem
    {
        public string itemId;
        public int quantity;
    }

    [System.Serializable]
    public class SerializedSkill
    {
        public string skillId;
        public int level;
    }

    [System.Serializable]
    public class SerializedReputation
    {
        public string factionId;
        public int reputation;
    }

    public struct SaveSlotInfo
    {
        public bool isEmpty;
        public int slot;
        public string timestamp;
        public int playerLevel;
        public int credits;
        public float playTime;
    }
}
