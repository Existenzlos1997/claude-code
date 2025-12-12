using UnityEngine;

namespace EarthUnderFreelancer.Data
{
    /// <summary>
    /// ScriptableObject defining mission templates
    /// </summary>
    [CreateAssetMenu(fileName = "NewMissionTemplate", menuName = "EarthUnder/Mission Template")]
    public class MissionTemplate : ScriptableObject
    {
        [Header("Identity")]
        public string missionId;
        public string missionName;
        [TextArea(3, 6)]
        public string description;
        public Sprite icon;
        public MissionType missionType;
        public MissionDifficulty difficulty;

        [Header("Requirements")]
        public int levelRequired = 1;
        public int reputationRequired = 0;
        public string factionRequired = "";

        [Header("Objectives")]
        public ObjectiveTemplate[] objectives;

        [Header("Rewards")]
        public int creditReward = 1000;
        public int experienceReward = 100;
        public int reputationReward = 10;
        public string rewardFaction = "";

        [Header("Settings")]
        public float timeLimit = 0f;
        public bool isRepeatable = true;
        public float missionRadius = 1000f;

        [Header("Enemy Spawns")]
        public EnemySpawn[] enemySpawns;
    }

    [System.Serializable]
    public class ObjectiveTemplate
    {
        public string objectiveId;
        [TextArea(1, 2)]
        public string description;
        public ObjectiveType type;
        public int targetCount = 1;
        public bool isOptional = false;
        public int bonusReward = 0;
    }

    [System.Serializable]
    public class EnemySpawn
    {
        public string enemyShipId;
        public int count = 1;
        public float spawnRadius = 100f;
        public SpawnTrigger trigger;
        public float delay = 0f;
    }

    public enum MissionType
    {
        Combat,
        Assassination,
        Patrol,
        Escort,
        Delivery,
        Mining,
        Salvage,
        Exploration,
        Defense
    }

    public enum MissionDifficulty
    {
        Easy,
        Normal,
        Hard,
        Expert,
        Nightmare
    }

    public enum ObjectiveType
    {
        Kill,
        KillAll,
        Destroy,
        Protect,
        Escort,
        Deliver,
        Collect,
        Navigate,
        Survive,
        Dock
    }

    public enum SpawnTrigger
    {
        Immediate,
        OnMissionStart,
        OnObjectiveComplete,
        OnPlayerEnterArea,
        OnTimer
    }
}
