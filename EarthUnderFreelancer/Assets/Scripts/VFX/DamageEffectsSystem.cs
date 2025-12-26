using UnityEngine;

namespace EarthUnderFreelancer.VFX
{
    /// <summary>
    /// Visual damage effects system
    /// Shows smoke, fire, sparks based on damage level
    /// </summary>
    public class DamageEffectsSystem : MonoBehaviour
    {
        [Header("Damage Thresholds")]
        [SerializeField] private float smokeThreshold = 70f; // Below 70% health
        [SerializeField] private float heavySmokeThreshold = 40f;
        [SerializeField] private float fireThreshold = 25f;
        
        [Header("Effect Prefabs")]
        [SerializeField] private GameObject lightSmokePrefab;
        [SerializeField] private GameObject heavySmokePrefab;
        [SerializeField] private GameObject firePrefab;
        [SerializeField] private GameObject sparksPrefab;
        
        [Header("Effect Points")]
        [SerializeField] private Transform[] damagePoints;
        
        private GameObject activeSmoke;
        private GameObject activeFire;
        private GameObject activeSparks;
        
        private float currentHealth = 100f;
        private float maxHealth = 100f;
        
        private void Update()
        {
            UpdateDamageEffects();
        }
        
        public void SetHealth(float health, float max = 100f)
        {
            currentHealth = health;
            maxHealth = max;
        }
        
        public void TakeDamage(float amount)
        {
            currentHealth = Mathf.Max(0, currentHealth - amount);
            
            // Trigger impact effects
            ShowImpactEffect();
        }
        
        private void UpdateDamageEffects()
        {
            float healthPercent = (currentHealth / maxHealth) * 100f;
            
            // Light smoke
            if (healthPercent < smokeThreshold && healthPercent >= heavySmokeThreshold)
            {
                if (activeSmoke == null && lightSmokePrefab != null)
                {
                    activeSmoke = CreateEffect(lightSmokePrefab);
                }
            }
            
            // Heavy smoke
            else if (healthPercent < heavySmokeThreshold && healthPercent >= fireThreshold)
            {
                if (activeSmoke != null && activeSmoke.name.Contains("Light"))
                {
                    Destroy(activeSmoke);
                }
                
                if (activeSmoke == null && heavySmokePrefab != null)
                {
                    activeSmoke = CreateEffect(heavySmokePrefab);
                }
            }
            
            // Fire
            else if (healthPercent < fireThreshold)
            {
                if (activeFire == null && firePrefab != null)
                {
                    activeFire = CreateEffect(firePrefab);
                }
                
                // Also show sparks at critical health
                if (activeSparks == null && sparksPrefab != null && healthPercent < 15f)
                {
                    activeSparks = CreateEffect(sparksPrefab);
                }
            }
            
            // Full health - clear effects
            else if (healthPercent >= smokeThreshold)
            {
                ClearAllEffects();
            }
        }
        
        private GameObject CreateEffect(GameObject prefab)
        {
            Transform spawnPoint = GetRandomDamagePoint();
            GameObject effect = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation, transform);
            return effect;
        }
        
        private Transform GetRandomDamagePoint()
        {
            if (damagePoints != null && damagePoints.Length > 0)
            {
                return damagePoints[Random.Range(0, damagePoints.Length)];
            }
            return transform;
        }
        
        private void ShowImpactEffect()
        {
            // Create temporary spark effect at hit location
            if (sparksPrefab != null)
            {
                Transform point = GetRandomDamagePoint();
                GameObject sparks = Instantiate(sparksPrefab, point.position, point.rotation);
                Destroy(sparks, 1f);
            }
        }
        
        private void ClearAllEffects()
        {
            if (activeSmoke != null)
            {
                Destroy(activeSmoke);
                activeSmoke = null;
            }
            
            if (activeFire != null)
            {
                Destroy(activeFire);
                activeFire = null;
            }
            
            if (activeSparks != null)
            {
                Destroy(activeSparks);
                activeSparks = null;
            }
        }
        
        public void Repair(float amount)
        {
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        }
        
        public float GetHealthPercent()
        {
            return (currentHealth / maxHealth) * 100f;
        }
    }
}
