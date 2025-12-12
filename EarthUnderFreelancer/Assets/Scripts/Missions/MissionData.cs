using UnityEngine;
using System;
using EarthUnderFreelancer.Core;

namespace EarthUnderFreelancer.Missions
{
    /// <summary>
    /// ScriptableObject containing mission data
    /// </summary>
    [CreateAssetMenu(fileName = "NewMission", menuName = "EarthUnder/Mission Data")]
    public class MissionData : ScriptableObject
    {
        [Header("Basic Info")]
        public string missionId;
        public string missionName;
        [TextArea(3, 6)]
        public string description;
        public Sprite missionIcon;
        public MissionType missionType;
        public MissionDifficulty difficulty;

        [Header("Requirements")]
        public int levelRequired = 1;
        public string[] requiredMissions;
        public int reputationRequired = 0;
        public string factionRequired;

        [Header("Rewards")]
        public int creditReward = 1000;
        public int experienceReward = 100;
        public ItemReward[] itemRewards;

        [Header("Settings")]
        public float timeLimit = 0f;
        public bool allowRespawn = false;
        public int maxRespawns = 3;
        public string sceneName;

        [Header("Objectives")]
        public ObjectiveData[] objectives;

        [Header("Spawns")]
        public SpawnData[] enemySpawns;
        public SpawnData[] allySpawns;

        [Header("Environment")]
        public string environmentPreset;
        public Vector3 playerSpawnPosition;
        public Vector3 playerSpawnRotation;

        [Header("Dialogue")]
        public DialogueLine[] introDialogue;
        public DialogueLine[] outroDialogue;

        [Header("Unlocks")]
        public string[] missionsToUnlock;
    }

    [Serializable]
    public class ObjectiveData
    {
        public string objectiveId;
        public string description;
        public ObjectiveType type;
        public string targetId;
        public int targetCount = 1;
        public bool isOptional = false;
        public int bonusScore = 100;
        public Vector3 location;
        public float radius = 10f;
    }

    [Serializable]
    public class SpawnData
    {
        public string spawnId;
        public GameObject enemyPrefab;
        public Vector3 spawnPosition;
        public Vector3 spawnRotation;
        public int count = 1;
        public float spawnDelay = 0f;
        public SpawnTrigger trigger;
        public string triggerId;
    }

    public enum SpawnTrigger { Immediate, OnObjectiveComplete, OnPlayerEnterArea, OnTimer, OnEnemiesDefeated }

    [Serializable]
    public class ItemReward { public string itemId; public int quantity = 1; }

    [Serializable]
    public class DialogueLine
    {
        public string speakerName;
        public Sprite speakerPortrait;
        [TextArea(2, 4)]
        public string text;
        public AudioClip voiceover;
        public float displayTime = 3f;
    }

    public enum MissionType { Story, Patrol, Escort, Bounty, Trading, Exploration, Defense, Raid, Tutorial }
    public enum MissionDifficulty { Easy, Normal, Hard, Expert, Nightmare }
}
