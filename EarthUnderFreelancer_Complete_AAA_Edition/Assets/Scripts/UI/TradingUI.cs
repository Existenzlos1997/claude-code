using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace EarthUnderFreelancer.UI
{
    /// <summary>
    /// Trading UI for space stations
    /// </summary>
    public class TradingUI : MonoBehaviour
    {
        [Header("Station Info")]
        [SerializeField] private TextMeshProUGUI stationNameText;
        [SerializeField] private TextMeshProUGUI factionText;

        [Header("Trading Lists")]
        [SerializeField] private Transform buyListParent;
        [SerializeField] private Transform sellListParent;
        [SerializeField] private GameObject tradeItemPrefab;

        [Header("Selected Item")]
        [SerializeField] private GameObject selectedItemPanel;
        [SerializeField] private TextMeshProUGUI selectedItemNameText;
        [SerializeField] private TextMeshProUGUI selectedItemPriceText;
        [SerializeField] private TextMeshProUGUI selectedItemQuantityText;
        [SerializeField] private Slider quantitySlider;
        [SerializeField] private Button buyButton;
        [SerializeField] private Button sellButton;

        [Header("Player Info")]
        [SerializeField] private TextMeshProUGUI creditsText;
        [SerializeField] private TextMeshProUGUI cargoSpaceText;

        private Systems.SpaceStation currentStation;
        private string selectedItemId;
        private int selectedQuantity = 1;
        private bool isBuying = true;
        private List<GameObject> tradeSlots = new List<GameObject>();

        private void OnEnable()
        {
            RefreshUI();

            if (Systems.EconomyManager.Instance != null)
            {
                Systems.EconomyManager.Instance.OnCreditsChanged += OnCreditsChanged;
            }
        }

        private void OnDisable()
        {
            if (Systems.EconomyManager.Instance != null)
            {
                Systems.EconomyManager.Instance.OnCreditsChanged -= OnCreditsChanged;
            }
        }

        public void SetStation(Systems.SpaceStation station)
        {
            currentStation = station;
            RefreshUI();
        }

        private void RefreshUI()
        {
            ClearTradeSlots();

            if (currentStation != null)
            {
                if (stationNameText != null) stationNameText.text = currentStation.StationName;
                if (factionText != null) factionText.text = currentStation.FactionId;

                // Populate buy list from station inventory
                foreach (var item in currentStation.AvailableItems)
                {
                    CreateBuySlot(item);
                }
            }

            // Populate sell list from player inventory
            if (Systems.InventoryManager.Instance != null)
            {
                foreach (var item in Systems.InventoryManager.Instance.CargoItems)
                {
                    CreateSellSlot(item);
                }
            }

            UpdatePlayerInfo();
        }

        private void ClearTradeSlots()
        {
            foreach (var slot in tradeSlots)
            {
                Destroy(slot);
            }
            tradeSlots.Clear();
        }

        private void CreateBuySlot(Systems.TradeItem item)
        {
            if (tradeItemPrefab == null || buyListParent == null) return;

            GameObject slot = Instantiate(tradeItemPrefab, buyListParent);
            tradeSlots.Add(slot);

            TextMeshProUGUI nameText = slot.transform.Find("ItemName")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI priceText = slot.transform.Find("Price")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI stockText = slot.transform.Find("Stock")?.GetComponent<TextMeshProUGUI>();

            int price = Systems.EconomyManager.Instance != null 
                ? Systems.EconomyManager.Instance.GetItemPrice(item.itemId, item.basePrice, currentStation?.FactionId)
                : item.basePrice;

            if (nameText != null) nameText.text = item.itemName;
            if (priceText != null) priceText.text = $"${price}";
            if (stockText != null) stockText.text = $"Stock: {item.stock}";

            Button btn = slot.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() => SelectBuyItem(item.itemId, price));
            }
        }

        private void CreateSellSlot(Systems.InventorySlot item)
        {
            if (tradeItemPrefab == null || sellListParent == null) return;

            GameObject slot = Instantiate(tradeItemPrefab, sellListParent);
            tradeSlots.Add(slot);

            TextMeshProUGUI nameText = slot.transform.Find("ItemName")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI priceText = slot.transform.Find("Price")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI quantityText = slot.transform.Find("Stock")?.GetComponent<TextMeshProUGUI>();

            int basePrice = 100; // Would need ItemData lookup
            int sellPrice = Systems.EconomyManager.Instance != null
                ? Systems.EconomyManager.Instance.GetSellPrice(item.itemId, basePrice, currentStation?.FactionId)
                : Mathf.RoundToInt(basePrice * 0.7f);

            if (nameText != null) nameText.text = item.itemId;
            if (priceText != null) priceText.text = $"${sellPrice}";
            if (quantityText != null) quantityText.text = $"x{item.quantity}";

            Button btn = slot.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() => SelectSellItem(item.itemId, sellPrice, item.quantity));
            }
        }

        private void SelectBuyItem(string itemId, int price)
        {
            selectedItemId = itemId;
            isBuying = true;
            selectedQuantity = 1;

            if (selectedItemPanel != null) selectedItemPanel.SetActive(true);
            if (selectedItemNameText != null) selectedItemNameText.text = itemId;
            if (selectedItemPriceText != null) selectedItemPriceText.text = $"${price}";
            if (buyButton != null) buyButton.gameObject.SetActive(true);
            if (sellButton != null) sellButton.gameObject.SetActive(false);

            UpdateQuantityUI(price);
        }

        private void SelectSellItem(string itemId, int price, int maxQuantity)
        {
            selectedItemId = itemId;
            isBuying = false;
            selectedQuantity = 1;

            if (selectedItemPanel != null) selectedItemPanel.SetActive(true);
            if (selectedItemNameText != null) selectedItemNameText.text = itemId;
            if (selectedItemPriceText != null) selectedItemPriceText.text = $"${price}";
            if (buyButton != null) buyButton.gameObject.SetActive(false);
            if (sellButton != null) sellButton.gameObject.SetActive(true);

            if (quantitySlider != null) quantitySlider.maxValue = maxQuantity;

            UpdateQuantityUI(price);
        }

        private void UpdateQuantityUI(int unitPrice)
        {
            if (selectedItemQuantityText != null)
            {
                selectedItemQuantityText.text = $"x{selectedQuantity} = ${unitPrice * selectedQuantity}";
            }
        }

        private int GetBasePrice(string itemId)
        {
            // Try to load ItemData from Resources
            var itemData = Resources.Load<Data.ItemData>($"Data/Items/{itemId}");
            if (itemData != null)
            {
                return itemData.basePrice;
            }

            // Check station available items
            if (currentStation != null)
            {
                foreach (var item in currentStation.AvailableItems)
                {
                    if (item.itemId == itemId)
                    {
                        return item.basePrice;
                    }
                }
            }

            // Default fallback price based on item category
            return 100;
        }

        public void OnQuantityChanged(float value)
        {
            selectedQuantity = Mathf.Max(1, Mathf.RoundToInt(value));
            // Recalculate price display
        }

        public void OnBuyClicked()
        {
            if (string.IsNullOrEmpty(selectedItemId)) return;

            int basePrice = GetBasePrice(item.itemId);
            for (int i = 0; i < selectedQuantity; i++)
            {
                if (!Systems.EconomyManager.Instance.BuyItem(selectedItemId, basePrice, currentStation?.FactionId))
                {
                    UIManager.Instance?.ShowNotification("Cannot afford item!", NotificationType.Error);
                    break;
                }
            }

            RefreshUI();
        }

        public void OnSellClicked()
        {
            if (string.IsNullOrEmpty(selectedItemId)) return;

            int basePrice = GetBasePrice(item.itemId);
            for (int i = 0; i < selectedQuantity; i++)
            {
                if (!Systems.EconomyManager.Instance.SellItem(selectedItemId, basePrice, currentStation?.FactionId))
                {
                    UIManager.Instance?.ShowNotification("Item not in inventory!", NotificationType.Error);
                    break;
                }
            }

            RefreshUI();
        }

        private void UpdatePlayerInfo()
        {
            if (creditsText != null && Systems.EconomyManager.Instance != null)
            {
                creditsText.text = $"Credits: ${Systems.EconomyManager.Instance.PlayerCredits:N0}";
            }

            if (cargoSpaceText != null && Systems.InventoryManager.Instance != null)
            {
                cargoSpaceText.text = $"Cargo: {Systems.InventoryManager.Instance.UsedCargoSpace}/{Systems.InventoryManager.Instance.MaxCargoCapacity}";
            }
        }

        private void OnCreditsChanged(int newCredits)
        {
            UpdatePlayerInfo();
        }

        public void OnCloseClicked()
        {
            UIManager.Instance?.ShowScreen(UIScreen.GameHUD);
        }
    }
}
