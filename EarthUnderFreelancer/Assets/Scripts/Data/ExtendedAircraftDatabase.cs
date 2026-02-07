using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EarthUnderFreelancer.Data
{
    /// <summary>
    /// Extended aircraft database with more nations and modern jets
    /// Includes Korean War, Cold War, and modern aircraft
    /// </summary>
    
    #region Korean War Era Aircraft
    
    public static class KoreanWarAircraft
    {
        public static AircraftSpecs F86F_Sabre => new AircraftSpecs
        {
            id = "f86f_sabre",
            name = "F-86F Sabre",
            manufacturer = "North American Aviation",
            nation = Nation.USA,
            type = AircraftType.Jet_Fighter,
            era = AircraftEra.KoreanWar,
            yearIntroduced = 1949,
            description = "The legendary Sabre. Dominant air superiority fighter of the Korean War era.",
            
            maxSpeedKmh = 1105f,
            maxSpeedAltitudeM = 0f, // Sea level
            climbRateMs = 42.7f,
            turnTime360 = 21.0f,
            rollRate = 140f,
            stallSpeedKmh = 205f,
            maxGForce = 7.33f,
            wingRipSpeedKmh = 1170f,
            
            engine = new EngineSpecs
            {
                name = "General Electric J47-GE-27",
                type = EngineType.Turbojet,
                thrustKN = 26.3f,
                thrustKN_AB = 0f // No afterburner
            },
            
            emptyWeightKg = 5046f,
            maxTakeoffWeightKg = 9350f,
            wingspanM = 11.3f,
            lengthM = 11.43f,
            heightM = 4.47f,
            wingAreaM2 = 27.76f,
            fuelCapacityL = 1815f,
            
            armament = new ArmamentSpecs
            {
                machineGuns = new List<GunMount>
                {
                    new GunMount { gunType = "M3 Browning .50 cal", count = 6, ammoTotal = 1800, rateOfFire = 1200 }
                },
                rockets = new List<OrdnanceMount>
                {
                    new OrdnanceMount { type = "HVAR 5-inch", count = 16, location = "Underwing" }
                },
                bombs = new List<OrdnanceMount>
                {
                    new OrdnanceMount { type = "1000 lb bomb", count = 2, location = "Underwing" }
                }
            },
            
            battleRating = 8.3f,
            researchRP = 380000,
            purchaseSL = 860000
        };
        
        public static AircraftSpecs MiG15bis => new AircraftSpecs
        {
            id = "mig15bis",
            name = "MiG-15bis",
            manufacturer = "Mikoyan-Gurevich",
            nation = Nation.USSR,
            type = AircraftType.Jet_Fighter,
            era = AircraftEra.KoreanWar,
            yearIntroduced = 1950,
            description = "Soviet jet fighter that shocked the West. Excellent climb rate and firepower.",
            
            maxSpeedKmh = 1075f,
            maxSpeedAltitudeM = 3000f,
            climbRateMs = 50.0f,
            turnTime360 = 19.7f,
            rollRate = 120f,
            stallSpeedKmh = 195f,
            maxGForce = 8f,
            wingRipSpeedKmh = 1132f,
            
            engine = new EngineSpecs
            {
                name = "Klimov VK-1",
                type = EngineType.Turbojet,
                thrustKN = 26.5f
            },
            
            emptyWeightKg = 3630f,
            maxTakeoffWeightKg = 6105f,
            wingspanM = 10.08f,
            lengthM = 10.1f,
            heightM = 3.7f,
            wingAreaM2 = 20.6f,
            fuelCapacityL = 1400f,
            
            armament = new ArmamentSpecs
            {
                cannons = new List<GunMount>
                {
                    new GunMount { gunType = "N-37D 37mm", count = 1, ammoTotal = 40, rateOfFire = 400 },
                    new GunMount { gunType = "NR-23 23mm", count = 2, ammoTotal = 160, rateOfFire = 650 }
                }
            },
            
            battleRating = 8.3f,
            researchRP = 380000,
            purchaseSL = 860000
        };
        
        public static AircraftSpecs F9F_Panther => new AircraftSpecs
        {
            id = "f9f5_panther",
            name = "F9F-5 Panther",
            manufacturer = "Grumman",
            nation = Nation.USA,
            type = AircraftType.Jet_Fighter,
            era = AircraftEra.KoreanWar,
            yearIntroduced = 1949,
            description = "US Navy jet fighter. Reliable and versatile carrier-based aircraft.",
            
            maxSpeedKmh = 932f,
            climbRateMs = 26.4f,
            turnTime360 = 24.0f,
            stallSpeedKmh = 190f,
            maxGForce = 7.5f,
            
            engine = new EngineSpecs
            {
                name = "Pratt & Whitney J48-P-6A",
                type = EngineType.Turbojet,
                thrustKN = 30.3f
            },
            
            emptyWeightKg = 4220f,
            maxTakeoffWeightKg = 8845f,
            
            armament = new ArmamentSpecs
            {
                machineGuns = new List<GunMount>
                {
                    new GunMount { gunType = "M3 Browning 20mm", count = 4, ammoTotal = 760 }
                },
                bombs = new List<OrdnanceMount>
                {
                    new OrdnanceMount { type = "1000 lb bomb", count = 2 }
                }
            },
            
            battleRating = 7.7f
        };
        
        public static AircraftSpecs Meteor_F8 => new AircraftSpecs
        {
            id = "meteor_f8",
            name = "Gloster Meteor F.8",
            manufacturer = "Gloster Aircraft Company",
            nation = Nation.UK,
            type = AircraftType.Jet_Fighter,
            era = AircraftEra.KoreanWar,
            yearIntroduced = 1949,
            description = "British jet fighter. First Allied jet to see combat in WWII, improved for Korea.",
            
            maxSpeedKmh = 965f,
            climbRateMs = 40.1f,
            turnTime360 = 22.5f,
            stallSpeedKmh = 185f,
            maxGForce = 8f,
            
            engine = new EngineSpecs
            {
                name = "Rolls-Royce Derwent 8",
                type = EngineType.Turbojet,
                thrustKN = 15.6f
            },
            
            emptyWeightKg = 4846f,
            maxTakeoffWeightKg = 7121f,
            
            armament = new ArmamentSpecs
            {
                cannons = new List<GunMount>
                {
                    new GunMount { gunType = "Hispano Mk.V 20mm", count = 4, ammoTotal = 780 }
                }
            },
            
            battleRating = 8.0f
        };
    }
    
    #endregion
    
    #region Cold War Era Aircraft
    
    public static class ColdWarAircraft
    {
        public static AircraftSpecs F4E_Phantom => new AircraftSpecs
        {
            id = "f4e_phantom",
            name = "F-4E Phantom II",
            manufacturer = "McDonnell Douglas",
            nation = Nation.USA,
            type = AircraftType.Jet_Fighter,
            era = AircraftEra.ColdWar,
            yearIntroduced = 1967,
            description = "Legendary supersonic interceptor and fighter-bomber. 'If it's ugly enough, it'll fly.'",
            
            maxSpeedKmh = 2370f,
            climbRateMs = 210f,
            turnTime360 = 28.0f,
            stallSpeedKmh = 260f,
            maxGForce = 7.5f,
            
            engine = new EngineSpecs
            {
                name = "General Electric J79-GE-17",
                type = EngineType.Turbojet,
                thrustKN = 52.9f,
                thrustKN_AB = 79.4f
            },
            
            emptyWeightKg = 13757f,
            maxTakeoffWeightKg = 28030f,
            
            armament = new ArmamentSpecs
            {
                cannons = new List<GunMount>
                {
                    new GunMount { gunType = "M61A1 Vulcan 20mm", count = 1, ammoTotal = 639, rateOfFire = 6000 }
                },
                missiles = new List<OrdnanceMount>
                {
                    new OrdnanceMount { type = "AIM-7 Sparrow", count = 4, location = "Fuselage" },
                    new OrdnanceMount { type = "AIM-9 Sidewinder", count = 4, location = "Underwing" }
                },
                bombs = new List<OrdnanceMount>
                {
                    new OrdnanceMount { type = "Mk 82 500 lb", count = 12, location = "Underwing" }
                }
            },
            
            battleRating = 10.3f,
            researchRP = 990000,
            purchaseSL = 2100000
        };
        
        public static AircraftSpecs MiG21bis => new AircraftSpecs
        {
            id = "mig21bis",
            name = "MiG-21bis",
            manufacturer = "Mikoyan-Gurevich",
            nation = Nation.USSR,
            type = AircraftType.Jet_Fighter,
            era = AircraftEra.ColdWar,
            yearIntroduced = 1972,
            description = "The ultimate MiG-21 variant. Fast, agile, and deadly in close combat.",
            
            maxSpeedKmh = 2175f,
            climbRateMs = 225f,
            turnTime360 = 22.0f,
            stallSpeedKmh = 230f,
            maxGForce = 8.5f,
            
            engine = new EngineSpecs
            {
                name = "Tumansky R-25-300",
                type = EngineType.Turbojet,
                thrustKN = 40.2f,
                thrustKN_AB = 69.6f
            },
            
            emptyWeightKg = 5350f,
            maxTakeoffWeightKg = 10400f,
            
            armament = new ArmamentSpecs
            {
                cannons = new List<GunMount>
                {
                    new GunMount { gunType = "GSh-23L 23mm", count = 1, ammoTotal = 200, rateOfFire = 3400 }
                },
                missiles = new List<OrdnanceMount>
                {
                    new OrdnanceMount { type = "R-60", count = 4, location = "Underwing" },
                    new OrdnanceMount { type = "R-3S", count = 2, location = "Underwing" }
                }
            },
            
            battleRating = 10.7f
        };
        
        public static AircraftSpecs Mirage_IIIC => new AircraftSpecs
        {
            id = "mirage_iiic",
            name = "Mirage IIIC",
            manufacturer = "Dassault Aviation",
            nation = Nation.France,
            type = AircraftType.Jet_Fighter,
            era = AircraftEra.ColdWar,
            yearIntroduced = 1961,
            description = "French delta-wing interceptor. Agile and fast, iconic design.",
            
            maxSpeedKmh = 2350f,
            climbRateMs = 130f,
            turnTime360 = 30.0f,
            stallSpeedKmh = 250f,
            maxGForce = 7f,
            
            engine = new EngineSpecs
            {
                name = "SNECMA Atar 09B",
                type = EngineType.Turbojet,
                thrustKN = 41.9f,
                thrustKN_AB = 60.8f
            },
            
            armament = new ArmamentSpecs
            {
                cannons = new List<GunMount>
                {
                    new GunMount { gunType = "DEFA 552A 30mm", count = 2, ammoTotal = 250 }
                },
                missiles = new List<OrdnanceMount>
                {
                    new OrdnanceMount { type = "Matra R530", count = 1 },
                    new OrdnanceMount { type = "AIM-9 Sidewinder", count = 2 }
                }
            },
            
            battleRating = 10.0f
        };
        
        public static AircraftSpecs A4E_Skyhawk => new AircraftSpecs
        {
            id = "a4e_skyhawk",
            name = "A-4E Skyhawk",
            manufacturer = "Douglas Aircraft",
            nation = Nation.USA,
            type = AircraftType.AttackAircraft,
            era = AircraftEra.ColdWar,
            yearIntroduced = 1962,
            description = "Heinemann's Hot Rod. Light, agile attack aircraft. Vietnam workhorse.",
            
            maxSpeedKmh = 1077f,
            climbRateMs = 43f,
            turnTime360 = 18.0f,
            stallSpeedKmh = 180f,
            maxGForce = 8f,
            
            engine = new EngineSpecs
            {
                name = "Pratt & Whitney J52-P-6A",
                type = EngineType.Turbojet,
                thrustKN = 38.2f
            },
            
            emptyWeightKg = 4469f,
            maxTakeoffWeightKg = 12437f,
            
            armament = new ArmamentSpecs
            {
                cannons = new List<GunMount>
                {
                    new GunMount { gunType = "Mk 12 20mm", count = 2, ammoTotal = 400 }
                },
                bombs = new List<OrdnanceMount>
                {
                    new OrdnanceMount { type = "Mk 82 500 lb", count = 6 }
                },
                rockets = new List<OrdnanceMount>
                {
                    new OrdnanceMount { type = "Zuni 5-inch", count = 16 }
                }
            },
            
            battleRating = 8.7f
        };
    }
    
    #endregion
    
    #region Modern Aircraft (4th Generation)
    
    public static class ModernAircraft
    {
        public static AircraftSpecs F16C_Viper => new AircraftSpecs
        {
            id = "f16c_viper",
            name = "F-16C Fighting Falcon",
            manufacturer = "General Dynamics / Lockheed Martin",
            nation = Nation.USA,
            type = AircraftType.Jet_Fighter,
            era = AircraftEra.Modern,
            yearIntroduced = 1984,
            description = "The Viper. Legendary multi-role fighter. Fly-by-wire pioneer.",
            
            maxSpeedKmh = 2410f,
            climbRateMs = 254f,
            turnTime360 = 18.0f,
            stallSpeedKmh = 210f,
            maxGForce = 9f,
            
            engine = new EngineSpecs
            {
                name = "General Electric F110-GE-100",
                type = EngineType.Turbofan,
                thrustKN = 76.3f,
                thrustKN_AB = 127.5f
            },
            
            emptyWeightKg = 8570f,
            maxTakeoffWeightKg = 19200f,
            
            armament = new ArmamentSpecs
            {
                cannons = new List<GunMount>
                {
                    new GunMount { gunType = "M61A1 Vulcan 20mm", count = 1, ammoTotal = 511, rateOfFire = 6000 }
                },
                missiles = new List<OrdnanceMount>
                {
                    new OrdnanceMount { type = "AIM-120 AMRAAM", count = 4 },
                    new OrdnanceMount { type = "AIM-9M Sidewinder", count = 2 }
                },
                bombs = new List<OrdnanceMount>
                {
                    new OrdnanceMount { type = "GBU-12 Paveway II", count = 4 }
                }
            },
            
            battleRating = 12.0f,
            researchRP = 2100000,
            purchaseSL = 6300000
        };
        
        public static AircraftSpecs Su27 => new AircraftSpecs
        {
            id = "su27",
            name = "Su-27 Flanker",
            manufacturer = "Sukhoi",
            nation = Nation.USSR,
            type = AircraftType.Jet_Fighter,
            era = AircraftEra.Modern,
            yearIntroduced = 1985,
            description = "Soviet air superiority fighter. Cobra maneuver capable. Highly maneuverable.",
            
            maxSpeedKmh = 2500f,
            climbRateMs = 300f,
            turnTime360 = 20.0f,
            stallSpeedKmh = 200f,
            maxGForce = 9f,
            
            engine = new EngineSpecs
            {
                name = "Saturn AL-31F",
                type = EngineType.Turbofan,
                thrustKN = 75.2f,
                thrustKN_AB = 122.6f
            },
            
            emptyWeightKg = 16380f,
            maxTakeoffWeightKg = 33000f,
            
            armament = new ArmamentSpecs
            {
                cannons = new List<GunMount>
                {
                    new GunMount { gunType = "GSh-30-1 30mm", count = 1, ammoTotal = 150, rateOfFire = 1800 }
                },
                missiles = new List<OrdnanceMount>
                {
                    new OrdnanceMount { type = "R-27ER", count = 4 },
                    new OrdnanceMount { type = "R-73", count = 4 }
                }
            },
            
            battleRating = 12.3f
        };
        
        public static AircraftSpecs Eurofighter_Typhoon => new AircraftSpecs
        {
            id = "eurofighter_typhoon",
            name = "Eurofighter Typhoon",
            manufacturer = "Eurofighter GmbH",
            nation = Nation.UK, // Multi-national
            type = AircraftType.Jet_Fighter,
            era = AircraftEra.Modern,
            yearIntroduced = 2003,
            description = "European multi-role fighter. Delta-canard configuration. Highly agile.",
            
            maxSpeedKmh = 2495f,
            climbRateMs = 315f,
            turnTime360 = 16.0f,
            stallSpeedKmh = 190f,
            maxGForce = 9f,
            
            engine = new EngineSpecs
            {
                name = "Eurojet EJ200",
                type = EngineType.Turbofan,
                thrustKN = 60f,
                thrustKN_AB = 90f
            },
            
            armament = new ArmamentSpecs
            {
                cannons = new List<GunMount>
                {
                    new GunMount { gunType = "Mauser BK-27 27mm", count = 1, ammoTotal = 150 }
                },
                missiles = new List<OrdnanceMount>
                {
                    new OrdnanceMount { type = "Meteor", count = 4 },
                    new OrdnanceMount { type = "ASRAAM", count = 4 }
                }
            },
            
            battleRating = 12.7f
        };
        
        public static AircraftSpecs Rafale_C => new AircraftSpecs
        {
            id = "rafale_c",
            name = "Dassault Rafale C",
            manufacturer = "Dassault Aviation",
            nation = Nation.France,
            type = AircraftType.Jet_Fighter,
            era = AircraftEra.Modern,
            yearIntroduced = 2004,
            description = "French omnirole fighter. Can do everything, and does it well.",
            
            maxSpeedKmh = 2223f,
            climbRateMs = 305f,
            turnTime360 = 18.0f,
            stallSpeedKmh = 185f,
            maxGForce = 9f,
            
            engine = new EngineSpecs
            {
                name = "Snecma M88-2",
                type = EngineType.Turbofan,
                thrustKN = 50f,
                thrustKN_AB = 75f
            },
            
            armament = new ArmamentSpecs
            {
                cannons = new List<GunMount>
                {
                    new GunMount { gunType = "GIAT 30/719B 30mm", count = 1, ammoTotal = 125 }
                },
                missiles = new List<OrdnanceMount>
                {
                    new OrdnanceMount { type = "MICA", count = 6 },
                    new OrdnanceMount { type = "Meteor", count = 4 }
                }
            },
            
            battleRating = 12.7f
        };
    }
    
    #endregion
    
    #region Bombers
    
    public static class BomberAircraft
    {
        public static AircraftSpecs B17G_FlyingFortress => new AircraftSpecs
        {
            id = "b17g_flyingfortress",
            name = "B-17G Flying Fortress",
            manufacturer = "Boeing",
            nation = Nation.USA,
            type = AircraftType.HeavyBomber,
            era = AircraftEra.MidWWII,
            yearIntroduced = 1943,
            description = "Iconic heavy bomber. Bristling with defensive guns. Strategic bombing workhorse.",
            
            maxSpeedKmh = 462f,
            climbRateMs = 4.5f,
            turnTime360 = 80.0f,
            stallSpeedKmh = 150f,
            maxGForce = 3f,
            
            engine = new EngineSpecs
            {
                name = "Wright R-1820-97",
                type = EngineType.RadialPiston,
                horsePower = 1200f
            },
            
            emptyWeightKg = 16391f,
            maxTakeoffWeightKg = 29710f,
            wingspanM = 31.62f,
            
            armament = new ArmamentSpecs
            {
                machineGuns = new List<GunMount>
                {
                    new GunMount { gunType = "M2 Browning .50 cal", count = 13, ammoTotal = 6380, location = "Turrets" }
                },
                bombs = new List<OrdnanceMount>
                {
                    new OrdnanceMount { type = "1000 lb bomb", count = 8 },
                    new OrdnanceMount { type = "500 lb bomb", count = 12 }
                }
            },
            
            crewCount = 10,
            battleRating = 5.3f
        };
        
        public static AircraftSpecs Lancaster_B_III => new AircraftSpecs
        {
            id = "lancaster_b_iii",
            name = "Avro Lancaster B Mk.III",
            manufacturer = "Avro",
            nation = Nation.UK,
            type = AircraftType.HeavyBomber,
            era = AircraftEra.MidWWII,
            yearIntroduced = 1942,
            description = "British heavy bomber. Carried the 'Tallboy' and 'Grand Slam' bombs.",
            
            maxSpeedKmh = 462f,
            climbRateMs = 4.1f,
            turnTime360 = 85.0f,
            stallSpeedKmh = 145f,
            
            engine = new EngineSpecs
            {
                name = "Rolls-Royce Merlin 28",
                type = EngineType.VeePiston,
                horsePower = 1280f
            },
            
            emptyWeightKg = 16705f,
            maxTakeoffWeightKg = 32659f,
            
            armament = new ArmamentSpecs
            {
                machineGuns = new List<GunMount>
                {
                    new GunMount { gunType = "Browning .303", count = 8, ammoTotal = 14000, location = "Turrets" }
                },
                bombs = new List<OrdnanceMount>
                {
                    new OrdnanceMount { type = "Grand Slam 22000 lb", count = 1 },
                    new OrdnanceMount { type = "1000 lb bomb", count = 14 }
                }
            },
            
            crewCount = 7,
            battleRating = 5.0f
        };
        
        public static AircraftSpecs He111H6 => new AircraftSpecs
        {
            id = "he111h6",
            name = "Heinkel He 111 H-6",
            manufacturer = "Heinkel",
            nation = Nation.Germany,
            type = AircraftType.MediumBomber,
            era = AircraftEra.MidWWII,
            yearIntroduced = 1940,
            description = "German medium bomber. Battle of Britain veteran.",
            
            maxSpeedKmh = 436f,
            climbRateMs = 5.2f,
            turnTime360 = 60.0f,
            stallSpeedKmh = 140f,
            
            engine = new EngineSpecs
            {
                name = "Junkers Jumo 211F-1",
                type = EngineType.VeePiston,
                horsePower = 1350f
            },
            
            armament = new ArmamentSpecs
            {
                machineGuns = new List<GunMount>
                {
                    new GunMount { gunType = "MG 15 7.92mm", count = 5, ammoTotal = 5325 },
                    new GunMount { gunType = "MG FF 20mm", count = 1, ammoTotal = 180 }
                },
                bombs = new List<OrdnanceMount>
                {
                    new OrdnanceMount { type = "SC 500 bomb", count = 4 }
                }
            },
            
            crewCount = 5,
            battleRating = 3.3f
        };
    }
    
    #endregion
    
    #region Ground Attack Aircraft
    
    public static class GroundAttackAircraft
    {
        public static AircraftSpecs A10A_Thunderbolt => new AircraftSpecs
        {
            id = "a10a_thunderbolt",
            name = "A-10A Thunderbolt II",
            manufacturer = "Fairchild Republic",
            nation = Nation.USA,
            type = AircraftType.AttackAircraft,
            era = AircraftEra.ColdWar,
            yearIntroduced = 1977,
            description = "The Warthog. Built around the massive GAU-8 cannon. Tank killer.",
            
            maxSpeedKmh = 706f,
            climbRateMs = 30f,
            turnTime360 = 22.0f,
            stallSpeedKmh = 220f,
            maxGForce = 7.33f,
            
            engine = new EngineSpecs
            {
                name = "General Electric TF34-GE-100",
                type = EngineType.Turbofan,
                thrustKN = 40.3f
            },
            
            emptyWeightKg = 11321f,
            maxTakeoffWeightKg = 22950f,
            
            armament = new ArmamentSpecs
            {
                cannons = new List<GunMount>
                {
                    new GunMount { gunType = "GAU-8/A Avenger 30mm", count = 1, ammoTotal = 1174, rateOfFire = 3900 }
                },
                missiles = new List<OrdnanceMount>
                {
                    new OrdnanceMount { type = "AGM-65 Maverick", count = 6 }
                },
                rockets = new List<OrdnanceMount>
                {
                    new OrdnanceMount { type = "Hydra 70", count = 76 }
                }
            },
            
            armorMm = 38f, // Titanium bathtub
            battleRating = 10.0f
        };
        
        public static AircraftSpecs Su25 => new AircraftSpecs
        {
            id = "su25",
            name = "Su-25 Frogfoot",
            manufacturer = "Sukhoi",
            nation = Nation.USSR,
            type = AircraftType.AttackAircraft,
            era = AircraftEra.ColdWar,
            yearIntroduced = 1981,
            description = "Soviet ground attack aircraft. Rugged and heavily armored.",
            
            maxSpeedKmh = 950f,
            climbRateMs = 58f,
            turnTime360 = 24.0f,
            stallSpeedKmh = 210f,
            maxGForce = 6.5f,
            
            engine = new EngineSpecs
            {
                name = "Soyuz/Tumansky R-195",
                type = EngineType.Turbojet,
                thrustKN = 44.2f
            },
            
            armament = new ArmamentSpecs
            {
                cannons = new List<GunMount>
                {
                    new GunMount { gunType = "GSh-30-2 30mm", count = 1, ammoTotal = 250, rateOfFire = 3000 }
                },
                rockets = new List<OrdnanceMount>
                {
                    new OrdnanceMount { type = "S-8KOM", count = 160 }
                },
                bombs = new List<OrdnanceMount>
                {
                    new OrdnanceMount { type = "FAB-500", count = 8 }
                }
            },
            
            armorMm = 24f,
            battleRating = 9.7f
        };
        
        public static AircraftSpecs Il2M3 => new AircraftSpecs
        {
            id = "il2m3",
            name = "Ilyushin Il-2M3",
            manufacturer = "Ilyushin",
            nation = Nation.USSR,
            type = AircraftType.AttackAircraft,
            era = AircraftEra.MidWWII,
            yearIntroduced = 1943,
            description = "The 'Flying Tank'. Most produced military aircraft in history.",
            
            maxSpeedKmh = 414f,
            climbRateMs = 7.3f,
            turnTime360 = 32.0f,
            stallSpeedKmh = 135f,
            
            engine = new EngineSpecs
            {
                name = "Mikulin AM-38F",
                type = EngineType.VeePiston,
                horsePower = 1720f
            },
            
            armament = new ArmamentSpecs
            {
                cannons = new List<GunMount>
                {
                    new GunMount { gunType = "VYa-23 23mm", count = 2, ammoTotal = 300 }
                },
                machineGuns = new List<GunMount>
                {
                    new GunMount { gunType = "ShKAS 7.62mm", count = 2, ammoTotal = 1500 }
                },
                rockets = new List<OrdnanceMount>
                {
                    new OrdnanceMount { type = "RS-132", count = 8 }
                },
                bombs = new List<OrdnanceMount>
                {
                    new OrdnanceMount { type = "FAB-100", count = 6 }
                }
            },
            
            armorMm = 12f,
            battleRating = 4.0f
        };
    }
    
    #endregion
    
    #region Master Aircraft List
    
    public static class AircraftMasterList
    {
        public static List<AircraftSpecs> GetAllAircraft()
        {
            var list = new List<AircraftSpecs>();
            
            // WWII Aircraft from main database
            list.Add(RealAircraftDatabase.P51D_Mustang);
            list.Add(RealAircraftDatabase.P47D_Thunderbolt);
            list.Add(RealAircraftDatabase.Bf109G6);
            list.Add(RealAircraftDatabase.Fw190A8);
            list.Add(RealAircraftDatabase.SpitfireMkIX);
            list.Add(RealAircraftDatabase.A6M5_Zero);
            list.Add(RealAircraftDatabase.Yak9U);
            list.Add(RealAircraftDatabase.La7);
            
            // Korean War
            list.Add(KoreanWarAircraft.F86F_Sabre);
            list.Add(KoreanWarAircraft.MiG15bis);
            list.Add(KoreanWarAircraft.F9F_Panther);
            list.Add(KoreanWarAircraft.Meteor_F8);
            
            // Cold War
            list.Add(ColdWarAircraft.F4E_Phantom);
            list.Add(ColdWarAircraft.MiG21bis);
            list.Add(ColdWarAircraft.Mirage_IIIC);
            list.Add(ColdWarAircraft.A4E_Skyhawk);
            
            // Modern
            list.Add(ModernAircraft.F16C_Viper);
            list.Add(ModernAircraft.Su27);
            list.Add(ModernAircraft.Eurofighter_Typhoon);
            list.Add(ModernAircraft.Rafale_C);
            
            // Bombers
            list.Add(BomberAircraft.B17G_FlyingFortress);
            list.Add(BomberAircraft.Lancaster_B_III);
            list.Add(BomberAircraft.He111H6);
            
            // Ground Attack
            list.Add(GroundAttackAircraft.A10A_Thunderbolt);
            list.Add(GroundAttackAircraft.Su25);
            list.Add(GroundAttackAircraft.Il2M3);
            
            return list;
        }
        
        public static List<AircraftSpecs> GetAircraftByNation(Nation nation)
        {
            return GetAllAircraft().Where(a => a.nation == nation).ToList();
        }
        
        public static List<AircraftSpecs> GetAircraftByEra(AircraftEra era)
        {
            return GetAllAircraft().Where(a => a.era == era).ToList();
        }
        
        public static List<AircraftSpecs> GetAircraftByType(AircraftType type)
        {
            return GetAllAircraft().Where(a => a.type == type).ToList();
        }
        
        public static List<AircraftSpecs> GetAircraftByBattleRating(float minBR, float maxBR)
        {
            return GetAllAircraft().Where(a => a.battleRating >= minBR && a.battleRating <= maxBR).ToList();
        }
        
        public static AircraftSpecs GetAircraftById(string id)
        {
            return GetAllAircraft().FirstOrDefault(a => a.id == id);
        }
    }
    
    #endregion
}
