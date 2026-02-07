using UnityEngine;

namespace EarthUnderFreelancer.Progression
{
    // Iteration 30: Military rank progression
    public class RankSystem : MonoBehaviour
    {
        private string[] ranks = { "Recruit", "Private", "Corporal", "Sergeant", "Lieutenant", "Captain", "Major", "Colonel", "General" };
        private int currentRank = 0;
        
        public void PromoteRank()
        {
            if (currentRank < ranks.Length - 1)
            {
                currentRank++;
                Debug.Log($"[Rank] Promoted to {ranks[currentRank]}!");
            }
        }
        
        public string GetRank() => ranks[currentRank];
    }
}
