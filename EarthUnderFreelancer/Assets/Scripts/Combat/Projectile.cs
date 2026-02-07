using UnityEngine;
using EarthUnderFreelancer.Core;
using EarthUnderFreelancer.Vehicles;

namespace EarthUnderFreelancer.Combat
{
    /// <summary>
    /// Base projectile class for all weapon projectiles
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour, IPooledObject
    {
        [Header("Settings")]
        [SerializeField] private float speed = 200f;
        [SerializeField] private float damage = 10f;
        [SerializeField] private float lifetime = 3f;
        [SerializeField] private float range = 300f;
        [SerializeField] private DamageType damageType = DamageType.Energy;

        [Header("Effects")]
        [SerializeField] private TrailRenderer trail;
        [SerializeField] private ParticleSystem impactEffect;
        [SerializeField] private Light projectileLight;

        private Rigidbody rb;
        private GameObject owner;
        private Vector3 startPosition;
        private float lifeTimer;
        private bool isInitialized = false;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        public void Initialize(GameObject projectileOwner, float projectileDamage, float projectileSpeed, float projectileRange)
        {
            owner = projectileOwner;
            damage = projectileDamage;
            speed = projectileSpeed;
            range = projectileRange;
            startPosition = transform.position;
            lifeTimer = lifetime;
            isInitialized = true;

            rb.linearVelocity = transform.forward * speed;

            if (trail != null)
            {
                trail.Clear();
                trail.emitting = true;
            }
        }

        private void Update()
        {
            if (!isInitialized) return;

            lifeTimer -= Time.deltaTime;
            
            if (lifeTimer <= 0)
            {
                DestroyProjectile();
                return;
            }

            float distanceTraveled = Vector3.Distance(startPosition, transform.position);
            if (distanceTraveled > range)
            {
                DestroyProjectile();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!isInitialized) return;

            if (collision.gameObject == owner) return;
            if (owner != null && collision.transform.IsChildOf(owner.transform)) return;

            HealthSystem health = collision.gameObject.GetComponent<HealthSystem>();
            if (health != null)
            {
                ContactPoint contact = collision.contacts[0];
                health.TakeDamage(damage, owner, contact.point, damageType);
            }

            if (impactEffect != null)
            {
                ContactPoint contact = collision.contacts[0];
                ParticleSystem impact = Instantiate(impactEffect, contact.point, Quaternion.LookRotation(contact.normal));
                Destroy(impact.gameObject, 2f);
            }

            DestroyProjectile();
        }

        private void DestroyProjectile()
        {
            isInitialized = false;

            if (trail != null) trail.emitting = false;

            if (ObjectPool.Instance != null)
            {
                ObjectPool.Instance.ReturnToPool("projectile", gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void SetDamageType(DamageType type) => damageType = type;

        public void OnObjectSpawn()
        {
            isInitialized = false;
            lifeTimer = lifetime;
            if (trail != null) trail.Clear();
        }

        public void OnObjectDespawn()
        {
            isInitialized = false;
            rb.linearVelocity = Vector3.zero;
            if (trail != null) trail.emitting = false;
        }
    }
}
