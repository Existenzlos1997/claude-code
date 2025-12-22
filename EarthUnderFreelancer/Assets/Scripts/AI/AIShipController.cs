using UnityEngine;
using System.Collections.Generic;
using EarthUnderFreelancer.Data;

namespace EarthUnderFreelancer.AI
{
    /// <summary>
    /// Main AI controller for enemy ships
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class AIShipController : MonoBehaviour
    {
        [Header("Ship Data")]
        [SerializeField] private ShipData shipData;
        [SerializeField] private string factionId = "Pirates";

        [Header("AI Settings")]
        [SerializeField] private AIBehavior currentBehavior = AIBehavior.Patrol;
        [SerializeField] private float detectionRange = 500f;
        [SerializeField] private float attackRange = 300f;
        [SerializeField] private float disengageRange = 800f;
        [SerializeField] private float patrolRadius = 200f;

        [Header("Combat")]
        [SerializeField] private float fireRate = 0.5f;
        [SerializeField] private float accuracy = 0.8f;
        [SerializeField] private float leadTargetFactor = 1f;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform[] firePoints;

        [Header("Movement")]
        [SerializeField] private float maxSpeed = 80f;
        [SerializeField] private float acceleration = 40f;
        [SerializeField] private float turnSpeed = 60f;
        [SerializeField] private float strafeSpeed = 20f;

        [Header("Health")]
        [SerializeField] private float maxHull = 100f;
        [SerializeField] private float currentHull;
        [SerializeField] private float maxShield = 50f;
        [SerializeField] private float currentShield;
        [SerializeField] private float shieldRegenRate = 5f;

        [Header("State")]
        [SerializeField] private Transform currentTarget;
        [SerializeField] private Vector3 patrolCenter;
        [SerializeField] private List<Vector3> patrolPoints = new List<Vector3>();
        [SerializeField] private int currentPatrolIndex = 0;

        [Header("Effects")]
        [SerializeField] private GameObject explosionPrefab;
        [SerializeField] private GameObject damageEffectPrefab;
        [SerializeField] private ParticleSystem[] engineParticles;

        private Rigidbody rb;
        private float lastFireTime;
        private float lastShieldDamageTime;
        private bool isDead = false;
        private Vector3 currentVelocity;
        private AIState currentState = AIState.Idle;

        // Events
        public System.Action<AIShipController> OnDestroyed;
        public System.Action<float, GameObject> OnDamaged;

        public string FactionId => factionId;
        public float CurrentHull => currentHull;
        public float MaxHull => maxHull;
        public bool IsDead => isDead;
        public Transform CurrentTarget => currentTarget;
        public AIBehavior Behavior => currentBehavior;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
        }

        private void Start()
        {
            InitializeFromData();
            patrolCenter = transform.position;
            GeneratePatrolPoints();
            currentHull = maxHull;
            currentShield = maxShield;
        }

        private void InitializeFromData()
        {
            if (shipData != null)
            {
                maxHull = shipData.maxHull;
                maxShield = shipData.maxShield;
                maxSpeed = shipData.maxSpeed;
                acceleration = shipData.acceleration;
                turnSpeed = shipData.turnRate;
                shieldRegenRate = shipData.shieldRegenRate;
            }
        }

        private void Update()
        {
            if (isDead) return;

            UpdateShieldRegen();
            UpdateAI();
        }

        private void FixedUpdate()
        {
            if (isDead) return;

            ExecuteCurrentBehavior();
        }

        private void UpdateShieldRegen()
        {
            if (Time.time - lastShieldDamageTime > 3f && currentShield < maxShield)
            {
                currentShield = Mathf.Min(currentShield + shieldRegenRate * Time.deltaTime, maxShield);
            }
        }

        private void UpdateAI()
        {
            // Find target if none
            if (currentTarget == null)
            {
                FindTarget();
            }
            else
            {
                // Check if target is still valid
                float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);
                
                if (distanceToTarget > disengageRange)
                {
                    currentTarget = null;
                    currentBehavior = AIBehavior.Patrol;
                    currentState = AIState.Patrolling;
                }
            }

            // Update behavior based on state
            UpdateBehavior();
        }

        private void UpdateBehavior()
        {
            if (currentTarget != null)
            {
                float distance = Vector3.Distance(transform.position, currentTarget.position);

                if (distance <= attackRange)
                {
                    currentBehavior = AIBehavior.Attack;
                    currentState = AIState.Attacking;
                }
                else if (distance <= detectionRange)
                {
                    currentBehavior = AIBehavior.Chase;
                    currentState = AIState.Chasing;
                }
            }
            else
            {
                currentBehavior = AIBehavior.Patrol;
                currentState = AIState.Patrolling;
            }
        }

        private void ExecuteCurrentBehavior()
        {
            switch (currentBehavior)
            {
                case AIBehavior.Idle:
                    ExecuteIdle();
                    break;
                case AIBehavior.Patrol:
                    ExecutePatrol();
                    break;
                case AIBehavior.Chase:
                    ExecuteChase();
                    break;
                case AIBehavior.Attack:
                    ExecuteAttack();
                    break;
                case AIBehavior.Flee:
                    ExecuteFlee();
                    break;
                case AIBehavior.Evade:
                    ExecuteEvade();
                    break;
            }
        }

        private void ExecuteIdle()
        {
            // Slow down
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, Time.fixedDeltaTime);
        }

        private void ExecutePatrol()
        {
            if (patrolPoints.Count == 0)
            {
                GeneratePatrolPoints();
                return;
            }

            Vector3 targetPoint = patrolPoints[currentPatrolIndex];
            float distance = Vector3.Distance(transform.position, targetPoint);

            if (distance < 20f)
            {
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
            }

            MoveTowards(targetPoint, maxSpeed * 0.5f);
        }

        private void ExecuteChase()
        {
            if (currentTarget == null) return;

            MoveTowards(currentTarget.position, maxSpeed);
            RotateTowards(currentTarget.position);
        }

        private void ExecuteAttack()
        {
            if (currentTarget == null) return;

            float distance = Vector3.Distance(transform.position, currentTarget.position);
            
            // Calculate lead position
            Vector3 targetPosition = CalculateLeadPosition();
            
            // Rotate towards target
            RotateTowards(targetPosition);

            // Strafe movement during combat
            Vector3 strafeDir = Vector3.Cross(transform.forward, Vector3.up).normalized;
            float strafeAmount = Mathf.Sin(Time.time * 2f) * strafeSpeed;
            
            // Maintain optimal attack distance
            float optimalDistance = attackRange * 0.7f;
            Vector3 moveDir = Vector3.zero;

            if (distance > optimalDistance + 30f)
            {
                moveDir = (currentTarget.position - transform.position).normalized;
            }
            else if (distance < optimalDistance - 30f)
            {
                moveDir = -(currentTarget.position - transform.position).normalized;
            }

            Vector3 targetVelocity = (moveDir * maxSpeed * 0.7f) + (strafeDir * strafeAmount);
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);

            // Fire at target
            TryFire();
        }

        private void ExecuteFlee()
        {
            if (currentTarget == null) return;

            Vector3 fleeDirection = (transform.position - currentTarget.position).normalized;
            Vector3 fleeTarget = transform.position + fleeDirection * 100f;
            
            MoveTowards(fleeTarget, maxSpeed);
        }

        private void ExecuteEvade()
        {
            // Random evasive maneuvers
            Vector3 randomDir = new Vector3(
                Mathf.Sin(Time.time * 3f),
                Mathf.Cos(Time.time * 2.5f),
                Mathf.Sin(Time.time * 2f)
            ).normalized;

            rb.AddForce(randomDir * acceleration * 2f, ForceMode.Force);
        }

        private void MoveTowards(Vector3 target, float speed)
        {
            Vector3 direction = (target - transform.position).normalized;
            Vector3 targetVelocity = direction * speed;
            
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, targetVelocity, acceleration * Time.fixedDeltaTime * 0.1f);
            
            RotateTowards(target);
        }

        private void RotateTowards(Vector3 target)
        {
            Vector3 direction = (target - transform.position).normalized;
            if (direction == Vector3.zero) return;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                turnSpeed * Time.fixedDeltaTime
            );
        }

        private Vector3 CalculateLeadPosition()
        {
            if (currentTarget == null) return transform.position + transform.forward * 100f;

            Rigidbody targetRb = currentTarget.GetComponent<Rigidbody>();
            if (targetRb == null) return currentTarget.position;

            float projectileSpeed = 300f; // Default projectile speed
            float distance = Vector3.Distance(transform.position, currentTarget.position);
            float timeToTarget = distance / projectileSpeed;

            Vector3 leadPosition = currentTarget.position + targetRb.linearVelocity * timeToTarget * leadTargetFactor;
            
            // Add some inaccuracy
            float inaccuracyAmount = (1f - accuracy) * 10f;
            leadPosition += UnityEngine.Random.insideUnitSphere * inaccuracyAmount;

            return leadPosition;
        }

        private void TryFire()
        {
            if (Time.time - lastFireTime < fireRate) return;
            if (currentTarget == null) return;

            // Check if target is in front
            Vector3 targetDir = (currentTarget.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, targetDir);
            
            if (angle > 30f) return;

            lastFireTime = Time.time;
            Fire();
        }

        private void Fire()
        {
            if (firePoints == null || firePoints.Length == 0)
            {
                // Fire from center if no fire points
                SpawnProjectile(transform.position + transform.forward, transform.rotation);
            }
            else
            {
                foreach (var firePoint in firePoints)
                {
                    if (firePoint != null)
                    {
                        SpawnProjectile(firePoint.position, firePoint.rotation);
                    }
                }
            }
        }

        private void SpawnProjectile(Vector3 position, Quaternion rotation)
        {
            if (projectilePrefab == null) return;

            GameObject proj = Instantiate(projectilePrefab, position, rotation);
            
            AIProjectile aiProj = proj.GetComponent<AIProjectile>();
            if (aiProj != null)
            {
                aiProj.Initialize(gameObject, 10f, 300f);
            }
            else
            {
                Rigidbody projRb = proj.GetComponent<Rigidbody>();
                if (projRb != null)
                {
                    projRb.linearVelocity = rotation * Vector3.forward * 300f;
                }
                Destroy(proj, 3f);
            }
        }

        private void FindTarget()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, detectionRange);
            
            float closestDistance = float.MaxValue;
            Transform closestTarget = null;

            foreach (var hit in hits)
            {
                // Skip self
                if (hit.transform == transform) continue;

                // Check for player
                if (hit.CompareTag("Player"))
                {
                    float dist = Vector3.Distance(transform.position, hit.transform.position);
                    if (dist < closestDistance)
                    {
                        closestDistance = dist;
                        closestTarget = hit.transform;
                    }
                }

                // Check for other factions
                AIShipController otherAI = hit.GetComponent<AIShipController>();
                if (otherAI != null && otherAI.FactionId != factionId)
                {
                    // Check faction relations (simplified - treat different factions as hostile)
                    float dist = Vector3.Distance(transform.position, hit.transform.position);
                    if (dist < closestDistance)
                    {
                        closestDistance = dist;
                        closestTarget = hit.transform;
                    }
                }
            }

            if (closestTarget != null)
            {
                currentTarget = closestTarget;
                currentBehavior = AIBehavior.Chase;
            }
        }

        private void GeneratePatrolPoints()
        {
            patrolPoints.Clear();
            int pointCount = UnityEngine.Random.Range(3, 6);

            for (int i = 0; i < pointCount; i++)
            {
                Vector3 randomPoint = patrolCenter + UnityEngine.Random.insideUnitSphere * patrolRadius;
                patrolPoints.Add(randomPoint);
            }
        }

        public void TakeDamage(float damage, GameObject source = null, Vector3 hitPoint = default)
        {
            if (isDead) return;

            // Shield absorbs damage first
            if (currentShield > 0)
            {
                float shieldDamage = Mathf.Min(currentShield, damage);
                currentShield -= shieldDamage;
                damage -= shieldDamage;
                lastShieldDamageTime = Time.time;
            }

            // Remaining damage goes to hull
            if (damage > 0)
            {
                currentHull -= damage;

                // Spawn damage effect
                if (damageEffectPrefab != null && hitPoint != default)
                {
                    Instantiate(damageEffectPrefab, hitPoint, Quaternion.identity);
                }
            }

            OnDamaged?.Invoke(damage, source);

            // React to damage
            if (source != null && currentTarget == null)
            {
                currentTarget = source.transform;
                currentBehavior = AIBehavior.Attack;
            }

            // Flee if low health
            if (currentHull < maxHull * 0.2f)
            {
                currentBehavior = AIBehavior.Flee;
            }

            // Check for death
            if (currentHull <= 0)
            {
                Die(source);
            }
        }

        private void Die(GameObject killer = null)
        {
            if (isDead) return;
            isDead = true;

            // Spawn explosion
            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            }

            OnDestroyed?.Invoke(this);

            // Drop loot (implement in loot system)
            
            Destroy(gameObject, 0.1f);
        }

        public void SetTarget(Transform target)
        {
            currentTarget = target;
            if (target != null)
            {
                currentBehavior = AIBehavior.Chase;
            }
        }

        public void SetBehavior(AIBehavior behavior)
        {
            currentBehavior = behavior;
        }

        public void SetPatrolCenter(Vector3 center)
        {
            patrolCenter = center;
            GeneratePatrolPoints();
        }

        private void OnDrawGizmosSelected()
        {
            // Detection range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);

            // Attack range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);

            // Patrol points
            Gizmos.color = Color.blue;
            foreach (var point in patrolPoints)
            {
                Gizmos.DrawSphere(point, 5f);
            }
        }
    }

    public enum AIBehavior
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Flee,
        Evade,
        Dock,
        Escort,
        Formation
    }

    public enum AIState
    {
        Idle,
        Patrolling,
        Chasing,
        Attacking,
        Fleeing,
        Evading,
        Docking
    }
}
