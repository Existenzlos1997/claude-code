using UnityEngine;

namespace EarthUnderFreelancer.Data
{
    /// <summary>
    /// ScriptableObject defining weapon properties
    /// </summary>
    [CreateAssetMenu(fileName = "NewWeaponData", menuName = "EarthUnder/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        [Header("Identity")]
        public string weaponId;
        public string weaponName;
        [TextArea(2, 4)]
        public string description;
        public Sprite icon;
        public WeaponType weaponType;
        public DamageType damageType;

        [Header("Combat Stats")]
        public float damage = 10f;
        public float fireRate = 0.2f;
        public float range = 500f;
        public float projectileSpeed = 300f;
        public float accuracy = 0.98f;

        [Header("Energy")]
        public float energyCost = 5f;
        public float heatGenerated = 10f;

        [Header("Projectile")]
        public GameObject projectilePrefab;
        public GameObject muzzleFlashPrefab;
        public GameObject impactEffectPrefab;
        public bool isHoming = false;
        public float explosionRadius = 0f;

        [Header("Audio")]
        public AudioClip fireSound;
        public AudioClip impactSound;

        [Header("Economy")]
        public int purchasePrice = 1000;
        public int sellPrice = 500;
    }

    public enum WeaponType
    {
        Laser,
        Plasma,
        Kinetic,
        Missile,
        Torpedo,
        Mine,
        Beam,
        Railgun
    }

    public enum DamageType
    {
        Kinetic,
        Energy,
        Explosive,
        EMP,
        Thermal
    }
}
