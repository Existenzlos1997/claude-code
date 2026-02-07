using UnityEngine;

namespace EarthUnderFreelancer.Accessibility
{
    // Iteration 63-64: Subtitle and caption system
    public class SubtitleSystem : MonoBehaviour
    {
        private bool subtitlesEnabled = true;
        private string currentSubtitle = "";
        
        public void ShowSubtitle(string text, float duration = 3f)
        {
            if (subtitlesEnabled)
            {
                currentSubtitle = text;
                Invoke(nameof(ClearSubtitle), duration);
            }
        }
        
        private void ClearSubtitle() => currentSubtitle = "";
        
        private void OnGUI()
        {
            if (!string.IsNullOrEmpty(currentSubtitle))
            {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.alignment = TextAnchor.MiddleCenter;
                style.fontSize = 18;
                GUI.Label(new Rect(0, Screen.height - 100, Screen.width, 50), currentSubtitle, style);
            }
        }
    }
}
