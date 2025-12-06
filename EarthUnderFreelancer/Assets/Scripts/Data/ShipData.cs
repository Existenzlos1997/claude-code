using UnityEngine;

namespace EarthUnderFreelancer.Data
{
    /// <summary>
    /// ScriptableObject defining ship statistics and properties
    /// </summary>
    [CreateAssetMenu(fileName = "NewShipData", menuName = "EarthUnder/Ship Data")]
    public class ShipData : ScriptableObject
    {
        [Header("Identity")]
        public string shipId;
        public string shipName;
        [TextArea(2, 4)]
        public string description;
        public Sprite icon;
        public GameObject prefab;
        public ShipClass shipClass;

        [Header("Performance")]
        public float maxSpeed = 100f;
        public float acceleration = 50f;
        public float turnRate = 60f;
        public float strafeSpeed = 30f;
        public float boostMultiplier = 1.5f;
        public float boostDuration = 5f;
        public float boostRechargeRate = 1f;

        [Header("Durability")]
        public float maxHull = 100f;
        public float maxShield = 50f;
        public float shieldRegenRate = 5f;
        public float shieldRegenDelay = 3f;
        public float armor = 0f;

        [Header("Weapons")]
        public int weaponSlots = 2;
        public int missileSlots = 1;
        public int turretSlots = 0;

        [Header("Cargo")]
        public int cargoCapacity = 50;
        public int fuelCapacity = 100;
        public float fuelConsumption = 1f;

        [Header("Economy")]
        public int purchasePrice = 10000;
        public int sellPrice = 7000;
        public int repairCostPerPoint = 10;

        [Header("Visual")]
        public Vector3 cameraOffset = new Vector3(0, 3, -10);
        public Vector3 cockpitPosition = new Vector3(0, 0.5f, 2);
    }

    public enum ShipClass
    {
        Scout,
        Fighter,
        Interceptor,
        Bomber,
        Freighter,
        Cruiser,
        Battleship,
        Carrier
    }
}
