using UnityEngine;
using System;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Combat
{
    /// <summary>
    /// Advanced damage system with component-based damage, critical hits, armor penetration,
    /// and detailed damage modeling for realistic aircraft combat
    /// </summary>
    public class AdvancedDamageSystem : MonoBehaviour
    {
        [Header("Aircraft Components")]
        [SerializeField] private AircraftComponent[] components;
        [SerializeField] private float totalHealth = 1000f;
        [SerializeField] private float armorRating = 50f;

        [Header("Critical Systems")]
        [SerializeField] private bool hasEngine = true;
        [SerializeField] private bool hasFuelTank = true;
        [SerializeField] private bool hasPilot = true;
        [SerializeField] private bool hasControlSurfaces = true;

        [Header("Damage Effects")]
        [SerializeField] private float fireChanceOnFuelHit = 0.3f;
        [SerializeField] private float explosionChanceOnCritical = 0.1f;
        [SerializeField] private float pilotKillChance = 0.05f;

        [Header("Visual Effects")]
        [SerializeField] private GameObject smokePrefab;
        [SerializeField] private GameObject firePrefab;
        [SerializeField] private GameObject explosionPrefab;
        [SerializeField] private Transform[] smokePoints;
        [SerializeField] private Transform[] enginePoints;

        // State
        private float currentHealth;
        private bool isOnFire;
        private bool isLeakingFuel;
        private bool engineDamaged;
        private bool controlsDamaged;
        private float engineEfficiency = 1f;
        private float controlEfficiency = 1f;
        private List<DamageEffect> activeEffects = new List<DamageEffect>();
        private Dictionary<AircraftComponentType, float> componentHealth = new Dictionary<AircraftComponentType, float>();

        // Events
        public event Action<DamageInfo> OnDamageTaken;
        public event Action<AircraftComponentType> OnComponentDestroyed;
        public event Action OnCriticalDamage;
        public event Action OnAircraftDestroyed;
        public event Action OnFireStarted;
        public event Action OnFireExtinguished;

        // Properties
        public float CurrentHealth => currentHealth;
        public float HealthPercent => currentHealth / totalHealth;
        public bool IsOnFire => isOnFire;
        public float EngineEfficiency => engineEfficiency;
        public float ControlEfficiency => controlEfficiency;
        public bool IsAlive => currentHealth > 0 && hasPilot;

        private void Awake()
        {
            currentHealth = totalHealth;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            // Initialize default components if not set
            if (components == null || components.Length == 0)
            {
                components = new AircraftComponent[]
                {
                    new AircraftComponent(AircraftComponentType.Engine, 200f, 1.5f, true),
                    new AircraftComponent(AircraftComponentType.FuelTank, 150f, 2f, true),
                    new AircraftComponent(AircraftComponentType.Cockpit, 100f, 3f, true),
                    new AircraftComponent(AircraftComponentType.Wings, 300f, 1.2f, false),
                    new AircraftComponent(AircraftComponentType.Tail, 150f, 1.3f, false),
                    new AircraftComponent(AircraftComponentType.Fuselage, 400f, 1f, false),
                    new AircraftComponent(AircraftComponentType.LandingGear, 100f, 0.8f, false),
                    new AircraftComponent(AircraftComponentType.ControlSurfaces, 100f, 1.4f, true)
                };
            }

            foreach (var comp in components)
            {
                componentHealth[comp.Type] = comp.MaxHealth;
            }
        }

        private void Update()
        {
            UpdateDamageEffects();
            UpdateFireDamage();
        }

        private void UpdateDamageEffects()
        {
            for (int i = activeEffects.Count - 1; i >= 0; i--)
            {
                activeEffects[i].duration -= Time.deltaTime;
                if (activeEffects[i].duration <= 0)
                {
                    activeEffects.RemoveAt(i);
                }
            }
        }

        private void UpdateFireDamage()
        {
            if (!isOnFire) return;

            // Fire deals continuous damage
            float fireDamage = 10f * Time.deltaTime;
            TakeDamage(new DamageInfo
            {
                damage = fireDamage,
                damageType = DamageType.Fire,
                hitPoint = transform.position,
                sourceId = "fire"
            });

            // Chance to extinguish naturally (low)
            if (UnityEngine.Random.value < 0.001f * Time.deltaTime)
            {
                ExtinguishFire();
            }
        }

        public void TakeDamage(DamageInfo damageInfo)
        {
            if (!IsAlive) return;

            // Calculate armor penetration
            float penetration = CalculateArmorPenetration(damageInfo);
            if (penetration <= 0) return;

            float actualDamage = damageInfo.damage * penetration;

            // Determine which component was hit
            AircraftComponent hitComponent = DetermineHitComponent(damageInfo.hitPoint);
            if (hitComponent != null)
            {
                actualDamage *= hitComponent.DamageMultiplier;
                DamageComponent(hitComponent, actualDamage, damageInfo);
            }

            // Apply damage to overall health
            currentHealth -= actualDamage;
            currentHealth = Mathf.Max(0, currentHealth);

            // Check for critical hits
            if (damageInfo.isCritical || UnityEngine.Random.value < 0.1f)
            {
                ProcessCriticalHit(hitComponent, damageInfo);
            }

            OnDamageTaken?.Invoke(damageInfo);

            // Check for destruction
            if (currentHealth <= 0)
            {
                DestroyAircraft();
            }
            else if (currentHealth < totalHealth * 0.25f)
            {
                OnCriticalDamage?.Invoke();
            }
        }

        private float CalculateArmorPenetration(DamageInfo damageInfo)
        {
            float effectiveArmor = armorRating;

            // Armor effectiveness varies by damage type
            switch (damageInfo.damageType)
            {
                case DamageType.Bullet:
                    effectiveArmor *= 1f;
                    break;
                case DamageType.Cannon:
                    effectiveArmor *= 0.5f; // Cannons are better at penetrating
                    break;
                case DamageType.Explosive:
                    effectiveArmor *= 0.3f; // Explosives bypass armor somewhat
                    break;
                case DamageType.Fire:
                    effectiveArmor = 0f; // Fire ignores armor
                    break;
                case DamageType.Shrapnel:
                    effectiveArmor *= 0.8f;
                    break;
            }

            // Calculate penetration chance (0 = blocked, 1 = full damage)
            float penetrationChance = damageInfo.penetration / (damageInfo.penetration + effectiveArmor);
            return penetrationChance;
        }

        private AircraftComponent DetermineHitComponent(Vector3 hitPoint)
        {
            if (components == null || components.Length == 0) return null;

            // In a real implementation, this would use the hit point to determine
            // which component was actually hit based on collider zones
            // For now, we use weighted random selection

            float totalWeight = 0f;
            foreach (var comp in components)
            {
                totalWeight += comp.HitChanceWeight;
            }

            float roll = UnityEngine.Random.value * totalWeight;
            float cumulative = 0f;

            foreach (var comp in components)
            {
                cumulative += comp.HitChanceWeight;
                if (roll <= cumulative)
                {
                    return comp;
                }
            }

            return components[0];
        }

        private void DamageComponent(AircraftComponent component, float damage, DamageInfo damageInfo)
        {
            if (!componentHealth.ContainsKey(component.Type)) return;

            componentHealth[component.Type] -= damage;

            if (componentHealth[component.Type] <= 0)
            {
                componentHealth[component.Type] = 0;
                OnComponentDestroyed?.Invoke(component.Type);
                ProcessComponentDestruction(component);
            }
        }

        private void ProcessComponentDestruction(AircraftComponent component)
        {
            switch (component.Type)
            {
                case AircraftComponentType.Engine:
                    hasEngine = false;
                    engineEfficiency = 0f;
                    engineDamaged = true;
                    SpawnSmokeEffect();
                    break;

                case AircraftComponentType.FuelTank:
                    hasFuelTank = false;
                    isLeakingFuel = true;
                    if (UnityEngine.Random.value < fireChanceOnFuelHit)
                    {
                        StartFire();
                    }
                    break;

                case AircraftComponentType.Cockpit:
                    if (UnityEngine.Random.value < pilotKillChance)
                    {
                        hasPilot = false;
                        DestroyAircraft();
                    }
                    break;

                case AircraftComponentType.ControlSurfaces:
                    hasControlSurfaces = false;
                    controlsDamaged = true;
                    controlEfficiency = 0.2f;
                    break;

                case AircraftComponentType.Wings:
                    controlEfficiency *= 0.5f;
                    // Severe wing damage can cause immediate destruction
                    if (UnityEngine.Random.value < 0.3f)
                    {
                        DestroyAircraft();
                    }
                    break;

                case AircraftComponentType.Tail:
                    controlEfficiency *= 0.3f;
                    break;
            }
        }

        private void ProcessCriticalHit(AircraftComponent component, DamageInfo damageInfo)
        {
            // Critical hits have special effects
            switch (component?.Type)
            {
                case AircraftComponentType.Engine:
                    engineEfficiency *= 0.5f;
                    SpawnSmokeEffect();
                    break;

                case AircraftComponentType.FuelTank:
                    if (UnityEngine.Random.value < fireChanceOnFuelHit * 2f)
                    {
                        StartFire();
                    }
                    break;

                case AircraftComponentType.Cockpit:
                    // Pilot injury
                    if (UnityEngine.Random.value < pilotKillChance * 3f)
                    {
                        hasPilot = false;
                    }
                    break;
            }

            // Chance for immediate explosion on critical
            if (UnityEngine.Random.value < explosionChanceOnCritical)
            {
                DestroyAircraft();
            }
        }

        public void StartFire()
        {
            if (isOnFire) return;

            isOnFire = true;
            OnFireStarted?.Invoke();

            // Spawn fire effect
            if (firePrefab != null && enginePoints != null && enginePoints.Length > 0)
            {
                foreach (var point in enginePoints)
                {
                    if (point != null)
                    {
                        Instantiate(firePrefab, point.position, point.rotation, point);
                    }
                }
            }
        }

        public void ExtinguishFire()
        {
            if (!isOnFire) return;

            isOnFire = false;
            OnFireExtinguished?.Invoke();

            // Remove fire effects
            foreach (var effect in GetComponentsInChildren<ParticleSystem>())
            {
                if (effect.gameObject.name.Contains("Fire"))
                {
                    Destroy(effect.gameObject);
                }
            }
        }

        private void SpawnSmokeEffect()
        {
            if (smokePrefab == null || smokePoints == null) return;

            foreach (var point in smokePoints)
            {
                if (point != null)
                {
                    Instantiate(smokePrefab, point.position, point.rotation, point);
                }
            }
        }

        private void DestroyAircraft()
        {
            OnAircraftDestroyed?.Invoke();

            // Spawn explosion
            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            }

            // In a real implementation, would trigger death sequence, respawn, etc.
            gameObject.SetActive(false);
        }

        public void Repair(float amount)
        {
            currentHealth = Mathf.Min(currentHealth + amount, totalHealth);
        }

        public void RepairComponent(AircraftComponentType type, float amount)
        {
            var component = Array.Find(components, c => c.Type == type);
            if (component != null && componentHealth.ContainsKey(type))
            {
                componentHealth[type] = Mathf.Min(componentHealth[type] + amount, component.MaxHealth);

                // Restore functionality if repaired enough
                if (componentHealth[type] > component.MaxHealth * 0.5f)
                {
                    RestoreComponent(type);
                }
            }
        }

        private void RestoreComponent(AircraftComponentType type)
        {
            switch (type)
            {
                case AircraftComponentType.Engine:
                    hasEngine = true;
                    engineDamaged = false;
                    engineEfficiency = componentHealth[type] / GetComponent(type).MaxHealth;
                    break;

                case AircraftComponentType.ControlSurfaces:
                    hasControlSurfaces = true;
                    controlsDamaged = false;
                    controlEfficiency = componentHealth[type] / GetComponent(type).MaxHealth;
                    break;
            }
        }

        private AircraftComponent GetComponent(AircraftComponentType type)
        {
            return Array.Find(components, c => c.Type == type);
        }

        public float GetComponentHealthPercent(AircraftComponentType type)
        {
            var component = GetComponent(type);
            if (component != null && componentHealth.ContainsKey(type))
            {
                return componentHealth[type] / component.MaxHealth;
            }
            return 0f;
        }

        public DamageSystemStatus GetStatus()
        {
            return new DamageSystemStatus
            {
                HealthPercent = HealthPercent,
                IsOnFire = isOnFire,
                IsLeakingFuel = isLeakingFuel,
                EngineDamaged = engineDamaged,
                ControlsDamaged = controlsDamaged,
                EngineEfficiency = engineEfficiency,
                ControlEfficiency = controlEfficiency,
                IsAlive = IsAlive
            };
        }
    }

    [Serializable]
    public class AircraftComponent
    {
        public AircraftComponentType Type;
        public float MaxHealth;
        public float DamageMultiplier;
        public bool IsCritical;
        public float HitChanceWeight;

        public AircraftComponent(AircraftComponentType type, float maxHealth, float damageMultiplier, bool isCritical)
        {
            Type = type;
            MaxHealth = maxHealth;
            DamageMultiplier = damageMultiplier;
            IsCritical = isCritical;
            HitChanceWeight = 1f;
        }
    }

    public enum AircraftComponentType
    {
        Engine,
        FuelTank,
        Cockpit,
        Wings,
        Tail,
        Fuselage,
        LandingGear,
        ControlSurfaces,
        Weapons,
        Avionics
    }

    public enum DamageType
    {
        Bullet,
        Cannon,
        Explosive,
        Fire,
        Shrapnel,
        Collision
    }

    public struct DamageInfo
    {
        public float damage;
        public DamageType damageType;
        public float penetration;
        public Vector3 hitPoint;
        public Vector3 hitDirection;
        public string sourceId;
        public bool isCritical;
    }

    public class DamageEffect
    {
        public DamageType type;
        public float duration;
        public float tickDamage;
    }

    public struct DamageSystemStatus
    {
        public float HealthPercent;
        public bool IsOnFire;
        public bool IsLeakingFuel;
        public bool EngineDamaged;
        public bool ControlsDamaged;
        public float EngineEfficiency;
        public float ControlEfficiency;
        public bool IsAlive;
    }
}
