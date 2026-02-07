using UnityEngine;
using System;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Housing
{
    /// <summary>
    /// Complete player hangar and base system
    /// Players can store aircraft, customize hangars, display trophies
    /// </summary>
    
    [Serializable]
    public enum HangarSlotType
    {
        Aircraft,
        Weapon,
        Equipment,
        Trophy,
        Decoration
    }
    
    [Serializable]
    public enum HangarUpgradeType
    {
        SlotCapacity,
        RepairSpeed,
        StorageSize,
        SecurityLevel,
        AestheticTier,
        WorkshopLevel,
        RadarRange,
        DefenseLevel
    }
    
    [Serializable]
    public class HangarSlot
    {
        public string slotId;
        public HangarSlotType slotType;
        public Vector3 localPosition;
        public Quaternion localRotation;
        public string occupyingItemId;
        public bool isUnlocked;
        public int unlockCost;
        
        public HangarSlot(string id, HangarSlotType type, Vector3 pos)
        {
            slotId = id;
            slotType = type;
            localPosition = pos;
            localRotation = Quaternion.identity;
            occupyingItemId = "";
            isUnlocked = false;
            unlockCost = 1000;
        }
    }
    
    [Serializable]
    public class HangarDecoration
    {
        public string decorationId;
        public string decorationName;
        public string prefabPath;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        public int purchaseCost;
        public bool isPlaced;
        
        public HangarDecoration(string id, string name, string prefab, int cost)
        {
            decorationId = id;
            decorationName = name;
            prefabPath = prefab;
            position = Vector3.zero;
            rotation = Quaternion.identity;
            scale = Vector3.one;
            purchaseCost = cost;
            isPlaced = false;
        }
    }
    
    [Serializable]
    public class StoredAircraft
    {
        public string aircraftId;
        public string aircraftType;
        public string customName;
        public float health;
        public float maxHealth;
        public bool needsRepair;
        public float repairProgress;
        public List<string> equippedWeapons;
        public List<string> equippedModifications;
        public int kills;
        public float flightHours;
        public DateTime lastUsed;
        public string skinId;
        public List<string> decals;
        
        public StoredAircraft(string id, string type)
        {
            aircraftId = id;
            aircraftType = type;
            customName = "";
            health = 100f;
            maxHealth = 100f;
            needsRepair = false;
            repairProgress = 0f;
            equippedWeapons = new List<string>();
            equippedModifications = new List<string>();
            kills = 0;
            flightHours = 0f;
            lastUsed = DateTime.Now;
            skinId = "default";
            decals = new List<string>();
        }
    }
    
    [Serializable]
    public class HangarTrophy
    {
        public string trophyId;
        public string trophyName;
        public string description;
        public string iconPath;
        public DateTime earnedDate;
        public string achievementSource;
        public int rarity; // 1-5
        
        public HangarTrophy(string id, string name, string desc, int rare)
        {
            trophyId = id;
            trophyName = name;
            description = desc;
            iconPath = "";
            earnedDate = DateTime.Now;
            achievementSource = "";
            rarity = rare;
        }
    }
    
    [Serializable]
    public class PlayerHangar
    {
        public string ownerId;
        public string hangarName;
        public int hangarLevel;
        public int maxAircraftSlots;
        public int maxWeaponSlots;
        public int maxDecorationSlots;
        public List<HangarSlot> slots;
        public List<StoredAircraft> storedAircraft;
        public List<HangarDecoration> decorations;
        public List<HangarTrophy> trophies;
        public Dictionary<HangarUpgradeType, int> upgradeLevels;
        public int currency;
        public bool isPublic;
        public int visitCount;
        public float rating;
        
        public PlayerHangar(string owner)
        {
            ownerId = owner;
            hangarName = "My Hangar";
            hangarLevel = 1;
            maxAircraftSlots = 3;
            maxWeaponSlots = 10;
            maxDecorationSlots = 5;
            slots = new List<HangarSlot>();
            storedAircraft = new List<StoredAircraft>();
            decorations = new List<HangarDecoration>();
            trophies = new List<HangarTrophy>();
            upgradeLevels = new Dictionary<HangarUpgradeType, int>();
            currency = 0;
            isPublic = false;
            visitCount = 0;
            rating = 0f;
            
            InitializeDefaultSlots();
            InitializeUpgrades();
        }
        
        private void InitializeDefaultSlots()
        {
            // Default aircraft slots
            for (int i = 0; i < 3; i++)
            {
                var slot = new HangarSlot($"aircraft_{i}", HangarSlotType.Aircraft, new Vector3(i * 15f, 0, 0));
                slot.isUnlocked = i == 0; // First slot free
                slot.unlockCost = (i + 1) * 5000;
                slots.Add(slot);
            }
            
            // Weapon storage slots
            for (int i = 0; i < 10; i++)
            {
                var slot = new HangarSlot($"weapon_{i}", HangarSlotType.Weapon, new Vector3(-10f, 0, i * 2f));
                slot.isUnlocked = i < 5; // First 5 free
                slot.unlockCost = 1000;
                slots.Add(slot);
            }
            
            // Trophy display slots
            for (int i = 0; i < 5; i++)
            {
                var slot = new HangarSlot($"trophy_{i}", HangarSlotType.Trophy, new Vector3(10f + i * 3f, 2f, 0));
                slot.isUnlocked = true;
                slots.Add(slot);
            }
        }
        
        private void InitializeUpgrades()
        {
            foreach (HangarUpgradeType type in Enum.GetValues(typeof(HangarUpgradeType)))
            {
                upgradeLevels[type] = 1;
            }
        }
    }
    
    public class HangarSystem : MonoBehaviour
    {
        public static HangarSystem Instance { get; private set; }
        
        [Header("Hangar Settings")]
        [SerializeField] private int maxHangarLevel = 10;
        [SerializeField] private int baseAircraftSlots = 3;
        [SerializeField] private int slotsPerLevel = 2;
        [SerializeField] private float baseRepairSpeed = 1f;
        [SerializeField] private float repairSpeedPerLevel = 0.5f;
        
        [Header("Upgrade Costs")]
        [SerializeField] private int[] levelUpgradeCosts = { 0, 5000, 15000, 35000, 75000, 150000, 300000, 600000, 1200000, 2500000 };
        
        [Header("Decoration Catalog")]
        [SerializeField] private List<HangarDecoration> availableDecorations = new List<HangarDecoration>();
        
        private Dictionary<string, PlayerHangar> playerHangars = new Dictionary<string, PlayerHangar>();
        private PlayerHangar currentViewingHangar;
        private bool isInEditMode = false;
        
        public event Action<PlayerHangar> OnHangarLoaded;
        public event Action<StoredAircraft> OnAircraftStored;
        public event Action<StoredAircraft> OnAircraftRetrieved;
        public event Action<HangarDecoration> OnDecorationPlaced;
        public event Action<int> OnHangarUpgraded;
        public event Action<StoredAircraft> OnRepairComplete;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeDecorationCatalog();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void InitializeDecorationCatalog()
        {
            availableDecorations = new List<HangarDecoration>
            {
                new HangarDecoration("poster_001", "Victory Poster", "Decorations/Posters/Victory", 500),
                new HangarDecoration("poster_002", "Pin-Up Art", "Decorations/Posters/PinUp", 750),
                new HangarDecoration("flag_001", "National Flag", "Decorations/Flags/National", 1000),
                new HangarDecoration("flag_002", "Squadron Banner", "Decorations/Flags/Squadron", 1500),
                new HangarDecoration("tool_001", "Tool Rack", "Decorations/Tools/Rack", 2000),
                new HangarDecoration("tool_002", "Workbench", "Decorations/Tools/Workbench", 3000),
                new HangarDecoration("light_001", "Hangar Lights", "Decorations/Lighting/HangarLight", 1000),
                new HangarDecoration("light_002", "Spotlight", "Decorations/Lighting/Spotlight", 1500),
                new HangarDecoration("floor_001", "Oil Stains", "Decorations/Floor/OilStains", 200),
                new HangarDecoration("floor_002", "Hazard Lines", "Decorations/Floor/HazardLines", 500),
                new HangarDecoration("vehicle_001", "Fuel Truck", "Decorations/Vehicles/FuelTruck", 5000),
                new HangarDecoration("vehicle_002", "Ammunition Cart", "Decorations/Vehicles/AmmoCart", 3000),
                new HangarDecoration("prop_001", "Propeller Display", "Decorations/Props/PropellerDisplay", 2500),
                new HangarDecoration("engine_001", "Spare Engine", "Decorations/Props/SpareEngine", 4000),
                new HangarDecoration("radio_001", "Radio Equipment", "Decorations/Electronics/Radio", 2000),
                new HangarDecoration("map_001", "World Map", "Decorations/Maps/WorldMap", 1500),
                new HangarDecoration("clock_001", "Wall Clock", "Decorations/Misc/Clock", 800),
                new HangarDecoration("calendar_001", "Calendar", "Decorations/Misc/Calendar", 300),
                new HangarDecoration("plant_001", "Potted Plant", "Decorations/Plants/Potted", 600),
                new HangarDecoration("trophy_display_001", "Trophy Cabinet", "Decorations/Displays/TrophyCabinet", 5000)
            };
        }
        
        private void Update()
        {
            if (currentViewingHangar != null)
            {
                ProcessRepairs();
            }
        }
        
        private void ProcessRepairs()
        {
            float repairSpeed = GetRepairSpeed(currentViewingHangar);
            
            foreach (var aircraft in currentViewingHangar.storedAircraft)
            {
                if (aircraft.needsRepair && aircraft.health < aircraft.maxHealth)
                {
                    aircraft.repairProgress += repairSpeed * Time.deltaTime;
                    
                    if (aircraft.repairProgress >= 100f)
                    {
                        aircraft.health = aircraft.maxHealth;
                        aircraft.needsRepair = false;
                        aircraft.repairProgress = 0f;
                        OnRepairComplete?.Invoke(aircraft);
                    }
                }
            }
        }
        
        public PlayerHangar GetOrCreateHangar(string playerId)
        {
            if (!playerHangars.ContainsKey(playerId))
            {
                playerHangars[playerId] = new PlayerHangar(playerId);
            }
            return playerHangars[playerId];
        }
        
        public void LoadHangar(string playerId)
        {
            currentViewingHangar = GetOrCreateHangar(playerId);
            OnHangarLoaded?.Invoke(currentViewingHangar);
        }
        
        public bool StoreAircraft(string playerId, string aircraftType, string customName = "")
        {
            var hangar = GetOrCreateHangar(playerId);
            
            // Check for available slot
            var availableSlot = hangar.slots.Find(s => 
                s.slotType == HangarSlotType.Aircraft && 
                s.isUnlocked && 
                string.IsNullOrEmpty(s.occupyingItemId));
            
            if (availableSlot == null)
            {
                Debug.LogWarning("No available aircraft slots in hangar");
                return false;
            }
            
            var aircraft = new StoredAircraft(Guid.NewGuid().ToString(), aircraftType);
            aircraft.customName = string.IsNullOrEmpty(customName) ? aircraftType : customName;
            
            availableSlot.occupyingItemId = aircraft.aircraftId;
            hangar.storedAircraft.Add(aircraft);
            
            OnAircraftStored?.Invoke(aircraft);
            return true;
        }
        
        public StoredAircraft RetrieveAircraft(string playerId, string aircraftId)
        {
            var hangar = GetOrCreateHangar(playerId);
            var aircraft = hangar.storedAircraft.Find(a => a.aircraftId == aircraftId);
            
            if (aircraft == null)
            {
                Debug.LogWarning($"Aircraft {aircraftId} not found in hangar");
                return null;
            }
            
            if (aircraft.needsRepair)
            {
                Debug.LogWarning("Aircraft needs repair before it can be used");
                return null;
            }
            
            // Clear slot
            var slot = hangar.slots.Find(s => s.occupyingItemId == aircraftId);
            if (slot != null)
            {
                slot.occupyingItemId = "";
            }
            
            hangar.storedAircraft.Remove(aircraft);
            aircraft.lastUsed = DateTime.Now;
            
            OnAircraftRetrieved?.Invoke(aircraft);
            return aircraft;
        }
        
        public bool UnlockSlot(string playerId, string slotId)
        {
            var hangar = GetOrCreateHangar(playerId);
            var slot = hangar.slots.Find(s => s.slotId == slotId);
            
            if (slot == null || slot.isUnlocked)
            {
                return false;
            }
            
            if (hangar.currency < slot.unlockCost)
            {
                Debug.LogWarning("Not enough currency to unlock slot");
                return false;
            }
            
            hangar.currency -= slot.unlockCost;
            slot.isUnlocked = true;
            return true;
        }
        
        public bool UpgradeHangar(string playerId)
        {
            var hangar = GetOrCreateHangar(playerId);
            
            if (hangar.hangarLevel >= maxHangarLevel)
            {
                Debug.LogWarning("Hangar already at max level");
                return false;
            }
            
            int cost = levelUpgradeCosts[hangar.hangarLevel];
            
            if (hangar.currency < cost)
            {
                Debug.LogWarning("Not enough currency to upgrade hangar");
                return false;
            }
            
            hangar.currency -= cost;
            hangar.hangarLevel++;
            hangar.maxAircraftSlots = baseAircraftSlots + (hangar.hangarLevel - 1) * slotsPerLevel;
            
            // Add new slots
            for (int i = 0; i < slotsPerLevel; i++)
            {
                int slotIndex = hangar.slots.FindAll(s => s.slotType == HangarSlotType.Aircraft).Count;
                var newSlot = new HangarSlot($"aircraft_{slotIndex}", HangarSlotType.Aircraft, 
                    new Vector3(slotIndex * 15f, 0, 0));
                newSlot.unlockCost = hangar.hangarLevel * 5000;
                hangar.slots.Add(newSlot);
            }
            
            OnHangarUpgraded?.Invoke(hangar.hangarLevel);
            return true;
        }
        
        public bool UpgradeSpecific(string playerId, HangarUpgradeType upgradeType)
        {
            var hangar = GetOrCreateHangar(playerId);
            
            int currentLevel = hangar.upgradeLevels[upgradeType];
            if (currentLevel >= 10)
            {
                return false;
            }
            
            int cost = currentLevel * 2000;
            if (hangar.currency < cost)
            {
                return false;
            }
            
            hangar.currency -= cost;
            hangar.upgradeLevels[upgradeType]++;
            return true;
        }
        
        public bool PlaceDecoration(string playerId, string decorationId, Vector3 position, Quaternion rotation)
        {
            var hangar = GetOrCreateHangar(playerId);
            
            var catalogItem = availableDecorations.Find(d => d.decorationId == decorationId);
            if (catalogItem == null)
            {
                return false;
            }
            
            // Check if already owned
            var owned = hangar.decorations.Find(d => d.decorationId == decorationId);
            if (owned == null)
            {
                if (hangar.currency < catalogItem.purchaseCost)
                {
                    return false;
                }
                
                hangar.currency -= catalogItem.purchaseCost;
                owned = new HangarDecoration(catalogItem.decorationId, catalogItem.decorationName, 
                    catalogItem.prefabPath, catalogItem.purchaseCost);
                hangar.decorations.Add(owned);
            }
            
            owned.position = position;
            owned.rotation = rotation;
            owned.isPlaced = true;
            
            OnDecorationPlaced?.Invoke(owned);
            return true;
        }
        
        public bool RemoveDecoration(string playerId, string decorationId)
        {
            var hangar = GetOrCreateHangar(playerId);
            var decoration = hangar.decorations.Find(d => d.decorationId == decorationId);
            
            if (decoration == null)
            {
                return false;
            }
            
            decoration.isPlaced = false;
            return true;
        }
        
        public void AddTrophy(string playerId, HangarTrophy trophy)
        {
            var hangar = GetOrCreateHangar(playerId);
            
            if (hangar.trophies.Exists(t => t.trophyId == trophy.trophyId))
            {
                return;
            }
            
            hangar.trophies.Add(trophy);
        }
        
        public void StartRepair(string playerId, string aircraftId)
        {
            var hangar = GetOrCreateHangar(playerId);
            var aircraft = hangar.storedAircraft.Find(a => a.aircraftId == aircraftId);
            
            if (aircraft != null && aircraft.health < aircraft.maxHealth)
            {
                aircraft.needsRepair = true;
                aircraft.repairProgress = 0f;
            }
        }
        
        public void InstantRepair(string playerId, string aircraftId, int cost)
        {
            var hangar = GetOrCreateHangar(playerId);
            
            if (hangar.currency < cost)
            {
                return;
            }
            
            var aircraft = hangar.storedAircraft.Find(a => a.aircraftId == aircraftId);
            if (aircraft != null)
            {
                hangar.currency -= cost;
                aircraft.health = aircraft.maxHealth;
                aircraft.needsRepair = false;
                aircraft.repairProgress = 0f;
                OnRepairComplete?.Invoke(aircraft);
            }
        }
        
        public void SetAircraftSkin(string playerId, string aircraftId, string skinId)
        {
            var hangar = GetOrCreateHangar(playerId);
            var aircraft = hangar.storedAircraft.Find(a => a.aircraftId == aircraftId);
            
            if (aircraft != null)
            {
                aircraft.skinId = skinId;
            }
        }
        
        public void AddDecal(string playerId, string aircraftId, string decalId)
        {
            var hangar = GetOrCreateHangar(playerId);
            var aircraft = hangar.storedAircraft.Find(a => a.aircraftId == aircraftId);
            
            if (aircraft != null && !aircraft.decals.Contains(decalId))
            {
                aircraft.decals.Add(decalId);
            }
        }
        
        public void EquipWeapon(string playerId, string aircraftId, string weaponId)
        {
            var hangar = GetOrCreateHangar(playerId);
            var aircraft = hangar.storedAircraft.Find(a => a.aircraftId == aircraftId);
            
            if (aircraft != null && !aircraft.equippedWeapons.Contains(weaponId))
            {
                aircraft.equippedWeapons.Add(weaponId);
            }
        }
        
        public void UnequipWeapon(string playerId, string aircraftId, string weaponId)
        {
            var hangar = GetOrCreateHangar(playerId);
            var aircraft = hangar.storedAircraft.Find(a => a.aircraftId == aircraftId);
            
            if (aircraft != null)
            {
                aircraft.equippedWeapons.Remove(weaponId);
            }
        }
        
        public void EquipModification(string playerId, string aircraftId, string modId)
        {
            var hangar = GetOrCreateHangar(playerId);
            var aircraft = hangar.storedAircraft.Find(a => a.aircraftId == aircraftId);
            
            if (aircraft != null && !aircraft.equippedModifications.Contains(modId))
            {
                aircraft.equippedModifications.Add(modId);
            }
        }
        
        public void SetHangarPublic(string playerId, bool isPublic)
        {
            var hangar = GetOrCreateHangar(playerId);
            hangar.isPublic = isPublic;
        }
        
        public void VisitHangar(string visitorId, string ownerId)
        {
            var hangar = GetOrCreateHangar(ownerId);
            
            if (hangar.isPublic || visitorId == ownerId)
            {
                hangar.visitCount++;
                LoadHangar(ownerId);
            }
        }
        
        public void RateHangar(string playerId, float rating)
        {
            var hangar = GetOrCreateHangar(playerId);
            
            // Simple average rating
            float totalRating = hangar.rating * (hangar.visitCount - 1) + rating;
            hangar.rating = totalRating / hangar.visitCount;
        }
        
        public void SetEditMode(bool editMode)
        {
            isInEditMode = editMode;
        }
        
        public bool IsInEditMode()
        {
            return isInEditMode;
        }
        
        public List<HangarDecoration> GetAvailableDecorations()
        {
            return new List<HangarDecoration>(availableDecorations);
        }
        
        public int GetAvailableAircraftSlots(string playerId)
        {
            var hangar = GetOrCreateHangar(playerId);
            return hangar.slots.FindAll(s => 
                s.slotType == HangarSlotType.Aircraft && 
                s.isUnlocked && 
                string.IsNullOrEmpty(s.occupyingItemId)).Count;
        }
        
        public float GetRepairSpeed(PlayerHangar hangar)
        {
            int workshopLevel = hangar.upgradeLevels[HangarUpgradeType.WorkshopLevel];
            return baseRepairSpeed + (workshopLevel - 1) * repairSpeedPerLevel;
        }
        
        public void AddCurrency(string playerId, int amount)
        {
            var hangar = GetOrCreateHangar(playerId);
            hangar.currency += amount;
        }
        
        public void RecordKill(string playerId, string aircraftId)
        {
            var hangar = GetOrCreateHangar(playerId);
            var aircraft = hangar.storedAircraft.Find(a => a.aircraftId == aircraftId);
            if (aircraft != null)
            {
                aircraft.kills++;
            }
        }
        
        public void RecordFlightTime(string playerId, string aircraftId, float hours)
        {
            var hangar = GetOrCreateHangar(playerId);
            var aircraft = hangar.storedAircraft.Find(a => a.aircraftId == aircraftId);
            if (aircraft != null)
            {
                aircraft.flightHours += hours;
            }
        }
    }
}
