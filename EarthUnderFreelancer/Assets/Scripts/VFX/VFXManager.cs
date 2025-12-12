using UnityEngine;

namespace EarthUnderFreelancer.VFX
{
    /// <summary>
    /// Manages visual effects like explosions, impacts, trails
    /// </summary>
    public class VFXManager : MonoBehaviour
    {
        public static VFXManager Instance { get; private set; }

        [Header("Effect Prefabs")]
        [SerializeField] private GameObject explosionSmallPrefab;
        [SerializeField] private GameObject explosionMediumPrefab;
        [SerializeField] private GameObject explosionLargePrefab;
        [SerializeField] private GameObject impactSparksPrefab;
        [SerializeField] private GameObject shieldHitPrefab;
        [SerializeField] private GameObject muzzleFlashPrefab;
        [SerializeField] private GameObject missileTrailPrefab;
        [SerializeField] private GameObject engineTrailPrefab;

        [Header("Pool Settings")]
        [SerializeField] private int poolSize = 20;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            CreateDefaultPrefabs();
        }

        private void CreateDefaultPrefabs()
        {
            if (explosionSmallPrefab == null)
            {
                explosionSmallPrefab = CreateExplosionPrefab(5f, Color.yellow);
            }
            if (explosionMediumPrefab == null)
            {
                explosionMediumPrefab = CreateExplosionPrefab(10f, new Color(1f, 0.5f, 0f));
            }
            if (explosionLargePrefab == null)
            {
                explosionLargePrefab = CreateExplosionPrefab(20f, Color.red);
            }
            if (impactSparksPrefab == null)
            {
                impactSparksPrefab = CreateSparksPrefab();
            }
            if (shieldHitPrefab == null)
            {
                shieldHitPrefab = CreateShieldHitPrefab();
            }
        }

        private GameObject CreateExplosionPrefab(float size, Color color)
        {
            GameObject prefab = new GameObject("Explosion");
            prefab.SetActive(false);

            ParticleSystem ps = prefab.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.duration = 0.5f;
            main.loop = false;
            main.startLifetime = 0.5f;
            main.startSpeed = size * 2f;
            main.startSize = size;
            main.startColor = color;
            main.maxParticles = 50;

            var emission = ps.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 30) });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = size * 0.5f;

            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient grad = new Gradient();
            grad.SetKeys(
                new GradientColorKey[] { new GradientColorKey(color, 0f), new GradientColorKey(Color.gray, 1f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
            );
            colorOverLifetime.color = grad;

            var sizeOverLifetime = ps.sizeOverLifetime;
            sizeOverLifetime.enabled = true;
            sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.EaseInOut(0f, 1f, 1f, 0f));

            prefab.AddComponent<DestroyAfterTime>().lifetime = 2f;

            return prefab;
        }

        private GameObject CreateSparksPrefab()
        {
            GameObject prefab = new GameObject("Sparks");
            prefab.SetActive(false);

            ParticleSystem ps = prefab.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.duration = 0.2f;
            main.loop = false;
            main.startLifetime = 0.3f;
            main.startSpeed = 20f;
            main.startSize = 0.2f;
            main.startColor = Color.yellow;
            main.maxParticles = 20;

            var emission = ps.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 15) });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 45f;

            prefab.AddComponent<DestroyAfterTime>().lifetime = 1f;

            return prefab;
        }

        private GameObject CreateShieldHitPrefab()
        {
            GameObject prefab = new GameObject("ShieldHit");
            prefab.SetActive(false);

            ParticleSystem ps = prefab.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.duration = 0.3f;
            main.loop = false;
            main.startLifetime = 0.3f;
            main.startSpeed = 5f;
            main.startSize = 2f;
            main.startColor = new Color(0.3f, 0.5f, 1f, 0.7f);
            main.maxParticles = 30;

            var emission = ps.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 20) });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Hemisphere;
            shape.radius = 2f;

            prefab.AddComponent<DestroyAfterTime>().lifetime = 1f;

            return prefab;
        }

        public void SpawnExplosion(Vector3 position, ExplosionSize size = ExplosionSize.Medium)
        {
            GameObject prefab = size switch
            {
                ExplosionSize.Small => explosionSmallPrefab,
                ExplosionSize.Large => explosionLargePrefab,
                _ => explosionMediumPrefab
            };

            if (prefab != null)
            {
                GameObject explosion = Instantiate(prefab, position, Quaternion.identity);
                explosion.SetActive(true);
            }
        }

        public void SpawnImpact(Vector3 position, Vector3 normal, bool isShieldHit = false)
        {
            GameObject prefab = isShieldHit ? shieldHitPrefab : impactSparksPrefab;

            if (prefab != null)
            {
                Quaternion rotation = Quaternion.LookRotation(normal);
                GameObject impact = Instantiate(prefab, position, rotation);
                impact.SetActive(true);
            }
        }

        public void SpawnMuzzleFlash(Vector3 position, Quaternion rotation)
        {
            if (muzzleFlashPrefab != null)
            {
                GameObject flash = Instantiate(muzzleFlashPrefab, position, rotation);
                flash.SetActive(true);
            }
        }

        public GameObject SpawnTrail(Transform parent, TrailType type)
        {
            GameObject prefab = type == TrailType.Missile ? missileTrailPrefab : engineTrailPrefab;

            if (prefab != null)
            {
                GameObject trail = Instantiate(prefab, parent);
                trail.SetActive(true);
                return trail;
            }
            return null;
        }
    }

    public class DestroyAfterTime : MonoBehaviour
    {
        public float lifetime = 2f;

        private void OnEnable()
        {
            Destroy(gameObject, lifetime);
        }
    }

    public enum ExplosionSize
    {
        Small,
        Medium,
        Large
    }

    public enum TrailType
    {
        Engine,
        Missile
    }
}
