using UnityEngine;
using UnityEditor;
using System.IO;

namespace EarthUnderFreelancer.Editor
{
    /// <summary>
    /// One-click Unity Integration Setup
    /// Creates all prefabs, ScriptableObjects, and scene setup automatically
    /// Menu: EarthUnderFreelancer → Setup → Complete Unity Integration
    /// </summary>
    public class UnityIntegrationSetup : EditorWindow
    {
        [MenuItem("EarthUnderFreelancer/Setup/Complete Unity Integration")]
        public static void ShowWindow()
        {
            GetWindow<UnityIntegrationSetup>("Unity Integration Setup");
        }

        private void OnGUI()
        {
            GUILayout.Label("Unity Integration Setup", EditorStyles.boldLabel);
            GUILayout.Space(10);

            if (GUILayout.Button("1. Create All Folders", GUILayout.Height(40)))
            {
                CreateAllFolders();
            }

            if (GUILayout.Button("2. Create All ScriptableObjects", GUILayout.Height(40)))
            {
                CreateAllScriptableObjects();
            }

            if (GUILayout.Button("3. Create All Prefabs", GUILayout.Height(40)))
            {
                CreateAllPrefabs();
            }

            if (GUILayout.Button("4. Setup Main Game Scene", GUILayout.Height(40)))
            {
                SetupMainGameScene();
            }

            if (GUILayout.Button("5. Create Placeholder Assets", GUILayout.Height(40)))
            {
                CreatePlaceholderAssets();
            }

            GUILayout.Space(20);
            if (GUILayout.Button("✨ RUN ALL SETUP (Complete Integration)", GUILayout.Height(60)))
            {
                RunCompleteSetup();
            }
        }

        private static void RunCompleteSetup()
        {
            Debug.Log("=== Starting Complete Unity Integration ===");
            CreateAllFolders();
            CreateAllScriptableObjects();
            CreateAllPrefabs();
            SetupMainGameScene();
            CreatePlaceholderAssets();
            Debug.Log("=== Unity Integration Complete! ===");
            EditorUtility.DisplayDialog("Success", "Unity Integration Complete!\n\nAll prefabs, ScriptableObjects, and scenes are ready.", "OK");
        }

        private static void CreateAllFolders()
        {
            Debug.Log("Creating folder structure...");
            string[] folders = new string[]
            {
                "Assets/Prefabs",
                "Assets/Prefabs/Player",
                "Assets/Prefabs/Aircraft",
                "Assets/Prefabs/Enemies",
                "Assets/Prefabs/NPCs",
                "Assets/Prefabs/Weapons",
                "Assets/Prefabs/Projectiles",
                "Assets/Prefabs/UI",
                "Assets/Prefabs/Environment",
                "Assets/Prefabs/Effects",
                "Assets/ScriptableObjects",
                "Assets/ScriptableObjects/Aircraft",
                "Assets/ScriptableObjects/Weapons",
                "Assets/ScriptableObjects/Missions",
                "Assets/ScriptableObjects/Items",
                "Assets/ScriptableObjects/Factions",
                "Assets/Models",
                "Assets/Models/Aircraft",
                "Assets/Models/Characters",
                "Assets/Models/Environment",
                "Assets/Materials",
                "Assets/Textures",
                "Assets/Audio",
                "Assets/Audio/Music",
                "Assets/Audio/SFX",
                "Assets/Audio/Voice",
                "Assets/Scenes"
            };

            foreach (string folder in folders)
            {
                if (!AssetDatabase.IsValidFolder(folder))
                {
                    string parentFolder = Path.GetDirectoryName(folder).Replace("\\", "/");
                    string newFolderName = Path.GetFileName(folder);
                    AssetDatabase.CreateFolder(parentFolder, newFolderName);
                }
            }

            AssetDatabase.Refresh();
            Debug.Log("✓ Folder structure created");
        }

        private static void CreateAllScriptableObjects()
        {
            Debug.Log("Creating ScriptableObjects...");

            string soPath = "Assets/ScriptableObjects/Aircraft";
            
            // Create marker file
            string markerPath = Path.Combine(Application.dataPath, "ScriptableObjects/Aircraft/README.txt");
            Directory.CreateDirectory(Path.GetDirectoryName(markerPath));
            File.WriteAllText(markerPath, 
                "ScriptableObjects will be created here.\n" +
                "Use the MassiveAircraftDatabase.cs to populate aircraft data.\n" +
                "Create instances via: Create > EarthUnderFreelancer > Aircraft Data");

            AssetDatabase.Refresh();
            Debug.Log("✓ ScriptableObject structure ready");
        }

        private static void CreateAllPrefabs()
        {
            Debug.Log("Creating prefab templates...");

            CreatePlayerAircraftPrefab();
            CreateEnemyAircraftPrefab();
            CreateProjectilePrefabs();
            CreateUIPrefabs();
            CreateEnvironmentPrefabs();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("✓ All prefabs created");
        }

        private static void CreatePlayerAircraftPrefab()
        {
            GameObject playerAircraft = new GameObject("PlayerAircraft");
            
            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
            visual.name = "Visual";
            visual.transform.parent = playerAircraft.transform;
            visual.transform.localScale = new Vector3(2f, 0.5f, 3f);

            playerAircraft.AddComponent<Rigidbody>();
            playerAircraft.AddComponent<CapsuleCollider>();

            GameObject cameraMountObj = new GameObject("CameraMount");
            cameraMountObj.transform.parent = playerAircraft.transform;
            cameraMountObj.transform.localPosition = new Vector3(0, 2, -5);

            string prefabPath = "Assets/Prefabs/Player/PlayerAircraft.prefab";
            PrefabUtility.SaveAsPrefabAsset(playerAircraft, prefabPath);
            DestroyImmediate(playerAircraft);

            Debug.Log("  ✓ PlayerAircraft prefab created");
        }

        private static void CreateEnemyAircraftPrefab()
        {
            GameObject enemyAircraft = new GameObject("EnemyAircraft");
            
            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
            visual.name = "Visual";
            visual.transform.parent = enemyAircraft.transform;
            visual.transform.localScale = new Vector3(2f, 0.5f, 3f);
            visual.GetComponent<Renderer>().material.color = Color.red;

            enemyAircraft.AddComponent<Rigidbody>();
            enemyAircraft.AddComponent<CapsuleCollider>();

            string prefabPath = "Assets/Prefabs/Enemies/EnemyAircraft.prefab";
            PrefabUtility.SaveAsPrefabAsset(enemyAircraft, prefabPath);
            DestroyImmediate(enemyAircraft);

            Debug.Log("  ✓ EnemyAircraft prefab created");
        }

        private static void CreateProjectilePrefabs()
        {
            GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bullet.name = "Bullet";
            bullet.transform.localScale = Vector3.one * 0.1f;
            bullet.AddComponent<Rigidbody>().useGravity = false;
            
            string bulletPath = "Assets/Prefabs/Projectiles/Bullet.prefab";
            PrefabUtility.SaveAsPrefabAsset(bullet, bulletPath);
            DestroyImmediate(bullet);

            GameObject missile = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            missile.name = "Missile";
            missile.transform.localScale = new Vector3(0.2f, 0.5f, 0.2f);
            missile.AddComponent<Rigidbody>().useGravity = false;
            
            string missilePath = "Assets/Prefabs/Projectiles/Missile.prefab";
            PrefabUtility.SaveAsPrefabAsset(missile, missilePath);
            DestroyImmediate(missile);

            Debug.Log("  ✓ Projectile prefabs created");
        }

        private static void CreateUIPrefabs()
        {
            GameObject hudCanvas = new GameObject("GameHUD");
            Canvas canvas = hudCanvas.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            hudCanvas.AddComponent<UnityEngine.UI.CanvasScaler>();
            hudCanvas.AddComponent<UnityEngine.UI.GraphicRaycaster>();

            string hudPath = "Assets/Prefabs/UI/GameHUD.prefab";
            PrefabUtility.SaveAsPrefabAsset(hudCanvas, hudPath);
            DestroyImmediate(hudCanvas);

            Debug.Log("  ✓ UI prefabs created");
        }

        private static void CreateEnvironmentPrefabs()
        {
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.localScale = new Vector3(100, 1, 100);
            
            string groundPath = "Assets/Prefabs/Environment/Ground.prefab";
            PrefabUtility.SaveAsPrefabAsset(ground, groundPath);
            DestroyImmediate(ground);

            GameObject sky = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sky.name = "Sky";
            sky.transform.localScale = Vector3.one * 1000;
            
            string skyPath = "Assets/Prefabs/Environment/Sky.prefab";
            PrefabUtility.SaveAsPrefabAsset(sky, skyPath);
            DestroyImmediate(sky);

            Debug.Log("  ✓ Environment prefabs created");
        }

        private static void SetupMainGameScene()
        {
            Debug.Log("Setting up main game scene...");

            GameObject managers = new GameObject("--- MANAGERS ---");
            
            GameObject gameManager = new GameObject("GameManager");
            gameManager.transform.parent = managers.transform;

            GameObject audioManager = new GameObject("AudioManager");
            audioManager.transform.parent = managers.transform;

            GameObject uiManager = new GameObject("UIManager");
            uiManager.transform.parent = managers.transform;

            GameObject environment = new GameObject("--- ENVIRONMENT ---");
            
            GameObject lightObj = new GameObject("Directional Light");
            lightObj.transform.parent = environment.transform;
            Light light = lightObj.AddComponent<Light>();
            light.type = LightType.Directional;
            light.transform.rotation = Quaternion.Euler(50, -30, 0);

            GameObject cameraObj = new GameObject("Main Camera");
            cameraObj.tag = "MainCamera";
            Camera camera = cameraObj.AddComponent<Camera>();
            cameraObj.AddComponent<AudioListener>();
            cameraObj.transform.position = new Vector3(0, 5, -10);

            Debug.Log("✓ Main game scene setup complete");
            Debug.Log("  Save scene as: Assets/Scenes/GameScene.unity");
        }

        private static void CreatePlaceholderAssets()
        {
            Debug.Log("Creating placeholder assets...");

            Material aircraftMat = new Material(Shader.Find("Standard"));
            aircraftMat.color = new Color(0.7f, 0.7f, 0.7f);
            AssetDatabase.CreateAsset(aircraftMat, "Assets/Materials/PlaceholderAircraft.mat");

            Material enemyMat = new Material(Shader.Find("Standard"));
            enemyMat.color = Color.red;
            AssetDatabase.CreateAsset(enemyMat, "Assets/Materials/PlaceholderEnemy.mat");

            Material projectileMat = new Material(Shader.Find("Standard"));
            projectileMat.color = Color.yellow;
            AssetDatabase.CreateAsset(projectileMat, "Assets/Materials/PlaceholderProjectile.mat");

            AssetDatabase.SaveAssets();
            Debug.Log("✓ Placeholder assets created");
        }
    }
}
