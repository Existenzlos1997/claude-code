using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Systems
{
    /// <summary>
    /// Space environment effects system (nebulae, asteroid fields, hazards)
    /// </summary>
    public class SpaceEnvironmentSystem : MonoBehaviour
    {
        public static SpaceEnvironmentSystem Instance { get; private set; }

        [Header("Current Environment")]
        [SerializeField] private SpaceEnvironmentType currentEnvironment = SpaceEnvironmentType.OpenSpace;
        [SerializeField] private float environmentDensity = 1f;

        [Header("Nebula Settings")]
        [SerializeField] private Color nebulaColor = new Color(0.5f, 0.2f, 0.8f, 0.3f);
        [SerializeField] private float nebulaSpeedReduction = 0.2f;
        [SerializeField] private float nebulaSensorRange = 0.5f;
        [SerializeField] private float nebulaShieldRegen = -0.5f;

        [Header("Asteroid Field Settings")]
        [SerializeField] private float asteroidDensity = 0.5f;
        [SerializeField] private float asteroidCollisionDamage = 10f;

        [Header("Radiation Zone Settings")]
        [SerializeField] private float radiationDamagePerSecond = 5f;
        [SerializeField] private Color radiationWarningColor = Color.yellow;

        [Header("Effects")]
        [SerializeField] private ParticleSystem nebulaParticles;
        [SerializeField] private ParticleSystem dustParticles;
        [SerializeField] private ParticleSystem radiationParticles;
        [SerializeField] private Light ambientLight;

        [Header("Skybox")]
        [SerializeField] private Material defaultSkybox;
        [SerializeField] private Material nebulaSkybox;
        [SerializeField] private Material asteroidFieldSkybox;

        [Header("Zone Detection")]
        [SerializeField] private List<EnvironmentZone> environmentZones = new List<EnvironmentZone>();

        private Transform playerTransform;
        private Vehicles.HealthSystem playerHealth;

        public SpaceEnvironmentType CurrentEnvironment => currentEnvironment;

        public event System.Action<SpaceEnvironmentType> OnEnvironmentChanged;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            FindPlayer();
            InitializeDefaultZones();
        }

        private void FindPlayer()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                playerHealth = player.GetComponent<Vehicles.HealthSystem>();
            }
        }

        private void InitializeDefaultZones()
        {
            // Add some example zones
            environmentZones.Add(new EnvironmentZone
            {
                zoneId = "nebula_alpha",
                zoneName = "Alpha Nebula",
                environmentType = SpaceEnvironmentType.Nebula,
                center = new Vector3(1000, 0, 1000),
                radius = 500f
            });

            environmentZones.Add(new EnvironmentZone
            {
                zoneId = "asteroid_belt",
                zoneName = "Asteroid Belt",
                environmentType = SpaceEnvironmentType.AsteroidField,
                center = new Vector3(-500, 0, 500),
                radius = 800f
            });

            environmentZones.Add(new EnvironmentZone
            {
                zoneId = "radiation_zone",
                zoneName = "Radiation Zone",
                environmentType = SpaceEnvironmentType.RadiationZone,
                center = new Vector3(0, 0, 2000),
                radius = 300f
            });
        }

        private void Update()
        {
            if (playerTransform != null)
            {
                CheckEnvironmentZone();
                ApplyEnvironmentEffects();
            }
        }

        private void CheckEnvironmentZone()
        {
            SpaceEnvironmentType newEnvironment = SpaceEnvironmentType.OpenSpace;
            float highestPriority = 0f;

            foreach (var zone in environmentZones)
            {
                float distance = Vector3.Distance(playerTransform.position, zone.center);
                if (distance < zone.radius)
                {
                    float priority = 1f - (distance / zone.radius);
                    if (priority > highestPriority)
                    {
                        highestPriority = priority;
                        newEnvironment = zone.environmentType;
                        environmentDensity = priority;
                    }
                }
            }

            if (newEnvironment != currentEnvironment)
            {
                SetEnvironment(newEnvironment);
            }
        }

        public void SetEnvironment(SpaceEnvironmentType environmentType)
        {
            SpaceEnvironmentType previousEnvironment = currentEnvironment;
            currentEnvironment = environmentType;

            UpdateVisualEffects();
            
            OnEnvironmentChanged?.Invoke(currentEnvironment);
            
            Debug.Log($"Environment changed: {previousEnvironment} -> {currentEnvironment}");
        }

        private void UpdateVisualEffects()
        {
            // Update particles
            if (nebulaParticles != null)
            {
                if (currentEnvironment == SpaceEnvironmentType.Nebula)
                {
                    nebulaParticles.Play();
                    var main = nebulaParticles.main;
                    main.startColor = nebulaColor;
                }
                else
                {
                    nebulaParticles.Stop();
                }
            }

            if (dustParticles != null)
            {
                if (currentEnvironment == SpaceEnvironmentType.AsteroidField)
                {
                    dustParticles.Play();
                }
                else
                {
                    dustParticles.Stop();
                }
            }

            if (radiationParticles != null)
            {
                if (currentEnvironment == SpaceEnvironmentType.RadiationZone)
                {
                    radiationParticles.Play();
                }
                else
                {
                    radiationParticles.Stop();
                }
            }

            // Update skybox
            UpdateSkybox();

            // Update ambient light
            UpdateAmbientLight();

            // Update fog
            UpdateFog();
        }

        private void UpdateSkybox()
        {
            Material skybox = defaultSkybox;

            switch (currentEnvironment)
            {
                case SpaceEnvironmentType.Nebula:
                    skybox = nebulaSkybox ?? defaultSkybox;
                    break;
                case SpaceEnvironmentType.AsteroidField:
                    skybox = asteroidFieldSkybox ?? defaultSkybox;
                    break;
            }

            if (skybox != null)
            {
                RenderSettings.skybox = skybox;
            }
        }

        private void UpdateAmbientLight()
        {
            if (ambientLight == null) return;

            switch (currentEnvironment)
            {
                case SpaceEnvironmentType.OpenSpace:
                    ambientLight.color = Color.white;
                    ambientLight.intensity = 1f;
                    break;
                case SpaceEnvironmentType.Nebula:
                    ambientLight.color = nebulaColor;
                    ambientLight.intensity = 0.5f;
                    break;
                case SpaceEnvironmentType.AsteroidField:
                    ambientLight.color = new Color(0.8f, 0.8f, 0.9f);
                    ambientLight.intensity = 0.7f;
                    break;
                case SpaceEnvironmentType.RadiationZone:
                    ambientLight.color = Color.yellow;
                    ambientLight.intensity = 0.6f;
                    break;
            }
        }

        private void UpdateFog()
        {
            switch (currentEnvironment)
            {
                case SpaceEnvironmentType.OpenSpace:
                    RenderSettings.fog = false;
                    break;
                case SpaceEnvironmentType.Nebula:
                    RenderSettings.fog = true;
                    RenderSettings.fogColor = new Color(nebulaColor.r, nebulaColor.g, nebulaColor.b, 1f);
                    RenderSettings.fogDensity = 0.01f * environmentDensity;
                    RenderSettings.fogMode = FogMode.Exponential;
                    break;
                case SpaceEnvironmentType.AsteroidField:
                    RenderSettings.fog = true;
                    RenderSettings.fogColor = new Color(0.3f, 0.3f, 0.35f);
                    RenderSettings.fogDensity = 0.005f * environmentDensity;
                    RenderSettings.fogMode = FogMode.Exponential;
                    break;
                case SpaceEnvironmentType.RadiationZone:
                    RenderSettings.fog = true;
                    RenderSettings.fogColor = new Color(0.5f, 0.5f, 0.2f);
                    RenderSettings.fogDensity = 0.008f * environmentDensity;
                    RenderSettings.fogMode = FogMode.Exponential;
                    break;
            }
        }

        private void ApplyEnvironmentEffects()
        {
            switch (currentEnvironment)
            {
                case SpaceEnvironmentType.RadiationZone:
                    ApplyRadiationDamage();
                    break;
            }
        }

        private void ApplyRadiationDamage()
        {
            if (playerHealth == null) return;

            float damage = radiationDamagePerSecond * environmentDensity * Time.deltaTime;
            playerHealth.TakeDamage(damage, Vehicles.DamageType.Radiation);
        }

        public float GetSpeedModifier()
        {
            switch (currentEnvironment)
            {
                case SpaceEnvironmentType.Nebula:
                    return 1f - (nebulaSpeedReduction * environmentDensity);
                case SpaceEnvironmentType.AsteroidField:
                    return 0.9f;
                default:
                    return 1f;
            }
        }

        public float GetSensorRangeModifier()
        {
            switch (currentEnvironment)
            {
                case SpaceEnvironmentType.Nebula:
                    return 1f - (nebulaSensorRange * environmentDensity);
                default:
                    return 1f;
            }
        }

        public float GetShieldRegenModifier()
        {
            switch (currentEnvironment)
            {
                case SpaceEnvironmentType.Nebula:
                    return 1f + (nebulaShieldRegen * environmentDensity);
                default:
                    return 1f;
            }
        }

        public bool IsInZone(Vector3 position, SpaceEnvironmentType type)
        {
            foreach (var zone in environmentZones)
            {
                if (zone.environmentType == type)
                {
                    float distance = Vector3.Distance(position, zone.center);
                    if (distance < zone.radius)
                        return true;
                }
            }
            return false;
        }

        public EnvironmentZone GetCurrentZone()
        {
            if (playerTransform == null) return null;

            foreach (var zone in environmentZones)
            {
                float distance = Vector3.Distance(playerTransform.position, zone.center);
                if (distance < zone.radius)
                {
                    return zone;
                }
            }
            return null;
        }

        public void AddZone(EnvironmentZone zone)
        {
            environmentZones.Add(zone);
        }

        public void RemoveZone(string zoneId)
        {
            for (int i = environmentZones.Count - 1; i >= 0; i--)
            {
                if (environmentZones[i].zoneId == zoneId)
                {
                    environmentZones.RemoveAt(i);
                    break;
                }
            }
        }

        public void SetPlayerTransform(Transform player)
        {
            playerTransform = player;
            if (player != null)
            {
                playerHealth = player.GetComponent<Vehicles.HealthSystem>();
            }
        }
    }

    [System.Serializable]
    public class EnvironmentZone
    {
        public string zoneId;
        public string zoneName;
        public SpaceEnvironmentType environmentType;
        public Vector3 center;
        public float radius;
        public float intensity = 1f;
    }

    public enum SpaceEnvironmentType
    {
        OpenSpace,
        Nebula,
        AsteroidField,
        RadiationZone,
        IonStorm,
        GravityWell,
        SafeZone
    }
}
