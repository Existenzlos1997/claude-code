using UnityEngine;

namespace EarthUnderFreelancer.Data
{
    /// <summary>
    /// ScriptableObject defining tradeable items
    /// </summary>
    [CreateAssetMenu(fileName = "NewItemData", menuName = "EarthUnder/Item Data")]
    public class ItemData : ScriptableObject
    {
        [Header("Identity")]
        public string itemId;
        public string itemName;
        [TextArea(2, 4)]
        public string description;
        public Sprite icon;
        public ItemCategory category;
        public ItemRarity rarity;

        [Header("Properties")]
        public float mass = 1f;
        public bool isStackable = true;
        public int maxStack = 999;
        public bool isConsumable = false;
        public bool isIllegal = false;

        [Header("Economy")]
        public int basePrice = 100;
        public float priceVariance = 0.2f;

        [Header("Equipment")]
        public EquipmentSlot equipSlot = EquipmentSlot.None;
        public StatModifier[] statModifiers;

        [Header("Crafting")]
        public CraftingIngredient[] craftingRecipe;
    }

    [System.Serializable]
    public class StatModifier
    {
        public StatType statType;
        public float value;
        public bool isPercentage;
    }

    [System.Serializable]
    public class CraftingIngredient
    {
        public string itemId;
        public int quantity;
    }

    public enum ItemCategory
    {
        Commodity,
        Resource,
        Component,
        Weapon,
        Shield,
        Engine,
        Module,
        Consumable,
        Blueprint
    }

    public enum ItemRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    public enum EquipmentSlot
    {
        None,
        PrimaryWeapon,
        SecondaryWeapon,
        Shield,
        Engine,
        Utility
    }

    public enum StatType
    {
        MaxHull,
        MaxShield,
        ShieldRegen,
        MaxSpeed,
        Acceleration,
        TurnRate,
        CargoCapacity,
        WeaponDamage,
        WeaponRange
    }
}
