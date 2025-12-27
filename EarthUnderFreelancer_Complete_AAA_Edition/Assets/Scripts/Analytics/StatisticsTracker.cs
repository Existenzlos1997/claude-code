using UnityEngine;

namespace EarthUnderFreelancer.Analytics
{
    // Iteration 41-43: Player statistics tracking
    public class StatisticsTracker : MonoBehaviour
    {
        private int totalFlightTime = 0;
        private int totalKills = 0;
        private int totalDeaths = 0;
        private int totalMissions = 0;
        private float totalDistance = 0f;
        
        public void RecordKill() => totalKills++;
        public void RecordDeath() => totalDeaths++;
        public void RecordMission() => totalMissions++;
        public void RecordFlightTime(int seconds) => totalFlightTime += seconds;
        public void RecordDistance(float meters) => totalDistance += meters;
        
        public float GetKDRatio() => totalDeaths > 0 ? (float)totalKills / totalDeaths : totalKills;
        public int GetTotalKills() => totalKills;
        public int GetTotalMissions() => totalMissions;
    }
}
