using UnityEngine;

namespace EarthUnderFreelancer.Core
{
    /// <summary>
    /// Global game constants and configuration
    /// </summary>
    public static class GameConstants
    {
        // Version
        public const string VERSION = "1.0.0";
        public const string GAME_NAME = "EarthUnder Freelancer";
        
        // World Scale
        public const float WORLD_SCALE = 1000f; // 1 unit = 1km
        public const float EARTH_RADIUS_KM = 6371f;
        
        // Economy
        public const int STARTING_CREDITS = 50000;
        public const float REPAIR_COST_MULTIPLIER = 0.1f;
        public const float FUEL_COST_PER_LITER = 2.5f;
        
        // Combat
        public const float MAX_RENDER_DISTANCE = 50000f; // 50km
        public const float MAX_RADAR_RANGE = 100000f; // 100km
        public const float BULLET_DROP_GRAVITY = 9.81f;
        
        // PVE Zone
        public const float PVE_ZONE_RADIUS_KM = 150f; // Safe zone around capitals
        
        // Factions
        public static readonly string[] FACTIONS = new string[]
        {
            "NATO", "Warsaw Pact", "Neutral", "Pirate", "Mercenary",
            "Corporate", "Civilian", "Military", "Trader Guild", "Smuggler Cartel"
        };
        
        // Player Classes
        public enum PlayerClass
        {
            Trader,
            Smuggler,
            Military,
            Pirate,
            Mercenary,
            Civilian
        }
        
        // Mission Types
        public enum MissionType
        {
            Combat,
            Cargo,
            Passenger,
            Smuggling,
            Military,
            Trading,
            Pirate,
            Escort,
            Reconnaissance,
            Interception,
            Strike,
            Patrol,
            Search,
            Rescue,
            Delivery,
            Courier
        }
    }
}
