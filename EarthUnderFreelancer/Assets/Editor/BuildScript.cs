using UnityEngine;
using UnityEditor;

namespace EarthUnderFreelancer.Editor
{
    /// <summary>
    /// Build automation script for EarthUnder Freelancer
    /// </summary>
    public class BuildScript
    {
        private static string[] GetScenes()
        {
            return new string[] 
            {
                "Assets/Scenes/MainMenu.unity",
                "Assets/Scenes/GameScene.unity"
            };
        }

        [MenuItem("Build/Build Windows")]
        public static void BuildWindows()
        {
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = GetScenes();
            buildPlayerOptions.locationPathName = "Builds/Windows/EarthUnderFreelancer.exe";
            buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
            buildPlayerOptions.options = BuildOptions.None;

            BuildPipeline.BuildPlayer(buildPlayerOptions);
            Debug.Log("Windows Build Complete!");
        }

        [MenuItem("Build/Build Android APK")]
        public static void BuildAndroid()
        {
            EditorUserBuildSettings.buildAppBundle = false;
            
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = GetScenes();
            buildPlayerOptions.locationPathName = "Builds/Android/EarthUnderFreelancer.apk";
            buildPlayerOptions.target = BuildTarget.Android;
            buildPlayerOptions.options = BuildOptions.None;

            BuildPipeline.BuildPlayer(buildPlayerOptions);
            Debug.Log("Android APK Build Complete!");
        }

        [MenuItem("Build/Build Android AAB (Play Store)")]
        public static void BuildAndroidAAB()
        {
            EditorUserBuildSettings.buildAppBundle = true;
            
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = GetScenes();
            buildPlayerOptions.locationPathName = "Builds/Android/EarthUnderFreelancer.aab";
            buildPlayerOptions.target = BuildTarget.Android;
            buildPlayerOptions.options = BuildOptions.None;

            BuildPipeline.BuildPlayer(buildPlayerOptions);
            Debug.Log("Android AAB Build Complete!");
        }

        [MenuItem("Build/Build All")]
        public static void BuildAll()
        {
            BuildWindows();
            BuildAndroid();
            Debug.Log("All Builds Complete!");
        }
    }
}
