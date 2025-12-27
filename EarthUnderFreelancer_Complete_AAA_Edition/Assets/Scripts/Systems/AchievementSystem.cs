using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Systems
{
    /// <summary>
    /// Achievement tracking and unlock system
    /// Iteration 10: Achievement system for player progression
    /// </summary>
    public class AchievementSystem : MonoBehaviour
    {
        [System.Serializable]
        public class Achievement
        {
            public string id;
            public string name;
            public string description;
            public int pointsReward;
            public bool isUnlocked;
            public float progress;
            public float requirement;
            public AchievementType type;
        }
        
        public enum AchievementType
        {
            MissionsCompleted,
            EnemiesDestroyed,
            DistanceFlown,
            TimeInAir,
            PerfectMissions,
            FormationKills,
            AceKills,
            SurvivalTime
        }
        
        private List<Achievement> achievements = new List<Achievement>();
        private int totalPoints = 0;
        
        public static AchievementSystem Instance { get; private set; }
        
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
            achievements.Add(new Achievement
            {
                id = "first_mission",
                name = "First Steps",
                description = "Complete your first mission",
                pointsReward = 10,
                requirement = 1,
                type = AchievementType.MissionsCompleted
            });
            
            achievements.Add(new Achievement
            {
                id = "ace_pilot",
                name = "Ace Pilot",
                description = "Destroy 5 enemies in one mission",
                pointsReward = 25,
                requirement = 5,
                type = AchievementType.EnemiesDestroyed
            });
            
            achievements.Add(new Achievement
            {
                id = "marathon_flight",
                name = "Marathon Flight",
                description = "Fly for 60 minutes continuously",
                pointsReward = 50,
                requirement = 3600,
                type = AchievementType.TimeInAir
            });
        }
        
        public void UpdateProgress(AchievementType type, float amount)
        {
            foreach (Achievement achievement in achievements)
            {
                if (achievement.type == type && !achievement.isUnlocked)
                {
                    achievement.progress += amount;
                    if (achievement.progress >= achievement.requirement)
                    {
                        UnlockAchievement(achievement);
                    }
                }
            }
        }
        
        private void UnlockAchievement(Achievement achievement)
        {
            achievement.isUnlocked = true;
            totalPoints += achievement.pointsReward;
            Debug.Log($"🏆 Achievement Unlocked: {achievement.name}");
        }
        
        public List<Achievement> GetAllAchievements() => achievements;
        public int GetTotalPoints() => totalPoints;
    }
}
