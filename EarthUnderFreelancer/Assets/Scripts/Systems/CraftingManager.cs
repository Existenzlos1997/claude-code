using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Systems
{
    /// <summary>
    /// Crafting and upgrade system
    /// </summary>
    public class CraftingManager : MonoBehaviour
    {
        public static CraftingManager Instance { get; private set; }

        [Header("Crafting Recipes")]
        [SerializeField] private List<CraftingRecipe> recipes = new List<CraftingRecipe>();

        public event System.Action<string> OnItemCrafted;
        public event System.Action<string> OnCraftingFailed;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            InitializeDefaultRecipes();
        }

        private void InitializeDefaultRecipes()
        {
            // Basic weapon upgrade
            recipes.Add(new CraftingRecipe
            {
                recipeId = "upgrade_laser_mk2",
                resultItemId = "laser_cannon_mk2",
                resultQuantity = 1,
                requirements = new List<CraftingRequirement>
                {
                    new CraftingRequirement { itemId = "laser_cannon_mk1", quantity = 1 },
                    new CraftingRequirement { itemId = "energy_cell", quantity = 5 },
                    new CraftingRequirement { itemId = "advanced_circuits", quantity = 2 }
                },
                creditCost = 1000
            });

            // Shield generator
            recipes.Add(new CraftingRecipe
            {
                recipeId = "craft_shield_mk1",
                resultItemId = "shield_generator_mk1",
                resultQuantity = 1,
                requirements = new List<CraftingRequirement>
                {
                    new CraftingRequirement { itemId = "metal_plates", quantity = 10 },
                    new CraftingRequirement { itemId = "energy_cell", quantity = 3 },
                    new CraftingRequirement { itemId = "shield_emitter", quantity = 1 }
                },
                creditCost = 500
            });

            // Engine upgrade
            recipes.Add(new CraftingRecipe
            {
                recipeId = "upgrade_engine_mk2",
                resultItemId = "engine_mk2",
                resultQuantity = 1,
                requirements = new List<CraftingRequirement>
                {
                    new CraftingRequirement { itemId = "engine_mk1", quantity = 1 },
                    new CraftingRequirement { itemId = "fuel_injector", quantity = 2 },
                    new CraftingRequirement { itemId = "thruster_nozzle", quantity = 4 }
                },
                creditCost = 1500
            });

            // Missile pack
            recipes.Add(new CraftingRecipe
            {
                recipeId = "craft_missiles",
                resultItemId = "homing_missile",
                resultQuantity = 5,
                requirements = new List<CraftingRequirement>
                {
                    new CraftingRequirement { itemId = "explosives", quantity = 3 },
                    new CraftingRequirement { itemId = "guidance_chip", quantity = 1 },
                    new CraftingRequirement { itemId = "metal_plates", quantity = 2 }
                },
                creditCost = 200
            });
        }

        public bool CanCraft(string recipeId)
        {
            CraftingRecipe recipe = GetRecipe(recipeId);
            if (recipe == null) return false;

            // Check credits
            if (EconomyManager.Instance != null && !EconomyManager.Instance.CanAfford(recipe.creditCost))
                return false;

            // Check materials
            if (InventoryManager.Instance == null) return false;

            foreach (var req in recipe.requirements)
            {
                if (!InventoryManager.Instance.HasItem(req.itemId, req.quantity))
                    return false;
            }

            return true;
        }

        public bool Craft(string recipeId)
        {
            if (!CanCraft(recipeId))
            {
                OnCraftingFailed?.Invoke(recipeId);
                return false;
            }

            CraftingRecipe recipe = GetRecipe(recipeId);

            // Spend credits
            if (EconomyManager.Instance != null)
                EconomyManager.Instance.SpendCredits(recipe.creditCost);

            // Consume materials
            foreach (var req in recipe.requirements)
            {
                InventoryManager.Instance.RemoveItem(req.itemId, req.quantity);
            }

            // Add result
            InventoryManager.Instance.AddItem(recipe.resultItemId, recipe.resultQuantity);

            OnItemCrafted?.Invoke(recipe.resultItemId);
            return true;
        }

        public CraftingRecipe GetRecipe(string recipeId)
        {
            foreach (var recipe in recipes)
            {
                if (recipe.recipeId == recipeId)
                    return recipe;
            }
            return null;
        }

        public List<CraftingRecipe> GetAllRecipes()
        {
            return new List<CraftingRecipe>(recipes);
        }

        public List<CraftingRecipe> GetAvailableRecipes()
        {
            List<CraftingRecipe> available = new List<CraftingRecipe>();
            foreach (var recipe in recipes)
            {
                if (CanCraft(recipe.recipeId))
                    available.Add(recipe);
            }
            return available;
        }

        public void AddRecipe(CraftingRecipe recipe)
        {
            recipes.Add(recipe);
        }
    }

    [System.Serializable]
    public class CraftingRecipe
    {
        public string recipeId;
        public string resultItemId;
        public int resultQuantity;
        public List<CraftingRequirement> requirements = new List<CraftingRequirement>();
        public int creditCost;
        public bool isUnlocked = true;
    }

    [System.Serializable]
    public class CraftingRequirement
    {
        public string itemId;
        public int quantity;
    }
}
