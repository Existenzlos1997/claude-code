using UnityEngine;

namespace EarthUnderFreelancer.Multiplayer
{
    // Iteration 21: Voice chat system placeholder
    public class VoiceChat : MonoBehaviour
    {
        private bool isMuted = false;
        private bool isPushToTalk = true;
        
        public void ToggleMute()
        {
            isMuted = !isMuted;
            Debug.Log($"[Voice] {(isMuted ? "Muted" : "Unmuted")}");
        }
        
        public void SetPushToTalk(bool enabled)
        {
            isPushToTalk = enabled;
        }
    }
}
