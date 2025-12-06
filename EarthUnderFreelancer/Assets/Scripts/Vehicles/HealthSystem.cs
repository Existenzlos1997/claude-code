using UnityEngine;
using EarthUnderFreelancer.Core;
using System;

namespace EarthUnderFreelancer.Vehicles
{
    /// <summary>
    /// Handles health, shields, and damage for vehicles
    /// </summary>
    public class HealthSystem : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth;

        [Header("Shield")]
        [SerializeField] private float maxShield = 50f;
        [SerializeField] private float currentShield;
        [SerializeField] private float shieldRegenRate = 5f;
        [SerializeField] private float shieldRegenDelay = 3f;

        [Header("Armor")]
        [SerializeField] private float armorValue = 0f;
        [SerializeField] private float damageReduction = 0f;

        [Header("Effects")]
        [SerializeField] private GameObject shieldHitEffect;
        [SerializeField] private GameObject hullHitEffect;
        [SerializeField] private GameObject explosionEffect;
        [SerializeField] private GameObject smokeEffect;

        private float lastDamageTime;
        private bool isInvulnerable = false;
        private bool isDead = false;

        public event Action<float, float> OnHealthChanged;
        public event Action<float, float> OnShieldChanged;
        public event Action OnDeath;
        public event Action<float, GameObject> OnDamageTaken;

        public float MaxHealth => maxHealth;
        public float CurrentHealth => currentHealth;
        public float MaxShield => maxShield;
        public float CurrentShield => currentShield;
        public float HealthPercent => currentHealth / maxHealth;
        public float ShieldPercent => maxShield > 0 ? currentShield / maxShield : 0;
        public bool IsDead => isDead;
        public bool HasShield => currentShield > 0;

        private void Start()
        {
            currentHealth = maxHealth;
            currentShield = maxShield;
        }

        private void Update()
        {
            RegenerateShield();
            UpdateDamageEffects();
        }

        private void RegenerateShield()
        {
            if (isDead || currentShield >= maxShield) return;

            if (Time.time - lastDamageTime >= shieldRegenDelay)
            {
                currentShield = Mathf.Min(currentShield + shieldRegenRate * Time.deltaTime, maxShield);
                OnShieldChanged?.Invoke(currentShield, maxShield);
            }
        }

        private void UpdateDamageEffects()
        {
            if (smokeEffect != null)
            {
                bool shouldSmoke = HealthPercent < 0.3f;
                if (smokeEffect.activeSelf != shouldSmoke)
                {
                    smokeEffect.SetActive(shouldSmoke);
                }
            }
        }

        public void TakeDamage(float damage, GameObject damageSource = null, Vector3 hitPoint = default, DamageType damageType = DamageType.Kinetic)
        {
            if (isDead || isInvulnerable) return;

            lastDamageTime = Time.time;
            float remainingDamage = damage;

            remainingDamage = ApplyDamageTypeModifier(remainingDamage, damageType);

            if (currentShield > 0)
            {
                float shieldDamage = Mathf.Min(currentShield, remainingDamage);
                currentShield -= shieldDamage;
                remainingDamage -= shieldDamage;

                OnShieldChanged?.Invoke(currentShield, maxShield);

                if (shieldHitEffect != null && hitPoint != default)
                {
                    Instantiate(shieldHitEffect, hitPoint, Quaternion.identity);
                }
                AudioManager.Instance?.PlayShieldHit();

                if (currentShield <= 0)
                {
                    EventManager.TriggerEvent(GameEvents.SHIELD_DEPLETED, gameObject);
                }
            }

            if (remainingDamage > 0)
            {
                remainingDamage *= (1f - damageReduction);
                remainingDamage = Mathf.Max(remainingDamage - armorValue, 0);

                currentHealth -= remainingDamage;
                OnHealthChanged?.Invoke(currentHealth, maxHealth);

                if (hullHitEffect != null && hitPoint != default)
                {
                    Instantiate(hullHitEffect, hitPoint, Quaternion.identity);
                }
                AudioManager.Instance?.PlayHullHit();
            }

            OnDamageTaken?.Invoke(damage, damageSource);

            var damageData = new DamageEventData
            {
                source = damageSource,
                target = gameObject,
                damage = damage,
                damageType = damageType,
                hitPoint = hitPoint
            };
            EventManager.TriggerEvent(GameEvents.DAMAGE_TAKEN, damageData);

            if (currentHealth <= 0)
            {
                Die(damageSource);
            }
        }

        private float ApplyDamageTypeModifier(float damage, DamageType type)
        {
            switch (type)
            {
                case DamageType.Kinetic:
                    return HasShield ? damage * 0.8f : damage;
                case DamageType.Energy:
                    return HasShield ? damage * 1.5f : damage * 0.8f;
                case DamageType.Explosive:
                    return HasShield ? damage : damage * 1.3f;
                case DamageType.EMP:
                    return HasShield ? damage * 3f : 0f;
                default:
                    return damage;
            }
        }

        private void Die(GameObject killer = null)
        {
            if (isDead) return;
            isDead = true;

            if (explosionEffect != null)
            {
                Instantiate(explosionEffect, transform.position, Quaternion.identity);
            }
            AudioManager.Instance?.PlayExplosion();

            OnDeath?.Invoke();
            EventManager.TriggerEvent(GameEvents.PLAYER_DEATH, new object[] { gameObject, killer });

            var vehicleController = GetComponent<VehicleController>();
            vehicleController?.SetControllable(false);

            Destroy(gameObject, 2f);
        }

        public void Heal(float amount)
        {
            if (isDead) return;
            currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        public void RepairShield(float amount)
        {
            if (isDead) return;
            currentShield = Mathf.Min(currentShield + amount, maxShield);
            OnShieldChanged?.Invoke(currentShield, maxShield);
        }

        public void FullRepair()
        {
            currentHealth = maxHealth;
            currentShield = maxShield;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            OnShieldChanged?.Invoke(currentShield, maxShield);
        }

        public void SetInvulnerable(bool invulnerable, float duration = 0f)
        {
            isInvulnerable = invulnerable;
            if (duration > 0)
            {
                Invoke(nameof(EndInvulnerability), duration);
            }
        }

        private void EndInvulnerability() => isInvulnerable = false;

        public void SetMaxHealth(float value) { maxHealth = value; currentHealth = Mathf.Min(currentHealth, maxHealth); }
        public void SetMaxShield(float value) { maxShield = value; currentShield = Mathf.Min(currentShield, maxShield); }
        public void SetShieldRegenRate(float rate) => shieldRegenRate = rate;
        public void SetArmor(float armor, float reduction) { armorValue = armor; damageReduction = Mathf.Clamp01(reduction); }
        public void SetHealth(float value) { currentHealth = Mathf.Clamp(value, 0, maxHealth); OnHealthChanged?.Invoke(currentHealth, maxHealth); }
        public void SetShield(float value) { currentShield = Mathf.Clamp(value, 0, maxShield); OnShieldChanged?.Invoke(currentShield, maxShield); }

        /// <summary>
        /// Overload for realistic damage system with DamageSource enum
        /// </summary>
        public void TakeDamage(float damage, DamageSource source)
        {
            // Convert DamageSource to DamageType for compatibility
            DamageType damageType = source switch
            {
                DamageSource.Projectile => DamageType.Kinetic,
                DamageSource.Explosion => DamageType.Explosive,
                DamageSource.Collision => DamageType.Kinetic,
                DamageSource.StructuralFailure => DamageType.Kinetic,
                DamageSource.Fire => DamageType.Energy,
                DamageSource.Crash => DamageType.Kinetic,
                _ => DamageType.Kinetic
            };

            TakeDamage(damage, null, Vector3.zero, damageType);
        }
    }
}
