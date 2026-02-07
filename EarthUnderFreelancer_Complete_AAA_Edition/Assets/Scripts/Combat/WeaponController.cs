using UnityEngine;
using System.Collections.Generic;
using EarthUnderFreelancer.Core;
using EarthUnderFreelancer.Vehicles;

namespace EarthUnderFreelancer.Combat
{
    /// <summary>
    /// Handles weapon systems and firing for vehicles
    /// </summary>
    public class WeaponController : MonoBehaviour
    {
        [Header("Weapon Mounts")]
        [SerializeField] private List<WeaponMount> weaponMounts = new List<WeaponMount>();
        [SerializeField] private Transform[] missileMounts;

        [Header("Targeting")]
        [SerializeField] private float targetingRange = 500f;
        [SerializeField] private float lockOnTime = 2f;
        [SerializeField] private LayerMask targetableLayers;

        [Header("Current Weapons")]
        [SerializeField] private WeaponData primaryWeapon;
        [SerializeField] private WeaponData secondaryWeapon;
        [SerializeField] private int missileCount = 4;

        private Transform currentTarget;
        private List<Transform> potentialTargets = new List<Transform>();
        private int currentTargetIndex = 0;
        private float lockOnProgress = 0f;
        private bool isLockedOn = false;

        private float primaryCooldown = 0f;
        private float secondaryCooldown = 0f;
        private int currentWeaponIndex = 0;

        public Transform CurrentTarget => currentTarget;
        public bool IsLockedOn => isLockedOn;
        public float LockOnProgress => lockOnProgress;
        public int MissileCount => missileCount;

        private void Update()
        {
            UpdateCooldowns();
            UpdateTargeting();
        }

        private void UpdateCooldowns()
        {
            if (primaryCooldown > 0) primaryCooldown -= Time.deltaTime;
            if (secondaryCooldown > 0) secondaryCooldown -= Time.deltaTime;
        }

        private void UpdateTargeting()
        {
            if (currentTarget == null)
            {
                lockOnProgress = 0f;
                isLockedOn = false;
                return;
            }

            float distance = Vector3.Distance(transform.position, currentTarget.position);
            if (distance > targetingRange)
            {
                currentTarget = null;
                lockOnProgress = 0f;
                isLockedOn = false;
                return;
            }

            Vector3 dirToTarget = (currentTarget.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dirToTarget);
            
            if (angle < 30f)
            {
                lockOnProgress += Time.deltaTime / lockOnTime;
                if (lockOnProgress >= 1f)
                {
                    lockOnProgress = 1f;
                    if (!isLockedOn)
                    {
                        isLockedOn = true;
                        AudioManager.Instance?.PlayMissileAlert();
                    }
                }
            }
            else
            {
                lockOnProgress = Mathf.Max(0, lockOnProgress - Time.deltaTime * 2f);
                isLockedOn = false;
            }
        }

        public void FirePrimary()
        {
            if (primaryCooldown > 0 || primaryWeapon == null) return;

            foreach (var mount in weaponMounts)
            {
                if (mount.weaponType == WeaponType.Primary)
                {
                    FireWeapon(mount, primaryWeapon);
                }
            }

            primaryCooldown = primaryWeapon.fireRate;
            EventManager.TriggerEvent(GameEvents.WEAPON_FIRED, primaryWeapon);
        }

        public void FireSecondary()
        {
            if (secondaryCooldown > 0) return;

            if (missileCount > 0 && isLockedOn && currentTarget != null)
            {
                FireMissile();
                missileCount--;
                secondaryCooldown = 1f;
            }
            else if (secondaryWeapon != null)
            {
                foreach (var mount in weaponMounts)
                {
                    if (mount.weaponType == WeaponType.Secondary)
                    {
                        FireWeapon(mount, secondaryWeapon);
                    }
                }
                secondaryCooldown = secondaryWeapon.fireRate;
            }
        }

        private void FireWeapon(WeaponMount mount, WeaponData weapon)
        {
            if (mount.firePoint == null) return;

            Vector3 aimDirection = mount.firePoint.forward;
            
            float spread = weapon.accuracy;
            aimDirection += new Vector3(
                Random.Range(-spread, spread),
                Random.Range(-spread, spread),
                Random.Range(-spread, spread)
            );

            if (weapon.projectilePrefab != null)
            {
                GameObject projectile = ObjectPool.Instance != null
                    ? ObjectPool.Instance.SpawnFromPool(weapon.weaponId, mount.firePoint.position, Quaternion.LookRotation(aimDirection))
                    : Instantiate(weapon.projectilePrefab, mount.firePoint.position, Quaternion.LookRotation(aimDirection));

                if (projectile != null)
                {
                    Projectile proj = projectile.GetComponent<Projectile>();
                    proj?.Initialize(gameObject, weapon.damage, weapon.projectileSpeed, weapon.range);
                }
            }

            if (mount.muzzleFlash != null) mount.muzzleFlash.Play();
            AudioManager.Instance?.PlayLaserFire();
        }

        private void FireMissile()
        {
            if (missileMounts == null || missileMounts.Length == 0) return;

            Transform mount = missileMounts[missileCount % missileMounts.Length];
            
            GameObject missilePrefab = Resources.Load<GameObject>("Prefabs/Missile");
            if (missilePrefab != null)
            {
                GameObject missile = Instantiate(missilePrefab, mount.position, mount.rotation);
                Missile missileComponent = missile.GetComponent<Missile>();
                missileComponent?.Initialize(currentTarget, gameObject, 100f);
            }

            AudioManager.Instance?.PlayMissileFire();
        }

        public void CycleTarget()
        {
            FindPotentialTargets();
            
            if (potentialTargets.Count == 0)
            {
                currentTarget = null;
                return;
            }

            currentTargetIndex = (currentTargetIndex + 1) % potentialTargets.Count;
            currentTarget = potentialTargets[currentTargetIndex];
            lockOnProgress = 0f;
            isLockedOn = false;
        }

        public void CycleWeapon()
        {
            currentWeaponIndex++;
        }

        private void FindPotentialTargets()
        {
            potentialTargets.Clear();

            Collider[] hits = Physics.OverlapSphere(transform.position, targetingRange, targetableLayers);
            
            foreach (var hit in hits)
            {
                if (hit.transform == transform) continue;

                if (hit.CompareTag("Enemy") || hit.CompareTag("Player"))
                {
                    if (hit.gameObject != gameObject)
                    {
                        potentialTargets.Add(hit.transform);
                    }
                }
            }

            potentialTargets.Sort((a, b) =>
            {
                float distA = Vector3.Distance(transform.position, a.position);
                float distB = Vector3.Distance(transform.position, b.position);
                return distA.CompareTo(distB);
            });
        }

        public void SetTarget(Transform target) { currentTarget = target; lockOnProgress = 0f; isLockedOn = false; }
        public void ClearTarget() { currentTarget = null; lockOnProgress = 0f; isLockedOn = false; }
        public void EquipWeapon(WeaponData weapon, bool primary) { if (primary) primaryWeapon = weapon; else secondaryWeapon = weapon; }
        public void AddMissiles(int count) => missileCount += count;
        public void SetMissiles(int count) => missileCount = count;
    }

    [System.Serializable]
    public class WeaponMount
    {
        public string mountName;
        public Transform firePoint;
        public WeaponType weaponType;
        public ParticleSystem muzzleFlash;
    }

    public enum WeaponType { Primary, Secondary, Turret }

    [CreateAssetMenu(fileName = "NewWeapon", menuName = "EarthUnder/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        public string weaponId;
        public string weaponName;
        public string description;
        public WeaponCategory category;
        public Sprite icon;

        [Header("Stats")]
        public float damage = 10f;
        public float fireRate = 0.2f;
        public float range = 300f;
        public float projectileSpeed = 200f;
        public float accuracy = 0.02f;

        [Header("Prefabs")]
        public GameObject projectilePrefab;
        public GameObject muzzleFlashPrefab;

        [Header("Economy")]
        public int purchasePrice = 1000;
        public int sellPrice = 500;

        [Header("Requirements")]
        public int levelRequired = 1;
        public VehicleType[] compatibleVehicles;
    }

    public enum WeaponCategory { Laser, Plasma, Kinetic, Missile, Torpedo, Mine }
}
