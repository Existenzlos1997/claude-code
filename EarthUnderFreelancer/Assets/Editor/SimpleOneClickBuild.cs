using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace EarthUnderFreelancer.Editor
{
    /// <summary>
    /// Simple One-Click Build System - Creates portable EXE directly
    /// No installer needed - just run the EXE!
    /// </summary>
    public class SimpleOneClickBuild : EditorWindow
    {
        private static string buildPath = "Builds";
        private static string gameName = "EarthUnderFreelancer";
        
        [MenuItem("EarthUnderFreelancer/🚀 Build Portable Windows EXE", false, 1)]
        public static void BuildWindowsPortable()
        {
            BuildPortableGame(BuildTarget.StandaloneWindows64, "Windows");
        }

        [MenuItem("EarthUnderFreelancer/📱 Build Android APK", false, 2)]
        public static void BuildAndroidAPK()
        {
            BuildAndroid();
        }

        [MenuItem("EarthUnderFreelancer/🎮 Build All Platforms", false, 20)]
        public static void BuildAll()
        {
            BuildWindowsPortable();
            BuildAndroidAPK();
            EditorUtility.DisplayDialog("Build Complete", 
                "All builds finished!\n\nWindows: Builds/Windows/EarthUnderFreelancer.exe\nAndroid: Builds/Android/EarthUnderFreelancer.apk", 
                "OK");
        }

        private static void BuildPortableGame(BuildTarget target, string platformName)
        {
            string outputPath = Path.Combine(buildPath, platformName);
            string exePath = Path.Combine(outputPath, $"{gameName}.exe");

            // Ensure directory exists
            Directory.CreateDirectory(outputPath);

            // Get all scenes
            string[] scenes = GetEnabledScenes();
            if (scenes.Length == 0)
            {
                scenes = CreateDefaultScenes();
            }

            // Build options
            BuildPlayerOptions buildOptions = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = exePath,
                target = target,
                options = BuildOptions.None
            };

            // Set player settings for portable build
            PlayerSettings.companyName = "EarthUnderFreelancer Studio";
            PlayerSettings.productName = gameName;
            PlayerSettings.bundleVersion = "1.0.0";
            PlayerSettings.fullScreenMode = FullScreenMode.FullScreenWindow;
            PlayerSettings.defaultIsNativeResolution = true;
            PlayerSettings.runInBackground = true;

            // Build
            Debug.Log($"Building {platformName}...");
            BuildReport report = BuildPipeline.BuildPlayer(buildOptions);

            if (report.summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"✅ Build succeeded: {exePath}");
                Debug.Log($"📁 Total size: {report.summary.totalSize / 1024 / 1024} MB");
                
                // Create portable readme
                CreatePortableReadme(outputPath);
                
                EditorUtility.DisplayDialog("Build Successful!", 
                    $"Portable EXE created!\n\nLocation: {exePath}\n\nYou can run this directly - no installation needed!", 
                    "Open Folder");
                
                // Open folder
                EditorUtility.RevealInFinder(exePath);
            }
            else
            {
                Debug.LogError($"❌ Build failed: {report.summary.result}");
                EditorUtility.DisplayDialog("Build Failed", 
                    $"Build failed with {report.summary.totalErrors} errors.\nCheck console for details.", 
                    "OK");
            }
        }

        private static void BuildAndroid()
        {
            string outputPath = Path.Combine(buildPath, "Android");
            string apkPath = Path.Combine(outputPath, $"{gameName}.apk");

            Directory.CreateDirectory(outputPath);

            string[] scenes = GetEnabledScenes();
            if (scenes.Length == 0)
            {
                scenes = CreateDefaultScenes();
            }

            // Android player settings
            PlayerSettings.Android.bundleVersionCode = 1;
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.earthunderfreelancer.game");
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel33;

            BuildPlayerOptions buildOptions = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = apkPath,
                target = BuildTarget.Android,
                options = BuildOptions.None
            };

            Debug.Log("Building Android APK...");
            BuildReport report = BuildPipeline.BuildPlayer(buildOptions);

            if (report.summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"✅ Android build succeeded: {apkPath}");
                EditorUtility.RevealInFinder(apkPath);
            }
            else
            {
                Debug.LogError($"❌ Android build failed");
            }
        }

        private static string[] GetEnabledScenes()
        {
            var scenes = new System.Collections.Generic.List<string>();
            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                {
                    scenes.Add(scene.path);
                }
            }
            return scenes.ToArray();
        }

        private static string[] CreateDefaultScenes()
        {
            // Create a minimal scene if none exist
            string scenePath = "Assets/Scenes/MainScene.unity";
            
            if (!File.Exists(scenePath))
            {
                Directory.CreateDirectory("Assets/Scenes");
                
                // Create and save a basic scene
                var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(
                    UnityEditor.SceneManagement.NewSceneSetup.DefaultGameObjects);
                
                // Add game setup
                GameObject gameSetup = new GameObject("GameSetup");
                // Note: GameSceneSetup component would be added here if available
                
                UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene, scenePath);
            }

            // Add to build settings
            EditorBuildSettingsScene[] buildScenes = new EditorBuildSettingsScene[]
            {
                new EditorBuildSettingsScene(scenePath, true)
            };
            EditorBuildSettings.scenes = buildScenes;

            return new string[] { scenePath };
        }

        private static void CreatePortableReadme(string outputPath)
        {
            string readmePath = Path.Combine(outputPath, "README.txt");
            string content = @"═══════════════════════════════════════════════════════════════
                    EARTHUNDERFREELANCER
              Combat Flight MMORPG - Portable Version
═══════════════════════════════════════════════════════════════

▶ SO STARTEST DU DAS SPIEL:

   Doppelklick auf: EarthUnderFreelancer.exe

   Das war's! Keine Installation erforderlich.


📋 SYSTEMANFORDERUNGEN:

   • Windows 10/11 (64-bit)
   • 4 GB RAM
   • DirectX 11 kompatible Grafikkarte
   • 2 GB freier Speicherplatz


🎮 STEUERUNG:

   WASD / Pfeiltasten - Flugzeug steuern
   Maus - Zielen
   Linke Maustaste - Schießen
   Leertaste - Boost
   Tab - Karte öffnen
   ESC - Pausenmenü
   I - Inventar
   T - Trading


🌐 ONLINE-FUNKTIONEN:

   • Account erstellen oder als Gast spielen
   • Mit Freunden spielen
   • Fraktionen beitreten
   • Territorien erobern
   • Handeln und Schmuggeln


📁 SPEICHERDATEN:

   Spielstände werden in deinem Benutzerordner gespeichert:
   C:\Users\[DeinName]\AppData\LocalLow\EarthUnderFreelancer Studio\


⚠ BEI PROBLEMEN:

   1. Stelle sicher, dass deine Grafiktreiber aktuell sind
   2. Führe das Spiel als Administrator aus
   3. Überprüfe die Windows Firewall-Einstellungen


═══════════════════════════════════════════════════════════════
                    Viel Spaß beim Spielen!
═══════════════════════════════════════════════════════════════
";
            File.WriteAllText(readmePath, content);
        }

        [MenuItem("EarthUnderFreelancer/📂 Open Build Folder", false, 50)]
        public static void OpenBuildFolder()
        {
            string fullPath = Path.GetFullPath(buildPath);
            if (Directory.Exists(fullPath))
            {
                EditorUtility.RevealInFinder(fullPath);
            }
            else
            {
                Directory.CreateDirectory(fullPath);
                EditorUtility.RevealInFinder(fullPath);
            }
        }

        [MenuItem("EarthUnderFreelancer/ℹ️ Build Info", false, 51)]
        public static void ShowBuildInfo()
        {
            string info = @"EarthUnderFreelancer Build System
═══════════════════════════════════

EINFACHER BUILD:
1. Klick auf 'Build Portable Windows EXE'
2. Warte bis der Build fertig ist
3. Die EXE findest du im 'Builds/Windows' Ordner
4. Doppelklick zum Starten - fertig!

FEATURES:
• Portable - keine Installation nötig
• Alle Dateien in einem Ordner
• Einfach kopieren und teilen
• Läuft auf jedem Windows 10/11 PC

ANDROID:
• Klick auf 'Build Android APK'
• APK auf Handy übertragen
• Installation erlauben
• Spielen!";

            EditorUtility.DisplayDialog("Build Information", info, "OK");
        }
    }
}
