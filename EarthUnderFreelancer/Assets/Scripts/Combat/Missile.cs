using UnityEngine;
using EarthUnderFreelancer.Core;
using EarthUnderFreelancer.Vehicles;

namespace EarthUnderFreelancer.Combat
{
    /// <summary>
    /// Guided missile that tracks a target
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class Missile : MonoBehaviour
    {
        [Header("Flight Settings")]
        [SerializeField] private float speed = 80f;
        [SerializeField] private float turnSpeed = 90f;
        [SerializeField] private float acceleration = 20f;
        [SerializeField] private float armingDistance = 20f;

        [Header("Damage")]
        [SerializeField] private float damage = 100f;
        [SerializeField] private float explosionRadius = 15f;
        [SerializeField] private LayerMask damageLayers;

        [Header("Tracking")]
        [SerializeField] private float trackingRange = 500f;
        [SerializeField] private float trackingCone = 60f;
        [SerializeField] private float loseTrackTime = 3f;

        [Header("Lifetime")]
        [SerializeField] private float lifetime = 10f;
        [SerializeField] private float fuel = 8f;

        [Header("Effects")]
        [SerializeField] private ParticleSystem thrusterEffect;
        [SerializeField] private ParticleSystem smokeTrail;
        [SerializeField] private GameObject explosionPrefab;
        [SerializeField] private Light missileLight;

        private Rigidbody rb;
        private Transform target;
        private GameObject owner;
        private Vector3 startPosition;
        
        private float currentSpeed;
        private float lifeTimer;
        private float lostTrackTimer;
        private bool isArmed = false;
        private bool hasTarget = false;
        private bool isActive = true;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        public void Initialize(Transform missileTarget, GameObject missileOwner, float missileDamage)
        {
            target = missileTarget;
            owner = missileOwner;
            damage = missileDamage;
            startPosition = transform.position;
            lifeTimer = lifetime;
            currentSpeed = speed * 0.5f;
            hasTarget = target != null;
            isActive = true;

            if (thrusterEffect != null) thrusterEffect.Play();
            if (smokeTrail != null) smokeTrail.Play();
        }

        private void FixedUpdate()
        {
            if (!isActive) return;

            lifeTimer -= Time.fixedDeltaTime;
            if (lifeTimer <= 0)
            {
                Explode();
                return;
            }

            float distanceFromStart = Vector3.Distance(startPosition, transform.position);
            if (!isArmed && distanceFromStart > armingDistance)
            {
                isArmed = true;
            }

            currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.fixedDeltaTime, speed);

            if (hasTarget && target != null)
            {
                TrackTarget();
            }
            else
            {
                rb.linearVelocity = transform.forward * currentSpeed;
            }

            fuel -= Time.fixedDeltaTime;
            if (fuel <= 0)
            {
                hasTarget = false;
                if (thrusterEffect != null) thrusterEffect.Stop();
            }
        }

        private void TrackTarget()
        {
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

            if (distanceToTarget <= trackingRange && angleToTarget <= trackingCone)
            {
                lostTrackTimer = 0f;

                Rigidbody targetRb = target.GetComponent<Rigidbody>();
                Vector3 targetPoint = target.position;
                
                if (targetRb != null)
                {
                    float timeToTarget = distanceToTarget / currentSpeed;
                    targetPoint = target.position + targetRb.linearVelocity * timeToTarget * 0.5f;
                }

                Vector3 targetDirection = (targetPoint - transform.position).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    turnSpeed * Time.fixedDeltaTime
                );
            }
            else
            {
                lostTrackTimer += Time.fixedDeltaTime;
                if (lostTrackTimer >= loseTrackTime)
                {
                    hasTarget = false;
                }
            }

            rb.linearVelocity = transform.forward * currentSpeed;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!isActive) return;

            if (collision.gameObject == owner) return;
            if (owner != null && collision.transform.IsChildOf(owner.transform)) return;

            if (isArmed) Explode();
        }

        private void Explode()
        {
            isActive = false;

            if (explosionPrefab != null)
            {
                GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                Destroy(explosion, 3f);
            }

            Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius, damageLayers);
            foreach (var hit in hits)
            {
                if (hit.gameObject == owner) continue;

                HealthSystem health = hit.GetComponent<HealthSystem>();
                if (health != null)
                {
                    float distance = Vector3.Distance(transform.position, hit.transform.position);
                    float damageMultiplier = 1f - (distance / explosionRadius);
                    float actualDamage = damage * Mathf.Max(damageMultiplier, 0.2f);

                    health.TakeDamage(actualDamage, owner, transform.position, DamageType.Explosive);
                }

                Rigidbody hitRb = hit.GetComponent<Rigidbody>();
                if (hitRb != null)
                {
                    hitRb.AddExplosionForce(damage * 10f, transform.position, explosionRadius);
                }
            }

            AudioManager.Instance?.PlayExplosion();
            Destroy(gameObject);
        }

        public void SetTarget(Transform newTarget) { target = newTarget; hasTarget = target != null; lostTrackTimer = 0f; }
        public void Abort() { hasTarget = false; if (thrusterEffect != null) thrusterEffect.Stop(); }
    }
}
