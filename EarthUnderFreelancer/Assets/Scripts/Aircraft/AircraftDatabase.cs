using System;
using System.Collections.Generic;
using UnityEngine;

namespace EarthUnderFreelancer.Aircraft
{
    /// <summary>
    /// Aircraft generation and era classification
    /// </summary>
    public enum AircraftGeneration
    {
        WWI,           // 1914-1918
        Interwar,      // 1919-1939
        WWII,          // 1939-1945
        FirstGen,      // 1945-1955
        SecondGen,     // 1955-1960
        ThirdGen,      // 1960-1970
        FourthGen,     // 1970-1990
        FourthGenPlus, // 1990-2005
        FifthGen,      // 2005+
        SixthGen       // Future
    }

    /// <summary>
    /// Aircraft role classification
    /// </summary>
    public enum AircraftRole
    {
        Fighter,
        Interceptor,
        Multirole,
        AttackAircraft,
        Bomber,
        StrategicBomber,
        TacticalBomber,
        DiveBomber,
        TorpedoBomber,
        FighterBomber,
        GroundAttack,
        Reconnaissance,
        AWACS,
        ElectronicWarfare,
        Trainer,
        Transport,
        CargoHeavy,
        CargoMedium,
        CargoLight,
        PassengerWide,
        PassengerNarrow,
        PassengerRegional,
        PassengerPrivate,
        Tanker,
        Maritime,
        Helicopter,
        VTOL,
        Stealth
    }

    /// <summary>
    /// Weapon system types
    /// </summary>
    [Serializable]
    public class WeaponSystem
    {
        public string Name;
        public string Type; // Gun, Missile, Bomb, Rocket
        public int Quantity;
        public float Damage;
        public float Range;
        public float RateOfFire;
        public float Accuracy;
    }

    /// <summary>
    /// Aircraft modification/upgrade
    /// </summary>
    [Serializable]
    public class AircraftModification
    {
        public string Name;
        public string Description;
        public int Cost;
        public float SpeedBonus;
        public float ArmorBonus;
        public float WeaponBonus;
        public float FuelEfficiencyBonus;
    }

    /// <summary>
    /// Complete aircraft data structure
    /// </summary>
    [Serializable]
    public class AircraftData
    {
        // Basic Info
        public string Name;
        public string Variant;
        public string Country;
        public string Manufacturer;
        public int YearIntroduced;
        public AircraftGeneration Generation;
        public AircraftRole Role;
        public string Description;
        
        // Performance
        public float MaxSpeed; // km/h
        public float CruiseSpeed; // km/h
        public float MaxAltitude; // meters
        public float RateOfClimb; // m/s
        public float Range; // km
        public float FerryRange; // km
        public float ServiceCeiling; // meters
        
        // Physical
        public float Length; // meters
        public float Wingspan; // meters
        public float Height; // meters
        public float EmptyWeight; // kg
        public float MaxTakeoffWeight; // kg
        public float FuelCapacity; // liters
        
        // Combat
        public int Crew;
        public float ArmorRating;
        public float Maneuverability;
        public List<WeaponSystem> Weapons;
        public List<string> Hardpoints;
        
        // Economics
        public int PurchasePrice;
        public int MaintenanceCost;
        public int RepairCost;
        
        // Upgrades
        public List<AircraftModification> AvailableModifications;
        
        // Game Stats
        public int UnlockLevel;
        public int ResearchPoints;
        public string[] Prerequisites;
    }

    /// <summary>
    /// Massive aircraft database with 500+ real aircraft
    /// </summary>
    [CreateAssetMenu(fileName = "CompleteAircraftDatabase", menuName = "EarthUnder/Aircraft Database")]
    public class CompleteAircraftDatabase : ScriptableObject
    {
        public List<AircraftData> AllAircraft = new List<AircraftData>();
        
        private Dictionary<string, AircraftData> _aircraftLookup;
        
        public void Initialize()
        {
            _aircraftLookup = new Dictionary<string, AircraftData>();
            foreach (var aircraft in AllAircraft)
            {
                string key = aircraft.Name + (string.IsNullOrEmpty(aircraft.Variant) ? "" : " " + aircraft.Variant);
                _aircraftLookup[key] = aircraft;
            }
        }
        
        public AircraftData GetAircraft(string name, string variant = "")
        {
            string key = name + (string.IsNullOrEmpty(variant) ? "" : " " + variant);
            return _aircraftLookup.TryGetValue(key, out var aircraft) ? aircraft : null;
        }
        
        public List<AircraftData> GetAircraftByGeneration(AircraftGeneration gen)
        {
            return AllAircraft.FindAll(a => a.Generation == gen);
        }
        
        public List<AircraftData> GetAircraftByRole(AircraftRole role)
        {
            return AllAircraft.FindAll(a => a.Role == role);
        }
        
        public List<AircraftData> GetAircraftByCountry(string country)
        {
            return AllAircraft.FindAll(a => a.Country == country);
        }
    }
}
