using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using System;
using System.IO;

namespace EarthUnderFreelancer.Editor
{
    /// <summary>
    /// Build automation system for EarthUnderFreelancer
    /// Implements VERBESSERUNGSPLAN Phase 1 - Build Automation
    /// Provides one-click builds for all platforms with proper configuration
    /// </summary>
    public class BuildAutomation : EditorWindow
    {
        private static string buildPath = "Builds/";
        private static bool developmentBuild = false;
        private static bool autoRunBuild = false;
        
        [MenuItem("EarthUnderFreelancer/Build/Build Automation")]
        public static void ShowWindow()
        {
            GetWindow<BuildAutomation>("Build Automation");
        }

        private void OnGUI()
        {
            GUILayout.Label("Build Automation System", EditorStyles.boldLabel);
            GUILayout.Label("One-click builds for all platforms", EditorStyles.miniLabel);
            
            EditorGUILayout.Space();
            
            // Settings
            GUILayout.Label("Build Settings:", EditorStyles.boldLabel);
            buildPath = EditorGUILayout.TextField("Build Path:", buildPath);
            developmentBuild = EditorGUILayout.Toggle("Development Build:", developmentBuild);
            autoRunBuild = EditorGUILayout.Toggle("Auto-Run After Build:", autoRunBuild);
            
            EditorGUILayout.Space();
            
            // Build buttons
            GUILayout.Label("Quick Build:", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Build Windows (x64)", GUILayout.Height(35)))
            {
                BuildWindows();
            }
            
            if (GUILayout.Button("Build Android (APK)", GUILayout.Height(35)))
            {
                BuildAndroid();
            }
            
            if (GUILayout.Button("Build Linux", GUILayout.Height(35)))
            {
                BuildLinux();
            }
            
            if (GUILayout.Button("Build macOS", GUILayout.Height(35)))
            {
                BuildMacOS();
            }
            
            EditorGUILayout.Space();
            
            GUILayout.Label("Batch Build:", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Build All Platforms", GUILayout.Height(35)))
            {
                BuildAllPlatforms();
            }
            
            EditorGUILayout.Space();
            
            // Info
            EditorGUILayout.HelpBox(
                "Automated build system with:\n" +
                "- One-click builds for all platforms\n" +
                "- Automatic versioning\n" +
                "- Build reports\n" +
                "- Development/Release configurations\n\n" +
                "Builds are saved to: " + Path.GetFullPath(buildPath),
                MessageType.Info);
        }

        [MenuItem("EarthUnderFreelancer/Build/Quick Build Windows")]
        private static void BuildWindows()
        {
            string productName = PlayerSettings.productName;
            string version = PlayerSettings.bundleVersion;
            string buildName = $"{productName}_v{version}_Windows_x64";
            string fullPath = Path.Combine(buildPath, "Windows", buildName, $"{productName}.exe");
            
            BuildPlayerOptions buildOptions = new BuildPlayerOptions
            {
                scenes = GetScenePaths(),
                locationPathName = fullPath,
                target = BuildTarget.StandaloneWindows64,
                options = GetBuildOptions()
            };
            
            Debug.Log($"Starting Windows build: {buildName}");
            BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
            ProcessBuildReport(report, "Windows");
        }

        [MenuItem("EarthUnderFreelancer/Build/Quick Build Android")]
        private static void BuildAndroid()
        {
            string productName = PlayerSettings.productName;
            string version = PlayerSettings.bundleVersion;
            string buildName = $"{productName}_v{version}_Android";
            string fullPath = Path.Combine(buildPath, "Android", $"{buildName}.apk");
            
            // Configure Android settings
            PlayerSettings.Android.bundleVersionCode++;
            
            BuildPlayerOptions buildOptions = new BuildPlayerOptions
            {
                scenes = GetScenePaths(),
                locationPathName = fullPath,
                target = BuildTarget.Android,
                options = GetBuildOptions()
            };
            
            Debug.Log($"Starting Android build: {buildName}");
            BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
            ProcessBuildReport(report, "Android");
        }

        private static void BuildLinux()
        {
            string productName = PlayerSettings.productName;
            string version = PlayerSettings.bundleVersion;
            string buildName = $"{productName}_v{version}_Linux_x64";
            string fullPath = Path.Combine(buildPath, "Linux", buildName, productName);
            
            BuildPlayerOptions buildOptions = new BuildPlayerOptions
            {
                scenes = GetScenePaths(),
                locationPathName = fullPath,
                target = BuildTarget.StandaloneLinux64,
                options = GetBuildOptions()
            };
            
            Debug.Log($"Starting Linux build: {buildName}");
            BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
            ProcessBuildReport(report, "Linux");
        }

        private static void BuildMacOS()
        {
            string productName = PlayerSettings.productName;
            string version = PlayerSettings.bundleVersion;
            string buildName = $"{productName}_v{version}_macOS";
            string fullPath = Path.Combine(buildPath, "macOS", $"{buildName}.app");
            
            BuildPlayerOptions buildOptions = new BuildPlayerOptions
            {
                scenes = GetScenePaths(),
                locationPathName = fullPath,
                target = BuildTarget.StandaloneOSX,
                options = GetBuildOptions()
            };
            
            Debug.Log($"Starting macOS build: {buildName}");
            BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
            ProcessBuildReport(report, "macOS");
        }

        private static void BuildAllPlatforms()
        {
            Debug.Log("Starting batch build for all platforms...");
            
            BuildWindows();
            BuildLinux();
            BuildMacOS();
            BuildAndroid();
            
            Debug.Log("Batch build complete!");
            EditorUtility.DisplayDialog("Build Complete", 
                "All platform builds finished!\nCheck the Builds folder for results.", "OK");
        }

        private static string[] GetScenePaths()
        {
            // Get all scenes from build settings
            string[] scenes = new string[EditorBuildSettings.scenes.Length];
            for (int i = 0; i < scenes.Length; i++)
            {
                scenes[i] = EditorBuildSettings.scenes[i].path;
            }
            
            // If no scenes in build settings, try to find scenes
            if (scenes.Length == 0)
            {
                string[] foundScenes = System.IO.Directory.GetFiles("Assets/Scenes", "*.unity", SearchOption.AllDirectories);
                if (foundScenes.Length > 0)
                {
                    Debug.LogWarning("No scenes in build settings. Using scenes from Assets/Scenes/");
                    return foundScenes;
                }
            }
            
            return scenes;
        }

        private static BuildOptions GetBuildOptions()
        {
            BuildOptions options = BuildOptions.None;
            
            if (developmentBuild)
            {
                options |= BuildOptions.Development;
                options |= BuildOptions.AllowDebugging;
            }
            
            if (autoRunBuild)
            {
                options |= BuildOptions.AutoRunPlayer;
            }
            
            return options;
        }

        private static void ProcessBuildReport(BuildReport report, string platform)
        {
            BuildSummary summary = report.summary;
            
            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"✅ {platform} build succeeded!");
                Debug.Log($"Build size: {FormatBytes(summary.totalSize)}");
                Debug.Log($"Build time: {summary.totalTime.TotalSeconds:F2} seconds");
                Debug.Log($"Output: {summary.outputPath}");
                
                // Show success dialog
                EditorUtility.DisplayDialog("Build Succeeded", 
                    $"{platform} build completed successfully!\n\n" +
                    $"Size: {FormatBytes(summary.totalSize)}\n" +
                    $"Time: {summary.totalTime.TotalSeconds:F2}s\n" +
                    $"Location: {summary.outputPath}", 
                    "OK");
            }
            else if (summary.result == BuildResult.Failed)
            {
                Debug.LogError($"❌ {platform} build failed!");
                Debug.LogError($"Errors: {summary.totalErrors}");
                Debug.LogError($"Warnings: {summary.totalWarnings}");
                
                EditorUtility.DisplayDialog("Build Failed", 
                    $"{platform} build failed!\n\n" +
                    $"Errors: {summary.totalErrors}\n" +
                    $"Check console for details.", 
                    "OK");
            }
            else if (summary.result == BuildResult.Cancelled)
            {
                Debug.LogWarning($"⚠️ {platform} build cancelled by user");
            }
            
            // Log detailed report
            LogDetailedReport(report);
        }

        private static void LogDetailedReport(BuildReport report)
        {
            BuildSummary summary = report.summary;
            
            Debug.Log("=== BUILD REPORT ===");
            Debug.Log($"Platform: {summary.platform}");
            Debug.Log($"Result: {summary.result}");
            Debug.Log($"Total Size: {FormatBytes(summary.totalSize)}");
            Debug.Log($"Build Time: {summary.totalTime}");
            Debug.Log($"Errors: {summary.totalErrors}");
            Debug.Log($"Warnings: {summary.totalWarnings}");
            
            // File count
            BuildStep[] steps = report.steps;
            Debug.Log($"Build Steps: {steps.Length}");
            
            Debug.Log("====================");
        }

        private static string FormatBytes(ulong bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            
            return $"{len:F2} {sizes[order]}";
        }

        // Quick build menu items
        [MenuItem("EarthUnderFreelancer/Build/Quick Actions/Increment Version")]
        private static void IncrementVersion()
        {
            string currentVersion = PlayerSettings.bundleVersion;
            string[] parts = currentVersion.Split('.');
            
            if (parts.Length >= 3)
            {
                int patch = int.Parse(parts[2]);
                patch++;
                parts[2] = patch.ToString();
                
                string newVersion = string.Join(".", parts);
                PlayerSettings.bundleVersion = newVersion;
                
                Debug.Log($"Version updated: {currentVersion} → {newVersion}");
            }
        }

        [MenuItem("EarthUnderFreelancer/Build/Quick Actions/Open Build Folder")]
        private static void OpenBuildFolder()
        {
            string fullPath = Path.GetFullPath(buildPath);
            
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }
            
            EditorUtility.RevealInFinder(fullPath);
        }

        [MenuItem("EarthUnderFreelancer/Build/Quick Actions/Clean Build Folder")]
        private static void CleanBuildFolder()
        {
            if (EditorUtility.DisplayDialog("Clean Build Folder", 
                "This will delete all builds. Continue?", "Yes", "Cancel"))
            {
                string fullPath = Path.GetFullPath(buildPath);
                
                if (Directory.Exists(fullPath))
                {
                    Directory.Delete(fullPath, true);
                    Directory.CreateDirectory(fullPath);
                    Debug.Log("Build folder cleaned");
                }
            }
        }
    }
}
