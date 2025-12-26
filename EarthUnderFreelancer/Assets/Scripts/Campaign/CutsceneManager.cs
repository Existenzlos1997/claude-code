using UnityEngine;

namespace EarthUnderFreelancer.Campaign
{
    // Iteration 54-55: Cutscene and dialogue system
    public class CutsceneManager : MonoBehaviour
    {
        private bool isPlaying = false;
        
        public void PlayCutscene(string cutsceneName)
        {
            isPlaying = true;
            Debug.Log($"[Cutscene] Playing: {cutsceneName}");
        }
        
        public void SkipCutscene()
        {
            isPlaying = false;
            Debug.Log("[Cutscene] Skipped");
        }
    }
}
