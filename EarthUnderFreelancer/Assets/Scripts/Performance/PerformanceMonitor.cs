using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;

namespace EarthUnderFreelancer.Performance
{
    /// <summary>
    /// Performance monitoring and profiling system
    /// Tracks FPS, memory usage, and provides optimization recommendations
    /// </summary>
    public class PerformanceMonitor : MonoBehaviour
    {
        public static PerformanceMonitor Instance { get; private set; }

        [Header("Performance Targets")]
        [SerializeField] private int targetFPS = 60;
        [SerializeField] private float lowFPSThreshold = 30f;
        [SerializeField] private long maxMemoryMB = 512;

        [Header("Monitoring Settings")]
        [SerializeField] private bool enableMonitoring = true;
        [SerializeField] private float updateInterval = 0.5f;
        [SerializeField] private bool showDebugInfo = false;

        // FPS Tracking
        private float fps;
        private float averageFPS;
        private float worstFPS = 999f;
        private int frameCount;
        private float fpsTimer;

        // Memory Tracking
        private long totalMemoryMB;
        private long usedMemoryMB;
        private long gcMemoryMB;

        // Performance Warnings
        private List<string> performanceWarnings = new List<string>();
        private bool isLowPerformance;

        // Statistics
        private Dictionary<string, ProfilerSample> profilerSamples = new Dictionary<string, ProfilerSample>();

        public float CurrentFPS => fps;
        public float AverageFPS => averageFPS;
        public float WorstFPS => worstFPS;
        public long UsedMemoryMB => usedMemoryMB;
        public bool IsLowPerformance => isLowPerformance;
        public List<string> PerformanceWarnings => performanceWarnings;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            if (!enableMonitoring) return;

            UpdateFPS();
            
            fpsTimer += Time.unscaledDeltaTime;
            if (fpsTimer >= updateInterval)
            {
                UpdateMemoryStats();
                CheckPerformance();
                fpsTimer = 0f;
            }
        }

        private void UpdateFPS()
        {
            frameCount++;
            float deltaTime = Time.unscaledDeltaTime;
            
            if (deltaTime > 0)
            {
                fps = 1f / deltaTime;
                
                // Track averages
                averageFPS = Mathf.Lerp(averageFPS, fps, 0.1f);
                
                // Track worst FPS
                if (fps < worstFPS)
                {
                    worstFPS = fps;
                }
            }
        }

        private void UpdateMemoryStats()
        {
            totalMemoryMB = System.GC.GetTotalMemory(false) / (1024 * 1024);
            usedMemoryMB = (long)(UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / (1024 * 1024));
            gcMemoryMB = totalMemoryMB;
        }

        private void CheckPerformance()
        {
            performanceWarnings.Clear();

            // Check FPS
            if (fps < lowFPSThreshold)
            {
                isLowPerformance = true;
                performanceWarnings.Add($"Low FPS: {fps:F1} (Target: {targetFPS})");
            }
            else
            {
                isLowPerformance = false;
            }

            // Check Memory
            if (usedMemoryMB > maxMemoryMB)
            {
                performanceWarnings.Add($"High Memory Usage: {usedMemoryMB}MB (Max: {maxMemoryMB}MB)");
            }

            // Check for GC pressure
            if (Time.frameCount % 300 == 0) // Every ~5 seconds at 60fps
            {
                long beforeGC = System.GC.GetTotalMemory(false);
                System.GC.Collect();
                long afterGC = System.GC.GetTotalMemory(true);
                long collected = (beforeGC - afterGC) / (1024 * 1024);
                
                if (collected > 50) // More than 50MB collected
                {
                    performanceWarnings.Add($"High GC Pressure: {collected}MB collected");
                }
            }
        }

        public void BeginSample(string sampleName)
        {
            if (!enableMonitoring) return;

            if (!profilerSamples.ContainsKey(sampleName))
            {
                profilerSamples[sampleName] = new ProfilerSample(sampleName);
            }

            profilerSamples[sampleName].Begin();
        }

        public void EndSample(string sampleName)
        {
            if (!enableMonitoring) return;

            if (profilerSamples.ContainsKey(sampleName))
            {
                profilerSamples[sampleName].End();
            }
        }

        public string GetPerformanceReport()
        {
            System.Text.StringBuilder report = new System.Text.StringBuilder();
            report.AppendLine("=== Performance Report ===");
            report.AppendLine($"FPS: {fps:F1} (Avg: {averageFPS:F1}, Worst: {worstFPS:F1})");
            report.AppendLine($"Memory: {usedMemoryMB}MB / {maxMemoryMB}MB");
            report.AppendLine($"Frame Count: {frameCount}");
            
            if (performanceWarnings.Count > 0)
            {
                report.AppendLine("\nWarnings:");
                foreach (var warning in performanceWarnings)
                {
                    report.AppendLine($"- {warning}");
                }
            }

            if (profilerSamples.Count > 0)
            {
                report.AppendLine("\nProfiler Samples:");
                foreach (var kvp in profilerSamples)
                {
                    var sample = kvp.Value;
                    report.AppendLine($"- {sample.Name}: {sample.AverageMS:F2}ms (Calls: {sample.CallCount})");
                }
            }

            return report.ToString();
        }

        public void ResetStats()
        {
            frameCount = 0;
            averageFPS = 0f;
            worstFPS = 999f;
            performanceWarnings.Clear();
            profilerSamples.Clear();
        }

        private void OnGUI()
        {
            if (!showDebugInfo) return;

            GUIStyle style = new GUIStyle();
            style.fontSize = 16;
            style.normal.textColor = isLowPerformance ? Color.red : Color.green;

            GUI.Label(new Rect(10, 10, 300, 30), $"FPS: {fps:F1} | Avg: {averageFPS:F1}", style);
            GUI.Label(new Rect(10, 30, 300, 30), $"Memory: {usedMemoryMB}MB", style);

            if (performanceWarnings.Count > 0)
            {
                int y = 60;
                style.normal.textColor = Color.yellow;
                foreach (var warning in performanceWarnings)
                {
                    GUI.Label(new Rect(10, y, 600, 30), warning, style);
                    y += 20;
                }
            }
        }

        private class ProfilerSample
        {
            public string Name { get; private set; }
            public int CallCount { get; private set; }
            public float TotalMS { get; private set; }
            public float AverageMS => CallCount > 0 ? TotalMS / CallCount : 0f;

            private Stopwatch stopwatch;

            public ProfilerSample(string name)
            {
                Name = name;
                stopwatch = new Stopwatch();
            }

            public void Begin()
            {
                stopwatch.Restart();
            }

            public void End()
            {
                stopwatch.Stop();
                TotalMS += (float)stopwatch.Elapsed.TotalMilliseconds;
                CallCount++;
            }
        }
    }
}
