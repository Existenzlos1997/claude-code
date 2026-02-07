using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace EarthUnderFreelancer.Editor
{
    /// <summary>
    /// Creates properly configured Unity scenes for the game
    /// </summary>
    public class SceneTemplateGenerator : EditorWindow
    {
        [MenuItem("EarthUnderFreelancer/Tools/Scene Template Generator")]
        public static void ShowWindow()
        {
            GetWindow<SceneTemplateGenerator>("Scene Template Generator");
        }

        private void OnGUI()
        {
            GUILayout.Label("Scene Template Generator", EditorStyles.boldLabel);
            GUILayout.Label("Create pre-configured scenes", EditorStyles.miniLabel);
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Create GameScene", GUILayout.Height(30)))
            {
                CreateGameScene();
            }
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Create MainMenu Scene", GUILayout.Height(30)))
            {
                CreateMainMenuScene();
            }
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Create Training Scene", GUILayout.Height(30)))
            {
                CreateTrainingScene();
            }
            
            EditorGUILayout.Space();
            
            EditorGUILayout.HelpBox(
                "Creates Unity scenes with proper setup:\n\n" +
                "GameScene: Main gameplay with WorldSetup\n" +
                "MainMenu: Title screen and login\n" +
                "Training: Tutorial level\n\n" +
                "Scenes are saved to Assets/Scenes/",
                MessageType.Info);
        }

        private static void CreateGameScene()
        {
            // Create new scene
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            
            // Configure camera for large view distances
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                mainCam.farClipPlane = 100000f;
                mainCam.transform.position = new Vector3(0, 200, -500);
                mainCam.clearFlags = CameraClearFlags.Skybox;
            }
            
            // Configure directional light
            Light[] lights = Object.FindObjectsOfType<Light>();
            foreach (Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    light.color = new Color(1f, 0.95f, 0.9f);
                    light.intensity = 1.2f;
                    light.shadows = LightShadows.Soft;
                    light.transform.rotation = Quaternion.Euler(50, -30, 0);
                    light.transform.position = new Vector3(0, 3000, 0);
                }
            }
            
            // Setup render settings
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.5f, 0.6f, 0.7f);
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = 0.00005f;
            RenderSettings.ambientIntensity = 0.4f;
            
            // Create WorldSetup GameObject
            GameObject worldSetup = new GameObject("WorldSetup");
            
            // Try to add SimpleWorldSetup component
            var setupType = System.Type.GetType("EarthUnderFreelancer.Gameplay.SimpleWorldSetup");
            if (setupType != null)
            {
                worldSetup.AddComponent(setupType);
            }
            else
            {
                Debug.LogWarning("SimpleWorldSetup script not found. Add it manually.");
            }
            
            // Create GameModeManager GameObject
            GameObject gameModeObj = new GameObject("GameModeManager");
            var gameModeType = System.Type.GetType("EarthUnderFreelancer.Gameplay.GameModeManager");
            if (gameModeType != null)
            {
                gameModeObj.AddComponent(gameModeType);
            }
            
            // Create DeveloperTools GameObject
            GameObject devToolsObj = new GameObject("DeveloperTools");
            var devToolsType = System.Type.GetType("EarthUnderFreelancer.Gameplay.DeveloperTools");
            if (devToolsType != null)
            {
                devToolsObj.AddComponent(devToolsType);
            }
            
            // Save scene
            string scenePath = "Assets/Scenes/GameScene.unity";
            EnsureDirectoryExists("Assets/Scenes");
            EditorSceneManager.SaveScene(scene, scenePath);
            
            EditorUtility.DisplayDialog("Success", 
                $"GameScene created successfully!\n\nLocation: {scenePath}\n\nPress Play to test.", "OK");
            
            Debug.Log($"GameScene created at {scenePath}");
        }

        private static void CreateMainMenuScene()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            
            // Configure for UI
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                mainCam.clearFlags = CameraClearFlags.SolidColor;
                mainCam.backgroundColor = new Color(0.1f, 0.1f, 0.15f);
            }
            
            // Create Canvas
            GameObject canvasObj = new GameObject("Canvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            
            // Save scene
            string scenePath = "Assets/Scenes/MainMenu.unity";
            EnsureDirectoryExists("Assets/Scenes");
            EditorSceneManager.SaveScene(scene, scenePath);
            
            EditorUtility.DisplayDialog("Success", 
                $"MainMenu scene created!\n\nLocation: {scenePath}", "OK");
        }

        private static void CreateTrainingScene()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            
            // Similar to GameScene but simplified
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                mainCam.farClipPlane = 50000f;
                mainCam.transform.position = new Vector3(0, 100, -300);
            }
            
            // Create smaller world setup
            GameObject worldSetup = new GameObject("TrainingWorldSetup");
            
            // Save scene
            string scenePath = "Assets/Scenes/TrainingScene.unity";
            EnsureDirectoryExists("Assets/Scenes");
            EditorSceneManager.SaveScene(scene, scenePath);
            
            EditorUtility.DisplayDialog("Success", 
                $"Training scene created!\n\nLocation: {scenePath}", "OK");
        }

        private static void EnsureDirectoryExists(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                string[] folders = path.Split('/');
                string currentPath = "";
                
                for (int i = 0; i < folders.Length; i++)
                {
                    if (string.IsNullOrEmpty(folders[i])) continue;
                    
                    string parentPath = i == 0 ? "" : currentPath;
                    string newPath = i == 0 ? folders[i] : currentPath + "/" + folders[i];
                    
                    if (!AssetDatabase.IsValidFolder(newPath) && !string.IsNullOrEmpty(parentPath))
                    {
                        AssetDatabase.CreateFolder(parentPath, folders[i]);
                    }
                    
                    currentPath = newPath;
                }
            }
        }
    }
}
