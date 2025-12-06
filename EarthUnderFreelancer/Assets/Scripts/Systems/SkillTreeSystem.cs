using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Systems
{
    /// <summary>
    /// Skill tree system for player progression
    /// </summary>
    public class SkillTreeSystem : MonoBehaviour
    {
        public static SkillTreeSystem Instance { get; private set; }

        [Header("Skill Points")]
        [SerializeField] private int availableSkillPoints = 0;
        [SerializeField] private int totalSkillPointsEarned = 0;

        [Header("Skill Trees")]
        [SerializeField] private List<SkillTree> skillTrees = new List<SkillTree>();
        [SerializeField] private List<string> unlockedSkills = new List<string>();

        public int AvailableSkillPoints => availableSkillPoints;
        public int TotalSkillPointsEarned => totalSkillPointsEarned;
        public List<SkillTree> SkillTrees => skillTrees;

        public event System.Action<Skill> OnSkillUnlocked;
        public event System.Action<int> OnSkillPointsChanged;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                InitializeSkillTrees();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeSkillTrees()
        {
            // Combat Skill Tree
            SkillTree combatTree = new SkillTree
            {
                treeId = "combat",
                treeName = "Combat",
                description = "Improve your combat effectiveness",
                icon = null
            };

            combatTree.skills.Add(new Skill
            {
                skillId = "combat_damage_1",
                skillName = "Weapon Training I",
                description = "+10% weapon damage",
                tier = 1,
                maxRank = 3,
                pointCost = 1,
                statModifiers = new List<SkillStatModifier>
                {
                    new SkillStatModifier { statType = SkillStatType.WeaponDamage, valuePerRank = 10f, isPercentage = true }
                }
            });

            combatTree.skills.Add(new Skill
            {
                skillId = "combat_firerate_1",
                skillName = "Rapid Fire I",
                description = "+5% fire rate",
                tier = 1,
                maxRank = 3,
                pointCost = 1,
                statModifiers = new List<SkillStatModifier>
                {
                    new SkillStatModifier { statType = SkillStatType.FireRate, valuePerRank = 5f, isPercentage = true }
                }
            });

            combatTree.skills.Add(new Skill
            {
                skillId = "combat_crit_1",
                skillName = "Critical Systems I",
                description = "+5% critical hit chance",
                tier = 2,
                maxRank = 3,
                pointCost = 2,
                prerequisites = new List<string> { "combat_damage_1" },
                statModifiers = new List<SkillStatModifier>
                {
                    new SkillStatModifier { statType = SkillStatType.CriticalChance, valuePerRank = 5f, isPercentage = false }
                }
            });

            combatTree.skills.Add(new Skill
            {
                skillId = "combat_missile_1",
                skillName = "Missile Expert I",
                description = "+15% missile damage, +10% tracking",
                tier = 2,
                maxRank = 3,
                pointCost = 2,
                prerequisites = new List<string> { "combat_firerate_1" },
                statModifiers = new List<SkillStatModifier>
                {
                    new SkillStatModifier { statType = SkillStatType.MissileDamage, valuePerRank = 15f, isPercentage = true },
                    new SkillStatModifier { statType = SkillStatType.MissileTracking, valuePerRank = 10f, isPercentage = true }
                }
            });

            combatTree.skills.Add(new Skill
            {
                skillId = "combat_master",
                skillName = "Combat Master",
                description = "+25% all damage, +10% fire rate",
                tier = 3,
                maxRank = 1,
                pointCost = 5,
                prerequisites = new List<string> { "combat_crit_1", "combat_missile_1" },
                statModifiers = new List<SkillStatModifier>
                {
                    new SkillStatModifier { statType = SkillStatType.WeaponDamage, valuePerRank = 25f, isPercentage = true },
                    new SkillStatModifier { statType = SkillStatType.FireRate, valuePerRank = 10f, isPercentage = true }
                }
            });

            skillTrees.Add(combatTree);

            // Defense Skill Tree
            SkillTree defenseTree = new SkillTree
            {
                treeId = "defense",
                treeName = "Defense",
                description = "Improve your survivability"
            };

            defenseTree.skills.Add(new Skill
            {
                skillId = "defense_hull_1",
                skillName = "Reinforced Hull I",
                description = "+10% hull integrity",
                tier = 1,
                maxRank = 3,
                pointCost = 1,
                statModifiers = new List<SkillStatModifier>
                {
                    new SkillStatModifier { statType = SkillStatType.HullStrength, valuePerRank = 10f, isPercentage = true }
                }
            });

            defenseTree.skills.Add(new Skill
            {
                skillId = "defense_shield_1",
                skillName = "Shield Boost I",
                description = "+10% shield capacity",
                tier = 1,
                maxRank = 3,
                pointCost = 1,
                statModifiers = new List<SkillStatModifier>
                {
                    new SkillStatModifier { statType = SkillStatType.ShieldCapacity, valuePerRank = 10f, isPercentage = true }
                }
            });

            defenseTree.skills.Add(new Skill
            {
                skillId = "defense_regen_1",
                skillName = "Shield Regeneration I",
                description = "+15% shield regen rate",
                tier = 2,
                maxRank = 3,
                pointCost = 2,
                prerequisites = new List<string> { "defense_shield_1" },
                statModifiers = new List<SkillStatModifier>
                {
                    new SkillStatModifier { statType = SkillStatType.ShieldRegen, valuePerRank = 15f, isPercentage = true }
                }
            });

            defenseTree.skills.Add(new Skill
            {
                skillId = "defense_armor_1",
                skillName = "Armor Plating I",
                description = "+5 damage reduction",
                tier = 2,
                maxRank = 3,
                pointCost = 2,
                prerequisites = new List<string> { "defense_hull_1" },
                statModifiers = new List<SkillStatModifier>
                {
                    new SkillStatModifier { statType = SkillStatType.DamageReduction, valuePerRank = 5f, isPercentage = false }
                }
            });

            defenseTree.skills.Add(new Skill
            {
                skillId = "defense_master",
                skillName = "Iron Fortress",
                description = "+30% hull and shields, +20% regen",
                tier = 3,
                maxRank = 1,
                pointCost = 5,
                prerequisites = new List<string> { "defense_regen_1", "defense_armor_1" },
                statModifiers = new List<SkillStatModifier>
                {
                    new SkillStatModifier { statType = SkillStatType.HullStrength, valuePerRank = 30f, isPercentage = true },
                    new SkillStatModifier { statType = SkillStatType.ShieldCapacity, valuePerRank = 30f, isPercentage = true },
                    new SkillStatModifier { statType = SkillStatType.ShieldRegen, valuePerRank = 20f, isPercentage = true }
                }
            });

            skillTrees.Add(defenseTree);

            // Piloting Skill Tree
            SkillTree pilotingTree = new SkillTree
            {
                treeId = "piloting",
                treeName = "Piloting",
                description = "Improve your flight capabilities"
            };

            pilotingTree.skills.Add(new Skill
            {
                skillId = "pilot_speed_1",
                skillName = "Afterburner I",
                description = "+5% max speed",
                tier = 1,
                maxRank = 3,
                pointCost = 1,
                statModifiers = new List<SkillStatModifier>
                {
                    new SkillStatModifier { statType = SkillStatType.MaxSpeed, valuePerRank = 5f, isPercentage = true }
                }
            });

            pilotingTree.skills.Add(new Skill
            {
                skillId = "pilot_maneuver_1",
                skillName = "Evasive Maneuvers I",
                description = "+10% turn rate",
                tier = 1,
                maxRank = 3,
                pointCost = 1,
                statModifiers = new List<SkillStatModifier>
                {
                    new SkillStatModifier { statType = SkillStatType.TurnRate, valuePerRank = 10f, isPercentage = true }
                }
            });

            pilotingTree.skills.Add(new Skill
            {
                skillId = "pilot_boost_1",
                skillName = "Boost Efficiency I",
                description = "+20% boost duration, -10% consumption",
                tier = 2,
                maxRank = 3,
                pointCost = 2,
                prerequisites = new List<string> { "pilot_speed_1" },
                statModifiers = new List<SkillStatModifier>
                {
                    new SkillStatModifier { statType = SkillStatType.BoostDuration, valuePerRank = 20f, isPercentage = true },
                    new SkillStatModifier { statType = SkillStatType.BoostEfficiency, valuePerRank = 10f, isPercentage = true }
                }
            });

            pilotingTree.skills.Add(new Skill
            {
                skillId = "pilot_evasion_1",
                skillName = "Evasion I",
                description = "+5% chance to evade attacks",
                tier = 2,
                maxRank = 3,
                pointCost = 2,
                prerequisites = new List<string> { "pilot_maneuver_1" },
                statModifiers = new List<SkillStatModifier>
                {
                    new SkillStatModifier { statType = SkillStatType.EvasionChance, valuePerRank = 5f, isPercentage = false }
                }
            });

            pilotingTree.skills.Add(new Skill
            {
                skillId = "pilot_master",
                skillName = "Ace Pilot",
                description = "+20% speed, +25% maneuverability, +10% evasion",
                tier = 3,
                maxRank = 1,
                pointCost = 5,
                prerequisites = new List<string> { "pilot_boost_1", "pilot_evasion_1" },
                statModifiers = new List<SkillStatModifier>
                {
                    new SkillStatModifier { statType = SkillStatType.MaxSpeed, valuePerRank = 20f, isPercentage = true },
                    new SkillStatModifier { statType = SkillStatType.TurnRate, valuePerRank = 25f, isPercentage = true },
                    new SkillStatModifier { statType = SkillStatType.EvasionChance, valuePerRank = 10f, isPercentage = false }
                }
            });

            skillTrees.Add(pilotingTree);

            // Trading Skill Tree
            SkillTree tradingTree = new SkillTree
            {
                treeId = "trading",
                treeName = "Trading",
                description = "Improve your trading capabilities"
            };

            tradingTree.skills.Add(new Skill
            {
                skillId = "trade_discount_1",
                skillName = "Haggling I",
                description = "-5% buy prices",
                tier = 1,
                maxRank = 3,
                pointCost = 1,
                statModifiers = new List<SkillStatModifier>
                {
                    new SkillStatModifier { statType = SkillStatType.BuyPriceReduction, valuePerRank = 5f, isPercentage = true }
                }
            });

            tradingTree.skills.Add(new Skill
            {
                skillId = "trade_profit_1",
                skillName = "Sales Expert I",
                description = "+5% sell prices",
                tier = 1,
                maxRank = 3,
                pointCost = 1,
                statModifiers = new List<SkillStatModifier>
                {
                    new SkillStatModifier { statType = SkillStatType.SellPriceBonus, valuePerRank = 5f, isPercentage = true }
                }
            });

            tradingTree.skills.Add(new Skill
            {
                skillId = "trade_cargo_1",
                skillName = "Cargo Expansion I",
                description = "+10% cargo capacity",
                tier = 2,
                maxRank = 3,
                pointCost = 2,
                prerequisites = new List<string> { "trade_discount_1" },
                statModifiers = new List<SkillStatModifier>
                {
                    new SkillStatModifier { statType = SkillStatType.CargoCapacity, valuePerRank = 10f, isPercentage = true }
                }
            });

            tradingTree.skills.Add(new Skill
            {
                skillId = "trade_reputation_1",
                skillName = "Diplomat I",
                description = "+10% reputation gain",
                tier = 2,
                maxRank = 3,
                pointCost = 2,
                prerequisites = new List<string> { "trade_profit_1" },
                statModifiers = new List<SkillStatModifier>
                {
                    new SkillStatModifier { statType = SkillStatType.ReputationGain, valuePerRank = 10f, isPercentage = true }
                }
            });

            tradingTree.skills.Add(new Skill
            {
                skillId = "trade_master",
                skillName = "Trade Baron",
                description = "-15% buy, +15% sell, +25% cargo",
                tier = 3,
                maxRank = 1,
                pointCost = 5,
                prerequisites = new List<string> { "trade_cargo_1", "trade_reputation_1" },
                statModifiers = new List<SkillStatModifier>
                {
                    new SkillStatModifier { statType = SkillStatType.BuyPriceReduction, valuePerRank = 15f, isPercentage = true },
                    new SkillStatModifier { statType = SkillStatType.SellPriceBonus, valuePerRank = 15f, isPercentage = true },
                    new SkillStatModifier { statType = SkillStatType.CargoCapacity, valuePerRank = 25f, isPercentage = true }
                }
            });

            skillTrees.Add(tradingTree);
        }

        public void AddSkillPoints(int points)
        {
            availableSkillPoints += points;
            totalSkillPointsEarned += points;
            OnSkillPointsChanged?.Invoke(availableSkillPoints);
        }

        public bool UnlockSkill(string skillId)
        {
            Skill skill = FindSkill(skillId);
            if (skill == null) return false;

            // Check if already maxed
            int currentRank = GetSkillRank(skillId);
            if (currentRank >= skill.maxRank) return false;

            // Check cost
            if (availableSkillPoints < skill.pointCost) return false;

            // Check prerequisites
            if (!ArePrerequisitesMet(skill)) return false;

            // Unlock skill
            availableSkillPoints -= skill.pointCost;
            unlockedSkills.Add(skillId);

            OnSkillUnlocked?.Invoke(skill);
            OnSkillPointsChanged?.Invoke(availableSkillPoints);

            // Apply stat modifiers
            ApplySkillModifiers(skill);

            return true;
        }

        public bool ArePrerequisitesMet(Skill skill)
        {
            if (skill.prerequisites == null || skill.prerequisites.Count == 0)
                return true;

            foreach (var prereq in skill.prerequisites)
            {
                Skill prereqSkill = FindSkill(prereq);
                if (prereqSkill == null) continue;

                int rank = GetSkillRank(prereq);
                if (rank < prereqSkill.maxRank)
                    return false;
            }

            return true;
        }

        public int GetSkillRank(string skillId)
        {
            int count = 0;
            foreach (var id in unlockedSkills)
            {
                if (id == skillId)
                    count++;
            }
            return count;
        }

        public bool IsSkillUnlocked(string skillId)
        {
            return unlockedSkills.Contains(skillId);
        }

        public bool IsSkillMaxed(string skillId)
        {
            Skill skill = FindSkill(skillId);
            if (skill == null) return false;
            return GetSkillRank(skillId) >= skill.maxRank;
        }

        private void ApplySkillModifiers(Skill skill)
        {
            // This would apply to the player's stats
            // Implementation depends on how stats are managed
            foreach (var modifier in skill.statModifiers)
            {
                Debug.Log($"Applying modifier: {modifier.statType} +{modifier.valuePerRank}{(modifier.isPercentage ? "%" : "")}");
            }
        }

        public float GetTotalStatBonus(SkillStatType statType)
        {
            float total = 0f;

            foreach (var tree in skillTrees)
            {
                foreach (var skill in tree.skills)
                {
                    int rank = GetSkillRank(skill.skillId);
                    if (rank > 0)
                    {
                        foreach (var mod in skill.statModifiers)
                        {
                            if (mod.statType == statType)
                            {
                                total += mod.valuePerRank * rank;
                            }
                        }
                    }
                }
            }

            return total;
        }

        private Skill FindSkill(string skillId)
        {
            foreach (var tree in skillTrees)
            {
                foreach (var skill in tree.skills)
                {
                    if (skill.skillId == skillId)
                        return skill;
                }
            }
            return null;
        }

        public SkillTree GetSkillTree(string treeId)
        {
            foreach (var tree in skillTrees)
            {
                if (tree.treeId == treeId)
                    return tree;
            }
            return null;
        }

        public void ResetSkills()
        {
            availableSkillPoints = totalSkillPointsEarned;
            unlockedSkills.Clear();
            OnSkillPointsChanged?.Invoke(availableSkillPoints);
        }

        public void SaveSkills()
        {
            string json = JsonUtility.ToJson(new SkillSaveData
            {
                available = availableSkillPoints,
                total = totalSkillPointsEarned,
                unlocked = unlockedSkills
            });
            PlayerPrefs.SetString("skills", json);
            PlayerPrefs.Save();
        }

        public void LoadSkills()
        {
            if (PlayerPrefs.HasKey("skills"))
            {
                string json = PlayerPrefs.GetString("skills");
                SkillSaveData data = JsonUtility.FromJson<SkillSaveData>(json);
                if (data != null)
                {
                    availableSkillPoints = data.available;
                    totalSkillPointsEarned = data.total;
                    unlockedSkills = data.unlocked ?? new List<string>();
                }
            }
        }
    }

    [System.Serializable]
    public class SkillTree
    {
        public string treeId;
        public string treeName;
        public string description;
        public Sprite icon;
        public List<Skill> skills = new List<Skill>();
    }

    [System.Serializable]
    public class Skill
    {
        public string skillId;
        public string skillName;
        [TextArea(1, 3)]
        public string description;
        public Sprite icon;
        
        public int tier;
        public int maxRank;
        public int pointCost;
        
        public List<string> prerequisites;
        public List<SkillStatModifier> statModifiers;
    }

    [System.Serializable]
    public class SkillStatModifier
    {
        public SkillStatType statType;
        public float valuePerRank;
        public bool isPercentage;
    }

    public enum SkillStatType
    {
        // Combat
        WeaponDamage,
        FireRate,
        CriticalChance,
        CriticalDamage,
        MissileDamage,
        MissileTracking,
        
        // Defense
        HullStrength,
        ShieldCapacity,
        ShieldRegen,
        DamageReduction,
        
        // Piloting
        MaxSpeed,
        Acceleration,
        TurnRate,
        BoostDuration,
        BoostEfficiency,
        EvasionChance,
        
        // Trading
        BuyPriceReduction,
        SellPriceBonus,
        CargoCapacity,
        ReputationGain,
        MissionReward
    }

    [System.Serializable]
    public class SkillSaveData
    {
        public int available;
        public int total;
        public List<string> unlocked;
    }
}
