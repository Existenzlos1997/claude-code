using UnityEngine;
using System;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Graphics
{
    /// <summary>
    /// AAA-quality shader management system for dynamic visual effects
    /// Manages clouds, water, terrain, atmosphere, and post-processing
    /// </summary>
    public class ShaderManager : MonoBehaviour
    {
        public static ShaderManager Instance { get; private set; }

        [Header("Global Shader Settings")]
        [SerializeField] private bool enableHDR = true;
        [SerializeField] private bool enableVolumetrics = true;
        [SerializeField] private QualityPreset qualityPreset = QualityPreset.High;

        [Header("Atmosphere")]
        [SerializeField] private Color skyColorDay = new Color(0.4f, 0.6f, 1f);
        [SerializeField] private Color skyColorSunset = new Color(1f, 0.5f, 0.2f);
        [SerializeField] private Color skyColorNight = new Color(0.02f, 0.02f, 0.08f);
        [SerializeField] private float atmosphereDensity = 1f;
        [SerializeField] private float rayleighScattering = 1f;
        [SerializeField] private float mieScattering = 0.01f;

        [Header("Volumetric Clouds")]
        [SerializeField] private float cloudDensity = 0.5f;
        [SerializeField] private float cloudCoverage = 0.6f;
        [SerializeField] private float cloudSpeed = 0.01f;
        [SerializeField] private float cloudHeight = 2000f;
        [SerializeField] private float cloudThickness = 500f;
        [SerializeField] private int cloudRaymarchSteps = 64;

        [Header("Water")]
        [SerializeField] private Color waterColor = new Color(0.1f, 0.3f, 0.5f);
        [SerializeField] private Color waterDeepColor = new Color(0.02f, 0.08f, 0.15f);
        [SerializeField] private float waterTransparency = 0.8f;
        [SerializeField] private float waveHeight = 1f;
        [SerializeField] private float waveSpeed = 1f;
        [SerializeField] private float foamIntensity = 0.5f;
        [SerializeField] private float refractionStrength = 0.1f;

        [Header("Terrain")]
        [SerializeField] private float terrainDetailDistance = 500f;
        [SerializeField] private float grassDensity = 1f;
        [SerializeField] private float snowLineHeight = 3000f;
        [SerializeField] private float terrainNormalStrength = 1f;

        [Header("Post Processing")]
        [SerializeField] private float bloomIntensity = 0.5f;
        [SerializeField] private float bloomThreshold = 0.9f;
        [SerializeField] private float ambientOcclusionIntensity = 0.5f;
        [SerializeField] private float ambientOcclusionRadius = 0.5f;
        [SerializeField] private float motionBlurIntensity = 0.5f;
        [SerializeField] private float chromaticAberration = 0.1f;
        [SerializeField] private float vignetteIntensity = 0.3f;
        [SerializeField] private float filmGrain = 0.1f;
        [SerializeField] private float colorGradingTemperature = 0f;
        [SerializeField] private float colorGradingSaturation = 1f;
        [SerializeField] private float colorGradingContrast = 1f;

        [Header("Lighting")]
        [SerializeField] private float sunIntensity = 1f;
        [SerializeField] private Color sunColor = Color.white;
        [SerializeField] private float ambientIntensity = 0.3f;
        [SerializeField] private bool enableGlobalIllumination = true;
        [SerializeField] private int shadowCascades = 4;
        [SerializeField] private float shadowDistance = 1000f;

        [Header("Aircraft Effects")]
        [SerializeField] private float contrailDensity = 0.5f;
        [SerializeField] private float contrailLifetime = 10f;
        [SerializeField] private float exhaustHeat = 0.5f;
        [SerializeField] private float metalReflectivity = 0.8f;

        // Shader property IDs for performance
        private static readonly int _SkyColor = Shader.PropertyToID("_SkyColor");
        private static readonly int _AtmosphereDensity = Shader.PropertyToID("_AtmosphereDensity");
        private static readonly int _CloudDensity = Shader.PropertyToID("_CloudDensity");
        private static readonly int _CloudCoverage = Shader.PropertyToID("_CloudCoverage");
        private static readonly int _WaterColor = Shader.PropertyToID("_WaterColor");
        private static readonly int _WaveHeight = Shader.PropertyToID("_WaveHeight");
        private static readonly int _SunDirection = Shader.PropertyToID("_SunDirection");
        private static readonly int _TimeOfDay = Shader.PropertyToID("_TimeOfDay");

        private float currentTimeOfDay = 12f; // 0-24
        private Dictionary<string, Material> cachedMaterials = new Dictionary<string, Material>();
        private List<ShaderEffectController> activeEffects = new List<ShaderEffectController>();

        public enum QualityPreset { Low, Medium, High, Ultra, Cinematic }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeShaderSystem();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeShaderSystem()
        {
            ApplyQualityPreset(qualityPreset);
            UpdateGlobalShaderProperties();
        }

        private void Update()
        {
            UpdateTimeOfDay();
            UpdateDynamicShaderProperties();
            UpdateActiveEffects();
        }

        public void ApplyQualityPreset(QualityPreset preset)
        {
            qualityPreset = preset;

            switch (preset)
            {
                case QualityPreset.Low:
                    cloudRaymarchSteps = 16;
                    shadowCascades = 1;
                    shadowDistance = 200f;
                    enableVolumetrics = false;
                    enableGlobalIllumination = false;
                    grassDensity = 0.25f;
                    terrainDetailDistance = 100f;
                    ambientOcclusionIntensity = 0f;
                    motionBlurIntensity = 0f;
                    break;

                case QualityPreset.Medium:
                    cloudRaymarchSteps = 32;
                    shadowCascades = 2;
                    shadowDistance = 400f;
                    enableVolumetrics = false;
                    enableGlobalIllumination = false;
                    grassDensity = 0.5f;
                    terrainDetailDistance = 250f;
                    ambientOcclusionIntensity = 0.3f;
                    motionBlurIntensity = 0.3f;
                    break;

                case QualityPreset.High:
                    cloudRaymarchSteps = 64;
                    shadowCascades = 4;
                    shadowDistance = 800f;
                    enableVolumetrics = true;
                    enableGlobalIllumination = true;
                    grassDensity = 0.75f;
                    terrainDetailDistance = 400f;
                    ambientOcclusionIntensity = 0.5f;
                    motionBlurIntensity = 0.5f;
                    break;

                case QualityPreset.Ultra:
                    cloudRaymarchSteps = 128;
                    shadowCascades = 4;
                    shadowDistance = 1500f;
                    enableVolumetrics = true;
                    enableGlobalIllumination = true;
                    grassDensity = 1f;
                    terrainDetailDistance = 600f;
                    ambientOcclusionIntensity = 0.7f;
                    motionBlurIntensity = 0.5f;
                    break;

                case QualityPreset.Cinematic:
                    cloudRaymarchSteps = 256;
                    shadowCascades = 4;
                    shadowDistance = 2500f;
                    enableVolumetrics = true;
                    enableGlobalIllumination = true;
                    grassDensity = 1f;
                    terrainDetailDistance = 1000f;
                    ambientOcclusionIntensity = 1f;
                    motionBlurIntensity = 0.7f;
                    filmGrain = 0.15f;
                    break;
            }

            UpdateGlobalShaderProperties();
        }

        private void UpdateTimeOfDay()
        {
            // Time passes - 24 hour cycle in configurable real-time minutes
            // For now, we'll use external time of day
        }

        public void SetTimeOfDay(float time)
        {
            currentTimeOfDay = Mathf.Repeat(time, 24f);
            Shader.SetGlobalFloat(_TimeOfDay, currentTimeOfDay);
            UpdateSkyColor();
        }

        private void UpdateSkyColor()
        {
            Color currentSky;
            
            if (currentTimeOfDay >= 6f && currentTimeOfDay < 8f) // Dawn
            {
                float t = (currentTimeOfDay - 6f) / 2f;
                currentSky = Color.Lerp(skyColorNight, skyColorSunset, t);
            }
            else if (currentTimeOfDay >= 8f && currentTimeOfDay < 10f) // Morning
            {
                float t = (currentTimeOfDay - 8f) / 2f;
                currentSky = Color.Lerp(skyColorSunset, skyColorDay, t);
            }
            else if (currentTimeOfDay >= 10f && currentTimeOfDay < 17f) // Day
            {
                currentSky = skyColorDay;
            }
            else if (currentTimeOfDay >= 17f && currentTimeOfDay < 19f) // Evening
            {
                float t = (currentTimeOfDay - 17f) / 2f;
                currentSky = Color.Lerp(skyColorDay, skyColorSunset, t);
            }
            else if (currentTimeOfDay >= 19f && currentTimeOfDay < 21f) // Dusk
            {
                float t = (currentTimeOfDay - 19f) / 2f;
                currentSky = Color.Lerp(skyColorSunset, skyColorNight, t);
            }
            else // Night
            {
                currentSky = skyColorNight;
            }

            Shader.SetGlobalColor(_SkyColor, currentSky);
        }

        private void UpdateGlobalShaderProperties()
        {
            // Atmosphere
            Shader.SetGlobalFloat(_AtmosphereDensity, atmosphereDensity);
            Shader.SetGlobalFloat("_RayleighScattering", rayleighScattering);
            Shader.SetGlobalFloat("_MieScattering", mieScattering);

            // Clouds
            Shader.SetGlobalFloat(_CloudDensity, cloudDensity);
            Shader.SetGlobalFloat(_CloudCoverage, cloudCoverage);
            Shader.SetGlobalFloat("_CloudSpeed", cloudSpeed);
            Shader.SetGlobalFloat("_CloudHeight", cloudHeight);
            Shader.SetGlobalFloat("_CloudThickness", cloudThickness);
            Shader.SetGlobalInt("_CloudRaymarchSteps", cloudRaymarchSteps);

            // Water
            Shader.SetGlobalColor(_WaterColor, waterColor);
            Shader.SetGlobalColor("_WaterDeepColor", waterDeepColor);
            Shader.SetGlobalFloat("_WaterTransparency", waterTransparency);
            Shader.SetGlobalFloat(_WaveHeight, waveHeight);
            Shader.SetGlobalFloat("_WaveSpeed", waveSpeed);
            Shader.SetGlobalFloat("_FoamIntensity", foamIntensity);
            Shader.SetGlobalFloat("_RefractionStrength", refractionStrength);

            // Terrain
            Shader.SetGlobalFloat("_TerrainDetailDistance", terrainDetailDistance);
            Shader.SetGlobalFloat("_GrassDensity", grassDensity);
            Shader.SetGlobalFloat("_SnowLineHeight", snowLineHeight);

            // Post-Processing
            Shader.SetGlobalFloat("_BloomIntensity", bloomIntensity);
            Shader.SetGlobalFloat("_BloomThreshold", bloomThreshold);
            Shader.SetGlobalFloat("_AOIntensity", ambientOcclusionIntensity);
            Shader.SetGlobalFloat("_AORadius", ambientOcclusionRadius);
            Shader.SetGlobalFloat("_MotionBlurIntensity", motionBlurIntensity);
            Shader.SetGlobalFloat("_ChromaticAberration", chromaticAberration);
            Shader.SetGlobalFloat("_VignetteIntensity", vignetteIntensity);
            Shader.SetGlobalFloat("_FilmGrain", filmGrain);

            // Lighting
            Shader.SetGlobalFloat("_SunIntensity", sunIntensity);
            Shader.SetGlobalColor("_SunColor", sunColor);
            Shader.SetGlobalFloat("_AmbientIntensity", ambientIntensity);

            // Aircraft
            Shader.SetGlobalFloat("_ContrailDensity", contrailDensity);
            Shader.SetGlobalFloat("_ContrailLifetime", contrailLifetime);
            Shader.SetGlobalFloat("_ExhaustHeat", exhaustHeat);
            Shader.SetGlobalFloat("_MetalReflectivity", metalReflectivity);

            // Quality
            QualitySettings.shadowCascades = shadowCascades;
            QualitySettings.shadowDistance = shadowDistance;
        }

        private void UpdateDynamicShaderProperties()
        {
            // Update sun direction based on time of day
            float sunAngle = (currentTimeOfDay / 24f) * 360f - 90f;
            Vector3 sunDirection = Quaternion.Euler(sunAngle, 0, 0) * Vector3.forward;
            Shader.SetGlobalVector(_SunDirection, sunDirection);

            // Update cloud movement
            float cloudOffset = Time.time * cloudSpeed;
            Shader.SetGlobalFloat("_CloudOffset", cloudOffset);

            // Update water waves
            Shader.SetGlobalFloat("_WaveTime", Time.time * waveSpeed);
        }

        private void UpdateActiveEffects()
        {
            for (int i = activeEffects.Count - 1; i >= 0; i--)
            {
                if (activeEffects[i] == null || !activeEffects[i].IsActive)
                {
                    activeEffects.RemoveAt(i);
                }
                else
                {
                    activeEffects[i].UpdateEffect();
                }
            }
        }

        public Material GetCachedMaterial(string materialName)
        {
            if (!cachedMaterials.TryGetValue(materialName, out Material mat))
            {
                mat = Resources.Load<Material>($"Materials/{materialName}");
                if (mat != null)
                {
                    cachedMaterials[materialName] = mat;
                }
            }
            return mat;
        }

        public void RegisterEffect(ShaderEffectController effect)
        {
            if (!activeEffects.Contains(effect))
            {
                activeEffects.Add(effect);
            }
        }

        public void UnregisterEffect(ShaderEffectController effect)
        {
            activeEffects.Remove(effect);
        }

        // Weather integration
        public void SetWeatherEffects(float rain, float fog, float storm)
        {
            Shader.SetGlobalFloat("_RainIntensity", rain);
            Shader.SetGlobalFloat("_FogDensity", fog);
            Shader.SetGlobalFloat("_StormIntensity", storm);

            // Adjust cloud coverage based on weather
            float weatherCloudBoost = Mathf.Max(rain, storm) * 0.3f;
            Shader.SetGlobalFloat(_CloudCoverage, cloudCoverage + weatherCloudBoost);
        }

        // Damage effects
        public void TriggerDamageEffect(float intensity)
        {
            StartCoroutine(DamageEffectCoroutine(intensity));
        }

        private System.Collections.IEnumerator DamageEffectCoroutine(float intensity)
        {
            float duration = 0.5f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                float currentIntensity = Mathf.Lerp(intensity, 0f, t);

                Shader.SetGlobalFloat("_DamageVignette", currentIntensity);
                Shader.SetGlobalFloat("_DamageChromatic", currentIntensity * 0.5f);

                yield return null;
            }

            Shader.SetGlobalFloat("_DamageVignette", 0f);
            Shader.SetGlobalFloat("_DamageChromatic", 0f);
        }

        // Speed effects (for high-speed flight)
        public void SetSpeedEffects(float speedNormalized)
        {
            float speedBlur = Mathf.Lerp(0f, motionBlurIntensity, speedNormalized);
            float speedFOV = Mathf.Lerp(0f, 10f, speedNormalized);
            float speedTunnel = Mathf.Lerp(0f, 0.3f, Mathf.Pow(speedNormalized, 2));

            Shader.SetGlobalFloat("_SpeedMotionBlur", speedBlur);
            Shader.SetGlobalFloat("_SpeedFOVIncrease", speedFOV);
            Shader.SetGlobalFloat("_SpeedTunnelVignette", speedTunnel);
        }

        // G-Force effects
        public void SetGForceEffects(float gForce)
        {
            float blackout = Mathf.Clamp01((gForce - 5f) / 4f); // Start blackout at 5G, full at 9G
            float redout = Mathf.Clamp01((-gForce - 2f) / 3f); // Negative G

            Shader.SetGlobalFloat("_GForceBlackout", blackout);
            Shader.SetGlobalFloat("_GForceRedout", redout);
            Shader.SetGlobalFloat("_GForceVignette", Mathf.Max(blackout, redout) * 0.8f);
        }

        public void SetContrailEnabled(bool enabled, float altitude)
        {
            // Contrails form at high altitude in cold air
            float contrailFactor = enabled && altitude > 5000f ? 1f : 0f;
            Shader.SetGlobalFloat("_ContrailEnabled", contrailFactor);
        }
    }

    /// <summary>
    /// Base class for shader-based visual effects
    /// </summary>
    public abstract class ShaderEffectController : MonoBehaviour
    {
        public abstract bool IsActive { get; }
        public abstract void UpdateEffect();
    }

    /// <summary>
    /// Contrail effect for aircraft
    /// </summary>
    public class ContrailEffect : ShaderEffectController
    {
        [SerializeField] private Transform[] contrailPoints;
        [SerializeField] private float minAltitude = 5000f;
        [SerializeField] private float maxIntensity = 1f;

        private List<TrailRenderer> trails = new List<TrailRenderer>();
        private bool isActive;

        public override bool IsActive => isActive;

        private void Start()
        {
            InitializeTrails();
            ShaderManager.Instance?.RegisterEffect(this);
        }

        private void OnDestroy()
        {
            ShaderManager.Instance?.UnregisterEffect(this);
        }

        private void InitializeTrails()
        {
            foreach (var point in contrailPoints)
            {
                if (point != null)
                {
                    var trail = point.GetComponent<TrailRenderer>();
                    if (trail == null)
                    {
                        trail = point.gameObject.AddComponent<TrailRenderer>();
                        trail.time = 10f;
                        trail.startWidth = 0.5f;
                        trail.endWidth = 2f;
                        trail.material = new Material(Shader.Find("Particles/Standard Unlit"));
                        trail.startColor = new Color(1f, 1f, 1f, 0.8f);
                        trail.endColor = new Color(1f, 1f, 1f, 0f);
                    }
                    trails.Add(trail);
                }
            }
        }

        public override void UpdateEffect()
        {
            float altitude = transform.position.y;
            isActive = altitude > minAltitude;

            float intensity = isActive ? Mathf.Clamp01((altitude - minAltitude) / 2000f) * maxIntensity : 0f;

            foreach (var trail in trails)
            {
                if (trail != null)
                {
                    trail.emitting = isActive;
                    var startColor = trail.startColor;
                    startColor.a = intensity * 0.8f;
                    trail.startColor = startColor;
                }
            }
        }
    }

    /// <summary>
    /// Exhaust heat shimmer effect
    /// </summary>
    public class ExhaustHeatEffect : ShaderEffectController
    {
        [SerializeField] private Transform exhaustPoint;
        [SerializeField] private float throttleMultiplier = 1f;

        private ParticleSystem heatParticles;
        private float currentThrottle;

        public override bool IsActive => currentThrottle > 0.1f;

        public void SetThrottle(float throttle)
        {
            currentThrottle = throttle;
        }

        public override void UpdateEffect()
        {
            if (heatParticles != null)
            {
                var emission = heatParticles.emission;
                emission.rateOverTime = currentThrottle * throttleMultiplier * 50f;
            }
        }
    }

    /// <summary>
    /// Muzzle flash effect for weapons
    /// </summary>
    public class MuzzleFlashEffect : ShaderEffectController
    {
        [SerializeField] private Light flashLight;
        [SerializeField] private float flashDuration = 0.05f;
        [SerializeField] private float flashIntensity = 5f;
        [SerializeField] private Color flashColor = Color.yellow;

        private float flashTimer;
        private bool isFlashing;

        public override bool IsActive => isFlashing;

        public void TriggerFlash()
        {
            isFlashing = true;
            flashTimer = flashDuration;

            if (flashLight != null)
            {
                flashLight.intensity = flashIntensity;
                flashLight.color = flashColor;
                flashLight.enabled = true;
            }
        }

        public override void UpdateEffect()
        {
            if (isFlashing)
            {
                flashTimer -= Time.deltaTime;

                if (flashTimer <= 0f)
                {
                    isFlashing = false;
                    if (flashLight != null)
                    {
                        flashLight.enabled = false;
                    }
                }
                else if (flashLight != null)
                {
                    flashLight.intensity = flashIntensity * (flashTimer / flashDuration);
                }
            }
        }
    }
}
