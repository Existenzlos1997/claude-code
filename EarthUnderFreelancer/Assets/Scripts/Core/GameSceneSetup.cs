using UnityEngine;

namespace EarthUnderFreelancer.Core
{
    /// <summary>
    /// Sets up the game scene with all required managers
    /// Attach to an empty GameObject in the scene
    /// </summary>
    public class GameSceneSetup : MonoBehaviour
    {
        [Header("Manager Prefabs")]
        [SerializeField] private bool createManagers = true;

        [Header("Scene Settings")]
        [SerializeField] private bool spawnPlayer = true;
        [SerializeField] private Transform playerSpawnPoint;
        [SerializeField] private GameObject playerPrefab;

        [Header("AI Settings")]
        [SerializeField] private bool spawnEnemies = true;
        [SerializeField] private int initialEnemyCount = 5;

        [Header("Environment")]
        [SerializeField] private bool createAsteroidField = true;
        [SerializeField] private bool createStation = true;
        [SerializeField] private Vector3 stationPosition = new Vector3(500, 0, 0);

        private void Awake()
        {
            if (createManagers)
            {
                CreateManagers();
            }
        }

        private void Start()
        {
            if (spawnPlayer)
            {
                SpawnPlayer();
            }

            if (createAsteroidField)
            {
                CreateAsteroidField();
            }

            if (createStation)
            {
                CreateSpaceStation();
            }

            if (spawnEnemies)
            {
                SpawnInitialEnemies();
            }
        }

        private void CreateManagers()
        {
            // Create GameManager if not exists
            if (GameManager.Instance == null)
            {
                GameObject gmObj = new GameObject("GameManager");
                gmObj.AddComponent<GameManager>();
            }

            // Create InputManager if not exists
            if (InputManager.Instance == null)
            {
                GameObject imObj = new GameObject("InputManager");
                imObj.AddComponent<InputManager>();
            }

            // Create AudioManager if not exists
            if (AudioManager.Instance == null)
            {
                GameObject amObj = new GameObject("AudioManager");
                amObj.AddComponent<AudioManager>();
            }

            // Create Economy Manager
            if (Systems.EconomyManager.Instance == null)
            {
                GameObject emObj = new GameObject("EconomyManager");
                emObj.AddComponent<Systems.EconomyManager>();
            }

            // Create Faction Manager
            if (Systems.FactionManager.Instance == null)
            {
                GameObject fmObj = new GameObject("FactionManager");
                fmObj.AddComponent<Systems.FactionManager>();
            }

            // Create Inventory Manager
            if (Systems.InventoryManager.Instance == null)
            {
                GameObject invObj = new GameObject("InventoryManager");
                invObj.AddComponent<Systems.InventoryManager>();
            }

            // Create Mission System
            if (Systems.MissionSystem.Instance == null)
            {
                GameObject msObj = new GameObject("MissionSystem");
                msObj.AddComponent<Systems.MissionSystem>();
            }

            // Create Progression Manager
            if (Systems.ProgressionManager.Instance == null)
            {
                GameObject pmObj = new GameObject("ProgressionManager");
                pmObj.AddComponent<Systems.ProgressionManager>();
            }

            // Create Crafting Manager
            if (Systems.CraftingManager.Instance == null)
            {
                GameObject cmObj = new GameObject("CraftingManager");
                cmObj.AddComponent<Systems.CraftingManager>();
            }

            // Create Station Manager
            if (Systems.StationManager.Instance == null)
            {
                GameObject smObj = new GameObject("StationManager");
                smObj.AddComponent<Systems.StationManager>();
            }

            // Create UI Manager
            if (UI.UIManager.Instance == null)
            {
                GameObject uiObj = new GameObject("UIManager");
                uiObj.AddComponent<UI.UIManager>();
            }
        }

        private void SpawnPlayer()
        {
            Vector3 spawnPos = playerSpawnPoint != null ? playerSpawnPoint.position : Vector3.zero;
            Quaternion spawnRot = playerSpawnPoint != null ? playerSpawnPoint.rotation : Quaternion.identity;

            GameObject player;

            if (playerPrefab != null)
            {
                player = Instantiate(playerPrefab, spawnPos, spawnRot);
            }
            else
            {
                // Create default player
                player = CreateDefaultPlayer(spawnPos, spawnRot);
            }

            player.name = "Player";
            player.tag = "Player";

            // Setup camera to follow player
            SetupCamera(player.transform);
        }

        private GameObject CreateDefaultPlayer(Vector3 position, Quaternion rotation)
        {
            GameObject player = new GameObject("Player");
            player.transform.position = position;
            player.transform.rotation = rotation;
            player.tag = "Player";

            // Add mesh (placeholder)
            GameObject mesh = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            mesh.transform.SetParent(player.transform);
            mesh.transform.localPosition = Vector3.zero;
            mesh.transform.localRotation = Quaternion.Euler(90, 0, 0);
            mesh.transform.localScale = new Vector3(1, 2, 1);

            // Add components
            Rigidbody rb = player.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.mass = 100f;
            rb.linearDamping = 0.5f;
            rb.angularDamping = 2f;

            player.AddComponent<Vehicles.VehicleController>();
            player.AddComponent<Vehicles.HealthSystem>();
            player.AddComponent<Combat.WeaponController>();
            player.AddComponent<Combat.TargetingSystem>();
            player.AddComponent<Player.PlayerController>();

            // Add collider
            CapsuleCollider col = player.GetComponent<CapsuleCollider>();
            if (col == null) col = player.AddComponent<CapsuleCollider>();
            col.height = 4f;
            col.radius = 1f;

            return player;
        }

        private void SetupCamera(Transform target)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                GameObject camObj = new GameObject("Main Camera");
                camObj.tag = "MainCamera";
                mainCamera = camObj.AddComponent<Camera>();
                camObj.AddComponent<AudioListener>();
            }

            Player.CameraController camController = mainCamera.GetComponent<Player.CameraController>();
            if (camController == null)
            {
                camController = mainCamera.gameObject.AddComponent<Player.CameraController>();
            }

            // Camera will find player automatically via tag
        }

        private void CreateAsteroidField()
        {
            GameObject fieldObj = new GameObject("AsteroidField");
            var generator = fieldObj.AddComponent<Systems.AsteroidFieldGenerator>();
            // Generator will create asteroids on Start
        }

        private void CreateSpaceStation()
        {
            GameObject stationObj = new GameObject("SpaceStation_Traders");
            stationObj.transform.position = stationPosition;

            // Add visual (placeholder)
            GameObject mesh = GameObject.CreatePrimitive(PrimitiveType.Cube);
            mesh.transform.SetParent(stationObj.transform);
            mesh.transform.localPosition = Vector3.zero;
            mesh.transform.localScale = new Vector3(50, 50, 50);

            // Add station component
            var station = stationObj.AddComponent<Systems.SpaceStation>();
            // Station properties are set via serialized fields or can be configured programmatically

            // Add collider
            BoxCollider col = stationObj.GetComponent<BoxCollider>();
            if (col == null) col = stationObj.AddComponent<BoxCollider>();
            col.size = new Vector3(50, 50, 50);
            col.isTrigger = true;
        }

        private void SpawnInitialEnemies()
        {
            if (AI.AISpawnManager.Instance != null)
            {
                // Use spawn manager
                return;
            }

            // Manual spawn
            for (int i = 0; i < initialEnemyCount; i++)
            {
                Vector3 spawnPos = Random.insideUnitSphere * 500f;
                spawnPos.y = 0; // Keep in same plane for easier combat
                
                CreateEnemy(spawnPos, "Pirates");
            }
        }

        private void CreateEnemy(Vector3 position, string factionId)
        {
            GameObject enemy = new GameObject($"Enemy_{factionId}");
            enemy.transform.position = position;
            enemy.tag = "Enemy";
            enemy.layer = LayerMask.NameToLayer("Enemy");

            // Add mesh (placeholder)
            GameObject mesh = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            mesh.transform.SetParent(enemy.transform);
            mesh.transform.localPosition = Vector3.zero;
            mesh.transform.localScale = new Vector3(2, 2, 4);

            // Set red color for pirates
            Renderer rend = mesh.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material.color = factionId == "Pirates" ? Color.red : Color.blue;
            }

            // Add components
            Rigidbody rb = enemy.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.mass = 80f;

            enemy.AddComponent<AI.AIShipController>();

            // Add collider
            SphereCollider col = enemy.GetComponent<SphereCollider>();
            if (col == null) col = enemy.AddComponent<SphereCollider>();
            col.radius = 2f;
        }
    }
}
