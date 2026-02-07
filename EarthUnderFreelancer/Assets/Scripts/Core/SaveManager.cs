using UnityEngine;
using System.IO;

namespace EarthUnderFreelancer.Core
{
    /// <summary>
    /// Handles all save/load operations for the game
    /// </summary>
    public static class SaveManager
    {
        private const string PLAYER_DATA_KEY = "PlayerData";
        private const string GAME_SETTINGS_KEY = "GameSettings";
        private const string HIGHSCORE_KEY = "Highscores";

        private static string SavePath => Application.persistentDataPath;

        public static void SavePlayerData(PlayerData data)
        {
            string json = JsonUtility.ToJson(data, true);
            PlayerPrefs.SetString(PLAYER_DATA_KEY, json);
            PlayerPrefs.Save();

            string filePath = Path.Combine(SavePath, "playerdata.json");
            File.WriteAllText(filePath, json);
        }

        public static PlayerData LoadPlayerData()
        {
            if (PlayerPrefs.HasKey(PLAYER_DATA_KEY))
            {
                string json = PlayerPrefs.GetString(PLAYER_DATA_KEY);
                return JsonUtility.FromJson<PlayerData>(json);
            }

            string filePath = Path.Combine(SavePath, "playerdata.json");
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                PlayerData data = JsonUtility.FromJson<PlayerData>(json);
                PlayerPrefs.SetString(PLAYER_DATA_KEY, json);
                PlayerPrefs.Save();
                return data;
            }

            return null;
        }

        public static void SaveGameSettings(GameSettings settings)
        {
            string json = JsonUtility.ToJson(settings, true);
            PlayerPrefs.SetString(GAME_SETTINGS_KEY, json);
            PlayerPrefs.Save();

            string filePath = Path.Combine(SavePath, "settings.json");
            File.WriteAllText(filePath, json);
        }

        public static GameSettings LoadGameSettings()
        {
            if (PlayerPrefs.HasKey(GAME_SETTINGS_KEY))
            {
                string json = PlayerPrefs.GetString(GAME_SETTINGS_KEY);
                return JsonUtility.FromJson<GameSettings>(json);
            }

            string filePath = Path.Combine(SavePath, "settings.json");
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return JsonUtility.FromJson<GameSettings>(json);
            }

            return null;
        }

        public static void SaveHighscore(string missionId, int score)
        {
            string key = $"{HIGHSCORE_KEY}_{missionId}";
            int currentHighscore = PlayerPrefs.GetInt(key, 0);
            
            if (score > currentHighscore)
            {
                PlayerPrefs.SetInt(key, score);
                PlayerPrefs.Save();
            }
        }

        public static int GetHighscore(string missionId)
        {
            string key = $"{HIGHSCORE_KEY}_{missionId}";
            return PlayerPrefs.GetInt(key, 0);
        }

        public static void DeleteAllSaveData()
        {
            PlayerPrefs.DeleteAll();
            
            string playerDataPath = Path.Combine(SavePath, "playerdata.json");
            string settingsPath = Path.Combine(SavePath, "settings.json");
            
            if (File.Exists(playerDataPath))
                File.Delete(playerDataPath);
            if (File.Exists(settingsPath))
                File.Delete(settingsPath);
        }

        public static bool HasSaveData()
        {
            return PlayerPrefs.HasKey(PLAYER_DATA_KEY);
        }
    }
}
