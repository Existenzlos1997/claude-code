using UnityEngine;
using System;
using System.Collections.Generic;
using EarthUnderFreelancer.Core;

namespace EarthUnderFreelancer.Trading
{
    /// <summary>
    /// Manages the trading system and economy
    /// </summary>
    public class TradingManager : MonoBehaviour
    {
        public static TradingManager Instance { get; private set; }

        [Header("Market Data")]
        [SerializeField] private List<MarketItem> marketInventory = new List<MarketItem>();
        [SerializeField] private List<TradingStation> tradingStations = new List<TradingStation>();

        [Header("Settings")]
        [SerializeField] private float priceFluctuation = 0.2f;
        [SerializeField] private float updateInterval = 300f;

        private float lastUpdateTime;
        private TradingStation currentStation;

        public TradingStation CurrentStation => currentStation;
        public event Action<MarketItem> OnItemPurchased;
        public event Action<MarketItem> OnItemSold;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start() => InitializeMarket();

        private void Update()
        {
            if (Time.time - lastUpdateTime >= updateInterval)
            {
                UpdateMarketPrices();
                lastUpdateTime = Time.time;
            }
        }

        private void InitializeMarket()
        {
            if (marketInventory.Count == 0) CreateDefaultMarketItems();
        }

        private void CreateDefaultMarketItems()
        {
            marketInventory.Add(new MarketItem { itemId = "ore_iron", itemName = "Iron Ore", itemType = TradeItemType.Resource, basePrice = 50, currentPrice = 50, supply = 100, demand = 1f });
            marketInventory.Add(new MarketItem { itemId = "ore_titanium", itemName = "Titanium Ore", itemType = TradeItemType.Resource, basePrice = 150, currentPrice = 150, supply = 50, demand = 1.2f });
            marketInventory.Add(new MarketItem { itemId = "fuel_hydrogen", itemName = "Hydrogen Fuel", itemType = TradeItemType.Fuel, basePrice = 25, currentPrice = 25, supply = 200, demand = 0.9f });
            marketInventory.Add(new MarketItem { itemId = "weapon_laser_mk2", itemName = "Laser Cannon MK-II", itemType = TradeItemType.Weapon, basePrice = 5000, currentPrice = 5000, supply = 10, demand = 1.5f });
            marketInventory.Add(new MarketItem { itemId = "shield_advanced", itemName = "Advanced Shield Generator", itemType = TradeItemType.Equipment, basePrice = 8000, currentPrice = 8000, supply = 5, demand = 2f });
        }

        private void UpdateMarketPrices()
        {
            foreach (var item in marketInventory)
            {
                float supplyModifier = 1f / Mathf.Max(item.supply / 100f, 0.1f);
                float demandModifier = item.demand;
                float fluctuation = UnityEngine.Random.Range(-priceFluctuation, priceFluctuation);
                float priceModifier = supplyModifier * demandModifier * (1f + fluctuation);
                item.currentPrice = Mathf.RoundToInt(item.basePrice * priceModifier);
                item.currentPrice = Mathf.Clamp(item.currentPrice, item.basePrice / 2, item.basePrice * 3);
            }
        }

        public bool PurchaseItem(string itemId, int quantity = 1)
        {
            MarketItem item = GetMarketItem(itemId);
            if (item == null) return false;

            int totalCost = item.currentPrice * quantity;
            PlayerData playerData = GameManager.Instance?.playerData;

            if (playerData == null || playerData.credits < totalCost)
            {
                AudioManager.Instance?.PlayPurchaseFail();
                return false;
            }

            if (item.supply < quantity) return false;

            playerData.credits -= totalCost;
            item.supply -= quantity;
            item.demand += 0.1f * quantity;

            AddToPlayerInventory(item, quantity);

            OnItemPurchased?.Invoke(item);
            EventManager.TriggerEvent(GameEvents.ITEM_PURCHASED, item);
            AudioManager.Instance?.PlayPurchaseSuccess();
            GameManager.Instance?.SaveGame();
            return true;
        }

        public bool SellItem(string itemId, int quantity = 1)
        {
            PlayerData playerData = GameManager.Instance?.playerData;
            if (playerData == null) return false;

            InventoryItem playerItem = null;
            foreach (var inv in playerData.inventory)
            {
                if (inv.itemId == itemId && inv.quantity >= quantity)
                {
                    playerItem = inv;
                    break;
                }
            }

            if (playerItem == null) return false;

            MarketItem marketItem = GetMarketItem(itemId);
            int sellPrice = marketItem != null ? Mathf.RoundToInt(marketItem.currentPrice * 0.7f) : playerItem.value;
            int totalValue = sellPrice * quantity;

            playerItem.quantity -= quantity;
            if (playerItem.quantity <= 0) playerData.inventory.Remove(playerItem);

            playerData.credits += totalValue;

            if (marketItem != null)
            {
                marketItem.supply += quantity;
                marketItem.demand -= 0.05f * quantity;
                marketItem.demand = Mathf.Max(marketItem.demand, 0.5f);
            }

            OnItemSold?.Invoke(marketItem);
            EventManager.TriggerEvent(GameEvents.ITEM_SOLD, marketItem);
            AudioManager.Instance?.PlayPurchaseSuccess();
            GameManager.Instance?.SaveGame();
            return true;
        }

        private void AddToPlayerInventory(MarketItem marketItem, int quantity)
        {
            PlayerData playerData = GameManager.Instance?.playerData;
            if (playerData == null) return;

            foreach (var inv in playerData.inventory)
            {
                if (inv.itemId == marketItem.itemId)
                {
                    inv.quantity += quantity;
                    return;
                }
            }

            playerData.inventory.Add(new InventoryItem
            {
                itemId = marketItem.itemId,
                itemName = marketItem.itemName,
                itemType = ConvertToItemType(marketItem.itemType),
                quantity = quantity,
                value = marketItem.currentPrice
            });
        }

        private ItemType ConvertToItemType(TradeItemType tradeType)
        {
            switch (tradeType)
            {
                case TradeItemType.Weapon: return ItemType.Weapon;
                case TradeItemType.Equipment: return ItemType.Module;
                case TradeItemType.Resource: return ItemType.Resource;
                case TradeItemType.Consumable: return ItemType.Consumable;
                default: return ItemType.Resource;
            }
        }

        public MarketItem GetMarketItem(string itemId)
        {
            foreach (var item in marketInventory) if (item.itemId == itemId) return item;
            return null;
        }

        public List<MarketItem> GetMarketItems(TradeItemType? type = null)
        {
            if (type == null) return new List<MarketItem>(marketInventory);

            List<MarketItem> filtered = new List<MarketItem>();
            foreach (var item in marketInventory) if (item.itemType == type) filtered.Add(item);
            return filtered;
        }

        public void SetCurrentStation(TradingStation station)
        {
            currentStation = station;
            if (station != null) ApplyStationModifiers();
        }

        private void ApplyStationModifiers()
        {
            if (currentStation == null) return;

            foreach (var item in marketInventory)
            {
                float modifier = 1f;
                foreach (var specialty in currentStation.specialties)
                {
                    if (specialty == item.itemType) { modifier = currentStation.specialtyDiscount; break; }
                }
                item.stationModifier = modifier;
            }
        }
    }

    [Serializable]
    public class MarketItem
    {
        public string itemId;
        public string itemName;
        public string description;
        public Sprite icon;
        public TradeItemType itemType;
        
        public int basePrice;
        public int currentPrice;
        public int supply;
        public float demand = 1f;
        public float stationModifier = 1f;

        public int GetBuyPrice() => Mathf.RoundToInt(currentPrice * stationModifier);
        public int GetSellPrice() => Mathf.RoundToInt(currentPrice * 0.7f * stationModifier);
    }

    [Serializable]
    public class TradingStation
    {
        public string stationId;
        public string stationName;
        public string faction;
        public Vector3 position;
        
        public TradeItemType[] specialties;
        public float specialtyDiscount = 0.8f;
        
        public List<string> availableItems = new List<string>();
        public List<string> illegalItems = new List<string>();
    }

    public enum TradeItemType { Resource, Fuel, Weapon, Equipment, Consumable, Cosmetic, Contraband }
}
