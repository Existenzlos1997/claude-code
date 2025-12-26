using UnityEngine;
using EarthUnderFreelancer.Missions;
using EarthUnderFreelancer.Data;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Gameplay
{
    /// <summary>
    /// Demo mission manager - provides pre-configured demo missions for testing
    /// Implements first playable missions as per VERBESSERUNGSPLAN Phase 1
    /// </summary>
    public class DemoMissionManager : MonoBehaviour
    {
        [Header("Demo Mission Settings")]
        [SerializeField] private bool autoStartDemoMission = false;
        [SerializeField] private DemoMissionType selectedDemo = DemoMissionType.BasicFlight;
        
        [Header("Mission Objectives")]
        [SerializeField] private Transform[] waypoints;
        [SerializeField] private GameObject[] targetEnemies;
        
        public enum DemoMissionType
        {
            BasicFlight,        // Tutorial: Learn to fly
            CombatIntro,        // Tutorial: First combat
            PatrolMission,      // Mission: Patrol an area
            InterceptMission,   // Mission: Intercept enemies
            EscortMission       // Mission: Escort friendly
        }

        private MissionData currentMission;
        private int currentObjectiveIndex = 0;
        private bool missionActive = false;
        private bool missionComplete = false;

        private void Start()
        {
            if (autoStartDemoMission)
            {
                StartDemoMission(selectedDemo);
            }
        }

        /// <summary>
        /// Start a demo mission
        /// </summary>
        public void StartDemoMission(DemoMissionType missionType)
        {
            currentMission = CreateDemoMission(missionType);
            currentObjectiveIndex = 0;
            missionActive = true;
            missionComplete = false;

            Debug.Log($"[DemoMission] Starting: {currentMission.missionName}");
            Debug.Log($"[DemoMission] Objective: {GetCurrentObjective()}");

            // Show mission UI
            ShowMissionBriefing();
        }

        private MissionData CreateDemoMission(DemoMissionType missionType)
        {
            MissionData mission = ScriptableObject.CreateInstance<MissionData>();

            switch (missionType)
            {
                case DemoMissionType.BasicFlight:
                    mission.missionName = "Tutorial: Basic Flight";
                    mission.description = "Learn the basic flight controls. Fly through all checkpoints.";
                    mission.difficulty = MissionData.MissionDifficulty.Easy;
                    mission.rewardCredits = 100;
                    mission.rewardXP = 50;
                    break;

                case DemoMissionType.CombatIntro:
                    mission.missionName = "Tutorial: First Combat";
                    mission.description = "Learn combat basics. Destroy 3 training targets.";
                    mission.difficulty = MissionData.MissionDifficulty.Easy;
                    mission.rewardCredits = 250;
                    mission.rewardXP = 100;
                    break;

                case DemoMissionType.PatrolMission:
                    mission.missionName = "Mission: Patrol Sector Alpha";
                    mission.description = "Patrol the designated area and report any enemy activity.";
                    mission.difficulty = MissionData.MissionDifficulty.Medium;
                    mission.rewardCredits = 500;
                    mission.rewardXP = 200;
                    break;

                case DemoMissionType.InterceptMission:
                    mission.missionName = "Mission: Intercept Enemy Squadron";
                    mission.description = "Enemy fighters detected. Intercept and destroy them.";
                    mission.difficulty = MissionData.MissionDifficulty.Medium;
                    mission.rewardCredits = 750;
                    mission.rewardXP = 300;
                    break;

                case DemoMissionType.EscortMission:
                    mission.missionName = "Mission: Escort Transport";
                    mission.description = "Escort the friendly transport to its destination safely.";
                    mission.difficulty = MissionData.MissionDifficulty.Hard;
                    mission.rewardCredits = 1000;
                    mission.rewardXP = 400;
                    break;
            }

            return mission;
        }

        private string GetCurrentObjective()
        {
            if (currentMission == null) return "No active mission";

            switch (selectedDemo)
            {
                case DemoMissionType.BasicFlight:
                    return $"Fly through checkpoint {currentObjectiveIndex + 1}/{waypoints?.Length ?? 5}";
                
                case DemoMissionType.CombatIntro:
                    return $"Destroy training target {currentObjectiveIndex + 1}/3";
                
                case DemoMissionType.PatrolMission:
                    return $"Patrol waypoint {currentObjectiveIndex + 1}/{waypoints?.Length ?? 4}";
                
                case DemoMissionType.InterceptMission:
                    int enemiesRemaining = (targetEnemies?.Length ?? 5) - currentObjectiveIndex;
                    return $"Enemies remaining: {enemiesRemaining}";
                
                case DemoMissionType.EscortMission:
                    return $"Escort checkpoint {currentObjectiveIndex + 1}/{waypoints?.Length ?? 3} - Keep transport safe!";
                
                default:
                    return "Complete the mission";
            }
        }

        private void Update()
        {
            if (!missionActive || missionComplete) return;

            // Simple objective checking (placeholder - extend based on mission type)
            CheckObjectiveProgress();

            // Debug display
            if (Input.GetKeyDown(KeyCode.M))
            {
                Debug.Log($"[DemoMission] Status: {GetCurrentObjective()}");
            }
        }

        private void CheckObjectiveProgress()
        {
            // This is a simplified check - in real implementation, 
            // you'd check against actual game state
            
            // For demo purposes, objectives auto-complete after some time
            // or you can manually call CompleteCurrentObjective()
        }

        /// <summary>
        /// Manually complete current objective (for testing)
        /// </summary>
        public void CompleteCurrentObjective()
        {
            if (!missionActive || missionComplete) return;

            currentObjectiveIndex++;

            int totalObjectives = GetTotalObjectives();
            
            if (currentObjectiveIndex >= totalObjectives)
            {
                CompleteMission();
            }
            else
            {
                Debug.Log($"[DemoMission] Objective complete! Next: {GetCurrentObjective()}");
            }
        }

        private int GetTotalObjectives()
        {
            switch (selectedDemo)
            {
                case DemoMissionType.BasicFlight:
                    return waypoints?.Length ?? 5;
                case DemoMissionType.CombatIntro:
                    return 3;
                case DemoMissionType.PatrolMission:
                    return waypoints?.Length ?? 4;
                case DemoMissionType.InterceptMission:
                    return targetEnemies?.Length ?? 5;
                case DemoMissionType.EscortMission:
                    return waypoints?.Length ?? 3;
                default:
                    return 1;
            }
        }

        private void CompleteMission()
        {
            missionComplete = true;
            missionActive = false;

            Debug.Log($"[DemoMission] MISSION COMPLETE!");
            Debug.Log($"[DemoMission] Rewards: {currentMission.rewardCredits} credits, {currentMission.rewardXP} XP");

            ShowMissionComplete();
        }

        /// <summary>
        /// Fail the mission
        /// </summary>
        public void FailMission(string reason = "Mission failed")
        {
            missionActive = false;
            missionComplete = false;

            Debug.Log($"[DemoMission] MISSION FAILED: {reason}");
            
            ShowMissionFailed(reason);
        }

        private void ShowMissionBriefing()
        {
            // In real implementation, this would show UI
            Debug.Log("=== MISSION BRIEFING ===");
            Debug.Log($"Mission: {currentMission.missionName}");
            Debug.Log($"Description: {currentMission.description}");
            Debug.Log($"Difficulty: {currentMission.difficulty}");
            Debug.Log($"Rewards: {currentMission.rewardCredits} credits, {currentMission.rewardXP} XP");
            Debug.Log("========================");
        }

        private void ShowMissionComplete()
        {
            // In real implementation, this would show UI with rewards
            Debug.Log("=== MISSION COMPLETE ===");
            Debug.Log($"Mission: {currentMission.missionName}");
            Debug.Log($"Status: SUCCESS");
            Debug.Log($"Rewards Earned: {currentMission.rewardCredits} credits, {currentMission.rewardXP} XP");
            Debug.Log("========================");
        }

        private void ShowMissionFailed(string reason)
        {
            // In real implementation, this would show UI
            Debug.Log("=== MISSION FAILED ===");
            Debug.Log($"Mission: {currentMission.missionName}");
            Debug.Log($"Reason: {reason}");
            Debug.Log("Try again?");
            Debug.Log("======================");
        }

        private void OnGUI()
        {
            if (!missionActive) return;

            // Simple on-screen mission display
            GUI.Box(new Rect(10, 10, 400, 120), "Mission Status");
            
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontSize = 14;
            labelStyle.normal.textColor = Color.white;

            GUI.Label(new Rect(20, 35, 380, 25), $"Mission: {currentMission.missionName}", labelStyle);
            GUI.Label(new Rect(20, 60, 380, 25), $"Objective: {GetCurrentObjective()}", labelStyle);
            GUI.Label(new Rect(20, 85, 380, 25), $"Progress: {currentObjectiveIndex}/{GetTotalObjectives()}", labelStyle);

            // Debug controls
            GUI.Label(new Rect(20, 105, 380, 20), "Press M for mission info, O to complete objective", GUI.skin.label);
        }

        // Public API for mission control
        public bool IsMissionActive() => missionActive;
        public bool IsMissionComplete() => missionComplete;
        public MissionData GetCurrentMission() => currentMission;
        public int GetCurrentObjectiveIndex() => currentObjectiveIndex;
    }
}
