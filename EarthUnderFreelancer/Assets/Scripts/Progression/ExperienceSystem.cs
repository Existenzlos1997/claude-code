using UnityEngine;

namespace EarthUnderFreelancer.Progression
{
    // Iteration 26-27: XP and leveling system
    public class ExperienceSystem : MonoBehaviour
    {
        private int level = 1;
        private int currentXP = 0;
        private int xpToNextLevel = 1000;
        
        public void AddExperience(int amount)
        {
            currentXP += amount;
            
            while (currentXP >= xpToNextLevel)
            {
                LevelUp();
            }
        }
        
        private void LevelUp()
        {
            level++;
            currentXP -= xpToNextLevel;
            xpToNextLevel = (int)(xpToNextLevel * 1.5f);
            Debug.Log($"[XP] Level Up! Now level {level}");
        }
        
        public int GetLevel() => level;
        public int GetCurrentXP() => currentXP;
        public int GetXPToNext() => xpToNextLevel;
    }
}
