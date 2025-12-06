using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Systems
{
    /// <summary>
    /// Player inventory system for items, equipment and cargo
    /// </summary>
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance { get; private set; }

        [Header("Inventory Settings")]
        [SerializeField] private int maxCargoCapacity = 100;
        [SerializeField] private int maxEquipmentSlots = 6;

        [Header("Current State")]
        [SerializeField] private List<InventorySlot> cargoItems = new List<InventorySlot>();
        [SerializeField] private EquipmentLoadout currentLoadout = new EquipmentLoadout();

        public int MaxCargoCapacity => maxCargoCapacity;
        public int UsedCargoSpace => CalculateUsedSpace();
        public int AvailableCargoSpace => maxCargoCapacity - UsedCargoSpace;
        public List<InventorySlot> CargoItems => cargoItems;
        public EquipmentLoadout CurrentLoadout => currentLoadout;

        public event System.Action OnInventoryChanged;
        public event System.Action OnEquipmentChanged;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private int CalculateUsedSpace()
        {
            int total = 0;
            foreach (var slot in cargoItems)
            {
                total += slot.quantity;
            }
            return total;
        }

        public bool AddItem(string itemId, int quantity = 1)
        {
            if (AvailableCargoSpace < quantity) return false;

            for (int i = 0; i < cargoItems.Count; i++)
            {
                if (cargoItems[i].itemId == itemId)
                {
                    cargoItems[i].quantity += quantity;
                    OnInventoryChanged?.Invoke();
                    return true;
                }
            }

            cargoItems.Add(new InventorySlot { itemId = itemId, quantity = quantity });
            OnInventoryChanged?.Invoke();
            return true;
        }

        public bool RemoveItem(string itemId, int quantity = 1)
        {
            for (int i = 0; i < cargoItems.Count; i++)
            {
                if (cargoItems[i].itemId == itemId)
                {
                    if (cargoItems[i].quantity < quantity) return false;

                    cargoItems[i].quantity -= quantity;
                    if (cargoItems[i].quantity <= 0)
                    {
                        cargoItems.RemoveAt(i);
                    }
                    OnInventoryChanged?.Invoke();
                    return true;
                }
            }
            return false;
        }

        public int GetItemCount(string itemId)
        {
            foreach (var slot in cargoItems)
            {
                if (slot.itemId == itemId) return slot.quantity;
            }
            return 0;
        }

        public bool HasItem(string itemId, int quantity = 1)
        {
            return GetItemCount(itemId) >= quantity;
        }

        public bool EquipItem(string itemId, EquipmentSlot slot)
        {
            if (!HasItem(itemId)) return false;

            string currentEquipped = GetEquippedItem(slot);
            if (!string.IsNullOrEmpty(currentEquipped))
            {
                UnequipItem(slot);
            }

            RemoveItem(itemId);
            SetEquippedItem(slot, itemId);
            OnEquipmentChanged?.Invoke();
            return true;
        }

        public bool UnequipItem(EquipmentSlot slot)
        {
            string itemId = GetEquippedItem(slot);
            if (string.IsNullOrEmpty(itemId)) return false;

            if (!AddItem(itemId)) return false;

            SetEquippedItem(slot, "");
            OnEquipmentChanged?.Invoke();
            return true;
        }

        private string GetEquippedItem(EquipmentSlot slot)
        {
            switch (slot)
            {
                case EquipmentSlot.PrimaryWeapon: return currentLoadout.primaryWeapon;
                case EquipmentSlot.SecondaryWeapon: return currentLoadout.secondaryWeapon;
                case EquipmentSlot.Shield: return currentLoadout.shield;
                case EquipmentSlot.Engine: return currentLoadout.engine;
                case EquipmentSlot.Utility: return currentLoadout.utility;
                default: return "";
            }
        }

        private void SetEquippedItem(EquipmentSlot slot, string itemId)
        {
            switch (slot)
            {
                case EquipmentSlot.PrimaryWeapon: currentLoadout.primaryWeapon = itemId; break;
                case EquipmentSlot.SecondaryWeapon: currentLoadout.secondaryWeapon = itemId; break;
                case EquipmentSlot.Shield: currentLoadout.shield = itemId; break;
                case EquipmentSlot.Engine: currentLoadout.engine = itemId; break;
                case EquipmentSlot.Utility: currentLoadout.utility = itemId; break;
            }
        }

        public void SetCargoCapacity(int capacity)
        {
            maxCargoCapacity = capacity;
        }

        public void ClearInventory()
        {
            cargoItems.Clear();
            OnInventoryChanged?.Invoke();
        }
    }

    [System.Serializable]
    public class InventorySlot
    {
        public string itemId;
        public int quantity;
    }

    [System.Serializable]
    public class EquipmentLoadout
    {
        public string primaryWeapon;
        public string secondaryWeapon;
        public string shield;
        public string engine;
        public string utility;
    }

    public enum EquipmentSlot
    {
        PrimaryWeapon,
        SecondaryWeapon,
        Shield,
        Engine,
        Utility
    }
}
