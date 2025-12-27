using UnityEngine;

namespace EarthUnderFreelancer.AI
{
    /// <summary>
    /// Projectile fired by AI ships
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class AIProjectile : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float speed = 300f;
        [SerializeField] private float damage = 10f;
        [SerializeField] private float lifetime = 3f;
        [SerializeField] private float range = 500f;

        [Header("Effects")]
        [SerializeField] private TrailRenderer trail;
        [SerializeField] private GameObject impactEffect;
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

        public void Initialize(GameObject projectileOwner, float projectileDamage, float projectileSpeed)
        {
            owner = projectileOwner;
            damage = projectileDamage;
            speed = projectileSpeed;
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

            if (lifeTimer <= 0 || Vector3.Distance(startPosition, transform.position) > range)
            {
                DestroyProjectile();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!isInitialized) return;
            if (collision.gameObject == owner) return;
            if (owner != null && collision.transform.IsChildOf(owner.transform)) return;

            // Try to deal damage to player
            var playerHealth = collision.gameObject.GetComponent<EarthUnderFreelancer.Systems.PlayerShipController>();
            if (playerHealth != null)
            {
                ContactPoint contact = collision.contacts[0];
                playerHealth.TakeDamage(damage, owner, contact.point);
            }

            // Try to deal damage to other AI
            var aiShip = collision.gameObject.GetComponent<AIShipController>();
            if (aiShip != null)
            {
                ContactPoint contact = collision.contacts[0];
                aiShip.TakeDamage(damage, owner, contact.point);
            }

            // Spawn impact effect
            if (impactEffect != null)
            {
                ContactPoint contact = collision.contacts[0];
                Instantiate(impactEffect, contact.point, Quaternion.LookRotation(contact.normal));
            }

            DestroyProjectile();
        }

        private void DestroyProjectile()
        {
            isInitialized = false;
            if (trail != null) trail.emitting = false;
            Destroy(gameObject);
        }
    }
}
