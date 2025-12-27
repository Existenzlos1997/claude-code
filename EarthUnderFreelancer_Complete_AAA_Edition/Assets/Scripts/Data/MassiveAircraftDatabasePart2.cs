using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Data
{
    /// <summary>
    /// Massive Aircraft Database Part 2: Korean War through Modern Era + Transport/Cargo/Passenger Aircraft
    /// Completes the 500+ aircraft database
    /// </summary>
    public static class MassiveAircraftDatabasePart2
    {
        // ==================== KOREAN WAR ERA (1950-1953) ====================
        
        public static List<AircraftData> GetKoreanWarFighters()
        {
            return new List<AircraftData>
            {
                // F-86 Sabre Series
                new AircraftData("F-86A Sabre", AircraftType.Fighter, AircraftEra.Korea, 1090f, 4800f, 8000f, "USA"),
                new AircraftData("F-86E Sabre", AircraftType.Fighter, AircraftEra.Korea, 1105f, 4900f, 8100f, "USA"),
                new AircraftData("F-86F Sabre", AircraftType.Fighter, AircraftEra.Korea, 1118f, 5000f, 8200f, "USA"),
                new AircraftData("F-86D Sabre Dog", AircraftType.Fighter, AircraftEra.Korea, 1138f, 5200f, 8500f, "USA"),
                new AircraftData("F-86H Sabre", AircraftType.Fighter, AircraftEra.Korea, 1106f, 5500f, 9000f, "USA"),
                new AircraftData("F-86L Sabre", AircraftType.Fighter, AircraftEra.Korea, 1138f, 5300f, 8600f, "USA"),
                
                // MiG-15 Series
                new AircraftData("MiG-15bis", AircraftType.Fighter, AircraftEra.Korea, 1076f, 3700f, 6000f, "USSR"),
                new AircraftData("MiG-15UTI (Trainer)", AircraftType.Trainer, AircraftEra.Korea, 1000f, 3800f, 6100f, "USSR"),
                
                // MiG-17 Series
                new AircraftData("MiG-17F Fresco", AircraftType.Fighter, AircraftEra.Korea, 1145f, 4000f, 6500f, "USSR"),
                new AircraftData("MiG-17PF", AircraftType.Fighter, AircraftEra.Korea, 1145f, 4100f, 6600f, "USSR"),
                
                // F-80 Shooting Star
                new AircraftData("F-80C Shooting Star", AircraftType.Fighter, AircraftEra.Korea, 965f, 4800f, 7500f, "USA"),
                
                // F-84 Thunderjet/Thunderstreak
                new AircraftData("F-84E Thunderjet", AircraftType.Fighter, AircraftEra.Korea, 1001f, 5200f, 8500f, "USA"),
                new AircraftData("F-84G Thunderjet", AircraftType.Fighter, AircraftEra.Korea, 1021f, 5400f, 8700f, "USA"),
                new AircraftData("F-84F Thunderstreak", AircraftType.Fighter, AircraftEra.Korea, 1118f, 6000f, 9500f, "USA"),
                
                // F-94 Starfire
                new AircraftData("F-94A Starfire", AircraftType.Fighter, AircraftEra.Korea, 966f, 5500f, 8500f, "USA"),
                new AircraftData("F-94C Starfire", AircraftType.Fighter, AircraftEra.Korea, 1030f, 6000f, 9000f, "USA"),
                
                // F-9F Panther/Cougar
                new AircraftData("F9F-2 Panther", AircraftType.Fighter, AircraftEra.Korea, 925f, 4600f, 7500f, "USA"),
                new AircraftData("F9F-5 Panther", AircraftType.Fighter, AircraftEra.Korea, 960f, 4800f, 7700f, "USA"),
                new AircraftData("F9F-6 Cougar", AircraftType.Fighter, AircraftEra.Korea, 1040f, 5000f, 8000f, "USA"),
                new AircraftData("F9F-8 Cougar", AircraftType.Fighter, AircraftEra.Korea, 1095f, 5200f, 8200f, "USA"),
                
                // Hawker Sea Hawk
                new AircraftData("Hawker Sea Hawk F.1", AircraftType.Fighter, AircraftEra.Korea, 950f, 4500f, 7200f, "UK"),
                new AircraftData("Hawker Sea Hawk FGA.6", AircraftType.Fighter, AircraftEra.Korea, 970f, 4700f, 7400f, "UK"),
            };
        }
        
        // ==================== COLD WAR ERA (1954-1990) ====================
        
        public static List<AircraftData> GetColdWarFighters()
        {
            return new List<AircraftData>
            {
                // F-4 Phantom II Series
                new AircraftData("F-4B Phantom II", AircraftType.Multirole, AircraftEra.ColdWar, 2370f, 13000f, 28000f, "USA"),
                new AircraftData("F-4C Phantom II", AircraftType.Multirole, AircraftEra.ColdWar, 2370f, 13100f, 28100f, "USA"),
                new AircraftData("F-4D Phantom II", AircraftType.Multirole, AircraftEra.ColdWar, 2370f, 13200f, 28200f, "USA"),
                new AircraftData("F-4E Phantom II", AircraftType.Multirole, AircraftEra.ColdWar, 2370f, 13400f, 28500f, "USA"),
                new AircraftData("F-4G Wild Weasel", AircraftType.Multirole, AircraftEra.ColdWar, 2370f, 13500f, 28600f, "USA"),
                new AircraftData("F-4J Phantom II", AircraftType.Multirole, AircraftEra.ColdWar, 2370f, 13600f, 28700f, "USA"),
                new AircraftData("F-4S Phantom II", AircraftType.Multirole, AircraftEra.ColdWar, 2370f, 13700f, 28800f, "USA"),
                
                // MiG-21 Series
                new AircraftData("MiG-21F-13 Fishbed", AircraftType.Fighter, AircraftEra.ColdWar, 2175f, 5400f, 9000f, "USSR"),
                new AircraftData("MiG-21PF Fishbed", AircraftType.Fighter, AircraftEra.ColdWar, 2175f, 5500f, 9100f, "USSR"),
                new AircraftData("MiG-21PFM Fishbed", AircraftType.Fighter, AircraftEra.ColdWar, 2175f, 5600f, 9200f, "USSR"),
                new AircraftData("MiG-21MF Fishbed", AircraftType.Fighter, AircraftEra.ColdWar, 2175f, 5700f, 9300f, "USSR"),
                new AircraftData("MiG-21bis Fishbed", AircraftType.Fighter, AircraftEra.ColdWar, 2230f, 5800f, 9500f, "USSR"),
                new AircraftData("MiG-21SMT Fishbed", AircraftType.Fighter, AircraftEra.ColdWar, 2175f, 5900f, 9600f, "USSR"),
                
                // MiG-23 Series
                new AircraftData("MiG-23M Flogger", AircraftType.Fighter, AircraftEra.ColdWar, 2500f, 10000f, 18000f, "USSR"),
                new AircraftData("MiG-23ML Flogger", AircraftType.Fighter, AircraftEra.ColdWar, 2500f, 9500f, 17500f, "USSR"),
                new AircraftData("MiG-23MLD Flogger", AircraftType.Fighter, AircraftEra.ColdWar, 2500f, 9300f, 17000f, "USSR"),
                
                // MiG-25 Foxbat
                new AircraftData("MiG-25P Foxbat-A", AircraftType.Fighter, AircraftEra.ColdWar, 3000f, 20000f, 37000f, "USSR"),
                new AircraftData("MiG-25PD Foxbat-E", AircraftType.Fighter, AircraftEra.ColdWar, 3000f, 20500f, 37500f, "USSR"),
                new AircraftData("MiG-25RB Foxbat-B (Recon)", AircraftType.Reconnaissance, AircraftEra.ColdWar, 3000f, 19500f, 36000f, "USSR"),
                
                // MiG-29 Fulcrum Series
                new AircraftData("MiG-29A Fulcrum", AircraftType.Fighter, AircraftEra.ColdWar, 2400f, 11000f, 20000f, "USSR"),
                new AircraftData("MiG-29S Fulcrum", AircraftType.Fighter, AircraftEra.ColdWar, 2400f, 11100f, 20100f, "USSR"),
                new AircraftData("MiG-29UB Fulcrum (Trainer)", AircraftType.Trainer, AircraftEra.ColdWar, 2300f, 11500f, 20500f, "USSR"),
                
                // Su-27 Flanker Series
                new AircraftData("Su-27 Flanker-B", AircraftType.Fighter, AircraftEra.ColdWar, 2500f, 16500f, 33000f, "USSR"),
                new AircraftData("Su-27P Flanker", AircraftType.Fighter, AircraftEra.ColdWar, 2500f, 16600f, 33100f, "USSR"),
                new AircraftData("Su-27S Flanker", AircraftType.Fighter, AircraftEra.ColdWar, 2500f, 16700f, 33200f, "USSR"),
                new AircraftData("Su-27UB Flanker-C (Trainer)", AircraftType.Trainer, AircraftEra.ColdWar, 2300f, 17500f, 34000f, "USSR"),
                
                // F-104 Starfighter Series
                new AircraftData("F-104A Starfighter", AircraftType.Fighter, AircraftEra.ColdWar, 2335f, 6350f, 13000f, "USA"),
                new AircraftData("F-104C Starfighter", AircraftType.Fighter, AircraftEra.ColdWar, 2335f, 6400f, 13100f, "USA"),
                new AircraftData("F-104G Starfighter", AircraftType.Fighter, AircraftEra.ColdWar, 2335f, 6800f, 14000f, "USA"),
                new AircraftData("F-104S Starfighter", AircraftType.Fighter, AircraftEra.ColdWar, 2335f, 7000f, 14200f, "Italy"),
                
                // F-105 Thunderchief
                new AircraftData("F-105B Thunderchief", AircraftType.Fighter, AircraftEra.ColdWar, 2237f, 12500f, 23000f, "USA"),
                new AircraftData("F-105D Thunderchief", AircraftType.Fighter, AircraftEra.ColdWar, 2237f, 12700f, 23200f, "USA"),
                new AircraftData("F-105F Wild Weasel", AircraftType.Fighter, AircraftEra.ColdWar, 2237f, 13000f, 24000f, "USA"),
                new AircraftData("F-105G Wild Weasel", AircraftType.Fighter, AircraftEra.ColdWar, 2237f, 13200f, 24200f, "USA"),
                
                // F-14 Tomcat Series
                new AircraftData("F-14A Tomcat", AircraftType.Fighter, AircraftEra.ColdWar, 2485f, 18000f, 33000f, "USA"),
                new AircraftData("F-14B Tomcat", AircraftType.Fighter, AircraftEra.ColdWar, 2485f, 18200f, 33200f, "USA"),
                new AircraftData("F-14D Super Tomcat", AircraftType.Fighter, AircraftEra.ColdWar, 2485f, 18500f, 33500f, "USA"),
                
                // F-15 Eagle Series
                new AircraftData("F-15A Eagle", AircraftType.Fighter, AircraftEra.ColdWar, 2665f, 12700f, 30800f, "USA"),
                new AircraftData("F-15B Eagle (Trainer)", AircraftType.Trainer, AircraftEra.ColdWar, 2665f, 13000f, 31000f, "USA"),
                new AircraftData("F-15C Eagle", AircraftType.Fighter, AircraftEra.ColdWar, 2665f, 13000f, 31000f, "USA"),
                new AircraftData("F-15D Eagle (Trainer)", AircraftType.Trainer, AircraftEra.ColdWar, 2665f, 13200f, 31200f, "USA"),
                new AircraftData("F-15E Strike Eagle", AircraftType.Multirole, AircraftEra.ColdWar, 2665f, 17000f, 36700f, "USA"),
                
                // F-16 Fighting Falcon Series
                new AircraftData("F-16A Block 1 Fighting Falcon", AircraftType.Multirole, AircraftEra.ColdWar, 2170f, 8600f, 19200f, "USA"),
                new AircraftData("F-16A Block 5 Fighting Falcon", AircraftType.Multirole, AircraftEra.ColdWar, 2170f, 8650f, 19250f, "USA"),
                new AircraftData("F-16A Block 10 Fighting Falcon", AircraftType.Multirole, AircraftEra.ColdWar, 2170f, 8700f, 19300f, "USA"),
                new AircraftData("F-16A Block 15 Fighting Falcon", AircraftType.Multirole, AircraftEra.ColdWar, 2170f, 8750f, 19350f, "USA"),
                new AircraftData("F-16B (Trainer)", AircraftType.Trainer, AircraftEra.ColdWar, 2100f, 9000f, 19500f, "USA"),
                new AircraftData("F-16C Block 25 Fighting Falcon", AircraftType.Multirole, AircraftEra.ColdWar, 2170f, 9000f, 19500f, "USA"),
                new AircraftData("F-16C Block 30 Fighting Falcon", AircraftType.Multirole, AircraftEra.ColdWar, 2170f, 9100f, 19600f, "USA"),
                new AircraftData("F-16C Block 40 Fighting Falcon", AircraftType.Multirole, AircraftEra.ColdWar, 2170f, 9200f, 19700f, "USA"),
                new AircraftData("F-16C Block 50 Fighting Falcon", AircraftType.Multirole, AircraftEra.ColdWar, 2170f, 9300f, 19800f, "USA"),
                new AircraftData("F-16D (Trainer)", AircraftType.Trainer, AircraftEra.ColdWar, 2100f, 9500f, 20000f, "USA"),
                
                // A-10 Thunderbolt II
                new AircraftData("A-10A Thunderbolt II 'Warthog'", AircraftType.AttackAircraft, AircraftEra.ColdWar, 706f, 11000f, 23000f, "USA"),
                new AircraftData("A-10C Thunderbolt II", AircraftType.AttackAircraft, AircraftEra.ColdWar, 706f, 11200f, 23200f, "USA"),
                
                // English Electric Lightning
                new AircraftData("English Electric Lightning F.1", AircraftType.Fighter, AircraftEra.ColdWar, 2415f, 12700f, 22700f, "UK"),
                new AircraftData("English Electric Lightning F.6", AircraftType.Fighter, AircraftEra.ColdWar, 2415f, 13000f, 23000f, "UK"),
                
                // Mirage Series
                new AircraftData("Dassault Mirage III-C", AircraftType.Fighter, AircraftEra.ColdWar, 2350f, 7050f, 13700f, "France"),
                new AircraftData("Dassault Mirage III-E", AircraftType.Fighter, AircraftEra.ColdWar, 2350f, 7200f, 13800f, "France"),
                new AircraftData("Dassault Mirage 5", AircraftType.Fighter, AircraftEra.ColdWar, 2350f, 6600f, 13700f, "France"),
                new AircraftData("Dassault Mirage F1", AircraftType.Fighter, AircraftEra.ColdWar, 2335f, 7400f, 16200f, "France"),
                new AircraftData("Dassault Mirage 2000", AircraftType.Multirole, AircraftEra.ColdWar, 2495f, 7500f, 17000f, "France"),
                
                // Jaguar
                new AircraftData("SEPECAT Jaguar A", AircraftType.AttackAircraft, AircraftEra.ColdWar, 1700f, 7000f, 15700f, "France/UK"),
                new AircraftData("SEPECAT Jaguar GR.1", AircraftType.AttackAircraft, AircraftEra.ColdWar, 1700f, 7100f, 15800f, "UK"),
                
                // Tornado
                new AircraftData("Panavia Tornado IDS", AircraftType.Multirole, AircraftEra.ColdWar, 2400f, 14000f, 28000f, "Germany/UK/Italy"),
                new AircraftData("Panavia Tornado ADV F.3", AircraftType.Fighter, AircraftEra.ColdWar, 2400f, 14500f, 28500f, "UK"),
            };
        }
        
        // ==================== MODERN ERA (1991-2010) ====================
        
        public static List<AircraftData> GetModernFighters()
        {
            return new List<AircraftData>
            {
                // Eurofighter Typhoon
                new AircraftData("Eurofighter Typhoon T1", AircraftType.Multirole, AircraftEra.Modern, 2495f, 11000f, 23500f, "Europe"),
                new AircraftData("Eurofighter Typhoon T2", AircraftType.Multirole, AircraftEra.Modern, 2495f, 11100f, 23600f, "Europe"),
                new AircraftData("Eurofighter Typhoon T3", AircraftType.Multirole, AircraftEra.Modern, 2495f, 11200f, 23700f, "Europe"),
                
                // Dassault Rafale
                new AircraftData("Dassault Rafale C", AircraftType.Multirole, AircraftEra.Modern, 1912f, 10000f, 24500f, "France"),
                new AircraftData("Dassault Rafale B", AircraftType.Multirole, AircraftEra.Modern, 1912f, 10100f, 24600f, "France"),
                new AircraftData("Dassault Rafale M", AircraftType.Multirole, AircraftEra.Modern, 1912f, 10200f, 24700f, "France"),
                
                // F/A-18 Hornet/Super Hornet
                new AircraftData("F/A-18C Hornet", AircraftType.Multirole, AircraftEra.Modern, 1915f, 10800f, 23000f, "USA"),
                new AircraftData("F/A-18D Hornet", AircraftType.Multirole, AircraftEra.Modern, 1915f, 11000f, 23200f, "USA"),
                new AircraftData("F/A-18E Super Hornet", AircraftType.Multirole, AircraftEra.Modern, 1915f, 13400f, 29900f, "USA"),
                new AircraftData("F/A-18F Super Hornet", AircraftType.Multirole, AircraftEra.Modern, 1915f, 13600f, 30100f, "USA"),
                new AircraftData("EA-18G Growler", AircraftType.Multirole, AircraftEra.Modern, 1915f, 13700f, 30200f, "USA"),
                
                // Su-30/35/37 Series
                new AircraftData("Su-30MKI Flanker-H", AircraftType.Multirole, AircraftEra.Modern, 2100f, 18400f, 38800f, "Russia"),
                new AircraftData("Su-30MKK Flanker-G", AircraftType.Multirole, AircraftEra.Modern, 2100f, 17700f, 38000f, "Russia"),
                new AircraftData("Su-35S Flanker-E+", AircraftType.Fighter, AircraftEra.Modern, 2400f, 18400f, 34500f, "Russia"),
                new AircraftData("Su-37 Flanker-F (Test)", AircraftType.Fighter, AircraftEra.Modern, 2500f, 18500f, 34600f, "Russia"),
                
                // MiG-29M/35
                new AircraftData("MiG-29M Fulcrum-E", AircraftType.Multirole, AircraftEra.Modern, 2400f, 11000f, 22000f, "Russia"),
                new AircraftData("MiG-29K Fulcrum-D", AircraftType.Multirole, AircraftEra.Modern, 2400f, 11200f, 22200f, "Russia"),
                new AircraftData("MiG-35 Fulcrum-F", AircraftType.Multirole, AircraftEra.Modern, 2400f, 11000f, 24500f, "Russia"),
                
                // Gripen
                new AircraftData("Saab JAS 39A Gripen", AircraftType.Multirole, AircraftEra.Modern, 2220f, 6800f, 14000f, "Sweden"),
                new AircraftData("Saab JAS 39C Gripen", AircraftType.Multirole, AircraftEra.Modern, 2220f, 6900f, 14100f, "Sweden"),
                new AircraftData("Saab JAS 39E Gripen", AircraftType.Multirole, AircraftEra.Modern, 2470f, 8000f, 16500f, "Sweden"),
            };
        }
        
        // ==================== CONTEMPORARY/5TH GEN (2011-Present) ====================
        
        public static List<AircraftData> GetContemporaryFighters()
        {
            return new List<AircraftData>
            {
                // F-22 Raptor
                new AircraftData("F-22A Raptor", AircraftType.Fighter, AircraftEra.Contemporary, 2410f, 19700f, 38000f, "USA"),
                
                // F-35 Lightning II
                new AircraftData("F-35A Lightning II", AircraftType.Multirole, AircraftEra.Contemporary, 1930f, 13300f, 31800f, "USA"),
                new AircraftData("F-35B Lightning II (STOVL)", AircraftType.Multirole, AircraftEra.Contemporary, 1930f, 14700f, 27200f, "USA"),
                new AircraftData("F-35C Lightning II (Carrier)", AircraftType.Multirole, AircraftEra.Contemporary, 1930f, 15800f, 31800f, "USA"),
                
                // Su-57 Felon
                new AircraftData("Su-57 Felon", AircraftType.Fighter, AircraftEra.Contemporary, 2600f, 18000f, 35000f, "Russia"),
                
                // J-20 Mighty Dragon
                new AircraftData("Chengdu J-20 Mighty Dragon", AircraftType.Fighter, AircraftEra.Contemporary, 2100f, 19000f, 37000f, "China"),
                
                // J-10
                new AircraftData("Chengdu J-10A", AircraftType.Multirole, AircraftEra.Contemporary, 2327f, 9750f, 19277f, "China"),
                new AircraftData("Chengdu J-10B", AircraftType.Multirole, AircraftEra.Contemporary, 2327f, 9800f, 19300f, "China"),
                new AircraftData("Chengdu J-10C", AircraftType.Multirole, AircraftEra.Contemporary, 2327f, 9850f, 19350f, "China"),
                
                // J-11
                new AircraftData("Shenyang J-11A", AircraftType.Fighter, AircraftEra.Contemporary, 2500f, 16380f, 33000f, "China"),
                new AircraftData("Shenyang J-11B", AircraftType.Fighter, AircraftEra.Contemporary, 2500f, 16400f, 33100f, "China"),
                
                // J-16
                new AircraftData("Shenyang J-16", AircraftType.Multirole, AircraftEra.Contemporary, 2400f, 19500f, 37500f, "China"),
                
                // J-31/FC-31
                new AircraftData("Shenyang J-31 Gyrfalcon", AircraftType.Fighter, AircraftEra.Contemporary, 1800f, 12000f, 28000f, "China"),
                
                // KF-21 Boramae
                new AircraftData("KAI KF-21 Boramae", AircraftType.Multirole, AircraftEra.Contemporary, 1950f, 12000f, 26000f, "South Korea"),
            };
        }
        
        // ==================== TRANSPORT AIRCRAFT ====================
        
        public static List<AircraftData> GetTransportAircraft()
        {
            return new List<AircraftData>
            {
                // C-130 Hercules
                new AircraftData("C-130A Hercules", AircraftType.Transport, AircraftEra.ColdWar, 592f, 29500f, 56000f, "USA"),
                new AircraftData("C-130E Hercules", AircraftType.Transport, AircraftEra.ColdWar, 618f, 30000f, 70000f, "USA"),
                new AircraftData("C-130H Hercules", AircraftType.Transport, AircraftEra.Modern, 594f, 30500f, 70300f, "USA"),
                new AircraftData("C-130J Super Hercules", AircraftType.Transport, AircraftEra.Contemporary, 671f, 34000f, 79400f, "USA"),
                
                // C-17 Globemaster III
                new AircraftData("C-17A Globemaster III", AircraftType.Transport, AircraftEra.Modern, 830f, 128100f, 265400f, "USA"),
                
                // C-5 Galaxy
                new AircraftData("C-5A Galaxy", AircraftType.Transport, AircraftEra.ColdWar, 919f, 172400f, 417300f, "USA"),
                new AircraftData("C-5B Galaxy", AircraftType.Transport, AircraftEra.ColdWar, 919f, 173000f, 418000f, "USA"),
                new AircraftData("C-5M Super Galaxy", AircraftType.Transport, AircraftEra.Contemporary, 919f, 172000f, 420000f, "USA"),
                
                // C-47 Skytrain/Dakota
                new AircraftData("C-47A Skytrain", AircraftType.Transport, AircraftEra.WWII, 360f, 8300f, 14000f, "USA"),
                new AircraftData("C-47B Skytrain", AircraftType.Transport, AircraftEra.WWII, 365f, 8400f, 14100f, "USA"),
                
                // Antonov Series
                new AircraftData("Antonov An-12 Cub", AircraftType.Transport, AircraftEra.ColdWar, 777f, 28000f, 61000f, "USSR"),
                new AircraftData("Antonov An-22 Antei", AircraftType.Transport, AircraftEra.ColdWar, 740f, 114000f, 250000f, "USSR"),
                new AircraftData("Antonov An-124 Ruslan", AircraftType.Transport, AircraftEra.ColdWar, 865f, 175000f, 405000f, "USSR"),
                new AircraftData("Antonov An-225 Mriya", AircraftType.Transport, AircraftEra.ColdWar, 850f, 285000f, 640000f, "USSR"),
                
                // Ilyushin IL-76
                new AircraftData("Ilyushin Il-76MD", AircraftType.Transport, AircraftEra.ColdWar, 850f, 92500f, 190000f, "USSR"),
                new AircraftData("Ilyushin Il-76MD-90A", AircraftType.Transport, AircraftEra.Contemporary, 850f, 93000f, 195000f, "Russia"),
                
                // Airbus A400M Atlas
                new AircraftData("Airbus A400M Atlas", AircraftType.Transport, AircraftEra.Contemporary, 780f, 76500f, 141000f, "Europe"),
                
                // Transall C-160
                new AircraftData("Transall C-160", AircraftType.Transport, AircraftEra.ColdWar, 513f, 28600f, 51000f, "France/Germany"),
            };
        }
        
        // ==================== CARGO AIRCRAFT ====================
        
        public static List<AircraftData> GetCargoAircraft()
        {
            return new List<AircraftData>
            {
                // Boeing Cargo
                new AircraftData("Boeing 747-400F", AircraftType.Cargo, AircraftEra.Modern, 988f, 180000f, 412800f, "USA"),
                new AircraftData("Boeing 747-8F", AircraftType.Cargo, AircraftEra.Contemporary, 988f, 197000f, 448000f, "USA"),
                new AircraftData("Boeing 767-300F", AircraftType.Cargo, AircraftEra.Modern, 913f, 90000f, 187000f, "USA"),
                new AircraftData("Boeing 777F", AircraftType.Cargo, AircraftEra.Contemporary, 896f, 145000f, 347000f, "USA"),
                
                // Airbus Cargo
                new AircraftData("Airbus A300-600ST Beluga", AircraftType.Cargo, AircraftEra.Modern, 750f, 86000f, 155000f, "Europe"),
                new AircraftData("Airbus A330-200F", AircraftType.Cargo, AircraftEra.Contemporary, 871f, 109000f, 230000f, "Europe"),
                new AircraftData("Airbus A330-300P2F", AircraftType.Cargo, AircraftEra.Contemporary, 871f, 110000f, 233000f, "Europe"),
                new AircraftData("Airbus BelugaXL", AircraftType.Cargo, AircraftEra.Contemporary, 870f, 125000f, 227000f, "Europe"),
                
                // McDonnell Douglas Cargo
                new AircraftData("McDonnell Douglas DC-8-73F", AircraftType.Cargo, AircraftEra.ColdWar, 965f, 75000f, 162000f, "USA"),
                new AircraftData("McDonnell Douglas MD-11F", AircraftType.Cargo, AircraftEra.Modern, 945f, 130000f, 273300f, "USA"),
                
                // Lockheed L-100 (Civil Hercules)
                new AircraftData("Lockheed L-100-20", AircraftType.Cargo, AircraftEra.ColdWar, 621f, 35000f, 70300f, "USA"),
                new AircraftData("Lockheed L-100-30", AircraftType.Cargo, AircraftEra.ColdWar, 621f, 36000f, 70600f, "USA"),
            };
        }
        
        // ==================== PASSENGER AIRCRAFT ====================
        
        public static List<AircraftData> GetPassengerAircraft()
        {
            return new List<AircraftData>
            {
                // Boeing Passenger Jets
                new AircraftData("Boeing 707-320", AircraftType.Passenger, AircraftEra.ColdWar, 973f, 66000f, 151000f, "USA"),
                new AircraftData("Boeing 727-200", AircraftType.Passenger, AircraftEra.ColdWar, 963f, 45000f, 95000f, "USA"),
                new AircraftData("Boeing 737-200", AircraftType.Passenger, AircraftEra.ColdWar, 943f, 28000f, 53000f, "USA"),
                new AircraftData("Boeing 737-300", AircraftType.Passenger, AircraftEra.Modern, 913f, 33000f, 63000f, "USA"),
                new AircraftData("Boeing 737-400", AircraftType.Passenger, AircraftEra.Modern, 913f, 34000f, 68000f, "USA"),
                new AircraftData("Boeing 737-500", AircraftType.Passenger, AircraftEra.Modern, 913f, 31000f, 60500f, "USA"),
                new AircraftData("Boeing 737-700", AircraftType.Passenger, AircraftEra.Modern, 876f, 38000f, 70100f, "USA"),
                new AircraftData("Boeing 737-800", AircraftType.Passenger, AircraftEra.Modern, 876f, 41500f, 79000f, "USA"),
                new AircraftData("Boeing 737-900ER", AircraftType.Passenger, AircraftEra.Contemporary, 876f, 44700f, 87900f, "USA"),
                new AircraftData("Boeing 737 MAX 7", AircraftType.Passenger, AircraftEra.Contemporary, 839f, 46000f, 80300f, "USA"),
                new AircraftData("Boeing 737 MAX 8", AircraftType.Passenger, AircraftEra.Contemporary, 839f, 45500f, 82200f, "USA"),
                new AircraftData("Boeing 737 MAX 9", AircraftType.Passenger, AircraftEra.Contemporary, 839f, 48000f, 88300f, "USA"),
                new AircraftData("Boeing 737 MAX 10", AircraftType.Passenger, AircraftEra.Contemporary, 839f, 50000f, 89800f, "USA"),
                new AircraftData("Boeing 747-100", AircraftType.Passenger, AircraftEra.ColdWar, 988f, 165000f, 334000f, "USA"),
                new AircraftData("Boeing 747-200", AircraftType.Passenger, AircraftEra.ColdWar, 988f, 170000f, 377000f, "USA"),
                new AircraftData("Boeing 747-300", AircraftType.Passenger, AircraftEra.ColdWar, 988f, 174000f, 378000f, "USA"),
                new AircraftData("Boeing 747-400", AircraftType.Passenger, AircraftEra.Modern, 988f, 180000f, 412800f, "USA"),
                new AircraftData("Boeing 747-8I", AircraftType.Passenger, AircraftEra.Contemporary, 988f, 197000f, 448000f, "USA"),
                new AircraftData("Boeing 757-200", AircraftType.Passenger, AircraftEra.Modern, 914f, 58400f, 116000f, "USA"),
                new AircraftData("Boeing 757-300", AircraftType.Passenger, AircraftEra.Modern, 914f, 64400f, 123800f, "USA"),
                new AircraftData("Boeing 767-200", AircraftType.Passenger, AircraftEra.ColdWar, 913f, 80000f, 136100f, "USA"),
                new AircraftData("Boeing 767-300", AircraftType.Passenger, AircraftEra.Modern, 913f, 86000f, 158800f, "USA"),
                new AircraftData("Boeing 767-400ER", AircraftType.Passenger, AircraftEra.Modern, 913f, 103900f, 204200f, "USA"),
                new AircraftData("Boeing 777-200", AircraftType.Passenger, AircraftEra.Modern, 896f, 134800f, 247200f, "USA"),
                new AircraftData("Boeing 777-200ER", AircraftType.Passenger, AircraftEra.Modern, 896f, 138100f, 297600f, "USA"),
                new AircraftData("Boeing 777-200LR", AircraftType.Passenger, AircraftEra.Modern, 896f, 145200f, 347500f, "USA"),
                new AircraftData("Boeing 777-300", AircraftType.Passenger, AircraftEra.Modern, 896f, 160500f, 299400f, "USA"),
                new AircraftData("Boeing 777-300ER", AircraftType.Passenger, AircraftEra.Contemporary, 896f, 167800f, 351500f, "USA"),
                new AircraftData("Boeing 787-8 Dreamliner", AircraftType.Passenger, AircraftEra.Contemporary, 954f, 110000f, 228000f, "USA"),
                new AircraftData("Boeing 787-9 Dreamliner", AircraftType.Passenger, AircraftEra.Contemporary, 954f, 119000f, 254000f, "USA"),
                new AircraftData("Boeing 787-10 Dreamliner", AircraftType.Passenger, AircraftEra.Contemporary, 954f, 128850f, 254010f, "USA"),
                
                // Airbus Passenger Jets
                new AircraftData("Airbus A300B2", AircraftType.Passenger, AircraftEra.ColdWar, 903f, 90000f, 142000f, "Europe"),
                new AircraftData("Airbus A300-600", AircraftType.Passenger, AircraftEra.ColdWar, 875f, 90800f, 171700f, "Europe"),
                new AircraftData("Airbus A310-200", AircraftType.Passenger, AircraftEra.ColdWar, 895f, 83600f, 142000f, "Europe"),
                new AircraftData("Airbus A310-300", AircraftType.Passenger, AircraftEra.ColdWar, 895f, 89400f, 164000f, "Europe"),
                new AircraftData("Airbus A318", AircraftType.Passenger, AircraftEra.Contemporary, 871f, 39500f, 68000f, "Europe"),
                new AircraftData("Airbus A319", AircraftType.Passenger, AircraftEra.Modern, 871f, 40000f, 75000f, "Europe"),
                new AircraftData("Airbus A320", AircraftType.Passenger, AircraftEra.Modern, 871f, 42000f, 78000f, "Europe"),
                new AircraftData("Airbus A321", AircraftType.Passenger, AircraftEra.Modern, 871f, 48000f, 93500f, "Europe"),
                new AircraftData("Airbus A320neo", AircraftType.Passenger, AircraftEra.Contemporary, 871f, 42600f, 79000f, "Europe"),
                new AircraftData("Airbus A321neo", AircraftType.Passenger, AircraftEra.Contemporary, 871f, 49000f, 97000f, "Europe"),
                new AircraftData("Airbus A330-200", AircraftType.Passenger, AircraftEra.Modern, 871f, 120000f, 242000f, "Europe"),
                new AircraftData("Airbus A330-300", AircraftType.Passenger, AircraftEra.Modern, 871f, 124000f, 242000f, "Europe"),
                new AircraftData("Airbus A330-800neo", AircraftType.Passenger, AircraftEra.Contemporary, 913f, 132000f, 251000f, "Europe"),
                new AircraftData("Airbus A330-900neo", AircraftType.Passenger, AircraftEra.Contemporary, 913f, 138000f, 251000f, "Europe"),
                new AircraftData("Airbus A340-200", AircraftType.Passenger, AircraftEra.Modern, 881f, 129300f, 275000f, "Europe"),
                new AircraftData("Airbus A340-300", AircraftType.Passenger, AircraftEra.Modern, 881f, 130300f, 276500f, "Europe"),
                new AircraftData("Airbus A340-500", AircraftType.Passenger, AircraftEra.Modern, 913f, 170400f, 380000f, "Europe"),
                new AircraftData("Airbus A340-600", AircraftType.Passenger, AircraftEra.Modern, 913f, 177000f, 380000f, "Europe"),
                new AircraftData("Airbus A350-900", AircraftType.Passenger, AircraftEra.Contemporary, 945f, 142000f, 280000f, "Europe"),
                new AircraftData("Airbus A350-1000", AircraftType.Passenger, AircraftEra.Contemporary, 945f, 156000f, 319000f, "Europe"),
                new AircraftData("Airbus A380-800", AircraftType.Passenger, AircraftEra.Contemporary, 945f, 277000f, 575000f, "Europe"),
                
                // Regional Jets
                new AircraftData("Bombardier CRJ200", AircraftType.Passenger, AircraftEra.Modern, 786f, 12700f, 23100f, "Canada"),
                new AircraftData("Bombardier CRJ700", AircraftType.Passenger, AircraftEra.Contemporary, 828f, 19000f, 34000f, "Canada"),
                new AircraftData("Bombardier CRJ900", AircraftType.Passenger, AircraftEra.Contemporary, 890f, 21000f, 38000f, "Canada"),
                new AircraftData("Bombardier CRJ1000", AircraftType.Passenger, AircraftEra.Contemporary, 890f, 23000f, 41000f, "Canada"),
                new AircraftData("Embraer ERJ 145", AircraftType.Passenger, AircraftEra.Modern, 833f, 12000f, 22000f, "Brazil"),
                new AircraftData("Embraer E170", AircraftType.Passenger, AircraftEra.Contemporary, 890f, 20000f, 38000f, "Brazil"),
                new AircraftData("Embraer E175", AircraftType.Passenger, AircraftEra.Contemporary, 890f, 21000f, 39000f, "Brazil"),
                new AircraftData("Embraer E190", AircraftType.Passenger, AircraftEra.Contemporary, 890f, 28000f, 51800f, "Brazil"),
                new AircraftData("Embraer E195", AircraftType.Passenger, AircraftEra.Contemporary, 890f, 29000f, 52300f, "Brazil"),
                
                // Soviet/Russian Passenger
                new AircraftData("Tupolev Tu-104", AircraftType.Passenger, AircraftEra.ColdWar, 950f, 37000f, 76000f, "USSR"),
                new AircraftData("Tupolev Tu-134", AircraftType.Passenger, AircraftEra.ColdWar, 900f, 29000f, 49000f, "USSR"),
                new AircraftData("Tupolev Tu-154M", AircraftType.Passenger, AircraftEra.ColdWar, 950f, 55300f, 100000f, "USSR"),
                new AircraftData("Ilyushin Il-62", AircraftType.Passenger, AircraftEra.ColdWar, 900f, 70000f, 165000f, "USSR"),
                new AircraftData("Ilyushin Il-86", AircraftType.Passenger, AircraftEra.ColdWar, 950f, 119000f, 208000f, "USSR"),
                new AircraftData("Ilyushin Il-96", AircraftType.Passenger, AircraftEra.Modern, 900f, 117000f, 250000f, "Russia"),
                new AircraftData("Sukhoi Superjet 100", AircraftType.Passenger, AircraftEra.Contemporary, 870f, 22300f, 49450f, "Russia"),
                new AircraftData("Irkut MC-21", AircraftType.Passenger, AircraftEra.Contemporary, 870f, 49200f, 86000f, "Russia"),
                
                // Classic Propeller Passenger
                new AircraftData("Douglas DC-3", AircraftType.Passenger, AircraftEra.WWII, 346f, 8000f, 12000f, "USA"),
                new AircraftData("Lockheed L-1049 Super Constellation", AircraftType.Passenger, AircraftEra.ColdWar, 580f, 34300f, 61300f, "USA"),
                new AircraftData("Douglas DC-6", AircraftType.Passenger, AircraftEra.ColdWar, 507f, 25100f, 48500f, "USA"),
                new AircraftData("Douglas DC-7", AircraftType.Passenger, AircraftEra.ColdWar, 580f, 33000f, 64800f, "USA"),
            };
        }
        
        // Helper method to get all Part 2 aircraft
        public static List<AircraftData> GetAllPart2Aircraft()
        {
            var allAircraft = new List<AircraftData>();
            
            // Korean War
            allAircraft.AddRange(GetKoreanWarFighters());
            
            // Cold War
            allAircraft.AddRange(GetColdWarFighters());
            
            // Modern
            allAircraft.AddRange(GetModernFighters());
            
            // Contemporary
            allAircraft.AddRange(GetContemporaryFighters());
            
            // Transport, Cargo, Passenger
            allAircraft.AddRange(GetTransportAircraft());
            allAircraft.AddRange(GetCargoAircraft());
            allAircraft.AddRange(GetPassengerAircraft());
            
            return allAircraft;
        }
        
        // Get total count of all aircraft
        public static int GetTotalAircraftCount()
        {
            return MassiveAircraftDatabase.GetAllAircraft().Count + GetAllPart2Aircraft().Count;
        }
    }
}
