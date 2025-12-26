using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Crafting
{
    [System.Serializable]
    public class CraftingRecipe
    {
        public string recipeName;
        public Dictionary<string, int> requiredItems = new Dictionary<string, int>();
        public string resultItem;
        public int resultQuantity = 1;
        public float craftTime = 5f;
        public int requiredSkillLevel = 0;
    }
    
    public enum ItemQuality { Normal, Rare, Epic, Legendary }
    
    public class CraftingSystem : MonoBehaviour
    {
        private static CraftingSystem instance;
        public static CraftingSystem Instance => instance;
        
        public Dictionary<string, CraftingRecipe> recipes = new Dictionary<string, CraftingRecipe>();
        public Dictionary<string, int> inventory = new Dictionary<string, int>();
        public int craftingSkill = 1;
        
        private void Awake() { if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); } else { Destroy(gameObject); } }
        
        public bool CanCraft(string recipeName)
        {
            if (!recipes.TryGetValue(recipeName, out CraftingRecipe recipe)) return false;
            if (craftingSkill < recipe.requiredSkillLevel) return false;
            
            foreach (var requirement in recipe.requiredItems)
            {
                if (!inventory.ContainsKey(requirement.Key) || inventory[requirement.Key] < requirement.Value)
                    return false;
            }
            return true;
        }
        
        public void Craft(string recipeName)
        {
            if (!CanCraft(recipeName)) return;
            
            CraftingRecipe recipe = recipes[recipeName];
            
            // Consume materials
            foreach (var requirement in recipe.requiredItems)
            {
                inventory[requirement.Key] -= requirement.Value;
            }
            
            // Produce result
            if (!inventory.ContainsKey(recipe.resultItem)) inventory[recipe.resultItem] = 0;
            inventory[recipe.resultItem] += recipe.resultQuantity;
            
            // Gain skill
            craftingSkill += 1;
        }
        
        public void AddItem(string itemName, int quantity)
        {
            if (!inventory.ContainsKey(itemName)) inventory[itemName] = 0;
            inventory[itemName] += quantity;
        }
        
        public ItemQuality DetermineQuality()
        {
            float roll = Random.value;
            if (roll < 0.01f) return ItemQuality.Legendary;
            if (roll < 0.10f) return ItemQuality.Epic;
            if (roll < 0.30f) return ItemQuality.Rare;
            return ItemQuality.Normal;
        }
    }
}
