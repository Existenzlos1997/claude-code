using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using EarthUnderFreelancer.Data;

namespace EarthUnderFreelancer.Editor
{
    /// <summary>
    /// Generates Unity prefabs from aircraft data
    /// Creates placeholder models and properly configured prefabs
    /// </summary>
    public class PrefabGenerator : EditorWindow
    {
        private bool generateAircraft = true;
        private bool generateWeapons = true;
        private bool generateProjectiles = true;
        private bool generateStations = false;
        
        private string prefabOutputPath = "Assets/Prefabs/";
        private string aircraftPath = "Aircraft/";
        private string weaponsPath = "Weapons/";
        private string projectilesPath = "Projectiles/";
        
        private Vector2 scrollPosition;

        [MenuItem("EarthUnderFreelancer/Tools/Prefab Generator")]
        public static void ShowWindow()
        {
            GetWindow<PrefabGenerator>("Prefab Generator");
        }

        private void OnGUI()
        {
            GUILayout.Label("Prefab Generator", EditorStyles.boldLabel);
            GUILayout.Label("Automatically generates prefabs from game data", EditorStyles.miniLabel);
            
            EditorGUILayout.Space();
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            // Options
            GUILayout.Label("Generate Options:", EditorStyles.boldLabel);
            generateAircraft = EditorGUILayout.Toggle("Aircraft Prefabs", generateAircraft);
            generateWeapons = EditorGUILayout.Toggle("Weapon Prefabs", generateWeapons);
            generateProjectiles = EditorGUILayout.Toggle("Projectile Prefabs", generateProjectiles);
            generateStations = EditorGUILayout.Toggle("Station Prefabs", generateStations);
            
            EditorGUILayout.Space();
            
            // Paths
            GUILayout.Label("Output Paths:", EditorStyles.boldLabel);
            prefabOutputPath = EditorGUILayout.TextField("Base Path:", prefabOutputPath);
            aircraftPath = EditorGUILayout.TextField("Aircraft:", aircraftPath);
            weaponsPath = EditorGUILayout.TextField("Weapons:", weaponsPath);
            projectilesPath = EditorGUILayout.TextField("Projectiles:", projectilesPath);
            
            EditorGUILayout.Space();
            
            // Action buttons
            if (GUILayout.Button("Generate All Selected", GUILayout.Height(30)))
            {
                GenerateAllPrefabs();
            }
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Generate Sample Aircraft (P-51D)", GUILayout.Height(25)))
            {
                GenerateSampleAircraft();
            }
            
            if (GUILayout.Button("Generate Sample Weapon (MG)", GUILayout.Height(25)))
            {
                GenerateSampleWeapon();
            }
            
            EditorGUILayout.Space();
            
            // Info
            EditorGUILayout.HelpBox(
                "This tool creates placeholder prefabs with proper components.\n\n" +
                "Aircraft will have:\n- Rigidbody\n- Collider\n- Placeholder mesh\n- Scripts (if available)\n\n" +
                "You can later replace placeholder meshes with real 3D models.",
                MessageType.Info);
            
            EditorGUILayout.EndScrollView();
        }

        private void GenerateAllPrefabs()
        {
            int count = 0;
            
            if (generateAircraft)
            {
                count += GenerateAircraftPrefabs();
            }
            
            if (generateWeapons)
            {
                count += GenerateWeaponPrefabs();
            }
            
            if (generateProjectiles)
            {
                count += GenerateProjectilePrefabs();
            }
            
            if (generateStations)
            {
                count += GenerateStationPrefabs();
            }
            
            EditorUtility.DisplayDialog("Prefab Generation Complete", 
                $"Successfully generated {count} prefabs!", "OK");
            
            AssetDatabase.Refresh();
        }

        private int GenerateAircraftPrefabs()
        {
            string path = prefabOutputPath + aircraftPath;
            EnsureDirectoryExists(path);
            
            // Generate a few sample aircraft
            string[] sampleAircraft = new string[] 
            {
                "P-51D Mustang",
                "Bf 109 G-6",
                "Spitfire Mk IX",
                "F-16 Fighting Falcon",
                "MiG-29 Fulcrum"
            };
            
            int count = 0;
            foreach (string aircraftName in sampleAircraft)
            {
                GameObject prefab = CreateAircraftPrefab(aircraftName);
                if (prefab != null)
                {
                    string prefabPath = path + SanitizeFileName(aircraftName) + ".prefab";
                    PrefabUtility.SaveAsPrefabAsset(prefab, prefabPath);
                    DestroyImmediate(prefab);
                    count++;
                }
            }
            
            return count;
        }

        private GameObject CreateAircraftPrefab(string aircraftName)
        {
            GameObject aircraft = new GameObject(aircraftName);
            
            // Create fuselage (main body)
            GameObject fuselage = GameObject.CreatePrimitive(PrimitiveType.Cube);
            fuselage.name = "Fuselage";
            fuselage.transform.parent = aircraft.transform;
            fuselage.transform.localScale = new Vector3(2f, 1f, 5f);
            
            // Create wings
            GameObject leftWing = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leftWing.name = "LeftWing";
            leftWing.transform.parent = aircraft.transform;
            leftWing.transform.localPosition = new Vector3(-3.5f, 0, 0);
            leftWing.transform.localScale = new Vector3(5f, 0.2f, 2f);
            
            GameObject rightWing = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rightWing.name = "RightWing";
            rightWing.transform.parent = aircraft.transform;
            rightWing.transform.localPosition = new Vector3(3.5f, 0, 0);
            rightWing.transform.localScale = new Vector3(5f, 0.2f, 2f);
            
            // Create tail
            GameObject tail = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tail.name = "Tail";
            tail.transform.parent = aircraft.transform;
            tail.transform.localPosition = new Vector3(0, 1f, -2.5f);
            tail.transform.localScale = new Vector3(0.2f, 2f, 1f);
            
            // Add physics
            Rigidbody rb = aircraft.AddComponent<Rigidbody>();
            rb.mass = 3000f;
            rb.drag = 0.5f;
            rb.angularDrag = 5f;
            rb.useGravity = false;
            
            // Add main collider
            BoxCollider collider = aircraft.AddComponent<BoxCollider>();
            collider.size = new Vector3(8f, 2f, 6f);
            
            // Color based on type (WW2 vs Modern) - use sharedMaterial for efficiency
            Material sharedMat = new Material(Shader.Find("Standard"));
            if (aircraftName.Contains("P-51") || aircraftName.Contains("Bf 109") || aircraftName.Contains("Spitfire"))
            {
                sharedMat.color = new Color(0.4f, 0.4f, 0.4f); // Gray for WW2
            }
            else
            {
                sharedMat.color = new Color(0.5f, 0.5f, 0.6f); // Blue-gray for modern
            }
            
            // Assign same material to all renderers to avoid memory waste
            foreach (Renderer renderer in aircraft.GetComponentsInChildren<Renderer>())
            {
                renderer.sharedMaterial = sharedMat;
            }
            
            return aircraft;
        }

        private void GenerateSampleAircraft()
        {
            string path = prefabOutputPath + aircraftPath;
            EnsureDirectoryExists(path);
            
            GameObject aircraft = CreateAircraftPrefab("P-51D Mustang");
            string prefabPath = path + "P51D_Mustang.prefab";
            PrefabUtility.SaveAsPrefabAsset(aircraft, prefabPath);
            DestroyImmediate(aircraft);
            
            EditorUtility.DisplayDialog("Success", 
                "Sample aircraft prefab created at:\n" + prefabPath, "OK");
            
            AssetDatabase.Refresh();
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        }

        private int GenerateWeaponPrefabs()
        {
            string path = prefabOutputPath + weaponsPath;
            EnsureDirectoryExists(path);
            
            // Sample weapons
            string[] weapons = new string[] { "MG", "Cannon", "Rocket", "Missile" };
            
            int count = 0;
            foreach (string weapon in weapons)
            {
                GameObject prefab = CreateWeaponPrefab(weapon);
                if (prefab != null)
                {
                    string prefabPath = path + weapon + ".prefab";
                    PrefabUtility.SaveAsPrefabAsset(prefab, prefabPath);
                    DestroyImmediate(prefab);
                    count++;
                }
            }
            
            return count;
        }

        private GameObject CreateWeaponPrefab(string weaponName)
        {
            GameObject weapon = new GameObject(weaponName);
            
            GameObject barrel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            barrel.name = "Barrel";
            barrel.transform.parent = weapon.transform;
            barrel.transform.localScale = new Vector3(0.1f, 1f, 0.1f);
            barrel.transform.localRotation = Quaternion.Euler(90, 0, 0);
            
            Material sharedMat = new Material(Shader.Find("Standard"));
            sharedMat.color = new Color(0.3f, 0.3f, 0.3f);
            barrel.GetComponent<Renderer>().sharedMaterial = sharedMat;
            
            return weapon;
        }

        private void GenerateSampleWeapon()
        {
            string path = prefabOutputPath + weaponsPath;
            EnsureDirectoryExists(path);
            
            GameObject weapon = CreateWeaponPrefab("MG");
            string prefabPath = path + "MG.prefab";
            PrefabUtility.SaveAsPrefabAsset(weapon, prefabPath);
            DestroyImmediate(weapon);
            
            EditorUtility.DisplayDialog("Success", 
                "Sample weapon prefab created at:\n" + prefabPath, "OK");
            
            AssetDatabase.Refresh();
        }

        private int GenerateProjectilePrefabs()
        {
            string path = prefabOutputPath + projectilesPath;
            EnsureDirectoryExists(path);
            
            // Sample projectiles
            string[] projectiles = new string[] { "Bullet", "Shell", "Rocket", "Missile" };
            
            int count = 0;
            foreach (string proj in projectiles)
            {
                GameObject prefab = CreateProjectilePrefab(proj);
                if (prefab != null)
                {
                    string prefabPath = path + proj + ".prefab";
                    PrefabUtility.SaveAsPrefabAsset(prefab, prefabPath);
                    DestroyImmediate(prefab);
                    count++;
                }
            }
            
            return count;
        }

        private GameObject CreateProjectilePrefab(string projectileName)
        {
            GameObject projectile = new GameObject(projectileName);
            
            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            visual.name = "Visual";
            visual.transform.parent = projectile.transform;
            visual.transform.localScale = Vector3.one * 0.2f;
            
            Rigidbody rb = projectile.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.mass = 0.01f;
            
            Material sharedMat = new Material(Shader.Find("Standard"));
            sharedMat.color = Color.yellow;
            visual.GetComponent<Renderer>().sharedMaterial = sharedMat;
            
            return projectile;
        }

        private int GenerateStationPrefabs()
        {
            string path = prefabOutputPath + "Stations/";
            EnsureDirectoryExists(path);
            
            // Create a simple station prefab
            GameObject station = new GameObject("SpaceStation");
            
            // Main body
            GameObject core = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            core.name = "Core";
            core.transform.parent = station.transform;
            core.transform.localScale = new Vector3(20f, 5f, 20f);
            
            // Docking arms
            for (int i = 0; i < 4; i++)
            {
                GameObject arm = GameObject.CreatePrimitive(PrimitiveType.Cube);
                arm.name = $"DockingArm_{i+1}";
                arm.transform.parent = station.transform;
                float angle = i * 90f;
                arm.transform.localPosition = new Vector3(
                    Mathf.Cos(angle * Mathf.Deg2Rad) * 15f,
                    0,
                    Mathf.Sin(angle * Mathf.Deg2Rad) * 15f
                );
                arm.transform.localScale = new Vector3(2f, 2f, 10f);
                arm.transform.localRotation = Quaternion.Euler(0, angle, 0);
            }
            
            // Station material
            Material stationMat = new Material(Shader.Find("Standard"));
            stationMat.color = new Color(0.7f, 0.7f, 0.8f);
            foreach (Renderer renderer in station.GetComponentsInChildren<Renderer>())
            {
                renderer.sharedMaterial = stationMat;
            }
            
            string prefabPath = path + "SpaceStation.prefab";
            PrefabUtility.SaveAsPrefabAsset(station, prefabPath);
            DestroyImmediate(station);
            
            return 1;
        }

        private void EnsureDirectoryExists(string path)
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

        private string SanitizeFileName(string name)
        {
            return name.Replace(" ", "_").Replace("/", "-").Replace("\\", "-");
        }
    }
}
