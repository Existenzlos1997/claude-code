using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Systems
{
    /// <summary>
    /// Manages faction relationships and reputation
    /// </summary>
    public class FactionManager : MonoBehaviour
    {
        public static FactionManager Instance { get; private set; }

        [SerializeField] private Dictionary<string, int> playerReputation = new Dictionary<string, int>();

        public event System.Action<string, int> OnReputationChanged;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            InitializeDefaultFactions();
        }

        private void InitializeDefaultFactions()
        {
            string[] defaultFactions = { "Federation", "Empire", "Pirates", "Freelancers", "Traders" };
            
            foreach (string faction in defaultFactions)
            {
                if (!playerReputation.ContainsKey(faction))
                {
                    int defaultRep = faction == "Freelancers" ? 50 : faction == "Pirates" ? -25 : 0;
                    playerReputation[faction] = defaultRep;
                }
            }
        }

        public int GetReputation(string factionId)
        {
            if (playerReputation.TryGetValue(factionId, out int rep))
                return rep;
            return 0;
        }

        public void ModifyReputation(string factionId, int amount)
        {
            if (!playerReputation.ContainsKey(factionId))
                playerReputation[factionId] = 0;

            playerReputation[factionId] = Mathf.Clamp(playerReputation[factionId] + amount, -100, 100);
            OnReputationChanged?.Invoke(factionId, playerReputation[factionId]);
        }

        public void SetReputation(string factionId, int value)
        {
            playerReputation[factionId] = Mathf.Clamp(value, -100, 100);
            OnReputationChanged?.Invoke(factionId, playerReputation[factionId]);
        }

        public RelationStatus GetRelationStatus(string factionId)
        {
            int rep = GetReputation(factionId);

            if (rep >= 80) return RelationStatus.Allied;
            if (rep >= 50) return RelationStatus.Friendly;
            if (rep >= 20) return RelationStatus.Neutral;
            if (rep >= -20) return RelationStatus.Neutral;
            if (rep >= -50) return RelationStatus.Unfriendly;
            return RelationStatus.Hostile;
        }

        public bool IsHostile(string factionId)
        {
            return GetReputation(factionId) < -50;
        }

        public bool IsFriendly(string factionId)
        {
            return GetReputation(factionId) >= 50;
        }

        public bool CanDock(string factionId)
        {
            return GetReputation(factionId) > -75;
        }

        public bool CanTrade(string factionId)
        {
            return GetReputation(factionId) > -50;
        }

        public bool CanAcceptMissions(string factionId)
        {
            return GetReputation(factionId) >= -20;
        }

        public float GetPriceModifier(string factionId)
        {
            int rep = GetReputation(factionId);
            return 1f - (rep * 0.002f);
        }

        public void OnEnemyKilled(string enemyFactionId)
        {
            ModifyReputation(enemyFactionId, -5);

            if (enemyFactionId == "Pirates")
            {
                ModifyReputation("Traders", 2);
                ModifyReputation("Federation", 1);
            }
            else if (enemyFactionId == "Federation" || enemyFactionId == "Empire")
            {
                ModifyReputation("Pirates", 2);
            }
        }

        public void OnMissionCompleted(string factionId, int reputationReward)
        {
            ModifyReputation(factionId, reputationReward);
        }

        public void OnMissionFailed(string factionId, int reputationPenalty)
        {
            ModifyReputation(factionId, -reputationPenalty);
        }

        public Dictionary<string, int> GetAllReputations()
        {
            return new Dictionary<string, int>(playerReputation);
        }
    }

    public enum RelationStatus
    {
        Allied,
        Friendly,
        Neutral,
        Unfriendly,
        Hostile
    }
}
