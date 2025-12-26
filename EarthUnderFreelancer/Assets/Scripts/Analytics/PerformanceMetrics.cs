using UnityEngine;

namespace EarthUnderFreelancer.Analytics
{
    // Iteration 44-45: Performance metrics and telemetry
    public class PerformanceMetrics : MonoBehaviour
    {
        private float averageFPS = 60f;
        private float peakMemoryUsage = 0f;
        private int totalFrames = 0;
        
        private void Update()
        {
            totalFrames++;
            float currentFPS = 1f / Time.deltaTime;
            averageFPS = (averageFPS * (totalFrames - 1) + currentFPS) / totalFrames;
        }
        
        public float GetAverageFPS() => averageFPS;
    }
}
