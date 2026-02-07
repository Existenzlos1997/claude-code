using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Systems
{
    /// <summary>
    /// Ship upgrade and customization system
    /// </summary>
    public class ShipUpgradeSystem : MonoBehaviour
    {
        public static ShipUpgradeSystem Instance { get; private set; }

        [Header("Current Ship")]
        [SerializeField] private string currentShipId = "default_ship";
        [SerializeField] private List<InstalledUpgrade> installedUpgrades = new List<InstalledUpgrade>();

        [Header("Available Upgrades")]
        [SerializeField] private List<ShipUpgrade> availableUpgrades = new List<ShipUpgrade>();

        [Header("Upgrade Slots")]
        [SerializeField] private int maxWeaponSlots = 4;
        [SerializeField] private int maxEngineSlots = 1;
        [SerializeField] private int maxShieldSlots = 1;
        [SerializeField] private int maxUtilitySlots = 3;
        [SerializeField] private int maxArmorSlots = 2;

        public string CurrentShipId => currentShipId;
        public List<InstalledUpgrade> InstalledUpgrades => installedUpgrades;

        public event System.Action<ShipUpgrade> OnUpgradeInstalled;
        public event System.Action<ShipUpgrade> OnUpgradeRemoved;
        public event System.Action OnShipChanged;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                InitializeUpgrades();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeUpgrades()
        {
            // Weapons
            availableUpgrades.Add(new ShipUpgrade
            {
                upgradeId = "weapon_laser_mk1",
                upgradeName = "Laser Cannon Mk1",
                description = "Basic laser weapon",
                category = UpgradeCategory.Weapon,
                tier = 1,
                price = 500,
                statModifiers = new List<UpgradeStatModifier>
                {
                    new UpgradeStatModifier { statType = UpgradeStatType.WeaponDamage, value = 10f }
                }
            });

            availableUpgrades.Add(new ShipUpgrade
            {
                upgradeId = "weapon_laser_mk2",
                upgradeName = "Laser Cannon Mk2",
                description = "Improved laser with higher damage",
                category = UpgradeCategory.Weapon,
                tier = 2,
                price = 1500,
                requiredLevel = 5,
                statModifiers = new List<UpgradeStatModifier>
                {
                    new UpgradeStatModifier { statType = UpgradeStatType.WeaponDamage, value = 20f },
                    new UpgradeStatModifier { statType = UpgradeStatType.FireRate, value = 0.1f }
                }
            });

            availableUpgrades.Add(new ShipUpgrade
            {
                upgradeId = "weapon_plasma_mk1",
                upgradeName = "Plasma Blaster Mk1",
                description = "High-damage plasma weapon",
                category = UpgradeCategory.Weapon,
                tier = 2,
                price = 2000,
                requiredLevel = 8,
                statModifiers = new List<UpgradeStatModifier>
                {
                    new UpgradeStatModifier { statType = UpgradeStatType.WeaponDamage, value = 35f }
                }
            });

            availableUpgrades.Add(new ShipUpgrade
            {
                upgradeId = "weapon_missile_launcher",
                upgradeName = "Missile Launcher",
                description = "Launches tracking missiles",
                category = UpgradeCategory.Weapon,
                tier = 2,
                price = 3000,
                requiredLevel = 10,
                statModifiers = new List<UpgradeStatModifier>
                {
                    new UpgradeStatModifier { statType = UpgradeStatType.MissileDamage, value = 50f }
                }
            });

            // Engines
            availableUpgrades.Add(new ShipUpgrade
            {
                upgradeId = "engine_mk1",
                upgradeName = "Thruster Mk1",
                description = "Basic engine upgrade",
                category = UpgradeCategory.Engine,
                tier = 1,
                price = 800,
                statModifiers = new List<UpgradeStatModifier>
                {
                    new UpgradeStatModifier { statType = UpgradeStatType.MaxSpeed, value = 10f, isPercentage = true }
                }
            });

            availableUpgrades.Add(new ShipUpgrade
            {
                upgradeId = "engine_mk2",
                upgradeName = "Thruster Mk2",
                description = "High-performance engine",
                category = UpgradeCategory.Engine,
                tier = 2,
                price = 2500,
                requiredLevel = 7,
                statModifiers = new List<UpgradeStatModifier>
                {
                    new UpgradeStatModifier { statType = UpgradeStatType.MaxSpeed, value = 20f, isPercentage = true },
                    new UpgradeStatModifier { statType = UpgradeStatType.Acceleration, value = 15f, isPercentage = true }
                }
            });

            availableUpgrades.Add(new ShipUpgrade
            {
                upgradeId = "engine_boost",
                upgradeName = "Afterburner",
                description = "Improved boost system",
                category = UpgradeCategory.Engine,
                tier = 2,
                price = 2000,
                requiredLevel = 5,
                statModifiers = new List<UpgradeStatModifier>
                {
                    new UpgradeStatModifier { statType = UpgradeStatType.BoostDuration, value = 50f, isPercentage = true },
                    new UpgradeStatModifier { statType = UpgradeStatType.BoostSpeed, value = 25f, isPercentage = true }
                }
            });

            // Shields
            availableUpgrades.Add(new ShipUpgrade
            {
                upgradeId = "shield_mk1",
                upgradeName = "Shield Generator Mk1",
                description = "Basic shield upgrade",
                category = UpgradeCategory.Shield,
                tier = 1,
                price = 1000,
                statModifiers = new List<UpgradeStatModifier>
                {
                    new UpgradeStatModifier { statType = UpgradeStatType.ShieldCapacity, value = 25f, isPercentage = true }
                }
            });

            availableUpgrades.Add(new ShipUpgrade
            {
                upgradeId = "shield_mk2",
                upgradeName = "Shield Generator Mk2",
                description = "Advanced shield with regen",
                category = UpgradeCategory.Shield,
                tier = 2,
                price = 3000,
                requiredLevel = 10,
                statModifiers = new List<UpgradeStatModifier>
                {
                    new UpgradeStatModifier { statType = UpgradeStatType.ShieldCapacity, value = 50f, isPercentage = true },
                    new UpgradeStatModifier { statType = UpgradeStatType.ShieldRegen, value = 25f, isPercentage = true }
                }
            });

            // Armor
            availableUpgrades.Add(new ShipUpgrade
            {
                upgradeId = "armor_light",
                upgradeName = "Light Armor Plating",
                description = "Adds extra hull protection",
                category = UpgradeCategory.Armor,
                tier = 1,
                price = 600,
                statModifiers = new List<UpgradeStatModifier>
                {
                    new UpgradeStatModifier { statType = UpgradeStatType.HullStrength, value = 15f, isPercentage = true }
                }
            });

            availableUpgrades.Add(new ShipUpgrade
            {
                upgradeId = "armor_heavy",
                upgradeName = "Heavy Armor Plating",
                description = "Maximum hull protection",
                category = UpgradeCategory.Armor,
                tier = 2,
                price = 2000,
                requiredLevel = 8,
                statModifiers = new List<UpgradeStatModifier>
                {
                    new UpgradeStatModifier { statType = UpgradeStatType.HullStrength, value = 35f, isPercentage = true },
                    new UpgradeStatModifier { statType = UpgradeStatType.DamageReduction, value = 10f }
                }
            });

            // Utility
            availableUpgrades.Add(new ShipUpgrade
            {
                upgradeId = "cargo_expander",
                upgradeName = "Cargo Bay Expander",
                description = "Increases cargo capacity",
                category = UpgradeCategory.Utility,
                tier = 1,
                price = 500,
                statModifiers = new List<UpgradeStatModifier>
                {
                    new UpgradeStatModifier { statType = UpgradeStatType.CargoCapacity, value = 25f, isPercentage = true }
                }
            });

            availableUpgrades.Add(new ShipUpgrade
            {
                upgradeId = "scanner_upgrade",
                upgradeName = "Enhanced Scanner",
                description = "Improves radar range",
                category = UpgradeCategory.Utility,
                tier = 1,
                price = 750,
                statModifiers = new List<UpgradeStatModifier>
                {
                    new UpgradeStatModifier { statType = UpgradeStatType.RadarRange, value = 50f, isPercentage = true }
                }
            });

            availableUpgrades.Add(new ShipUpgrade
            {
                upgradeId = "targeting_computer",
                upgradeName = "Targeting Computer",
                description = "Improves weapon accuracy",
                category = UpgradeCategory.Utility,
                tier = 2,
                price = 1500,
                requiredLevel = 6,
                statModifiers = new List<UpgradeStatModifier>
                {
                    new UpgradeStatModifier { statType = UpgradeStatType.WeaponAccuracy, value = 20f, isPercentage = true },
                    new UpgradeStatModifier { statType = UpgradeStatType.LockOnSpeed, value = 30f, isPercentage = true }
                }
            });

            availableUpgrades.Add(new ShipUpgrade
            {
                upgradeId = "repair_drone",
                upgradeName = "Repair Drone",
                description = "Slowly repairs hull over time",
                category = UpgradeCategory.Utility,
                tier = 3,
                price = 5000,
                requiredLevel = 15,
                statModifiers = new List<UpgradeStatModifier>
                {
                    new UpgradeStatModifier { statType = UpgradeStatType.HullRegen, value = 1f }
                }
            });
        }

        public bool CanInstallUpgrade(string upgradeId)
        {
            ShipUpgrade upgrade = GetUpgrade(upgradeId);
            if (upgrade == null) return false;

            // Check level requirement
            if (ProgressionManager.Instance != null)
            {
                if (ProgressionManager.Instance.CurrentLevel < upgrade.requiredLevel)
                    return false;
            }

            // Check slot availability
            int usedSlots = GetUsedSlots(upgrade.category);
            int maxSlots = GetMaxSlots(upgrade.category);
            
            if (usedSlots >= maxSlots) return false;

            // Check if already installed
            foreach (var installed in installedUpgrades)
            {
                if (installed.upgradeId == upgradeId)
                    return false;
            }

            return true;
        }

        public bool InstallUpgrade(string upgradeId)
        {
            if (!CanInstallUpgrade(upgradeId)) return false;

            ShipUpgrade upgrade = GetUpgrade(upgradeId);
            if (upgrade == null) return false;

            // Check and deduct cost
            if (EconomyManager.Instance != null)
            {
                if (EconomyManager.Instance.Credits < upgrade.price)
                    return false;
                
                EconomyManager.Instance.SpendCredits(upgrade.price);
            }

            installedUpgrades.Add(new InstalledUpgrade
            {
                upgradeId = upgradeId,
                category = upgrade.category,
                installTime = Time.time
            });

            ApplyUpgradeStats(upgrade);
            OnUpgradeInstalled?.Invoke(upgrade);

            return true;
        }

        public bool RemoveUpgrade(string upgradeId)
        {
            InstalledUpgrade toRemove = null;
            foreach (var installed in installedUpgrades)
            {
                if (installed.upgradeId == upgradeId)
                {
                    toRemove = installed;
                    break;
                }
            }

            if (toRemove == null) return false;

            ShipUpgrade upgrade = GetUpgrade(upgradeId);
            
            installedUpgrades.Remove(toRemove);
            RemoveUpgradeStats(upgrade);
            
            // Refund partial cost
            if (EconomyManager.Instance != null && upgrade != null)
            {
                EconomyManager.Instance.AddCredits(upgrade.price / 2);
            }

            OnUpgradeRemoved?.Invoke(upgrade);
            return true;
        }

        private void ApplyUpgradeStats(ShipUpgrade upgrade)
        {
            // Apply stat modifiers to player ship
            // This would integrate with VehicleController and HealthSystem
            foreach (var mod in upgrade.statModifiers)
            {
                Debug.Log($"Applying upgrade stat: {mod.statType} +{mod.value}{(mod.isPercentage ? "%" : "")}");
            }
        }

        private void RemoveUpgradeStats(ShipUpgrade upgrade)
        {
            if (upgrade == null) return;
            
            foreach (var mod in upgrade.statModifiers)
            {
                Debug.Log($"Removing upgrade stat: {mod.statType}");
            }
        }

        public float GetTotalStatBonus(UpgradeStatType statType, bool percentageOnly = false)
        {
            float total = 0f;

            foreach (var installed in installedUpgrades)
            {
                ShipUpgrade upgrade = GetUpgrade(installed.upgradeId);
                if (upgrade == null) continue;

                foreach (var mod in upgrade.statModifiers)
                {
                    if (mod.statType == statType)
                    {
                        if (percentageOnly && !mod.isPercentage) continue;
                        total += mod.value;
                    }
                }
            }

            return total;
        }

        public int GetUsedSlots(UpgradeCategory category)
        {
            int count = 0;
            foreach (var installed in installedUpgrades)
            {
                if (installed.category == category)
                    count++;
            }
            return count;
        }

        public int GetMaxSlots(UpgradeCategory category)
        {
            switch (category)
            {
                case UpgradeCategory.Weapon: return maxWeaponSlots;
                case UpgradeCategory.Engine: return maxEngineSlots;
                case UpgradeCategory.Shield: return maxShieldSlots;
                case UpgradeCategory.Utility: return maxUtilitySlots;
                case UpgradeCategory.Armor: return maxArmorSlots;
                default: return 1;
            }
        }

        public ShipUpgrade GetUpgrade(string upgradeId)
        {
            foreach (var upgrade in availableUpgrades)
            {
                if (upgrade.upgradeId == upgradeId)
                    return upgrade;
            }
            return null;
        }

        public List<ShipUpgrade> GetUpgradesByCategory(UpgradeCategory category)
        {
            List<ShipUpgrade> result = new List<ShipUpgrade>();
            foreach (var upgrade in availableUpgrades)
            {
                if (upgrade.category == category)
                    result.Add(upgrade);
            }
            return result;
        }

        public List<InstalledUpgrade> GetInstalledByCategory(UpgradeCategory category)
        {
            List<InstalledUpgrade> result = new List<InstalledUpgrade>();
            foreach (var installed in installedUpgrades)
            {
                if (installed.category == category)
                    result.Add(installed);
            }
            return result;
        }

        public bool IsUpgradeInstalled(string upgradeId)
        {
            foreach (var installed in installedUpgrades)
            {
                if (installed.upgradeId == upgradeId)
                    return true;
            }
            return false;
        }

        public void ChangeShip(string shipId)
        {
            currentShipId = shipId;
            installedUpgrades.Clear();
            OnShipChanged?.Invoke();
        }

        public void SaveUpgrades()
        {
            string json = JsonUtility.ToJson(new UpgradeSaveData
            {
                shipId = currentShipId,
                upgrades = installedUpgrades
            });
            PlayerPrefs.SetString("ship_upgrades", json);
            PlayerPrefs.Save();
        }

        public void LoadUpgrades()
        {
            if (PlayerPrefs.HasKey("ship_upgrades"))
            {
                string json = PlayerPrefs.GetString("ship_upgrades");
                UpgradeSaveData data = JsonUtility.FromJson<UpgradeSaveData>(json);
                if (data != null)
                {
                    currentShipId = data.shipId;
                    installedUpgrades = data.upgrades ?? new List<InstalledUpgrade>();
                }
            }
        }
    }

    [System.Serializable]
    public class ShipUpgrade
    {
        public string upgradeId;
        public string upgradeName;
        [TextArea(1, 3)]
        public string description;
        public Sprite icon;
        public UpgradeCategory category;
        public int tier;
        public int price;
        public int requiredLevel;
        public List<UpgradeStatModifier> statModifiers = new List<UpgradeStatModifier>();
    }

    [System.Serializable]
    public class InstalledUpgrade
    {
        public string upgradeId;
        public UpgradeCategory category;
        public float installTime;
    }

    [System.Serializable]
    public class UpgradeStatModifier
    {
        public UpgradeStatType statType;
        public float value;
        public bool isPercentage;
    }

    public enum UpgradeCategory
    {
        Weapon,
        Engine,
        Shield,
        Armor,
        Utility
    }

    public enum UpgradeStatType
    {
        // Weapons
        WeaponDamage,
        FireRate,
        WeaponRange,
        WeaponAccuracy,
        MissileDamage,
        LockOnSpeed,
        
        // Defense
        ShieldCapacity,
        ShieldRegen,
        HullStrength,
        HullRegen,
        DamageReduction,
        
        // Mobility
        MaxSpeed,
        Acceleration,
        TurnRate,
        BoostDuration,
        BoostSpeed,
        
        // Utility
        CargoCapacity,
        RadarRange,
        FuelEfficiency
    }

    [System.Serializable]
    public class UpgradeSaveData
    {
        public string shipId;
        public List<InstalledUpgrade> upgrades;
    }
}
