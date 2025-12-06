using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Data
{
    /// <summary>
    /// ScriptableObject defining realistic aircraft weapon properties
    /// Based on War Thunder style weapons
    /// </summary>
    [CreateAssetMenu(fileName = "NewWeaponData", menuName = "EarthUnder/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        [Header("Weapon Identity")]
        public string weaponId;
        public string weaponName;
        public string designation; // e.g., "MG 151/20", "M2 Browning"
        public string manufacturer;
        public string countryOfOrigin;
        [TextArea(2, 4)]
        public string description;
        public Sprite icon;
        public WeaponType weaponType;
        public WeaponCategory category;

        [Header("Ballistics")]
        [Tooltip("Caliber in mm")]
        public float caliberMm = 20f;
        [Tooltip("Muzzle velocity in m/s")]
        public float muzzleVelocityMs = 800f;
        [Tooltip("Effective range in meters")]
        public float effectiveRangeM = 800f;
        [Tooltip("Maximum range in meters")]
        public float maxRangeM = 2000f;
        [Tooltip("Rate of fire in rounds per minute")]
        public float rateOfFireRPM = 600f;

        [Header("Damage")]
        [Tooltip("Base damage per round")]
        public float baseDamage = 15f;
        [Tooltip("Armor penetration in mm at 100m")]
        public float armorPenetrationMm = 15f;
        [Tooltip("Penetration falloff per 100m")]
        public float penetrationFalloff = 1f;
        public DamageType primaryDamageType;
        public bool hasExplosiveFiller = false;
        [Tooltip("Explosive filler in grams (if any)")]
        public float explosiveFillerG = 0f;

        [Header("Ammunition Types")]
        public List<AmmoBeltOption> availableAmmoTypes;
        public AmmoBeltType defaultBelt;

        [Header("Reliability")]
        [Tooltip("Chance of jam per 100 rounds")]
        public float jamChancePercent = 0.5f;
        [Tooltip("Overheating threshold (seconds of continuous fire)")]
        public float overheatTime = 10f;
        [Tooltip("Cooldown rate (seconds to cool)")]
        public float cooldownTime = 15f;

        [Header("Physical")]
        public float weightKg = 25f;
        public float recoilForce = 100f;

        [Header("Projectile Visuals")]
        public GameObject projectilePrefab;
        public GameObject muzzleFlashPrefab;
        public GameObject impactEffectPrefab;
        public Color tracerColor = Color.yellow;
        public int tracerInterval = 5; // Every Nth round is a tracer

        [Header("Audio")]
        public AudioClip fireSound;
        public AudioClip fireSoundLoop;
        public AudioClip impactSound;
        public AudioClip reloadSound;
        public AudioClip jamSound;

        [Header("Economy")]
        public int researchCost = 5000;
        public int purchaseCost = 2000;
        public int ammoCostPerRound = 1;
    }

    [System.Serializable]
    public class AmmoBeltOption
    {
        public AmmoBeltType beltType;
        public string beltName;
        public List<AmmoRoundType> roundSequence;
        public bool isDefault = false;
        public int researchCost = 0;
    }

    public enum WeaponType
    {
        // Machine Guns
        LightMachineGun,        // 7.62mm / .303 / 7.7mm
        HeavyMachineGun,        // 12.7mm / .50 cal / 13mm
        
        // Cannons
        Autocannon,             // 20mm
        HeavyAutocannon,        // 23mm - 30mm
        LargeCaliberCannon,     // 37mm+
        
        // Ordnance
        Bomb,
        UnGuidedRocket,
        GuidedMissile,
        Torpedo,
        
        // Other
        GunPod,
        DefensiveTurret
    }

    public enum WeaponCategory
    {
        Primary,
        Secondary,
        Suspended,
        Defensive
    }

    public enum DamageType
    {
        Kinetic,                // AP rounds
        HighExplosive,          // HE rounds
        Incendiary,             // I/IT rounds
        FragmentationHE,        // HEF rounds
        ArmorPiercingIncendiary,// API rounds
        ArmorPiercingHE,        // APHE rounds
        PracticeRound,          // P/Ball rounds
        Shrapnel                // Bomb fragments
    }

    public enum AmmoBeltType
    {
        Default,
        Universal,
        Tracers,
        Stealth,            // No tracers
        Ground_Targets,     // More AP
        Air_Targets,        // More HE/I
        Armored_Targets     // All AP
    }

    public enum AmmoRoundType
    {
        // 7.62mm / .303 / 7.7mm
        Ball_7_62mm,
        AP_7_62mm,
        I_7_62mm,           // Incendiary
        T_7_62mm,           // Tracer
        API_T_7_62mm,       // AP Incendiary Tracer
        
        // 12.7mm / .50 cal
        Ball_12_7mm,
        AP_12_7mm,
        API_12_7mm,
        I_12_7mm,
        T_12_7mm,
        APIT_12_7mm,        // AP Incendiary Tracer
        
        // 13mm
        AP_13mm,
        HEI_13mm,
        IT_13mm,
        
        // 20mm
        AP_20mm,
        API_20mm,
        HE_20mm,
        HEF_20mm,
        HEFI_20mm,          // HE Fragmentation Incendiary
        HEI_20mm,
        HEFI_T_20mm,
        FI_T_20mm,
        
        // 23mm
        AP_23mm,
        API_23mm,
        HEI_23mm,
        FI_T_23mm,
        
        // 30mm
        AP_I_30mm,
        HEI_30mm,
        HEFI_30mm,
        IT_30mm,
        Minengeschoss_30mm, // German mine shell
        
        // 37mm+
        AP_37mm,
        HE_37mm,
        HEFI_T_37mm,
        AP_45mm,
        HE_45mm
    }

    /// <summary>
    /// Database of real-world aircraft weapons with accurate stats
    /// </summary>
    public static class RealWeaponDatabase
    {
        public static WeaponStats GetWeaponStats(MachineGunType gunType)
        {
            return gunType switch
            {
                // USA
                MachineGunType.M2_Browning_12_7mm => new WeaponStats
                {
                    name = "M2 Browning .50 cal",
                    caliberMm = 12.7f,
                    rpmLow = 750f, rpmHigh = 850f,
                    muzzleVelocity = 890f,
                    effectiveRange = 1800f,
                    weightKg = 38f,
                    armorPenMm = 20f
                },
                MachineGunType.M1919_Browning_7_62mm => new WeaponStats
                {
                    name = "M1919 Browning .30 cal",
                    caliberMm = 7.62f,
                    rpmLow = 400f, rpmHigh = 600f,
                    muzzleVelocity = 853f,
                    effectiveRange = 1370f,
                    weightKg = 14f,
                    armorPenMm = 10f
                },
                
                // Germany
                MachineGunType.MG17_7_92mm => new WeaponStats
                {
                    name = "MG 17",
                    caliberMm = 7.92f,
                    rpmLow = 1100f, rpmHigh = 1200f,
                    muzzleVelocity = 905f,
                    effectiveRange = 1200f,
                    weightKg = 10.2f,
                    armorPenMm = 9f
                },
                MachineGunType.MG131_13mm => new WeaponStats
                {
                    name = "MG 131",
                    caliberMm = 13f,
                    rpmLow = 850f, rpmHigh = 930f,
                    muzzleVelocity = 750f,
                    effectiveRange = 1500f,
                    weightKg = 16.6f,
                    armorPenMm = 17f
                },
                
                // USSR
                MachineGunType.ShKAS_7_62mm => new WeaponStats
                {
                    name = "ShKAS",
                    caliberMm = 7.62f,
                    rpmLow = 1650f, rpmHigh = 1800f,
                    muzzleVelocity = 825f,
                    effectiveRange = 1000f,
                    weightKg = 10.5f,
                    armorPenMm = 8f
                },
                MachineGunType.UB_12_7mm => new WeaponStats
                {
                    name = "Berezin UB",
                    caliberMm = 12.7f,
                    rpmLow = 1000f, rpmHigh = 1050f,
                    muzzleVelocity = 814f,
                    effectiveRange = 1500f,
                    weightKg = 21.5f,
                    armorPenMm = 20f
                },
                
                // Britain
                MachineGunType.Browning_303 => new WeaponStats
                {
                    name = "Browning .303",
                    caliberMm = 7.7f,
                    rpmLow = 1100f, rpmHigh = 1150f,
                    muzzleVelocity = 845f,
                    effectiveRange = 1200f,
                    weightKg = 10f,
                    armorPenMm = 9f
                },
                
                _ => new WeaponStats { name = "Unknown", caliberMm = 7.62f }
            };
        }

        public static WeaponStats GetCannonStats(CannonType cannonType)
        {
            return cannonType switch
            {
                // Germany
                CannonType.MG_FF_20mm => new WeaponStats
                {
                    name = "MG FF",
                    caliberMm = 20f,
                    rpmLow = 520f, rpmHigh = 540f,
                    muzzleVelocity = 585f,
                    effectiveRange = 700f,
                    weightKg = 26f,
                    armorPenMm = 25f,
                    hasHE = true, explosiveFiller = 18f
                },
                CannonType.MG151_20mm => new WeaponStats
                {
                    name = "MG 151/20",
                    caliberMm = 20f,
                    rpmLow = 680f, rpmHigh = 750f,
                    muzzleVelocity = 785f,
                    effectiveRange = 800f,
                    weightKg = 42f,
                    armorPenMm = 30f,
                    hasHE = true, explosiveFiller = 20f
                },
                CannonType.MK108_30mm => new WeaponStats
                {
                    name = "MK 108",
                    caliberMm = 30f,
                    rpmLow = 600f, rpmHigh = 650f,
                    muzzleVelocity = 540f,
                    effectiveRange = 600f,
                    weightKg = 58f,
                    armorPenMm = 25f,
                    hasHE = true, explosiveFiller = 85f // Minengeschoss
                },
                
                // USSR
                CannonType.ShVAK_20mm => new WeaponStats
                {
                    name = "ShVAK",
                    caliberMm = 20f,
                    rpmLow = 750f, rpmHigh = 800f,
                    muzzleVelocity = 815f,
                    effectiveRange = 800f,
                    weightKg = 42f,
                    armorPenMm = 28f,
                    hasHE = true, explosiveFiller = 8f
                },
                CannonType.VYa_23mm => new WeaponStats
                {
                    name = "VYa-23",
                    caliberMm = 23f,
                    rpmLow = 550f, rpmHigh = 600f,
                    muzzleVelocity = 905f,
                    effectiveRange = 1000f,
                    weightKg = 66f,
                    armorPenMm = 35f,
                    hasHE = true, explosiveFiller = 15f
                },
                CannonType.NS37_37mm => new WeaponStats
                {
                    name = "NS-37",
                    caliberMm = 37f,
                    rpmLow = 240f, rpmHigh = 260f,
                    muzzleVelocity = 900f,
                    effectiveRange = 1200f,
                    weightKg = 150f,
                    armorPenMm = 50f,
                    hasHE = true, explosiveFiller = 34f
                },
                
                // Britain
                CannonType.Hispano_Mk_II_20mm => new WeaponStats
                {
                    name = "Hispano Mk.II",
                    caliberMm = 20f,
                    rpmLow = 600f, rpmHigh = 650f,
                    muzzleVelocity = 880f,
                    effectiveRange = 900f,
                    weightKg = 50f,
                    armorPenMm = 28f,
                    hasHE = true, explosiveFiller = 11f
                },
                CannonType.ADEN_30mm => new WeaponStats
                {
                    name = "ADEN",
                    caliberMm = 30f,
                    rpmLow = 1200f, rpmHigh = 1400f,
                    muzzleVelocity = 790f,
                    effectiveRange = 1000f,
                    weightKg = 87f,
                    armorPenMm = 45f,
                    hasHE = true, explosiveFiller = 40f
                },
                
                // USA
                CannonType.AN_M2_20mm => new WeaponStats
                {
                    name = "AN/M2",
                    caliberMm = 20f,
                    rpmLow = 600f, rpmHigh = 650f,
                    muzzleVelocity = 840f,
                    effectiveRange = 800f,
                    weightKg = 49f,
                    armorPenMm = 26f,
                    hasHE = true, explosiveFiller = 10f
                },
                CannonType.M39_20mm => new WeaponStats
                {
                    name = "M39",
                    caliberMm = 20f,
                    rpmLow = 1500f, rpmHigh = 1700f,
                    muzzleVelocity = 1030f,
                    effectiveRange = 1000f,
                    weightKg = 80f,
                    armorPenMm = 32f,
                    hasHE = true, explosiveFiller = 12f
                },
                
                // Japan
                CannonType.Type99_Mk2_20mm => new WeaponStats
                {
                    name = "Type 99 Model 2",
                    caliberMm = 20f,
                    rpmLow = 700f, rpmHigh = 750f,
                    muzzleVelocity = 750f,
                    effectiveRange = 750f,
                    weightKg = 34f,
                    armorPenMm = 24f,
                    hasHE = true, explosiveFiller = 14f
                },
                CannonType.Ho155_30mm => new WeaponStats
                {
                    name = "Ho-155",
                    caliberMm = 30f,
                    rpmLow = 400f, rpmHigh = 450f,
                    muzzleVelocity = 720f,
                    effectiveRange = 800f,
                    weightKg = 45f,
                    armorPenMm = 30f,
                    hasHE = true, explosiveFiller = 32f
                },
                
                _ => new WeaponStats { name = "Unknown", caliberMm = 20f }
            };
        }
    }

    [System.Serializable]
    public class WeaponStats
    {
        public string name;
        public float caliberMm;
        public float rpmLow;
        public float rpmHigh;
        public float muzzleVelocity;
        public float effectiveRange;
        public float weightKg;
        public float armorPenMm;
        public bool hasHE;
        public float explosiveFiller;

        public float AverageRPM => (rpmLow + rpmHigh) / 2f;
        public float FireInterval => 60f / AverageRPM;
    }
}
