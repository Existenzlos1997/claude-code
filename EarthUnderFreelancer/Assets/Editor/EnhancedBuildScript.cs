using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Editor
{
    /// <summary>
    /// Enhanced build script for professional game distribution
    /// </summary>
    public class EnhancedBuildScript
    {
        private static readonly string[] GameScenes = new string[]
        {
            "Assets/Scenes/Launcher.unity",
            "Assets/Scenes/MainMenu.unity",
            "Assets/Scenes/GameScene.unity",
            "Assets/Scenes/LoadingScene.unity"
        };
        
        private static string BuildPath => Path.Combine(Application.dataPath, "..", "Builds");
        private static string Version => Application.version;
        
        // ============================================================
        // MENU ITEMS
        // ============================================================
        
        [MenuItem("EarthUnderFreelancer/Build/Windows (x64)", priority = 100)]
        public static void BuildWindows()
        {
            BuildWindowsInternal(false);
        }
        
        [MenuItem("EarthUnderFreelancer/Build/Windows (x64) - Development", priority = 101)]
        public static void BuildWindowsDev()
        {
            BuildWindowsInternal(true);
        }
        
        [MenuItem("EarthUnderFreelancer/Build/Android APK", priority = 200)]
        public static void BuildAndroid()
        {
            BuildAndroidInternal(false, false);
        }
        
        [MenuItem("EarthUnderFreelancer/Build/Android AAB (Play Store)", priority = 201)]
        public static void BuildAndroidAAB()
        {
            BuildAndroidInternal(false, true);
        }
        
        [MenuItem("EarthUnderFreelancer/Build/macOS", priority = 300)]
        public static void BuildMacOS()
        {
            BuildMacOSInternal(false);
        }
        
        [MenuItem("EarthUnderFreelancer/Build/Linux", priority = 400)]
        public static void BuildLinux()
        {
            BuildLinuxInternal(false);
        }
        
        [MenuItem("EarthUnderFreelancer/Build/All Platforms", priority = 500)]
        public static void BuildAll()
        {
            bool success = true;
            
            success &= BuildWindowsInternal(false);
            success &= BuildAndroidInternal(false, false);
            success &= BuildAndroidInternal(false, true);
            success &= BuildMacOSInternal(false);
            success &= BuildLinuxInternal(false);
            
            if (success)
            {
                EditorUtility.DisplayDialog("Build Complete", 
                    "All platforms built successfully!\n\nOutput: " + BuildPath, 
                    "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Build Failed", 
                    "Some builds failed. Check the console for details.", 
                    "OK");
            }
        }
        
        [MenuItem("EarthUnderFreelancer/Build/Create Release Package", priority = 600)]
        public static void CreateReleasePackage()
        {
            string releasePath = Path.Combine(BuildPath, "Release", $"v{Version}");
            Directory.CreateDirectory(releasePath);
            
            // Create manifest
            CreateReleaseManifest(releasePath);
            
            // Create checksums
            CreateChecksums(releasePath);
            
            EditorUtility.DisplayDialog("Release Package", 
                $"Release package created at:\n{releasePath}", 
                "OK");
        }
        
        // ============================================================
        // COMMAND LINE ENTRY POINTS
        // ============================================================
        
        public static void BuildWindowsFromCommandLine()
        {
            bool success = BuildWindowsInternal(false);
            EditorApplication.Exit(success ? 0 : 1);
        }
        
        public static void BuildAndroidFromCommandLine()
        {
            bool success = BuildAndroidInternal(false, false);
            EditorApplication.Exit(success ? 0 : 1);
        }
        
        public static void BuildMacOSFromCommandLine()
        {
            bool success = BuildMacOSInternal(false);
            EditorApplication.Exit(success ? 0 : 1);
        }
        
        public static void BuildLinuxFromCommandLine()
        {
            bool success = BuildLinuxInternal(false);
            EditorApplication.Exit(success ? 0 : 1);
        }
        
        // ============================================================
        // BUILD IMPLEMENTATIONS
        // ============================================================
        
        private static bool BuildWindowsInternal(bool development)
        {
            string outputPath = Path.Combine(BuildPath, "Windows", "EarthUnderFreelancer.exe");
            
            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = GameScenes,
                locationPathName = outputPath,
                target = BuildTarget.StandaloneWindows64,
                options = development ? BuildOptions.Development | BuildOptions.AllowDebugging : BuildOptions.None
            };
            
            // Set player settings
            PlayerSettings.productName = "EarthUnderFreelancer";
            PlayerSettings.companyName = "EarthUnder Studios";
            PlayerSettings.bundleVersion = Version;
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.IL2CPP);
            PlayerSettings.SetArchitecture(BuildTargetGroup.Standalone, 1); // x64
            
            return ExecuteBuild(options, "Windows");
        }
        
        private static bool BuildAndroidInternal(bool development, bool appBundle)
        {
            string extension = appBundle ? "aab" : "apk";
            string outputPath = Path.Combine(BuildPath, "Android", $"EarthUnderFreelancer.{extension}");
            
            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = GameScenes,
                locationPathName = outputPath,
                target = BuildTarget.Android,
                options = development ? BuildOptions.Development | BuildOptions.AllowDebugging : BuildOptions.None
            };
            
            // Set player settings
            PlayerSettings.productName = "EarthUnderFreelancer";
            PlayerSettings.companyName = "EarthUnder Studios";
            PlayerSettings.bundleVersion = Version;
            PlayerSettings.Android.bundleVersionCode = GetAndroidVersionCode();
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.earthunderstudios.earthunderfreelancer");
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel33;
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;
            
            EditorUserBuildSettings.buildAppBundle = appBundle;
            
            // Set keystore if available
            string keystorePath = Path.Combine(Application.dataPath, "..", "Keystore", "earthunderfreelancer.keystore");
            if (File.Exists(keystorePath))
            {
                PlayerSettings.Android.keystoreName = keystorePath;
                PlayerSettings.Android.keyaliasName = "earthunderfreelancer";
                // Note: Passwords should be provided securely, not hardcoded
            }
            
            return ExecuteBuild(options, appBundle ? "Android (AAB)" : "Android (APK)");
        }
        
        private static bool BuildMacOSInternal(bool development)
        {
            string outputPath = Path.Combine(BuildPath, "macOS", "EarthUnderFreelancer.app");
            
            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = GameScenes,
                locationPathName = outputPath,
                target = BuildTarget.StandaloneOSX,
                options = development ? BuildOptions.Development | BuildOptions.AllowDebugging : BuildOptions.None
            };
            
            // Set player settings
            PlayerSettings.productName = "EarthUnderFreelancer";
            PlayerSettings.companyName = "EarthUnder Studios";
            PlayerSettings.bundleVersion = Version;
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.IL2CPP);
            
            return ExecuteBuild(options, "macOS");
        }
        
        private static bool BuildLinuxInternal(bool development)
        {
            string outputPath = Path.Combine(BuildPath, "Linux", "EarthUnderFreelancer");
            
            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = GameScenes,
                locationPathName = outputPath,
                target = BuildTarget.StandaloneLinux64,
                options = development ? BuildOptions.Development | BuildOptions.AllowDebugging : BuildOptions.None
            };
            
            // Set player settings
            PlayerSettings.productName = "EarthUnderFreelancer";
            PlayerSettings.companyName = "EarthUnder Studios";
            PlayerSettings.bundleVersion = Version;
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.IL2CPP);
            
            return ExecuteBuild(options, "Linux");
        }
        
        private static bool ExecuteBuild(BuildPlayerOptions options, string platformName)
        {
            Debug.Log($"[Build] Starting {platformName} build...");
            
            // Ensure output directory exists
            string directory = Path.GetDirectoryName(options.locationPathName);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            // Execute build
            BuildReport report = BuildPipeline.BuildPlayer(options);
            BuildSummary summary = report.summary;
            
            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"[Build] {platformName} build succeeded: {summary.totalSize / (1024 * 1024):F1} MB in {summary.totalTime.TotalSeconds:F1}s");
                return true;
            }
            else
            {
                Debug.LogError($"[Build] {platformName} build failed with {summary.totalErrors} errors");
                
                foreach (var step in report.steps)
                {
                    foreach (var message in step.messages)
                    {
                        if (message.type == LogType.Error)
                        {
                            Debug.LogError($"[Build Error] {message.content}");
                        }
                    }
                }
                
                return false;
            }
        }
        
        private static int GetAndroidVersionCode()
        {
            // Convert version string to version code
            // e.g., "1.2.3" -> 10203
            string[] parts = Version.Split('.');
            int versionCode = 0;
            
            if (parts.Length >= 1 && int.TryParse(parts[0], out int major))
                versionCode += major * 10000;
            if (parts.Length >= 2 && int.TryParse(parts[1], out int minor))
                versionCode += minor * 100;
            if (parts.Length >= 3 && int.TryParse(parts[2], out int patch))
                versionCode += patch;
            
            return versionCode > 0 ? versionCode : 1;
        }
        
        private static void CreateReleaseManifest(string releasePath)
        {
            var manifest = new
            {
                version = Version,
                buildDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                platforms = new[]
                {
                    new { name = "Windows", file = "EarthUnderFreelancer_Setup.exe" },
                    new { name = "Android APK", file = "EarthUnderFreelancer.apk" },
                    new { name = "Android AAB", file = "EarthUnderFreelancer.aab" },
                    new { name = "macOS", file = "EarthUnderFreelancer.dmg" },
                    new { name = "Linux", file = "EarthUnderFreelancer_Linux.zip" }
                }
            };
            
            string json = JsonUtility.ToJson(manifest, true);
            File.WriteAllText(Path.Combine(releasePath, "manifest.json"), json);
        }
        
        private static void CreateChecksums(string releasePath)
        {
            List<string> checksums = new List<string>();
            
            foreach (string file in Directory.GetFiles(releasePath))
            {
                if (file.EndsWith(".json") || file.EndsWith(".txt")) continue;
                
                using (var sha256 = System.Security.Cryptography.SHA256.Create())
                {
                    using (var stream = File.OpenRead(file))
                    {
                        byte[] hash = sha256.ComputeHash(stream);
                        string hashString = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                        checksums.Add($"{hashString}  {Path.GetFileName(file)}");
                    }
                }
            }
            
            File.WriteAllLines(Path.Combine(releasePath, "SHA256SUMS.txt"), checksums);
        }
    }
}
