using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Graphics
{
    /// <summary>
    /// Advanced graphics settings manager for AAA quality visuals
    /// Supports Built-in, URP, and HDRP pipelines
    /// </summary>
    public class GraphicsSettingsManager : MonoBehaviour
    {
        public enum QualityPreset
        {
            Low,
            Medium,
            High,
            Ultra,
            Custom
        }
        
        [System.Serializable]
        public class GraphicsConfig
        {
            [Header("Resolution")]
            public int resolutionWidth = 1920;
            public int resolutionHeight = 1080;
            public bool fullscreen = true;
            public int targetFrameRate = 60;
            
            [Header("Quality")]
            public QualityPreset preset = QualityPreset.High;
            public int shadowQuality = 2; // 0-4
            public float shadowDistance = 150f;
            public int shadowCascades = 4;
            public int textureQuality = 0; // 0 = Full, 1-3 = lower
            public int antiAliasing = 4; // 0, 2, 4, 8
            public int anisotropicFiltering = 2; // 0-2
            
            [Header("LOD")]
            public float lodBias = 1f;
            public int maximumLODLevel = 0;
            
            [Header("Post Processing")]
            public bool usePostProcessing = true;
            public bool bloomEnabled = true;
            public bool ambientOcclusionEnabled = true;
            public bool motionBlurEnabled = false;
            public bool depthOfFieldEnabled = true;
            public bool chromaticAberrationEnabled = false;
            
            [Header("Advanced")]
            public bool vSyncEnabled = true;
            public bool hdrEnabled = false;
            public float renderScale = 1f;
        }
        
        [SerializeField] private GraphicsConfig currentConfig = new GraphicsConfig();
        
        public static GraphicsSettingsManager Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadSettings();
                ApplySettings();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        /// <summary>
        /// Apply all graphics settings
        /// </summary>
        public void ApplySettings()
        {
            ApplyResolution();
            ApplyQualitySettings();
            ApplyShadowSettings();
            ApplyTextureSettings();
            ApplyPostProcessing();
            ApplyAdvancedSettings();
            
            SaveSettings();
        }
        
        private void ApplyResolution()
        {
            Screen.SetResolution(
                currentConfig.resolutionWidth,
                currentConfig.resolutionHeight,
                currentConfig.fullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed
            );
            
            Application.targetFrameRate = currentConfig.targetFrameRate;
        }
        
        private void ApplyQualitySettings()
        {
            switch (currentConfig.preset)
            {
                case QualityPreset.Low:
                    QualitySettings.SetQualityLevel(0);
                    break;
                case QualityPreset.Medium:
                    QualitySettings.SetQualityLevel(2);
                    break;
                case QualityPreset.High:
                    QualitySettings.SetQualityLevel(4);
                    break;
                case QualityPreset.Ultra:
                    QualitySettings.SetQualityLevel(5);
                    break;
            }
        }
        
        private void ApplyShadowSettings()
        {
            switch (currentConfig.shadowQuality)
            {
                case 0:
                    QualitySettings.shadows = ShadowQuality.Disable;
                    break;
                case 1:
                    QualitySettings.shadows = ShadowQuality.HardOnly;
                    break;
                case 2:
                case 3:
                case 4:
                    QualitySettings.shadows = ShadowQuality.All;
                    break;
            }
            
            QualitySettings.shadowDistance = currentConfig.shadowDistance;
            QualitySettings.shadowCascades = currentConfig.shadowCascades;
            QualitySettings.shadowResolution = (ShadowResolution)currentConfig.shadowQuality;
        }
        
        private void ApplyTextureSettings()
        {
            QualitySettings.masterTextureLimit = currentConfig.textureQuality;
            
            switch (currentConfig.anisotropicFiltering)
            {
                case 0:
                    QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
                    break;
                case 1:
                    QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
                    break;
                case 2:
                    QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
                    break;
            }
        }
        
        private void ApplyPostProcessing()
        {
            // Post-processing would be applied via Volume framework in URP/HDRP
            // Or post-processing stack in Built-in pipeline
            
            #if USING_URP || USING_HDRP
            // Get Volume from scene
            var volume = FindObjectOfType<UnityEngine.Rendering.Volume>();
            if (volume != null)
            {
                // Configure volume profile based on settings
                // Implementation depends on specific post-processing effects
            }
            #endif
        }
        
        private void ApplyAdvancedSettings()
        {
            QualitySettings.vSyncCount = currentConfig.vSyncEnabled ? 1 : 0;
            QualitySettings.lodBias = currentConfig.lodBias;
            QualitySettings.maximumLODLevel = currentConfig.maximumLODLevel;
            QualitySettings.antiAliasing = currentConfig.antiAliasing;
            
            // Render scale (for dynamic resolution)
            if (currentConfig.renderScale != 1f)
            {
                // Implementation depends on pipeline
                #if USING_URP
                // UniversalRenderPipeline.asset.renderScale = currentConfig.renderScale;
                #endif
            }
        }
        
        /// <summary>
        /// Set quality preset and apply
        /// </summary>
        public void SetPreset(QualityPreset preset)
        {
            currentConfig.preset = preset;
            
            switch (preset)
            {
                case QualityPreset.Low:
                    currentConfig.shadowQuality = 0;
                    currentConfig.shadowDistance = 50f;
                    currentConfig.textureQuality = 2;
                    currentConfig.antiAliasing = 0;
                    currentConfig.usePostProcessing = false;
                    currentConfig.renderScale = 0.75f;
                    break;
                    
                case QualityPreset.Medium:
                    currentConfig.shadowQuality = 1;
                    currentConfig.shadowDistance = 100f;
                    currentConfig.textureQuality = 1;
                    currentConfig.antiAliasing = 2;
                    currentConfig.usePostProcessing = true;
                    currentConfig.renderScale = 0.9f;
                    break;
                    
                case QualityPreset.High:
                    currentConfig.shadowQuality = 2;
                    currentConfig.shadowDistance = 150f;
                    currentConfig.textureQuality = 0;
                    currentConfig.antiAliasing = 4;
                    currentConfig.usePostProcessing = true;
                    currentConfig.renderScale = 1f;
                    break;
                    
                case QualityPreset.Ultra:
                    currentConfig.shadowQuality = 4;
                    currentConfig.shadowDistance = 250f;
                    currentConfig.textureQuality = 0;
                    currentConfig.antiAliasing = 8;
                    currentConfig.usePostProcessing = true;
                    currentConfig.bloomEnabled = true;
                    currentConfig.ambientOcclusionEnabled = true;
                    currentConfig.renderScale = 1f;
                    currentConfig.hdrEnabled = true;
                    break;
            }
            
            ApplySettings();
        }
        
        /// <summary>
        /// Auto-detect optimal settings based on hardware
        /// </summary>
        public void AutoDetectSettings()
        {
            int vram = SystemInfo.graphicsMemorySize;
            int cpuCores = SystemInfo.processorCount;
            
            if (vram < 2048 || cpuCores < 4)
            {
                SetPreset(QualityPreset.Low);
            }
            else if (vram < 4096 || cpuCores < 6)
            {
                SetPreset(QualityPreset.Medium);
            }
            else if (vram < 8192)
            {
                SetPreset(QualityPreset.High);
            }
            else
            {
                SetPreset(QualityPreset.Ultra);
            }
        }
        
        private void SaveSettings()
        {
            PlayerPrefs.SetInt("Graphics_ResWidth", currentConfig.resolutionWidth);
            PlayerPrefs.SetInt("Graphics_ResHeight", currentConfig.resolutionHeight);
            PlayerPrefs.SetInt("Graphics_Fullscreen", currentConfig.fullscreen ? 1 : 0);
            PlayerPrefs.SetInt("Graphics_Preset", (int)currentConfig.preset);
            PlayerPrefs.SetInt("Graphics_ShadowQuality", currentConfig.shadowQuality);
            PlayerPrefs.SetFloat("Graphics_ShadowDistance", currentConfig.shadowDistance);
            PlayerPrefs.SetInt("Graphics_AntiAliasing", currentConfig.antiAliasing);
            PlayerPrefs.SetInt("Graphics_VSync", currentConfig.vSyncEnabled ? 1 : 0);
            PlayerPrefs.Save();
        }
        
        private void LoadSettings()
        {
            currentConfig.resolutionWidth = PlayerPrefs.GetInt("Graphics_ResWidth", 1920);
            currentConfig.resolutionHeight = PlayerPrefs.GetInt("Graphics_ResHeight", 1080);
            currentConfig.fullscreen = PlayerPrefs.GetInt("Graphics_Fullscreen", 1) == 1;
            currentConfig.preset = (QualityPreset)PlayerPrefs.GetInt("Graphics_Preset", 2);
            currentConfig.shadowQuality = PlayerPrefs.GetInt("Graphics_ShadowQuality", 2);
            currentConfig.shadowDistance = PlayerPrefs.GetFloat("Graphics_ShadowDistance", 150f);
            currentConfig.antiAliasing = PlayerPrefs.GetInt("Graphics_AntiAliasing", 4);
            currentConfig.vSyncEnabled = PlayerPrefs.GetInt("Graphics_VSync", 1) == 1;
        }
    }
}
