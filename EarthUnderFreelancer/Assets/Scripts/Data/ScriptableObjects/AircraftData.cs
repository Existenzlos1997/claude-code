using UnityEngine;

namespace EarthUnderFreelancer.Data
{
    /// <summary>
    /// ScriptableObject for individual aircraft data
    /// Create instances via: Create > EarthUnderFreelancer > Aircraft Data
    /// </summary>
    [CreateAssetMenu(fileName = "New Aircraft", menuName = "EarthUnderFreelancer/Aircraft Data", order = 1)]
    public class AircraftData : ScriptableObject
    {
        [Header("Basic Information")]
        public string aircraftName = "Aircraft Name";
        public string designation = "F-16C";
        public string manufacturer = "Lockheed Martin";
        public int yearIntroduced = 1978;
        public string era = "Modern";
        public string aircraftType = "Fighter";

        [Header("Performance")]
        public float maxSpeed = 2124f;
        public float cruiseSpeed = 900f;
        public float maxAltitude = 15000f;
        public float range = 3200f;
        public float climbRate = 254f;

        [Header("Physical Properties")]
        public float length = 15f;
        public float wingspan = 10f;
        public float height = 5f;
        public float emptyWeight = 8500f;
        public float maxTakeoffWeight = 19200f;

        [Header("Combat Stats")]
        public int hardpoints = 11;
        public float maxWeaponLoad = 7700f;
        public string[] weaponTypes = new string[] { "AIM-120", "AIM-9", "AGM-65", "Bombs" };
        public int armor = 50;
        public int health = 1000;

        [Header("Crew")]
        public int crewCapacity = 1;
        public int passengerCapacity = 0;
        public int cargoCapacity = 0;

        [Header("Faction & Availability")]
        public string primaryFaction = "Allied";
        public string[] availableToFactions = new string[] { "Allied", "Neutral" };
        public int unlockLevel = 1;
        public int purchaseCost = 10000;
        public int maintenanceCost = 500;

        [Header("Visual & Audio")]
        public GameObject prefab;
        public Sprite icon;
        public AudioClip engineSound;
        public AudioClip weaponSound;

        [Header("Description")]
        [TextArea(3, 10)]
        public string description = "Aircraft description and history...";
    }
}
