using UnityEngine;

namespace EarthUnderFreelancer.UI
{
    // Iteration 14: Radial quick action menu
    public class QuickActionMenu : MonoBehaviour
    {
        private bool isMenuOpen = false;
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                ToggleMenu();
            }
        }
        
        private void ToggleMenu()
        {
            isMenuOpen = !isMenuOpen;
            Debug.Log($"[Quick Menu] {(isMenuOpen ? "Opened" : "Closed")}");
        }
        
        private void OnGUI()
        {
            if (!isMenuOpen) return;
            
            float centerX = Screen.width / 2;
            float centerY = Screen.height / 2;
            float radius = 100f;
            
            GUI.Box(new Rect(centerX - 150, centerY - 150, 300, 300), "Quick Actions (Q)");
            
            string[] actions = { "Repair", "Refuel", "Rearm", "Map", "Squad" };
            for (int i = 0; i < actions.Length; i++)
            {
                float angle = i * (360f / actions.Length) * Mathf.Deg2Rad;
                float x = centerX + Mathf.Cos(angle) * radius - 40;
                float y = centerY + Mathf.Sin(angle) * radius - 10;
                
                if (GUI.Button(new Rect(x, y, 80, 20), actions[i]))
                {
                    Debug.Log($"[Quick Menu] Selected: {actions[i]}");
                    isMenuOpen = false;
                }
            }
        }
    }
}
