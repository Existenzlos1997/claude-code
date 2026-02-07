using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Combat
{
    /// <summary>
    /// Advanced targeting system with lead calculation and target prioritization
    /// </summary>
    public class AdvancedTargetingSystem : MonoBehaviour
    {
        [Header("Targeting Settings")]
        [SerializeField] private float maxTargetRange = 5000f;
        [SerializeField] private float targetSwitchCooldown = 0.5f;
        [SerializeField] private LayerMask targetLayers;
        
        [Header("Lead Calculation")]
        [SerializeField] private bool showLeadIndicator = true;
        [SerializeField] private float projectileSpeed = 500f;
        
        [Header("Target Prioritization")]
        [SerializeField] private TargetPriority priorityMode = TargetPriority.Nearest;
        
        public enum TargetPriority
        {
            Nearest,
            MostDangerous,
            Weakest,
            Manual
        }
        
        private Transform currentTarget;
        private List<Transform> potentialTargets = new List<Transform>();
        private float lastTargetSwitch = 0f;
        private Vector3 leadPosition;
        
        private void Update()
        {
            ScanForTargets();
            
            if (Input.GetKeyDown(KeyCode.Tab) && Time.time > lastTargetSwitch + targetSwitchCooldown)
            {
                CycleTarget();
                lastTargetSwitch = Time.time;
            }
            
            if (currentTarget != null)
            {
                CalculateLeadPosition();
            }
        }
        
        private void ScanForTargets()
        {
            potentialTargets.Clear();
            
            Collider[] hits = Physics.OverlapSphere(transform.position, maxTargetRange, targetLayers);
            
            foreach (Collider hit in hits)
            {
                if (hit.transform != transform && hit.CompareTag("Enemy"))
                {
                    potentialTargets.Add(hit.transform);
                }
            }
        }
        
        private void CycleTarget()
        {
            if (potentialTargets.Count == 0)
            {
                currentTarget = null;
                return;
            }
            
            // Find current target index
            int currentIndex = potentialTargets.IndexOf(currentTarget);
            int nextIndex = (currentIndex + 1) % potentialTargets.Count;
            
            currentTarget = potentialTargets[nextIndex];
            Debug.Log($"[Targeting] Locked: {currentTarget.name}");
        }
        
        public void SetTarget(Transform target)
        {
            currentTarget = target;
        }
        
        public Transform GetBestTarget()
        {
            if (potentialTargets.Count == 0) return null;
            
            switch (priorityMode)
            {
                case TargetPriority.Nearest:
                    return GetNearestTarget();
                    
                case TargetPriority.MostDangerous:
                    return GetMostDangerousTarget();
                    
                case TargetPriority.Weakest:
                    return GetWeakestTarget();
                    
                default:
                    return currentTarget;
            }
        }
        
        private Transform GetNearestTarget()
        {
            Transform nearest = null;
            float minDistance = float.MaxValue;
            
            foreach (Transform target in potentialTargets)
            {
                float distance = Vector3.Distance(transform.position, target.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = target;
                }
            }
            
            return nearest;
        }
        
        private Transform GetMostDangerousTarget()
        {
            // Would check threat level, weapons, etc.
            // For now, return nearest
            return GetNearestTarget();
        }
        
        private Transform GetWeakestTarget()
        {
            // Would check health/shields
            // For now, return nearest
            return GetNearestTarget();
        }
        
        private void CalculateLeadPosition()
        {
            if (currentTarget == null) return;
            
            // Get target velocity
            Rigidbody targetRb = currentTarget.GetComponent<Rigidbody>();
            Vector3 targetVelocity = targetRb != null ? targetRb.velocity : Vector3.zero;
            
            // Calculate interception point
            Vector3 toTarget = currentTarget.position - transform.position;
            float distance = toTarget.magnitude;
            float timeToHit = distance / projectileSpeed;
            
            leadPosition = currentTarget.position + targetVelocity * timeToHit;
        }
        
        public Vector3 GetLeadPosition()
        {
            return leadPosition;
        }
        
        public Transform GetCurrentTarget()
        {
            return currentTarget;
        }
        
        public float GetTargetDistance()
        {
            if (currentTarget == null) return 0f;
            return Vector3.Distance(transform.position, currentTarget.position);
        }
        
        public bool HasTarget()
        {
            return currentTarget != null;
        }
        
        private void OnDrawGizmos()
        {
            if (currentTarget != null)
            {
                // Draw line to target
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, currentTarget.position);
                
                // Draw lead indicator
                if (showLeadIndicator)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere(leadPosition, 10f);
                    Gizmos.DrawLine(transform.position, leadPosition);
                }
            }
            
            // Draw targeting range
            Gizmos.color = new Color(1f, 0f, 0f, 0.1f);
            Gizmos.DrawWireSphere(transform.position, maxTargetRange);
        }
    }
}
