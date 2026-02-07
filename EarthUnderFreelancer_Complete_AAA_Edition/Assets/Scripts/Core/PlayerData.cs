using UnityEngine;
using System;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Core
{
    /// <summary>
    /// Player data class containing all persistent player information
    /// </summary>
    [Serializable]
    public class PlayerData
    {
        public string playerName = "Pilot";
        public string playerId;
        public int playerLevel = 1;
        public int experiencePoints = 0;

        public int credits = 10000;
        public int premiumCurrency = 0;

        public int totalMissionsCompleted = 0;
        public int totalKills = 0;
        public int totalDeaths = 0;
        public int pvpWins = 0;
        public int pvpLosses = 0;
        public float totalPlayTime = 0f;
        public int highScore = 0;

        public List<OwnedVehicle> ownedVehicles = new List<OwnedVehicle>();
        public string currentVehicleId = "fighter_basic";

        public List<InventoryItem> inventory = new List<InventoryItem>();

        public List<string> unlockedMissions = new List<string>();
        public List<string> unlockedAchievements = new List<string>();
        public List<string> purchasedCosmetics = new List<string>();

        public Dictionary<string, int> factionReputation = new Dictionary<string, int>();

        public PlayerData()
        {
            playerId = Guid.NewGuid().ToString();
            
            ownedVehicles.Add(new OwnedVehicle
            {
                vehicleId = "fighter_basic",
                vehicleName = "Hawk MK-I",
                vehicleType = VehicleType.Fighter,
                level = 1,
                customization = new VehicleCustomization()
            });

            factionReputation["Federation"] = 0;
            factionReputation["Empire"] = 0;
            factionReputation["Freelancers"] = 50;
            factionReputation["Pirates"] = -25;

            unlockedMissions.Add("tutorial_01");
            unlockedMissions.Add("patrol_01");
        }

        public int GetExperienceForNextLevel()
        {
            return playerLevel * 1000 + (playerLevel - 1) * 500;
        }

        public void AddExperience(int amount)
        {
            experiencePoints += amount;
            while (experiencePoints >= GetExperienceForNextLevel())
            {
                experiencePoints -= GetExperienceForNextLevel();
                playerLevel++;
            }
        }

        public float GetKillDeathRatio()
        {
            return totalDeaths > 0 ? (float)totalKills / totalDeaths : totalKills;
        }

        public float GetWinRate()
        {
            int totalMatches = pvpWins + pvpLosses;
            return totalMatches > 0 ? (float)pvpWins / totalMatches * 100f : 0f;
        }
    }

    [Serializable]
    public class OwnedVehicle
    {
        public string vehicleId;
        public string vehicleName;
        public VehicleType vehicleType;
        public int level = 1;
        public int upgradePoints = 0;
        
        public int engineLevel = 0;
        public int armorLevel = 0;
        public int weaponLevel = 0;
        public int shieldLevel = 0;

        public List<string> equippedWeapons = new List<string>();
        public List<string> equippedModules = new List<string>();

        public VehicleCustomization customization;
    }

    [Serializable]
    public class VehicleCustomization
    {
        public int skinIndex = 0;
        public Color primaryColor = Color.gray;
        public Color secondaryColor = Color.blue;
        public string decalId = "";
        public string trailEffectId = "";
    }

    public enum VehicleType
    {
        Fighter,
        Bomber,
        Interceptor,
        Transport,
        Cruiser,
        Battleship
    }

    [Serializable]
    public class InventoryItem
    {
        public string itemId;
        public string itemName;
        public ItemType itemType;
        public int quantity = 1;
        public int value;
    }

    public enum ItemType
    {
        Weapon,
        Module,
        Consumable,
        Resource,
        Cosmetic
    }
}
