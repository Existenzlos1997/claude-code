using UnityEngine;
using System;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Character
{
    /// <summary>
    /// Complete Character System - Classes, Races, Stats, and Character Creation
    /// Similar to WoW's character system
    /// </summary>
    public class CharacterSystem : MonoBehaviour
    {
        public static CharacterSystem Instance { get; private set; }

        [Header("Configuration")]
        [SerializeField] private List<RaceDefinition> availableRaces;
        [SerializeField] private List<ClassDefinition> availableClasses;
        [SerializeField] private List<BackgroundDefinition> availableBackgrounds;
        [SerializeField] private int maxCharactersPerAccount = 10;

        // Current Character
        public CharacterData CurrentCharacter { get; private set; }
        public List<CharacterData> AllCharacters { get; private set; } = new List<CharacterData>();

        // Events
        public event Action<CharacterData> OnCharacterCreated;
        public event Action<CharacterData> OnCharacterSelected;
        public event Action<CharacterData> OnCharacterDeleted;
        public event Action<int> OnLevelUp;
        public event Action<CharacterStats> OnStatsUpdated;

        #region Data Structures

        [Serializable]
        public class CharacterData
        {
            public string characterId;
            public string characterName;
            public Race race;
            public CharacterClass characterClass;
            public Background background;
            public int level;
            public long experience;
            public long experienceToNextLevel;
            public CharacterStats baseStats;
            public CharacterStats currentStats;
            public CharacterAppearance appearance;
            public EquipmentSlots equipment;
            public List<string> talents;
            public List<string> activeAbilities;
            public string currentShipId;
            public Vector3 lastPosition;
            public string lastZone;
            public DateTime createdAt;
            public int playTimeMinutes;
        }

        [Serializable]
        public class CharacterStats
        {
            public int strength;       // Weapon damage, cargo capacity
            public int agility;        // Ship handling, evasion
            public int endurance;      // Hull HP, shield regen
            public int intelligence;   // Energy weapons, hacking
            public int charisma;       // Trading prices, faction gains
            public int luck;           // Crit chance, loot quality

            // Derived Stats
            public float maxHealth;
            public float maxShield;
            public float maxEnergy;
            public float shieldRegen;
            public float energyRegen;
            public float critChance;
            public float critMultiplier;
            public float evasion;
            public float accuracy;
            public float weaponDamageBonus;
            public float tradingBonus;
            public float experienceBonus;

            public static CharacterStats operator +(CharacterStats a, CharacterStats b)
            {
                return new CharacterStats
                {
                    strength = a.strength + b.strength,
                    agility = a.agility + b.agility,
                    endurance = a.endurance + b.endurance,
                    intelligence = a.intelligence + b.intelligence,
                    charisma = a.charisma + b.charisma,
                    luck = a.luck + b.luck
                };
            }

            public void CalculateDerivedStats(int level)
            {
                maxHealth = 100 + (endurance * 10) + (level * 5);
                maxShield = 50 + (intelligence * 5) + (level * 3);
                maxEnergy = 100 + (intelligence * 3);
                shieldRegen = 1f + (endurance * 0.1f);
                energyRegen = 2f + (intelligence * 0.2f);
                critChance = 5f + (luck * 0.5f) + (agility * 0.2f);
                critMultiplier = 1.5f + (luck * 0.02f);
                evasion = agility * 0.5f;
                accuracy = 80f + (agility * 0.3f);
                weaponDamageBonus = strength * 2f;
                tradingBonus = charisma * 2f;
                experienceBonus = intelligence * 0.5f;
            }
        }

        [Serializable]
        public class CharacterAppearance
        {
            public int headStyle;
            public int faceStyle;
            public int hairStyle;
            public Color hairColor;
            public Color skinColor;
            public Color eyeColor;
            public int bodyType;
            public int voiceType;
            public string pilotSuitStyle;
        }

        [Serializable]
        public class EquipmentSlots
        {
            public string pilotHelmet;
            public string pilotSuit;
            public string implantSlot1;
            public string implantSlot2;
            public string implantSlot3;
            public string accessory1;
            public string accessory2;
        }

        public enum Race
        {
            Human,          // Balanced stats
            Cyborg,         // +Intelligence, +Endurance, -Charisma
            Android,        // +Intelligence, +Agility, -Luck
            Alien_Zorath,   // +Strength, +Endurance, -Intelligence
            Alien_Vexari,   // +Agility, +Luck, -Strength
            Hybrid          // Customizable
        }

        public enum CharacterClass
        {
            Freelancer,     // Balanced, jack of all trades
            Mercenary,      // Combat focused (+Strength, +Endurance)
            Trader,         // Economy focused (+Charisma, +Luck)
            Explorer,       // Discovery focused (+Agility, +Intelligence)
            Engineer,       // Technical focused (+Intelligence)
            Pirate,         // PvP focused (+Agility, +Strength)
            Diplomat,       // Social focused (+Charisma, +Intelligence)
            Scientist       // Research focused (+Intelligence, +Luck)
        }

        public enum Background
        {
            Military,       // +Combat skills start
            Merchant,       // +Trading skills, starting credits
            Criminal,       // +Stealth, bad faction starts
            Noble,          // +Charisma, more starting credits
            Orphan,         // +Luck, neutral factions
            Scientist,      // +Intelligence, research bonuses
            Pilot,          // +Agility, ship handling
            Miner           // +Endurance, mining bonuses
        }

        [Serializable]
        public class RaceDefinition
        {
            public Race race;
            public string displayName;
            public string description;
            public CharacterStats statModifiers;
            public List<string> racialAbilities;
            public string startingZone;
            public Sprite portrait;
        }

        [Serializable]
        public class ClassDefinition
        {
            public CharacterClass characterClass;
            public string displayName;
            public string description;
            public CharacterStats statModifiers;
            public List<string> startingAbilities;
            public List<string> classSkillTree;
            public string primaryAttribute;
            public string startingShip;
            public Sprite icon;
        }

        [Serializable]
        public class BackgroundDefinition
        {
            public Background background;
            public string displayName;
            public string description;
            public CharacterStats statModifiers;
            public int startingCredits;
            public List<string> startingItems;
            public Dictionary<string, int> factionModifiers;
        }

        #endregion

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeDefinitions();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeDefinitions()
        {
            // Initialize race definitions
            availableRaces = new List<RaceDefinition>
            {
                new RaceDefinition
                {
                    race = Race.Human,
                    displayName = "Human",
                    description = "Versatile and adaptable. Humans have spread across the galaxy and excel in all fields.",
                    statModifiers = new CharacterStats { strength = 0, agility = 0, endurance = 0, intelligence = 0, charisma = 1, luck = 1 },
                    racialAbilities = new List<string> { "Adaptability", "Diplomacy" },
                    startingZone = "Earth Station Alpha"
                },
                new RaceDefinition
                {
                    race = Race.Cyborg,
                    displayName = "Cyborg",
                    description = "Humans enhanced with cybernetic implants. Superior processing power but reduced social skills.",
                    statModifiers = new CharacterStats { strength = 0, agility = 1, endurance = 2, intelligence = 3, charisma = -2, luck = 0 },
                    racialAbilities = new List<string> { "System Override", "Enhanced Vision" },
                    startingZone = "Cybernetics Hub"
                },
                new RaceDefinition
                {
                    race = Race.Android,
                    displayName = "Android",
                    description = "Fully synthetic beings with advanced AI cores. Logical and precise, but lack intuition.",
                    statModifiers = new CharacterStats { strength = 1, agility = 2, endurance = 1, intelligence = 3, charisma = 0, luck = -3 },
                    racialAbilities = new List<string> { "Logic Core", "Self Repair" },
                    startingZone = "Factory Station"
                },
                new RaceDefinition
                {
                    race = Race.Alien_Zorath,
                    displayName = "Zorath",
                    description = "Powerful warrior race from the Zorath system. Strong and tough, but not technically minded.",
                    statModifiers = new CharacterStats { strength = 4, agility = 0, endurance = 3, intelligence = -3, charisma = 0, luck = 0 },
                    racialAbilities = new List<string> { "Berserker Rage", "Thick Hide" },
                    startingZone = "Zorath Homeworld"
                },
                new RaceDefinition
                {
                    race = Race.Alien_Vexari,
                    displayName = "Vexari",
                    description = "Mysterious telepathic aliens. Quick and fortunate, but physically weak.",
                    statModifiers = new CharacterStats { strength = -2, agility = 3, endurance = 0, intelligence = 1, charisma = 0, luck = 4 },
                    racialAbilities = new List<string> { "Precognition", "Mind Link" },
                    startingZone = "Vexari Station"
                }
            };

            // Initialize class definitions
            availableClasses = new List<ClassDefinition>
            {
                new ClassDefinition
                {
                    characterClass = CharacterClass.Freelancer,
                    displayName = "Freelancer",
                    description = "Jack of all trades. Balanced skills for combat, trading, and exploration.",
                    statModifiers = new CharacterStats { strength = 1, agility = 1, endurance = 1, intelligence = 1, charisma = 1, luck = 1 },
                    startingAbilities = new List<string> { "Basic Combat", "Basic Trading", "Basic Navigation" },
                    primaryAttribute = "None (Balanced)",
                    startingShip = "Starwing"
                },
                new ClassDefinition
                {
                    characterClass = CharacterClass.Mercenary,
                    displayName = "Mercenary",
                    description = "Combat specialist. Excels in ship-to-ship combat and ground operations.",
                    statModifiers = new CharacterStats { strength = 3, agility = 2, endurance = 3, intelligence = -1, charisma = -1, luck = 0 },
                    startingAbilities = new List<string> { "Heavy Weapons", "Combat Maneuvers", "Armor Piercing" },
                    primaryAttribute = "Strength",
                    startingShip = "Warbird"
                },
                new ClassDefinition
                {
                    characterClass = CharacterClass.Trader,
                    displayName = "Trader",
                    description = "Economic mastermind. Gets the best deals and has the largest cargo holds.",
                    statModifiers = new CharacterStats { strength = -1, agility = 0, endurance = 1, intelligence = 1, charisma = 4, luck = 2 },
                    startingAbilities = new List<string> { "Haggling", "Market Analysis", "Cargo Expansion" },
                    primaryAttribute = "Charisma",
                    startingShip = "Hauler"
                },
                new ClassDefinition
                {
                    characterClass = CharacterClass.Explorer,
                    displayName = "Explorer",
                    description = "Discoverer of the unknown. Finds hidden places and rare resources.",
                    statModifiers = new CharacterStats { strength = 0, agility = 3, endurance = 1, intelligence = 2, charisma = 0, luck = 1 },
                    startingAbilities = new List<string> { "Advanced Scanning", "Stealth Systems", "Jump Range Boost" },
                    primaryAttribute = "Agility",
                    startingShip = "Scout"
                },
                new ClassDefinition
                {
                    characterClass = CharacterClass.Engineer,
                    displayName = "Engineer",
                    description = "Technical genius. Repairs, upgrades, and creates advanced technology.",
                    statModifiers = new CharacterStats { strength = 0, agility = 1, endurance = 2, intelligence = 4, charisma = 0, luck = 0 },
                    startingAbilities = new List<string> { "Ship Repair", "System Overcharge", "Drone Deployment" },
                    primaryAttribute = "Intelligence",
                    startingShip = "Workshop"
                },
                new ClassDefinition
                {
                    characterClass = CharacterClass.Pirate,
                    displayName = "Pirate",
                    description = "Outlaw of the stars. Specializes in PvP combat and illegal activities.",
                    statModifiers = new CharacterStats { strength = 2, agility = 3, endurance = 0, intelligence = 0, charisma = -1, luck = 2 },
                    startingAbilities = new List<string> { "Ambush", "Cargo Theft", "Escape Artist" },
                    primaryAttribute = "Agility",
                    startingShip = "Raider"
                }
            };
        }

        #region Character Creation

        public CharacterData CreateCharacter(string name, Race race, CharacterClass charClass, Background background, CharacterAppearance appearance)
        {
            if (AllCharacters.Count >= maxCharactersPerAccount)
            {
                Debug.LogError("[Character] Maximum characters reached!");
                return null;
            }

            if (!IsValidCharacterName(name))
            {
                Debug.LogError("[Character] Invalid character name!");
                return null;
            }

            var character = new CharacterData
            {
                characterId = Guid.NewGuid().ToString(),
                characterName = name,
                race = race,
                characterClass = charClass,
                background = background,
                level = 1,
                experience = 0,
                experienceToNextLevel = CalculateExperienceRequired(2),
                baseStats = CalculateBaseStats(race, charClass, background),
                appearance = appearance,
                equipment = new EquipmentSlots(),
                talents = new List<string>(),
                activeAbilities = GetStartingAbilities(charClass),
                currentShipId = GetStartingShip(charClass),
                lastPosition = Vector3.zero,
                lastZone = GetStartingZone(race),
                createdAt = DateTime.UtcNow,
                playTimeMinutes = 0
            };

            character.currentStats = new CharacterStats();
            RecalculateStats(character);

            AllCharacters.Add(character);
            OnCharacterCreated?.Invoke(character);

            Debug.Log($"[Character] Created: {name} - Level {character.level} {race} {charClass}");
            return character;
        }

        public bool DeleteCharacter(string characterId)
        {
            var character = AllCharacters.Find(c => c.characterId == characterId);
            if (character != null)
            {
                AllCharacters.Remove(character);
                OnCharacterDeleted?.Invoke(character);
                return true;
            }
            return false;
        }

        public void SelectCharacter(string characterId)
        {
            CurrentCharacter = AllCharacters.Find(c => c.characterId == characterId);
            if (CurrentCharacter != null)
            {
                OnCharacterSelected?.Invoke(CurrentCharacter);
            }
        }

        private bool IsValidCharacterName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            if (name.Length < 3 || name.Length > 20) return false;
            // Would also check for profanity and duplicates
            return true;
        }

        private CharacterStats CalculateBaseStats(Race race, CharacterClass charClass, Background background)
        {
            // Base stats for all characters
            var stats = new CharacterStats
            {
                strength = 10,
                agility = 10,
                endurance = 10,
                intelligence = 10,
                charisma = 10,
                luck = 10
            };

            // Apply race modifiers
            var raceDef = availableRaces.Find(r => r.race == race);
            if (raceDef != null)
            {
                stats = stats + raceDef.statModifiers;
            }

            // Apply class modifiers
            var classDef = availableClasses.Find(c => c.characterClass == charClass);
            if (classDef != null)
            {
                stats = stats + classDef.statModifiers;
            }

            return stats;
        }

        private List<string> GetStartingAbilities(CharacterClass charClass)
        {
            var classDef = availableClasses.Find(c => c.characterClass == charClass);
            return classDef?.startingAbilities ?? new List<string>();
        }

        private string GetStartingShip(CharacterClass charClass)
        {
            var classDef = availableClasses.Find(c => c.characterClass == charClass);
            return classDef?.startingShip ?? "Starwing";
        }

        private string GetStartingZone(Race race)
        {
            var raceDef = availableRaces.Find(r => r.race == race);
            return raceDef?.startingZone ?? "Earth Station Alpha";
        }

        #endregion

        #region Progression

        public void AddExperience(long amount)
        {
            if (CurrentCharacter == null) return;

            // Apply experience bonus
            float bonus = 1f + (CurrentCharacter.currentStats.experienceBonus / 100f);
            long totalXP = (long)(amount * bonus);

            CurrentCharacter.experience += totalXP;

            // Check for level up
            while (CurrentCharacter.experience >= CurrentCharacter.experienceToNextLevel && CurrentCharacter.level < 100)
            {
                LevelUp();
            }
        }

        private void LevelUp()
        {
            if (CurrentCharacter == null) return;

            CurrentCharacter.level++;
            CurrentCharacter.experience -= CurrentCharacter.experienceToNextLevel;
            CurrentCharacter.experienceToNextLevel = CalculateExperienceRequired(CurrentCharacter.level + 1);

            // Grant stat points based on class
            GrantLevelUpStats();

            // Recalculate derived stats
            RecalculateStats(CurrentCharacter);

            OnLevelUp?.Invoke(CurrentCharacter.level);
            Debug.Log($"[Character] Level Up! Now level {CurrentCharacter.level}");
        }

        private void GrantLevelUpStats()
        {
            // Each level gives stat points based on class primary attribute
            CurrentCharacter.baseStats.strength += 1;
            CurrentCharacter.baseStats.agility += 1;
            CurrentCharacter.baseStats.endurance += 1;
            CurrentCharacter.baseStats.intelligence += 1;
            CurrentCharacter.baseStats.charisma += 1;
            CurrentCharacter.baseStats.luck += 1;
        }

        public long CalculateExperienceRequired(int level)
        {
            // WoW-like experience curve
            return (long)(100 * Mathf.Pow(level, 2.5f));
        }

        public void RecalculateStats(CharacterData character)
        {
            // Start with base stats
            character.currentStats = new CharacterStats
            {
                strength = character.baseStats.strength,
                agility = character.baseStats.agility,
                endurance = character.baseStats.endurance,
                intelligence = character.baseStats.intelligence,
                charisma = character.baseStats.charisma,
                luck = character.baseStats.luck
            };

            // Add equipment bonuses (would iterate equipment)

            // Calculate derived stats
            character.currentStats.CalculateDerivedStats(character.level);

            OnStatsUpdated?.Invoke(character.currentStats);
        }

        #endregion

        #region Stat Queries

        public float GetStatValue(string statName)
        {
            if (CurrentCharacter == null) return 0;

            return statName.ToLower() switch
            {
                "strength" => CurrentCharacter.currentStats.strength,
                "agility" => CurrentCharacter.currentStats.agility,
                "endurance" => CurrentCharacter.currentStats.endurance,
                "intelligence" => CurrentCharacter.currentStats.intelligence,
                "charisma" => CurrentCharacter.currentStats.charisma,
                "luck" => CurrentCharacter.currentStats.luck,
                "health" => CurrentCharacter.currentStats.maxHealth,
                "shield" => CurrentCharacter.currentStats.maxShield,
                "energy" => CurrentCharacter.currentStats.maxEnergy,
                "critchance" => CurrentCharacter.currentStats.critChance,
                "evasion" => CurrentCharacter.currentStats.evasion,
                _ => 0
            };
        }

        public RaceDefinition GetRaceDefinition(Race race)
        {
            return availableRaces.Find(r => r.race == race);
        }

        public ClassDefinition GetClassDefinition(CharacterClass charClass)
        {
            return availableClasses.Find(c => c.characterClass == charClass);
        }

        #endregion
    }
}
