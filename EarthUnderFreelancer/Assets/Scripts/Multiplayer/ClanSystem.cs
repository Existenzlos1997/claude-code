using UnityEngine;

namespace EarthUnderFreelancer.Multiplayer
{
    // Iteration 24-25: Clan and guild system
    public class ClanSystem : MonoBehaviour
    {
        private string clanName = "";
        private int clanLevel = 1;
        private int clanMembers = 1;
        
        public void CreateClan(string name)
        {
            clanName = name;
            Debug.Log($"[Clan] Created: {name}");
        }
        
        public void JoinClan(string name)
        {
            clanName = name;
            clanMembers++;
            Debug.Log($"[Clan] Joined: {name}");
        }
        
        public string GetClanName() => clanName;
    }
}
