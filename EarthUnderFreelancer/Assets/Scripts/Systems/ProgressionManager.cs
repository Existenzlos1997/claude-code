using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Systems
{
    /// <summary>
    /// Player progression and XP system
    /// </summary>
    public class ProgressionManager : MonoBehaviour
    {
        public static ProgressionManager Instance { get; private set; }

        [Header("Player Level")]
        [SerializeField] private int currentLevel = 1;
        [SerializeField] private int currentXP = 0;
        [SerializeField] private int maxLevel = 50;

        [Header("XP Settings")]
        [SerializeField] private int baseXPRequired = 100;
        [SerializeField] private float xpScalingFactor = 1.5f;

        [Header("Skill Points")]
        [SerializeField] private int availableSkillPoints = 0;
        [SerializeField] private Dictionary<string, int> skills = new Dictionary<string, int>();

        public int CurrentLevel => currentLevel;
        public int CurrentXP => currentXP;
        public int XPToNextLevel => CalculateXPRequired(currentLevel);
        public int AvailableSkillPoints => availableSkillPoints;

        public event System.Action<int> OnLevelUp;
        public event System.Action<int> OnXPGained;
        public event System.Action<string, int> OnSkillUpgraded;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            InitializeSkills();
        }

        private void InitializeSkills()
        {
            // Combat skills
            skills["weapon_damage"] = 0;
            skills["fire_rate"] = 0;
            skills["missile_damage"] = 0;
            skills["targeting_range"] = 0;

            // Defense skills
            skills["shield_capacity"] = 0;
            skills["shield_regen"] = 0;
            skills["hull_strength"] = 0;
            skills["damage_resistance"] = 0;

            // Flight skills
            skills["max_speed"] = 0;
            skills["acceleration"] = 0;
            skills["maneuverability"] = 0;
            skills["boost_efficiency"] = 0;

            // Trading skills
            skills["buy_discount"] = 0;
            skills["sell_bonus"] = 0;
            skills["cargo_capacity"] = 0;
        }

        public int CalculateXPRequired(int level)
        {
            return Mathf.RoundToInt(baseXPRequired * Mathf.Pow(xpScalingFactor, level - 1));
        }

        public void AddXP(int amount)
        {
            currentXP += amount;
            OnXPGained?.Invoke(amount);

            // Check for level up
            while (currentXP >= XPToNextLevel && currentLevel < maxLevel)
            {
                currentXP -= XPToNextLevel;
                LevelUp();
            }
        }

        private void LevelUp()
        {
            currentLevel++;
            availableSkillPoints++;

            OnLevelUp?.Invoke(currentLevel);

            Debug.Log($"Level Up! Now level {currentLevel}");
        }

        public bool UpgradeSkill(string skillId)
        {
            if (availableSkillPoints <= 0) return false;
            if (!skills.ContainsKey(skillId)) return false;
            if (skills[skillId] >= 10) return false; // Max skill level

            skills[skillId]++;
            availableSkillPoints--;

            OnSkillUpgraded?.Invoke(skillId, skills[skillId]);
            ApplySkillEffect(skillId, skills[skillId]);

            return true;
        }

        public int GetSkillLevel(string skillId)
        {
            if (skills.TryGetValue(skillId, out int level))
                return level;
            return 0;
        }

        public float GetSkillBonus(string skillId)
        {
            int level = GetSkillLevel(skillId);
            return level * 0.05f; // 5% per level
        }

        private void ApplySkillEffect(string skillId, int level)
        {
            float bonus = level * 0.05f;

            switch (skillId)
            {
                case "cargo_capacity":
                    if (InventoryManager.Instance != null)
                    {
                        int extraCapacity = level * 10;
                        InventoryManager.Instance.SetCargoCapacity(100 + extraCapacity);
                    }
                    break;

                // Other skill effects would modify relevant systems
                // weapon_damage, shield_capacity, etc. would affect combat
            }
        }

        public Dictionary<string, int> GetAllSkills()
        {
            return new Dictionary<string, int>(skills);
        }

        public void ResetSkills()
        {
            int totalPoints = 0;
            foreach (var skill in skills)
            {
                totalPoints += skill.Value;
            }

            InitializeSkills();
            availableSkillPoints += totalPoints;
        }

        public float GetXPProgress()
        {
            return (float)currentXP / XPToNextLevel;
        }

        public void SetLevel(int level, int xp = 0)
        {
            currentLevel = Mathf.Clamp(level, 1, maxLevel);
            currentXP = xp;
        }
    }
}
