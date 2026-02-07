using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Systems
{
    /// <summary>
    /// Generates procedural asteroid fields
    /// </summary>
    public class AsteroidFieldGenerator : MonoBehaviour
    {
        [Header("Field Settings")]
        [SerializeField] private Vector3 fieldCenter;
        [SerializeField] private float fieldRadius = 1000f;
        [SerializeField] private int asteroidCount = 100;

        [Header("Asteroid Settings")]
        [SerializeField] private float minSize = 1f;
        [SerializeField] private float maxSize = 20f;
        [SerializeField] private float minRotationSpeed = 0f;
        [SerializeField] private float maxRotationSpeed = 10f;

        [Header("Prefabs")]
        [SerializeField] private GameObject[] asteroidPrefabs;

        [Header("Resources")]
        [SerializeField] private bool containsResources = true;
        [SerializeField] private float resourceChance = 0.3f;
        [SerializeField] private string[] resourceTypes = { "iron_ore", "gold_ore", "platinum_ore", "ice_crystal" };

        private List<GameObject> spawnedAsteroids = new List<GameObject>();

        private void Start()
        {
            if (asteroidPrefabs == null || asteroidPrefabs.Length == 0)
            {
                CreateDefaultAsteroidPrefab();
            }

            GenerateField();
        }

        private void CreateDefaultAsteroidPrefab()
        {
            // Create a simple asteroid prefab if none assigned
            GameObject asteroid = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            asteroid.name = "DefaultAsteroid";
            asteroid.SetActive(false);

            // Add rigidbody for physics
            Rigidbody rb = asteroid.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.mass = 1000f;

            // Add asteroid component
            asteroid.AddComponent<Asteroid>();

            asteroidPrefabs = new GameObject[] { asteroid };
        }

        public void GenerateField()
        {
            ClearField();

            for (int i = 0; i < asteroidCount; i++)
            {
                SpawnAsteroid();
            }
        }

        private void SpawnAsteroid()
        {
            Vector3 position = fieldCenter + Random.insideUnitSphere * fieldRadius;
            Quaternion rotation = Random.rotation;

            GameObject prefab = asteroidPrefabs[Random.Range(0, asteroidPrefabs.Length)];
            GameObject asteroid = Instantiate(prefab, position, rotation, transform);
            asteroid.SetActive(true);

            // Random scale
            float scale = Random.Range(minSize, maxSize);
            asteroid.transform.localScale = Vector3.one * scale;

            // Setup asteroid component
            Asteroid asteroidComp = asteroid.GetComponent<Asteroid>();
            if (asteroidComp == null)
            {
                asteroidComp = asteroid.AddComponent<Asteroid>();
            }

            asteroidComp.SetRotationSpeed(Random.Range(minRotationSpeed, maxRotationSpeed));
            asteroidComp.SetRotationAxis(Random.insideUnitSphere.normalized);

            // Random resource
            if (containsResources && Random.value < resourceChance)
            {
                string resourceType = resourceTypes[Random.Range(0, resourceTypes.Length)];
                int resourceAmount = Random.Range(1, 10);
                asteroidComp.SetResource(resourceType, resourceAmount);
            }

            spawnedAsteroids.Add(asteroid);
        }

        public void ClearField()
        {
            foreach (var asteroid in spawnedAsteroids)
            {
                if (asteroid != null)
                {
                    Destroy(asteroid);
                }
            }
            spawnedAsteroids.Clear();
        }

        public void SetFieldParameters(Vector3 center, float radius, int count)
        {
            fieldCenter = center;
            fieldRadius = radius;
            asteroidCount = count;
        }
    }

    public class Asteroid : MonoBehaviour
    {
        [Header("Rotation")]
        [SerializeField] private float rotationSpeed;
        [SerializeField] private Vector3 rotationAxis = Vector3.up;

        [Header("Resources")]
        [SerializeField] private bool hasResources;
        [SerializeField] private string resourceType;
        [SerializeField] private int resourceAmount;

        [Header("Health")]
        [SerializeField] private float health = 100f;
        [SerializeField] private float maxHealth = 100f;

        public bool HasResources => hasResources;
        public string ResourceType => resourceType;
        public int ResourceAmount => resourceAmount;

        public event System.Action<Asteroid> OnDestroyed;
        public event System.Action<string, int> OnResourceMined;

        private void Update()
        {
            transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
        }

        public void SetRotationSpeed(float speed)
        {
            rotationSpeed = speed;
        }

        public void SetRotationAxis(Vector3 axis)
        {
            rotationAxis = axis;
        }

        public void SetResource(string type, int amount)
        {
            hasResources = true;
            resourceType = type;
            resourceAmount = amount;
        }

        public void TakeDamage(float damage)
        {
            health -= damage;

            if (health <= 0)
            {
                DestroyAsteroid();
            }
        }

        private void DestroyAsteroid()
        {
            // Drop resources
            if (hasResources && resourceAmount > 0)
            {
                OnResourceMined?.Invoke(resourceType, resourceAmount);

                // Add to inventory if player nearby
                if (InventoryManager.Instance != null)
                {
                    InventoryManager.Instance.AddItem(resourceType, resourceAmount);
                }
            }

            OnDestroyed?.Invoke(this);

            // Spawn explosion effect (placeholder)
            // Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }

        public int Mine()
        {
            if (!hasResources || resourceAmount <= 0) return 0;

            int mined = Mathf.Min(resourceAmount, 1);
            resourceAmount -= mined;

            OnResourceMined?.Invoke(resourceType, mined);

            if (resourceAmount <= 0)
            {
                hasResources = false;
            }

            return mined;
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Damage from high-speed collisions
            float impactForce = collision.impulse.magnitude;
            if (impactForce > 1000f)
            {
                TakeDamage(impactForce * 0.01f);
            }
        }
    }
}
