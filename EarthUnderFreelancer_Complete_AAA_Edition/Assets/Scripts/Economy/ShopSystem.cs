using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Economy
{
    // Iteration 17: Shop and trading system
    public class ShopSystem : MonoBehaviour
    {
        [System.Serializable]
        public class ShopItem
        {
            public string name;
            public int price;
            public string description;
        }
        
        private List<ShopItem> items = new List<ShopItem>();
        
        private void Start()
        {
            items.Add(new ShopItem { name = "Fuel Tank", price = 500, description = "Refuel your aircraft" });
            items.Add(new ShopItem { name = "Ammo Pack", price = 300, description = "Restock ammunition" });
            items.Add(new ShopItem { name = "Repair Kit", price = 1000, description = "Repair damage" });
        }
        
        public List<ShopItem> GetItems() => items;
    }
}
