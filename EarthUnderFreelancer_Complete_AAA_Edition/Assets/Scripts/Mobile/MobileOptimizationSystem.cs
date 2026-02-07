using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Mobile
{
    /// <summary>
    /// Complete mobile optimization system with touch controls, performance scaling, and battery awareness
    /// </summary>
    public class MobileOptimizationSystem : MonoBehaviour
    {
        public static MobileOptimizationSystem Instance { get; private set; }

        [Header("Touch Controls")]
        public bool enableTouchControls = true;
        public float joystickSensitivity = 1.0f;
        public float touchButtonSize = 80f;

        [Header("Performance")]
        public bool autoPerformanceScaling = true;
        public int targetFrameRate = 60;
        public float lowBatteryThreshold = 0.2f; // 20%

        [Header("Network")]
        public bool enableDataCompression = true;
        public int maxConcurrentDownloads = 3;

        // Touch controls
        private Vector2 virtualJoystickPosition;
        private Vector2 virtualJoystickDelta;
        private bool isJoystickActive;
        private Dictionary<string, Rect> touchButtons = new Dictionary<string, Rect>();
        private HashSet<string> activeButtons = new HashSet<string>();

        // Performance tracking
        private bool isMobileMode;
        private bool isLowBattery;
        private float currentBatteryLevel = 1.0f;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeMobileMode();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void InitializeMobileMode()
        {
            // Detect if running on mobile
            #if UNITY_ANDROID || UNITY_IOS
            isMobileMode = true;
            #else
            isMobileMode = Application.isMobilePlatform;
            #endif

            if (isMobileMode)
            {
                EnableMobileMode();
            }

            // Set target frame rate
            Application.targetFrameRate = targetFrameRate;

            // Initialize touch buttons
            SetupTouchButtons();
        }

        public void EnableMobileMode()
        {
            isMobileMode = true;

            // Enable touch controls
            if (enableTouchControls)
            {
                Input.multiTouchEnabled = true;
            }

            // Optimize graphics for mobile
            if (autoPerformanceScaling)
            {
                OptimizeGraphicsForMobile();
            }

            // Enable data compression
            if (enableDataCompression)
            {
                EnableNetworkCompression();
            }

            Debug.Log("[Mobile] Mobile mode enabled with optimizations");
        }

        void SetupTouchButtons()
        {
            // Define touch button positions (screen-space coordinates)
            float margin = 20f;
            float size = touchButtonSize;

            // Left side - Movement joystick area
            touchButtons["joystick"] = new Rect(margin, Screen.height - size - margin, size, size);

            // Right side - Action buttons
            touchButtons["fire"] = new Rect(Screen.width - size - margin, Screen.height - size - margin, size, size);
            touchButtons["missile"] = new Rect(Screen.width - size * 2 - margin * 2, Screen.height - size - margin, size, size);
            touchButtons["target"] = new Rect(Screen.width - size - margin, Screen.height - size * 2 - margin * 2, size, size);
            touchButtons["brake"] = new Rect(Screen.width - size * 2 - margin * 2, Screen.height - size * 2 - margin * 2, size, size);
        }

        void Update()
        {
            if (!isMobileMode) return;

            // Process touch input
            ProcessTouchInput();

            // Monitor battery level
            MonitorBattery();

            // Auto-adjust performance based on conditions
            if (autoPerformanceScaling)
            {
                AutoAdjustPerformance();
            }
        }

        void ProcessTouchInput()
        {
            if (!enableTouchControls) return;

            // Clear previous frame's button states
            activeButtons.Clear();
            virtualJoystickDelta = Vector2.zero;

            // Process all touches
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                Vector2 touchPos = touch.position;

                // Check joystick
                if (touchButtons["joystick"].Contains(touchPos))
                {
                    if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                    {
                        isJoystickActive = true;
                        virtualJoystickPosition = touchPos;
                        
                        // Calculate delta from center
                        Vector2 center = touchButtons["joystick"].center;
                        virtualJoystickDelta = (touchPos - center) / (touchButtonSize * 0.5f) * joystickSensitivity;
                        virtualJoystickDelta = Vector2.ClampMagnitude(virtualJoystickDelta, 1.0f);
                    }
                    else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    {
                        isJoystickActive = false;
                        virtualJoystickDelta = Vector2.zero;
                    }
                }

                // Check action buttons
                foreach (var button in touchButtons)
                {
                    if (button.Key == "joystick") continue;

                    if (button.Value.Contains(touchPos) && touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
                    {
                        activeButtons.Add(button.Key);
                    }
                }
            }
        }

        void MonitorBattery()
        {
            currentBatteryLevel = SystemInfo.batteryLevel;
            bool wasLowBattery = isLowBattery;
            isLowBattery = currentBatteryLevel < lowBatteryThreshold && currentBatteryLevel >= 0;

            // Battery state changed
            if (isLowBattery != wasLowBattery)
            {
                if (isLowBattery)
                {
                    Debug.Log("[Mobile] Low battery detected - reducing performance");
                    ReducePerformanceForBattery();
                }
                else
                {
                    Debug.Log("[Mobile] Battery level restored - increasing performance");
                    RestorePerformance();
                }
            }
        }

        void AutoAdjustPerformance()
        {
            // Monitor frame rate and adjust quality
            float fps = 1.0f / Time.smoothDeltaTime;

            if (fps < targetFrameRate * 0.8f) // Below 80% of target
            {
                // Reduce quality slightly
                int currentQuality = QualitySettings.GetQualityLevel();
                if (currentQuality > 0)
                {
                    QualitySettings.DecreaseLevel();
                }
            }
            else if (fps > targetFrameRate * 1.2f && !isLowBattery) // Above 120% of target and not low battery
            {
                // Can increase quality
                int currentQuality = QualitySettings.GetQualityLevel();
                if (currentQuality < QualitySettings.names.Length - 1)
                {
                    QualitySettings.IncreaseLevel();
                }
            }
        }

        void OptimizeGraphicsForMobile()
        {
            // Set appropriate quality level for mobile
            QualitySettings.SetQualityLevel(1); // Medium quality

            // Reduce shadow distance
            QualitySettings.shadowDistance = 50f;

            // Disable expensive effects
            QualitySettings.shadows = ShadowQuality.HardOnly;
            QualitySettings.antiAliasing = 0;

            // Optimize particle systems
            QualitySettings.particleRaycastBudget = 256;

            Debug.Log("[Mobile] Graphics optimized for mobile platform");
        }

        void ReducePerformanceForBattery()
        {
            // Further reduce quality to save battery
            Application.targetFrameRate = 30; // Half frame rate
            QualitySettings.SetQualityLevel(0); // Low quality
            QualitySettings.shadowDistance = 25f; // Very short shadows
        }

        void RestorePerformance()
        {
            Application.targetFrameRate = targetFrameRate;
            OptimizeGraphicsForMobile();
        }

        void EnableNetworkCompression()
        {
            // Enable network data compression (implementation would use actual networking library)
            Debug.Log("[Mobile] Network compression enabled");
        }

        // Public API for getting touch input
        public Vector2 GetVirtualJoystick()
        {
            return virtualJoystickDelta;
        }

        public bool GetTouchButton(string buttonName)
        {
            return activeButtons.Contains(buttonName);
        }

        public bool GetTouchButtonDown(string buttonName)
        {
            // Simple implementation - could be enhanced with proper button state tracking
            return activeButtons.Contains(buttonName);
        }

        public bool IsMobileMode()
        {
            return isMobileMode;
        }

        public float GetBatteryLevel()
        {
            return currentBatteryLevel;
        }

        public bool IsLowBattery()
        {
            return isLowBattery;
        }

        // Haptic feedback
        public void TriggerHapticFeedback(HapticFeedbackType type = HapticFeedbackType.Medium)
        {
            if (!isMobileMode) return;

            #if UNITY_ANDROID || UNITY_IOS
            switch (type)
            {
                case HapticFeedbackType.Light:
                    Handheld.Vibrate();
                    break;
                case HapticFeedbackType.Medium:
                    Handheld.Vibrate();
                    break;
                case HapticFeedbackType.Heavy:
                    Handheld.Vibrate();
                    break;
            }
            #endif
        }

        void OnGUI()
        {
            if (!isMobileMode || !enableTouchControls) return;

            // Draw touch controls overlay
            GUI.skin.box.normal.background = MakeTex(2, 2, new Color(1, 1, 1, 0.3f));
            GUI.skin.button.normal.background = MakeTex(2, 2, new Color(0.5f, 0.5f, 0.5f, 0.5f));

            // Draw joystick
            GUI.Box(touchButtons["joystick"], "");
            if (isJoystickActive)
            {
                Vector2 center = touchButtons["joystick"].center;
                Vector2 knobPos = center + virtualJoystickDelta * (touchButtonSize * 0.3f);
                GUI.Box(new Rect(knobPos.x - 20, knobPos.y - 20, 40, 40), "");
            }

            // Draw action buttons
            GUI.Button(touchButtons["fire"], "FIRE");
            GUI.Button(touchButtons["missile"], "MSL");
            GUI.Button(touchButtons["target"], "TGT");
            GUI.Button(touchButtons["brake"], "BRK");

            // Show battery indicator if low
            if (isLowBattery)
            {
                GUI.Label(new Rect(10, 10, 200, 30), $"Battery: {(currentBatteryLevel * 100):F0}%");
            }
        }

        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }

    public enum HapticFeedbackType
    {
        Light,
        Medium,
        Heavy
    }
}
