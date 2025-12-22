using UnityEngine;

namespace EarthUnderFreelancer.Gameplay
{
    /// <summary>
    /// Creates a simple playable world with basic elements
    /// Perfect for quick testing and demos
    /// </summary>
    public class SimpleWorldSetup : MonoBehaviour
    {
        [Header("World Settings")]
        [SerializeField] private bool createSkybox = true;
        [SerializeField] private bool createTerrain = true;
        [SerializeField] private bool createLighting = true;
        [SerializeField] private float worldSize = 50000f;

        [Header("Gameplay Elements")]
        [SerializeField] private bool spawnPlayer = true;
        [SerializeField] private bool spawnEnemies = true;
        [SerializeField] private int enemyCount = 5;
        [SerializeField] private bool createCheckpoints = true;

        private void Start()
        {
            Debug.Log("SimpleWorldSetup: Creating basic world...");

            if (createLighting) SetupLighting();
            if (createSkybox) SetupSkybox();
            if (createTerrain) SetupTerrain();
            if (spawnPlayer) SetupPlayer();
            if (spawnEnemies) SetupEnemies();
            if (createCheckpoints) SetupCheckpoints();

            Debug.Log("SimpleWorldSetup: World created successfully!");
        }

        private void SetupLighting()
        {
            // Create directional light (sun)
            GameObject sunObj = new GameObject("Directional Light (Sun)");
            Light sun = sunObj.AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.color = new Color(1f, 0.95f, 0.9f); // Slightly warm white
            sun.intensity = 1.2f;
            sun.shadows = LightShadows.Soft;
            sunObj.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            // Set ambient lighting
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
            RenderSettings.ambientIntensity = 0.4f;

            Debug.Log("SimpleWorldSetup: Lighting configured");
        }

        private void SetupSkybox()
        {
            // Create procedural sky (Unity's built-in)
            RenderSettings.skybox = null; // Use default/procedural sky
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.5f, 0.6f, 0.7f);
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = 0.00005f;

            Debug.Log("SimpleWorldSetup: Skybox and fog configured");
        }

        private void SetupTerrain()
        {
            // Create a simple ground plane
            GameObject groundObj = GameObject.CreatePrimitive(PrimitiveType.Plane);
            groundObj.name = "Ground";
            groundObj.transform.position = new Vector3(0, -1000, 0);
            groundObj.transform.localScale = new Vector3(worldSize / 10f, 1, worldSize / 10f);

            // Create terrain material
            Material terrainMat = new Material(Shader.Find("Standard"));
            terrainMat.color = new Color(0.3f, 0.5f, 0.3f); // Greenish
            groundObj.GetComponent<Renderer>().material = terrainMat;

            Debug.Log($"SimpleWorldSetup: Ground plane created ({worldSize}x{worldSize})");
        }

        private void SetupPlayer()
        {
            // Create player spawn point
            GameObject playerSpawn = new GameObject("PlayerSpawn");
            playerSpawn.transform.position = new Vector3(0, 100, 0);

            // Create player aircraft spawner
            GameObject spawnerObj = new GameObject("PlayerSpawner");
            AircraftSpawner spawner = spawnerObj.AddComponent<AircraftSpawner>();
            
            // Configure for player
            // Note: This requires reflection or making fields public
            Debug.Log("SimpleWorldSetup: Player spawn point created at (0, 100, 0)");
        }

        private void SetupEnemies()
        {
            // Create enemy spawn points in a circle
            float radius = 2000f;
            for (int i = 0; i < enemyCount; i++)
            {
                float angle = (360f / enemyCount) * i;
                float x = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
                float z = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
                float y = Random.Range(500f, 1500f);

                GameObject enemySpawn = new GameObject($"EnemySpawn_{i+1}");
                enemySpawn.transform.position = new Vector3(x, y, z);

                // Add spawner component
                AircraftSpawner spawner = enemySpawn.AddComponent<AircraftSpawner>();
                // Configure for enemy AI
            }

            Debug.Log($"SimpleWorldSetup: Created {enemyCount} enemy spawn points");
        }

        private void SetupCheckpoints()
        {
            // Create a few checkpoint markers for navigation
            Vector3[] checkpointPositions = new Vector3[]
            {
                new Vector3(1000, 500, 0),
                new Vector3(0, 500, 1000),
                new Vector3(-1000, 500, 0),
                new Vector3(0, 500, -1000)
            };

            for (int i = 0; i < checkpointPositions.Length; i++)
            {
                GameObject checkpoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                checkpoint.name = $"Checkpoint_{i+1}";
                checkpoint.transform.position = checkpointPositions[i];
                checkpoint.transform.localScale = Vector3.one * 100f;

                // Make it transparent-ish
                Material mat = new Material(Shader.Find("Standard"));
                mat.color = new Color(0, 1, 0, 0.3f);
                mat.SetFloat("_Mode", 3); // Transparent mode
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = 3000;
                checkpoint.GetComponent<Renderer>().material = mat;

                // Remove collider to not interfere
                Destroy(checkpoint.GetComponent<Collider>());
            }

            Debug.Log($"SimpleWorldSetup: Created {checkpointPositions.Length} navigation checkpoints");
        }

        private void OnDrawGizmos()
        {
            // Draw world bounds
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(worldSize, 5000, worldSize));

            // Draw center marker
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(Vector3.zero, 100f);
        }
    }
}
