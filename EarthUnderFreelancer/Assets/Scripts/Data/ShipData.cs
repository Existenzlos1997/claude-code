using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Data
{
    /// <summary>
    /// ScriptableObject defining real aircraft statistics and properties
    /// Based on War Thunder style realistic aircraft
    /// </summary>
    [CreateAssetMenu(fileName = "NewAircraftData", menuName = "EarthUnder/Aircraft Data")]
    public class ShipData : ScriptableObject
    {
        [Header("Aircraft Identity")]
        public string aircraftId;
        public string aircraftName;
        public string manufacturer;
        public string countryOfOrigin;
        public int yearIntroduced;
        [TextArea(2, 4)]
        public string description;
        public Sprite icon;
        public GameObject prefab;
        public AircraftType aircraftType;
        public AircraftEra era;
        public Nation nation;

        [Header("Flight Performance")]
        [Tooltip("Maximum speed in km/h")]
        public float maxSpeedKmh = 600f;
        [Tooltip("Climb rate in m/s")]
        public float climbRateMs = 20f;
        [Tooltip("Turn time in seconds for 360°")]
        public float turnTime = 20f;
        [Tooltip("Roll rate in degrees/second")]
        public float rollRate = 100f;
        [Tooltip("Stall speed in km/h")]
        public float stallSpeedKmh = 150f;
        [Tooltip("Maximum G-force before structural damage")]
        public float maxGForce = 12f;
        [Tooltip("Wing rip speed in km/h")]
        public float wingRipSpeedKmh = 800f;
        
        [Header("Engine")]
        public EngineType engineType;
        public int engineCount = 1;
        public float enginePowerHP = 1200f;
        [Tooltip("Boost/WEP duration in seconds")]
        public float wepDuration = 180f;
        [Tooltip("Fuel capacity in minutes at normal power")]
        public float fuelMinutes = 45f;

        [Header("Structural")]
        [Tooltip("Structural integrity / HP")]
        public float structuralIntegrity = 100f;
        [Tooltip("Armor thickness in mm")]
        public float armorMm = 8f;
        [Tooltip("Self-sealing fuel tanks")]
        public bool selfSealingTanks = true;
        [Tooltip("Pilot armor protection")]
        public float pilotArmorMm = 12f;
        public float emptyWeightKg = 3000f;
        public float maxTakeoffWeightKg = 4500f;

        [Header("Armament - Machine Guns")]
        public List<MachineGunMount> machineGuns;

        [Header("Armament - Cannons")]
        public List<CannonMount> cannons;

        [Header("Armament - Ordnance")]
        public List<OrdnanceMount> ordnance;

        [Header("Modules & Crew")]
        public int crewCount = 1;
        public bool hasGunner = false;
        public bool hasRadar = false;
        public bool hasAirbrake = false;
        public bool hasLeadingEdgeSlats = false;
        public bool hasRetractableGear = true;

        [Header("Battle Rating")]
        [Range(1.0f, 12.0f)]
        public float battleRating = 3.0f;
        public int researchCost = 10000;
        public int purchaseCost = 50000;
        public int repairCostSL = 5000;
        public int crewTrainingCost = 10000;

        [Header("Visual")]
        public Vector3 cameraOffset = new Vector3(0, 3, -15);
        public Vector3 cockpitPosition = new Vector3(0, 0.5f, 2);
        public float wingspan = 12f;
        public float fuselageLength = 10f;
    }

    [System.Serializable]
    public class MachineGunMount
    {
        public MachineGunType gunType;
        public int count = 2;
        public int ammoPerGun = 500;
        public MountLocation location;
    }

    [System.Serializable]
    public class CannonMount
    {
        public CannonType cannonType;
        public int count = 1;
        public int ammoPerCannon = 60;
        public MountLocation location;
    }

    [System.Serializable]
    public class OrdnanceMount
    {
        public OrdnanceType ordnanceType;
        public int maxCount = 2;
        public MountLocation location;
    }

    public enum MountLocation
    {
        NoseMounted,
        WingMounted,
        FuselageMounted,
        EngineGondola,
        TurretTop,
        TurretBelly,
        TurretTail
    }

    public enum AircraftType
    {
        // Fighters
        Fighter,
        Interceptor,
        StrikeFighter,
        NavalFighter,
        JetFighter,
        
        // Attackers
        Attacker,
        FighterBomber,
        DiveBomber,
        TorpedoBomber,
        JetAttacker,
        
        // Bombers
        LightBomber,
        MediumBomber,
        HeavyBomber,
        StrategicBomber,
        JetBomber,
        
        // Other
        Reconnaissance,
        Transport,
        Trainer
    }

    public enum AircraftEra
    {
        WWI,            // 1914-1918
        Interwar,       // 1919-1938
        EarlyWWII,      // 1939-1941
        MidWWII,        // 1942-1943
        LateWWII,       // 1944-1945
        EarlyJet,       // 1946-1955
        ColdWar,        // 1956-1975
        Modern          // 1976+
    }

    public enum Nation
    {
        USA,
        Germany,
        USSR,
        Britain,
        Japan,
        Italy,
        France,
        Sweden,
        China,
        Israel
    }

    public enum EngineType
    {
        RadialPiston,
        InlinePiston,
        VeePiston,
        TurboProp,
        TurboJet,
        TurboFan,
        Afterburner
    }

    public enum MachineGunType
    {
        // USA
        M2_Browning_12_7mm,     // .50 cal - 750-850 rpm
        M1919_Browning_7_62mm,  // .30 cal - 400-600 rpm
        
        // Germany
        MG17_7_92mm,            // 1200 rpm
        MG81_7_92mm,            // 1600 rpm
        MG131_13mm,             // 900 rpm
        
        // USSR
        ShKAS_7_62mm,           // 1800 rpm
        UB_12_7mm,              // 1050 rpm
        
        // Britain
        Browning_303,           // .303 - 1150 rpm
        
        // Japan
        Type89_7_7mm,           // 900 rpm
        Ho103_12_7mm,           // 900 rpm
        
        // Italy
        Breda_SAFAT_7_7mm,      // 900 rpm
        Breda_SAFAT_12_7mm      // 700 rpm
    }

    public enum CannonType
    {
        // USA
        M4_37mm,                // 150 rpm
        AN_M2_20mm,             // 600 rpm
        M39_20mm,               // 1500 rpm
        
        // Germany
        MG_FF_20mm,             // 520 rpm
        MG151_20mm,             // 700 rpm
        MK108_30mm,             // 650 rpm
        MK103_30mm,             // 380-420 rpm
        BK37_37mm,              // 160 rpm
        
        // USSR
        ShVAK_20mm,             // 800 rpm
        VYa_23mm,               // 550-600 rpm
        NS37_37mm,              // 250 rpm
        N45_45mm,               // 260 rpm
        
        // Britain
        Hispano_Mk_II_20mm,     // 600 rpm
        Hispano_Mk_V_20mm,      // 750 rpm
        ADEN_30mm,              // 1200-1700 rpm
        
        // Japan
        Type99_Mk1_20mm,        // 490 rpm
        Type99_Mk2_20mm,        // 750 rpm
        Ho5_20mm,               // 850 rpm
        Ho155_30mm,             // 450 rpm
        
        // France
        Hispano_404_20mm        // 700 rpm
    }

    public enum OrdnanceType
    {
        // Bombs - Light
        Bomb_50kg,
        Bomb_100lb,
        Bomb_250lb,
        
        // Bombs - Medium
        Bomb_250kg,
        Bomb_500lb,
        SC500_500kg,
        
        // Bombs - Heavy
        Bomb_1000lb,
        SC1000_1000kg,
        Bomb_2000lb,
        
        // Rockets - Unguided
        HVAR_127mm,             // US High Velocity Aircraft Rocket
        RS82_Rocket,            // Soviet 82mm rocket
        RS132_Rocket,           // Soviet 132mm rocket
        RP3_Rocket,             // British 3-inch rocket
        Wfr_Gr21_210mm,         // German 210mm rocket
        
        // Guided Missiles (Late War / Post-War)
        AIM9_Sidewinder,
        AIM7_Sparrow,
        R60_Aphid,
        
        // Torpedoes
        Mk13_Torpedo,
        Type91_Torpedo,
        F5W_Torpedo,
        
        // Gun Pods
        GunPod_20mm,
        GunPod_30mm,
        
        // Fuel Tanks
        DropTank_Small,
        DropTank_Medium,
        DropTank_Large
    }
}
