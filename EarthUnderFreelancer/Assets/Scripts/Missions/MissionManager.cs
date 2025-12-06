using UnityEngine;
using System;
using System.Collections.Generic;
using EarthUnderFreelancer.Core;

namespace EarthUnderFreelancer.Missions
{
    /// <summary>
    /// Manages all mission-related functionality
    /// </summary>
    public class MissionManager : MonoBehaviour
    {
        public static MissionManager Instance { get; private set; }

        [Header("Mission Database")]
        [SerializeField] private List<MissionData> availableMissions = new List<MissionData>();
        [SerializeField] private MissionData currentMission;

        [Header("Mission State")]
        [SerializeField] private List<MissionObjective> activeObjectives = new List<MissionObjective>();
        [SerializeField] private int currentScore = 0;
        [SerializeField] private float missionTimer = 0f;
        [SerializeField] private bool missionActive = false;

        public MissionData CurrentMission => currentMission;
        public bool IsMissionActive => missionActive;
        public int CurrentScore => currentScore;
        public float MissionTimer => missionTimer;
        public List<MissionObjective> ActiveObjectives => activeObjectives;

        public event Action<MissionData> OnMissionStarted;
        public event Action<MissionData, bool> OnMissionEnded;
        public event Action<MissionObjective> OnObjectiveCompleted;
        public event Action<int> OnScoreChanged;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Update()
        {
            if (missionActive)
            {
                missionTimer += Time.deltaTime;
                CheckObjectives();
                CheckMissionCompletion();
            }
        }

        public void StartMission(MissionData mission)
        {
            currentMission = mission;
            missionActive = true;
            missionTimer = 0f;
            currentScore = 0;
            
            activeObjectives.Clear();
            foreach (var objData in mission.objectives)
            {
                activeObjectives.Add(new MissionObjective
                {
                    objectiveId = objData.objectiveId,
                    description = objData.description,
                    type = objData.type,
                    targetCount = objData.targetCount,
                    currentCount = 0,
                    isCompleted = false,
                    isOptional = objData.isOptional,
                    bonusScore = objData.bonusScore
                });
            }

            SpawnMissionEntities();

            OnMissionStarted?.Invoke(mission);
            EventManager.TriggerEvent(GameEvents.MISSION_STARTED, mission);
            GameManager.Instance?.SetGameState(GameManager.GameState.Playing);
        }

        private void SpawnMissionEntities()
        {
            if (currentMission.enemySpawns != null)
            {
                foreach (var spawn in currentMission.enemySpawns)
                {
                    if (spawn.enemyPrefab != null)
                    {
                        Instantiate(spawn.enemyPrefab, spawn.spawnPosition, Quaternion.Euler(spawn.spawnRotation));
                    }
                }
            }
        }

        private void CheckObjectives()
        {
            foreach (var objective in activeObjectives)
            {
                if (objective.isCompleted) continue;

                if (objective.type == ObjectiveType.Timer && missionTimer >= objective.targetCount)
                {
                    CompleteObjective(objective);
                }
            }
        }

        private void CheckMissionCompletion()
        {
            bool allRequiredComplete = true;
            foreach (var objective in activeObjectives)
            {
                if (!objective.isOptional && !objective.isCompleted)
                {
                    allRequiredComplete = false;
                    break;
                }
            }

            if (allRequiredComplete) CompleteMission(true);
            if (currentMission.timeLimit > 0 && missionTimer > currentMission.timeLimit) CompleteMission(false);
        }

        public void UpdateObjective(ObjectiveType type, string targetId = null, int amount = 1)
        {
            foreach (var objective in activeObjectives)
            {
                if (objective.isCompleted) continue;
                if (objective.type != type) continue;
                if (!string.IsNullOrEmpty(targetId) && objective.targetId != targetId) continue;

                objective.currentCount += amount;
                EventManager.TriggerEvent(GameEvents.OBJECTIVE_UPDATED, objective);

                if (objective.currentCount >= objective.targetCount) CompleteObjective(objective);
            }
        }

        private void CompleteObjective(MissionObjective objective)
        {
            objective.isCompleted = true;
            AddScore(objective.bonusScore);
            OnObjectiveCompleted?.Invoke(objective);
            EventManager.TriggerEvent(GameEvents.OBJECTIVE_COMPLETED, objective);
        }

        public void AddScore(int points)
        {
            currentScore += points;
            OnScoreChanged?.Invoke(currentScore);
        }

        public void CompleteMission(bool success)
        {
            missionActive = false;

            int creditsEarned = success ? currentMission.creditReward : 0;
            int experienceEarned = success ? currentMission.experienceReward : currentMission.experienceReward / 4;

            if (success)
            {
                foreach (var objective in activeObjectives)
                {
                    if (objective.isOptional && objective.isCompleted)
                    {
                        creditsEarned += objective.bonusScore * 10;
                    }
                }
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.playerData.credits += creditsEarned;
                GameManager.Instance.playerData.AddExperience(experienceEarned);
                
                if (success)
                {
                    GameManager.Instance.playerData.totalMissionsCompleted++;
                    if (currentScore > GameManager.Instance.playerData.highScore)
                    {
                        GameManager.Instance.playerData.highScore = currentScore;
                    }
                }
                GameManager.Instance.SaveGame();
            }

            SaveManager.SaveHighscore(currentMission.missionId, currentScore);

            var eventData = new MissionEventData
            {
                missionId = currentMission.missionId,
                missionName = currentMission.missionName,
                score = currentScore,
                creditsEarned = creditsEarned,
                experienceEarned = experienceEarned
            };

            OnMissionEnded?.Invoke(currentMission, success);
            
            if (success)
            {
                EventManager.TriggerEvent(GameEvents.MISSION_COMPLETED, eventData);
                GameManager.Instance?.SetGameState(GameManager.GameState.Victory);
            }
            else
            {
                EventManager.TriggerEvent(GameEvents.MISSION_FAILED, eventData);
                GameManager.Instance?.SetGameState(GameManager.GameState.GameOver);
            }
        }

        public void FailMission() => CompleteMission(false);

        public List<MissionData> GetAvailableMissions()
        {
            List<MissionData> available = new List<MissionData>();
            PlayerData playerData = GameManager.Instance?.playerData;

            foreach (var mission in availableMissions)
            {
                if (playerData != null && playerData.playerLevel < mission.levelRequired) continue;
                if (playerData != null && !playerData.unlockedMissions.Contains(mission.missionId)) continue;
                available.Add(mission);
            }

            return available;
        }

        public void UnlockMission(string missionId)
        {
            if (GameManager.Instance != null && !GameManager.Instance.playerData.unlockedMissions.Contains(missionId))
            {
                GameManager.Instance.playerData.unlockedMissions.Add(missionId);
                GameManager.Instance.SaveGame();
            }
        }
    }

    [Serializable]
    public class MissionObjective
    {
        public string objectiveId;
        public string description;
        public ObjectiveType type;
        public string targetId;
        public int targetCount;
        public int currentCount;
        public bool isCompleted;
        public bool isOptional;
        public int bonusScore;
    }

    public enum ObjectiveType { Kill, Destroy, Protect, Escort, Collect, Navigate, Survive, Timer, Interact }
}
