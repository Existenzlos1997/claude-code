using UnityEngine;
using EarthUnderFreelancer.Data;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Combat
{
    /// <summary>
    /// Realistic gun weapon system for WWII/Korean War aircraft
    /// Simulates machine guns and cannons with authentic ballistics
    /// </summary>
    public class RealisticGunController : MonoBehaviour
    {
        [Header("Primary Weapons (Machine Guns)")]
        [SerializeField] private List<GunHardpoint> machineGunHardpoints;
        [SerializeField] private MachineGunType machineGunType = MachineGunType.M2_Browning_12_7mm;
        [SerializeField] private int totalMGAmmo = 1880;
        
        [Header("Secondary Weapons (Cannons)")]
        [SerializeField] private List<GunHardpoint> cannonHardpoints;
        [SerializeField] private CannonType cannonType = CannonType.Hispano_Mk_II_20mm;
        [SerializeField] private int totalCannonAmmo = 240;

        [Header("Ordnance")]
        [SerializeField] private List<OrdnanceHardpoint> bombHardpoints;
        [SerializeField] private List<OrdnanceHardpoint> rocketHardpoints;

        [Header("Firing Settings")]
        [SerializeField] private bool convergenceEnabled = true;
        [SerializeField] private float convergenceDistance = 400f; // meters
        [SerializeField] private float spreadAngle = 0.5f;

        [Header("Heating")]
        [SerializeField] private float overheatThreshold = 1f;
        [SerializeField] private float coolingRate = 0.1f;
        [SerializeField] private bool jamsEnabled = true;

        [Header("Effects")]
        [SerializeField] private GameObject muzzleFlashPrefab;
        [SerializeField] private GameObject tracerPrefab;
        [SerializeField] private GameObject shellCasingPrefab;
        [SerializeField] private AudioClip mgFireSound;
        [SerializeField] private AudioClip cannonFireSound;
        [SerializeField] private AudioClip reloadSound;
        [SerializeField] private AudioClip jamSound;
        [SerializeField] private AudioClip overheatSound;

        // State
        private WeaponStats mgStats;
        private WeaponStats cannonStats;
        private int currentMGAmmo;
        private int currentCannonAmmo;
        private float mgHeat;
        private float cannonHeat;
        private float lastMGFireTime;
        private float lastCannonFireTime;
        private bool mgJammed;
        private bool cannonJammed;
        private bool isFiringMG;
        private bool isFiringCannon;
        private int tracerCounter;

        // Audio
        private AudioSource audioSource;
        private AudioSource loopAudioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            // Create loop audio source for continuous fire
            loopAudioSource = gameObject.AddComponent<AudioSource>();
            loopAudioSource.loop = true;
            loopAudioSource.playOnAwake = false;
            loopAudioSource.spatialBlend = 1f;

            InitializeWeapons();
        }

        private void InitializeWeapons()
        {
            mgStats = RealWeaponDatabase.GetWeaponStats(machineGunType);
            cannonStats = RealWeaponDatabase.GetCannonStats(cannonType);
            
            currentMGAmmo = totalMGAmmo;
            currentCannonAmmo = totalCannonAmmo;

            Debug.Log($"[Guns] Initialized: {mgStats.name} ({totalMGAmmo} rounds), {cannonStats.name} ({totalCannonAmmo} rounds)");
        }

        private void Update()
        {
            // Cool down weapons
            if (!isFiringMG)
            {
                mgHeat = Mathf.Max(0, mgHeat - coolingRate * Time.deltaTime);
            }
            if (!isFiringCannon)
            {
                cannonHeat = Mathf.Max(0, cannonHeat - coolingRate * Time.deltaTime);
            }
        }

        #region Firing

        public void SetFiringMG(bool firing)
        {
            isFiringMG = firing;
            if (firing)
            {
                StartFiringMG();
            }
            else
            {
                StopFiringMG();
            }
        }

        public void SetFiringCannon(bool firing)
        {
            isFiringCannon = firing;
            if (firing)
            {
                StartFiringCannon();
            }
            else
            {
                StopFiringCannon();
            }
        }

        private void StartFiringMG()
        {
            if (currentMGAmmo <= 0 || mgJammed || mgHeat >= overheatThreshold) return;

            if (Time.time - lastMGFireTime >= mgStats.FireInterval)
            {
                FireMGBurst();
            }

            // Start loop sound
            if (loopAudioSource != null && mgFireSound != null && !loopAudioSource.isPlaying)
            {
                loopAudioSource.clip = mgFireSound;
                loopAudioSource.Play();
            }
        }

        private void StopFiringMG()
        {
            if (loopAudioSource != null && loopAudioSource.isPlaying)
            {
                loopAudioSource.Stop();
            }
        }

        private void FireMGBurst()
        {
            foreach (var hardpoint in machineGunHardpoints)
            {
                if (currentMGAmmo <= 0) break;

                for (int i = 0; i < hardpoint.gunCount; i++)
                {
                    FireProjectile(hardpoint.muzzleTransforms[i], mgStats, true);
                    currentMGAmmo--;
                    
                    // Heat buildup
                    mgHeat += 0.01f;
                    
                    // Jam check
                    if (jamsEnabled && Random.value < mgStats.jamChance)
                    {
                        TriggerJam(true);
                        return;
                    }
                }
            }

            lastMGFireTime = Time.time;

            // Overheat check
            if (mgHeat >= overheatThreshold)
            {
                TriggerOverheat(true);
            }
        }

        private void StartFiringCannon()
        {
            if (currentCannonAmmo <= 0 || cannonJammed || cannonHeat >= overheatThreshold) return;

            if (Time.time - lastCannonFireTime >= cannonStats.FireInterval)
            {
                FireCannonRound();
            }
        }

        private void StopFiringCannon()
        {
            // Cannon doesn't loop, individual shots
        }

        private void FireCannonRound()
        {
            foreach (var hardpoint in cannonHardpoints)
            {
                if (currentCannonAmmo <= 0) break;

                for (int i = 0; i < hardpoint.gunCount; i++)
                {
                    FireProjectile(hardpoint.muzzleTransforms[i], cannonStats, false);
                    currentCannonAmmo--;
                    
                    // Heat buildup
                    cannonHeat += 0.02f;
                    
                    // Play sound
                    if (audioSource != null && cannonFireSound != null)
                    {
                        audioSource.PlayOneShot(cannonFireSound);
                    }
                    
                    // Jam check
                    if (jamsEnabled && Random.value < cannonStats.jamChance)
                    {
                        TriggerJam(false);
                        return;
                    }
                }
            }

            lastCannonFireTime = Time.time;

            // Overheat check
            if (cannonHeat >= overheatThreshold)
            {
                TriggerOverheat(false);
            }
        }

        private void FireProjectile(Transform muzzle, WeaponStats stats, bool isMG)
        {
            if (muzzle == null) return;

            // Calculate firing direction with convergence
            Vector3 targetPoint = muzzle.position + muzzle.forward * convergenceDistance;
            Vector3 direction;
            
            if (convergenceEnabled)
            {
                direction = (targetPoint - muzzle.position).normalized;
            }
            else
            {
                direction = muzzle.forward;
            }

            // Add spread
            direction += UnityEngine.Random.insideUnitSphere * spreadAngle * 0.01f;
            direction.Normalize();

            // Spawn projectile or use raycast for performance
            tracerCounter++;
            bool isTracer = tracerCounter % 5 == 0; // Every 5th round is a tracer

            SpawnProjectile(muzzle.position, direction, stats, isTracer, isMG);

            // Muzzle flash
            if (muzzleFlashPrefab != null)
            {
                var flash = Instantiate(muzzleFlashPrefab, muzzle.position, muzzle.rotation);
                Destroy(flash, 0.1f);
            }

            // Shell casing
            if (shellCasingPrefab != null)
            {
                var casing = Instantiate(shellCasingPrefab, muzzle.position + muzzle.right * 0.1f, Quaternion.identity);
                var rb = casing.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = (muzzle.right + Vector3.down) * 5f + UnityEngine.Random.insideUnitSphere;
                    rb.angularVelocity = UnityEngine.Random.insideUnitSphere * 20f;
                }
                Destroy(casing, 3f);
            }
        }

        private void SpawnProjectile(Vector3 position, Vector3 direction, WeaponStats stats, bool isTracer, bool isMG)
        {
            // Option 1: Physical projectile (more accurate, more expensive)
            if (tracerPrefab != null && isTracer)
            {
                var projectile = Instantiate(tracerPrefab, position, Quaternion.LookRotation(direction));
                var projScript = projectile.GetComponent<BallisticProjectile>();
                if (projScript != null)
                {
                    projScript.Initialize(
                        direction * stats.muzzleVelocity,
                        stats.baseDamage,
                        stats.armorPenMm,
                        stats.hasHE,
                        stats.explosiveFiller,
                        gameObject
                    );
                }
                else
                {
                    // Simple rigidbody projectile
                    var rb = projectile.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.linearVelocity = direction * stats.muzzleVelocity;
                    }
                }
            }

            // Option 2: Raycast for non-tracers (performance optimization)
            if (!isTracer)
            {
                RaycastHit hit;
                float maxRange = stats.effectiveRange;
                
                if (Physics.Raycast(position, direction, out hit, maxRange))
                {
                    // Apply damage
                    var health = hit.collider.GetComponentInParent<HealthSystem>();
                    if (health != null)
                    {
                        float damage = CalculateDamage(stats, Vector3.Distance(position, hit.point));
                        health.TakeDamage(damage, Vehicles.DamageSource.Projectile);
                    }
                }
            }
        }

        private float CalculateDamage(WeaponStats stats, float distance)
        {
            // Damage falloff over distance
            float falloff = 1f - (distance / stats.effectiveRange) * 0.3f;
            falloff = Mathf.Clamp01(falloff);
            
            float damage = stats.baseDamage * falloff;
            
            // HE bonus
            if (stats.hasHE)
            {
                damage += stats.explosiveFiller * 0.5f;
            }
            
            return damage;
        }

        #endregion

        #region Ordnance

        public void DropBomb(int hardpointIndex = -1)
        {
            if (hardpointIndex >= 0 && hardpointIndex < bombHardpoints.Count)
            {
                ReleaseOrdnance(bombHardpoints[hardpointIndex]);
            }
            else
            {
                // Drop from first available
                foreach (var hp in bombHardpoints)
                {
                    if (hp.currentCount > 0)
                    {
                        ReleaseOrdnance(hp);
                        break;
                    }
                }
            }
        }

        public void FireRocket(int hardpointIndex = -1)
        {
            if (hardpointIndex >= 0 && hardpointIndex < rocketHardpoints.Count)
            {
                ReleaseOrdnance(rocketHardpoints[hardpointIndex]);
            }
            else
            {
                // Fire from first available
                foreach (var hp in rocketHardpoints)
                {
                    if (hp.currentCount > 0)
                    {
                        ReleaseOrdnance(hp);
                        break;
                    }
                }
            }
        }

        private void ReleaseOrdnance(OrdnanceHardpoint hardpoint)
        {
            if (hardpoint.currentCount <= 0 || hardpoint.ordnancePrefab == null) return;

            var ordnance = Instantiate(hardpoint.ordnancePrefab, hardpoint.releasePoint.position, hardpoint.releasePoint.rotation);
            
            // Inherit parent velocity
            var rb = ordnance.GetComponent<Rigidbody>();
            var parentRb = GetComponentInParent<Rigidbody>();
            if (rb != null && parentRb != null)
            {
                rb.linearVelocity = parentRb.linearVelocity;
            }

            hardpoint.currentCount--;
        }

        public void JettisonAll()
        {
            foreach (var hp in bombHardpoints)
            {
                while (hp.currentCount > 0)
                {
                    ReleaseOrdnance(hp);
                }
            }
        }

        #endregion

        #region Jam/Overheat

        private void TriggerJam(bool isMG)
        {
            if (isMG)
            {
                mgJammed = true;
                Debug.Log("[Guns] Machine gun jammed!");
            }
            else
            {
                cannonJammed = true;
                Debug.Log("[Guns] Cannon jammed!");
            }

            if (audioSource != null && jamSound != null)
            {
                audioSource.PlayOneShot(jamSound);
            }
        }

        private void TriggerOverheat(bool isMG)
        {
            Debug.Log($"[Guns] {(isMG ? "Machine gun" : "Cannon")} overheated!");
            
            if (audioSource != null && overheatSound != null)
            {
                audioSource.PlayOneShot(overheatSound);
            }
        }

        public void ClearJam(bool isMG)
        {
            if (isMG)
            {
                mgJammed = false;
            }
            else
            {
                cannonJammed = false;
            }

            if (audioSource != null && reloadSound != null)
            {
                audioSource.PlayOneShot(reloadSound);
            }
        }

        #endregion

        #region Properties

        public int MGAmmo => currentMGAmmo;
        public int CannonAmmo => currentCannonAmmo;
        public float MGHeat => mgHeat;
        public float CannonHeat => cannonHeat;
        public bool MGJammed => mgJammed;
        public bool CannonJammed => cannonJammed;
        public int TotalBombs => GetTotalOrdnance(bombHardpoints);
        public int TotalRockets => GetTotalOrdnance(rocketHardpoints);

        private int GetTotalOrdnance(List<OrdnanceHardpoint> hardpoints)
        {
            int total = 0;
            foreach (var hp in hardpoints)
            {
                total += hp.currentCount;
            }
            return total;
        }

        #endregion
    }

    [System.Serializable]
    public class GunHardpoint
    {
        public string name;
        public int gunCount = 1;
        public Transform[] muzzleTransforms;
    }

    [System.Serializable]
    public class OrdnanceHardpoint
    {
        public string name;
        public OrdnanceType ordnanceType;
        public int maxCount;
        public int currentCount;
        public Transform releasePoint;
        public GameObject ordnancePrefab;
    }

    /// <summary>
    /// Ballistic projectile with realistic physics
    /// </summary>
    public class BallisticProjectile : MonoBehaviour
    {
        [SerializeField] private TrailRenderer trail;
        [SerializeField] private GameObject impactEffectPrefab;
        [SerializeField] private float lifetime = 5f;
        
        private Vector3 velocity;
        private float damage;
        private float armorPenetration;
        private bool hasHE;
        private float explosiveFiller;
        private GameObject owner;
        private float spawnTime;

        private const float GRAVITY = 9.81f;

        public void Initialize(Vector3 initialVelocity, float dmg, float pen, bool he, float heFiller, GameObject shooter)
        {
            velocity = initialVelocity;
            damage = dmg;
            armorPenetration = pen;
            hasHE = he;
            explosiveFiller = heFiller;
            owner = shooter;
            spawnTime = Time.time;
        }

        private void Update()
        {
            if (Time.time - spawnTime > lifetime)
            {
                Destroy(gameObject);
                return;
            }

            // Apply gravity (bullet drop)
            velocity += Vector3.down * GRAVITY * Time.deltaTime;

            // Move projectile
            Vector3 movement = velocity * Time.deltaTime;
            
            // Raycast for collision
            RaycastHit hit;
            if (Physics.Raycast(transform.position, velocity.normalized, out hit, movement.magnitude))
            {
                OnHit(hit);
                return;
            }

            transform.position += movement;
            transform.rotation = Quaternion.LookRotation(velocity);
        }

        private void OnHit(RaycastHit hit)
        {
            // Check if hit aircraft has health system
            var health = hit.collider.GetComponentInParent<HealthSystem>();
            if (health != null && hit.collider.gameObject != owner)
            {
                // Calculate damage based on remaining velocity
                float speedRetention = velocity.magnitude / 800f; // Assume 800 m/s initial
                float effectiveDamage = damage * Mathf.Clamp01(speedRetention);
                
                // HE damage
                if (hasHE)
                {
                    effectiveDamage += explosiveFiller * 0.5f;
                }
                
                health.TakeDamage(effectiveDamage, Vehicles.DamageSource.Projectile);
            }

            // Spawn impact effect
            if (impactEffectPrefab != null)
            {
                var effect = Instantiate(impactEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(effect, 2f);
            }

            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Bomb physics and damage
    /// </summary>
    public class Bomb : MonoBehaviour
    {
        [SerializeField] private float mass = 250f; // kg
        [SerializeField] private float explosiveFillerKg = 100f;
        [SerializeField] private float explosionRadius = 30f;
        [SerializeField] private float fuzeDelay = 0f;
        [SerializeField] private bool impactFuze = true;
        [SerializeField] private GameObject explosionEffectPrefab;
        [SerializeField] private AudioClip explosionSound;

        private bool armed = false;
        private float armDelay = 0.5f;
        private float spawnTime;
        private Rigidbody rb;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.mass = mass;
                rb.useGravity = true;
            }
            spawnTime = Time.time;
        }

        private void Update()
        {
            // Arm after delay
            if (!armed && Time.time - spawnTime > armDelay)
            {
                armed = true;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!armed) return;

            if (impactFuze)
            {
                Explode();
            }
        }

        private void Explode()
        {
            // Damage all nearby objects
            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
            foreach (var col in colliders)
            {
                var health = col.GetComponentInParent<HealthSystem>();
                if (health != null)
                {
                    float distance = Vector3.Distance(transform.position, col.transform.position);
                    float falloff = 1f - (distance / explosionRadius);
                    float damage = explosiveFillerKg * 10f * Mathf.Clamp01(falloff);
                    health.TakeDamage(damage, Vehicles.DamageSource.Explosion);
                }

                // Apply force
                var rb = col.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddExplosionForce(explosiveFillerKg * 100f, transform.position, explosionRadius);
                }
            }

            // Effects
            if (explosionEffectPrefab != null)
            {
                var effect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
                Destroy(effect, 5f);
            }

            // Audio
            if (explosionSound != null)
            {
                AudioSource.PlayClipAtPoint(explosionSound, transform.position);
            }

            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Unguided rocket physics
    /// </summary>
    public class UnGuidedRocket : MonoBehaviour
    {
        [SerializeField] private float thrust = 5000f;
        [SerializeField] private float burnTime = 1f;
        [SerializeField] private float explosiveFillerKg = 5f;
        [SerializeField] private float explosionRadius = 15f;
        [SerializeField] private ParticleSystem rocketTrail;
        [SerializeField] private GameObject explosionEffectPrefab;
        [SerializeField] private AudioClip launchSound;
        [SerializeField] private AudioClip explosionSound;

        private Rigidbody rb;
        private float launchTime;
        private bool burning = true;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            launchTime = Time.time;

            if (launchSound != null)
            {
                AudioSource.PlayClipAtPoint(launchSound, transform.position);
            }

            if (rocketTrail != null)
            {
                rocketTrail.Play();
            }
        }

        private void FixedUpdate()
        {
            if (burning && Time.time - launchTime < burnTime)
            {
                rb.AddForce(transform.forward * thrust, ForceMode.Force);
            }
            else if (burning)
            {
                burning = false;
                if (rocketTrail != null)
                {
                    rocketTrail.Stop();
                }
            }

            // Align with velocity
            if (rb.linearVelocity.magnitude > 1f)
            {
                transform.rotation = Quaternion.LookRotation(rb.linearVelocity);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            Explode();
        }

        private void Explode()
        {
            // Similar to bomb explosion but smaller
            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
            foreach (var col in colliders)
            {
                var health = col.GetComponentInParent<HealthSystem>();
                if (health != null)
                {
                    float distance = Vector3.Distance(transform.position, col.transform.position);
                    float falloff = 1f - (distance / explosionRadius);
                    float damage = explosiveFillerKg * 20f * Mathf.Clamp01(falloff);
                    health.TakeDamage(damage, Vehicles.DamageSource.Explosion);
                }
            }

            if (explosionEffectPrefab != null)
            {
                var effect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
                Destroy(effect, 5f);
            }

            if (explosionSound != null)
            {
                AudioSource.PlayClipAtPoint(explosionSound, transform.position);
            }

            Destroy(gameObject);
        }
    }
}
