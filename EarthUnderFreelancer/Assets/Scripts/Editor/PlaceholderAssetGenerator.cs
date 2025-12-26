using UnityEngine;
using UnityEditor;

namespace EarthUnderFreelancer.Editor
{
    /// <summary>
    /// Generates placeholder assets for testing and development
    /// Implements VERBESSERUNGSPLAN Priority 2 - Placeholder Asset Generation
    /// Creates simple textures, materials, and meshes procedurally
    /// </summary>
    public class PlaceholderAssetGenerator : EditorWindow
    {
        private string outputPath = "Assets/Placeholder/";
        
        [MenuItem("EarthUnderFreelancer/Tools/Placeholder Asset Generator")]
        public static void ShowWindow()
        {
            GetWindow<PlaceholderAssetGenerator>("Placeholder Assets");
        }

        private void OnGUI()
        {
            GUILayout.Label("Placeholder Asset Generator", EditorStyles.boldLabel);
            GUILayout.Label("Generate simple assets for prototyping", EditorStyles.miniLabel);
            
            EditorGUILayout.Space();
            
            outputPath = EditorGUILayout.TextField("Output Path:", outputPath);
            
            EditorGUILayout.Space();
            
            GUILayout.Label("Generate:", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Basic Textures", GUILayout.Height(30)))
            {
                GenerateBasicTextures();
            }
            
            if (GUILayout.Button("Team-Colored Materials", GUILayout.Height(30)))
            {
                GenerateTeamMaterials();
            }
            
            if (GUILayout.Button("Simple Skybox", GUILayout.Height(30)))
            {
                GenerateSkybox();
            }
            
            if (GUILayout.Button("Particle Systems", GUILayout.Height(30)))
            {
                GenerateParticleSystems();
            }
            
            if (GUILayout.Button("UI Sprites", GUILayout.Height(30)))
            {
                GenerateUISprites();
            }
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Generate All", GUILayout.Height(40)))
            {
                GenerateAll();
            }
            
            EditorGUILayout.Space();
            
            EditorGUILayout.HelpBox(
                "Creates placeholder assets for rapid prototyping:\n" +
                "- Textures (solid colors, patterns)\n" +
                "- Materials (PBR, team colors)\n" +
                "- Skybox materials\n" +
                "- Particle systems (effects)\n" +
                "- UI sprites",
                MessageType.Info);
        }

        private void GenerateBasicTextures()
        {
            EnsureDirectory(outputPath + "Textures/");
            
            // Generate colored textures
            Color[] colors = new Color[]
            {
                Color.white,
                Color.black,
                Color.red,
                Color.green,
                Color.blue,
                Color.yellow,
                Color.cyan,
                Color.magenta,
                new Color(0.5f, 0.5f, 0.5f), // Gray
            };
            
            string[] names = new string[]
            {
                "White",
                "Black",
                "Red",
                "Green",
                "Blue",
                "Yellow",
                "Cyan",
                "Magenta",
                "Gray"
            };
            
            for (int i = 0; i < colors.Length; i++)
            {
                Texture2D texture = CreateSolidTexture(32, 32, colors[i]);
                SaveTexture(texture, $"{outputPath}Textures/Tex_{names[i]}.png");
            }
            
            // Generate pattern textures
            Texture2D checkerboard = CreateCheckerboardTexture(128, 128);
            SaveTexture(checkerboard, $"{outputPath}Textures/Tex_Checkerboard.png");
            
            Texture2D grid = CreateGridTexture(128, 128);
            SaveTexture(grid, $"{outputPath}Textures/Tex_Grid.png");
            
            Debug.Log($"Generated basic textures in {outputPath}Textures/");
            AssetDatabase.Refresh();
            
            EditorUtility.DisplayDialog("Success", "Basic textures generated!", "OK");
        }

        private void GenerateTeamMaterials()
        {
            EnsureDirectory(outputPath + "Materials/");
            
            // Team colors
            CreateAndSaveMaterial("Mat_Player", new Color(0.2f, 0.5f, 1f)); // Blue
            CreateAndSaveMaterial("Mat_Friendly", new Color(0.2f, 1f, 0.2f)); // Green
            CreateAndSaveMaterial("Mat_Enemy", new Color(1f, 0.2f, 0.2f)); // Red
            CreateAndSaveMaterial("Mat_Neutral", new Color(0.8f, 0.8f, 0.2f)); // Yellow
            
            // Common materials
            CreateAndSaveMaterial("Mat_Metal", new Color(0.7f, 0.7f, 0.75f), 0.9f, 0.5f);
            CreateAndSaveMaterial("Mat_Sky", new Color(0.5f, 0.7f, 1f), 0f, 1f);
            CreateAndSaveMaterial("Mat_Ground", new Color(0.3f, 0.4f, 0.25f), 0.1f, 0.8f);
            
            Debug.Log($"Generated team materials in {outputPath}Materials/");
            AssetDatabase.Refresh();
            
            EditorUtility.DisplayDialog("Success", "Team materials generated!", "OK");
        }

        private void GenerateSkybox()
        {
            EnsureDirectory(outputPath + "Skybox/");
            
            // Create simple gradient skybox
            Material skyboxMat = new Material(Shader.Find("Skybox/Procedural"));
            skyboxMat.SetFloat("_SunSize", 0.04f);
            skyboxMat.SetFloat("_SunSizeConvergence", 5f);
            skyboxMat.SetFloat("_AtmosphereThickness", 1f);
            skyboxMat.SetColor("_SkyTint", new Color(0.5f, 0.6f, 0.7f));
            skyboxMat.SetColor("_GroundColor", new Color(0.4f, 0.35f, 0.3f));
            skyboxMat.SetFloat("_Exposure", 1.3f);
            
            AssetDatabase.CreateAsset(skyboxMat, $"{outputPath}Skybox/Skybox_Default.mat");
            
            Debug.Log($"Generated skybox in {outputPath}Skybox/");
            AssetDatabase.Refresh();
            
            EditorUtility.DisplayDialog("Success", "Skybox generated!", "OK");
        }

        private void GenerateParticleSystems()
        {
            EnsureDirectory(outputPath + "Prefabs/");
            
            // Create explosion effect
            GameObject explosion = CreateExplosionEffect();
            PrefabUtility.SaveAsPrefabAsset(explosion, $"{outputPath}Prefabs/FX_Explosion.prefab");
            DestroyImmediate(explosion);
            
            // Create muzzle flash
            GameObject muzzleFlash = CreateMuzzleFlashEffect();
            PrefabUtility.SaveAsPrefabAsset(muzzleFlash, $"{outputPath}Prefabs/FX_MuzzleFlash.prefab");
            DestroyImmediate(muzzleFlash);
            
            // Create smoke trail
            GameObject smokeTrail = CreateSmokeTrailEffect();
            PrefabUtility.SaveAsPrefabAsset(smokeTrail, $"{outputPath}Prefabs/FX_SmokeTrail.prefab");
            DestroyImmediate(smokeTrail);
            
            Debug.Log($"Generated particle systems in {outputPath}Prefabs/");
            AssetDatabase.Refresh();
            
            EditorUtility.DisplayDialog("Success", "Particle systems generated!", "OK");
        }

        private void GenerateUISprites()
        {
            EnsureDirectory(outputPath + "UI/");
            
            // Create simple UI elements
            Texture2D button = CreateRoundedRect(128, 32, new Color(0.2f, 0.2f, 0.25f));
            SaveTexture(button, $"{outputPath}UI/UI_Button.png");
            
            Texture2D panel = CreateRoundedRect(256, 256, new Color(0.1f, 0.1f, 0.15f, 0.9f));
            SaveTexture(panel, $"{outputPath}UI/UI_Panel.png");
            
            Texture2D crosshair = CreateCrosshair(64, 64);
            SaveTexture(crosshair, $"{outputPath}UI/UI_Crosshair.png");
            
            Texture2D healthBar = CreateSolidTexture(128, 16, new Color(0.8f, 0.2f, 0.2f));
            SaveTexture(healthBar, $"{outputPath}UI/UI_HealthBar.png");
            
            Debug.Log($"Generated UI sprites in {outputPath}UI/");
            AssetDatabase.Refresh();
            
            EditorUtility.DisplayDialog("Success", "UI sprites generated!", "OK");
        }

        private void GenerateAll()
        {
            GenerateBasicTextures();
            GenerateTeamMaterials();
            GenerateSkybox();
            GenerateParticleSystems();
            GenerateUISprites();
            
            EditorUtility.DisplayDialog("Complete", 
                "All placeholder assets generated!\n\nCheck the Placeholder folder.", "OK");
        }

        // Utility methods
        private Texture2D CreateSolidTexture(int width, int height, Color color)
        {
            Texture2D texture = new Texture2D(width, height);
            Color[] pixels = new Color[width * height];
            
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }
            
            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }

        private Texture2D CreateCheckerboardTexture(int width, int height, int checkSize = 16)
        {
            Texture2D texture = new Texture2D(width, height);
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool isWhite = ((x / checkSize) + (y / checkSize)) % 2 == 0;
                    texture.SetPixel(x, y, isWhite ? Color.white : Color.gray);
                }
            }
            
            texture.Apply();
            return texture;
        }

        private Texture2D CreateGridTexture(int width, int height, int gridSize = 16)
        {
            Texture2D texture = new Texture2D(width, height);
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool isLine = (x % gridSize == 0 || y % gridSize == 0);
                    texture.SetPixel(x, y, isLine ? Color.black : Color.white);
                }
            }
            
            texture.Apply();
            return texture;
        }

        private Texture2D CreateRoundedRect(int width, int height, Color color)
        {
            Texture2D texture = new Texture2D(width, height);
            int radius = Mathf.Min(width, height) / 8;
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool inCorner = false;
                    
                    // Check if pixel is in a corner radius
                    if (x < radius && y < radius)
                        inCorner = Vector2.Distance(new Vector2(x, y), new Vector2(radius, radius)) > radius;
                    else if (x > width - radius && y < radius)
                        inCorner = Vector2.Distance(new Vector2(x, y), new Vector2(width - radius, radius)) > radius;
                    else if (x < radius && y > height - radius)
                        inCorner = Vector2.Distance(new Vector2(x, y), new Vector2(radius, height - radius)) > radius;
                    else if (x > width - radius && y > height - radius)
                        inCorner = Vector2.Distance(new Vector2(x, y), new Vector2(width - radius, height - radius)) > radius;
                    
                    texture.SetPixel(x, y, inCorner ? Color.clear : color);
                }
            }
            
            texture.Apply();
            return texture;
        }

        private Texture2D CreateCrosshair(int width, int height)
        {
            Texture2D texture = new Texture2D(width, height);
            int centerX = width / 2;
            int centerY = height / 2;
            int lineWidth = 2;
            int lineLength = width / 4;
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool isHorizontalLine = Mathf.Abs(y - centerY) < lineWidth && 
                                          (Mathf.Abs(x - centerX) > 4 && Mathf.Abs(x - centerX) < lineLength);
                    bool isVerticalLine = Mathf.Abs(x - centerX) < lineWidth && 
                                        (Mathf.Abs(y - centerY) > 4 && Mathf.Abs(y - centerY) < lineLength);
                    
                    texture.SetPixel(x, y, (isHorizontalLine || isVerticalLine) ? Color.green : Color.clear);
                }
            }
            
            texture.Apply();
            return texture;
        }

        private void CreateAndSaveMaterial(string name, Color color, float metallic = 0f, float smoothness = 0.5f)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = color;
            mat.SetFloat("_Metallic", metallic);
            mat.SetFloat("_Glossiness", smoothness);
            
            AssetDatabase.CreateAsset(mat, $"{outputPath}Materials/{name}.mat");
        }

        private GameObject CreateExplosionEffect()
        {
            GameObject effect = new GameObject("FX_Explosion");
            ParticleSystem ps = effect.AddComponent<ParticleSystem>();
            
            var main = ps.main;
            main.duration = 1f;
            main.startLifetime = 0.5f;
            main.startSpeed = 5f;
            main.startSize = 2f;
            main.startColor = new Color(1f, 0.5f, 0f);
            main.maxParticles = 50;
            
            var emission = ps.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 50) });
            
            return effect;
        }

        private GameObject CreateMuzzleFlashEffect()
        {
            GameObject effect = new GameObject("FX_MuzzleFlash");
            ParticleSystem ps = effect.AddComponent<ParticleSystem>();
            
            var main = ps.main;
            main.duration = 0.1f;
            main.startLifetime = 0.05f;
            main.startSpeed = 0f;
            main.startSize = 0.5f;
            main.startColor = new Color(1f, 0.8f, 0.3f);
            
            return effect;
        }

        private GameObject CreateSmokeTrailEffect()
        {
            GameObject effect = new GameObject("FX_SmokeTrail");
            ParticleSystem ps = effect.AddComponent<ParticleSystem>();
            
            var main = ps.main;
            main.duration = 2f;
            main.startLifetime = 1f;
            main.startSpeed = 0.5f;
            main.startSize = 1f;
            main.startColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            
            return effect;
        }

        private void SaveTexture(Texture2D texture, string path)
        {
            byte[] bytes = texture.EncodeToPNG();
            System.IO.File.WriteAllBytes(path, bytes);
        }

        private void EnsureDirectory(string path)
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
