using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EarthUnderFreelancer.World
{
    /// <summary>
    /// Dynamic Trading and Smuggling Mission System
    /// Creates trade runs, smuggling missions, and border crossing gameplay
    /// </summary>
    public class TradeSmugglingSystem : MonoBehaviour
    {
        public static TradeSmugglingSystem Instance { get; private set; }

        [Header("Trade Configuration")]
        [SerializeField] private List<TradeGoodDefinition> tradeGoods;
        [SerializeField] private float priceUpdateInterval = 300f; // 5 minutes
        [SerializeField] private float priceFluctuation = 0.2f;

        [Header("Smuggling Configuration")]
        [SerializeField] private float baseDetectionChance = 0.1f;
        [SerializeField] private float borderPatrolRange = 1000f;
        [SerializeField] private float smuggleRewardMultiplier = 2.5f;

        [Header("Mission Generation")]
        [SerializeField] private int maxActiveTradeMissions = 20;
        [SerializeField] private int maxActiveSmugglingMissions = 10;
        [SerializeField] private float missionRefreshInterval = 600f;

        // Current state
        public List<TradeMission> AvailableTradeMissions { get; private set; }
        public List<SmugglingMission> AvailableSmugglingMissions { get; private set; }
        public TradeMission ActiveTradeMission { get; private set; }
        public SmugglingMission ActiveSmugglingMission { get; private set; }
        public Dictionary<string, MarketData> StationMarkets { get; private set; }

        // Player cargo
        public List<CargoItem> PlayerCargo { get; private set; }
        public int CargoCapacity { get; set; } = 100;
        public int CurrentCargoUsed => PlayerCargo.Sum(c => c.quantity);

        // Events
        public event Action<TradeMission> OnTradeMissionAccepted;
        public event Action<TradeMission, bool> OnTradeMissionCompleted; // mission, success
        public event Action<SmugglingMission> OnSmugglingMissionAccepted;
        public event Action<SmugglingMission, bool> OnSmugglingMissionCompleted;
        public event Action<BorderCrossingEvent> OnBorderCrossing;
        public event Action<bool> OnSmugglingDetected; // caught or escaped
        public event Action<CargoItem, int> OnCargoChanged; // item, quantity change
        public event Action<string, float> OnReputationChanged; // faction, change

        #region Data Structures

        [Serializable]
        public class TradeGoodDefinition
        {
            public string goodId;
            public string goodName;
            public string description;
            public GoodCategory category;
            public bool isContraband;
            public bool isLegal;
            public float basePrice;
            public float weight; // per unit
            public Sprite icon;
            
            // Faction modifiers
            public List<FactionPriceModifier> factionModifiers;
            
            // Legality by faction
            public List<string> illegalInFactions;
            public List<string> producedByFactions;
            public List<string> demandedByFactions;
        }

        [Serializable]
        public class FactionPriceModifier
        {
            public string factionId;
            public float buyPriceModifier = 1.0f;
            public float sellPriceModifier = 1.0f;
        }

        public enum GoodCategory
        {
            RawMaterials,
            Manufactured,
            Food,
            Medicine,
            Weapons,
            Luxury,
            Technology,
            Fuel,
            Contraband
        }

        [Serializable]
        public class MarketData
        {
            public string stationId;
            public string stationName;
            public string factionId;
            public Vector3 position;
            public Dictionary<string, GoodMarketPrice> prices;
            public DateTime lastUpdate;
        }

        [Serializable]
        public class GoodMarketPrice
        {
            public string goodId;
            public float buyPrice;
            public float sellPrice;
            public int availableQuantity;
            public int demand;
            public bool isIllegal;
        }

        [Serializable]
        public class CargoItem
        {
            public string goodId;
            public string goodName;
            public int quantity;
            public float purchasePrice;
            public string purchasedAt;
            public bool isContraband;
        }

        [Serializable]
        public class TradeMission
        {
            public string missionId;
            public string title;
            public string description;
            public TradeMissionType type;
            
            // Route
            public string sourceStationId;
            public string sourceStationName;
            public string sourceFactionId;
            public string destinationStationId;
            public string destinationStationName;
            public string destinationFactionId;
            public float distance;
            public bool crossesBorder;
            
            // Cargo
            public string requiredGoodId;
            public string requiredGoodName;
            public int requiredQuantity;
            
            // Rewards
            public int creditReward;
            public int experienceReward;
            public float reputationReward;
            public string reputationFaction;
            
            // Time
            public float timeLimit; // seconds, 0 = no limit
            public DateTime createdAt;
            public DateTime? acceptedAt;
            
            // State
            public MissionState state;
        }

        public enum TradeMissionType
        {
            Delivery,           // Simple A to B
            Supply,             // Buy and deliver
            EmergencySupply,    // Time critical
            BulkTransport,      // Large quantity
            SpecialCargo        // Unique item
        }

        [Serializable]
        public class SmugglingMission
        {
            public string missionId;
            public string title;
            public string description;
            public SmugglingType type;
            
            // Route
            public string sourceStationId;
            public string sourceFactionId;
            public string destinationStationId;
            public string destinationFactionId;
            public List<string> borderCrossings;
            public float distance;
            
            // Cargo
            public string contrabandId;
            public string contrabandName;
            public int quantity;
            
            // Risk/Reward
            public float riskLevel; // 0-1
            public int creditReward;
            public float reputationPenaltyIfCaught;
            public string penaltyFaction;
            
            // State
            public MissionState state;
            public int bordersCrossed;
            public int timesScanned;
            public bool hasBeenDetected;
        }

        public enum SmugglingType
        {
            ContrabandDelivery,     // Illegal goods
            SanctionedGoods,        // Legal goods to enemy faction
            WeaponsRunning,         // Military hardware
            DrugTrafficking,        // Narcotics
            DataSmuggling,          // Information/tech
            PersonSmuggling         // VIP transport
        }

        public enum MissionState
        {
            Available,
            Accepted,
            InProgress,
            Completed,
            Failed,
            Abandoned
        }

        [Serializable]
        public class BorderCrossingEvent
        {
            public Vector3 crossingPoint;
            public string fromFaction;
            public string toFaction;
            public bool wasScanned;
            public bool contrabandDetected;
            public float detectionChance;
            public DateTime timestamp;
        }

        #endregion

        private float lastPriceUpdate;
        private float lastMissionRefresh;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            if (Time.time - lastPriceUpdate >= priceUpdateInterval)
            {
                UpdateMarketPrices();
                lastPriceUpdate = Time.time;
            }

            if (Time.time - lastMissionRefresh >= missionRefreshInterval)
            {
                GenerateMissions();
                lastMissionRefresh = Time.time;
            }

            // Check mission timers
            CheckMissionTimers();
        }

        #region Initialization

        private void Initialize()
        {
            InitializeTradeGoods();
            InitializeMarkets();
            AvailableTradeMissions = new List<TradeMission>();
            AvailableSmugglingMissions = new List<SmugglingMission>();
            PlayerCargo = new List<CargoItem>();
            GenerateMissions();
        }

        private void InitializeTradeGoods()
        {
            tradeGoods = new List<TradeGoodDefinition>
            {
                // Raw Materials
                new TradeGoodDefinition { goodId = "ore", goodName = "Raw Ore", basePrice = 50, category = GoodCategory.RawMaterials, weight = 2f },
                new TradeGoodDefinition { goodId = "water", goodName = "Water", basePrice = 30, category = GoodCategory.RawMaterials, weight = 1f },
                new TradeGoodDefinition { goodId = "fuel", goodName = "Hydrogen Fuel", basePrice = 100, category = GoodCategory.Fuel, weight = 0.5f },
                new TradeGoodDefinition { goodId = "crystals", goodName = "Energy Crystals", basePrice = 500, category = GoodCategory.RawMaterials, weight = 0.2f },

                // Manufactured
                new TradeGoodDefinition { goodId = "electronics", goodName = "Electronics", basePrice = 200, category = GoodCategory.Manufactured, weight = 0.5f },
                new TradeGoodDefinition { goodId = "machinery", goodName = "Industrial Machinery", basePrice = 350, category = GoodCategory.Manufactured, weight = 3f },
                new TradeGoodDefinition { goodId = "ship_parts", goodName = "Ship Components", basePrice = 450, category = GoodCategory.Manufactured, weight = 2f },

                // Food
                new TradeGoodDefinition { goodId = "food", goodName = "Basic Foodstuffs", basePrice = 40, category = GoodCategory.Food, weight = 1f },
                new TradeGoodDefinition { goodId = "luxury_food", goodName = "Luxury Cuisine", basePrice = 300, category = GoodCategory.Food, weight = 0.5f },

                // Medicine
                new TradeGoodDefinition { goodId = "medicine", goodName = "Medical Supplies", basePrice = 250, category = GoodCategory.Medicine, weight = 0.3f },
                new TradeGoodDefinition { goodId = "pharmaceuticals", goodName = "Pharmaceuticals", basePrice = 400, category = GoodCategory.Medicine, weight = 0.2f },

                // Weapons (restricted)
                new TradeGoodDefinition 
                { 
                    goodId = "weapons", 
                    goodName = "Military Weapons", 
                    basePrice = 800, 
                    category = GoodCategory.Weapons, 
                    weight = 1f,
                    illegalInFactions = new List<string> { "liberty", "bretonia" }
                },
                new TradeGoodDefinition 
                { 
                    goodId = "explosives", 
                    goodName = "Explosives", 
                    basePrice = 600, 
                    category = GoodCategory.Weapons, 
                    weight = 1.5f,
                    illegalInFactions = new List<string> { "liberty", "bretonia", "kusari" }
                },

                // Luxury
                new TradeGoodDefinition { goodId = "luxury_goods", goodName = "Luxury Goods", basePrice = 500, category = GoodCategory.Luxury, weight = 0.3f },
                new TradeGoodDefinition { goodId = "art", goodName = "Rare Artwork", basePrice = 1500, category = GoodCategory.Luxury, weight = 0.5f },
                new TradeGoodDefinition { goodId = "alien_artifacts", goodName = "Alien Artifacts", basePrice = 5000, category = GoodCategory.Luxury, weight = 0.2f },

                // Technology
                new TradeGoodDefinition { goodId = "software", goodName = "Advanced Software", basePrice = 300, category = GoodCategory.Technology, weight = 0f },
                new TradeGoodDefinition { goodId = "robotics", goodName = "Robotics", basePrice = 600, category = GoodCategory.Technology, weight = 1f },

                // Contraband
                new TradeGoodDefinition 
                { 
                    goodId = "cardamine", 
                    goodName = "Cardamine", 
                    basePrice = 2000, 
                    category = GoodCategory.Contraband, 
                    weight = 0.1f,
                    isContraband = true,
                    illegalInFactions = new List<string> { "liberty", "bretonia", "kusari", "rheinland" },
                    producedByFactions = new List<string> { "outcasts" }
                },
                new TradeGoodDefinition 
                { 
                    goodId = "synth_drugs", 
                    goodName = "Synthetic Drugs", 
                    basePrice = 1500, 
                    category = GoodCategory.Contraband, 
                    weight = 0.1f,
                    isContraband = true,
                    illegalInFactions = new List<string> { "liberty", "bretonia", "kusari", "rheinland" }
                },
                new TradeGoodDefinition 
                { 
                    goodId = "slaves", 
                    goodName = "Human Cargo", 
                    basePrice = 3000, 
                    category = GoodCategory.Contraband, 
                    weight = 0.5f,
                    isContraband = true,
                    illegalInFactions = new List<string> { "liberty", "bretonia", "kusari", "rheinland", "border_worlds" }
                },
                new TradeGoodDefinition 
                { 
                    goodId = "stolen_goods", 
                    goodName = "Stolen Merchandise", 
                    basePrice = 800, 
                    category = GoodCategory.Contraband, 
                    weight = 1f,
                    isContraband = true,
                    illegalInFactions = new List<string> { "liberty", "bretonia", "kusari", "rheinland" }
                }
            };
        }

        private void InitializeMarkets()
        {
            StationMarkets = new Dictionary<string, MarketData>();
            // Would generate markets for all stations in the world
            // For now, create some sample markets
            CreateSampleMarkets();
        }

        private void CreateSampleMarkets()
        {
            var factions = new[] { "liberty", "rheinland", "bretonia", "kusari", "outcasts", "corsairs" };
            var stationTypes = new[] { "Hub", "Outpost", "Factory", "Base" };

            for (int i = 0; i < 20; i++)
            {
                string factionId = factions[i % factions.Length];
                string stationType = stationTypes[i % stationTypes.Length];
                string stationId = $"station_{factionId}_{i}";

                var market = new MarketData
                {
                    stationId = stationId,
                    stationName = $"{GetFactionName(factionId)} {stationType} {i + 1}",
                    factionId = factionId,
                    position = new Vector3(
                        UnityEngine.Random.Range(-40000, 40000),
                        0,
                        UnityEngine.Random.Range(-40000, 40000)
                    ),
                    prices = new Dictionary<string, GoodMarketPrice>(),
                    lastUpdate = DateTime.UtcNow
                };

                // Generate prices for all goods
                foreach (var good in tradeGoods)
                {
                    float priceModifier = 1f;
                    
                    // Produced goods are cheaper
                    if (good.producedByFactions != null && good.producedByFactions.Contains(factionId))
                    {
                        priceModifier = 0.6f;
                    }
                    // Demanded goods are more expensive
                    else if (good.demandedByFactions != null && good.demandedByFactions.Contains(factionId))
                    {
                        priceModifier = 1.4f;
                    }

                    priceModifier += UnityEngine.Random.Range(-priceFluctuation, priceFluctuation);

                    bool isIllegal = good.isContraband || 
                                     (good.illegalInFactions != null && good.illegalInFactions.Contains(factionId));

                    market.prices[good.goodId] = new GoodMarketPrice
                    {
                        goodId = good.goodId,
                        buyPrice = good.basePrice * priceModifier,
                        sellPrice = good.basePrice * priceModifier * 0.9f,
                        availableQuantity = UnityEngine.Random.Range(0, 1000),
                        demand = UnityEngine.Random.Range(0, 100),
                        isIllegal = isIllegal
                    };
                }

                StationMarkets[stationId] = market;
            }
        }

        private string GetFactionName(string factionId)
        {
            return factionId switch
            {
                "liberty" => "Liberty",
                "rheinland" => "Rheinland",
                "bretonia" => "Bretonia",
                "kusari" => "Kusari",
                "outcasts" => "Outcast",
                "corsairs" => "Corsair",
                _ => factionId
            };
        }

        #endregion

        #region Market System

        private void UpdateMarketPrices()
        {
            foreach (var market in StationMarkets.Values)
            {
                foreach (var price in market.prices.Values)
                {
                    // Random fluctuation
                    float change = UnityEngine.Random.Range(-priceFluctuation, priceFluctuation);
                    var good = GetGoodDefinition(price.goodId);
                    if (good != null)
                    {
                        float minPrice = good.basePrice * 0.5f;
                        float maxPrice = good.basePrice * 2f;
                        price.buyPrice = Mathf.Clamp(price.buyPrice * (1 + change), minPrice, maxPrice);
                        price.sellPrice = price.buyPrice * 0.9f;
                    }

                    // Update quantities
                    price.availableQuantity += UnityEngine.Random.Range(-50, 100);
                    price.availableQuantity = Mathf.Max(0, price.availableQuantity);
                    price.demand = Mathf.Clamp(price.demand + UnityEngine.Random.Range(-10, 10), 0, 100);
                }
                market.lastUpdate = DateTime.UtcNow;
            }

            Debug.Log("[Trade] Market prices updated");
        }

        public bool BuyCargo(string stationId, string goodId, int quantity)
        {
            if (!StationMarkets.TryGetValue(stationId, out var market)) return false;
            if (!market.prices.TryGetValue(goodId, out var price)) return false;

            if (price.availableQuantity < quantity)
            {
                Debug.LogWarning("[Trade] Not enough stock!");
                return false;
            }

            if (CurrentCargoUsed + quantity > CargoCapacity)
            {
                Debug.LogWarning("[Trade] Not enough cargo space!");
                return false;
            }

            float totalCost = price.buyPrice * quantity;
            // Would check player credits here

            // Add to cargo
            var existingCargo = PlayerCargo.Find(c => c.goodId == goodId && c.purchasedAt == stationId);
            if (existingCargo != null)
            {
                existingCargo.quantity += quantity;
            }
            else
            {
                var good = GetGoodDefinition(goodId);
                PlayerCargo.Add(new CargoItem
                {
                    goodId = goodId,
                    goodName = good?.goodName ?? goodId,
                    quantity = quantity,
                    purchasePrice = price.buyPrice,
                    purchasedAt = stationId,
                    isContraband = good?.isContraband ?? false
                });
            }

            // Update market
            price.availableQuantity -= quantity;

            OnCargoChanged?.Invoke(PlayerCargo.Last(), quantity);
            Debug.Log($"[Trade] Bought {quantity}x {goodId} for {totalCost:C}");
            return true;
        }

        public float SellCargo(string stationId, string goodId, int quantity)
        {
            if (!StationMarkets.TryGetValue(stationId, out var market)) return 0;
            if (!market.prices.TryGetValue(goodId, out var price)) return 0;

            var cargoItem = PlayerCargo.Find(c => c.goodId == goodId);
            if (cargoItem == null || cargoItem.quantity < quantity)
            {
                Debug.LogWarning("[Trade] Not enough cargo to sell!");
                return 0;
            }

            // Check if illegal
            if (price.isIllegal)
            {
                // Can only sell contraband at certain places
                Debug.LogWarning("[Trade] Cannot sell contraband here legally!");
                // But might have black market option
            }

            float totalEarnings = price.sellPrice * quantity;
            float profit = (price.sellPrice - cargoItem.purchasePrice) * quantity;

            // Remove from cargo
            cargoItem.quantity -= quantity;
            if (cargoItem.quantity <= 0)
            {
                PlayerCargo.Remove(cargoItem);
            }

            // Update market
            price.availableQuantity += quantity;

            OnCargoChanged?.Invoke(cargoItem, -quantity);
            Debug.Log($"[Trade] Sold {quantity}x {goodId} for {totalEarnings:C} (Profit: {profit:C})");
            return totalEarnings;
        }

        public TradeGoodDefinition GetGoodDefinition(string goodId)
        {
            return tradeGoods.Find(g => g.goodId == goodId);
        }

        public List<TradeOpportunity> FindBestTradeOpportunities(string currentStationId, int topN = 5)
        {
            var opportunities = new List<TradeOpportunity>();

            if (!StationMarkets.TryGetValue(currentStationId, out var currentMarket)) return opportunities;

            foreach (var targetMarket in StationMarkets.Values)
            {
                if (targetMarket.stationId == currentStationId) continue;

                foreach (var good in tradeGoods)
                {
                    if (!currentMarket.prices.TryGetValue(good.goodId, out var buyPrice)) continue;
                    if (!targetMarket.prices.TryGetValue(good.goodId, out var sellPrice)) continue;

                    float profit = sellPrice.sellPrice - buyPrice.buyPrice;
                    if (profit <= 0) continue;

                    float distance = Vector3.Distance(currentMarket.position, targetMarket.position);
                    float profitPerDistance = profit / (distance / 1000f);

                    opportunities.Add(new TradeOpportunity
                    {
                        goodId = good.goodId,
                        goodName = good.goodName,
                        sourceStation = currentStationId,
                        targetStation = targetMarket.stationId,
                        targetStationName = targetMarket.stationName,
                        buyPrice = buyPrice.buyPrice,
                        sellPrice = sellPrice.sellPrice,
                        profitPerUnit = profit,
                        distance = distance,
                        profitPerKm = profitPerDistance,
                        isContraband = good.isContraband,
                        crossesBorder = currentMarket.factionId != targetMarket.factionId
                    });
                }
            }

            return opportunities.OrderByDescending(o => o.profitPerKm).Take(topN).ToList();
        }

        [Serializable]
        public class TradeOpportunity
        {
            public string goodId;
            public string goodName;
            public string sourceStation;
            public string targetStation;
            public string targetStationName;
            public float buyPrice;
            public float sellPrice;
            public float profitPerUnit;
            public float distance;
            public float profitPerKm;
            public bool isContraband;
            public bool crossesBorder;
        }

        #endregion

        #region Mission System

        private void GenerateMissions()
        {
            GenerateTradeMissions();
            GenerateSmugglingMissions();
        }

        private void GenerateTradeMissions()
        {
            while (AvailableTradeMissions.Count < maxActiveTradeMissions)
            {
                var mission = CreateRandomTradeMission();
                if (mission != null)
                {
                    AvailableTradeMissions.Add(mission);
                }
            }
        }

        private TradeMission CreateRandomTradeMission()
        {
            var markets = StationMarkets.Values.ToList();
            if (markets.Count < 2) return null;

            var source = markets[UnityEngine.Random.Range(0, markets.Count)];
            var destination = markets[UnityEngine.Random.Range(0, markets.Count)];
            while (destination.stationId == source.stationId)
            {
                destination = markets[UnityEngine.Random.Range(0, markets.Count)];
            }

            var good = tradeGoods[UnityEngine.Random.Range(0, tradeGoods.Count)];
            while (good.isContraband)
            {
                good = tradeGoods[UnityEngine.Random.Range(0, tradeGoods.Count)];
            }

            int quantity = UnityEngine.Random.Range(10, 100);
            float distance = Vector3.Distance(source.position, destination.position);
            bool crossesBorder = source.factionId != destination.factionId;

            float baseReward = good.basePrice * quantity * 0.3f;
            if (crossesBorder) baseReward *= 1.3f;

            return new TradeMission
            {
                missionId = Guid.NewGuid().ToString(),
                title = $"Deliver {good.goodName} to {destination.stationName}",
                description = $"Transport {quantity} units of {good.goodName} from {source.stationName} to {destination.stationName}.",
                type = TradeMissionType.Delivery,
                sourceStationId = source.stationId,
                sourceStationName = source.stationName,
                sourceFactionId = source.factionId,
                destinationStationId = destination.stationId,
                destinationStationName = destination.stationName,
                destinationFactionId = destination.factionId,
                distance = distance,
                crossesBorder = crossesBorder,
                requiredGoodId = good.goodId,
                requiredGoodName = good.goodName,
                requiredQuantity = quantity,
                creditReward = (int)baseReward,
                experienceReward = (int)(baseReward / 10),
                reputationReward = 5f,
                reputationFaction = destination.factionId,
                timeLimit = crossesBorder ? 1800f : 0f,
                createdAt = DateTime.UtcNow,
                state = MissionState.Available
            };
        }

        private void GenerateSmugglingMissions()
        {
            while (AvailableSmugglingMissions.Count < maxActiveSmugglingMissions)
            {
                var mission = CreateRandomSmugglingMission();
                if (mission != null)
                {
                    AvailableSmugglingMissions.Add(mission);
                }
            }
        }

        private SmugglingMission CreateRandomSmugglingMission()
        {
            var markets = StationMarkets.Values.ToList();
            var contrabandGoods = tradeGoods.Where(g => g.isContraband).ToList();
            
            if (markets.Count < 2 || contrabandGoods.Count == 0) return null;

            // Source should be criminal faction
            var criminalMarkets = markets.Where(m => 
                m.factionId == "outcasts" || m.factionId == "corsairs" || m.factionId == "border_worlds").ToList();
            
            if (criminalMarkets.Count == 0) criminalMarkets = markets;

            var source = criminalMarkets[UnityEngine.Random.Range(0, criminalMarkets.Count)];
            
            // Destination should be different faction where goods are illegal
            var good = contrabandGoods[UnityEngine.Random.Range(0, contrabandGoods.Count)];
            var validDestinations = markets.Where(m => 
                m.factionId != source.factionId &&
                good.illegalInFactions != null &&
                good.illegalInFactions.Contains(m.factionId)).ToList();

            if (validDestinations.Count == 0) return null;

            var destination = validDestinations[UnityEngine.Random.Range(0, validDestinations.Count)];

            int quantity = UnityEngine.Random.Range(5, 50);
            float distance = Vector3.Distance(source.position, destination.position);
            
            // Calculate risk
            int borderCrossings = 1;
            float riskLevel = 0.2f + borderCrossings * 0.15f;

            float baseReward = good.basePrice * quantity * smuggleRewardMultiplier;

            return new SmugglingMission
            {
                missionId = Guid.NewGuid().ToString(),
                title = $"Smuggle {good.goodName} past {GetFactionName(destination.factionId)} border",
                description = $"Deliver {quantity} units of {good.goodName} to {destination.stationName} without getting caught. This is illegal in {GetFactionName(destination.factionId)} space.",
                type = SmugglingType.ContrabandDelivery,
                sourceStationId = source.stationId,
                sourceFactionId = source.factionId,
                destinationStationId = destination.stationId,
                destinationFactionId = destination.factionId,
                borderCrossings = new List<string> { destination.factionId },
                distance = distance,
                contrabandId = good.goodId,
                contrabandName = good.goodName,
                quantity = quantity,
                riskLevel = riskLevel,
                creditReward = (int)baseReward,
                reputationPenaltyIfCaught = 50f,
                penaltyFaction = destination.factionId,
                state = MissionState.Available
            };
        }

        public bool AcceptTradeMission(string missionId)
        {
            var mission = AvailableTradeMissions.Find(m => m.missionId == missionId);
            if (mission == null || mission.state != MissionState.Available) return false;

            if (ActiveTradeMission != null)
            {
                Debug.LogWarning("[Trade] Already have active trade mission!");
                return false;
            }

            mission.state = MissionState.Accepted;
            mission.acceptedAt = DateTime.UtcNow;
            ActiveTradeMission = mission;
            AvailableTradeMissions.Remove(mission);

            OnTradeMissionAccepted?.Invoke(mission);
            Debug.Log($"[Trade] Accepted mission: {mission.title}");
            return true;
        }

        public bool AcceptSmugglingMission(string missionId)
        {
            var mission = AvailableSmugglingMissions.Find(m => m.missionId == missionId);
            if (mission == null || mission.state != MissionState.Available) return false;

            if (ActiveSmugglingMission != null)
            {
                Debug.LogWarning("[Trade] Already have active smuggling mission!");
                return false;
            }

            mission.state = MissionState.Accepted;
            ActiveSmugglingMission = mission;
            AvailableSmugglingMissions.Remove(mission);

            OnSmugglingMissionAccepted?.Invoke(mission);
            Debug.Log($"[Trade] Accepted smuggling mission: {mission.title}");
            return true;
        }

        public void CompleteTradeMission()
        {
            if (ActiveTradeMission == null) return;

            // Check if player has required cargo at destination
            var cargo = PlayerCargo.Find(c => c.goodId == ActiveTradeMission.requiredGoodId);
            if (cargo == null || cargo.quantity < ActiveTradeMission.requiredQuantity)
            {
                Debug.LogWarning("[Trade] Missing required cargo!");
                return;
            }

            // Remove cargo
            cargo.quantity -= ActiveTradeMission.requiredQuantity;
            if (cargo.quantity <= 0) PlayerCargo.Remove(cargo);

            ActiveTradeMission.state = MissionState.Completed;

            // Give rewards
            // Would add credits, XP, reputation here

            OnTradeMissionCompleted?.Invoke(ActiveTradeMission, true);
            OnReputationChanged?.Invoke(ActiveTradeMission.reputationFaction, ActiveTradeMission.reputationReward);

            Debug.Log($"[Trade] Completed mission! Reward: {ActiveTradeMission.creditReward}");
            ActiveTradeMission = null;
        }

        public void CompleteSmugglingMission()
        {
            if (ActiveSmugglingMission == null) return;

            // Check cargo
            var cargo = PlayerCargo.Find(c => c.goodId == ActiveSmugglingMission.contrabandId);
            if (cargo == null || cargo.quantity < ActiveSmugglingMission.quantity)
            {
                Debug.LogWarning("[Trade] Missing contraband!");
                return;
            }

            // Remove cargo
            cargo.quantity -= ActiveSmugglingMission.quantity;
            if (cargo.quantity <= 0) PlayerCargo.Remove(cargo);

            ActiveSmugglingMission.state = MissionState.Completed;

            OnSmugglingMissionCompleted?.Invoke(ActiveSmugglingMission, true);
            Debug.Log($"[Trade] Smuggling complete! Reward: {ActiveSmugglingMission.creditReward}");
            ActiveSmugglingMission = null;
        }

        private void CheckMissionTimers()
        {
            if (ActiveTradeMission != null && ActiveTradeMission.timeLimit > 0)
            {
                float elapsed = (float)(DateTime.UtcNow - ActiveTradeMission.acceptedAt.Value).TotalSeconds;
                if (elapsed > ActiveTradeMission.timeLimit)
                {
                    ActiveTradeMission.state = MissionState.Failed;
                    OnTradeMissionCompleted?.Invoke(ActiveTradeMission, false);
                    Debug.Log("[Trade] Mission failed - time expired!");
                    ActiveTradeMission = null;
                }
            }
        }

        #endregion

        #region Border Crossing & Smuggling

        public void ProcessBorderCrossing(Vector3 position, string fromFaction, string toFaction)
        {
            var crossing = new BorderCrossingEvent
            {
                crossingPoint = position,
                fromFaction = fromFaction,
                toFaction = toFaction,
                timestamp = DateTime.UtcNow
            };

            // Check for contraband
            bool hasContraband = PlayerCargo.Any(c => c.isContraband);
            
            // Determine if scanned
            var toFactionDef = WorldFactionSystem.Instance?.GetFaction(toFaction);
            bool hasPatrols = toFactionDef?.traits.hasBorderControl ?? false;

            if (hasPatrols)
            {
                float scanChance = baseDetectionChance;
                if (ActiveSmugglingMission != null)
                {
                    scanChance += ActiveSmugglingMission.riskLevel;
                    ActiveSmugglingMission.bordersCrossed++;
                }

                crossing.wasScanned = UnityEngine.Random.value < scanChance;

                if (crossing.wasScanned && hasContraband)
                {
                    // Contraband check
                    float detectionChance = 0.5f; // Base 50% if scanned
                    
                    // Modifiers could include: ship type, skills, bribes, etc.
                    crossing.detectionChance = detectionChance;
                    crossing.contrabandDetected = UnityEngine.Random.value < detectionChance;

                    if (crossing.contrabandDetected)
                    {
                        HandleContrabandDetected(toFaction);
                    }
                    else
                    {
                        Debug.Log("[Trade] Scanned but contraband not detected! Close call!");
                    }
                }
                else if (crossing.wasScanned)
                {
                    Debug.Log("[Trade] Scanned at border - all clear!");
                }
            }

            if (ActiveSmugglingMission != null)
            {
                ActiveSmugglingMission.timesScanned += crossing.wasScanned ? 1 : 0;
            }

            OnBorderCrossing?.Invoke(crossing);
        }

        private void HandleContrabandDetected(string factionId)
        {
            Debug.LogWarning("[Trade] CONTRABAND DETECTED! You're being pursued!");

            if (ActiveSmugglingMission != null)
            {
                ActiveSmugglingMission.hasBeenDetected = true;
            }

            // Reputation penalty
            OnReputationChanged?.Invoke(factionId, -50f);

            // Could trigger patrol spawn, wanted status, etc.
            OnSmugglingDetected?.Invoke(true);
        }

        public void EscapePursuit()
        {
            if (ActiveSmugglingMission != null && ActiveSmugglingMission.hasBeenDetected)
            {
                Debug.Log("[Trade] Escaped pursuit!");
                OnSmugglingDetected?.Invoke(false);
            }
        }

        public bool HasContrabandForFaction(string factionId)
        {
            foreach (var cargo in PlayerCargo)
            {
                var good = GetGoodDefinition(cargo.goodId);
                if (good != null && good.illegalInFactions != null && good.illegalInFactions.Contains(factionId))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion
    }
}
