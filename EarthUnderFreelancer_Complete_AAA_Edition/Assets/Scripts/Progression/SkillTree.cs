using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Progression
{
    // Iteration 28-29: Skill tree system
    public class SkillTree : MonoBehaviour
    {
        [System.Serializable]
        public class Skill
        {
            public string name;
            public int cost;
            public bool unlocked;
        }
        
        private List<Skill> skills = new List<Skill>();
        private int skillPoints = 0;
        
        private void Start()
        {
            skills.Add(new Skill { name = "Improved Aim", cost = 1 });
            skills.Add(new Skill { name = "Faster Reload", cost = 2 });
            skills.Add(new Skill { name = "Extra Armor", cost = 3 });
        }
        
        public bool UnlockSkill(int index)
        {
            if (index < skills.Count && skillPoints >= skills[index].cost)
            {
                skills[index].unlocked = true;
                skillPoints -= skills[index].cost;
                Debug.Log($"[Skills] Unlocked: {skills[index].name}");
                return true;
            }
            return false;
        }
        
        public void AddSkillPoints(int points) => skillPoints += points;
    }
}
