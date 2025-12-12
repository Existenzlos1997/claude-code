using UnityEngine;

namespace EarthUnderFreelancer.Data
{
    /// <summary>
    /// ScriptableObject defining faction properties
    /// </summary>
    [CreateAssetMenu(fileName = "NewFactionData", menuName = "EarthUnder/Faction Data")]
    public class FactionData : ScriptableObject
    {
        [Header("Identity")]
        public string factionId;
        public string factionName;
        [TextArea(2, 4)]
        public string description;
        public Sprite icon;
        public Color factionColor = Color.white;
        public FactionType factionType;

        [Header("Behavior")]
        public bool isHostileByDefault = false;
        public float aggressionLevel = 0.5f;

        [Header("Relations")]
        public FactionRelation[] defaultRelations;

        [Header("Reputation Thresholds")]
        public int hostileThreshold = -50;
        public int unfriendlyThreshold = -20;
        public int neutralThreshold = 20;
        public int friendlyThreshold = 50;
        public int alliedThreshold = 80;
    }

    [System.Serializable]
    public class FactionRelation
    {
        public string factionId;
        public int baseReputation;
    }

    public enum FactionType
    {
        Government,
        Corporation,
        Military,
        Criminal,
        Pirate,
        Freelancer,
        Scientific
    }
}
