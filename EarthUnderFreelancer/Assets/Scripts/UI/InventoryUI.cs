using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace EarthUnderFreelancer.UI
{
    /// <summary>
    /// Inventory UI screen
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform itemGrid;
        [SerializeField] private GameObject itemSlotPrefab;
        [SerializeField] private Transform equipmentSlotsParent;

        [Header("Item Details")]
        [SerializeField] private GameObject detailsPanel;
        [SerializeField] private Image itemIcon;
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private TextMeshProUGUI itemDescriptionText;
        [SerializeField] private TextMeshProUGUI itemStatsText;
        [SerializeField] private Button equipButton;
        [SerializeField] private Button dropButton;

        [Header("Stats")]
        [SerializeField] private TextMeshProUGUI cargoCapacityText;
        [SerializeField] private TextMeshProUGUI creditsText;

        private List<GameObject> itemSlots = new List<GameObject>();
        private Systems.InventorySlot selectedSlot;

        private void OnEnable()
        {
            RefreshInventory();

            if (Systems.InventoryManager.Instance != null)
            {
                Systems.InventoryManager.Instance.OnInventoryChanged += RefreshInventory;
            }
        }

        private void OnDisable()
        {
            if (Systems.InventoryManager.Instance != null)
            {
                Systems.InventoryManager.Instance.OnInventoryChanged -= RefreshInventory;
            }
        }

        public void RefreshInventory()
        {
            // Clear existing slots
            foreach (var slot in itemSlots)
            {
                Destroy(slot);
            }
            itemSlots.Clear();

            if (Systems.InventoryManager.Instance == null) return;

            // Create item slots
            foreach (var item in Systems.InventoryManager.Instance.CargoItems)
            {
                CreateItemSlot(item);
            }

            UpdateStats();
        }

        private void CreateItemSlot(Systems.InventorySlot inventorySlot)
        {
            if (itemSlotPrefab == null || itemGrid == null) return;

            GameObject slot = Instantiate(itemSlotPrefab, itemGrid);
            itemSlots.Add(slot);

            // Setup slot visuals
            TextMeshProUGUI nameText = slot.GetComponentInChildren<TextMeshProUGUI>();
            if (nameText != null)
            {
                nameText.text = $"{inventorySlot.itemId} x{inventorySlot.quantity}";
            }

            // Add click handler
            Button btn = slot.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() => SelectItem(inventorySlot));
            }
        }

        private void SelectItem(Systems.InventorySlot slot)
        {
            selectedSlot = slot;

            if (detailsPanel != null) detailsPanel.SetActive(true);
            if (itemNameText != null) itemNameText.text = slot.itemId;
            if (itemDescriptionText != null) itemDescriptionText.text = $"Quantity: {slot.quantity}";

            // Load item data for more details
            // ItemData data = Resources.Load<ItemData>($"Data/Items/{slot.itemId}");
        }

        private void UpdateStats()
        {
            if (Systems.InventoryManager.Instance == null) return;

            if (cargoCapacityText != null)
            {
                cargoCapacityText.text = $"Cargo: {Systems.InventoryManager.Instance.UsedCargoSpace}/{Systems.InventoryManager.Instance.MaxCargoCapacity}";
            }

            if (creditsText != null && Systems.EconomyManager.Instance != null)
            {
                creditsText.text = $"Credits: ${Systems.EconomyManager.Instance.PlayerCredits:N0}";
            }
        }

        public void OnEquipClicked()
        {
            if (selectedSlot == null) return;

            // Determine equipment slot based on item type
            // For now, use primary weapon as default
            Systems.InventoryManager.Instance?.EquipItem(selectedSlot.itemId, Systems.EquipmentSlot.PrimaryWeapon);
            RefreshInventory();
        }

        public void OnDropClicked()
        {
            if (selectedSlot == null) return;

            Systems.InventoryManager.Instance?.RemoveItem(selectedSlot.itemId, 1);
            RefreshInventory();

            if (detailsPanel != null) detailsPanel.SetActive(false);
            selectedSlot = null;
        }

        public void OnCloseClicked()
        {
            UIManager.Instance?.ShowScreen(UIScreen.GameHUD);
        }
    }
}
