using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Systems
{
    /// <summary>
    /// Manages the game economy, prices, and trading
    /// </summary>
    public class EconomyManager : MonoBehaviour
    {
        public static EconomyManager Instance { get; private set; }

        [Header("Economy Settings")]
        [SerializeField] private float priceUpdateInterval = 60f;
        [SerializeField] private float maxPriceFluctuation = 0.3f;
        [SerializeField] private float supplyDemandInfluence = 0.1f;

        [Header("Player Economy")]
        [SerializeField] private int playerCredits = 10000;
        [SerializeField] private int totalEarned = 0;
        [SerializeField] private int totalSpent = 0;

        private Dictionary<string, float> itemPriceModifiers = new Dictionary<string, float>();
        private float lastPriceUpdate;

        public int PlayerCredits => playerCredits;
        public int TotalEarned => totalEarned;
        public int TotalSpent => totalSpent;

        public event System.Action<int> OnCreditsChanged;
        public event System.Action OnPricesUpdated;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Update()
        {
            if (Time.time - lastPriceUpdate > priceUpdateInterval)
            {
                UpdateMarketPrices();
                lastPriceUpdate = Time.time;
            }
        }

        private void UpdateMarketPrices()
        {
            List<string> items = new List<string>(itemPriceModifiers.Keys);
            foreach (string itemId in items)
            {
                float fluctuation = Random.Range(-maxPriceFluctuation, maxPriceFluctuation);
                float currentMod = itemPriceModifiers[itemId];
                itemPriceModifiers[itemId] = Mathf.Clamp(currentMod + fluctuation * 0.1f, 0.5f, 2f);
            }

            OnPricesUpdated?.Invoke();
        }

        public bool CanAfford(int amount)
        {
            return playerCredits >= amount;
        }

        public bool SpendCredits(int amount)
        {
            if (!CanAfford(amount)) return false;

            playerCredits -= amount;
            totalSpent += amount;
            OnCreditsChanged?.Invoke(playerCredits);
            return true;
        }

        public void AddCredits(int amount)
        {
            playerCredits += amount;
            totalEarned += amount;
            OnCreditsChanged?.Invoke(playerCredits);
        }

        public void SetCredits(int amount)
        {
            playerCredits = amount;
            OnCreditsChanged?.Invoke(playerCredits);
        }

        public int GetItemPrice(string itemId, int basePrice, string stationFaction = "")
        {
            float modifier = 1f;

            if (itemPriceModifiers.TryGetValue(itemId, out float priceMod))
            {
                modifier *= priceMod;
            }
            else
            {
                itemPriceModifiers[itemId] = 1f;
            }

            if (!string.IsNullOrEmpty(stationFaction) && FactionManager.Instance != null)
            {
                modifier *= FactionManager.Instance.GetPriceModifier(stationFaction);
            }

            return Mathf.RoundToInt(basePrice * modifier);
        }

        public int GetSellPrice(string itemId, int basePrice, string stationFaction = "")
        {
            int buyPrice = GetItemPrice(itemId, basePrice, stationFaction);
            return Mathf.RoundToInt(buyPrice * 0.7f);
        }

        public bool BuyItem(string itemId, int basePrice, string stationFaction = "")
        {
            int price = GetItemPrice(itemId, basePrice, stationFaction);
            if (!SpendCredits(price)) return false;

            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.AddItem(itemId);
            }

            if (itemPriceModifiers.ContainsKey(itemId))
            {
                itemPriceModifiers[itemId] += supplyDemandInfluence;
            }

            return true;
        }

        public bool SellItem(string itemId, int basePrice, string stationFaction = "")
        {
            if (InventoryManager.Instance != null)
            {
                if (!InventoryManager.Instance.RemoveItem(itemId)) return false;
            }

            int price = GetSellPrice(itemId, basePrice, stationFaction);
            AddCredits(price);

            if (itemPriceModifiers.ContainsKey(itemId))
            {
                itemPriceModifiers[itemId] -= supplyDemandInfluence;
                itemPriceModifiers[itemId] = Mathf.Max(itemPriceModifiers[itemId], 0.5f);
            }

            return true;
        }

        public float GetPriceModifier(string itemId)
        {
            if (itemPriceModifiers.TryGetValue(itemId, out float mod))
                return mod;
            return 1f;
        }

        public void ResetPrices()
        {
            itemPriceModifiers.Clear();
        }

        public Dictionary<string, float> GetAllPriceModifiers()
        {
            return new Dictionary<string, float>(itemPriceModifiers);
        }
    }
}
