using UnityEngine;

namespace EarthUnderFreelancer.Multiplayer
{
    // Iteration 22-23: Matchmaking and lobbies
    public class MatchmakingSystem : MonoBehaviour
    {
        public enum MatchType { QuickMatch, Ranked, Custom }
        
        public void FindMatch(MatchType type)
        {
            Debug.Log($"[Matchmaking] Searching for {type} match...");
        }
        
        public void CancelSearch()
        {
            Debug.Log("[Matchmaking] Search cancelled");
        }
    }
}
