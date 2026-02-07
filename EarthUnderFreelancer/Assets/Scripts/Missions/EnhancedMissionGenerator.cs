using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Missions
{
    public class EnhancedMissionGenerator : MonoBehaviour
    {
        private static EnhancedMissionGenerator instance;
        public static EnhancedMissionGenerator Instance => instance;
        
        [Header("Generation Settings")]
        public int playerLevel = 1;
        public Vector3 playerPosition;
        public float maxMissionDistance = 10000f;
        
        private string[] missionTypes = { "Patrol", "Escort", "Strike", "Recon", "Rescue", "Intercept", "Defense" };
        private string[] enemyTypes = { "Fighter", "Bomber", "Transport", "Ace" };
        
        private void Awake() { if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); } else { Destroy(gameObject); } }
        
        public MissionData GenerateRandomMission()
        {
            string missionType = missionTypes[Random.Range(0, missionTypes.Length)];
            Vector3 missionLocation = playerPosition + new Vector3(
                Random.Range(-maxMissionDistance, maxMissionDistance),
                Random.Range(500f, 5000f),
                Random.Range(-maxMissionDistance, maxMissionDistance)
            );
            
            MissionData mission = new MissionData
            {
                missionName = $"Dynamic {missionType} Mission",
                description = GenerateDescription(missionType),
                location = missionLocation,
                difficulty = DetermineDifficulty(),
                creditReward = playerLevel * 1000 + Random.Range(0, 500),
                experienceReward = playerLevel * 100 + Random.Range(0, 50),
                timeLimit = 600f
            };
            
            // Add objectives based on type
            mission.objectives = GenerateObjectives(missionType);
            
            return mission;
        }
        
        private string GenerateDescription(string missionType)
        {
            switch (missionType)
            {
                case "Patrol": return "Patrol the designated area and report enemy activity.";
                case "Escort": return "Escort friendly transport to destination safely.";
                case "Strike": return "Strike enemy position and eliminate all targets.";
                case "Recon": return "Perform reconnaissance of enemy territory.";
                case "Rescue": return "Rescue downed pilot from hostile area.";
                case "Intercept": return "Intercept incoming enemy bombers.";
                case "Defense": return "Defend allied base from enemy attack.";
                default: return "Complete the assigned mission.";
            }
        }
        
        private MissionDifficulty DetermineDifficulty()
        {
            float roll = Random.value;
            if (roll < 0.5f) return MissionDifficulty.Easy;
            if (roll < 0.8f) return MissionDifficulty.Normal;
            if (roll < 0.95f) return MissionDifficulty.Hard;
            return MissionDifficulty.Extreme;
        }
        
        private List<string> GenerateObjectives(string missionType)
        {
            List<string> objectives = new List<string>();
            
            switch (missionType)
            {
                case "Patrol":
                    objectives.Add("Patrol waypoint 1");
                    objectives.Add("Patrol waypoint 2");
                    objectives.Add("Patrol waypoint 3");
                    break;
                case "Strike":
                    int targets = Random.Range(3, 8);
                    for (int i = 0; i < targets; i++)
                        objectives.Add($"Destroy {enemyTypes[Random.Range(0, enemyTypes.Length)]} {i + 1}");
                    break;
                case "Escort":
                    objectives.Add("Protect transport");
                    objectives.Add("Reach destination");
                    break;
                default:
                    objectives.Add("Complete primary objective");
                    break;
            }
            
            return objectives;
        }
        
        public MissionData GenerateContextAwareMission()
        {
            // Consider player's current situation
            MissionData mission = GenerateRandomMission();
            
            // Scale difficulty based on player level
            if (playerLevel < 5)
                mission.difficulty = MissionDifficulty.Easy;
            else if (playerLevel < 15)
                mission.difficulty = MissionDifficulty.Normal;
            else if (playerLevel < 30)
                mission.difficulty = MissionDifficulty.Hard;
            else
                mission.difficulty = MissionDifficulty.Extreme;
            
            return mission;
        }
    }
}
