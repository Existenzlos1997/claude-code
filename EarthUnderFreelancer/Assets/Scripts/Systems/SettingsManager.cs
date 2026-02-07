using UnityEngine;

namespace EarthUnderFreelancer.Systems
{
    // Iteration 94-96: Comprehensive settings management
    public class SettingsManager : MonoBehaviour
    {
        public void SetGraphicsQuality(int level)
        {
            QualitySettings.SetQualityLevel(level);
            Debug.Log($"[Settings] Graphics quality: {level}");
        }
        
        public void SetResolution(int width, int height)
        {
            Screen.SetResolution(width, height, Screen.fullScreen);
        }
        
        public void SetFullscreen(bool fullscreen)
        {
            Screen.fullScreen = fullscreen;
        }
    }
}
