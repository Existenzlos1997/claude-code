using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Data
{
    /// <summary>
    /// Complete database of real-world aircraft with historically accurate specifications
    /// Based on War Thunder data and historical records
    /// </summary>
    public static class RealAircraftDatabase
    {
        #region USA Aircraft

        public static AircraftSpecs P51D_Mustang => new AircraftSpecs
        {
            id = "p51d_mustang",
            name = "P-51D Mustang",
            manufacturer = "North American Aviation",
            nation = Nation.USA,
            type = AircraftType.Fighter,
            era = AircraftEra.LateWWII,
            yearIntroduced = 1944,
            description = "The definitive American fighter of WWII. Excellent range, speed, and all-around performance.",
            
            // Performance
            maxSpeedKmh = 703f,
            maxSpeedAltitudeM = 7620f,
            climbRateMs = 16.3f,
            turnTime360 = 20.0f,
            rollRate = 95f,
            stallSpeedKmh = 160f,
            maxGForce = 12f,
            wingRipSpeedKmh = 912f,
            
            // Engine
            engine = new EngineSpecs
            {
                name = "Packard V-1650-7 Merlin",
                type = EngineType.VeePiston,
                horsePower = 1490f,
                horsePowerWEP = 1720f,
                wepDurationSeconds = 300f
            },
            
            // Physical
            emptyWeightKg = 3465f,
            maxTakeoffWeightKg = 5490f,
            wingspanM = 11.28f,
            lengthM = 9.83f,
            heightM = 4.17f,
            wingAreaM2 = 21.83f,
            fuelCapacityL = 1018f,
            
            // Armament
            armament = new ArmamentSpecs
            {
                machineGuns = new List<GunMount>
                {
                    new GunMount { gunType = "M2 Browning .50 cal", count = 6, ammoTotal = 1880, location = "Wing-mounted" }
                },
                cannons = new List<GunMount>(),
                bombs = new List<OrdnanceMount>
                {
                    new OrdnanceMount { type = "1000 lb bomb", count = 2, location = "Underwing" },
                    new OrdnanceMount { type = "500 lb bomb", count = 2, location = "Underwing" }
                },
                rockets = new List<OrdnanceMount>
                {
                    new OrdnanceMount { type = "HVAR 5-inch", count = 6, location = "Underwing" }
                }
            },
            
            // Protection
            armorMm = 11f,
            pilotArmorMm = 38f,
            selfSealingTanks = true,
            
            // Battle Rating
            battleRating = 4.7f,
            researchRP = 46000,
            purchaseSL = 174000
        };

        public static AircraftSpecs P47D_Thunderbolt => new AircraftSpecs
        {
            id = "p47d_thunderbolt",
            name = "P-47D Thunderbolt",
            manufacturer = "Republic Aviation",
            nation = Nation.USA,
            type = AircraftType.FighterBomber,
            era = AircraftEra.MidWWII,
            yearIntroduced = 1943,
            description = "The 'Jug' - heavily armed and armored fighter-bomber. Devastating in ground attack.",
            
            maxSpeedKmh = 697f,
            maxSpeedAltitudeM = 9144f,
            climbRateMs = 12.1f,
            turnTime360 = 26.0f,
            rollRate = 80f,
            stallSpeedKmh = 185f,
            maxGForce = 11f,
            wingRipSpeedKmh = 885f,
            
            engine = new EngineSpecs
            {
                name = "Pratt & Whitney R-2800-59",
                type = EngineType.RadialPiston,
                horsePower = 2000f,
                horsePowerWEP = 2535f,
                wepDurationSeconds = 300f
            },
            
            emptyWeightKg = 4853f,
            maxTakeoffWeightKg = 7938f,
            wingspanM = 12.42f,
            lengthM = 11.02f,
            heightM = 4.47f,
            wingAreaM2 = 27.87f,
            fuelCapacityL = 1400f,
            
            armament = new ArmamentSpecs
            {
                machineGuns = new List<GunMount>
                {
                    new GunMount { gunType = "M2 Browning .50 cal", count = 8, ammoTotal = 3400, location = "Wing-mounted" }
                },
                cannons = new List<GunMount>(),
                bombs = new List<OrdnanceMount>
                {
                    new OrdnanceMount { type = "1000 lb bomb", count = 2, location = "Underwing" },
                    new OrdnanceMount { type = "500 lb bomb", count = 1, location = "Centerline" }
                },
                rockets = new List<OrdnanceMount>
                {
                    new OrdnanceMount { type = "HVAR 5-inch", count = 10, location = "Underwing" }
                }
            },
            
            armorMm = 12.7f,
            pilotArmorMm = 38f,
            selfSealingTanks = true,
            
            battleRating = 4.3f,
            researchRP = 36000,
            purchaseSL = 150000
        };

        public static AircraftSpecs F4U_Corsair => new AircraftSpecs
        {
            id = "f4u4_corsair",
            name = "F4U-4 Corsair",
            manufacturer = "Vought",
            nation = Nation.USA,
            type = AircraftType.NavalFighter,
            era = AircraftEra.LateWWII,
            yearIntroduced = 1944,
            description = "Iconic US Navy fighter with distinctive gull wings. Excellent speed and firepower.",
            
            maxSpeedKmh = 717f,
            maxSpeedAltitudeM = 8230f,
            climbRateMs = 19.1f,
            turnTime360 = 21.5f,
            rollRate = 100f,
            stallSpeedKmh = 158f,
            maxGForce = 12f,
            wingRipSpeedKmh = 885f,
            
            engine = new EngineSpecs
            {
                name = "Pratt & Whitney R-2800-18W",
                type = EngineType.RadialPiston,
                horsePower = 2100f,
                horsePowerWEP = 2450f,
                wepDurationSeconds = 300f
            },
            
            emptyWeightKg = 4175f,
            maxTakeoffWeightKg = 6654f,
            wingspanM = 12.49f,
            lengthM = 10.26f,
            heightM = 4.50f,
            wingAreaM2 = 29.17f,
            fuelCapacityL = 681f,
            
            armament = new ArmamentSpecs
            {
                machineGuns = new List<GunMount>
                {
                    new GunMount { gunType = "M2 Browning .50 cal", count = 6, ammoTotal = 2350, location = "Wing-mounted" }
                },
                cannons = new List<GunMount>(),
                bombs = new List<OrdnanceMount>
                {
                    new OrdnanceMount { type = "1000 lb bomb", count = 2, location = "Underwing" }
                },
                rockets = new List<OrdnanceMount>
                {
                    new OrdnanceMount { type = "HVAR 5-inch", count = 8, location = "Underwing" },
                    new OrdnanceMount { type = "Tiny Tim", count = 2, location = "Underwing" }
                }
            },
            
            armorMm = 12.7f,
            pilotArmorMm = 38f,
            selfSealingTanks = true,
            
            battleRating = 5.0f,
            researchRP = 54000,
            purchaseSL = 190000
        };

        public static AircraftSpecs F86F_Sabre => new AircraftSpecs
        {
            id = "f86f_sabre",
            name = "F-86F-25 Sabre",
            manufacturer = "North American Aviation",
            nation = Nation.USA,
            type = AircraftType.JetFighter,
            era = AircraftEra.ColdWar,
            yearIntroduced = 1952,
            description = "America's premier Korean War jet fighter. Excellent transonic performance.",
            
            maxSpeedKmh = 1105f,
            maxSpeedAltitudeM = 0f,
            climbRateMs = 42.0f,
            turnTime360 = 24.5f,
            rollRate = 225f,
            stallSpeedKmh = 222f,
            maxGForce = 11f,
            wingRipSpeedKmh = 1170f,
            
            engine = new EngineSpecs
            {
                name = "General Electric J47-GE-27",
                type = EngineType.TurboJet,
                horsePower = 0f, // Jet - thrust measured differently
                thrustKN = 26.3f,
                wepDurationSeconds = 0f
            },
            
            emptyWeightKg = 5046f,
            maxTakeoffWeightKg = 9350f,
            wingspanM = 11.30f,
            lengthM = 11.43f,
            heightM = 4.47f,
            wingAreaM2 = 26.76f,
            fuelCapacityL = 1648f,
            
            armament = new ArmamentSpecs
            {
                machineGuns = new List<GunMount>
                {
                    new GunMount { gunType = "M3 Browning .50 cal", count = 6, ammoTotal = 1800, location = "Nose-mounted" }
                },
                cannons = new List<GunMount>(),
                bombs = new List<OrdnanceMount>
                {
                    new OrdnanceMount { type = "1000 lb bomb", count = 2, location = "Underwing" }
                },
                rockets = new List<OrdnanceMount>
                {
                    new OrdnanceMount { type = "HVAR 5-inch", count = 16, location = "Underwing" }
                },
                missiles = new List<OrdnanceMount>
                {
                    new OrdnanceMount { type = "AIM-9B Sidewinder", count = 2, location = "Wingtip" }
                }
            },
            
            armorMm = 6.35f,
            pilotArmorMm = 12.7f,
            selfSealingTanks = false,
            
            battleRating = 8.7f,
            researchRP = 180000,
            purchaseSL = 490000
        };

        #endregion

        #region Germany Aircraft

        public static AircraftSpecs Bf109G6 => new AircraftSpecs
        {
            id = "bf109g6",
            name = "Bf 109 G-6",
            manufacturer = "Messerschmitt",
            nation = Nation.Germany,
            type = AircraftType.Fighter,
            era = AircraftEra.MidWWII,
            yearIntroduced = 1943,
            description = "The backbone of the Luftwaffe. Excellent climb rate and energy fighting capability.",
            
            maxSpeedKmh = 640f,
            maxSpeedAltitudeM = 6600f,
            climbRateMs = 19.8f,
            turnTime360 = 20.5f,
            rollRate = 95f,
            stallSpeedKmh = 155f,
            maxGForce = 13f,
            wingRipSpeedKmh = 790f,
            
            engine = new EngineSpecs
            {
                name = "Daimler-Benz DB 605A",
                type = EngineType.VeePiston,
                horsePower = 1475f,
                horsePowerWEP = 1800f,
                wepDurationSeconds = 180f
            },
            
            emptyWeightKg = 2673f,
            maxTakeoffWeightKg = 3400f,
            wingspanM = 9.92f,
            lengthM = 8.95f,
            heightM = 2.60f,
            wingAreaM2 = 16.05f,
            fuelCapacityL = 400f,
            
            armament = new ArmamentSpecs
            {
                machineGuns = new List<GunMount>
                {
                    new GunMount { gunType = "MG 131 13mm", count = 2, ammoTotal = 600, location = "Cowling" }
                },
                cannons = new List<GunMount>
                {
                    new GunMount { gunType = "MG 151/20", count = 1, ammoTotal = 200, location = "Engine-mounted" },
                    new GunMount { gunType = "MG 151/20", count = 2, ammoTotal = 270, location = "Underwing gunpods (optional)" }
                },
                bombs = new List<OrdnanceMount>
                {
                    new OrdnanceMount { type = "SC250 250kg bomb", count = 1, location = "Centerline" }
                },
                rockets = new List<OrdnanceMount>
                {
                    new OrdnanceMount { type = "Wfr.Gr. 21 210mm", count = 2, location = "Underwing" }
                }
            },
            
            armorMm = 10f,
            pilotArmorMm = 60f,
            selfSealingTanks = true,
            
            battleRating = 4.7f,
            researchRP = 46000,
            purchaseSL = 174000
        };

        public static AircraftSpecs Fw190A8 => new AircraftSpecs
        {
            id = "fw190a8",
            name = "Fw 190 A-8",
            manufacturer = "Focke-Wulf",
            nation = Nation.Germany,
            type = AircraftType.Fighter,
            era = AircraftEra.LateWWII,
            yearIntroduced = 1944,
            description = "The 'Butcher Bird'. Devastating firepower and excellent roll rate.",
            
            maxSpeedKmh = 656f,
            maxSpeedAltitudeM = 6000f,
            climbRateMs = 15.0f,
            turnTime360 = 22.0f,
            rollRate = 160f,
            stallSpeedKmh = 175f,
            maxGForce = 12f,
            wingRipSpeedKmh = 912f,
            
            engine = new EngineSpecs
            {
                name = "BMW 801 D-2",
                type = EngineType.RadialPiston,
                horsePower = 1700f,
                horsePowerWEP = 2100f,
                wepDurationSeconds = 180f
            },
            
            emptyWeightKg = 3470f,
            maxTakeoffWeightKg = 4900f,
            wingspanM = 10.51f,
            lengthM = 9.00f,
            heightM = 3.95f,
            wingAreaM2 = 18.30f,
            fuelCapacityL = 524f,
            
            armament = new ArmamentSpecs
            {
                machineGuns = new List<GunMount>
                {
                    new GunMount { gunType = "MG 131 13mm", count = 2, ammoTotal = 900, location = "Cowling" }
                },
                cannons = new List<GunMount>
                {
                    new GunMount { gunType = "MG 151/20", count = 2, ammoTotal = 500, location = "Wing root" },
                    new GunMount { gunType = "MG 151/20", count = 2, ammoTotal = 250, location = "Outer wing" }
                },
                bombs = new List<OrdnanceMount>
                {
                    new OrdnanceMount { type = "SC500 500kg bomb", count = 1, location = "Centerline" },
                    new OrdnanceMount { type = "SC250 250kg bomb", count = 2, location = "Underwing" }
                },
                rockets = new List<OrdnanceMount>
                {
                    new OrdnanceMount { type = "Wfr.Gr. 21 210mm", count = 2, location = "Underwing" }
                }
            },
            
            armorMm = 12f,
            pilotArmorMm = 57f,
            selfSealingTanks = true,
            
            battleRating = 5.3f,
            researchRP = 54000,
            purchaseSL = 190000
        };

        public static AircraftSpecs Me262A1 => new AircraftSpecs
        {
            id = "me262a1",
            name = "Me 262 A-1a",
            manufacturer = "Messerschmitt",
            nation = Nation.Germany,
            type = AircraftType.JetFighter,
            era = AircraftEra.LateWWII,
            yearIntroduced = 1944,
            description = "World's first operational jet fighter. Devastating 30mm armament.",
            
            maxSpeedKmh = 870f,
            maxSpeedAltitudeM = 6000f,
            climbRateMs = 20.0f,
            turnTime360 = 28.5f,
            rollRate = 140f,
            stallSpeedKmh = 200f,
            maxGForce = 10f,
            wingRipSpeedKmh = 1050f,
            
            engine = new EngineSpecs
            {
                name = "Junkers Jumo 004 B-1",
                type = EngineType.TurboJet,
                horsePower = 0f,
                thrustKN = 8.8f,
                engineCount = 2,
                wepDurationSeconds = 0f
            },
            
            emptyWeightKg = 3795f,
            maxTakeoffWeightKg = 6775f,
            wingspanM = 12.48f,
            lengthM = 10.60f,
            heightM = 3.84f,
            wingAreaM2 = 21.70f,
            fuelCapacityL = 2570f,
            
            armament = new ArmamentSpecs
            {
                machineGuns = new List<GunMount>(),
                cannons = new List<GunMount>
                {
                    new GunMount { gunType = "MK 108 30mm", count = 4, ammoTotal = 360, location = "Nose-mounted" }
                },
                bombs = new List<OrdnanceMount>
                {
                    new OrdnanceMount { type = "SC500 500kg bomb", count = 2, location = "Underwing" }
                },
                rockets = new List<OrdnanceMount>
                {
                    new OrdnanceMount { type = "R4M 55mm", count = 24, location = "Underwing racks" }
                }
            },
            
            armorMm = 15f,
            pilotArmorMm = 90f,
            selfSealingTanks = false,
            
            battleRating = 7.0f,
            researchRP = 99000,
            purchaseSL = 390000
        };

        #endregion

        #region USSR Aircraft

        public static AircraftSpecs Yak3 => new AircraftSpecs
        {
            id = "yak3",
            name = "Yak-3",
            manufacturer = "Yakovlev",
            nation = Nation.USSR,
            type = AircraftType.Fighter,
            era = AircraftEra.LateWWII,
            yearIntroduced = 1944,
            description = "Light and agile Soviet fighter. Exceptional low-altitude dogfighter.",
            
            maxSpeedKmh = 646f,
            maxSpeedAltitudeM = 4100f,
            climbRateMs = 18.5f,
            turnTime360 = 17.5f,
            rollRate = 110f,
            stallSpeedKmh = 140f,
            maxGForce = 11f,
            wingRipSpeedKmh = 720f,
            
            engine = new EngineSpecs
            {
                name = "Klimov VK-105PF-2",
                type = EngineType.VeePiston,
                horsePower = 1290f,
                horsePowerWEP = 1360f,
                wepDurationSeconds = 300f
            },
            
            emptyWeightKg = 2105f,
            maxTakeoffWeightKg = 2660f,
            wingspanM = 9.20f,
            lengthM = 8.50f,
            heightM = 2.42f,
            wingAreaM2 = 14.85f,
            fuelCapacityL = 350f,
            
            armament = new ArmamentSpecs
            {
                machineGuns = new List<GunMount>
                {
                    new GunMount { gunType = "Berezin UBS 12.7mm", count = 2, ammoTotal = 300, location = "Cowling" }
                },
                cannons = new List<GunMount>
                {
                    new GunMount { gunType = "ShVAK 20mm", count = 1, ammoTotal = 120, location = "Engine-mounted" }
                },
                bombs = new List<OrdnanceMount>(),
                rockets = new List<OrdnanceMount>()
            },
            
            armorMm = 8f,
            pilotArmorMm = 68f,
            selfSealingTanks = true,
            
            battleRating = 4.3f,
            researchRP = 36000,
            purchaseSL = 150000
        };

        public static AircraftSpecs La7 => new AircraftSpecs
        {
            id = "la7",
            name = "La-7",
            manufacturer = "Lavochkin",
            nation = Nation.USSR,
            type = AircraftType.Fighter,
            era = AircraftEra.LateWWII,
            yearIntroduced = 1944,
            description = "Ace-maker of the Soviet VVS. Excellent climb and acceleration.",
            
            maxSpeedKmh = 680f,
            maxSpeedAltitudeM = 6000f,
            climbRateMs = 19.2f,
            turnTime360 = 19.0f,
            rollRate = 105f,
            stallSpeedKmh = 155f,
            maxGForce = 11f,
            wingRipSpeedKmh = 810f,
            
            engine = new EngineSpecs
            {
                name = "Shvetsov ASh-82FN",
                type = EngineType.RadialPiston,
                horsePower = 1850f,
                horsePowerWEP = 1950f,
                wepDurationSeconds = 300f
            },
            
            emptyWeightKg = 2638f,
            maxTakeoffWeightKg = 3315f,
            wingspanM = 9.80f,
            lengthM = 8.67f,
            heightM = 2.54f,
            wingAreaM2 = 17.59f,
            fuelCapacityL = 466f,
            
            armament = new ArmamentSpecs
            {
                machineGuns = new List<GunMount>(),
                cannons = new List<GunMount>
                {
                    new GunMount { gunType = "ShVAK 20mm", count = 2, ammoTotal = 340, location = "Cowling" }
                },
                bombs = new List<OrdnanceMount>
                {
                    new OrdnanceMount { type = "FAB-100 100kg bomb", count = 2, location = "Underwing" }
                },
                rockets = new List<OrdnanceMount>()
            },
            
            armorMm = 8f,
            pilotArmorMm = 75f,
            selfSealingTanks = true,
            
            battleRating = 4.7f,
            researchRP = 46000,
            purchaseSL = 174000
        };

        public static AircraftSpecs Il2M => new AircraftSpecs
        {
            id = "il2m",
            name = "Il-2M",
            manufacturer = "Ilyushin",
            nation = Nation.USSR,
            type = AircraftType.Attacker,
            era = AircraftEra.MidWWII,
            yearIntroduced = 1942,
            description = "The 'Flying Tank'. Legendary Soviet ground attacker with heavy armor.",
            
            maxSpeedKmh = 414f,
            maxSpeedAltitudeM = 1200f,
            climbRateMs = 6.8f,
            turnTime360 = 28.0f,
            rollRate = 55f,
            stallSpeedKmh = 145f,
            maxGForce = 8f,
            wingRipSpeedKmh = 620f,
            
            engine = new EngineSpecs
            {
                name = "Mikulin AM-38F",
                type = EngineType.VeePiston,
                horsePower = 1720f,
                horsePowerWEP = 1760f,
                wepDurationSeconds = 300f
            },
            
            emptyWeightKg = 4360f,
            maxTakeoffWeightKg = 6360f,
            wingspanM = 14.60f,
            lengthM = 11.60f,
            heightM = 4.17f,
            wingAreaM2 = 38.50f,
            fuelCapacityL = 535f,
            
            armament = new ArmamentSpecs
            {
                machineGuns = new List<GunMount>
                {
                    new GunMount { gunType = "Berezin UBT 12.7mm (rear)", count = 1, ammoTotal = 150, location = "Rear turret" }
                },
                cannons = new List<GunMount>
                {
                    new GunMount { gunType = "VYa-23 23mm", count = 2, ammoTotal = 300, location = "Wing-mounted" }
                },
                bombs = new List<OrdnanceMount>
                {
                    new OrdnanceMount { type = "FAB-250 250kg bomb", count = 2, location = "Internal bays" },
                    new OrdnanceMount { type = "FAB-100 100kg bomb", count = 4, location = "Underwing" }
                },
                rockets = new List<OrdnanceMount>
                {
                    new OrdnanceMount { type = "RS-132 132mm", count = 8, location = "Underwing" }
                }
            },
            
            armorMm = 12f,
            pilotArmorMm = 64f,
            selfSealingTanks = true,
            hasRearGunner = true,
            
            battleRating = 3.7f,
            researchRP = 22000,
            purchaseSL = 75000
        };

        #endregion

        #region Britain Aircraft

        public static AircraftSpecs SpitfireMkIX => new AircraftSpecs
        {
            id = "spitfire_mk9",
            name = "Spitfire F Mk IX",
            manufacturer = "Supermarine",
            nation = Nation.Britain,
            type = AircraftType.Fighter,
            era = AircraftEra.MidWWII,
            yearIntroduced = 1942,
            description = "The quintessential British fighter. Excellent agility and climb.",
            
            maxSpeedKmh = 650f,
            maxSpeedAltitudeM = 6400f,
            climbRateMs = 23.8f,
            turnTime360 = 17.0f,
            rollRate = 90f,
            stallSpeedKmh = 135f,
            maxGForce = 12f,
            wingRipSpeedKmh = 790f,
            
            engine = new EngineSpecs
            {
                name = "Rolls-Royce Merlin 66",
                type = EngineType.VeePiston,
                horsePower = 1580f,
                horsePowerWEP = 1720f,
                wepDurationSeconds = 300f
            },
            
            emptyWeightKg = 2545f,
            maxTakeoffWeightKg = 3400f,
            wingspanM = 11.23f,
            lengthM = 9.12f,
            heightM = 3.02f,
            wingAreaM2 = 22.48f,
            fuelCapacityL = 386f,
            
            armament = new ArmamentSpecs
            {
                machineGuns = new List<GunMount>
                {
                    new GunMount { gunType = "Browning .303", count = 4, ammoTotal = 1400, location = "Wing-mounted" }
                },
                cannons = new List<GunMount>
                {
                    new GunMount { gunType = "Hispano Mk.II 20mm", count = 2, ammoTotal = 240, location = "Wing-mounted" }
                },
                bombs = new List<OrdnanceMount>
                {
                    new OrdnanceMount { type = "250 lb bomb", count = 2, location = "Underwing" }
                },
                rockets = new List<OrdnanceMount>()
            },
            
            armorMm = 4f,
            pilotArmorMm = 38f,
            selfSealingTanks = true,
            
            battleRating = 4.7f,
            researchRP = 46000,
            purchaseSL = 174000
        };

        public static AircraftSpecs TyphoonMkIb => new AircraftSpecs
        {
            id = "typhoon_mk1b",
            name = "Typhoon Mk Ib",
            manufacturer = "Hawker",
            nation = Nation.Britain,
            type = AircraftType.FighterBomber,
            era = AircraftEra.MidWWII,
            yearIntroduced = 1942,
            description = "The 'Tiffy'. Fast, heavily armed ground attacker with RP-3 rockets.",
            
            maxSpeedKmh = 663f,
            maxSpeedAltitudeM = 5100f,
            climbRateMs = 13.0f,
            turnTime360 = 21.0f,
            rollRate = 85f,
            stallSpeedKmh = 170f,
            maxGForce = 10f,
            wingRipSpeedKmh = 853f,
            
            engine = new EngineSpecs
            {
                name = "Napier Sabre IIA",
                type = EngineType.InlinePiston,
                horsePower = 2180f,
                horsePowerWEP = 2420f,
                wepDurationSeconds = 300f
            },
            
            emptyWeightKg = 3992f,
            maxTakeoffWeightKg = 6010f,
            wingspanM = 12.67f,
            lengthM = 9.73f,
            heightM = 4.66f,
            wingAreaM2 = 25.92f,
            fuelCapacityL = 800f,
            
            armament = new ArmamentSpecs
            {
                machineGuns = new List<GunMount>(),
                cannons = new List<GunMount>
                {
                    new GunMount { gunType = "Hispano Mk.II 20mm", count = 4, ammoTotal = 560, location = "Wing-mounted" }
                },
                bombs = new List<OrdnanceMount>
                {
                    new OrdnanceMount { type = "1000 lb bomb", count = 2, location = "Underwing" }
                },
                rockets = new List<OrdnanceMount>
                {
                    new OrdnanceMount { type = "RP-3 60 lb", count = 8, location = "Underwing" }
                }
            },
            
            armorMm = 12.7f,
            pilotArmorMm = 42f,
            selfSealingTanks = true,
            
            battleRating = 4.3f,
            researchRP = 36000,
            purchaseSL = 150000
        };

        #endregion

        #region Japan Aircraft

        public static AircraftSpecs A6M5_Zero => new AircraftSpecs
        {
            id = "a6m5_zero",
            name = "A6M5 Zero",
            manufacturer = "Mitsubishi",
            nation = Nation.Japan,
            type = AircraftType.NavalFighter,
            era = AircraftEra.MidWWII,
            yearIntroduced = 1943,
            description = "The legendary Zero. Exceptional maneuverability but fragile.",
            
            maxSpeedKmh = 565f,
            maxSpeedAltitudeM = 6000f,
            climbRateMs = 15.7f,
            turnTime360 = 15.5f,
            rollRate = 80f,
            stallSpeedKmh = 115f,
            maxGForce = 10f,
            wingRipSpeedKmh = 740f,
            
            engine = new EngineSpecs
            {
                name = "Nakajima Sakae 21",
                type = EngineType.RadialPiston,
                horsePower = 1130f,
                horsePowerWEP = 1210f,
                wepDurationSeconds = 300f
            },
            
            emptyWeightKg = 1876f,
            maxTakeoffWeightKg = 2733f,
            wingspanM = 11.00f,
            lengthM = 9.06f,
            heightM = 3.05f,
            wingAreaM2 = 21.30f,
            fuelCapacityL = 480f,
            
            armament = new ArmamentSpecs
            {
                machineGuns = new List<GunMount>
                {
                    new GunMount { gunType = "Type 97 7.7mm", count = 2, ammoTotal = 1400, location = "Cowling" }
                },
                cannons = new List<GunMount>
                {
                    new GunMount { gunType = "Type 99 Model 2 20mm", count = 2, ammoTotal = 200, location = "Wing-mounted" }
                },
                bombs = new List<OrdnanceMount>
                {
                    new OrdnanceMount { type = "60 kg bomb", count = 2, location = "Underwing" }
                },
                rockets = new List<OrdnanceMount>()
            },
            
            armorMm = 0f,
            pilotArmorMm = 0f,
            selfSealingTanks = false,
            
            battleRating = 4.0f,
            researchRP = 29000,
            purchaseSL = 120000
        };

        public static AircraftSpecs N1K2_Shiden => new AircraftSpecs
        {
            id = "n1k2_shiden",
            name = "N1K2-J Shiden-Kai",
            manufacturer = "Kawanishi",
            nation = Nation.Japan,
            type = AircraftType.Fighter,
            era = AircraftEra.LateWWII,
            yearIntroduced = 1944,
            description = "Japan's best late-war fighter. Excellent all-around performance.",
            
            maxSpeedKmh = 595f,
            maxSpeedAltitudeM = 5600f,
            climbRateMs = 17.0f,
            turnTime360 = 18.5f,
            rollRate = 85f,
            stallSpeedKmh = 145f,
            maxGForce = 11f,
            wingRipSpeedKmh = 800f,
            
            engine = new EngineSpecs
            {
                name = "Nakajima Homare 21",
                type = EngineType.RadialPiston,
                horsePower = 1990f,
                horsePowerWEP = 2100f,
                wepDurationSeconds = 300f
            },
            
            emptyWeightKg = 2657f,
            maxTakeoffWeightKg = 4321f,
            wingspanM = 12.00f,
            lengthM = 9.35f,
            heightM = 3.96f,
            wingAreaM2 = 23.50f,
            fuelCapacityL = 650f,
            
            armament = new ArmamentSpecs
            {
                machineGuns = new List<GunMount>(),
                cannons = new List<GunMount>
                {
                    new GunMount { gunType = "Type 99 Model 2 20mm", count = 4, ammoTotal = 900, location = "Wing-mounted" }
                },
                bombs = new List<OrdnanceMount>
                {
                    new OrdnanceMount { type = "250 kg bomb", count = 2, location = "Underwing" }
                },
                rockets = new List<OrdnanceMount>()
            },
            
            armorMm = 8f,
            pilotArmorMm = 70f,
            selfSealingTanks = true,
            
            battleRating = 5.7f,
            researchRP = 69000,
            purchaseSL = 250000
        };

        #endregion

        #region All Aircraft List

        public static List<AircraftSpecs> GetAllAircraft()
        {
            return new List<AircraftSpecs>
            {
                // USA
                P51D_Mustang,
                P47D_Thunderbolt,
                F4U_Corsair,
                F86F_Sabre,
                
                // Germany
                Bf109G6,
                Fw190A8,
                Me262A1,
                
                // USSR
                Yak3,
                La7,
                Il2M,
                
                // Britain
                SpitfireMkIX,
                TyphoonMkIb,
                
                // Japan
                A6M5_Zero,
                N1K2_Shiden
            };
        }

        public static List<AircraftSpecs> GetAircraftByNation(Nation nation)
        {
            return GetAllAircraft().FindAll(a => a.nation == nation);
        }

        public static List<AircraftSpecs> GetAircraftByType(AircraftType type)
        {
            return GetAllAircraft().FindAll(a => a.type == type);
        }

        public static List<AircraftSpecs> GetAircraftByBRRange(float minBR, float maxBR)
        {
            return GetAllAircraft().FindAll(a => a.battleRating >= minBR && a.battleRating <= maxBR);
        }

        public static AircraftSpecs GetAircraftById(string id)
        {
            return GetAllAircraft().Find(a => a.id == id);
        }

        #endregion
    }

    #region Data Structures

    [System.Serializable]
    public class AircraftSpecs
    {
        // Identity
        public string id;
        public string name;
        public string manufacturer;
        public Nation nation;
        public AircraftType type;
        public AircraftEra era;
        public int yearIntroduced;
        public string description;

        // Performance
        public float maxSpeedKmh;
        public float maxSpeedAltitudeM;
        public float climbRateMs;
        public float turnTime360;
        public float rollRate;
        public float stallSpeedKmh;
        public float maxGForce;
        public float wingRipSpeedKmh;

        // Engine
        public EngineSpecs engine;

        // Physical
        public float emptyWeightKg;
        public float maxTakeoffWeightKg;
        public float wingspanM;
        public float lengthM;
        public float heightM;
        public float wingAreaM2;
        public float fuelCapacityL;

        // Armament
        public ArmamentSpecs armament;

        // Protection
        public float armorMm;
        public float pilotArmorMm;
        public bool selfSealingTanks;
        public bool hasRearGunner;

        // Economy
        public float battleRating;
        public int researchRP;
        public int purchaseSL;

        // Calculated properties
        public float WingLoading => maxTakeoffWeightKg / wingAreaM2;
        public float PowerToWeight => engine.horsePower / maxTakeoffWeightKg;
    }

    [System.Serializable]
    public class EngineSpecs
    {
        public string name;
        public EngineType type;
        public float horsePower;
        public float horsePowerWEP;
        public float thrustKN; // For jets
        public int engineCount = 1;
        public float wepDurationSeconds;
    }

    [System.Serializable]
    public class ArmamentSpecs
    {
        public List<GunMount> machineGuns;
        public List<GunMount> cannons;
        public List<OrdnanceMount> bombs;
        public List<OrdnanceMount> rockets;
        public List<OrdnanceMount> missiles;
        public List<OrdnanceMount> torpedoes;
    }

    [System.Serializable]
    public class GunMount
    {
        public string gunType;
        public int count;
        public int ammoTotal;
        public string location;
    }

    [System.Serializable]
    public class OrdnanceMount
    {
        public string type;
        public int count;
        public string location;
    }

    #endregion
}
