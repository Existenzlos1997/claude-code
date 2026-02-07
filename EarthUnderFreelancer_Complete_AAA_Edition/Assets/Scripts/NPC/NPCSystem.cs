using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EarthUnderFreelancer.NPC
{
    /// <summary>
    /// Complete NPC system for a living, breathing world
    /// NPCs have schedules, relationships, dialogue, and dynamic behaviors
    /// </summary>
    
    [Serializable]
    public enum NPCType
    {
        Civilian,
        Merchant,
        Pilot,
        MilitaryOfficer,
        Mechanic,
        IntelligenceAgent,
        Smuggler,
        FactionLeader,
        QuestGiver,
        TrainingInstructor,
        Bartender,
        RadioOperator,
        ControlTower,
        GroundCrew
    }
    
    [Serializable]
    public enum NPCState
    {
        Idle,
        Walking,
        Working,
        Talking,
        Sleeping,
        Eating,
        Flying,
        Combat,
        Trading,
        Fleeing
    }
    
    [Serializable]
    public enum NPCMood
    {
        Happy,
        Neutral,
        Annoyed,
        Angry,
        Scared,
        Sad,
        Excited,
        Suspicious
    }
    
    [Serializable]
    public class NPCScheduleEntry
    {
        public float startHour;
        public float endHour;
        public string locationId;
        public NPCState activity;
        public string description;
        
        public NPCScheduleEntry(float start, float end, string loc, NPCState state, string desc = "")
        {
            startHour = start;
            endHour = end;
            locationId = loc;
            activity = state;
            description = desc;
        }
    }
    
    [Serializable]
    public class NPCRelationship
    {
        public string targetNPCId;
        public float affinity; // -100 to 100
        public string relationshipType; // friend, rival, family, colleague
        public List<string> sharedMemories;
        
        public NPCRelationship(string targetId, float aff, string type)
        {
            targetNPCId = targetId;
            affinity = aff;
            relationshipType = type;
            sharedMemories = new List<string>();
        }
    }
    
    [Serializable]
    public class DialogueLine
    {
        public string lineId;
        public string text;
        public string voiceClipPath;
        public float duration;
        public List<string> requiredConditions;
        public List<DialogueOption> options;
        
        public DialogueLine(string id, string txt)
        {
            lineId = id;
            text = txt;
            voiceClipPath = "";
            duration = 3f;
            requiredConditions = new List<string>();
            options = new List<DialogueOption>();
        }
    }
    
    [Serializable]
    public class DialogueOption
    {
        public string optionText;
        public string nextLineId;
        public string requiredReputation;
        public int reputationChange;
        public string triggerQuest;
        public string giveItem;
        
        public DialogueOption(string text, string nextLine)
        {
            optionText = text;
            nextLineId = nextLine;
            requiredReputation = "";
            reputationChange = 0;
            triggerQuest = "";
            giveItem = "";
        }
    }
    
    [Serializable]
    public class NPCMemory
    {
        public string memoryId;
        public string description;
        public DateTime timestamp;
        public string involvedPlayerId;
        public float impactOnRelationship;
        
        public NPCMemory(string id, string desc, string playerId, float impact)
        {
            memoryId = id;
            description = desc;
            timestamp = DateTime.Now;
            involvedPlayerId = playerId;
            impactOnRelationship = impact;
        }
    }
    
    [Serializable]
    public class NPCData
    {
        public string npcId;
        public string npcName;
        public string title;
        public NPCType type;
        public string factionId;
        public NPCState currentState;
        public NPCMood currentMood;
        
        // Location
        public Vector3 currentPosition;
        public Vector3 homePosition;
        public string currentLocationId;
        
        // Stats
        public int level;
        public float health;
        public float maxHealth;
        public int combatSkill;
        public int pilotingSkill;
        public int tradingSkill;
        
        // Schedule
        public List<NPCScheduleEntry> schedule;
        
        // Relationships
        public Dictionary<string, float> playerRelationships; // playerId -> reputation
        public List<NPCRelationship> npcRelationships;
        
        // Dialogue
        public List<DialogueLine> dialogueTree;
        public string currentDialogueId;
        
        // Memory
        public List<NPCMemory> memories;
        
        // Inventory (for merchants)
        public List<string> inventory;
        public int currency;
        
        // Appearance
        public string appearancePreset;
        public string uniformType;
        public bool isEssential;
        
        public NPCData(string id, string name, NPCType npcType)
        {
            npcId = id;
            npcName = name;
            title = "";
            type = npcType;
            factionId = "";
            currentState = NPCState.Idle;
            currentMood = NPCMood.Neutral;
            
            currentPosition = Vector3.zero;
            homePosition = Vector3.zero;
            currentLocationId = "";
            
            level = 1;
            health = 100f;
            maxHealth = 100f;
            combatSkill = 50;
            pilotingSkill = 50;
            tradingSkill = 50;
            
            schedule = new List<NPCScheduleEntry>();
            playerRelationships = new Dictionary<string, float>();
            npcRelationships = new List<NPCRelationship>();
            dialogueTree = new List<DialogueLine>();
            currentDialogueId = "";
            memories = new List<NPCMemory>();
            inventory = new List<string>();
            currency = 1000;
            
            appearancePreset = "default";
            uniformType = "";
            isEssential = false;
        }
    }
    
    [Serializable]
    public class NPCSpawnPoint
    {
        public string spawnId;
        public Vector3 position;
        public Quaternion rotation;
        public NPCType allowedType;
        public string factionId;
        public string locationId;
        public bool isOccupied;
        public string occupyingNPCId;
        
        public NPCSpawnPoint(Vector3 pos, NPCType type, string faction, string location)
        {
            spawnId = Guid.NewGuid().ToString();
            position = pos;
            rotation = Quaternion.identity;
            allowedType = type;
            factionId = faction;
            locationId = location;
            isOccupied = false;
            occupyingNPCId = "";
        }
    }
    
    public class NPCSystem : MonoBehaviour
    {
        public static NPCSystem Instance { get; private set; }
        
        [Header("NPC Settings")]
        [SerializeField] private int maxActiveNPCs = 100;
        [SerializeField] private float npcUpdateInterval = 1f;
        [SerializeField] private float npcSpawnDistance = 500f;
        [SerializeField] private float npcDespawnDistance = 1000f;
        
        [Header("NPC Prefabs")]
        [SerializeField] private GameObject civilianPrefab;
        [SerializeField] private GameObject pilotPrefab;
        [SerializeField] private GameObject merchantPrefab;
        [SerializeField] private GameObject militaryPrefab;
        
        private Dictionary<string, NPCData> allNPCs = new Dictionary<string, NPCData>();
        private Dictionary<string, GameObject> activeNPCObjects = new Dictionary<string, GameObject>();
        private List<NPCSpawnPoint> spawnPoints = new List<NPCSpawnPoint>();
        private float updateTimer = 0f;
        
        public event Action<NPCData> OnNPCSpawned;
        public event Action<NPCData> OnNPCDespawned;
        public event Action<NPCData, string> OnNPCStateChanged;
        public event Action<string, string, DialogueLine> OnDialogueStarted;
        public event Action<string, string> OnDialogueEnded;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeNPCs();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void InitializeNPCs()
        {
            // Create essential NPCs for each major location
            CreateEssentialNPCs();
            
            // Create spawn points
            CreateSpawnPoints();
            
            // Populate world with NPCs
            PopulateWorld();
        }
        
        private void CreateEssentialNPCs()
        {
            // Faction leaders
            CreateFactionLeader("faction_leader_usa", "General Mitchell", "usa");
            CreateFactionLeader("faction_leader_ger", "Generalfeldmarschall Weber", "germany");
            CreateFactionLeader("faction_leader_uk", "Air Marshal Harrison", "uk");
            CreateFactionLeader("faction_leader_ussr", "Marshall Petrov", "ussr");
            CreateFactionLeader("faction_leader_jpn", "Admiral Yamamoto", "japan");
            
            // Quest givers
            CreateQuestGiver("quest_main_01", "Colonel Rhodes", "usa", new Vector3(0, 0, 100));
            CreateQuestGiver("quest_smuggler", "Black Market Pete", "pirates", new Vector3(-500, 0, 200));
            CreateQuestGiver("quest_intel", "Agent Shadow", "neutral", new Vector3(200, 0, -100));
            
            // Merchants
            CreateMerchant("merchant_weapons_01", "Viktor Arms Dealer", new Vector3(50, 0, 50));
            CreateMerchant("merchant_parts_01", "Martha's Aircraft Parts", new Vector3(-50, 0, 50));
            CreateMerchant("merchant_fuel_01", "Big Joe's Fuel Station", new Vector3(0, 0, -50));
            
            // Trainers
            CreateTrainer("trainer_combat", "Ace Williams", "Combat", new Vector3(100, 0, 0));
            CreateTrainer("trainer_piloting", "Captain Sky", "Piloting", new Vector3(-100, 0, 0));
            CreateTrainer("trainer_trading", "Trader Tom", "Trading", new Vector3(0, 0, 100));
            
            // Service NPCs
            CreateServiceNPC("mechanic_main", "Chief Mechanic Rodriguez", NPCType.Mechanic, new Vector3(30, 0, 30));
            CreateServiceNPC("tower_main", "Tower Control Officer", NPCType.ControlTower, new Vector3(0, 10, 0));
            CreateServiceNPC("radio_main", "Radio Operator Sarah", NPCType.RadioOperator, new Vector3(20, 0, -20));
        }
        
        private void CreateFactionLeader(string id, string name, string factionId)
        {
            var npc = new NPCData(id, name, NPCType.FactionLeader);
            npc.title = "Faction Leader";
            npc.factionId = factionId;
            npc.isEssential = true;
            npc.level = 50;
            npc.combatSkill = 95;
            npc.pilotingSkill = 90;
            
            // Create dialogue
            var greeting = new DialogueLine("greeting", $"Welcome, pilot. I am {name}, leader of our forces.");
            greeting.options.Add(new DialogueOption("Tell me about our faction", "faction_info"));
            greeting.options.Add(new DialogueOption("I want to serve", "join_faction"));
            greeting.options.Add(new DialogueOption("Any missions?", "missions"));
            greeting.options.Add(new DialogueOption("Goodbye", "farewell"));
            npc.dialogueTree.Add(greeting);
            
            var factionInfo = new DialogueLine("faction_info", $"We fight for honor and freedom. Our pilots are the best in the world.");
            factionInfo.options.Add(new DialogueOption("Back", "greeting"));
            npc.dialogueTree.Add(factionInfo);
            
            var join = new DialogueLine("join_faction", "Prove yourself in combat, and you shall be rewarded.");
            join.options.Add(new DialogueOption("I will", "greeting"));
            npc.dialogueTree.Add(join);
            
            var farewell = new DialogueLine("farewell", "Fly safe, pilot. Glory awaits.");
            npc.dialogueTree.Add(farewell);
            
            // Schedule
            npc.schedule.Add(new NPCScheduleEntry(6f, 8f, "office", NPCState.Working, "Morning briefing"));
            npc.schedule.Add(new NPCScheduleEntry(8f, 12f, "command_center", NPCState.Working, "Command duties"));
            npc.schedule.Add(new NPCScheduleEntry(12f, 13f, "mess_hall", NPCState.Eating, "Lunch"));
            npc.schedule.Add(new NPCScheduleEntry(13f, 18f, "command_center", NPCState.Working, "Afternoon duties"));
            npc.schedule.Add(new NPCScheduleEntry(18f, 20f, "quarters", NPCState.Idle, "Personal time"));
            npc.schedule.Add(new NPCScheduleEntry(20f, 6f, "quarters", NPCState.Sleeping, "Rest"));
            
            allNPCs[id] = npc;
        }
        
        private void CreateQuestGiver(string id, string name, string factionId, Vector3 position)
        {
            var npc = new NPCData(id, name, NPCType.QuestGiver);
            npc.factionId = factionId;
            npc.isEssential = true;
            npc.currentPosition = position;
            npc.homePosition = position;
            npc.level = 30;
            
            var greeting = new DialogueLine("greeting", $"Looking for work, pilot? I might have something for you.");
            greeting.options.Add(new DialogueOption("Show me what you've got", "show_quests"));
            greeting.options.Add(new DialogueOption("Not right now", "farewell"));
            npc.dialogueTree.Add(greeting);
            
            var showQuests = new DialogueLine("show_quests", "Here are the jobs available. Choose wisely.");
            npc.dialogueTree.Add(showQuests);
            
            allNPCs[id] = npc;
        }
        
        private void CreateMerchant(string id, string name, Vector3 position)
        {
            var npc = new NPCData(id, name, NPCType.Merchant);
            npc.currentPosition = position;
            npc.homePosition = position;
            npc.tradingSkill = 80;
            npc.currency = 50000;
            
            // Stock inventory
            npc.inventory.AddRange(new[] { 
                "ammo_50cal", "ammo_20mm", "ammo_37mm",
                "fuel_standard", "fuel_premium",
                "repair_kit_small", "repair_kit_large",
                "engine_upgrade_1", "armor_plate_1"
            });
            
            var greeting = new DialogueLine("greeting", $"Welcome to {name}! What can I get you?");
            greeting.options.Add(new DialogueOption("Let's trade", "trade"));
            greeting.options.Add(new DialogueOption("What's new?", "news"));
            greeting.options.Add(new DialogueOption("Leaving", "farewell"));
            npc.dialogueTree.Add(greeting);
            
            npc.schedule.Add(new NPCScheduleEntry(8f, 20f, "shop", NPCState.Working, "Shop hours"));
            npc.schedule.Add(new NPCScheduleEntry(20f, 8f, "home", NPCState.Sleeping, "Closed"));
            
            allNPCs[id] = npc;
        }
        
        private void CreateTrainer(string id, string name, string specialty, Vector3 position)
        {
            var npc = new NPCData(id, name, NPCType.TrainingInstructor);
            npc.title = $"{specialty} Instructor";
            npc.currentPosition = position;
            npc.homePosition = position;
            npc.isEssential = true;
            npc.level = 40;
            
            switch (specialty)
            {
                case "Combat":
                    npc.combatSkill = 95;
                    break;
                case "Piloting":
                    npc.pilotingSkill = 95;
                    break;
                case "Trading":
                    npc.tradingSkill = 95;
                    break;
            }
            
            var greeting = new DialogueLine("greeting", $"Ready to learn {specialty.ToLower()}, recruit?");
            greeting.options.Add(new DialogueOption("Train me", "train"));
            greeting.options.Add(new DialogueOption("Show skills", "skills"));
            greeting.options.Add(new DialogueOption("Later", "farewell"));
            npc.dialogueTree.Add(greeting);
            
            allNPCs[id] = npc;
        }
        
        private void CreateServiceNPC(string id, string name, NPCType type, Vector3 position)
        {
            var npc = new NPCData(id, name, type);
            npc.currentPosition = position;
            npc.homePosition = position;
            npc.level = 20;
            
            string service = type switch
            {
                NPCType.Mechanic => "repair your aircraft",
                NPCType.ControlTower => "coordinate air traffic",
                NPCType.RadioOperator => "provide communications",
                _ => "help you"
            };
            
            var greeting = new DialogueLine("greeting", $"I'm here to {service}.");
            npc.dialogueTree.Add(greeting);
            
            npc.schedule.Add(new NPCScheduleEntry(0f, 24f, "station", NPCState.Working, "On duty 24/7"));
            
            allNPCs[id] = npc;
        }
        
        private void CreateSpawnPoints()
        {
            // Airbase spawn points
            for (int i = 0; i < 20; i++)
            {
                Vector3 pos = new Vector3(
                    UnityEngine.Random.Range(-200f, 200f),
                    0,
                    UnityEngine.Random.Range(-200f, 200f)
                );
                spawnPoints.Add(new NPCSpawnPoint(pos, NPCType.Pilot, "mixed", "airbase_main"));
            }
            
            // Town spawn points
            for (int i = 0; i < 30; i++)
            {
                Vector3 pos = new Vector3(
                    UnityEngine.Random.Range(-500f, 500f),
                    0,
                    UnityEngine.Random.Range(300f, 800f)
                );
                spawnPoints.Add(new NPCSpawnPoint(pos, NPCType.Civilian, "neutral", "town_main"));
            }
            
            // Military base spawn points
            for (int i = 0; i < 15; i++)
            {
                Vector3 pos = new Vector3(
                    UnityEngine.Random.Range(-300f, 300f),
                    0,
                    UnityEngine.Random.Range(-500f, -200f)
                );
                spawnPoints.Add(new NPCSpawnPoint(pos, NPCType.MilitaryOfficer, "usa", "military_base"));
            }
        }
        
        private void PopulateWorld()
        {
            // Spawn random NPCs at spawn points
            foreach (var spawnPoint in spawnPoints)
            {
                if (UnityEngine.Random.value > 0.5f) continue; // 50% chance to spawn
                
                string npcId = $"random_npc_{Guid.NewGuid().ToString().Substring(0, 8)}";
                string[] names = GetRandomNames(spawnPoint.allowedType);
                string name = names[UnityEngine.Random.Range(0, names.Length)];
                
                var npc = new NPCData(npcId, name, spawnPoint.allowedType);
                npc.factionId = spawnPoint.factionId;
                npc.currentPosition = spawnPoint.position;
                npc.homePosition = spawnPoint.position;
                npc.currentLocationId = spawnPoint.locationId;
                
                // Random stats
                npc.level = UnityEngine.Random.Range(1, 30);
                npc.combatSkill = UnityEngine.Random.Range(20, 80);
                npc.pilotingSkill = UnityEngine.Random.Range(20, 80);
                npc.tradingSkill = UnityEngine.Random.Range(20, 80);
                
                // Random schedule
                GenerateRandomSchedule(npc);
                
                // Basic dialogue
                var greeting = new DialogueLine("greeting", GetRandomGreeting(spawnPoint.allowedType));
                npc.dialogueTree.Add(greeting);
                
                allNPCs[npcId] = npc;
                spawnPoint.isOccupied = true;
                spawnPoint.occupyingNPCId = npcId;
            }
        }
        
        private string[] GetRandomNames(NPCType type)
        {
            switch (type)
            {
                case NPCType.Pilot:
                    return new[] { "Red Baron", "Sky Wolf", "Eagle Eye", "Thunder", "Ghost", "Maverick", "Ice", "Viper" };
                case NPCType.Civilian:
                    return new[] { "John", "Mary", "Hans", "Yuki", "Ivan", "Pierre", "Sofia", "Carlos" };
                case NPCType.MilitaryOfficer:
                    return new[] { "Captain Smith", "Lieutenant Brown", "Sergeant Davis", "Major Wilson" };
                case NPCType.Mechanic:
                    return new[] { "Wrench", "Grease Monkey", "Gear Head", "Spark Plug" };
                default:
                    return new[] { "NPC" };
            }
        }
        
        private string GetRandomGreeting(NPCType type)
        {
            switch (type)
            {
                case NPCType.Pilot:
                    return "Clear skies out there. Good day for flying.";
                case NPCType.Civilian:
                    return "Good day, traveler.";
                case NPCType.MilitaryOfficer:
                    return "Stay alert, soldier.";
                case NPCType.Mechanic:
                    return "Need a tune-up?";
                default:
                    return "Hello there.";
            }
        }
        
        private void GenerateRandomSchedule(NPCData npc)
        {
            switch (npc.type)
            {
                case NPCType.Civilian:
                    npc.schedule.Add(new NPCScheduleEntry(6f, 8f, "home", NPCState.Idle, "Morning"));
                    npc.schedule.Add(new NPCScheduleEntry(8f, 17f, "work", NPCState.Working, "Work"));
                    npc.schedule.Add(new NPCScheduleEntry(17f, 22f, "town", NPCState.Walking, "Evening"));
                    npc.schedule.Add(new NPCScheduleEntry(22f, 6f, "home", NPCState.Sleeping, "Sleep"));
                    break;
                case NPCType.Pilot:
                    npc.schedule.Add(new NPCScheduleEntry(5f, 7f, "barracks", NPCState.Idle, "Wake up"));
                    npc.schedule.Add(new NPCScheduleEntry(7f, 8f, "mess_hall", NPCState.Eating, "Breakfast"));
                    npc.schedule.Add(new NPCScheduleEntry(8f, 12f, "hangar", NPCState.Working, "Duty"));
                    npc.schedule.Add(new NPCScheduleEntry(12f, 13f, "mess_hall", NPCState.Eating, "Lunch"));
                    npc.schedule.Add(new NPCScheduleEntry(13f, 17f, "airfield", NPCState.Working, "Training"));
                    npc.schedule.Add(new NPCScheduleEntry(17f, 22f, "bar", NPCState.Idle, "Off-duty"));
                    npc.schedule.Add(new NPCScheduleEntry(22f, 5f, "barracks", NPCState.Sleeping, "Sleep"));
                    break;
                case NPCType.MilitaryOfficer:
                    npc.schedule.Add(new NPCScheduleEntry(0f, 24f, "base", NPCState.Working, "On patrol"));
                    break;
            }
        }
        
        private void Update()
        {
            updateTimer += Time.deltaTime;
            
            if (updateTimer >= npcUpdateInterval)
            {
                updateTimer = 0f;
                UpdateAllNPCs();
                ManageNPCSpawning();
            }
        }
        
        private void UpdateAllNPCs()
        {
            float currentHour = GetCurrentHour();
            
            foreach (var kvp in allNPCs)
            {
                var npc = kvp.Value;
                UpdateNPCSchedule(npc, currentHour);
                UpdateNPCBehavior(npc);
                UpdateNPCMood(npc);
            }
        }
        
        private float GetCurrentHour()
        {
            // Get from WeatherSystem if available
            var weatherSystem = FindFirstObjectByType<Weather.WeatherSystem>();
            if (weatherSystem != null)
            {
                return weatherSystem.GetCurrentTime();
            }
            return 12f; // Default noon
        }
        
        private void UpdateNPCSchedule(NPCData npc, float currentHour)
        {
            foreach (var entry in npc.schedule)
            {
                bool inTimeRange;
                if (entry.startHour < entry.endHour)
                {
                    inTimeRange = currentHour >= entry.startHour && currentHour < entry.endHour;
                }
                else
                {
                    inTimeRange = currentHour >= entry.startHour || currentHour < entry.endHour;
                }
                
                if (inTimeRange && npc.currentState != entry.activity)
                {
                    string previousState = npc.currentState.ToString();
                    npc.currentState = entry.activity;
                    npc.currentLocationId = entry.locationId;
                    OnNPCStateChanged?.Invoke(npc, previousState);
                }
            }
        }
        
        private void UpdateNPCBehavior(NPCData npc)
        {
            // AI behavior based on state
            switch (npc.currentState)
            {
                case NPCState.Walking:
                    // Move towards destination
                    // Implemented via NavMesh in actual game
                    break;
                case NPCState.Combat:
                    // Combat AI
                    break;
                case NPCState.Fleeing:
                    // Run away from danger
                    break;
            }
        }
        
        private void UpdateNPCMood(NPCData npc)
        {
            // Mood based on various factors
            if (npc.health < npc.maxHealth * 0.3f)
            {
                npc.currentMood = NPCMood.Scared;
            }
            else if (npc.currentState == NPCState.Combat)
            {
                npc.currentMood = NPCMood.Angry;
            }
            else if (npc.currentState == NPCState.Eating || npc.currentState == NPCState.Idle)
            {
                npc.currentMood = NPCMood.Happy;
            }
            else
            {
                npc.currentMood = NPCMood.Neutral;
            }
        }
        
        private void ManageNPCSpawning()
        {
            Transform player = Camera.main?.transform;
            if (player == null) return;
            
            Vector3 playerPos = player.position;
            
            // Despawn far NPCs
            List<string> toDespawn = new List<string>();
            foreach (var kvp in activeNPCObjects)
            {
                if (kvp.Value == null) continue;
                float dist = Vector3.Distance(kvp.Value.transform.position, playerPos);
                if (dist > npcDespawnDistance && !allNPCs[kvp.Key].isEssential)
                {
                    toDespawn.Add(kvp.Key);
                }
            }
            
            foreach (var id in toDespawn)
            {
                DespawnNPC(id);
            }
            
            // Spawn nearby NPCs
            if (activeNPCObjects.Count < maxActiveNPCs)
            {
                foreach (var kvp in allNPCs)
                {
                    if (activeNPCObjects.ContainsKey(kvp.Key)) continue;
                    
                    float dist = Vector3.Distance(kvp.Value.currentPosition, playerPos);
                    if (dist < npcSpawnDistance)
                    {
                        SpawnNPC(kvp.Key);
                        if (activeNPCObjects.Count >= maxActiveNPCs) break;
                    }
                }
            }
        }
        
        private void SpawnNPC(string npcId)
        {
            if (!allNPCs.ContainsKey(npcId)) return;
            if (activeNPCObjects.ContainsKey(npcId)) return;
            
            var npcData = allNPCs[npcId];
            GameObject prefab = GetPrefabForType(npcData.type);
            
            if (prefab == null)
            {
                // Create simple placeholder
                prefab = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            }
            
            GameObject npcObject = Instantiate(prefab, npcData.currentPosition, Quaternion.identity);
            npcObject.name = $"NPC_{npcData.npcName}";
            
            activeNPCObjects[npcId] = npcObject;
            OnNPCSpawned?.Invoke(npcData);
        }
        
        private void DespawnNPC(string npcId)
        {
            if (!activeNPCObjects.ContainsKey(npcId)) return;
            
            var npcObject = activeNPCObjects[npcId];
            if (npcObject != null)
            {
                Destroy(npcObject);
            }
            
            activeNPCObjects.Remove(npcId);
            
            if (allNPCs.ContainsKey(npcId))
            {
                OnNPCDespawned?.Invoke(allNPCs[npcId]);
            }
        }
        
        private GameObject GetPrefabForType(NPCType type)
        {
            switch (type)
            {
                case NPCType.Civilian: return civilianPrefab;
                case NPCType.Pilot: return pilotPrefab;
                case NPCType.Merchant: return merchantPrefab;
                case NPCType.MilitaryOfficer: return militaryPrefab;
                default: return civilianPrefab;
            }
        }
        
        // Public API
        
        public void StartDialogue(string playerId, string npcId)
        {
            if (!allNPCs.ContainsKey(npcId)) return;
            
            var npc = allNPCs[npcId];
            npc.currentDialogueId = "greeting";
            
            var dialogueLine = npc.dialogueTree.Find(d => d.lineId == "greeting");
            if (dialogueLine != null)
            {
                OnDialogueStarted?.Invoke(playerId, npcId, dialogueLine);
            }
        }
        
        public DialogueLine SelectDialogueOption(string playerId, string npcId, int optionIndex)
        {
            if (!allNPCs.ContainsKey(npcId)) return null;
            
            var npc = allNPCs[npcId];
            var currentLine = npc.dialogueTree.Find(d => d.lineId == npc.currentDialogueId);
            
            if (currentLine == null || optionIndex >= currentLine.options.Count) return null;
            
            var option = currentLine.options[optionIndex];
            
            // Apply effects
            if (option.reputationChange != 0)
            {
                ModifyRelationship(npcId, playerId, option.reputationChange);
            }
            
            // Move to next dialogue
            if (string.IsNullOrEmpty(option.nextLineId) || option.nextLineId == "farewell")
            {
                OnDialogueEnded?.Invoke(playerId, npcId);
                return null;
            }
            
            npc.currentDialogueId = option.nextLineId;
            return npc.dialogueTree.Find(d => d.lineId == option.nextLineId);
        }
        
        public void ModifyRelationship(string npcId, string playerId, float change)
        {
            if (!allNPCs.ContainsKey(npcId)) return;
            
            var npc = allNPCs[npcId];
            if (!npc.playerRelationships.ContainsKey(playerId))
            {
                npc.playerRelationships[playerId] = 0f;
            }
            
            npc.playerRelationships[playerId] = Mathf.Clamp(
                npc.playerRelationships[playerId] + change,
                -100f, 100f
            );
            
            // Create memory
            string desc = change > 0 ? "Positive interaction" : "Negative interaction";
            npc.memories.Add(new NPCMemory(Guid.NewGuid().ToString(), desc, playerId, change));
        }
        
        public float GetRelationship(string npcId, string playerId)
        {
            if (!allNPCs.ContainsKey(npcId)) return 0f;
            
            var npc = allNPCs[npcId];
            return npc.playerRelationships.ContainsKey(playerId) ? npc.playerRelationships[playerId] : 0f;
        }
        
        public NPCData GetNPC(string npcId)
        {
            return allNPCs.ContainsKey(npcId) ? allNPCs[npcId] : null;
        }
        
        public List<NPCData> GetNPCsInRadius(Vector3 position, float radius)
        {
            return allNPCs.Values.Where(npc => 
                Vector3.Distance(npc.currentPosition, position) <= radius
            ).ToList();
        }
        
        public List<NPCData> GetNPCsByType(NPCType type)
        {
            return allNPCs.Values.Where(npc => npc.type == type).ToList();
        }
        
        public List<NPCData> GetNPCsByFaction(string factionId)
        {
            return allNPCs.Values.Where(npc => npc.factionId == factionId).ToList();
        }
        
        public void DamageNPC(string npcId, float damage, string attackerId)
        {
            if (!allNPCs.ContainsKey(npcId)) return;
            
            var npc = allNPCs[npcId];
            npc.health -= damage;
            npc.currentMood = NPCMood.Angry;
            
            // Create hostile memory
            npc.memories.Add(new NPCMemory(
                Guid.NewGuid().ToString(),
                "Was attacked",
                attackerId,
                -50f
            ));
            ModifyRelationship(npcId, attackerId, -50f);
            
            if (npc.health <= 0 && !npc.isEssential)
            {
                KillNPC(npcId);
            }
            else if (npc.health < npc.maxHealth * 0.3f)
            {
                npc.currentState = NPCState.Fleeing;
            }
            else
            {
                npc.currentState = NPCState.Combat;
            }
        }
        
        public void KillNPC(string npcId)
        {
            if (!allNPCs.ContainsKey(npcId)) return;
            
            var npc = allNPCs[npcId];
            if (npc.isEssential)
            {
                // Essential NPCs become incapacitated instead
                npc.health = 1f;
                npc.currentState = NPCState.Idle;
                return;
            }
            
            DespawnNPC(npcId);
            allNPCs.Remove(npcId);
            
            // Free spawn point
            var spawnPoint = spawnPoints.Find(sp => sp.occupyingNPCId == npcId);
            if (spawnPoint != null)
            {
                spawnPoint.isOccupied = false;
                spawnPoint.occupyingNPCId = "";
            }
        }
        
        public void HealNPC(string npcId, float amount)
        {
            if (!allNPCs.ContainsKey(npcId)) return;
            
            var npc = allNPCs[npcId];
            npc.health = Mathf.Min(npc.health + amount, npc.maxHealth);
        }
        
        public bool TradeWithNPC(string npcId, string playerId, string itemId, bool buying, int price)
        {
            if (!allNPCs.ContainsKey(npcId)) return false;
            
            var npc = allNPCs[npcId];
            if (npc.type != NPCType.Merchant) return false;
            
            if (buying)
            {
                if (!npc.inventory.Contains(itemId)) return false;
                npc.inventory.Remove(itemId);
                npc.currency += price;
            }
            else
            {
                npc.inventory.Add(itemId);
                npc.currency -= price;
            }
            
            // Positive interaction
            ModifyRelationship(npcId, playerId, 1f);
            return true;
        }
    }
}
