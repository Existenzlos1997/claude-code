using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EarthUnderFreelancer.World
{
    /// <summary>
    /// Persistent World Faction Territory System
    /// Manages faction territories, borders, conquest, and dynamic world state
    /// Inspired by Freelancer with MMO persistent world mechanics
    /// </summary>
    public class WorldFactionSystem : MonoBehaviour
    {
        public static WorldFactionSystem Instance { get; private set; }

        [Header("World Configuration")]
        [SerializeField] private Vector2 worldSize = new Vector2(100000, 100000); // km
        [SerializeField] private int territoryGridSize = 50; // 50x50 territories
        [SerializeField] private float territoryUpdateInterval = 60f; // Update every minute
        [SerializeField] private float borderConflictRadius = 500f;

        [Header("Faction Settings")]
        [SerializeField] private List<FactionDefinition> factionDefinitions;
        [SerializeField] private float conquestThreshold = 1000f; // Influence needed to flip
        [SerializeField] private float influenceDecayRate = 0.01f; // Per minute
        [SerializeField] private float maxInfluencePerPlayer = 100f;

        // World State
        public Dictionary<Vector2Int, TerritoryData> Territories { get; private set; }
        public Dictionary<string, FactionState> FactionStates { get; private set; }
        public List<BorderConflict> ActiveConflicts { get; private set; }
        public List<TradeRoute> ActiveTradeRoutes { get; private set; }

        // Player's Faction
        public string PlayerFactionId { get; private set; }
        public FactionDefinition PlayerFaction => GetFaction(PlayerFactionId);

        // Events
        public event Action<TerritoryData, string, string> OnTerritoryConquered; // territory, newOwner, oldOwner
        public event Action<BorderConflict> OnConflictStarted;
        public event Action<BorderConflict> OnConflictEnded;
        public event Action<FactionState> OnFactionStateChanged;
        public event Action<string, float> OnPlayerInfluenceChanged;
        public event Action OnWorldStateUpdated;

        #region Data Structures

        [Serializable]
        public class FactionDefinition
        {
            public string factionId;
            public string factionName;
            public string shortName; // 3-4 letter abbreviation
            public string description;
            public FactionType type;
            public Color factionColor;
            public Color territoryColor;
            
            // Starting position (center of initial territory)
            public Vector2 capitalPosition;
            public int initialTerritoryRadius = 5;
            
            // Attributes
            public FactionTraits traits;
            public List<string> alliedFactions;
            public List<string> enemyFactions;
            public List<string> neutralFactions;
            
            // Economy
            public float tradeTaxRate = 0.05f;
            public float smugglingRisk = 0.1f;
            public List<string> specializedGoods; // Goods this faction produces cheaper
            public List<string> demandedGoods; // Goods this faction pays more for
            
            // Military
            public float militaryStrength = 1.0f;
            public string[] patrolAircraftTypes;
            public float borderPatrolDensity = 1.0f;
            
            // Icons
            public Sprite factionEmblem;
            public Sprite territoryIcon;
        }

        [Serializable]
        public class FactionTraits
        {
            public bool allowsSmuggling;
            public bool hasBorderControl;
            public bool isAggressive;
            public bool isTradeFocused;
            public float playerReputationModifier = 1.0f;
            public float conquestDefenseBonus = 0f;
        }

        public enum FactionType
        {
            Government,     // Major power
            Corporation,    // Trade focused
            Military,       // Aggressive expansion
            Criminal,       // Pirates, smugglers
            Independent,    // Neutral zones
            Rebel,          // Insurgents
            Ancient         // Mysterious old power
        }

        [Serializable]
        public class FactionState
        {
            public string factionId;
            public int controlledTerritories;
            public float totalInfluence;
            public float economicPower;
            public float militaryPower;
            public int activePlayers;
            public int totalMembers;
            public List<string> controlledStations;
            public Dictionary<string, float> relationshipModifiers; // With other factions
            public DateTime lastMajorEvent;
            public FactionMorale morale;
        }

        public enum FactionMorale
        {
            Desperate,
            Low,
            Stable,
            High,
            Dominant
        }

        [Serializable]
        public class TerritoryData
        {
            public Vector2Int gridPosition;
            public string territoryId;
            public string territoryName;
            public string controllingFactionId;
            public TerritoryType type;
            
            // Influence tracking
            public Dictionary<string, float> factionInfluence; // factionId -> influence
            public float contestedLevel; // 0-1, how contested this territory is
            public bool isBorderTerritory;
            public bool isCapital;
            public bool isConflictZone;
            
            // Resources
            public List<string> resources;
            public float economicValue;
            public float strategicValue;
            
            // Structures
            public List<string> stationIds;
            public List<string> jumpGateIds;
            public int patrolStrength;
            
            // World position
            public Vector3 worldCenter;
            public float territoryRadius;
            
            // History
            public string previousOwner;
            public DateTime lastConquered;
            public int timesConquered;
        }

        public enum TerritoryType
        {
            Core,           // Central, well defended
            Frontier,       // Border region
            Contested,      // Active conflict
            Neutral,        // No faction control
            Lawless,        // Pirate/criminal territory
            Restricted,     // Special access only
            TradeHub,       // Major commerce
            Industrial,     // Manufacturing
            Mining,         // Resource extraction
            Agricultural    // Food production
        }

        [Serializable]
        public class BorderConflict
        {
            public string conflictId;
            public Vector2Int territoryPosition;
            public string attackingFactionId;
            public string defendingFactionId;
            public float attackerInfluence;
            public float defenderInfluence;
            public DateTime startTime;
            public ConflictPhase phase;
            public List<string> attackerParticipants;
            public List<string> defenderParticipants;
            public int attackerKills;
            public int defenderKills;
        }

        public enum ConflictPhase
        {
            Building,       // Influence accumulating
            Skirmish,       // Light fighting
            FullConflict,   // Major battle
            Siege,          // Final push
            Resolved        // Ended
        }

        [Serializable]
        public class TradeRoute
        {
            public string routeId;
            public string routeName;
            public Vector2Int startTerritory;
            public Vector2Int endTerritory;
            public List<Vector2Int> waypoints;
            public float distance;
            public float dangerLevel;
            public float profitMultiplier;
            public bool crossesBorder;
            public List<string> borderCrossings; // Faction IDs crossed
            public bool isSmuggleRoute;
            public List<string> requiredPermits;
        }

        #endregion

        private float lastUpdateTime;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeWorld();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            if (Time.time - lastUpdateTime >= territoryUpdateInterval)
            {
                UpdateWorldState();
                lastUpdateTime = Time.time;
            }
        }

        #region World Initialization

        private void InitializeWorld()
        {
            InitializeFactions();
            InitializeTerritories();
            AssignInitialTerritories();
            CalculateBorders();
            GenerateTradeRoutes();

            Debug.Log($"[World] Initialized {Territories.Count} territories for {factionDefinitions.Count} factions");
        }

        private void InitializeFactions()
        {
            factionDefinitions = new List<FactionDefinition>
            {
                // Major Powers
                new FactionDefinition
                {
                    factionId = "liberty",
                    factionName = "Liberty Republic",
                    shortName = "LIB",
                    description = "A democratic republic focused on freedom and trade. Home to humanity's largest commercial fleets.",
                    type = FactionType.Government,
                    factionColor = new Color(0.2f, 0.4f, 0.8f),
                    territoryColor = new Color(0.3f, 0.5f, 0.9f, 0.5f),
                    capitalPosition = new Vector2(-20000, 0),
                    initialTerritoryRadius = 8,
                    traits = new FactionTraits { allowsSmuggling = false, hasBorderControl = true, isTradeFocused = true },
                    alliedFactions = new List<string> { "bretonia" },
                    enemyFactions = new List<string> { "rheinland", "outcasts" },
                    tradeTaxRate = 0.05f,
                    specializedGoods = new List<string> { "electronics", "software", "luxury_goods" },
                    demandedGoods = new List<string> { "ore", "food", "water" }
                },
                new FactionDefinition
                {
                    factionId = "rheinland",
                    factionName = "Rheinland Empire",
                    shortName = "RHE",
                    description = "A militaristic industrial powerhouse. Known for heavy manufacturing and strict order.",
                    type = FactionType.Military,
                    factionColor = new Color(0.6f, 0.6f, 0.6f),
                    territoryColor = new Color(0.5f, 0.5f, 0.5f, 0.5f),
                    capitalPosition = new Vector2(20000, 0),
                    initialTerritoryRadius = 7,
                    traits = new FactionTraits { hasBorderControl = true, isAggressive = true, conquestDefenseBonus = 0.2f },
                    alliedFactions = new List<string>(),
                    enemyFactions = new List<string> { "liberty", "kusari" },
                    tradeTaxRate = 0.08f,
                    militaryStrength = 1.3f,
                    specializedGoods = new List<string> { "weapons", "ships", "machinery" },
                    demandedGoods = new List<string> { "fuel", "food", "medicine" }
                },
                new FactionDefinition
                {
                    factionId = "bretonia",
                    factionName = "Bretonia Kingdom",
                    shortName = "BRT",
                    description = "An old aristocratic monarchy with rich traditions. Masters of diplomacy and refined goods.",
                    type = FactionType.Government,
                    factionColor = new Color(0.8f, 0.2f, 0.2f),
                    territoryColor = new Color(0.9f, 0.3f, 0.3f, 0.5f),
                    capitalPosition = new Vector2(0, 20000),
                    initialTerritoryRadius = 6,
                    traits = new FactionTraits { isTradeFocused = true, playerReputationModifier = 1.2f },
                    alliedFactions = new List<string> { "liberty" },
                    enemyFactions = new List<string> { "corsairs" },
                    tradeTaxRate = 0.06f,
                    specializedGoods = new List<string> { "luxury_goods", "art", "textiles" },
                    demandedGoods = new List<string> { "ore", "machinery", "weapons" }
                },
                new FactionDefinition
                {
                    factionId = "kusari",
                    factionName = "Kusari Shogunate",
                    shortName = "KUS",
                    description = "An honor-bound empire with advanced technology and ancient traditions.",
                    type = FactionType.Government,
                    factionColor = new Color(1f, 0.4f, 0f),
                    territoryColor = new Color(1f, 0.5f, 0.1f, 0.5f),
                    capitalPosition = new Vector2(0, -20000),
                    initialTerritoryRadius = 6,
                    traits = new FactionTraits { hasBorderControl = true },
                    alliedFactions = new List<string>(),
                    enemyFactions = new List<string> { "rheinland", "blood_dragons" },
                    tradeTaxRate = 0.07f,
                    specializedGoods = new List<string> { "electronics", "robotics", "pharmaceuticals" },
                    demandedGoods = new List<string> { "ore", "food", "luxury_goods" }
                },

                // Criminal Factions
                new FactionDefinition
                {
                    factionId = "outcasts",
                    factionName = "The Outcasts",
                    shortName = "OUT",
                    description = "Dangerous drug runners and pirates. Control the illegal cardamine trade.",
                    type = FactionType.Criminal,
                    factionColor = new Color(0.5f, 0f, 0.5f),
                    territoryColor = new Color(0.6f, 0.1f, 0.6f, 0.5f),
                    capitalPosition = new Vector2(-15000, -15000),
                    initialTerritoryRadius = 4,
                    traits = new FactionTraits { allowsSmuggling = true, isAggressive = true },
                    enemyFactions = new List<string> { "liberty", "bretonia", "kusari", "rheinland" },
                    tradeTaxRate = 0f,
                    smugglingRisk = 0f,
                    specializedGoods = new List<string> { "cardamine", "drugs", "contraband" }
                },
                new FactionDefinition
                {
                    factionId = "corsairs",
                    factionName = "The Corsairs",
                    shortName = "COR",
                    description = "Ruthless pirates who prey on trade lanes. Skilled fighters and raiders.",
                    type = FactionType.Criminal,
                    factionColor = new Color(0.3f, 0f, 0f),
                    territoryColor = new Color(0.4f, 0.1f, 0.1f, 0.5f),
                    capitalPosition = new Vector2(15000, 15000),
                    initialTerritoryRadius = 4,
                    traits = new FactionTraits { allowsSmuggling = true, isAggressive = true },
                    enemyFactions = new List<string> { "bretonia", "outcasts" },
                    tradeTaxRate = 0f,
                    militaryStrength = 1.2f,
                    specializedGoods = new List<string> { "artifacts", "stolen_goods", "weapons" }
                },

                // Independent Zones
                new FactionDefinition
                {
                    factionId = "border_worlds",
                    factionName = "Border Worlds Coalition",
                    shortName = "BWC",
                    description = "Loose alliance of independent systems. Freedom at any cost.",
                    type = FactionType.Independent,
                    factionColor = new Color(0.2f, 0.6f, 0.2f),
                    territoryColor = new Color(0.3f, 0.7f, 0.3f, 0.5f),
                    capitalPosition = new Vector2(25000, -10000),
                    initialTerritoryRadius = 5,
                    traits = new FactionTraits { allowsSmuggling = true, hasBorderControl = false },
                    tradeTaxRate = 0.02f,
                    smugglingRisk = 0.05f
                },

                // Corporations
                new FactionDefinition
                {
                    factionId = "interspace",
                    factionName = "Interspace Commerce",
                    shortName = "ISC",
                    description = "The galaxy's largest trading corporation. Money is the only loyalty.",
                    type = FactionType.Corporation,
                    factionColor = new Color(1f, 0.8f, 0f),
                    territoryColor = new Color(1f, 0.9f, 0.2f, 0.5f),
                    capitalPosition = new Vector2(-10000, 10000),
                    initialTerritoryRadius = 3,
                    traits = new FactionTraits { isTradeFocused = true, playerReputationModifier = 0.8f },
                    tradeTaxRate = 0.03f,
                    specializedGoods = new List<string> { "all" }
                }
            };

            // Initialize faction states
            FactionStates = new Dictionary<string, FactionState>();
            foreach (var faction in factionDefinitions)
            {
                FactionStates[faction.factionId] = new FactionState
                {
                    factionId = faction.factionId,
                    controlledTerritories = 0,
                    totalInfluence = 0,
                    economicPower = 100,
                    militaryPower = faction.militaryStrength * 100,
                    activePlayers = 0,
                    relationshipModifiers = new Dictionary<string, float>(),
                    morale = FactionMorale.Stable
                };
            }
        }

        private void InitializeTerritories()
        {
            Territories = new Dictionary<Vector2Int, TerritoryData>();
            float cellSize = worldSize.x / territoryGridSize;

            for (int x = 0; x < territoryGridSize; x++)
            {
                for (int y = 0; y < territoryGridSize; y++)
                {
                    var pos = new Vector2Int(x, y);
                    var worldPos = GridToWorld(pos);

                    Territories[pos] = new TerritoryData
                    {
                        gridPosition = pos,
                        territoryId = $"territory_{x}_{y}",
                        territoryName = GenerateTerritoryName(x, y),
                        controllingFactionId = null,
                        type = TerritoryType.Neutral,
                        factionInfluence = new Dictionary<string, float>(),
                        worldCenter = new Vector3(worldPos.x, 0, worldPos.y),
                        territoryRadius = cellSize / 2,
                        resources = GenerateResources(x, y),
                        economicValue = UnityEngine.Random.Range(50f, 200f),
                        strategicValue = UnityEngine.Random.Range(10f, 100f),
                        stationIds = new List<string>(),
                        jumpGateIds = new List<string>()
                    };
                }
            }
        }

        private void AssignInitialTerritories()
        {
            foreach (var faction in factionDefinitions)
            {
                var capitalGrid = WorldToGrid(faction.capitalPosition);
                
                // Assign territories in radius around capital
                for (int dx = -faction.initialTerritoryRadius; dx <= faction.initialTerritoryRadius; dx++)
                {
                    for (int dy = -faction.initialTerritoryRadius; dy <= faction.initialTerritoryRadius; dy++)
                    {
                        float distance = Mathf.Sqrt(dx * dx + dy * dy);
                        if (distance <= faction.initialTerritoryRadius)
                        {
                            var pos = new Vector2Int(capitalGrid.x + dx, capitalGrid.y + dy);
                            if (Territories.TryGetValue(pos, out var territory))
                            {
                                // Only claim if unclaimed or weaker claim
                                if (territory.controllingFactionId == null)
                                {
                                    territory.controllingFactionId = faction.factionId;
                                    territory.factionInfluence[faction.factionId] = conquestThreshold;
                                    territory.type = distance <= 2 ? TerritoryType.Core : TerritoryType.Frontier;
                                    
                                    if (dx == 0 && dy == 0)
                                    {
                                        territory.isCapital = true;
                                        territory.territoryName = $"{faction.factionName} Capital";
                                    }

                                    FactionStates[faction.factionId].controlledTerritories++;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void CalculateBorders()
        {
            ActiveConflicts = new List<BorderConflict>();

            foreach (var territory in Territories.Values)
            {
                territory.isBorderTerritory = false;

                if (territory.controllingFactionId == null) continue;

                // Check neighbors
                var neighbors = GetNeighborTerritories(territory.gridPosition);
                foreach (var neighbor in neighbors)
                {
                    if (neighbor.controllingFactionId != territory.controllingFactionId)
                    {
                        territory.isBorderTerritory = true;
                        break;
                    }
                }
            }
        }

        private void GenerateTradeRoutes()
        {
            ActiveTradeRoutes = new List<TradeRoute>();

            // Generate routes between major hubs
            var tradeHubs = Territories.Values.Where(t => 
                t.isCapital || t.economicValue > 150).ToList();

            for (int i = 0; i < tradeHubs.Count; i++)
            {
                for (int j = i + 1; j < tradeHubs.Count; j++)
                {
                    var route = CreateTradeRoute(tradeHubs[i], tradeHubs[j]);
                    ActiveTradeRoutes.Add(route);
                }
            }

            Debug.Log($"[World] Generated {ActiveTradeRoutes.Count} trade routes");
        }

        private TradeRoute CreateTradeRoute(TerritoryData start, TerritoryData end)
        {
            var route = new TradeRoute
            {
                routeId = $"route_{start.territoryId}_{end.territoryId}",
                routeName = $"{start.territoryName} - {end.territoryName}",
                startTerritory = start.gridPosition,
                endTerritory = end.gridPosition,
                waypoints = CalculateRouteWaypoints(start.gridPosition, end.gridPosition),
                borderCrossings = new List<string>()
            };

            // Calculate route properties
            route.distance = Vector2Int.Distance(start.gridPosition, end.gridPosition) * 1000; // km

            // Check for border crossings
            string lastFaction = start.controllingFactionId;
            foreach (var waypoint in route.waypoints)
            {
                if (Territories.TryGetValue(waypoint, out var territory))
                {
                    if (territory.controllingFactionId != lastFaction && territory.controllingFactionId != null)
                    {
                        route.borderCrossings.Add(territory.controllingFactionId);
                        lastFaction = territory.controllingFactionId;
                    }
                }
            }

            route.crossesBorder = route.borderCrossings.Count > 0;
            route.dangerLevel = route.crossesBorder ? 0.3f + route.borderCrossings.Count * 0.1f : 0.1f;
            route.profitMultiplier = 1f + route.distance / 10000f + (route.crossesBorder ? 0.5f : 0f);
            route.isSmuggleRoute = route.crossesBorder && 
                                   route.borderCrossings.Any(f => GetFaction(f)?.traits.hasBorderControl == true);

            return route;
        }

        private List<Vector2Int> CalculateRouteWaypoints(Vector2Int start, Vector2Int end)
        {
            var waypoints = new List<Vector2Int>();
            
            // Simple line algorithm
            int dx = Math.Abs(end.x - start.x);
            int dy = Math.Abs(end.y - start.y);
            int sx = start.x < end.x ? 1 : -1;
            int sy = start.y < end.y ? 1 : -1;
            int err = dx - dy;

            var current = start;
            while (current != end)
            {
                waypoints.Add(current);
                int e2 = 2 * err;
                if (e2 > -dy) { err -= dy; current.x += sx; }
                if (e2 < dx) { err += dx; current.y += sy; }
            }
            waypoints.Add(end);

            return waypoints;
        }

        #endregion

        #region World Updates

        private void UpdateWorldState()
        {
            // Decay influence over time
            foreach (var territory in Territories.Values)
            {
                var keysToUpdate = territory.factionInfluence.Keys.ToList();
                foreach (var factionId in keysToUpdate)
                {
                    if (factionId != territory.controllingFactionId)
                    {
                        territory.factionInfluence[factionId] *= (1f - influenceDecayRate);
                        if (territory.factionInfluence[factionId] < 1f)
                        {
                            territory.factionInfluence.Remove(factionId);
                        }
                    }
                }

                // Update contested level
                if (territory.factionInfluence.Count > 1)
                {
                    float maxInfluence = territory.factionInfluence.Values.Max();
                    float secondMax = territory.factionInfluence.Values.OrderByDescending(v => v).Skip(1).FirstOrDefault();
                    territory.contestedLevel = secondMax / maxInfluence;
                }
                else
                {
                    territory.contestedLevel = 0;
                }
            }

            // Update conflicts
            UpdateConflicts();

            // Update faction states
            UpdateFactionStates();

            OnWorldStateUpdated?.Invoke();
        }

        private void UpdateConflicts()
        {
            // Check for new conflicts
            foreach (var territory in Territories.Values.Where(t => t.isBorderTerritory))
            {
                if (territory.contestedLevel > 0.5f && !territory.isConflictZone)
                {
                    // Start new conflict
                    var attackingFaction = territory.factionInfluence
                        .Where(kv => kv.Key != territory.controllingFactionId)
                        .OrderByDescending(kv => kv.Value)
                        .FirstOrDefault();

                    if (attackingFaction.Key != null)
                    {
                        StartConflict(territory, attackingFaction.Key);
                    }
                }
            }

            // Update existing conflicts
            for (int i = ActiveConflicts.Count - 1; i >= 0; i--)
            {
                var conflict = ActiveConflicts[i];
                var territory = GetTerritory(conflict.territoryPosition);

                if (territory == null) continue;

                // Check for conquest
                if (conflict.attackerInfluence >= conquestThreshold * 1.5f)
                {
                    ConquestTerritory(territory, conflict.attackingFactionId);
                    EndConflict(conflict);
                }
                else if (conflict.defenderInfluence >= conquestThreshold * 2f)
                {
                    // Defender repelled attack
                    EndConflict(conflict);
                }
            }
        }

        private void StartConflict(TerritoryData territory, string attackingFactionId)
        {
            var conflict = new BorderConflict
            {
                conflictId = Guid.NewGuid().ToString(),
                territoryPosition = territory.gridPosition,
                attackingFactionId = attackingFactionId,
                defendingFactionId = territory.controllingFactionId,
                startTime = DateTime.UtcNow,
                phase = ConflictPhase.Building,
                attackerParticipants = new List<string>(),
                defenderParticipants = new List<string>()
            };

            territory.isConflictZone = true;
            ActiveConflicts.Add(conflict);
            OnConflictStarted?.Invoke(conflict);

            Debug.Log($"[World] Conflict started: {GetFaction(attackingFactionId)?.factionName} attacking {territory.territoryName}");
        }

        private void EndConflict(BorderConflict conflict)
        {
            var territory = GetTerritory(conflict.territoryPosition);
            if (territory != null)
            {
                territory.isConflictZone = false;
            }

            conflict.phase = ConflictPhase.Resolved;
            ActiveConflicts.Remove(conflict);
            OnConflictEnded?.Invoke(conflict);
        }

        private void ConquestTerritory(TerritoryData territory, string newOwnerId)
        {
            string oldOwnerId = territory.controllingFactionId;

            // Update territory
            territory.previousOwner = oldOwnerId;
            territory.controllingFactionId = newOwnerId;
            territory.lastConquered = DateTime.UtcNow;
            territory.timesConquered++;
            territory.isConflictZone = false;

            // Update faction stats
            if (oldOwnerId != null && FactionStates.ContainsKey(oldOwnerId))
            {
                FactionStates[oldOwnerId].controlledTerritories--;
            }
            if (FactionStates.ContainsKey(newOwnerId))
            {
                FactionStates[newOwnerId].controlledTerritories++;
            }

            // Recalculate borders
            CalculateBorders();

            OnTerritoryConquered?.Invoke(territory, newOwnerId, oldOwnerId);

            Debug.Log($"[World] {GetFaction(newOwnerId)?.factionName} conquered {territory.territoryName} from {GetFaction(oldOwnerId)?.factionName}");
        }

        private void UpdateFactionStates()
        {
            foreach (var factionId in FactionStates.Keys.ToList())
            {
                var state = FactionStates[factionId];
                
                // Recalculate totals
                state.totalInfluence = Territories.Values
                    .Where(t => t.factionInfluence.ContainsKey(factionId))
                    .Sum(t => t.factionInfluence[factionId]);

                state.economicPower = Territories.Values
                    .Where(t => t.controllingFactionId == factionId)
                    .Sum(t => t.economicValue);

                // Update morale based on recent events
                float territoriesPercent = (float)state.controlledTerritories / Territories.Count;
                state.morale = territoriesPercent switch
                {
                    < 0.02f => FactionMorale.Desperate,
                    < 0.05f => FactionMorale.Low,
                    < 0.15f => FactionMorale.Stable,
                    < 0.25f => FactionMorale.High,
                    _ => FactionMorale.Dominant
                };

                OnFactionStateChanged?.Invoke(state);
            }
        }

        #endregion

        #region Player Actions

        public void SetPlayerFaction(string factionId)
        {
            PlayerFactionId = factionId;
            Debug.Log($"[World] Player joined {GetFaction(factionId)?.factionName}");
        }

        public void AddInfluence(Vector2Int territoryPos, string factionId, float amount)
        {
            if (!Territories.TryGetValue(territoryPos, out var territory)) return;

            amount = Mathf.Min(amount, maxInfluencePerPlayer);

            if (!territory.factionInfluence.ContainsKey(factionId))
            {
                territory.factionInfluence[factionId] = 0;
            }

            territory.factionInfluence[factionId] += amount;
            OnPlayerInfluenceChanged?.Invoke(factionId, amount);

            // Update conflict if active
            var conflict = ActiveConflicts.Find(c => c.territoryPosition == territoryPos);
            if (conflict != null)
            {
                if (factionId == conflict.attackingFactionId)
                {
                    conflict.attackerInfluence += amount;
                }
                else if (factionId == conflict.defendingFactionId)
                {
                    conflict.defenderInfluence += amount;
                }
            }
        }

        public void ParticipateInConflict(string conflictId, string playerId, bool asAttacker)
        {
            var conflict = ActiveConflicts.Find(c => c.conflictId == conflictId);
            if (conflict == null) return;

            if (asAttacker)
            {
                if (!conflict.attackerParticipants.Contains(playerId))
                    conflict.attackerParticipants.Add(playerId);
            }
            else
            {
                if (!conflict.defenderParticipants.Contains(playerId))
                    conflict.defenderParticipants.Add(playerId);
            }
        }

        public void RecordConflictKill(string conflictId, bool attackerKill)
        {
            var conflict = ActiveConflicts.Find(c => c.conflictId == conflictId);
            if (conflict == null) return;

            if (attackerKill)
                conflict.attackerKills++;
            else
                conflict.defenderKills++;

            // Kills add influence
            float influenceGain = 50f;
            var factionId = attackerKill ? conflict.attackingFactionId : conflict.defendingFactionId;
            AddInfluence(conflict.territoryPosition, factionId, influenceGain);
        }

        #endregion

        #region Queries

        public TerritoryData GetTerritory(Vector2Int pos)
        {
            return Territories.TryGetValue(pos, out var territory) ? territory : null;
        }

        public TerritoryData GetTerritoryAtWorldPos(Vector3 worldPos)
        {
            var gridPos = WorldToGrid(new Vector2(worldPos.x, worldPos.z));
            return GetTerritory(gridPos);
        }

        public FactionDefinition GetFaction(string factionId)
        {
            return factionDefinitions.Find(f => f.factionId == factionId);
        }

        public FactionState GetFactionState(string factionId)
        {
            return FactionStates.TryGetValue(factionId, out var state) ? state : null;
        }

        public List<TerritoryData> GetFactionTerritories(string factionId)
        {
            return Territories.Values.Where(t => t.controllingFactionId == factionId).ToList();
        }

        public List<TerritoryData> GetBorderTerritories(string factionId)
        {
            return Territories.Values.Where(t => 
                t.controllingFactionId == factionId && t.isBorderTerritory).ToList();
        }

        public List<TerritoryData> GetContestedTerritories()
        {
            return Territories.Values.Where(t => t.isConflictZone).ToList();
        }

        public List<BorderConflict> GetActiveConflictsForFaction(string factionId)
        {
            return ActiveConflicts.Where(c => 
                c.attackingFactionId == factionId || c.defendingFactionId == factionId).ToList();
        }

        public List<TradeRoute> GetRoutesFromTerritory(Vector2Int pos)
        {
            return ActiveTradeRoutes.Where(r => r.startTerritory == pos || r.endTerritory == pos).ToList();
        }

        public List<TradeRoute> GetSmuggleRoutes()
        {
            return ActiveTradeRoutes.Where(r => r.isSmuggleRoute).ToList();
        }

        public bool IsSafeTerritory(Vector3 worldPos, string playerFactionId)
        {
            var territory = GetTerritoryAtWorldPos(worldPos);
            if (territory == null) return false;

            if (territory.controllingFactionId == playerFactionId) return true;

            var controllingFaction = GetFaction(territory.controllingFactionId);
            var playerFaction = GetFaction(playerFactionId);

            if (controllingFaction != null && playerFaction != null)
            {
                return playerFaction.alliedFactions.Contains(territory.controllingFactionId);
            }

            return false;
        }

        public float GetSmuggleRisk(Vector3 worldPos, string playerFactionId)
        {
            var territory = GetTerritoryAtWorldPos(worldPos);
            if (territory == null) return 0f;

            if (territory.controllingFactionId == playerFactionId) return 0f;

            var controllingFaction = GetFaction(territory.controllingFactionId);
            if (controllingFaction == null) return 0.1f;

            if (controllingFaction.enemyFactions.Contains(playerFactionId))
            {
                return controllingFaction.smugglingRisk * 2f;
            }

            return controllingFaction.smugglingRisk;
        }

        #endregion

        #region Helpers

        private List<TerritoryData> GetNeighborTerritories(Vector2Int pos)
        {
            var neighbors = new List<TerritoryData>();
            var offsets = new Vector2Int[] 
            {
                new Vector2Int(-1, 0), new Vector2Int(1, 0),
                new Vector2Int(0, -1), new Vector2Int(0, 1),
                new Vector2Int(-1, -1), new Vector2Int(1, -1),
                new Vector2Int(-1, 1), new Vector2Int(1, 1)
            };

            foreach (var offset in offsets)
            {
                var neighborPos = pos + offset;
                if (Territories.TryGetValue(neighborPos, out var neighbor))
                {
                    neighbors.Add(neighbor);
                }
            }

            return neighbors;
        }

        private Vector2Int WorldToGrid(Vector2 worldPos)
        {
            float cellSize = worldSize.x / territoryGridSize;
            int x = Mathf.FloorToInt((worldPos.x + worldSize.x / 2) / cellSize);
            int y = Mathf.FloorToInt((worldPos.y + worldSize.y / 2) / cellSize);
            return new Vector2Int(
                Mathf.Clamp(x, 0, territoryGridSize - 1),
                Mathf.Clamp(y, 0, territoryGridSize - 1)
            );
        }

        private Vector2 GridToWorld(Vector2Int gridPos)
        {
            float cellSize = worldSize.x / territoryGridSize;
            float x = (gridPos.x * cellSize) - worldSize.x / 2 + cellSize / 2;
            float y = (gridPos.y * cellSize) - worldSize.y / 2 + cellSize / 2;
            return new Vector2(x, y);
        }

        private string GenerateTerritoryName(int x, int y)
        {
            string[] prefixes = { "Alpha", "Beta", "Gamma", "Delta", "Epsilon", "Zeta", "Eta", "Theta", "Iota", "Kappa" };
            string[] suffixes = { "Sector", "Zone", "Region", "Quadrant", "System", "Field", "Cluster", "Expanse" };
            
            int prefixIndex = (x + y * territoryGridSize) % prefixes.Length;
            int suffixIndex = (x * y) % suffixes.Length;
            
            return $"{prefixes[prefixIndex]}-{x}{y} {suffixes[suffixIndex]}";
        }

        private List<string> GenerateResources(int x, int y)
        {
            var resources = new List<string>();
            string[] allResources = { "ore", "fuel", "water", "food", "electronics", "weapons", "luxury_goods", "medicine", "machinery" };

            // Each territory has 1-3 resources based on position
            int resourceCount = 1 + (x + y) % 3;
            for (int i = 0; i < resourceCount; i++)
            {
                int index = (x * 7 + y * 11 + i * 13) % allResources.Length;
                if (!resources.Contains(allResources[index]))
                {
                    resources.Add(allResources[index]);
                }
            }

            return resources;
        }

        #endregion
    }
}
