using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Data
{
    /// <summary>
    /// Massive Aircraft Database with 500+ aircraft including all historical variants
    /// Organized by era and type: Combat, Transport, Cargo, Passenger
    /// </summary>
    public static class MassiveAircraftDatabase
    {
        // ==================== WWI ERA (1914-1918) ====================
        
        public static List<AircraftData> GetWWIFighters()
        {
            return new List<AircraftData>
            {
                // Fokker Series
                new AircraftData("Fokker Dr.I", AircraftType.Fighter, AircraftEra.WWI, 165f, 1000f, 2400f, "Germany"),
                new AircraftData("Fokker D.VII", AircraftType.Fighter, AircraftEra.WWI, 189f, 1200f, 2600f, "Germany"),
                new AircraftData("Fokker D.VIII", AircraftType.Fighter, AircraftEra.WWI, 198f, 1100f, 2500f, "Germany"),
                new AircraftData("Fokker E.I Eindecker", AircraftType.Fighter, AircraftEra.WWI, 135f, 800f, 2200f, "Germany"),
                new AircraftData("Fokker E.III", AircraftType.Fighter, AircraftEra.WWI, 142f, 900f, 2300f, "Germany"),
                
                // Albatros Series
                new AircraftData("Albatros D.I", AircraftType.Fighter, AircraftEra.WWI, 175f, 1050f, 2500f, "Germany"),
                new AircraftData("Albatros D.II", AircraftType.Fighter, AircraftEra.WWI, 177f, 1100f, 2600f, "Germany"),
                new AircraftData("Albatros D.III", AircraftType.Fighter, AircraftEra.WWI, 180f, 1150f, 2700f, "Germany"),
                new AircraftData("Albatros D.V", AircraftType.Fighter, AircraftEra.WWI, 186f, 1200f, 2800f, "Germany"),
                new AircraftData("Albatros D.Va", AircraftType.Fighter, AircraftEra.WWI, 188f, 1220f, 2850f, "Germany"),
                
                // Sopwith Series
                new AircraftData("Sopwith Camel F.1", AircraftType.Fighter, AircraftEra.WWI, 182f, 1050f, 2400f, "UK"),
                new AircraftData("Sopwith Pup", AircraftType.Fighter, AircraftEra.WWI, 170f, 900f, 2200f, "UK"),
                new AircraftData("Sopwith Triplane", AircraftType.Fighter, AircraftEra.WWI, 180f, 1000f, 2300f, "UK"),
                new AircraftData("Sopwith Dolphin", AircraftType.Fighter, AircraftEra.WWI, 200f, 1200f, 2600f, "UK"),
                new AircraftData("Sopwith Snipe", AircraftType.Fighter, AircraftEra.WWI, 195f, 1150f, 2550f, "UK"),
                
                // SPAD Series
                new AircraftData("SPAD S.VII", AircraftType.Fighter, AircraftEra.WWI, 192f, 1100f, 2500f, "France"),
                new AircraftData("SPAD S.XIII", AircraftType.Fighter, AircraftEra.WWI, 218f, 1300f, 2700f, "France"),
                new AircraftData("SPAD S.XII", AircraftType.Fighter, AircraftEra.WWI, 210f, 1250f, 2650f, "France"),
                
                // Nieuport Series
                new AircraftData("Nieuport 11 'Bébé'", AircraftType.Fighter, AircraftEra.WWI, 156f, 800f, 2200f, "France"),
                new AircraftData("Nieuport 17", AircraftType.Fighter, AircraftEra.WWI, 177f, 1000f, 2400f, "France"),
                new AircraftData("Nieuport 24", AircraftType.Fighter, AircraftEra.WWI, 185f, 1050f, 2450f, "France"),
                new AircraftData("Nieuport 27", AircraftType.Fighter, AircraftEra.WWI, 190f, 1100f, 2500f, "France"),
                new AircraftData("Nieuport 28", AircraftType.Fighter, AircraftEra.WWI, 195f, 1150f, 2550f, "France"),
                
                // SE5 Series
                new AircraftData("Royal Aircraft Factory S.E.5", AircraftType.Fighter, AircraftEra.WWI, 222f, 1200f, 2600f, "UK"),
                new AircraftData("Royal Aircraft Factory S.E.5a", AircraftType.Fighter, AircraftEra.WWI, 225f, 1250f, 2650f, "UK"),
                
                // Other Notable WWI Fighters
                new AircraftData("Bristol F.2 Fighter", AircraftType.Fighter, AircraftEra.WWI, 200f, 1300f, 2800f, "UK"),
                new AircraftData("Airco DH.2", AircraftType.Fighter, AircraftEra.WWI, 150f, 900f, 2300f, "UK"),
                new AircraftData("Airco DH.5", AircraftType.Fighter, AircraftEra.WWI, 165f, 950f, 2400f, "UK"),
                new AircraftData("Pfalz D.III", AircraftType.Fighter, AircraftEra.WWI, 170f, 1000f, 2500f, "Germany"),
                new AircraftData("Pfalz D.XII", AircraftType.Fighter, AircraftEra.WWI, 185f, 1100f, 2600f, "Germany"),
                new AircraftData("Halberstadt D.II", AircraftType.Fighter, AircraftEra.WWI, 155f, 900f, 2300f, "Germany"),
                new AircraftData("Halberstadt D.V", AircraftType.Fighter, AircraftEra.WWI, 165f, 950f, 2400f, "Germany"),
                new AircraftData("Morane-Saulnier N", AircraftType.Fighter, AircraftEra.WWI, 165f, 950f, 2400f, "France"),
                new AircraftData("Morane-Saulnier Type L", AircraftType.Fighter, AircraftEra.WWI, 145f, 850f, 2200f, "France"),
            };
        }
        
        public static List<AircraftData> GetWWIBombers()
        {
            return new List<AircraftData>
            {
                new AircraftData("Handley Page O/100", AircraftType.Bomber, AircraftEra.WWI, 145f, 3000f, 4500f, "UK"),
                new AircraftData("Handley Page O/400", AircraftType.Bomber, AircraftEra.WWI, 155f, 3500f, 5000f, "UK"),
                new AircraftData("Handley Page V/1500", AircraftType.Bomber, AircraftEra.WWI, 160f, 4000f, 5500f, "UK"),
                new AircraftData("Gotha G.IV", AircraftType.Bomber, AircraftEra.WWI, 140f, 2500f, 4000f, "Germany"),
                new AircraftData("Gotha G.V", AircraftType.Bomber, AircraftEra.WWI, 145f, 2700f, 4200f, "Germany"),
                new AircraftData("Zeppelin-Staaken R.VI", AircraftType.Bomber, AircraftEra.WWI, 130f, 4000f, 6000f, "Germany"),
                new AircraftData("Caproni Ca.3", AircraftType.Bomber, AircraftEra.WWI, 140f, 2200f, 3800f, "Italy"),
                new AircraftData("Caproni Ca.5", AircraftType.Bomber, AircraftEra.WWI, 150f, 2500f, 4000f, "Italy"),
                new AircraftData("Breguet 14", AircraftType.Bomber, AircraftEra.WWI, 177f, 1500f, 3000f, "France"),
                new AircraftData("Voisin III", AircraftType.Bomber, AircraftEra.WWI, 105f, 1200f, 2800f, "France"),
            };
        }
        
        // ==================== INTERWAR ERA (1919-1938) ====================
        
        public static List<AircraftData> GetInterwarFighters()
        {
            return new List<AircraftData>
            {
                // Boeing
                new AircraftData("Boeing P-12", AircraftType.Fighter, AircraftEra.Interwar, 305f, 1500f, 3000f, "USA"),
                new AircraftData("Boeing P-26 Peashooter", AircraftType.Fighter, AircraftEra.Interwar, 377f, 1800f, 3200f, "USA"),
                
                // Curtiss
                new AircraftData("Curtiss P-1 Hawk", AircraftType.Fighter, AircraftEra.Interwar, 257f, 1400f, 2800f, "USA"),
                new AircraftData("Curtiss P-6 Hawk", AircraftType.Fighter, AircraftEra.Interwar, 311f, 1600f, 3000f, "USA"),
                new AircraftData("Curtiss P-36 Hawk", AircraftType.Fighter, AircraftEra.Interwar, 500f, 2000f, 3500f, "USA"),
                
                // Polikarpov
                new AircraftData("Polikarpov I-15", AircraftType.Fighter, AircraftEra.Interwar, 370f, 1700f, 3200f, "USSR"),
                new AircraftData("Polikarpov I-15bis", AircraftType.Fighter, AircraftEra.Interwar, 379f, 1750f, 3250f, "USSR"),
                new AircraftData("Polikarpov I-153 Chaika", AircraftType.Fighter, AircraftEra.Interwar, 426f, 1900f, 3400f, "USSR"),
                new AircraftData("Polikarpov I-16", AircraftType.Fighter, AircraftEra.Interwar, 489f, 2000f, 3500f, "USSR"),
                
                // Fiat
                new AircraftData("Fiat CR.30", AircraftType.Fighter, AircraftEra.Interwar, 355f, 1600f, 3000f, "Italy"),
                new AircraftData("Fiat CR.32", AircraftType.Fighter, AircraftEra.Interwar, 375f, 1700f, 3100f, "Italy"),
                new AircraftData("Fiat CR.42 Falco", AircraftType.Fighter, AircraftEra.Interwar, 441f, 1900f, 3300f, "Italy"),
                
                // Gloster
                new AircraftData("Gloster Gamecock", AircraftType.Fighter, AircraftEra.Interwar, 249f, 1400f, 2800f, "UK"),
                new AircraftData("Gloster Gauntlet", AircraftType.Fighter, AircraftEra.Interwar, 370f, 1700f, 3100f, "UK"),
                new AircraftData("Gloster Gladiator", AircraftType.Fighter, AircraftEra.Interwar, 414f, 1900f, 3300f, "UK"),
                
                // Hawker
                new AircraftData("Hawker Fury", AircraftType.Fighter, AircraftEra.Interwar, 359f, 1700f, 3100f, "UK"),
                new AircraftData("Hawker Nimrod", AircraftType.Fighter, AircraftEra.Interwar, 329f, 1600f, 3000f, "UK"),
                
                // Avia
                new AircraftData("Avia B-534", AircraftType.Fighter, AircraftEra.Interwar, 405f, 1850f, 3250f, "Czechoslovakia"),
                
                // PZL
                new AircraftData("PZL P.11", AircraftType.Fighter, AircraftEra.Interwar, 390f, 1800f, 3200f, "Poland"),
                new AircraftData("PZL P.24", AircraftType.Fighter, AircraftEra.Interwar, 430f, 1950f, 3400f, "Poland"),
            };
        }
        
        // ==================== WWII ERA (1939-1945) ====================
        
        public static List<AircraftData> GetWWIIFighters()
        {
            return new List<AircraftData>
            {
                // Messerschmitt Bf 109 Series (All Variants)
                new AircraftData("Messerschmitt Bf 109B", AircraftType.Fighter, AircraftEra.WWII, 470f, 2400f, 4000f, "Germany"),
                new AircraftData("Messerschmitt Bf 109C", AircraftType.Fighter, AircraftEra.WWII, 485f, 2450f, 4100f, "Germany"),
                new AircraftData("Messerschmitt Bf 109D", AircraftType.Fighter, AircraftEra.WWII, 495f, 2500f, 4200f, "Germany"),
                new AircraftData("Messerschmitt Bf 109E-1 'Emil'", AircraftType.Fighter, AircraftEra.WWII, 560f, 2600f, 4500f, "Germany"),
                new AircraftData("Messerschmitt Bf 109E-3", AircraftType.Fighter, AircraftEra.WWII, 570f, 2650f, 4600f, "Germany"),
                new AircraftData("Messerschmitt Bf 109E-4", AircraftType.Fighter, AircraftEra.WWII, 575f, 2700f, 4700f, "Germany"),
                new AircraftData("Messerschmitt Bf 109E-7", AircraftType.Fighter, AircraftEra.WWII, 580f, 2750f, 4800f, "Germany"),
                new AircraftData("Messerschmitt Bf 109F-1 'Friedrich'", AircraftType.Fighter, AircraftEra.WWII, 600f, 2800f, 4900f, "Germany"),
                new AircraftData("Messerschmitt Bf 109F-2", AircraftType.Fighter, AircraftEra.WWII, 605f, 2850f, 5000f, "Germany"),
                new AircraftData("Messerschmitt Bf 109F-4", AircraftType.Fighter, AircraftEra.WWII, 615f, 2900f, 5100f, "Germany"),
                new AircraftData("Messerschmitt Bf 109G-2 'Gustav'", AircraftType.Fighter, AircraftEra.WWII, 620f, 3000f, 5200f, "Germany"),
                new AircraftData("Messerschmitt Bf 109G-4", AircraftType.Fighter, AircraftEra.WWII, 625f, 3050f, 5300f, "Germany"),
                new AircraftData("Messerschmitt Bf 109G-6", AircraftType.Fighter, AircraftEra.WWII, 630f, 3100f, 5400f, "Germany"),
                new AircraftData("Messerschmitt Bf 109G-10", AircraftType.Fighter, AircraftEra.WWII, 640f, 3150f, 5500f, "Germany"),
                new AircraftData("Messerschmitt Bf 109G-14", AircraftType.Fighter, AircraftEra.WWII, 645f, 3200f, 5600f, "Germany"),
                new AircraftData("Messerschmitt Bf 109K-4 'Kurfürst'", AircraftType.Fighter, AircraftEra.WWII, 710f, 3300f, 5800f, "Germany"),
                
                // Focke-Wulf Fw 190 Series (All Variants)
                new AircraftData("Focke-Wulf Fw 190A-1", AircraftType.Fighter, AircraftEra.WWII, 610f, 3200f, 5500f, "Germany"),
                new AircraftData("Focke-Wulf Fw 190A-2", AircraftType.Fighter, AircraftEra.WWII, 615f, 3250f, 5600f, "Germany"),
                new AircraftData("Focke-Wulf Fw 190A-3", AircraftType.Fighter, AircraftEra.WWII, 620f, 3300f, 5700f, "Germany"),
                new AircraftData("Focke-Wulf Fw 190A-4", AircraftType.Fighter, AircraftEra.WWII, 625f, 3350f, 5800f, "Germany"),
                new AircraftData("Focke-Wulf Fw 190A-5", AircraftType.Fighter, AircraftEra.WWII, 635f, 3400f, 5900f, "Germany"),
                new AircraftData("Focke-Wulf Fw 190A-6", AircraftType.Fighter, AircraftEra.WWII, 640f, 3450f, 6000f, "Germany"),
                new AircraftData("Focke-Wulf Fw 190A-7", AircraftType.Fighter, AircraftEra.WWII, 645f, 3500f, 6100f, "Germany"),
                new AircraftData("Focke-Wulf Fw 190A-8", AircraftType.Fighter, AircraftEra.WWII, 650f, 3550f, 6200f, "Germany"),
                new AircraftData("Focke-Wulf Fw 190D-9 'Dora'", AircraftType.Fighter, AircraftEra.WWII, 685f, 3600f, 6300f, "Germany"),
                new AircraftData("Focke-Wulf Fw 190D-11", AircraftType.Fighter, AircraftEra.WWII, 695f, 3650f, 6400f, "Germany"),
                new AircraftData("Focke-Wulf Fw 190D-13", AircraftType.Fighter, AircraftEra.WWII, 705f, 3700f, 6500f, "Germany"),
                new AircraftData("Focke-Wulf Fw 190F-8 (Fighter-Bomber)", AircraftType.Multirole, AircraftEra.WWII, 630f, 3800f, 6600f, "Germany"),
                new AircraftData("Focke-Wulf Ta 152H", AircraftType.Fighter, AircraftEra.WWII, 760f, 3500f, 6800f, "Germany"),
                
                // Supermarine Spitfire Series (All Major Variants)
                new AircraftData("Supermarine Spitfire Mk I", AircraftType.Fighter, AircraftEra.WWII, 580f, 2400f, 4500f, "UK"),
                new AircraftData("Supermarine Spitfire Mk II", AircraftType.Fighter, AircraftEra.WWII, 590f, 2450f, 4600f, "UK"),
                new AircraftData("Supermarine Spitfire Mk V", AircraftType.Fighter, AircraftEra.WWII, 605f, 2600f, 4800f, "UK"),
                new AircraftData("Supermarine Spitfire Mk IX", AircraftType.Fighter, AircraftEra.WWII, 650f, 2800f, 5200f, "UK"),
                new AircraftData("Supermarine Spitfire Mk XIV", AircraftType.Fighter, AircraftEra.WWII, 720f, 3000f, 5600f, "UK"),
                new AircraftData("Supermarine Spitfire Mk XVI", AircraftType.Fighter, AircraftEra.WWII, 655f, 2850f, 5300f, "UK"),
                new AircraftData("Supermarine Spitfire Mk XVIII", AircraftType.Fighter, AircraftEra.WWII, 730f, 3050f, 5700f, "UK"),
                new AircraftData("Supermarine Spitfire Mk 21", AircraftType.Fighter, AircraftEra.WWII, 735f, 3100f, 5800f, "UK"),
                new AircraftData("Supermarine Spitfire Mk 22", AircraftType.Fighter, AircraftEra.WWII, 740f, 3150f, 5900f, "UK"),
                new AircraftData("Supermarine Spitfire F Mk 24", AircraftType.Fighter, AircraftEra.WWII, 745f, 3200f, 6000f, "UK"),
                new AircraftData("Supermarine Seafire Mk III", AircraftType.Fighter, AircraftEra.WWII, 605f, 2600f, 4800f, "UK"),
                
                // Hawker Hurricane Series
                new AircraftData("Hawker Hurricane Mk I", AircraftType.Fighter, AircraftEra.WWII, 547f, 2500f, 4400f, "UK"),
                new AircraftData("Hawker Hurricane Mk IIA", AircraftType.Fighter, AircraftEra.WWII, 555f, 2550f, 4500f, "UK"),
                new AircraftData("Hawker Hurricane Mk IIB", AircraftType.Fighter, AircraftEra.WWII, 560f, 2600f, 4600f, "UK"),
                new AircraftData("Hawker Hurricane Mk IIC", AircraftType.Fighter, AircraftEra.WWII, 565f, 2650f, 4700f, "UK"),
                new AircraftData("Hawker Hurricane Mk IV", AircraftType.Fighter, AircraftEra.WWII, 570f, 2700f, 4800f, "UK"),
                new AircraftData("Hawker Typhoon Mk IB", AircraftType.Fighter, AircraftEra.WWII, 663f, 3500f, 6000f, "UK"),
                new AircraftData("Hawker Tempest Mk V", AircraftType.Fighter, AircraftEra.WWII, 700f, 3200f, 5800f, "UK"),
                new AircraftData("Hawker Sea Fury", AircraftType.Fighter, AircraftEra.WWII, 740f, 3300f, 6000f, "UK"),
                
                // North American P-51 Mustang Series
                new AircraftData("P-51A Mustang", AircraftType.Fighter, AircraftEra.WWII, 635f, 2800f, 5000f, "USA"),
                new AircraftData("P-51B Mustang", AircraftType.Fighter, AircraftEra.WWII, 703f, 3000f, 5400f, "USA"),
                new AircraftData("P-51C Mustang", AircraftType.Fighter, AircraftEra.WWII, 708f, 3050f, 5500f, "USA"),
                new AircraftData("P-51D Mustang", AircraftType.Fighter, AircraftEra.WWII, 703f, 3100f, 5600f, "USA"),
                new AircraftData("P-51H Mustang", AircraftType.Fighter, AircraftEra.WWII, 784f, 3200f, 5800f, "USA"),
                new AircraftData("P-51K Mustang", AircraftType.Fighter, AircraftEra.WWII, 705f, 3080f, 5550f, "USA"),
                
                // Republic P-47 Thunderbolt Series
                new AircraftData("P-47B Thunderbolt", AircraftType.Fighter, AircraftEra.WWII, 690f, 4200f, 7000f, "USA"),
                new AircraftData("P-47C Thunderbolt", AircraftType.Fighter, AircraftEra.WWII, 695f, 4250f, 7100f, "USA"),
                new AircraftData("P-47D Thunderbolt", AircraftType.Fighter, AircraftEra.WWII, 697f, 4300f, 7200f, "USA"),
                new AircraftData("P-47M Thunderbolt", AircraftType.Fighter, AircraftEra.WWII, 761f, 4400f, 7400f, "USA"),
                new AircraftData("P-47N Thunderbolt", AircraftType.Fighter, AircraftEra.WWII, 750f, 4500f, 7600f, "USA"),
                
                // Lockheed P-38 Lightning Series
                new AircraftData("P-38E Lightning", AircraftType.Fighter, AircraftEra.WWII, 625f, 4500f, 7500f, "USA"),
                new AircraftData("P-38G Lightning", AircraftType.Fighter, AircraftEra.WWII, 640f, 4600f, 7600f, "USA"),
                new AircraftData("P-38H Lightning", AircraftType.Fighter, AircraftEra.WWII, 645f, 4650f, 7700f, "USA"),
                new AircraftData("P-38J Lightning", AircraftType.Fighter, AircraftEra.WWII, 666f, 4700f, 7800f, "USA"),
                new AircraftData("P-38L Lightning", AircraftType.Fighter, AircraftEra.WWII, 666f, 4750f, 7900f, "USA"),
                
                // Grumman F4F Wildcat / F6F Hellcat / F8F Bearcat
                new AircraftData("F4F-3 Wildcat", AircraftType.Fighter, AircraftEra.WWII, 531f, 2700f, 4800f, "USA"),
                new AircraftData("F4F-4 Wildcat", AircraftType.Fighter, AircraftEra.WWII, 512f, 2750f, 4900f, "USA"),
                new AircraftData("FM-2 Wildcat", AircraftType.Fighter, AircraftEra.WWII, 534f, 2800f, 5000f, "USA"),
                new AircraftData("F6F-3 Hellcat", AircraftType.Fighter, AircraftEra.WWII, 629f, 3500f, 6000f, "USA"),
                new AircraftData("F6F-5 Hellcat", AircraftType.Fighter, AircraftEra.WWII, 644f, 3600f, 6100f, "USA"),
                new AircraftData("F8F-1 Bearcat", AircraftType.Fighter, AircraftEra.WWII, 730f, 3200f, 5500f, "USA"),
                new AircraftData("F8F-2 Bearcat", AircraftType.Fighter, AircraftEra.WWII, 740f, 3250f, 5600f, "USA"),
                
                // Vought F4U Corsair Series
                new AircraftData("F4U-1 Corsair", AircraftType.Fighter, AircraftEra.WWII, 671f, 3500f, 6000f, "USA"),
                new AircraftData("F4U-1A Corsair", AircraftType.Fighter, AircraftEra.WWII, 678f, 3550f, 6100f, "USA"),
                new AircraftData("F4U-1D Corsair", AircraftType.Fighter, AircraftEra.WWII, 683f, 3600f, 6200f, "USA"),
                new AircraftData("F4U-4 Corsair", AircraftType.Fighter, AircraftEra.WWII, 721f, 3700f, 6400f, "USA"),
                new AircraftData("F4U-5 Corsair", AircraftType.Fighter, AircraftEra.WWII, 735f, 3800f, 6600f, "USA"),
                
                // Curtiss P-40 Warhawk Series
                new AircraftData("P-40B Warhawk", AircraftType.Fighter, AircraftEra.WWII, 555f, 2700f, 4800f, "USA"),
                new AircraftData("P-40C Warhawk (Tomahawk)", AircraftType.Fighter, AircraftEra.WWII, 560f, 2750f, 4900f, "USA"),
                new AircraftData("P-40E Warhawk (Kittyhawk)", AircraftType.Fighter, AircraftEra.WWII, 580f, 2850f, 5100f, "USA"),
                new AircraftData("P-40F Warhawk", AircraftType.Fighter, AircraftEra.WWII, 595f, 2900f, 5200f, "USA"),
                new AircraftData("P-40N Warhawk", AircraftType.Fighter, AircraftEra.WWII, 608f, 3000f, 5400f, "USA"),
                
                // Yakovlev Yak Series
                new AircraftData("Yakovlev Yak-1", AircraftType.Fighter, AircraftEra.WWII, 592f, 2600f, 4500f, "USSR"),
                new AircraftData("Yakovlev Yak-3", AircraftType.Fighter, AircraftEra.WWII, 655f, 2700f, 4700f, "USSR"),
                new AircraftData("Yakovlev Yak-7", AircraftType.Fighter, AircraftEra.WWII, 571f, 2750f, 4800f, "USSR"),
                new AircraftData("Yakovlev Yak-9", AircraftType.Fighter, AircraftEra.WWII, 600f, 2800f, 4900f, "USSR"),
                new AircraftData("Yakovlev Yak-9U", AircraftType.Fighter, AircraftEra.WWII, 672f, 2900f, 5100f, "USSR"),
                
                // Lavochkin La Series
                new AircraftData("Lavochkin LaGG-3", AircraftType.Fighter, AircraftEra.WWII, 575f, 2700f, 4700f, "USSR"),
                new AircraftData("Lavochkin La-5", AircraftType.Fighter, AircraftEra.WWII, 648f, 2800f, 4900f, "USSR"),
                new AircraftData("Lavochkin La-5FN", AircraftType.Fighter, AircraftEra.WWII, 668f, 2850f, 5000f, "USSR"),
                new AircraftData("Lavochkin La-7", AircraftType.Fighter, AircraftEra.WWII, 661f, 2900f, 5100f, "USSR"),
                
                // Mitsubishi A6M Zero Series
                new AircraftData("Mitsubishi A6M2 Zero (Model 21)", AircraftType.Fighter, AircraftEra.WWII, 533f, 2200f, 4000f, "Japan"),
                new AircraftData("Mitsubishi A6M3 Zero (Model 22)", AircraftType.Fighter, AircraftEra.WWII, 540f, 2250f, 4100f, "Japan"),
                new AircraftData("Mitsubishi A6M5 Zero (Model 52)", AircraftType.Fighter, AircraftEra.WWII, 565f, 2350f, 4300f, "Japan"),
                new AircraftData("Mitsubishi A6M6c Zero", AircraftType.Fighter, AircraftEra.WWII, 570f, 2400f, 4400f, "Japan"),
                new AircraftData("Mitsubishi A6M7 Zero", AircraftType.Fighter, AircraftEra.WWII, 575f, 2450f, 4500f, "Japan"),
                new AircraftData("Mitsubishi A6M8 Zero", AircraftType.Fighter, AircraftEra.WWII, 580f, 2500f, 4600f, "Japan"),
                
                // Nakajima Ki-43 Hayabusa "Oscar"
                new AircraftData("Nakajima Ki-43-I Hayabusa", AircraftType.Fighter, AircraftEra.WWII, 495f, 2100f, 3900f, "Japan"),
                new AircraftData("Nakajima Ki-43-II Hayabusa", AircraftType.Fighter, AircraftEra.WWII, 530f, 2200f, 4000f, "Japan"),
                new AircraftData("Nakajima Ki-43-III Hayabusa", AircraftType.Fighter, AircraftEra.WWII, 570f, 2300f, 4100f, "Japan"),
                
                // Nakajima Ki-84 Hayate "Frank"
                new AircraftData("Nakajima Ki-84 Hayate", AircraftType.Fighter, AircraftEra.WWII, 687f, 3000f, 5400f, "Japan"),
                
                // Kawasaki Ki-61 Hien "Tony"
                new AircraftData("Kawasaki Ki-61-I Hien", AircraftType.Fighter, AircraftEra.WWII, 590f, 2700f, 4800f, "Japan"),
                new AircraftData("Kawasaki Ki-61-II Hien", AircraftType.Fighter, AircraftEra.WWII, 610f, 2800f, 4900f, "Japan"),
                
                // Kawanishi N1K Shiden "George"
                new AircraftData("Kawanishi N1K1-J Shiden", AircraftType.Fighter, AircraftEra.WWII, 658f, 3100f, 5500f, "Japan"),
                new AircraftData("Kawanishi N1K2-J Shiden-Kai", AircraftType.Fighter, AircraftEra.WWII, 670f, 3200f, 5600f, "Japan"),
                
                // Jets
                new AircraftData("Messerschmitt Me 262A-1a Schwalbe", AircraftType.Fighter, AircraftEra.WWII, 870f, 3800f, 6500f, "Germany"),
                new AircraftData("Messerschmitt Me 262A-2a Sturmvogel", AircraftType.Fighter, AircraftEra.WWII, 850f, 4000f, 6800f, "Germany"),
                new AircraftData("Heinkel He 162 Volksjäger", AircraftType.Fighter, AircraftEra.WWII, 840f, 2600f, 4800f, "Germany"),
                new AircraftData("Gloster Meteor F.1", AircraftType.Fighter, AircraftEra.WWII, 668f, 3600f, 6200f, "UK"),
                new AircraftData("Gloster Meteor F.3", AircraftType.Fighter, AircraftEra.WWII, 710f, 3700f, 6400f, "UK"),
            };
        }
        
        public static List<AircraftData> GetWWIIBombers()
        {
            return new List<AircraftData>
            {
                // American Heavy Bombers
                new AircraftData("Boeing B-17E Flying Fortress", AircraftType.Bomber, AircraftEra.WWII, 510f, 15000f, 22000f, "USA"),
                new AircraftData("Boeing B-17F Flying Fortress", AircraftType.Bomber, AircraftEra.WWII, 520f, 15500f, 22500f, "USA"),
                new AircraftData("Boeing B-17G Flying Fortress", AircraftType.Bomber, AircraftEra.WWII, 515f, 16000f, 23000f, "USA"),
                new AircraftData("Consolidated B-24D Liberator", AircraftType.Bomber, AircraftEra.WWII, 488f, 17000f, 25000f, "USA"),
                new AircraftData("Consolidated B-24J Liberator", AircraftType.Bomber, AircraftEra.WWII, 490f, 17500f, 25500f, "USA"),
                new AircraftData("Boeing B-29 Superfortress", AircraftType.Bomber, AircraftEra.WWII, 576f, 20000f, 35000f, "USA"),
                
                // American Medium Bombers
                new AircraftData("North American B-25B Mitchell", AircraftType.Bomber, AircraftEra.WWII, 515f, 9000f, 15000f, "USA"),
                new AircraftData("North American B-25C Mitchell", AircraftType.Bomber, AircraftEra.WWII, 520f, 9200f, 15200f, "USA"),
                new AircraftData("North American B-25D Mitchell", AircraftType.Bomber, AircraftEra.WWII, 520f, 9300f, 15300f, "USA"),
                new AircraftData("North American B-25H Mitchell", AircraftType.Bomber, AircraftEra.WWII, 525f, 9500f, 15500f, "USA"),
                new AircraftData("North American B-25J Mitchell", AircraftType.Bomber, AircraftEra.WWII, 528f, 9700f, 15700f, "USA"),
                new AircraftData("Martin B-26B Marauder", AircraftType.Bomber, AircraftEra.WWII, 510f, 11000f, 17000f, "USA"),
                new AircraftData("Martin B-26G Marauder", AircraftType.Bomber, AircraftEra.WWII, 515f, 11200f, 17200f, "USA"),
                
                // British Heavy Bombers
                new AircraftData("Avro Lancaster B.I", AircraftType.Bomber, AircraftEra.WWII, 462f, 18000f, 28000f, "UK"),
                new AircraftData("Avro Lancaster B.III", AircraftType.Bomber, AircraftEra.WWII, 465f, 18500f, 28500f, "UK"),
                new AircraftData("Handley Page Halifax B.III", AircraftType.Bomber, AircraftEra.WWII, 454f, 17000f, 25000f, "UK"),
                new AircraftData("Short Stirling Mk I", AircraftType.Bomber, AircraftEra.WWII, 435f, 18000f, 27000f, "UK"),
                
                // British Medium Bombers
                new AircraftData("Vickers Wellington Mk IC", AircraftType.Bomber, AircraftEra.WWII, 410f, 8000f, 13000f, "UK"),
                new AircraftData("Vickers Wellington Mk III", AircraftType.Bomber, AircraftEra.WWII, 415f, 8200f, 13200f, "UK"),
                new AircraftData("Bristol Blenheim Mk IV", AircraftType.Bomber, AircraftEra.WWII, 420f, 5000f, 9000f, "UK"),
                new AircraftData("de Havilland Mosquito B.IV", AircraftType.Bomber, AircraftEra.WWII, 668f, 6000f, 10000f, "UK"),
                new AircraftData("de Havilland Mosquito B.XVI", AircraftType.Bomber, AircraftEra.WWII, 675f, 6500f, 10500f, "UK"),
                
                // German Bombers
                new AircraftData("Heinkel He 111H-3", AircraftType.Bomber, AircraftEra.WWII, 440f, 8000f, 14000f, "Germany"),
                new AircraftData("Heinkel He 111H-6", AircraftType.Bomber, AircraftEra.WWII, 445f, 8200f, 14200f, "Germany"),
                new AircraftData("Dornier Do 17Z", AircraftType.Bomber, AircraftEra.WWII, 425f, 4000f, 8000f, "Germany"),
                new AircraftData("Junkers Ju 88A-4", AircraftType.Bomber, AircraftEra.WWII, 470f, 6000f, 10000f, "Germany"),
                new AircraftData("Junkers Ju 188", AircraftType.Bomber, AircraftEra.WWII, 499f, 7000f, 11000f, "Germany"),
                new AircraftData("Arado Ar 234B Blitz", AircraftType.Bomber, AircraftEra.WWII, 742f, 4500f, 9000f, "Germany"),
                
                // Soviet Bombers
                new AircraftData("Ilyushin Il-2 Sturmovik", AircraftType.AttackAircraft, AircraftEra.WWII, 414f, 5000f, 9000f, "USSR"),
                new AircraftData("Ilyushin Il-10", AircraftType.AttackAircraft, AircraftEra.WWII, 551f, 5500f, 9500f, "USSR"),
                new AircraftData("Petlyakov Pe-2", AircraftType.Bomber, AircraftEra.WWII, 540f, 6000f, 10000f, "USSR"),
                new AircraftData("Tupolev Tu-2", AircraftType.Bomber, AircraftEra.WWII, 547f, 8000f, 13000f, "USSR"),
                
                // Japanese Bombers
                new AircraftData("Mitsubishi G4M 'Betty'", AircraftType.Bomber, AircraftEra.WWII, 428f, 9000f, 15000f, "Japan"),
                new AircraftData("Nakajima B5N 'Kate'", AircraftType.Bomber, AircraftEra.WWII, 378f, 3500f, 7000f, "Japan"),
                new AircraftData("Aichi D3A 'Val'", AircraftType.Bomber, AircraftEra.WWII, 389f, 3200f, 6500f, "Japan"),
                new AircraftData("Yokosuka D4Y 'Judy'", AircraftType.Bomber, AircraftEra.WWII, 570f, 4000f, 8000f, "Japan"),
            };
        }
        
        // Continue in next part due to character limit...
        
        // Helper method to get all aircraft
        public static List<AircraftData> GetAllAircraft()
        {
            var allAircraft = new List<AircraftData>();
            
            // WWI
            allAircraft.AddRange(GetWWIFighters());
            allAircraft.AddRange(GetWWIBombers());
            
            // Interwar
            allAircraft.AddRange(GetInterwarFighters());
            
            // WWII
            allAircraft.AddRange(GetWWIIFighters());
            allAircraft.AddRange(GetWWIIBombers());
            
            // Will add more in next file: Korea, Cold War, Modern, etc.
            
            return allAircraft;
        }
    }
    
    // Aircraft data structure
    [System.Serializable]
    public class AircraftData
    {
        public string name;
        public AircraftType type;
        public AircraftEra era;
        public float maxSpeed;
        public float weight;
        public float maxWeight;
        public string nation;
        public List<string> weapons = new List<string>();
        public Dictionary<string, float> performance = new Dictionary<string, float>();
        
        public AircraftData(string name, AircraftType type, AircraftEra era, float maxSpeed, float weight, float maxWeight, string nation)
        {
            this.name = name;
            this.type = type;
            this.era = era;
            this.maxSpeed = maxSpeed;
            this.weight = weight;
            this.maxWeight = maxWeight;
            this.nation = nation;
            
            // Initialize performance stats
            performance["Maneuverability"] = CalculateManeuverability();
            performance["ClimbRate"] = CalculateClimbRate();
            performance["DiveSpeed"] = CalculateDiv Speed();
            performance["TurnRate"] = CalculateTurnRate();
        }
        
        private float CalculateManeuverability()
        {
            return Mathf.Clamp(1000f / weight * 10f, 1f, 100f);
        }
        
        private float CalculateClimbRate()
        {
            return maxSpeed / weight * 100f;
        }
        
        private float CalculateDiveSpeed()
        {
            return maxSpeed * 1.2f;
        }
        
        private float CalculateTurnRate()
        {
            return Mathf.Clamp(500f / weight * maxSpeed / 100f, 5f, 50f);
        }
    }
    
    public enum AircraftType
    {
        Fighter,
        Bomber,
        AttackAircraft,
        Multirole,
        Transport,
        Cargo,
        Passenger,
        Reconnaissance,
        Trainer
    }
    
    public enum AircraftEra
    {
        WWI,
        Interwar,
        WWII,
        Korea,
        ColdWar,
        Modern,
        Contemporary
    }
}
