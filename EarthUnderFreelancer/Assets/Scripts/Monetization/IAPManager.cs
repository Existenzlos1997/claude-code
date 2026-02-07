using UnityEngine;
using System;
using System.Collections.Generic;
using EarthUnderFreelancer.Core;

namespace EarthUnderFreelancer.Monetization
{
    /// <summary>
    /// Manages In-App Purchases
    /// </summary>
    public class IAPManager : MonoBehaviour
    {
        public static IAPManager Instance { get; private set; }

        [Header("Products")]
        [SerializeField] private List<IAPProduct> products = new List<IAPProduct>();

        private bool isInitialized = false;

        public event Action<string> OnPurchaseSuccess;
        public event Action<string, string> OnPurchaseFailed;
        public event Action OnRestoreComplete;

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

        private void Start()
        {
            InitializeProducts();
            InitializeIAP();
        }

        private void InitializeProducts()
        {
            if (products.Count == 0)
            {
                // Credits packs
                products.Add(new IAPProduct { productId = "credits_small", productName = "500 Credits", productType = IAPProductType.Consumable, priceString = "$0.99", creditsAmount = 500 });
                products.Add(new IAPProduct { productId = "credits_medium", productName = "1500 Credits", productType = IAPProductType.Consumable, priceString = "$2.99", creditsAmount = 1500 });
                products.Add(new IAPProduct { productId = "credits_large", productName = "5000 Credits", productType = IAPProductType.Consumable, priceString = "$9.99", creditsAmount = 5000 });
                products.Add(new IAPProduct { productId = "credits_mega", productName = "15000 Credits", productType = IAPProductType.Consumable, priceString = "$24.99", creditsAmount = 15000 });

                // Premium currency
                products.Add(new IAPProduct { productId = "gems_small", productName = "50 Gems", productType = IAPProductType.Consumable, priceString = "$0.99", premiumAmount = 50 });
                products.Add(new IAPProduct { productId = "gems_medium", productName = "150 Gems", productType = IAPProductType.Consumable, priceString = "$2.99", premiumAmount = 150 });
                products.Add(new IAPProduct { productId = "gems_large", productName = "500 Gems", productType = IAPProductType.Consumable, priceString = "$9.99", premiumAmount = 500 });

                // Subscriptions
                products.Add(new IAPProduct { productId = "premium_weekly", productName = "Premium Week", productType = IAPProductType.Subscription, priceString = "$1.99", isPremium = true });
                products.Add(new IAPProduct { productId = "premium_monthly", productName = "Premium Month", productType = IAPProductType.Subscription, priceString = "$4.99", isPremium = true });

                // One-time purchases
                products.Add(new IAPProduct { productId = "remove_ads", productName = "Remove Ads", productType = IAPProductType.NonConsumable, priceString = "$4.99" });
                products.Add(new IAPProduct { productId = "starter_pack", productName = "Starter Pack", productType = IAPProductType.NonConsumable, priceString = "$9.99", creditsAmount = 5000, premiumAmount = 100 });
            }
        }

        public void InitializeIAP()
        {
            // Unity IAP initialization would go here
            // var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            // foreach (var product in products)
            // {
            //     builder.AddProduct(product.productId, product.productType);
            // }
            // UnityPurchasing.Initialize(this, builder);
            
            Debug.Log("Initializing IAP");
            isInitialized = true;
        }

        public void PurchaseProduct(string productId)
        {
            if (!isInitialized)
            {
                Debug.LogWarning("IAP not initialized");
                OnPurchaseFailed?.Invoke(productId, "IAP not initialized");
                return;
            }

            IAPProduct product = GetProduct(productId);
            if (product == null)
            {
                OnPurchaseFailed?.Invoke(productId, "Product not found");
                return;
            }

            Debug.Log($"Initiating purchase: {product.productName}");
            
            // Unity IAP purchase would go here
            // UnityPurchasing.BuyProduct(productId);
            
            // Simulate successful purchase for testing
            ProcessPurchase(productId);
        }

        private void ProcessPurchase(string productId)
        {
            IAPProduct product = GetProduct(productId);
            if (product == null) return;

            // Grant rewards
            if (GameManager.Instance != null)
            {
                if (product.creditsAmount > 0)
                {
                    GameManager.Instance.playerData.credits += product.creditsAmount;
                }
                if (product.premiumAmount > 0)
                {
                    GameManager.Instance.playerData.premiumCurrency += product.premiumAmount;
                }
                
                GameManager.Instance.SaveGame();
            }

            // Handle special products
            switch (productId)
            {
                case "remove_ads":
                    PlayerPrefs.SetInt("AdsRemoved", 1);
                    PlayerPrefs.Save();
                    break;
                case "starter_pack":
                    // Grant starter pack items
                    break;
            }

            Debug.Log($"Purchase successful: {product.productName}");
            OnPurchaseSuccess?.Invoke(productId);
            EventManager.TriggerEvent(GameEvents.PURCHASE_COMPLETED, product);
            AudioManager.Instance?.PlayPurchaseSuccess();
        }

        public void RestorePurchases()
        {
            if (!isInitialized)
            {
                Debug.LogWarning("IAP not initialized");
                return;
            }

            Debug.Log("Restoring purchases...");
            
            // Unity IAP restore would go here
            // Only for non-consumable and subscription products on iOS
            
            OnRestoreComplete?.Invoke();
        }

        public IAPProduct GetProduct(string productId)
        {
            foreach (var product in products)
            {
                if (product.productId == productId)
                    return product;
            }
            return null;
        }

        public List<IAPProduct> GetAllProducts() => new List<IAPProduct>(products);

        public List<IAPProduct> GetProductsByType(IAPProductType type)
        {
            List<IAPProduct> filtered = new List<IAPProduct>();
            foreach (var product in products)
            {
                if (product.productType == type)
                    filtered.Add(product);
            }
            return filtered;
        }

        public bool HasPurchased(string productId)
        {
            // Check PlayerPrefs for non-consumables
            return PlayerPrefs.GetInt($"Purchased_{productId}", 0) == 1;
        }

        public bool AreAdsRemoved()
        {
            return PlayerPrefs.GetInt("AdsRemoved", 0) == 1;
        }
    }

    [Serializable]
    public class IAPProduct
    {
        public string productId;
        public string productName;
        public string description;
        public IAPProductType productType;
        public string priceString;
        public Sprite icon;
        
        public int creditsAmount = 0;
        public int premiumAmount = 0;
        public bool isPremium = false;
    }

    public enum IAPProductType
    {
        Consumable,
        NonConsumable,
        Subscription
    }
}
