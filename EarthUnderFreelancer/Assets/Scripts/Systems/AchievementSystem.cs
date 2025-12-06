using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Systems
{
    /// <summary>
    /// Achievement tracking and unlocking system
    /// </summary>
    public class AchievementSystem : MonoBehaviour
    {
        public static AchievementSystem Instance { get; private set; }

        [Header("Achievements")]
        [SerializeField] private List<Achievement> allAchievements = new List<Achievement>();
        [SerializeField] private List<string> unlockedAchievements = new List<string>();

        [Header("Statistics")]
        [SerializeField] private PlayerStatistics statistics = new PlayerStatistics();

        public List<Achievement> AllAchievements => allAchievements;
        public List<string> UnlockedAchievements => unlockedAchievements;
        public PlayerStatistics Statistics => statistics;

        public event System.Action<Achievement> OnAchievementUnlocked;
        public event System.Action<string, int> OnStatisticUpdated;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeAchievements();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeAchievements()
        {
            // Combat achievements
            allAchievements.Add(new Achievement
            {
                achievementId = "first_blood",
                title = "First Blood",
                description = "Destroy your first enemy",
                icon = null,
                category = AchievementCategory.Combat,
                rewardCredits = 100,
                rewardXP = 50,
                requirement = 1,
                statisticKey = "enemies_killed"
            });

            allAchievements.Add(new Achievement
            {
                achievementId = "ace_pilot",
                title = "Ace Pilot",
                description = "Destroy 50 enemies",
                icon = null,
                category = AchievementCategory.Combat,
                rewardCredits = 500,
                rewardXP = 250,
                requirement = 50,
                statisticKey = "enemies_killed"
            });

            allAchievements.Add(new Achievement
            {
                achievementId = "legendary_warrior",
                title = "Legendary Warrior",
                description = "Destroy 500 enemies",
                icon = null,
                category = AchievementCategory.Combat,
                rewardCredits = 5000,
                rewardXP = 2500,
                requirement = 500,
                statisticKey = "enemies_killed"
            });

            // Trading achievements
            allAchievements.Add(new Achievement
            {
                achievementId = "first_trade",
                title = "Trader",
                description = "Complete your first trade",
                icon = null,
                category = AchievementCategory.Trading,
                rewardCredits = 100,
                rewardXP = 50,
                requirement = 1,
                statisticKey = "trades_completed"
            });

            allAchievements.Add(new Achievement
            {
                achievementId = "merchant",
                title = "Merchant",
                description = "Complete 50 trades",
                icon = null,
                category = AchievementCategory.Trading,
                rewardCredits = 1000,
                rewardXP = 500,
                requirement = 50,
                statisticKey = "trades_completed"
            });

            allAchievements.Add(new Achievement
            {
                achievementId = "millionaire",
                title = "Millionaire",
                description = "Earn 1,000,000 credits total",
                icon = null,
                category = AchievementCategory.Trading,
                rewardCredits = 10000,
                rewardXP = 5000,
                requirement = 1000000,
                statisticKey = "credits_earned"
            });

            // Mission achievements
            allAchievements.Add(new Achievement
            {
                achievementId = "mission_complete",
                title = "Mission Accomplished",
                description = "Complete your first mission",
                icon = null,
                category = AchievementCategory.Missions,
                rewardCredits = 100,
                rewardXP = 50,
                requirement = 1,
                statisticKey = "missions_completed"
            });

            allAchievements.Add(new Achievement
            {
                achievementId = "veteran",
                title = "Veteran",
                description = "Complete 25 missions",
                icon = null,
                category = AchievementCategory.Missions,
                rewardCredits = 750,
                rewardXP = 375,
                requirement = 25,
                statisticKey = "missions_completed"
            });

            allAchievements.Add(new Achievement
            {
                achievementId = "elite",
                title = "Elite Freelancer",
                description = "Complete 100 missions",
                icon = null,
                category = AchievementCategory.Missions,
                rewardCredits = 3000,
                rewardXP = 1500,
                requirement = 100,
                statisticKey = "missions_completed"
            });

            // Exploration achievements
            allAchievements.Add(new Achievement
            {
                achievementId = "explorer",
                title = "Explorer",
                description = "Visit 10 different stations",
                icon = null,
                category = AchievementCategory.Exploration,
                rewardCredits = 500,
                rewardXP = 250,
                requirement = 10,
                statisticKey = "stations_visited"
            });

            allAchievements.Add(new Achievement
            {
                achievementId = "miner",
                title = "Miner",
                description = "Mine 100 asteroids",
                icon = null,
                category = AchievementCategory.Exploration,
                rewardCredits = 400,
                rewardXP = 200,
                requirement = 100,
                statisticKey = "asteroids_mined"
            });

            // Progression achievements
            allAchievements.Add(new Achievement
            {
                achievementId = "level_10",
                title = "Rising Star",
                description = "Reach level 10",
                icon = null,
                category = AchievementCategory.Progression,
                rewardCredits = 500,
                rewardXP = 100,
                requirement = 10,
                statisticKey = "player_level"
            });

            allAchievements.Add(new Achievement
            {
                achievementId = "level_25",
                title = "Experienced",
                description = "Reach level 25",
                icon = null,
                category = AchievementCategory.Progression,
                rewardCredits = 1500,
                rewardXP = 300,
                requirement = 25,
                statisticKey = "player_level"
            });

            allAchievements.Add(new Achievement
            {
                achievementId = "level_50",
                title = "Master",
                description = "Reach level 50",
                icon = null,
                category = AchievementCategory.Progression,
                rewardCredits = 5000,
                rewardXP = 1000,
                requirement = 50,
                statisticKey = "player_level"
            });

            // Special achievements
            allAchievements.Add(new Achievement
            {
                achievementId = "survivor",
                title = "Survivor",
                description = "Survive near-death (below 10% hull)",
                icon = null,
                category = AchievementCategory.Special,
                rewardCredits = 200,
                rewardXP = 100,
                requirement = 1,
                statisticKey = "near_death_escapes",
                isHidden = true
            });

            allAchievements.Add(new Achievement
            {
                achievementId = "speedster",
                title = "Speedster",
                description = "Reach maximum boost speed",
                icon = null,
                category = AchievementCategory.Special,
                rewardCredits = 150,
                rewardXP = 75,
                requirement = 1,
                statisticKey = "max_speed_reached",
                isHidden = true
            });
        }

        public void UpdateStatistic(string statKey, int value)
        {
            int currentValue = statistics.GetStatistic(statKey);
            int newValue = currentValue + value;
            statistics.SetStatistic(statKey, newValue);

            OnStatisticUpdated?.Invoke(statKey, newValue);

            // Check achievements
            CheckAchievementsForStat(statKey, newValue);
        }

        public void SetStatistic(string statKey, int value)
        {
            statistics.SetStatistic(statKey, value);
            OnStatisticUpdated?.Invoke(statKey, value);
            CheckAchievementsForStat(statKey, value);
        }

        private void CheckAchievementsForStat(string statKey, int value)
        {
            foreach (var achievement in allAchievements)
            {
                if (achievement.statisticKey == statKey && 
                    !unlockedAchievements.Contains(achievement.achievementId))
                {
                    if (value >= achievement.requirement)
                    {
                        UnlockAchievement(achievement);
                    }
                }
            }
        }

        public void UnlockAchievement(string achievementId)
        {
            Achievement achievement = GetAchievement(achievementId);
            if (achievement != null)
            {
                UnlockAchievement(achievement);
            }
        }

        private void UnlockAchievement(Achievement achievement)
        {
            if (unlockedAchievements.Contains(achievement.achievementId)) return;

            unlockedAchievements.Add(achievement.achievementId);
            achievement.isUnlocked = true;
            achievement.unlockTime = System.DateTime.Now;

            // Grant rewards
            if (EconomyManager.Instance != null)
            {
                EconomyManager.Instance.AddCredits(achievement.rewardCredits);
            }

            if (ProgressionManager.Instance != null)
            {
                ProgressionManager.Instance.AddExperience(achievement.rewardXP);
            }

            OnAchievementUnlocked?.Invoke(achievement);

            Debug.Log($"Achievement Unlocked: {achievement.title}");
        }

        public Achievement GetAchievement(string achievementId)
        {
            foreach (var achievement in allAchievements)
            {
                if (achievement.achievementId == achievementId)
                    return achievement;
            }
            return null;
        }

        public bool IsAchievementUnlocked(string achievementId)
        {
            return unlockedAchievements.Contains(achievementId);
        }

        public float GetAchievementProgress(string achievementId)
        {
            Achievement achievement = GetAchievement(achievementId);
            if (achievement == null) return 0f;

            int currentValue = statistics.GetStatistic(achievement.statisticKey);
            return Mathf.Clamp01((float)currentValue / achievement.requirement);
        }

        public List<Achievement> GetAchievementsByCategory(AchievementCategory category)
        {
            List<Achievement> result = new List<Achievement>();
            foreach (var achievement in allAchievements)
            {
                if (achievement.category == category)
                {
                    result.Add(achievement);
                }
            }
            return result;
        }

        public int GetUnlockedCount()
        {
            return unlockedAchievements.Count;
        }

        public int GetTotalCount()
        {
            return allAchievements.Count;
        }

        public void SaveAchievements()
        {
            string json = JsonUtility.ToJson(new AchievementSaveData
            {
                unlockedIds = unlockedAchievements,
                statistics = statistics
            });
            PlayerPrefs.SetString("achievements", json);
            PlayerPrefs.Save();
        }

        public void LoadAchievements()
        {
            if (PlayerPrefs.HasKey("achievements"))
            {
                string json = PlayerPrefs.GetString("achievements");
                AchievementSaveData data = JsonUtility.FromJson<AchievementSaveData>(json);
                if (data != null)
                {
                    unlockedAchievements = data.unlockedIds ?? new List<string>();
                    statistics = data.statistics ?? new PlayerStatistics();

                    // Mark achievements as unlocked
                    foreach (var id in unlockedAchievements)
                    {
                        Achievement ach = GetAchievement(id);
                        if (ach != null) ach.isUnlocked = true;
                    }
                }
            }
        }
    }

    [System.Serializable]
    public class Achievement
    {
        public string achievementId;
        public string title;
        public string description;
        public Sprite icon;
        public AchievementCategory category;
        public bool isHidden;
        public bool isUnlocked;
        public System.DateTime unlockTime;

        public int rewardCredits;
        public int rewardXP;
        public string rewardItemId;

        public int requirement;
        public string statisticKey;
    }

    public enum AchievementCategory
    {
        Combat,
        Trading,
        Missions,
        Exploration,
        Progression,
        Special
    }

    [System.Serializable]
    public class PlayerStatistics
    {
        public int enemiesKilled;
        public int missionsCompleted;
        public int tradesCompleted;
        public int creditsEarned;
        public int creditsSpent;
        public int stationsVisited;
        public int asteroidsMined;
        public int itemsCrafted;
        public int playerLevel;
        public int nearDeathEscapes;
        public int maxSpeedReached;
        public float totalPlayTime;
        public float totalDistanceTraveled;

        public int GetStatistic(string key)
        {
            switch (key)
            {
                case "enemies_killed": return enemiesKilled;
                case "missions_completed": return missionsCompleted;
                case "trades_completed": return tradesCompleted;
                case "credits_earned": return creditsEarned;
                case "credits_spent": return creditsSpent;
                case "stations_visited": return stationsVisited;
                case "asteroids_mined": return asteroidsMined;
                case "items_crafted": return itemsCrafted;
                case "player_level": return playerLevel;
                case "near_death_escapes": return nearDeathEscapes;
                case "max_speed_reached": return maxSpeedReached;
                default: return 0;
            }
        }

        public void SetStatistic(string key, int value)
        {
            switch (key)
            {
                case "enemies_killed": enemiesKilled = value; break;
                case "missions_completed": missionsCompleted = value; break;
                case "trades_completed": tradesCompleted = value; break;
                case "credits_earned": creditsEarned = value; break;
                case "credits_spent": creditsSpent = value; break;
                case "stations_visited": stationsVisited = value; break;
                case "asteroids_mined": asteroidsMined = value; break;
                case "items_crafted": itemsCrafted = value; break;
                case "player_level": playerLevel = value; break;
                case "near_death_escapes": nearDeathEscapes = value; break;
                case "max_speed_reached": maxSpeedReached = value; break;
            }
        }
    }

    [System.Serializable]
    public class AchievementSaveData
    {
        public List<string> unlockedIds;
        public PlayerStatistics statistics;
    }
}
