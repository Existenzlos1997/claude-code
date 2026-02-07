using UnityEngine;
using UnityEngine.Profiling;
using System.Text;

namespace EarthUnderFreelancer.Gameplay
{
    /// <summary>
    /// Performance profiler integration for EarthUnderFreelancer
    /// Monitors FPS, memory, and provides optimization suggestions
    /// Implements VERBESSERUNGSPLAN Phase 1 - Performance Profiling
    /// </summary>
    public class PerformanceProfiler : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool enableOnStart = true;
        [SerializeField] private KeyCode toggleKey = KeyCode.F2;
        [SerializeField] private float updateInterval = 1f;

        [Header("Display Settings")]
        [SerializeField] private bool showGUI = true;
        [SerializeField] private int fontSize = 12;
        [SerializeField] private Color goodColor = Color.green;
        [SerializeField] private Color warningColor = Color.yellow;
        [SerializeField] private Color badColor = Color.red;

        // Performance metrics
        private float fps = 0f;
        private float minFps = float.MaxValue;
        private float maxFps = 0f;
        private float avgFps = 0f;
        
        private long totalMemory = 0;
        private long usedMemory = 0;
        private long monoMemory = 0;
        
        private int drawCalls = 0;
        private int triangles = 0;
        private int vertices = 0;

        // Tracking
        private float fpsSum = 0f;
        private int fpsSamples = 0;
        private float timer = 0f;
        
        private bool isVisible = false;
        private Rect windowRect = new Rect(10, 140, 350, 450);
        private Vector2 scrollPosition;

        private StringBuilder logBuilder = new StringBuilder();

        private void Start()
        {
            if (enableOnStart)
            {
                enabled = true;
            }

            Application.targetFrameRate = -1; // Unlock framerate for profiling
        }

        private void Update()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                isVisible = !isVisible;
                Debug.Log($"Performance Profiler: {(isVisible ? "Enabled" : "Disabled")}");
            }

            timer += Time.unscaledDeltaTime;

            if (timer >= updateInterval)
            {
                UpdateMetrics();
                timer = 0f;
            }

            // Track FPS continuously for average
            float currentFps = 1f / Time.unscaledDeltaTime;
            fpsSum += currentFps;
            fpsSamples++;
        }

        private void UpdateMetrics()
        {
            // Calculate FPS
            fps = fpsSamples > 0 ? fpsSum / fpsSamples : 0f;
            
            if (fps < minFps && fps > 0) minFps = fps;
            if (fps > maxFps) maxFps = fps;
            avgFps = fps;

            // Reset for next interval
            fpsSum = 0f;
            fpsSamples = 0;

            // Memory metrics
            totalMemory = Profiler.GetTotalReservedMemoryLong() / (1024 * 1024); // MB
            usedMemory = Profiler.GetTotalAllocatedMemoryLong() / (1024 * 1024); // MB
            monoMemory = Profiler.GetMonoUsedSizeLong() / (1024 * 1024); // MB

            // Rendering stats (approximation - Unity doesn't expose all stats easily)
            // In a real build, you'd use Unity Profiler API or rendering stats
        }

        private void OnGUI()
        {
            if (!showGUI || !isVisible) return;

            windowRect = GUI.Window(1, windowRect, DrawProfilerWindow, "Performance Profiler");
        }

        private void DrawProfilerWindow(int windowID)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            GUIStyle headerStyle = new GUIStyle(GUI.skin.label);
            headerStyle.fontStyle = FontStyle.Bold;
            headerStyle.fontSize = fontSize + 2;

            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontSize = fontSize;

            // FPS Section
            GUILayout.Label("=== Frame Rate ===", headerStyle);
            DrawMetric("FPS:", $"{fps:F1}", GetFPSColor(fps), labelStyle);
            DrawMetric("Min FPS:", $"{minFps:F1}", Color.white, labelStyle);
            DrawMetric("Max FPS:", $"{maxFps:F1}", Color.white, labelStyle);
            DrawMetric("Frame Time:", $"{(1000f / fps):F2} ms", Color.white, labelStyle);

            GUILayout.Space(10);

            // Memory Section
            GUILayout.Label("=== Memory ===", headerStyle);
            DrawMetric("Total Reserved:", $"{totalMemory} MB", Color.white, labelStyle);
            DrawMetric("Used Memory:", $"{usedMemory} MB", GetMemoryColor(usedMemory), labelStyle);
            DrawMetric("Mono Memory:", $"{monoMemory} MB", Color.white, labelStyle);
            float memoryPercent = totalMemory > 0 ? (usedMemory * 100f / totalMemory) : 0f;
            DrawMetric("Usage:", $"{memoryPercent:F1}%", GetMemoryPercentColor(memoryPercent), labelStyle);

            GUILayout.Space(10);

            // System Info
            GUILayout.Label("=== System ===", headerStyle);
            DrawMetric("Graphics:", SystemInfo.graphicsDeviceName, Color.white, labelStyle);
            DrawMetric("VRAM:", $"{SystemInfo.graphicsMemorySize} MB", Color.white, labelStyle);
            DrawMetric("CPU Cores:", SystemInfo.processorCount.ToString(), Color.white, labelStyle);
            DrawMetric("System RAM:", $"{SystemInfo.systemMemorySize} MB", Color.white, labelStyle);

            GUILayout.Space(10);

            // Quality Settings
            GUILayout.Label("=== Quality ===", headerStyle);
            DrawMetric("Quality Level:", QualitySettings.names[QualitySettings.GetQualityLevel()], Color.white, labelStyle);
            DrawMetric("VSync:", QualitySettings.vSyncCount == 0 ? "Off" : "On", Color.white, labelStyle);
            DrawMetric("Target FPS:", Application.targetFrameRate == -1 ? "Unlimited" : Application.targetFrameRate.ToString(), Color.white, labelStyle);

            GUILayout.Space(10);

            // Optimization Suggestions
            GUILayout.Label("=== Suggestions ===", headerStyle);
            ShowOptimizationSuggestions(labelStyle);

            GUILayout.Space(10);

            // Actions
            GUILayout.Label("=== Actions ===", headerStyle);
            
            if (GUILayout.Button("Reset Stats"))
            {
                ResetStats();
            }

            if (GUILayout.Button("Force GC"))
            {
                System.GC.Collect();
                Debug.Log("Forced garbage collection");
            }

            if (GUILayout.Button("Log Report"))
            {
                LogPerformanceReport();
            }

            GUILayout.Space(5);
            GUILayout.Label($"Toggle: {toggleKey}", GUI.skin.label);

            GUILayout.EndScrollView();

            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }

        private void DrawMetric(string label, string value, Color color, GUIStyle style)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, style, GUILayout.Width(150));
            
            Color oldColor = GUI.contentColor;
            GUI.contentColor = color;
            GUILayout.Label(value, style);
            GUI.contentColor = oldColor;
            
            GUILayout.EndHorizontal();
        }

        private Color GetFPSColor(float fps)
        {
            if (fps >= 60f) return goodColor;
            if (fps >= 30f) return warningColor;
            return badColor;
        }

        private Color GetMemoryColor(long memoryMB)
        {
            if (memoryMB < 512) return goodColor;
            if (memoryMB < 1024) return warningColor;
            return badColor;
        }

        private Color GetMemoryPercentColor(float percent)
        {
            if (percent < 60f) return goodColor;
            if (percent < 80f) return warningColor;
            return badColor;
        }

        private void ShowOptimizationSuggestions(GUIStyle style)
        {
            if (fps < 30f)
            {
                GUILayout.Label("⚠️ Low FPS - Consider reducing quality", style);
            }

            if (usedMemory > 1024)
            {
                GUILayout.Label("⚠️ High memory usage - Check for leaks", style);
            }

            if (fps >= 60f && usedMemory < 512)
            {
                GUILayout.Label("✅ Performance is good!", style);
            }

            // Object count warning
            int objectCount = FindObjectsOfType<GameObject>().Length;
            if (objectCount > 1000)
            {
                GUILayout.Label($"⚠️ High object count ({objectCount}) - Use pooling", style);
            }
        }

        private void ResetStats()
        {
            minFps = float.MaxValue;
            maxFps = 0f;
            avgFps = 0f;
            fpsSum = 0f;
            fpsSamples = 0;
            
            Debug.Log("Performance stats reset");
        }

        private void LogPerformanceReport()
        {
            logBuilder.Clear();
            logBuilder.AppendLine("=== PERFORMANCE REPORT ===");
            logBuilder.AppendLine($"Time: {System.DateTime.Now}");
            logBuilder.AppendLine();
            
            logBuilder.AppendLine("Frame Rate:");
            logBuilder.AppendLine($"  Current: {fps:F1} FPS");
            logBuilder.AppendLine($"  Min: {minFps:F1} FPS");
            logBuilder.AppendLine($"  Max: {maxFps:F1} FPS");
            logBuilder.AppendLine();
            
            logBuilder.AppendLine("Memory:");
            logBuilder.AppendLine($"  Total Reserved: {totalMemory} MB");
            logBuilder.AppendLine($"  Used: {usedMemory} MB");
            logBuilder.AppendLine($"  Mono: {monoMemory} MB");
            logBuilder.AppendLine();
            
            logBuilder.AppendLine("System:");
            logBuilder.AppendLine($"  GPU: {SystemInfo.graphicsDeviceName}");
            logBuilder.AppendLine($"  VRAM: {SystemInfo.graphicsMemorySize} MB");
            logBuilder.AppendLine($"  CPU Cores: {SystemInfo.processorCount}");
            logBuilder.AppendLine($"  System RAM: {SystemInfo.systemMemorySize} MB");
            logBuilder.AppendLine();
            
            logBuilder.AppendLine("Quality:");
            logBuilder.AppendLine($"  Level: {QualitySettings.names[QualitySettings.GetQualityLevel()]}");
            logBuilder.AppendLine($"  VSync: {(QualitySettings.vSyncCount == 0 ? "Off" : "On")}");
            logBuilder.AppendLine();
            
            int objectCount = FindObjectsOfType<GameObject>().Length;
            logBuilder.AppendLine($"Scene Objects: {objectCount}");
            
            logBuilder.AppendLine("========================");
            
            Debug.Log(logBuilder.ToString());
        }

        public void Toggle()
        {
            isVisible = !isVisible;
        }

        // Public API
        public float GetFPS() => fps;
        public float GetMinFPS() => minFps;
        public float GetMaxFPS() => maxFps;
        public long GetUsedMemoryMB() => usedMemory;
        public long GetTotalMemoryMB() => totalMemory;
    }
}
