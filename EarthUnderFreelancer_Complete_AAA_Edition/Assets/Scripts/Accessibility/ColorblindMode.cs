using UnityEngine;

namespace EarthUnderFreelancer.Accessibility
{
    // Iteration 61-62: Colorblind accessibility modes
    public class ColorblindMode : MonoBehaviour
    {
        public enum ColorblindType { None, Protanopia, Deuteranopia, Tritanopia }
        
        private ColorblindType currentMode = ColorblindType.None;
        
        public void SetColorblindMode(ColorblindType mode)
        {
            currentMode = mode;
            Debug.Log($"[Accessibility] Colorblind mode: {mode}");
        }
    }
}
