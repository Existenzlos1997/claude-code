using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Combat
{
    /// <summary>
    /// Advanced targeting system with lead indicators
    /// </summary>
    public class TargetingSystem : MonoBehaviour
    {
        public static TargetingSystem Instance { get; private set; }

        [Header("Targeting Settings")]
        [SerializeField] private float maxTargetingRange = 1000f;
        [SerializeField] private float autoTargetRange = 500f;
        [SerializeField] private float lockOnCone = 30f;
        [SerializeField] private LayerMask targetableLayers;

        [Header("Lead Indicator")]
        [SerializeField] private float projectileSpeed = 300f;
        [SerializeField] private bool showLeadIndicator = true;

        [Header("Current State")]
        [SerializeField] private Transform currentTarget;
        [SerializeField] private List<TargetInfo> allTargets = new List<TargetInfo>();
        [SerializeField] private int currentTargetIndex = -1;

        // Lead indicator calculation
        private Vector3 leadPosition;
        private bool hasValidLead;
        private float distanceToTarget;
        private float timeToTarget;

        public Transform CurrentTarget => currentTarget;
        public Vector3 LeadPosition => leadPosition;
        public bool HasValidLead => hasValidLead;
        public float DistanceToTarget => distanceToTarget;
        public List<TargetInfo> AllTargets => allTargets;

        public event System.Action<Transform> OnTargetChanged;
        public event System.Action<Transform> OnTargetLost;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Update()
        {
            ScanForTargets();
            UpdateLeadIndicator();
            ValidateCurrentTarget();
        }

        private void ScanForTargets()
        {
            allTargets.Clear();

            Collider[] hits = Physics.OverlapSphere(transform.position, maxTargetingRange, targetableLayers);

            foreach (var hit in hits)
            {
                if (hit.transform == transform) continue;

                // Check if it's a valid target
                if (IsValidTarget(hit.transform))
                {
                    TargetInfo info = new TargetInfo
                    {
                        target = hit.transform,
                        distance = Vector3.Distance(transform.position, hit.transform.position),
                        isHostile = IsHostile(hit.transform),
                        healthPercent = GetHealthPercent(hit.transform)
                    };

                    // Calculate angle to target
                    Vector3 dirToTarget = (hit.transform.position - transform.position).normalized;
                    info.angleFromForward = Vector3.Angle(transform.forward, dirToTarget);

                    allTargets.Add(info);
                }
            }

            // Sort by distance
            allTargets.Sort((a, b) => a.distance.CompareTo(b.distance));
        }

        private bool IsValidTarget(Transform target)
        {
            // Check for AI ships
            var aiShip = target.GetComponent<AI.AIShipController>();
            if (aiShip != null) return true;

            // Check for player (for AI targeting)
            if (target.CompareTag("Player")) return true;

            // Check for destructible objects
            var health = target.GetComponent<Vehicles.HealthSystem>();
            if (health != null) return true;

            return false;
        }

        private bool IsHostile(Transform target)
        {
            // Check for AI ship faction
            var aiShip = target.GetComponent<AI.AIShipController>();
            if (aiShip != null)
            {
                if (Systems.FactionManager.Instance != null)
                {
                    return Systems.FactionManager.Instance.IsHostile(aiShip.FactionId);
                }
                return true; // Default to hostile if no faction system
            }

            // AI targeting player is always hostile
            if (target.CompareTag("Player") && !gameObject.CompareTag("Player"))
            {
                return true;
            }

            return false;
        }

        private float GetHealthPercent(Transform target)
        {
            var health = target.GetComponent<Vehicles.HealthSystem>();
            if (health != null)
            {
                return health.CurrentHealth / health.MaxHealth;
            }

            var aiShip = target.GetComponent<AI.AIShipController>();
            if (aiShip != null)
            {
                return aiShip.CurrentHull / aiShip.MaxHull;
            }

            return 1f;
        }

        private void ValidateCurrentTarget()
        {
            if (currentTarget == null) return;

            // Check if target is destroyed
            if (currentTarget.gameObject == null)
            {
                ClearTarget();
                return;
            }

            // Check if target is out of range
            distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);
            if (distanceToTarget > maxTargetingRange)
            {
                OnTargetLost?.Invoke(currentTarget);
                ClearTarget();
            }
        }

        private void UpdateLeadIndicator()
        {
            if (currentTarget == null)
            {
                hasValidLead = false;
                return;
            }

            // Get target velocity
            Rigidbody targetRb = currentTarget.GetComponent<Rigidbody>();
            Vector3 targetVelocity = targetRb != null ? targetRb.linearVelocity : Vector3.zero;

            // Get our velocity
            Rigidbody ourRb = GetComponent<Rigidbody>();
            Vector3 ourVelocity = ourRb != null ? ourRb.linearVelocity : Vector3.zero;

            // Calculate relative velocity
            Vector3 relativeVelocity = targetVelocity - ourVelocity;

            // Calculate time to intercept
            distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);
            timeToTarget = distanceToTarget / projectileSpeed;

            // Calculate lead position using iterative method for accuracy
            leadPosition = CalculateLeadPosition(currentTarget.position, relativeVelocity, projectileSpeed);
            hasValidLead = true;
        }

        private Vector3 CalculateLeadPosition(Vector3 targetPos, Vector3 targetVel, float projectileSpeed)
        {
            Vector3 displacement = targetPos - transform.position;
            float a = targetVel.sqrMagnitude - projectileSpeed * projectileSpeed;
            float b = 2f * Vector3.Dot(displacement, targetVel);
            float c = displacement.sqrMagnitude;

            // Quadratic formula
            float discriminant = b * b - 4f * a * c;

            if (discriminant < 0f || Mathf.Abs(a) < 0.0001f)
            {
                // No solution, return direct position
                return targetPos;
            }

            float t1 = (-b - Mathf.Sqrt(discriminant)) / (2f * a);
            float t2 = (-b + Mathf.Sqrt(discriminant)) / (2f * a);

            float t = t1 > 0f ? t1 : t2;

            if (t < 0f)
            {
                return targetPos;
            }

            timeToTarget = t;
            return targetPos + targetVel * t;
        }

        public void CycleTarget()
        {
            if (allTargets.Count == 0)
            {
                ClearTarget();
                return;
            }

            currentTargetIndex = (currentTargetIndex + 1) % allTargets.Count;
            SetTarget(allTargets[currentTargetIndex].target);
        }

        public void CycleHostileTarget()
        {
            List<TargetInfo> hostiles = allTargets.FindAll(t => t.isHostile);
            if (hostiles.Count == 0)
            {
                ClearTarget();
                return;
            }

            int index = 0;
            if (currentTarget != null)
            {
                for (int i = 0; i < hostiles.Count; i++)
                {
                    if (hostiles[i].target == currentTarget)
                    {
                        index = (i + 1) % hostiles.Count;
                        break;
                    }
                }
            }

            SetTarget(hostiles[index].target);
        }

        public void TargetNearest()
        {
            if (allTargets.Count == 0)
            {
                ClearTarget();
                return;
            }

            SetTarget(allTargets[0].target);
        }

        public void TargetNearestHostile()
        {
            TargetInfo nearest = allTargets.Find(t => t.isHostile);
            if (nearest.target != null)
            {
                SetTarget(nearest.target);
            }
        }

        public void SetTarget(Transform target)
        {
            if (currentTarget != target)
            {
                currentTarget = target;
                OnTargetChanged?.Invoke(target);
            }
        }

        public void ClearTarget()
        {
            currentTarget = null;
            currentTargetIndex = -1;
            hasValidLead = false;
        }

        public void SetProjectileSpeed(float speed)
        {
            projectileSpeed = speed;
        }

        public Vector3 GetAimPoint()
        {
            if (hasValidLead)
            {
                return leadPosition;
            }
            else if (currentTarget != null)
            {
                return currentTarget.position;
            }
            return transform.position + transform.forward * 1000f;
        }

        public bool IsTargetInCone()
        {
            if (currentTarget == null) return false;

            Vector3 dirToTarget = (currentTarget.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dirToTarget);
            return angle <= lockOnCone;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, maxTargetingRange);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, autoTargetRange);

            if (hasValidLead)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(leadPosition, 2f);
                Gizmos.DrawLine(transform.position, leadPosition);
            }
        }
    }

    [System.Serializable]
    public struct TargetInfo
    {
        public Transform target;
        public float distance;
        public float angleFromForward;
        public bool isHostile;
        public float healthPercent;
    }
}
