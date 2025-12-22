using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Data
{
    /// <summary>
    /// Extended aircraft database with 100+ real aircraft from all eras
    /// Complete with authentic specifications, weapons, and performance data
    /// </summary>
    public static class CompleteAircraftDatabase
    {
        public static Dictionary<string, AircraftDefinition> AllAircraft = new Dictionary<string, AircraftDefinition>()
        {
            // ==================== WORLD WAR I ====================
            { "fokker_dr1", new AircraftDefinition("Fokker Dr.I", "Germany", AircraftEra.WWI, AircraftType.Fighter, 185f, 6100f, 400f, new string[] { "7.92mm MG08" }) },
            { "sopwith_camel", new AircraftDefinition("Sopwith Camel", "Britain", AircraftEra.WWI, AircraftType.Fighter, 188f, 5800f, 385f, new string[] { "Vickers .303" }) },
            { "spad_xiii", new AircraftDefinition("SPAD XIII", "France", AircraftEra.WWI, AircraftType.Fighter, 218f, 6650f, 400f, new string[] { "Vickers .303" }) },

            // ==================== WORLD WAR II - GERMANY ====================
            { "bf109e", new AircraftDefinition("Bf 109 E-4", "Germany", AircraftEra.WWII, AircraftType.Fighter, 570f, 10500f, 500f, new string[] { "7.92mm MG17", "20mm MG FF" }) },
            { "bf109f", new AircraftDefinition("Bf 109 F-4", "Germany", AircraftEra.WWII, AircraftType.Fighter, 624f, 11300f, 525f, new string[] { "7.92mm MG17", "20mm MG151" }) },
            { "bf109g", new AircraftDefinition("Bf 109 G-6", "Germany", AircraftEra.WWII, AircraftType.Fighter, 640f, 11550f, 540f, new string[] { "13mm MG131", "20mm MG151" }) },
            { "bf109k", new AircraftDefinition("Bf 109 K-4", "Germany", AircraftEra.WWII, AircraftType.Fighter, 710f, 12500f, 560f, new string[] { "13mm MG131", "30mm MK108" }) },
            { "fw190a", new AircraftDefinition("Fw 190 A-8", "Germany", AircraftEra.WWII, AircraftType.Fighter, 656f, 10300f, 800f, new string[] { "13mm MG131", "20mm MG151" }) },
            { "fw190d", new AircraftDefinition("Fw 190 D-9", "Germany", AircraftEra.WWII, AircraftType.Fighter, 686f, 12000f, 700f, new string[] { "13mm MG131", "20mm MG151" }) },
            { "ta152h", new AircraftDefinition("Ta 152 H-1", "Germany", AircraftEra.WWII, AircraftType.Fighter, 759f, 14800f, 620f, new string[] { "20mm MG151", "30mm MK108" }) },
            { "me262", new AircraftDefinition("Me 262 A-1a", "Germany", AircraftEra.WWII, AircraftType.JetFighter, 870f, 11450f, 1000f, new string[] { "30mm MK108" }) },
            { "me163", new AircraftDefinition("Me 163 B Komet", "Germany", AircraftEra.WWII, AircraftType.Interceptor, 960f, 12000f, 200f, new string[] { "30mm MK108" }) },
            { "he162", new AircraftDefinition("He 162 Salamander", "Germany", AircraftEra.WWII, AircraftType.JetFighter, 840f, 12000f, 600f, new string[] { "20mm MG151" }) },
            { "do335", new AircraftDefinition("Do 335 Pfeil", "Germany", AircraftEra.WWII, AircraftType.HeavyFighter, 765f, 11400f, 900f, new string[] { "15mm MG151", "30mm MK103" }) },
            { "bf110g", new AircraftDefinition("Bf 110 G-4", "Germany", AircraftEra.WWII, AircraftType.HeavyFighter, 550f, 8000f, 1200f, new string[] { "20mm MG151", "30mm MK108" }) },
            { "ju87", new AircraftDefinition("Ju 87 D Stuka", "Germany", AircraftEra.WWII, AircraftType.DiveBomber, 410f, 7300f, 1800f, new string[] { "7.92mm MG17", "Bombs" }) },
            { "he111", new AircraftDefinition("He 111 H-6", "Germany", AircraftEra.WWII, AircraftType.Bomber, 435f, 6500f, 5000f, new string[] { "7.92mm MG15", "Bombs" }) },
            { "ju88", new AircraftDefinition("Ju 88 A-4", "Germany", AircraftEra.WWII, AircraftType.Bomber, 470f, 8200f, 2500f, new string[] { "7.92mm MG81", "Bombs" }) },

            // ==================== WORLD WAR II - BRITAIN ====================
            { "spitfire_mk1", new AircraftDefinition("Spitfire Mk.I", "Britain", AircraftEra.WWII, AircraftType.Fighter, 582f, 10970f, 455f, new string[] { ".303 Browning" }) },
            { "spitfire_mk5", new AircraftDefinition("Spitfire Mk.V", "Britain", AircraftEra.WWII, AircraftType.Fighter, 594f, 11125f, 490f, new string[] { ".303 Browning", "20mm Hispano" }) },
            { "spitfire_mk9", new AircraftDefinition("Spitfire Mk.IX", "Britain", AircraftEra.WWII, AircraftType.Fighter, 650f, 13100f, 510f, new string[] { ".303 Browning", "20mm Hispano" }) },
            { "spitfire_mk14", new AircraftDefinition("Spitfire Mk.XIV", "Britain", AircraftEra.WWII, AircraftType.Fighter, 721f, 13600f, 520f, new string[] { ".303 Browning", "20mm Hispano" }) },
            { "hurricane_mk1", new AircraftDefinition("Hurricane Mk.I", "Britain", AircraftEra.WWII, AircraftType.Fighter, 547f, 10900f, 545f, new string[] { ".303 Browning" }) },
            { "hurricane_mk2", new AircraftDefinition("Hurricane Mk.IIB", "Britain", AircraftEra.WWII, AircraftType.Fighter, 550f, 10850f, 590f, new string[] { ".303 Browning", "Rockets" }) },
            { "typhoon", new AircraftDefinition("Hawker Typhoon", "Britain", AircraftEra.WWII, AircraftType.FighterBomber, 663f, 10600f, 900f, new string[] { "20mm Hispano", "Rockets" }) },
            { "tempest_mk5", new AircraftDefinition("Hawker Tempest Mk.V", "Britain", AircraftEra.WWII, AircraftType.Fighter, 700f, 11200f, 680f, new string[] { "20mm Hispano" }) },
            { "mosquito", new AircraftDefinition("de Havilland Mosquito", "Britain", AircraftEra.WWII, AircraftType.HeavyFighter, 668f, 11000f, 1200f, new string[] { "20mm Hispano", ".303 Browning" }) },
            { "beaufighter", new AircraftDefinition("Bristol Beaufighter", "Britain", AircraftEra.WWII, AircraftType.HeavyFighter, 515f, 8077f, 1100f, new string[] { "20mm Hispano", ".303 Browning" }) },
            { "lancaster", new AircraftDefinition("Avro Lancaster", "Britain", AircraftEra.WWII, AircraftType.HeavyBomber, 454f, 7470f, 6350f, new string[] { ".303 Browning", "Bombs" }) },
            { "meteor_f3", new AircraftDefinition("Gloster Meteor F.3", "Britain", AircraftEra.WWII, AircraftType.JetFighter, 793f, 13100f, 850f, new string[] { "20mm Hispano" }) },

            // ==================== WORLD WAR II - USA ====================
            { "p51b", new AircraftDefinition("P-51B Mustang", "USA", AircraftEra.WWII, AircraftType.Fighter, 708f, 12770f, 580f, new string[] { ".50 cal M2" }) },
            { "p51d", new AircraftDefinition("P-51D Mustang", "USA", AircraftEra.WWII, AircraftType.Fighter, 703f, 12800f, 600f, new string[] { ".50 cal M2" }) },
            { "p51h", new AircraftDefinition("P-51H Mustang", "USA", AircraftEra.WWII, AircraftType.Fighter, 784f, 12650f, 560f, new string[] { ".50 cal M2" }) },
            { "p47d", new AircraftDefinition("P-47D Thunderbolt", "USA", AircraftEra.WWII, AircraftType.FighterBomber, 697f, 12800f, 1100f, new string[] { ".50 cal M2", "Rockets" }) },
            { "p47n", new AircraftDefinition("P-47N Thunderbolt", "USA", AircraftEra.WWII, AircraftType.FighterBomber, 752f, 13100f, 1150f, new string[] { ".50 cal M2", "Rockets" }) },
            { "p38l", new AircraftDefinition("P-38L Lightning", "USA", AircraftEra.WWII, AircraftType.HeavyFighter, 666f, 13400f, 900f, new string[] { ".50 cal M2", "20mm AN/M2" }) },
            { "p40e", new AircraftDefinition("P-40E Warhawk", "USA", AircraftEra.WWII, AircraftType.Fighter, 580f, 8800f, 590f, new string[] { ".50 cal M2" }) },
            { "f4f", new AircraftDefinition("F4F Wildcat", "USA", AircraftEra.WWII, AircraftType.NavalFighter, 531f, 10360f, 500f, new string[] { ".50 cal M2" }) },
            { "f6f", new AircraftDefinition("F6F Hellcat", "USA", AircraftEra.WWII, AircraftType.NavalFighter, 621f, 11370f, 620f, new string[] { ".50 cal M2" }) },
            { "f4u1", new AircraftDefinition("F4U-1 Corsair", "USA", AircraftEra.WWII, AircraftType.NavalFighter, 671f, 11250f, 670f, new string[] { ".50 cal M2" }) },
            { "f4u4", new AircraftDefinition("F4U-4 Corsair", "USA", AircraftEra.WWII, AircraftType.NavalFighter, 718f, 12650f, 700f, new string[] { ".50 cal M2", "20mm AN/M3" }) },
            { "f8f", new AircraftDefinition("F8F Bearcat", "USA", AircraftEra.WWII, AircraftType.NavalFighter, 678f, 12400f, 520f, new string[] { ".50 cal M2", "20mm AN/M3" }) },
            { "b17g", new AircraftDefinition("B-17G Flying Fortress", "USA", AircraftEra.WWII, AircraftType.HeavyBomber, 462f, 10670f, 4000f, new string[] { ".50 cal M2", "Bombs" }) },
            { "b24", new AircraftDefinition("B-24 Liberator", "USA", AircraftEra.WWII, AircraftType.HeavyBomber, 467f, 8500f, 3600f, new string[] { ".50 cal M2", "Bombs" }) },
            { "b25", new AircraftDefinition("B-25 Mitchell", "USA", AircraftEra.WWII, AircraftType.MediumBomber, 438f, 7600f, 1360f, new string[] { ".50 cal M2", "Bombs" }) },
            { "b29", new AircraftDefinition("B-29 Superfortress", "USA", AircraftEra.WWII, AircraftType.HeavyBomber, 574f, 10200f, 9000f, new string[] { ".50 cal M2", "Bombs" }) },
            { "p80", new AircraftDefinition("P-80 Shooting Star", "USA", AircraftEra.WWII, AircraftType.JetFighter, 898f, 14260f, 850f, new string[] { ".50 cal M2" }) },

            // ==================== WORLD WAR II - USSR ====================
            { "yak1", new AircraftDefinition("Yak-1", "USSR", AircraftEra.WWII, AircraftType.Fighter, 592f, 10000f, 420f, new string[] { "7.62mm ShKAS", "20mm ShVAK" }) },
            { "yak3", new AircraftDefinition("Yak-3", "USSR", AircraftEra.WWII, AircraftType.Fighter, 655f, 10700f, 380f, new string[] { "12.7mm UBS", "20mm ShVAK" }) },
            { "yak9", new AircraftDefinition("Yak-9U", "USSR", AircraftEra.WWII, AircraftType.Fighter, 672f, 10650f, 430f, new string[] { "12.7mm UBS", "20mm ShVAK" }) },
            { "la5fn", new AircraftDefinition("La-5FN", "USSR", AircraftEra.WWII, AircraftType.Fighter, 648f, 11200f, 450f, new string[] { "20mm ShVAK" }) },
            { "la7", new AircraftDefinition("La-7", "USSR", AircraftEra.WWII, AircraftType.Fighter, 661f, 10450f, 460f, new string[] { "20mm ShVAK" }) },
            { "mig3", new AircraftDefinition("MiG-3", "USSR", AircraftEra.WWII, AircraftType.Interceptor, 640f, 12000f, 420f, new string[] { "7.62mm ShKAS", "12.7mm UBS" }) },
            { "il2", new AircraftDefinition("Il-2 Sturmovik", "USSR", AircraftEra.WWII, AircraftType.AttackAircraft, 414f, 6000f, 900f, new string[] { "7.62mm ShKAS", "23mm VYa", "Rockets" }) },
            { "pe2", new AircraftDefinition("Pe-2", "USSR", AircraftEra.WWII, AircraftType.DiveBomber, 540f, 8700f, 1000f, new string[] { "7.62mm ShKAS", "Bombs" }) },

            // ==================== WORLD WAR II - JAPAN ====================
            { "a6m2", new AircraftDefinition("A6M2 Zero", "Japan", AircraftEra.WWII, AircraftType.NavalFighter, 533f, 10000f, 330f, new string[] { "7.7mm Type 97", "20mm Type 99" }) },
            { "a6m5", new AircraftDefinition("A6M5 Zero", "Japan", AircraftEra.WWII, AircraftType.NavalFighter, 565f, 11740f, 350f, new string[] { "7.7mm Type 97", "20mm Type 99" }) },
            { "ki43", new AircraftDefinition("Ki-43 Hayabusa", "Japan", AircraftEra.WWII, AircraftType.Fighter, 530f, 11200f, 300f, new string[] { "12.7mm Ho-103" }) },
            { "ki61", new AircraftDefinition("Ki-61 Hien", "Japan", AircraftEra.WWII, AircraftType.Fighter, 590f, 11600f, 380f, new string[] { "12.7mm Ho-103", "20mm Ho-5" }) },
            { "ki84", new AircraftDefinition("Ki-84 Hayate", "Japan", AircraftEra.WWII, AircraftType.Fighter, 631f, 10500f, 430f, new string[] { "12.7mm Ho-103", "20mm Ho-5" }) },
            { "n1k2", new AircraftDefinition("N1K2-J Shiden-Kai", "Japan", AircraftEra.WWII, AircraftType.Fighter, 594f, 10760f, 520f, new string[] { "20mm Type 99" }) },
            { "ki100", new AircraftDefinition("Ki-100", "Japan", AircraftEra.WWII, AircraftType.Fighter, 580f, 11000f, 400f, new string[] { "12.7mm Ho-103", "20mm Ho-5" }) },
            { "j2m", new AircraftDefinition("J2M Raiden", "Japan", AircraftEra.WWII, AircraftType.Interceptor, 612f, 11430f, 450f, new string[] { "20mm Type 99" }) },
            { "g4m", new AircraftDefinition("G4M Betty", "Japan", AircraftEra.WWII, AircraftType.Bomber, 428f, 8950f, 1000f, new string[] { "7.7mm Type 92", "20mm Type 99" }) },

            // ==================== KOREAN WAR ====================
            { "f86a", new AircraftDefinition("F-86A Sabre", "USA", AircraftEra.KoreanWar, AircraftType.JetFighter, 1093f, 14630f, 650f, new string[] { ".50 cal M3" }) },
            { "f86f", new AircraftDefinition("F-86F Sabre", "USA", AircraftEra.KoreanWar, AircraftType.JetFighter, 1106f, 14935f, 680f, new string[] { ".50 cal M3" }) },
            { "mig15", new AircraftDefinition("MiG-15", "USSR", AircraftEra.KoreanWar, AircraftType.JetFighter, 1076f, 15500f, 600f, new string[] { "23mm NS-23", "37mm N-37" }) },
            { "mig15bis", new AircraftDefinition("MiG-15bis", "USSR", AircraftEra.KoreanWar, AircraftType.JetFighter, 1083f, 15500f, 630f, new string[] { "23mm NR-23", "37mm N-37" }) },
            { "f84g", new AircraftDefinition("F-84G Thunderjet", "USA", AircraftEra.KoreanWar, AircraftType.FighterBomber, 1001f, 12350f, 900f, new string[] { ".50 cal M3", "Bombs", "Rockets" }) },
            { "f9f", new AircraftDefinition("F9F Panther", "USA", AircraftEra.KoreanWar, AircraftType.NavalFighter, 925f, 13600f, 750f, new string[] { "20mm AN/M3" }) },
            { "meteor_f8", new AircraftDefinition("Gloster Meteor F.8", "Britain", AircraftEra.KoreanWar, AircraftType.JetFighter, 965f, 13100f, 880f, new string[] { "20mm Hispano" }) },
            { "sea_fury", new AircraftDefinition("Hawker Sea Fury", "Britain", AircraftEra.KoreanWar, AircraftType.NavalFighter, 740f, 10670f, 750f, new string[] { "20mm Hispano" }) },

            // ==================== COLD WAR - 1950s-60s ====================
            { "f100d", new AircraftDefinition("F-100D Super Sabre", "USA", AircraftEra.ColdWar, AircraftType.JetFighter, 1390f, 15240f, 1000f, new string[] { "20mm M39", "AIM-9", "Bombs" }) },
            { "f104g", new AircraftDefinition("F-104G Starfighter", "USA/Germany", AircraftEra.ColdWar, AircraftType.Interceptor, 2330f, 17680f, 850f, new string[] { "20mm M61", "AIM-9" }) },
            { "f105d", new AircraftDefinition("F-105D Thunderchief", "USA", AircraftEra.ColdWar, AircraftType.FighterBomber, 2208f, 15850f, 1400f, new string[] { "20mm M61", "AIM-9", "Bombs" }) },
            { "f4c", new AircraftDefinition("F-4C Phantom II", "USA", AircraftEra.ColdWar, AircraftType.MultiroleJet, 2370f, 18600f, 1500f, new string[] { "20mm M61", "AIM-7", "AIM-9" }) },
            { "f4e", new AircraftDefinition("F-4E Phantom II", "USA", AircraftEra.ColdWar, AircraftType.MultiroleJet, 2390f, 18975f, 1600f, new string[] { "20mm M61", "AIM-7", "AIM-9" }) },
            { "mig17", new AircraftDefinition("MiG-17F", "USSR", AircraftEra.ColdWar, AircraftType.JetFighter, 1145f, 16600f, 650f, new string[] { "23mm NR-23", "37mm N-37" }) },
            { "mig19", new AircraftDefinition("MiG-19S", "USSR", AircraftEra.ColdWar, AircraftType.JetFighter, 1452f, 17900f, 750f, new string[] { "30mm NR-30" }) },
            { "mig21f", new AircraftDefinition("MiG-21F-13", "USSR", AircraftEra.ColdWar, AircraftType.Interceptor, 2175f, 19000f, 700f, new string[] { "30mm NR-30", "K-13" }) },
            { "mig21bis", new AircraftDefinition("MiG-21bis", "USSR", AircraftEra.ColdWar, AircraftType.MultiroleJet, 2230f, 17500f, 800f, new string[] { "23mm GSh-23L", "R-60" }) },
            { "hunter", new AircraftDefinition("Hawker Hunter F.6", "Britain", AircraftEra.ColdWar, AircraftType.JetFighter, 1150f, 15400f, 950f, new string[] { "30mm ADEN" }) },
            { "lightning", new AircraftDefinition("English Electric Lightning", "Britain", AircraftEra.ColdWar, AircraftType.Interceptor, 2415f, 18300f, 850f, new string[] { "30mm ADEN", "Firestreak" }) },
            { "mirage3", new AircraftDefinition("Mirage IIIC", "France", AircraftEra.ColdWar, AircraftType.Interceptor, 2350f, 17000f, 800f, new string[] { "30mm DEFA", "R.530" }) },
            { "draken", new AircraftDefinition("J35 Draken", "Sweden", AircraftEra.ColdWar, AircraftType.Interceptor, 2150f, 18000f, 700f, new string[] { "30mm Aden", "Rb24" }) },

            // ==================== VIETNAM ERA ====================
            { "a4", new AircraftDefinition("A-4 Skyhawk", "USA", AircraftEra.Vietnam, AircraftType.AttackAircraft, 1083f, 12880f, 800f, new string[] { "20mm Mk12", "Bombs", "Rockets" }) },
            { "a6", new AircraftDefinition("A-6 Intruder", "USA", AircraftEra.Vietnam, AircraftType.AttackAircraft, 1036f, 12925f, 1800f, new string[] { "Bombs", "Missiles" }) },
            { "a7", new AircraftDefinition("A-7 Corsair II", "USA", AircraftEra.Vietnam, AircraftType.AttackAircraft, 1110f, 14500f, 1200f, new string[] { "20mm M61", "AIM-9", "Bombs" }) },
            { "f8", new AircraftDefinition("F-8 Crusader", "USA", AircraftEra.Vietnam, AircraftType.NavalFighter, 1975f, 17680f, 900f, new string[] { "20mm Mk12", "AIM-9" }) },
            { "f111", new AircraftDefinition("F-111 Aardvark", "USA", AircraftEra.Vietnam, AircraftType.StrikeFighter, 2655f, 17270f, 2200f, new string[] { "20mm M61", "Bombs" }) },
            { "b52d", new AircraftDefinition("B-52D Stratofortress", "USA", AircraftEra.Vietnam, AircraftType.StrategicBomber, 1047f, 15150f, 30000f, new string[] { ".50 cal M3", "Bombs" }) },

            // ==================== MODERN - 1970s-90s ====================
            { "f14a", new AircraftDefinition("F-14A Tomcat", "USA", AircraftEra.Modern, AircraftType.NavalFighter, 2485f, 16150f, 1600f, new string[] { "20mm M61", "AIM-54", "AIM-7", "AIM-9" }) },
            { "f14d", new AircraftDefinition("F-14D Super Tomcat", "USA", AircraftEra.Modern, AircraftType.NavalFighter, 2480f, 16150f, 1700f, new string[] { "20mm M61", "AIM-54", "AIM-120", "AIM-9" }) },
            { "f15a", new AircraftDefinition("F-15A Eagle", "USA", AircraftEra.Modern, AircraftType.AirSuperiority, 2655f, 20000f, 1200f, new string[] { "20mm M61", "AIM-7", "AIM-9" }) },
            { "f15c", new AircraftDefinition("F-15C Eagle", "USA", AircraftEra.Modern, AircraftType.AirSuperiority, 2660f, 20000f, 1300f, new string[] { "20mm M61", "AIM-120", "AIM-9" }) },
            { "f15e", new AircraftDefinition("F-15E Strike Eagle", "USA", AircraftEra.Modern, AircraftType.MultiroleJet, 2655f, 18300f, 2000f, new string[] { "20mm M61", "AIM-120", "AIM-9", "Bombs" }) },
            { "f16a", new AircraftDefinition("F-16A Fighting Falcon", "USA", AircraftEra.Modern, AircraftType.MultiroleJet, 2120f, 15240f, 900f, new string[] { "20mm M61", "AIM-9", "AIM-7" }) },
            { "f16c", new AircraftDefinition("F-16C Fighting Falcon", "USA", AircraftEra.Modern, AircraftType.MultiroleJet, 2124f, 15240f, 1000f, new string[] { "20mm M61", "AIM-120", "AIM-9" }) },
            { "f18a", new AircraftDefinition("F/A-18A Hornet", "USA", AircraftEra.Modern, AircraftType.MultiroleJet, 1915f, 15240f, 1100f, new string[] { "20mm M61", "AIM-7", "AIM-9" }) },
            { "f18c", new AircraftDefinition("F/A-18C Hornet", "USA", AircraftEra.Modern, AircraftType.MultiroleJet, 1915f, 15240f, 1200f, new string[] { "20mm M61", "AIM-120", "AIM-9" }) },
            { "a10a", new AircraftDefinition("A-10A Thunderbolt II", "USA", AircraftEra.Modern, AircraftType.AttackAircraft, 706f, 13700f, 1200f, new string[] { "30mm GAU-8", "AGM-65", "Bombs" }) },
            { "mig23", new AircraftDefinition("MiG-23MLD", "USSR", AircraftEra.Modern, AircraftType.MultiroleJet, 2445f, 18500f, 1000f, new string[] { "23mm GSh-23L", "R-24", "R-60" }) },
            { "mig25", new AircraftDefinition("MiG-25PD", "USSR", AircraftEra.Modern, AircraftType.Interceptor, 3000f, 20700f, 1200f, new string[] { "R-40" }) },
            { "mig29a", new AircraftDefinition("MiG-29A", "USSR", AircraftEra.Modern, AircraftType.AirSuperiority, 2445f, 18000f, 1000f, new string[] { "30mm GSh-30-1", "R-27", "R-73" }) },
            { "mig29s", new AircraftDefinition("MiG-29S", "Russia", AircraftEra.Modern, AircraftType.AirSuperiority, 2450f, 18000f, 1100f, new string[] { "30mm GSh-30-1", "R-27", "R-73", "R-77" }) },
            { "mig31", new AircraftDefinition("MiG-31", "USSR", AircraftEra.Modern, AircraftType.Interceptor, 3000f, 20600f, 1800f, new string[] { "23mm GSh-6-23", "R-33" }) },
            { "su27", new AircraftDefinition("Su-27", "USSR", AircraftEra.Modern, AircraftType.AirSuperiority, 2500f, 18500f, 1400f, new string[] { "30mm GSh-30-1", "R-27", "R-73" }) },
            { "su33", new AircraftDefinition("Su-33", "Russia", AircraftEra.Modern, AircraftType.NavalFighter, 2300f, 17000f, 1500f, new string[] { "30mm GSh-30-1", "R-27", "R-73" }) },
            { "tornado_ids", new AircraftDefinition("Tornado IDS", "Germany/UK/Italy", AircraftEra.Modern, AircraftType.StrikeFighter, 2337f, 15240f, 1500f, new string[] { "27mm Mauser", "Bombs" }) },
            { "tornado_adv", new AircraftDefinition("Tornado ADV", "Britain", AircraftEra.Modern, AircraftType.Interceptor, 2338f, 21335f, 1400f, new string[] { "27mm Mauser", "Skyflash", "AIM-9" }) },
            { "mirage2000", new AircraftDefinition("Mirage 2000C", "France", AircraftEra.Modern, AircraftType.MultiroleJet, 2336f, 18000f, 1000f, new string[] { "30mm DEFA", "Super 530", "Magic" }) },
            { "viggen", new AircraftDefinition("JA 37 Viggen", "Sweden", AircraftEra.Modern, AircraftType.MultiroleJet, 2231f, 18290f, 1100f, new string[] { "30mm Oerlikon", "Rb71", "Rb74" }) },

            // ==================== 5TH GENERATION ====================
            { "f22", new AircraftDefinition("F-22 Raptor", "USA", AircraftEra.FifthGen, AircraftType.Stealth, 2414f, 20000f, 1100f, new string[] { "20mm M61", "AIM-120", "AIM-9" }) },
            { "f35a", new AircraftDefinition("F-35A Lightning II", "USA", AircraftEra.FifthGen, AircraftType.Stealth, 1960f, 15240f, 1200f, new string[] { "25mm GAU-22", "AIM-120", "AIM-9" }) },
            { "f35b", new AircraftDefinition("F-35B Lightning II", "USA", AircraftEra.FifthGen, AircraftType.Stealth, 1930f, 15240f, 1100f, new string[] { "25mm GAU-22", "AIM-120", "AIM-9" }) },
            { "f35c", new AircraftDefinition("F-35C Lightning II", "USA", AircraftEra.FifthGen, AircraftType.Stealth, 1960f, 15240f, 1300f, new string[] { "25mm GAU-22", "AIM-120", "AIM-9" }) },
            { "su57", new AircraftDefinition("Su-57 Felon", "Russia", AircraftEra.FifthGen, AircraftType.Stealth, 2600f, 20000f, 1500f, new string[] { "30mm GSh-30-1", "K-77M", "K-74M2" }) },
            { "j20", new AircraftDefinition("J-20", "China", AircraftEra.FifthGen, AircraftType.Stealth, 2100f, 20000f, 1400f, new string[] { "Cannon", "PL-15", "PL-10" }) },

            // ==================== 4.5 GENERATION ====================
            { "f18ef", new AircraftDefinition("F/A-18E/F Super Hornet", "USA", AircraftEra.Modern, AircraftType.MultiroleJet, 1915f, 15240f, 1400f, new string[] { "20mm M61", "AIM-120", "AIM-9X" }) },
            { "rafale", new AircraftDefinition("Dassault Rafale", "France", AircraftEra.Modern, AircraftType.MultiroleJet, 1912f, 15235f, 1100f, new string[] { "30mm DEFA", "MICA", "Meteor" }) },
            { "eurofighter", new AircraftDefinition("Eurofighter Typhoon", "Europe", AircraftEra.Modern, AircraftType.MultiroleJet, 2495f, 19812f, 1200f, new string[] { "27mm Mauser", "AIM-120", "IRIS-T" }) },
            { "gripen", new AircraftDefinition("JAS 39 Gripen", "Sweden", AircraftEra.Modern, AircraftType.MultiroleJet, 2204f, 15240f, 900f, new string[] { "27mm Mauser", "AIM-120", "IRIS-T" }) },
            { "su35", new AircraftDefinition("Su-35S", "Russia", AircraftEra.Modern, AircraftType.AirSuperiority, 2500f, 18000f, 1600f, new string[] { "30mm GSh-30-1", "R-77", "R-73" }) },
            { "su30", new AircraftDefinition("Su-30MKI", "Russia/India", AircraftEra.Modern, AircraftType.MultiroleJet, 2120f, 17300f, 1700f, new string[] { "30mm GSh-30-1", "R-77", "R-73" }) },
        };

        public static AircraftDefinition GetAircraft(string id)
        {
            return AllAircraft.TryGetValue(id, out var aircraft) ? aircraft : null;
        }

        public static List<AircraftDefinition> GetAircraftByEra(AircraftEra era)
        {
            var result = new List<AircraftDefinition>();
            foreach (var kvp in AllAircraft)
            {
                if (kvp.Value.Era == era)
                    result.Add(kvp.Value);
            }
            return result;
        }

        public static List<AircraftDefinition> GetAircraftByNation(string nation)
        {
            var result = new List<AircraftDefinition>();
            foreach (var kvp in AllAircraft)
            {
                if (kvp.Value.Nation.Contains(nation))
                    result.Add(kvp.Value);
            }
            return result;
        }

        public static List<AircraftDefinition> GetAircraftByType(AircraftType type)
        {
            var result = new List<AircraftDefinition>();
            foreach (var kvp in AllAircraft)
            {
                if (kvp.Value.Type == type)
                    result.Add(kvp.Value);
            }
            return result;
        }
    }

    public class AircraftDefinition
    {
        public string Name { get; private set; }
        public string Nation { get; private set; }
        public AircraftEra Era { get; private set; }
        public AircraftType Type { get; private set; }
        public float MaxSpeed { get; private set; } // km/h
        public float ServiceCeiling { get; private set; } // meters
        public float Range { get; private set; } // km
        public string[] Armament { get; private set; }

        public AircraftDefinition(string name, string nation, AircraftEra era, AircraftType type, float speed, float ceiling, float range, string[] armament)
        {
            Name = name;
            Nation = nation;
            Era = era;
            Type = type;
            MaxSpeed = speed;
            ServiceCeiling = ceiling;
            Range = range;
            Armament = armament;
        }
    }

    public enum AircraftEra
    {
        WWI,
        WWII,
        KoreanWar,
        ColdWar,
        Vietnam,
        Modern,
        FifthGen
    }

    public enum AircraftType
    {
        Fighter,
        HeavyFighter,
        Interceptor,
        FighterBomber,
        AttackAircraft,
        DiveBomber,
        MediumBomber,
        HeavyBomber,
        StrategicBomber,
        NavalFighter,
        JetFighter,
        MultiroleJet,
        AirSuperiority,
        StrikeFighter,
        Stealth,
        Bomber
    }
}
