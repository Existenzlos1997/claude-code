using UnityEngine;

namespace EarthUnderFreelancer.Automation
{
    // Iteration 80-82: Advanced autopilot features
    public class AutoPilotAdvanced : MonoBehaviour
    {
        public enum AutoPilotMode { Off, Hold, Follow, Combat, Landing }
        
        private AutoPilotMode mode = AutoPilotMode.Off;
        private Transform followTarget;
        
        public void SetMode(AutoPilotMode newMode)
        {
            mode = newMode;
            Debug.Log($"[AutoPilot] Mode: {mode}");
        }
        
        public void SetFollowTarget(Transform target)
        {
            followTarget = target;
            mode = AutoPilotMode.Follow;
        }
    }
}
