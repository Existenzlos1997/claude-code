using UnityEngine;

namespace EarthUnderFreelancer.UI
{
    // Iteration 89-90: Tooltip system
    public class TooltipSystem : MonoBehaviour
    {
        private string currentTooltip = "";
        
        public void ShowTooltip(string text) => currentTooltip = text;
        public void HideTooltip() => currentTooltip = "";
        
        private void OnGUI()
        {
            if (!string.IsNullOrEmpty(currentTooltip))
            {
                Vector2 mousePos = Input.mousePosition;
                GUI.Label(new Rect(mousePos.x + 10, Screen.height - mousePos.y + 10, 200, 50), currentTooltip);
            }
        }
    }
}
