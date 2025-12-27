using UnityEngine;
using System.Collections.Generic;
using EarthUnderFreelancer.Missions;

namespace EarthUnderFreelancer.Gameplay
{
    /// <summary>
    /// Extended mission library with 15+ additional missions
    /// Expands DemoMissionManager to reach 20 total missions
    /// </summary>
    public class ExtendedMissionLibrary : MonoBehaviour
    {
        public static List<MissionTemplate> GetAllMissions()
        {
            List<MissionTemplate> missions = new List<MissionTemplate>();
            
            // Training Missions (6-10)
            missions.Add(new MissionTemplate
            {
                name = "Advanced Flight Training",
                description = "Master advanced flight maneuvers including loops and rolls",
                difficulty = MissionDifficulty.Normal,
                credits = 300,
                xp = 150,
                type = MissionType.Tutorial
            });
            
            missions.Add(new MissionTemplate
            {
                name = "Weapons Qualification",
                description = "Complete weapons training course with all weapon types",
                difficulty = MissionDifficulty.Normal,
                credits = 350,
                xp = 175,
                type = MissionType.Tutorial
            });
            
            missions.Add(new MissionTemplate
            {
                name = "Night Flight Certification",
                description = "Learn to fly in low visibility conditions",
                difficulty = MissionDifficulty.Normal,
                credits = 400,
                xp = 200,
                type = MissionType.Tutorial
            });
            
            // Combat Missions (11-15)
            missions.Add(new MissionTemplate
            {
                name = "Defend the Base",
                description = "Enemy bombers approaching! Intercept before they reach the base",
                difficulty = MissionDifficulty.Normal,
                credits = 600,
                xp = 250,
                type = MissionType.Defense
            });
            
            missions.Add(new MissionTemplate
            {
                name = "Strike Mission",
                description = "Destroy enemy ground targets in hostile territory",
                difficulty = MissionDifficulty.Hard,
                credits = 900,
                xp = 400,
                type = MissionType.Raid
            });
            
            missions.Add(new MissionTemplate
            {
                name = "Bomber Escort",
                description = "Protect friendly bombers during their attack run",
                difficulty = MissionDifficulty.Hard,
                credits = 800,
                xp = 350,
                type = MissionType.Escort
            });
            
            missions.Add(new MissionTemplate
            {
                name = "Air Superiority",
                description = "Establish air dominance by defeating all enemy fighters",
                difficulty = MissionDifficulty.Hard,
                credits = 1000,
                xp = 450,
                type = MissionType.Patrol
            });
            
            missions.Add(new MissionTemplate
            {
                name = "Ace Duel",
                description = "Face off against an enemy ace pilot in one-on-one combat",
                difficulty = MissionDifficulty.Expert,
                credits = 1500,
                xp = 600,
                type = MissionType.Bounty
            });
            
            // Exploration Missions (16-18)
            missions.Add(new MissionTemplate
            {
                name = "Reconnaissance Flight",
                description = "Scout enemy positions and report back",
                difficulty = MissionDifficulty.Normal,
                credits = 500,
                xp = 225,
                type = MissionType.Exploration
            });
            
            missions.Add(new MissionTemplate
            {
                name = "Search and Rescue",
                description = "Locate and rescue downed pilots behind enemy lines",
                difficulty = MissionDifficulty.Hard,
                credits = 850,
                xp = 375,
                type = MissionType.Exploration
            });
            
            missions.Add(new MissionTemplate
            {
                name = "Long Range Patrol",
                description = "Patrol remote sectors and investigate suspicious activity",
                difficulty = MissionDifficulty.Normal,
                credits = 550,
                xp = 250,
                type = MissionType.Patrol
            });
            
            // Trading Missions (19-20)
            missions.Add(new MissionTemplate
            {
                name = "Cargo Delivery",
                description = "Transport valuable cargo to distant station",
                difficulty = MissionDifficulty.Easy,
                credits = 400,
                xp = 150,
                type = MissionType.Trading
            });
            
            missions.Add(new MissionTemplate
            {
                name = "Smuggling Run",
                description = "Evade patrols while transporting contraband",
                difficulty = MissionDifficulty.Hard,
                credits = 1200,
                xp = 500,
                type = MissionType.Trading
            });
            
            // Elite Missions (21-25)
            missions.Add(new MissionTemplate
            {
                name = "Squadron Leader",
                description = "Lead a squadron in a major offensive operation",
                difficulty = MissionDifficulty.Expert,
                credits = 2000,
                xp = 800,
                type = MissionType.Story
            });
            
            missions.Add(new MissionTemplate
            {
                name = "Behind Enemy Lines",
                description = "Deep strike mission into heavily defended territory",
                difficulty = MissionDifficulty.Expert,
                credits = 1800,
                xp = 750,
                type = MissionType.Raid
            });
            
            missions.Add(new MissionTemplate
            {
                name = "Last Stand",
                description = "Defend against overwhelming enemy forces",
                difficulty = MissionDifficulty.Nightmare,
                credits = 3000,
                xp = 1000,
                type = MissionType.Defense
            });
            
            return missions;
        }
        
        public class MissionTemplate
        {
            public string name;
            public string description;
            public MissionDifficulty difficulty;
            public int credits;
            public int xp;
            public MissionType type;
        }
    }
}
