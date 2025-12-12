using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Systems
{
    /// <summary>
    /// Mission generator and manager
    /// </summary>
    public class MissionSystem : MonoBehaviour
    {
        public static MissionSystem Instance { get; private set; }

        [Header("Active Missions")]
        [SerializeField] private List<ActiveMission> activeMissions = new List<ActiveMission>();
        [SerializeField] private int maxActiveMissions = 3;

        [Header("Available Missions")]
        [SerializeField] private List<GeneratedMission> availableMissions = new List<GeneratedMission>();
        [SerializeField] private int maxAvailableMissions = 10;

        [Header("Generation Settings")]
        [SerializeField] private float missionRefreshInterval = 300f;
        private float lastRefreshTime;

        public List<ActiveMission> ActiveMissions => activeMissions;
        public List<GeneratedMission> AvailableMissions => availableMissions;

        public event System.Action<ActiveMission> OnMissionAccepted;
        public event System.Action<ActiveMission> OnMissionCompleted;
        public event System.Action<ActiveMission> OnMissionFailed;
        public event System.Action<ActiveMission, int> OnObjectiveUpdated;
        public event System.Action OnMissionsRefreshed;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            GenerateRandomMissions();
        }

        private void Update()
        {
            UpdateActiveMissions();

            if (Time.time - lastRefreshTime > missionRefreshInterval)
            {
                RefreshAvailableMissions();
                lastRefreshTime = Time.time;
            }
        }

        private void UpdateActiveMissions()
        {
            for (int i = activeMissions.Count - 1; i >= 0; i--)
            {
                ActiveMission mission = activeMissions[i];
                
                if (mission.timeLimit > 0)
                {
                    mission.timeRemaining -= Time.deltaTime;
                    if (mission.timeRemaining <= 0)
                    {
                        FailMission(mission);
                        continue;
                    }
                }
            }
        }

        public void GenerateRandomMissions()
        {
            availableMissions.Clear();

            string[] factions = { "Federation", "Empire", "Traders", "Pirates", "Freelancers" };
            MissionType[] types = { MissionType.Combat, MissionType.Patrol, MissionType.Delivery, MissionType.Exploration };

            for (int i = 0; i < maxAvailableMissions; i++)
            {
                MissionType mType = types[Random.Range(0, types.Length)];
                MissionDifficulty difficulty = (MissionDifficulty)Random.Range(0, 5);
                
                GeneratedMission mission = new GeneratedMission
                {
                    missionId = System.Guid.NewGuid().ToString(),
                    missionName = GenerateMissionName(mType),
                    missionType = mType,
                    difficulty = difficulty,
                    factionId = factions[Random.Range(0, factions.Length)],
                    creditReward = Random.Range(500, 5000),
                    experienceReward = Random.Range(50, 500),
                    reputationReward = Random.Range(5, 20),
                    timeLimit = Random.value > 0.5f ? Random.Range(180f, 600f) : 0f
                };

                mission.objectives = GenerateObjectives(mission.missionType, mission.difficulty);
                mission.description = GenerateDescription(mission);

                availableMissions.Add(mission);
            }

            OnMissionsRefreshed?.Invoke();
        }

        private string GenerateMissionName(MissionType type)
        {
            string[] combatNames = { "Eliminate Hostiles", "Clear the Sector", "Bounty Hunt", "Pirate Purge" };
            string[] patrolNames = { "Sector Patrol", "Security Detail", "Perimeter Check", "Reconnaissance" };
            string[] deliveryNames = { "Urgent Delivery", "Cargo Transport", "Supply Run", "Priority Shipment" };
            string[] explorationNames = { "Survey Mission", "Unknown Signal", "Anomaly Investigation", "Deep Space Scan" };

            switch (type)
            {
                case MissionType.Combat: return combatNames[Random.Range(0, combatNames.Length)];
                case MissionType.Patrol: return patrolNames[Random.Range(0, patrolNames.Length)];
                case MissionType.Delivery: return deliveryNames[Random.Range(0, deliveryNames.Length)];
                case MissionType.Exploration: return explorationNames[Random.Range(0, explorationNames.Length)];
                default: return "Mission";
            }
        }

        private List<MissionObjective> GenerateObjectives(MissionType type, MissionDifficulty difficulty)
        {
            List<MissionObjective> objectives = new List<MissionObjective>();
            int difficultyMultiplier = (int)difficulty + 1;

            switch (type)
            {
                case MissionType.Combat:
                    objectives.Add(new MissionObjective
                    {
                        objectiveId = "kill_enemies",
                        description = $"Destroy {3 * difficultyMultiplier} enemy ships",
                        type = ObjectiveType.Kill,
                        targetCount = 3 * difficultyMultiplier,
                        currentCount = 0
                    });
                    break;

                case MissionType.Patrol:
                    objectives.Add(new MissionObjective
                    {
                        objectiveId = "visit_waypoints",
                        description = $"Visit {2 + difficultyMultiplier} waypoints",
                        type = ObjectiveType.Navigate,
                        targetCount = 2 + difficultyMultiplier,
                        currentCount = 0
                    });
                    break;

                case MissionType.Delivery:
                    objectives.Add(new MissionObjective
                    {
                        objectiveId = "deliver_cargo",
                        description = "Deliver cargo to destination",
                        type = ObjectiveType.Deliver,
                        targetCount = 1,
                        currentCount = 0
                    });
                    break;

                case MissionType.Exploration:
                    objectives.Add(new MissionObjective
                    {
                        objectiveId = "scan_area",
                        description = "Scan the designated area",
                        type = ObjectiveType.Scan,
                        targetCount = 1,
                        currentCount = 0
                    });
                    break;
            }

            return objectives;
        }

        private string GenerateDescription(GeneratedMission mission)
        {
            switch (mission.missionType)
            {
                case MissionType.Combat:
                    return $"Hostile forces have been spotted in the sector. Eliminate all threats to secure the area. Reward: {mission.creditReward} credits.";
                case MissionType.Patrol:
                    return $"We need you to patrol the sector and report any suspicious activity. Check all waypoints. Reward: {mission.creditReward} credits.";
                case MissionType.Delivery:
                    return $"Time-sensitive cargo needs to be delivered. Handle with care. Reward: {mission.creditReward} credits.";
                case MissionType.Exploration:
                    return $"Unknown signals detected in deep space. Investigate and report findings. Reward: {mission.creditReward} credits.";
                default:
                    return $"Complete the mission objectives. Reward: {mission.creditReward} credits.";
            }
        }

        public void RefreshAvailableMissions()
        {
            GenerateRandomMissions();
        }

        public bool AcceptMission(string missionId)
        {
            if (activeMissions.Count >= maxActiveMissions) return false;

            GeneratedMission generated = null;
            foreach (var m in availableMissions)
            {
                if (m.missionId == missionId)
                {
                    generated = m;
                    break;
                }
            }

            if (generated == null) return false;

            ActiveMission active = new ActiveMission
            {
                missionId = generated.missionId,
                missionName = generated.missionName,
                missionType = generated.missionType,
                difficulty = generated.difficulty,
                factionId = generated.factionId,
                creditReward = generated.creditReward,
                experienceReward = generated.experienceReward,
                reputationReward = generated.reputationReward,
                timeLimit = generated.timeLimit,
                timeRemaining = generated.timeLimit,
                objectives = new List<MissionObjective>(generated.objectives),
                startTime = Time.time
            };

            activeMissions.Add(active);
            availableMissions.Remove(generated);

            OnMissionAccepted?.Invoke(active);
            return true;
        }

        public void UpdateObjective(string missionId, string objectiveId, int progress = 1)
        {
            foreach (var mission in activeMissions)
            {
                if (mission.missionId == missionId)
                {
                    for (int i = 0; i < mission.objectives.Count; i++)
                    {
                        if (mission.objectives[i].objectiveId == objectiveId)
                        {
                            mission.objectives[i].currentCount += progress;
                            OnObjectiveUpdated?.Invoke(mission, i);

                            if (mission.objectives[i].currentCount >= mission.objectives[i].targetCount)
                            {
                                mission.objectives[i].isCompleted = true;
                            }

                            CheckMissionCompletion(mission);
                            return;
                        }
                    }
                }
            }
        }

        public void UpdateObjectiveByType(ObjectiveType type, int progress = 1)
        {
            foreach (var mission in activeMissions)
            {
                foreach (var objective in mission.objectives)
                {
                    if (objective.type == type && !objective.isCompleted)
                    {
                        objective.currentCount += progress;
                        if (objective.currentCount >= objective.targetCount)
                        {
                            objective.isCompleted = true;
                        }
                        CheckMissionCompletion(mission);
                    }
                }
            }
        }

        private void CheckMissionCompletion(ActiveMission mission)
        {
            bool allComplete = true;
            foreach (var obj in mission.objectives)
            {
                if (!obj.isOptional && !obj.isCompleted)
                {
                    allComplete = false;
                    break;
                }
            }

            if (allComplete)
            {
                CompleteMission(mission);
            }
        }

        public void CompleteMission(ActiveMission mission)
        {
            if (EconomyManager.Instance != null)
            {
                EconomyManager.Instance.AddCredits(mission.creditReward);
            }

            if (FactionManager.Instance != null)
            {
                FactionManager.Instance.ModifyReputation(mission.factionId, mission.reputationReward);
            }

            activeMissions.Remove(mission);
            OnMissionCompleted?.Invoke(mission);
        }

        public void FailMission(ActiveMission mission)
        {
            if (FactionManager.Instance != null)
            {
                FactionManager.Instance.ModifyReputation(mission.factionId, -5);
            }

            activeMissions.Remove(mission);
            OnMissionFailed?.Invoke(mission);
        }

        public void AbandonMission(string missionId)
        {
            for (int i = activeMissions.Count - 1; i >= 0; i--)
            {
                if (activeMissions[i].missionId == missionId)
                {
                    FailMission(activeMissions[i]);
                    return;
                }
            }
        }

        public ActiveMission GetActiveMission(string missionId)
        {
            foreach (var mission in activeMissions)
            {
                if (mission.missionId == missionId)
                    return mission;
            }
            return null;
        }
    }

    public enum MissionType
    {
        Combat,
        Patrol,
        Delivery,
        Exploration
    }

    public enum MissionDifficulty
    {
        Easy,
        Normal,
        Hard,
        VeryHard,
        Extreme
    }

    public enum ObjectiveType
    {
        Kill,
        Navigate,
        Deliver,
        Scan,
        Protect,
        Collect
    }

    [System.Serializable]
    public class GeneratedMission
    {
        public string missionId;
        public string missionName;
        public string description;
        public MissionType missionType;
        public MissionDifficulty difficulty;
        public string factionId;
        public int creditReward;
        public int experienceReward;
        public int reputationReward;
        public float timeLimit;
        public List<MissionObjective> objectives = new List<MissionObjective>();
    }

    [System.Serializable]
    public class ActiveMission : GeneratedMission
    {
        public float startTime;
        public float timeRemaining;
    }

    [System.Serializable]
    public class MissionObjective
    {
        public string objectiveId;
        public string description;
        public ObjectiveType type;
        public int targetCount;
        public int currentCount;
        public bool isCompleted;
        public bool isOptional;
    }
}
