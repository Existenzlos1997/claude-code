using UnityEngine;

namespace EarthUnderFreelancer.Communication
{
    // Iteration 39-40: Emote and gesture system
    public class EmoteSystem : MonoBehaviour
    {
        public enum Emote { Salute, Wave, Thumbsup, Victory }
        
        public void PlayEmote(Emote emote)
        {
            Debug.Log($"[Emote] Playing: {emote}");
        }
    }
}
