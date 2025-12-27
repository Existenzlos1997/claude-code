using UnityEngine;

namespace EarthUnderFreelancer.Campaign
{
    // Iteration 51-53: Story progression system
    public class StoryManager : MonoBehaviour
    {
        private int currentChapter = 1;
        private int currentMission = 1;
        
        public void CompleteChapter()
        {
            currentChapter++;
            currentMission = 1;
            Debug.Log($"[Story] Chapter {currentChapter} unlocked!");
        }
        
        public void CompleteMission()
        {
            currentMission++;
            Debug.Log($"[Story] Mission {currentMission} of Chapter {currentChapter}");
        }
        
        public int GetCurrentChapter() => currentChapter;
    }
}
