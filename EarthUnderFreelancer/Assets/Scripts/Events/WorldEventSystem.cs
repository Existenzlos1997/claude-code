using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EarthUnderFreelancer.Events
{
    /// <summary>
    /// Dynamic world event system
    /// Creates large-scale events that all players can participate in
    /// </summary>
    
    [Serializable]
    public enum WorldEventType
    {
        // Combat Events
        AirRaid,
        TerritoryInvasion,
        EnemyConvoy,
        BomberIntercept,
        AceShowdown,
        AirSupremacy,
        
        // Economic Events
        TradeFleet,
        ResourceRush,
        MarketCrash,
        SmugglerSeason,
        ArmsDeal,
        
        // Environmental Events
        StormFront,
        VolcanicActivity,
        FogOfWar,
        ArcticBlast,
        
        // Special Events
        HolidayEvent,
        AnniversaryEvent,
        CompetitionWeek,
        DoubleXP,
        DoubleCredits,
        
        // Faction Events
        CivilWar,
        Alliance,
        Rebellion,
        PeaceTreaty
    }
    
    [Serializable]
    public enum EventPhase
    {
        Announced,
        Preparation,
        Active,
        Climax,
        Conclusion,
        Completed
    }
    
    [Serializable]
    public class EventObjective
    {
        public string objectiveId;
        public string description;
        public float targetValue;
        public float currentValue;
        public bool isCompleted;
        public bool isFailed;
        public float rewardMultiplier;
        
        public EventObjective(string id, string desc, float target)
        {
            objectiveId = id;
            description = desc;
            targetValue = target;
            currentValue = 0f;
            isCompleted = false;
            isFailed = false;
            rewardMultiplier = 1f;
        }
        
        public float GetProgress()
        {
            return Mathf.Clamp01(currentValue / targetValue);
        }
    }
    
    [Serializable]
    public class EventReward
    {
        public string rewardId;
        public string rewardType; // currency, item, xp, reputation
        public string itemId;
        public int amount;
        public float chance;
        public int participationThreshold;
        
        public EventReward(string type, int amt, float c = 1f)
        {
            rewardId = Guid.NewGuid().ToString();
            rewardType = type;
            amount = amt;
            chance = c;
            participationThreshold = 0;
        }
    }
    
    [Serializable]
    public class EventParticipant
    {
        public string playerId;
        public string playerName;
        public string factionId;
        public float contribution;
        public int kills;
        public float damageDealt;
        public float objectiveScore;
        public DateTime joinTime;
        public bool isActive;
        
        public EventParticipant(string id, string name, string faction)
        {
            playerId = id;
            playerName = name;
            factionId = faction;
            contribution = 0f;
            kills = 0;
            damageDealt = 0f;
            objectiveScore = 0f;
            joinTime = DateTime.UtcNow;
            isActive = true;
        }
        
        public float GetTotalScore()
        {
            return contribution + kills * 100f + damageDealt * 0.1f + objectiveScore;
        }
    }
    
    [Serializable]
    public class WorldEvent
    {
        public string eventId;
        public string eventName;
        public string description;
        public WorldEventType type;
        public EventPhase phase;
        
        // Timing
        public DateTime announcementTime;
        public DateTime startTime;
        public DateTime endTime;
        public float duration; // In minutes
        
        // Location
        public Vector3 centerPosition;
        public float eventRadius;
        public string[] involvedTerritories;
        public string[] involvedFactions;
        
        // Progress
        public List<EventObjective> objectives;
        public List<EventParticipant> participants;
        public float globalProgress;
        
        // Rewards
        public List<EventReward> rewards;
        public Dictionary<string, List<EventReward>> distributedRewards;
        
        // Settings
        public int minLevel;
        public int maxParticipants;
        public bool isPvPEnabled;
        public bool isFactionLocked;
        public float difficultyMultiplier;
        
        public WorldEvent(string name, WorldEventType t)
        {
            eventId = Guid.NewGuid().ToString();
            eventName = name;
            description = "";
            type = t;
            phase = EventPhase.Announced;
            
            announcementTime = DateTime.UtcNow;
            startTime = announcementTime.AddMinutes(30);
            duration = 60f;
            endTime = startTime.AddMinutes(duration);
            
            centerPosition = Vector3.zero;
            eventRadius = 5000f;
            involvedTerritories = new string[0];
            involvedFactions = new string[0];
            
            objectives = new List<EventObjective>();
            participants = new List<EventParticipant>();
            globalProgress = 0f;
            
            rewards = new List<EventReward>();
            distributedRewards = new Dictionary<string, List<EventReward>>();
            
            minLevel = 1;
            maxParticipants = 100;
            isPvPEnabled = false;
            isFactionLocked = false;
            difficultyMultiplier = 1f;
        }
        
        public bool IsPlayerEligible(string playerId, int playerLevel, string playerFaction)
        {
            if (playerLevel < minLevel) return false;
            if (participants.Count >= maxParticipants) return false;
            if (isFactionLocked && !involvedFactions.Contains(playerFaction)) return false;
            if (participants.Any(p => p.playerId == playerId)) return false;
            return true;
        }
        
        public EventParticipant GetParticipant(string playerId)
        {
            return participants.FirstOrDefault(p => p.playerId == playerId);
        }
    }
    
    public class WorldEventSystem : MonoBehaviour
    {
        public static WorldEventSystem Instance { get; private set; }
        
        [Header("Event Settings")]
        [SerializeField] private float eventCheckInterval = 60f; // Check for new events every minute
        [SerializeField] private int maxConcurrentEvents = 5;
        [SerializeField] private float baseEventChance = 0.1f;
        [SerializeField] private float specialEventChance = 0.01f;
        
        [Header("Event Scheduling")]
        [SerializeField] private bool enableScheduledEvents = true;
        [SerializeField] private float peakHourMultiplier = 2f; // More events during peak hours
        
        private List<WorldEvent> activeEvents = new List<WorldEvent>();
        private List<WorldEvent> upcomingEvents = new List<WorldEvent>();
        private List<WorldEvent> completedEvents = new List<WorldEvent>();
        private float eventTimer = 0f;
        
        public event Action<WorldEvent> OnEventAnnounced;
        public event Action<WorldEvent> OnEventStarted;
        public event Action<WorldEvent> OnEventPhaseChanged;
        public event Action<WorldEvent> OnEventCompleted;
        public event Action<WorldEvent, EventObjective> OnObjectiveCompleted;
        public event Action<string, WorldEvent, List<EventReward>> OnRewardsDistributed;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Update()
        {
            eventTimer += Time.deltaTime;
            
            if (eventTimer >= eventCheckInterval)
            {
                eventTimer = 0f;
                CheckForNewEvents();
            }
            
            UpdateActiveEvents();
            UpdateUpcomingEvents();
        }
        
        private void CheckForNewEvents()
        {
            if (activeEvents.Count >= maxConcurrentEvents) return;
            
            float chance = baseEventChance;
            
            // Peak hours (evening) have more events
            float hour = DateTime.Now.Hour;
            if (hour >= 18 && hour <= 23)
            {
                chance *= peakHourMultiplier;
            }
            
            if (UnityEngine.Random.value < chance)
            {
                CreateRandomEvent();
            }
            
            // Special events
            if (UnityEngine.Random.value < specialEventChance)
            {
                CreateSpecialEvent();
            }
        }
        
        private void CreateRandomEvent()
        {
            WorldEventType[] combatTypes = { WorldEventType.AirRaid, WorldEventType.EnemyConvoy, 
                                              WorldEventType.BomberIntercept, WorldEventType.AceShowdown };
            WorldEventType[] economicTypes = { WorldEventType.TradeFleet, WorldEventType.ResourceRush,
                                                WorldEventType.SmugglerSeason };
            
            WorldEventType type;
            if (UnityEngine.Random.value > 0.5f)
            {
                type = combatTypes[UnityEngine.Random.Range(0, combatTypes.Length)];
            }
            else
            {
                type = economicTypes[UnityEngine.Random.Range(0, economicTypes.Length)];
            }
            
            var worldEvent = GenerateEvent(type);
            AnnounceEvent(worldEvent);
        }
        
        private void CreateSpecialEvent()
        {
            // Check for holidays
            DateTime now = DateTime.Now;
            
            if (now.Month == 12 && now.Day >= 20 && now.Day <= 31)
            {
                CreateHolidayEvent("Winter Holiday Event");
            }
            else if (now.Month == 7 && now.Day == 4)
            {
                CreateHolidayEvent("Independence Day");
            }
            else
            {
                // Random double XP/credits event
                var type = UnityEngine.Random.value > 0.5f ? WorldEventType.DoubleXP : WorldEventType.DoubleCredits;
                var worldEvent = GenerateEvent(type);
                worldEvent.duration = 120f; // 2 hours
                AnnounceEvent(worldEvent);
            }
        }
        
        private void CreateHolidayEvent(string name)
        {
            var worldEvent = new WorldEvent(name, WorldEventType.HolidayEvent);
            worldEvent.description = $"Celebrate with special rewards and bonuses!";
            worldEvent.duration = 1440f; // 24 hours
            worldEvent.startTime = DateTime.UtcNow;
            worldEvent.endTime = worldEvent.startTime.AddMinutes(worldEvent.duration);
            
            worldEvent.rewards.Add(new EventReward("currency", 10000));
            worldEvent.rewards.Add(new EventReward("xp", 5000));
            worldEvent.rewards.Add(new EventReward("item", 1) { itemId = "holiday_skin" });
            
            AnnounceEvent(worldEvent);
        }
        
        private WorldEvent GenerateEvent(WorldEventType type)
        {
            string name = GetEventName(type);
            var worldEvent = new WorldEvent(name, type);
            
            // Random location
            worldEvent.centerPosition = new Vector3(
                UnityEngine.Random.Range(-10000f, 10000f),
                0,
                UnityEngine.Random.Range(-10000f, 10000f)
            );
            worldEvent.eventRadius = UnityEngine.Random.Range(2000f, 8000f);
            
            // Generate objectives and rewards based on type
            switch (type)
            {
                case WorldEventType.AirRaid:
                    worldEvent.description = "Enemy bombers are approaching! Shoot them down!";
                    worldEvent.objectives.Add(new EventObjective("destroy_bombers", "Destroy enemy bombers", 20));
                    worldEvent.objectives.Add(new EventObjective("protect_base", "Protect the airbase", 1) { rewardMultiplier = 2f });
                    worldEvent.duration = 45f;
                    worldEvent.isPvPEnabled = false;
                    worldEvent.rewards.Add(new EventReward("currency", 5000));
                    worldEvent.rewards.Add(new EventReward("xp", 2000));
                    break;
                    
                case WorldEventType.TerritoryInvasion:
                    worldEvent.description = "A territory is under attack! Defend or conquer!";
                    worldEvent.objectives.Add(new EventObjective("capture_points", "Capture control points", 5));
                    worldEvent.objectives.Add(new EventObjective("eliminate_defenders", "Eliminate defenders", 50));
                    worldEvent.duration = 60f;
                    worldEvent.isPvPEnabled = true;
                    worldEvent.isFactionLocked = true;
                    worldEvent.rewards.Add(new EventReward("currency", 10000));
                    worldEvent.rewards.Add(new EventReward("reputation", 500));
                    break;
                    
                case WorldEventType.EnemyConvoy:
                    worldEvent.description = "An enemy supply convoy has been spotted! Intercept it!";
                    worldEvent.objectives.Add(new EventObjective("destroy_trucks", "Destroy convoy vehicles", 10));
                    worldEvent.objectives.Add(new EventObjective("destroy_escorts", "Destroy escort fighters", 8));
                    worldEvent.duration = 30f;
                    worldEvent.rewards.Add(new EventReward("currency", 3000));
                    worldEvent.rewards.Add(new EventReward("item", 1) { itemId = "supply_crate" });
                    break;
                    
                case WorldEventType.BomberIntercept:
                    worldEvent.description = "Heavy bombers are heading to strategic targets!";
                    worldEvent.objectives.Add(new EventObjective("intercept", "Intercept bombers before they reach target", 15));
                    worldEvent.duration = 25f;
                    worldEvent.rewards.Add(new EventReward("currency", 4000));
                    worldEvent.rewards.Add(new EventReward("xp", 1500));
                    break;
                    
                case WorldEventType.AceShowdown:
                    worldEvent.description = "An enemy ace pilot has been spotted! Engage and defeat them!";
                    worldEvent.objectives.Add(new EventObjective("defeat_ace", "Defeat the enemy ace", 1) { rewardMultiplier = 5f });
                    worldEvent.duration = 20f;
                    worldEvent.difficultyMultiplier = 3f;
                    worldEvent.rewards.Add(new EventReward("currency", 15000));
                    worldEvent.rewards.Add(new EventReward("item", 1) { itemId = "ace_trophy" });
                    break;
                    
                case WorldEventType.TradeFleet:
                    worldEvent.description = "A merchant fleet is passing through! Great trading opportunity!";
                    worldEvent.objectives.Add(new EventObjective("trade_value", "Complete trades worth", 50000));
                    worldEvent.objectives.Add(new EventObjective("protect_fleet", "Protect merchant ships", 5));
                    worldEvent.duration = 90f;
                    worldEvent.isPvPEnabled = false;
                    worldEvent.rewards.Add(new EventReward("currency", 8000));
                    break;
                    
                case WorldEventType.ResourceRush:
                    worldEvent.description = "Rare resources have been discovered! Gather them before others!";
                    worldEvent.objectives.Add(new EventObjective("gather", "Gather rare resources", 100));
                    worldEvent.duration = 45f;
                    worldEvent.rewards.Add(new EventReward("item", 5) { itemId = "rare_material" });
                    break;
                    
                case WorldEventType.SmugglerSeason:
                    worldEvent.description = "Border patrols are light! Perfect time for smuggling!";
                    worldEvent.objectives.Add(new EventObjective("smuggle", "Complete smuggling runs", 10));
                    worldEvent.duration = 120f;
                    worldEvent.rewards.Add(new EventReward("currency", 20000));
                    break;
                    
                case WorldEventType.DoubleXP:
                    worldEvent.description = "Double XP for all activities!";
                    worldEvent.duration = 60f;
                    worldEvent.rewards.Add(new EventReward("xp", 0)); // Bonus applied during event
                    break;
                    
                case WorldEventType.DoubleCredits:
                    worldEvent.description = "Double credits for all activities!";
                    worldEvent.duration = 60f;
                    worldEvent.rewards.Add(new EventReward("currency", 0)); // Bonus applied during event
                    break;
                    
                default:
                    worldEvent.duration = 60f;
                    worldEvent.rewards.Add(new EventReward("currency", 5000));
                    break;
            }
            
            worldEvent.startTime = DateTime.UtcNow.AddMinutes(UnityEngine.Random.Range(5f, 30f));
            worldEvent.endTime = worldEvent.startTime.AddMinutes(worldEvent.duration);
            
            return worldEvent;
        }
        
        private string GetEventName(WorldEventType type)
        {
            string[] prefixes = { "Operation", "Mission", "Battle of", "Assault on", "Defense of" };
            string[] suffixes = { "Thunder", "Storm", "Eagle", "Phoenix", "Shadow", "Dawn", "Dusk" };
            
            string prefix = prefixes[UnityEngine.Random.Range(0, prefixes.Length)];
            string suffix = suffixes[UnityEngine.Random.Range(0, suffixes.Length)];
            
            return $"{prefix} {suffix}";
        }
        
        private void AnnounceEvent(WorldEvent worldEvent)
        {
            worldEvent.phase = EventPhase.Announced;
            upcomingEvents.Add(worldEvent);
            OnEventAnnounced?.Invoke(worldEvent);
            
            Debug.Log($"[Event Announced] {worldEvent.eventName} starting at {worldEvent.startTime}");
        }
        
        private void UpdateUpcomingEvents()
        {
            for (int i = upcomingEvents.Count - 1; i >= 0; i--)
            {
                var worldEvent = upcomingEvents[i];
                
                if (DateTime.UtcNow >= worldEvent.startTime)
                {
                    StartEvent(worldEvent);
                    upcomingEvents.RemoveAt(i);
                }
                else if (DateTime.UtcNow >= worldEvent.startTime.AddMinutes(-5))
                {
                    worldEvent.phase = EventPhase.Preparation;
                    OnEventPhaseChanged?.Invoke(worldEvent);
                }
            }
        }
        
        private void StartEvent(WorldEvent worldEvent)
        {
            worldEvent.phase = EventPhase.Active;
            activeEvents.Add(worldEvent);
            OnEventStarted?.Invoke(worldEvent);
            
            Debug.Log($"[Event Started] {worldEvent.eventName}");
        }
        
        private void UpdateActiveEvents()
        {
            for (int i = activeEvents.Count - 1; i >= 0; i--)
            {
                var worldEvent = activeEvents[i];
                
                // Check if event should end
                if (DateTime.UtcNow >= worldEvent.endTime)
                {
                    EndEvent(worldEvent);
                    activeEvents.RemoveAt(i);
                    continue;
                }
                
                // Update progress
                UpdateEventProgress(worldEvent);
                
                // Check for phase transitions
                float timeProgress = (float)(DateTime.UtcNow - worldEvent.startTime).TotalMinutes / worldEvent.duration;
                
                if (timeProgress > 0.8f && worldEvent.phase == EventPhase.Active)
                {
                    worldEvent.phase = EventPhase.Climax;
                    OnEventPhaseChanged?.Invoke(worldEvent);
                }
            }
        }
        
        private void UpdateEventProgress(WorldEvent worldEvent)
        {
            if (worldEvent.objectives.Count == 0)
            {
                worldEvent.globalProgress = 1f;
                return;
            }
            
            float totalProgress = 0f;
            foreach (var objective in worldEvent.objectives)
            {
                totalProgress += objective.GetProgress();
                
                if (!objective.isCompleted && objective.currentValue >= objective.targetValue)
                {
                    objective.isCompleted = true;
                    OnObjectiveCompleted?.Invoke(worldEvent, objective);
                }
            }
            
            worldEvent.globalProgress = totalProgress / worldEvent.objectives.Count;
            
            // Check if all objectives completed
            if (worldEvent.objectives.All(o => o.isCompleted))
            {
                worldEvent.phase = EventPhase.Conclusion;
                OnEventPhaseChanged?.Invoke(worldEvent);
            }
        }
        
        private void EndEvent(WorldEvent worldEvent)
        {
            worldEvent.phase = EventPhase.Completed;
            
            // Calculate and distribute rewards
            DistributeRewards(worldEvent);
            
            completedEvents.Add(worldEvent);
            OnEventCompleted?.Invoke(worldEvent);
            
            Debug.Log($"[Event Completed] {worldEvent.eventName} - Progress: {worldEvent.globalProgress:P}");
        }
        
        private void DistributeRewards(WorldEvent worldEvent)
        {
            // Sort participants by contribution
            var rankedParticipants = worldEvent.participants
                .Where(p => p.isActive)
                .OrderByDescending(p => p.GetTotalScore())
                .ToList();
            
            for (int i = 0; i < rankedParticipants.Count; i++)
            {
                var participant = rankedParticipants[i];
                List<EventReward> playerRewards = new List<EventReward>();
                
                // Base participation rewards
                foreach (var reward in worldEvent.rewards)
                {
                    if (UnityEngine.Random.value <= reward.chance)
                    {
                        // Scale rewards by rank
                        float rankMultiplier = 1f - (i * 0.05f); // Top ranks get more
                        rankMultiplier = Mathf.Max(rankMultiplier, 0.5f);
                        
                        var adjustedReward = new EventReward(reward.rewardType, 
                            Mathf.RoundToInt(reward.amount * rankMultiplier * worldEvent.globalProgress));
                        adjustedReward.itemId = reward.itemId;
                        
                        playerRewards.Add(adjustedReward);
                    }
                }
                
                // Bonus for top contributors
                if (i < 3)
                {
                    int bonus = (3 - i) * 2000;
                    playerRewards.Add(new EventReward("currency", bonus));
                }
                
                worldEvent.distributedRewards[participant.playerId] = playerRewards;
                OnRewardsDistributed?.Invoke(participant.playerId, worldEvent, playerRewards);
            }
        }
        
        // Public API
        
        public bool JoinEvent(string eventId, string playerId, string playerName, string factionId, int playerLevel)
        {
            var worldEvent = activeEvents.Find(e => e.eventId == eventId);
            if (worldEvent == null)
            {
                worldEvent = upcomingEvents.Find(e => e.eventId == eventId);
            }
            
            if (worldEvent == null) return false;
            if (!worldEvent.IsPlayerEligible(playerId, playerLevel, factionId)) return false;
            
            var participant = new EventParticipant(playerId, playerName, factionId);
            worldEvent.participants.Add(participant);
            
            return true;
        }
        
        public void LeaveEvent(string eventId, string playerId)
        {
            var worldEvent = activeEvents.Find(e => e.eventId == eventId);
            if (worldEvent == null) return;
            
            var participant = worldEvent.GetParticipant(playerId);
            if (participant != null)
            {
                participant.isActive = false;
            }
        }
        
        public void RecordContribution(string eventId, string playerId, float amount, int kills = 0, float damage = 0f)
        {
            var worldEvent = activeEvents.Find(e => e.eventId == eventId);
            if (worldEvent == null) return;
            
            var participant = worldEvent.GetParticipant(playerId);
            if (participant == null) return;
            
            participant.contribution += amount;
            participant.kills += kills;
            participant.damageDealt += damage;
        }
        
        public void RecordObjectiveProgress(string eventId, string objectiveId, float amount)
        {
            var worldEvent = activeEvents.Find(e => e.eventId == eventId);
            if (worldEvent == null) return;
            
            var objective = worldEvent.objectives.Find(o => o.objectiveId == objectiveId);
            if (objective == null || objective.isCompleted) return;
            
            objective.currentValue += amount;
        }
        
        public List<WorldEvent> GetActiveEvents()
        {
            return new List<WorldEvent>(activeEvents);
        }
        
        public List<WorldEvent> GetUpcomingEvents()
        {
            return new List<WorldEvent>(upcomingEvents);
        }
        
        public WorldEvent GetEvent(string eventId)
        {
            var worldEvent = activeEvents.Find(e => e.eventId == eventId);
            if (worldEvent == null)
            {
                worldEvent = upcomingEvents.Find(e => e.eventId == eventId);
            }
            return worldEvent;
        }
        
        public List<WorldEvent> GetEventsNearPosition(Vector3 position, float maxDistance)
        {
            return activeEvents.Where(e => 
                Vector3.Distance(new Vector3(e.centerPosition.x, 0, e.centerPosition.z), 
                                new Vector3(position.x, 0, position.z)) <= maxDistance
            ).ToList();
        }
        
        public bool IsDoubleXPActive()
        {
            return activeEvents.Any(e => e.type == WorldEventType.DoubleXP);
        }
        
        public bool IsDoubleCreditsActive()
        {
            return activeEvents.Any(e => e.type == WorldEventType.DoubleCredits);
        }
        
        public void ForceStartEvent(WorldEventType type)
        {
            var worldEvent = GenerateEvent(type);
            worldEvent.startTime = DateTime.UtcNow;
            worldEvent.endTime = worldEvent.startTime.AddMinutes(worldEvent.duration);
            StartEvent(worldEvent);
        }
    }
}
