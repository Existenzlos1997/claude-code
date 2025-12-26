using UnityEngine;

namespace EarthUnderFreelancer.UI
{
    /// <summary>
    /// Enhanced HUD system with comprehensive flight and combat information
    /// Implements VERBESSERUNGSPLAN Priority 7 - UI Improvements
    /// </summary>
    public class EnhancedHUD : MonoBehaviour
    {
        [Header("HUD Sections")]
        [SerializeField] private bool showFlightInfo = true;
        [SerializeField] private bool showCombatInfo = true;
        [SerializeField] private bool showMissionInfo = true;
        [SerializeField] private bool showSystemInfo = true;
        
        [Header("HUD Style")]
        [SerializeField] private Color hudColor = new Color(0f, 1f, 0.5f, 0.8f);
        [SerializeField] private Color warningColor = Color.yellow;
        [SerializeField] private Color dangerColor = Color.red;
        [SerializeField] private int fontSize = 14;
        
        // Flight data
        private float altitude = 0f;
        private float speed = 0f;
        private float throttle = 0f;
        private float heading = 0f;
        
        // Combat data
        private int ammo = 100;
        private int missiles = 8;
        private float health = 100f;
        private float shields = 100f;
        private string targetName = "None";
        private float targetDistance = 0f;
        
        // Mission data
        private string missionObjective = "Patrol Area";
        private int objectivesComplete = 0;
        private int totalObjectives = 5;
        
        private GUIStyle hudStyle;
        private GUIStyle warningStyle;
        private GUIStyle dangerStyle;
        
        private void Start()
        {
            InitializeStyles();
        }
        
        private void Update()
        {
            UpdateFlightData();
            UpdateCombatData();
        }
        
        private void InitializeStyles()
        {
            hudStyle = new GUIStyle();
            hudStyle.fontSize = fontSize;
            hudStyle.normal.textColor = hudColor;
            hudStyle.alignment = TextAnchor.UpperLeft;
            
            warningStyle = new GUIStyle(hudStyle);
            warningStyle.normal.textColor = warningColor;
            
            dangerStyle = new GUIStyle(hudStyle);
            dangerStyle.normal.textColor = dangerColor;
        }
        
        private void UpdateFlightData()
        {
            // Get data from player (placeholder values for now)
            altitude = transform.position.y;
            speed = GetComponent<Rigidbody>()?.velocity.magnitude ?? 0f;
            heading = transform.eulerAngles.y;
        }
        
        private void UpdateCombatData()
        {
            // Would get from combat systems
        }
        
        private void OnGUI()
        {
            if (hudStyle == null) InitializeStyles();
            
            float leftMargin = 20f;
            float rightMargin = Screen.width - 220f;
            float topMargin = 20f;
            float lineHeight = fontSize + 5f;
            float currentY = topMargin;
            
            // Left side - Flight info
            if (showFlightInfo)
            {
                GUI.Label(new Rect(leftMargin, currentY, 200, lineHeight), "=== FLIGHT ===", hudStyle);
                currentY += lineHeight;
                
                GUI.Label(new Rect(leftMargin, currentY, 200, lineHeight), $"ALT: {altitude:F0}m", hudStyle);
                currentY += lineHeight;
                
                GUI.Label(new Rect(leftMargin, currentY, 200, lineHeight), $"SPD: {speed:F0} m/s", hudStyle);
                currentY += lineHeight;
                
                GUI.Label(new Rect(leftMargin, currentY, 200, lineHeight), $"HDG: {heading:F0}°", hudStyle);
                currentY += lineHeight;
                
                GUIStyle throttleStyle = throttle > 0.9f ? warningStyle : hudStyle;
                GUI.Label(new Rect(leftMargin, currentY, 200, lineHeight), $"THR: {throttle * 100f:F0}%", throttleStyle);
                currentY += lineHeight + 10f;
            }
            
            // Left side - Combat info
            if (showCombatInfo)
            {
                GUI.Label(new Rect(leftMargin, currentY, 200, lineHeight), "=== COMBAT ===", hudStyle);
                currentY += lineHeight;
                
                GUIStyle healthStyle = health < 30f ? dangerStyle : (health < 60f ? warningStyle : hudStyle);
                GUI.Label(new Rect(leftMargin, currentY, 200, lineHeight), $"HP: {health:F0}%", healthStyle);
                currentY += lineHeight;
                
                GUIStyle shieldStyle = shields < 30f ? warningStyle : hudStyle;
                GUI.Label(new Rect(leftMargin, currentY, 200, lineHeight), $"SHD: {shields:F0}%", shieldStyle);
                currentY += lineHeight;
                
                GUI.Label(new Rect(leftMargin, currentY, 200, lineHeight), $"AMMO: {ammo}", hudStyle);
                currentY += lineHeight;
                
                GUI.Label(new Rect(leftMargin, currentY, 200, lineHeight), $"MISL: {missiles}", hudStyle);
                currentY += lineHeight;
                
                GUI.Label(new Rect(leftMargin, currentY, 200, lineHeight), $"TGT: {targetName}", hudStyle);
                currentY += lineHeight;
                
                if (targetDistance > 0)
                {
                    GUI.Label(new Rect(leftMargin, currentY, 200, lineHeight), $"DST: {targetDistance:F0}m", hudStyle);
                    currentY += lineHeight;
                }
            }
            
            // Right side - Mission info
            currentY = topMargin;
            if (showMissionInfo)
            {
                GUI.Label(new Rect(rightMargin, currentY, 200, lineHeight), "=== MISSION ===", hudStyle);
                currentY += lineHeight;
                
                GUI.Label(new Rect(rightMargin, currentY, 200, lineHeight), missionObjective, hudStyle);
                currentY += lineHeight;
                
                GUI.Label(new Rect(rightMargin, currentY, 200, lineHeight), 
                    $"Progress: {objectivesComplete}/{totalObjectives}", hudStyle);
                currentY += lineHeight + 10f;
            }
            
            // Bottom center - System info
            if (showSystemInfo)
            {
                float centerX = Screen.width / 2 - 100f;
                float bottomY = Screen.height - 40f;
                
                GUI.Label(new Rect(centerX, bottomY, 200, lineHeight), 
                    $"FPS: {(1f / Time.deltaTime):F0}", hudStyle);
            }
            
            // Crosshair
            DrawCrosshair();
        }
        
        private void DrawCrosshair()
        {
            float centerX = Screen.width / 2;
            float centerY = Screen.height / 2;
            float size = 20f;
            float thickness = 2f;
            
            // Horizontal line
            GUI.DrawTexture(new Rect(centerX - size, centerY - thickness / 2, size - 5, thickness), 
                Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 0f, hudColor, 0f, 0f);
            GUI.DrawTexture(new Rect(centerX + 5, centerY - thickness / 2, size - 5, thickness), 
                Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 0f, hudColor, 0f, 0f);
            
            // Vertical line
            GUI.DrawTexture(new Rect(centerX - thickness / 2, centerY - size, thickness, size - 5), 
                Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 0f, hudColor, 0f, 0f);
            GUI.DrawTexture(new Rect(centerX - thickness / 2, centerY + 5, thickness, size - 5), 
                Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 0f, hudColor, 0f, 0f);
        }
        
        // Public API to update HUD data
        public void UpdateFlightInfo(float alt, float spd, float thr, float hdg)
        {
            altitude = alt;
            speed = spd;
            throttle = thr;
            heading = hdg;
        }
        
        public void UpdateCombatInfo(float hp, float shd, int ammoCount, int missileCount)
        {
            health = hp;
            shields = shd;
            ammo = ammoCount;
            missiles = missileCount;
        }
        
        public void UpdateTarget(string name, float distance)
        {
            targetName = name;
            targetDistance = distance;
        }
        
        public void UpdateMission(string objective, int complete, int total)
        {
            missionObjective = objective;
            objectivesComplete = complete;
            totalObjectives = total;
        }
    }
}
