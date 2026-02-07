using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Gameplay
{
    /// <summary>
    /// Waypoint navigation system for missions and autopilot
    /// Implements VERBESSERUNGSPLAN navigation improvements
    /// </summary>
    public class WaypointNavigationSystem : MonoBehaviour
    {
        [Header("Waypoints")]
        [SerializeField] private List<Transform> waypoints = new List<Transform>();
        [SerializeField] private bool loopWaypoints = false;
        [SerializeField] private bool autoAdvance = true;
        
        [Header("Navigation Settings")]
        [SerializeField] private float waypointReachDistance = 50f;
        [SerializeField] private bool showWaypointMarkers = true;
        [SerializeField] private Color waypointColor = Color.cyan;
        
        [Header("Autopilot")]
        [SerializeField] private bool autopilotEnabled = false;
        [SerializeField] private float autopilotSpeed = 100f;
        [SerializeField] private float turnSpeed = 2f;
        
        private int currentWaypointIndex = 0;
        private Transform currentWaypoint;
        private float distanceToWaypoint = 0f;
        
        private void Start()
        {
            if (waypoints.Count > 0)
            {
                SetCurrentWaypoint(0);
            }
        }
        
        private void Update()
        {
            if (currentWaypoint == null) return;
            
            UpdateNavigation();
            
            if (autopilotEnabled)
            {
                FlyToWaypoint();
            }
            
            if (autoAdvance)
            {
                CheckWaypointReached();
            }
        }
        
        private void UpdateNavigation()
        {
            distanceToWaypoint = Vector3.Distance(transform.position, currentWaypoint.position);
        }
        
        private void FlyToWaypoint()
        {
            if (currentWaypoint == null) return;
            
            // Calculate direction to waypoint
            Vector3 direction = (currentWaypoint.position - transform.position).normalized;
            
            // Rotate toward waypoint
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            
            // Move forward
            transform.position += transform.forward * autopilotSpeed * Time.deltaTime;
        }
        
        private void CheckWaypointReached()
        {
            if (distanceToWaypoint <= waypointReachDistance)
            {
                OnWaypointReached();
                AdvanceToNextWaypoint();
            }
        }
        
        private void AdvanceToNextWaypoint()
        {
            int nextIndex = currentWaypointIndex + 1;
            
            if (nextIndex >= waypoints.Count)
            {
                if (loopWaypoints)
                {
                    nextIndex = 0;
                }
                else
                {
                    OnAllWaypointsComplete();
                    return;
                }
            }
            
            SetCurrentWaypoint(nextIndex);
        }
        
        private void SetCurrentWaypoint(int index)
        {
            if (index < 0 || index >= waypoints.Count) return;
            
            currentWaypointIndex = index;
            currentWaypoint = waypoints[index];
            
            Debug.Log($"[Navigation] Waypoint {index + 1}/{waypoints.Count}: {currentWaypoint.name}");
        }
        
        private void OnWaypointReached()
        {
            Debug.Log($"[Navigation] Waypoint {currentWaypointIndex + 1} reached!");
        }
        
        private void OnAllWaypointsComplete()
        {
            Debug.Log("[Navigation] All waypoints complete!");
            autopilotEnabled = false;
        }
        
        public void AddWaypoint(Transform waypoint)
        {
            waypoints.Add(waypoint);
            if (currentWaypoint == null)
            {
                SetCurrentWaypoint(0);
            }
        }
        
        public void ClearWaypoints()
        {
            waypoints.Clear();
            currentWaypoint = null;
            currentWaypointIndex = 0;
        }
        
        public void EnableAutopilot()
        {
            autopilotEnabled = true;
        }
        
        public void DisableAutopilot()
        {
            autopilotEnabled = false;
        }
        
        private void OnDrawGizmos()
        {
            if (!showWaypointMarkers || waypoints.Count == 0) return;
            
            Gizmos.color = waypointColor;
            
            // Draw waypoint spheres
            foreach (Transform waypoint in waypoints)
            {
                if (waypoint != null)
                {
                    Gizmos.DrawWireSphere(waypoint.position, waypointReachDistance);
                }
            }
            
            // Draw path lines
            for (int i = 0; i < waypoints.Count - 1; i++)
            {
                if (waypoints[i] != null && waypoints[i + 1] != null)
                {
                    Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                }
            }
            
            // Draw loop line if enabled
            if (loopWaypoints && waypoints.Count > 1)
            {
                if (waypoints[waypoints.Count - 1] != null && waypoints[0] != null)
                {
                    Gizmos.DrawLine(waypoints[waypoints.Count - 1].position, waypoints[0].position);
                }
            }
            
            // Highlight current waypoint
            if (currentWaypoint != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(currentWaypoint.position, waypointReachDistance * 1.2f);
            }
        }
        
        // Public API
        public Transform GetCurrentWaypoint() => currentWaypoint;
        public int GetCurrentWaypointIndex() => currentWaypointIndex;
        public float GetDistanceToWaypoint() => distanceToWaypoint;
        public int GetTotalWaypoints() => waypoints.Count;
        public bool IsAutopilotEnabled() => autopilotEnabled;
    }
}
