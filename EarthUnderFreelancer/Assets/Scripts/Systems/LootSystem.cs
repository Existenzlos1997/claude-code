using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Systems
{
    /// <summary>
    /// Loot drop and collection system
    /// </summary>
    public class LootSystem : MonoBehaviour
    {
        public static LootSystem Instance { get; private set; }

        [Header("Loot Settings")]
        [SerializeField] private float lootAttractionRange = 30f;
        [SerializeField] private float lootAttractionSpeed = 50f;
        [SerializeField] private float lootLifetime = 60f;
        [SerializeField] private GameObject defaultLootPrefab;

        [Header("Drop Tables")]
        [SerializeField] private List<LootTable> lootTables = new List<LootTable>();

        private List<LootDrop> activeDrops = new List<LootDrop>();
        private Transform playerTransform;

        public event System.Action<string, int> OnLootCollected;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            // Find player
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
        }

        private void Update()
        {
            UpdateLootAttraction();
            CleanupExpiredLoot();
        }

        private void UpdateLootAttraction()
        {
            if (playerTransform == null) return;

            for (int i = activeDrops.Count - 1; i >= 0; i--)
            {
                LootDrop drop = activeDrops[i];
                if (drop == null || drop.gameObject == null)
                {
                    activeDrops.RemoveAt(i);
                    continue;
                }

                float distance = Vector3.Distance(drop.transform.position, playerTransform.position);

                if (distance < lootAttractionRange)
                {
                    // Attract loot towards player
                    Vector3 direction = (playerTransform.position - drop.transform.position).normalized;
                    float attractionStrength = 1f - (distance / lootAttractionRange);
                    drop.transform.position += direction * lootAttractionSpeed * attractionStrength * Time.deltaTime;

                    // Check for collection
                    if (distance < 5f)
                    {
                        CollectLoot(drop);
                        activeDrops.RemoveAt(i);
                    }
                }
            }
        }

        private void CleanupExpiredLoot()
        {
            for (int i = activeDrops.Count - 1; i >= 0; i--)
            {
                LootDrop drop = activeDrops[i];
                if (drop == null || Time.time - drop.spawnTime > lootLifetime)
                {
                    if (drop != null && drop.gameObject != null)
                    {
                        Destroy(drop.gameObject);
                    }
                    activeDrops.RemoveAt(i);
                }
            }
        }

        public void SpawnLoot(Vector3 position, string lootTableId)
        {
            LootTable table = GetLootTable(lootTableId);
            if (table == null) return;

            List<LootEntry> droppedItems = RollLoot(table);

            foreach (var entry in droppedItems)
            {
                SpawnLootDrop(position + Random.insideUnitSphere * 5f, entry.itemId, entry.quantity);
            }
        }

        public void SpawnLoot(Vector3 position, int difficultyLevel)
        {
            // Generate random loot based on difficulty
            int numDrops = Random.Range(1, 3 + difficultyLevel);
            
            string[] commonItems = { "iron_ore", "copper_ore", "fuel_cell", "scrap_metal" };
            string[] rareItems = { "gold_ore", "plasma_core", "advanced_circuit", "quantum_crystal" };
            
            for (int i = 0; i < numDrops; i++)
            {
                bool isRare = Random.value < 0.1f + (difficultyLevel * 0.05f);
                string[] itemPool = isRare ? rareItems : commonItems;
                string itemId = itemPool[Random.Range(0, itemPool.Length)];
                int quantity = Random.Range(1, 5 + difficultyLevel);
                
                SpawnLootDrop(position + Random.insideUnitSphere * 5f, itemId, quantity);
            }

            // Credit drops
            int creditDrop = Random.Range(50, 200) * (1 + difficultyLevel);
            SpawnCreditDrop(position, creditDrop);
        }

        private void SpawnLootDrop(Vector3 position, string itemId, int quantity)
        {
            GameObject lootObj;
            
            if (defaultLootPrefab != null)
            {
                lootObj = Instantiate(defaultLootPrefab, position, Random.rotation);
            }
            else
            {
                // Create simple loot object
                lootObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                lootObj.transform.position = position;
                lootObj.transform.localScale = Vector3.one * 0.5f;
                
                var renderer = lootObj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = Color.yellow;
                }
                
                // Remove default collider and add trigger
                var collider = lootObj.GetComponent<Collider>();
                if (collider != null) Destroy(collider);
            }

            lootObj.name = $"Loot_{itemId}";
            lootObj.tag = "Loot";

            // Add rigidbody for floating effect
            Rigidbody rb = lootObj.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.AddTorque(Random.insideUnitSphere * 2f, ForceMode.Impulse);

            LootDrop drop = lootObj.AddComponent<LootDrop>();
            drop.Initialize(itemId, quantity, false);

            activeDrops.Add(drop);
        }

        private void SpawnCreditDrop(Vector3 position, int amount)
        {
            GameObject lootObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            lootObj.transform.position = position + Random.insideUnitSphere * 3f;
            lootObj.transform.localScale = Vector3.one * 0.3f;
            
            var renderer = lootObj.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.green;
                renderer.material.SetColor("_EmissionColor", Color.green * 0.5f);
            }

            var collider = lootObj.GetComponent<Collider>();
            if (collider != null) Destroy(collider);

            lootObj.name = $"Credits_{amount}";
            lootObj.tag = "Loot";

            Rigidbody rb = lootObj.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.AddTorque(Random.insideUnitSphere * 3f, ForceMode.Impulse);

            LootDrop drop = lootObj.AddComponent<LootDrop>();
            drop.Initialize("credits", amount, true);

            activeDrops.Add(drop);
        }

        private void CollectLoot(LootDrop drop)
        {
            if (drop.isCredits)
            {
                if (EconomyManager.Instance != null)
                {
                    EconomyManager.Instance.AddCredits(drop.quantity);
                }
            }
            else
            {
                if (InventoryManager.Instance != null)
                {
                    InventoryManager.Instance.AddItem(drop.itemId, drop.quantity);
                }
            }

            OnLootCollected?.Invoke(drop.itemId, drop.quantity);
            
            // Play collection effect/sound
            // AudioManager.Instance?.PlaySFX("loot_collect");

            Destroy(drop.gameObject);
        }

        private LootTable GetLootTable(string tableId)
        {
            foreach (var table in lootTables)
            {
                if (table.tableId == tableId)
                    return table;
            }
            return null;
        }

        private List<LootEntry> RollLoot(LootTable table)
        {
            List<LootEntry> result = new List<LootEntry>();

            foreach (var entry in table.entries)
            {
                if (Random.value <= entry.dropChance)
                {
                    int qty = Random.Range(entry.minQuantity, entry.maxQuantity + 1);
                    result.Add(new LootEntry
                    {
                        itemId = entry.itemId,
                        quantity = qty
                    });
                }
            }

            return result;
        }

        public void SetPlayerTransform(Transform player)
        {
            playerTransform = player;
        }
    }

    public class LootDrop : MonoBehaviour
    {
        public string itemId;
        public int quantity;
        public bool isCredits;
        public float spawnTime;

        private float bobSpeed = 2f;
        private float bobAmount = 0.3f;
        private Vector3 startPosition;

        public void Initialize(string id, int qty, bool credits)
        {
            itemId = id;
            quantity = qty;
            isCredits = credits;
            spawnTime = Time.time;
            startPosition = transform.position;
        }

        private void Update()
        {
            // Bobbing effect
            float yOffset = Mathf.Sin(Time.time * bobSpeed) * bobAmount;
            transform.position = new Vector3(
                transform.position.x,
                startPosition.y + yOffset,
                transform.position.z
            );

            // Slow rotation
            transform.Rotate(0, 30f * Time.deltaTime, 0);
        }
    }

    [System.Serializable]
    public class LootTable
    {
        public string tableId;
        public string tableName;
        public List<LootTableEntry> entries = new List<LootTableEntry>();
    }

    [System.Serializable]
    public class LootTableEntry
    {
        public string itemId;
        [Range(0f, 1f)]
        public float dropChance = 0.5f;
        public int minQuantity = 1;
        public int maxQuantity = 1;
    }

    [System.Serializable]
    public class LootEntry
    {
        public string itemId;
        public int quantity;
    }
}
