using UnityEngine;
using System;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Weather
{
    /// <summary>
    /// Dynamic weather and environmental system
    /// Affects gameplay, visibility, and aircraft performance
    /// </summary>
    
    [Serializable]
    public enum WeatherType
    {
        Clear,
        PartlyCloudy,
        Overcast,
        LightRain,
        HeavyRain,
        Thunderstorm,
        Fog,
        Snow,
        Blizzard,
        Sandstorm,
        Hail,
        VolcanicAsh
    }
    
    [Serializable]
    public enum TimeOfDay
    {
        Dawn,
        Morning,
        Noon,
        Afternoon,
        Dusk,
        Night,
        Midnight
    }
    
    [Serializable]
    public enum Season
    {
        Spring,
        Summer,
        Autumn,
        Winter
    }
    
    [Serializable]
    public class WeatherCondition
    {
        public WeatherType type;
        public float intensity; // 0-1
        public float visibility; // 0-1, 1 = perfect visibility
        public float windSpeed; // m/s
        public Vector3 windDirection;
        public float temperature; // Celsius
        public float humidity; // 0-1
        public float cloudCoverage; // 0-1
        public float precipitation; // 0-1
        
        public WeatherCondition()
        {
            type = WeatherType.Clear;
            intensity = 0f;
            visibility = 1f;
            windSpeed = 5f;
            windDirection = Vector3.forward;
            temperature = 20f;
            humidity = 0.5f;
            cloudCoverage = 0.2f;
            precipitation = 0f;
        }
        
        public WeatherCondition Clone()
        {
            return new WeatherCondition
            {
                type = type,
                intensity = intensity,
                visibility = visibility,
                windSpeed = windSpeed,
                windDirection = windDirection,
                temperature = temperature,
                humidity = humidity,
                cloudCoverage = cloudCoverage,
                precipitation = precipitation
            };
        }
    }
    
    [Serializable]
    public class RegionalWeather
    {
        public string regionId;
        public string regionName;
        public Vector2 regionCenter;
        public float regionRadius;
        public WeatherCondition currentWeather;
        public WeatherCondition forecastWeather;
        public float transitionProgress;
        public List<WeatherType> commonWeatherTypes;
        public float baseTemperature;
        public float temperatureVariation;
        
        public RegionalWeather(string id, string name, Vector2 center, float radius)
        {
            regionId = id;
            regionName = name;
            regionCenter = center;
            regionRadius = radius;
            currentWeather = new WeatherCondition();
            forecastWeather = new WeatherCondition();
            transitionProgress = 0f;
            commonWeatherTypes = new List<WeatherType> { WeatherType.Clear, WeatherType.PartlyCloudy };
            baseTemperature = 20f;
            temperatureVariation = 15f;
        }
    }
    
    [Serializable]
    public class WeatherEvent
    {
        public string eventId;
        public string eventName;
        public WeatherType weatherType;
        public float intensity;
        public float duration;
        public float elapsedTime;
        public Vector2 position;
        public float radius;
        public Vector2 movementDirection;
        public float movementSpeed;
        public bool isActive;
        
        public WeatherEvent(string name, WeatherType type, Vector2 pos, float rad)
        {
            eventId = Guid.NewGuid().ToString();
            eventName = name;
            weatherType = type;
            intensity = 1f;
            duration = 1800f; // 30 minutes
            elapsedTime = 0f;
            position = pos;
            radius = rad;
            movementDirection = Vector2.right;
            movementSpeed = 10f;
            isActive = true;
        }
    }
    
    [Serializable]
    public class WeatherEffects
    {
        public float enginePowerModifier;
        public float liftModifier;
        public float dragModifier;
        public float controlSensitivityModifier;
        public float radarRangeModifier;
        public float weaponAccuracyModifier;
        public float fuelConsumptionModifier;
        public bool icingPossible;
        public bool lightningPossible;
        
        public WeatherEffects()
        {
            enginePowerModifier = 1f;
            liftModifier = 1f;
            dragModifier = 1f;
            controlSensitivityModifier = 1f;
            radarRangeModifier = 1f;
            weaponAccuracyModifier = 1f;
            fuelConsumptionModifier = 1f;
            icingPossible = false;
            lightningPossible = false;
        }
    }
    
    public class WeatherSystem : MonoBehaviour
    {
        public static WeatherSystem Instance { get; private set; }
        
        [Header("Time Settings")]
        [SerializeField] private float dayLengthMinutes = 60f; // Real-time minutes per in-game day
        [SerializeField] private float currentTime = 12f; // Hours (0-24)
        [SerializeField] private int currentDay = 1;
        [SerializeField] private Season currentSeason = Season.Summer;
        
        [Header("Weather Settings")]
        [SerializeField] private float weatherUpdateInterval = 60f; // Seconds between weather checks
        [SerializeField] private float weatherTransitionSpeed = 0.1f;
        [SerializeField] private float stormChance = 0.1f;
        [SerializeField] private float extremeWeatherChance = 0.05f;
        
        [Header("Lighting")]
        [SerializeField] private Light sunLight;
        [SerializeField] private Light moonLight;
        [SerializeField] private Gradient dayNightGradient;
        [SerializeField] private AnimationCurve sunIntensityCurve;
        
        [Header("Visual Effects")]
        [SerializeField] private ParticleSystem rainParticles;
        [SerializeField] private ParticleSystem snowParticles;
        [SerializeField] private ParticleSystem fogParticles;
        [SerializeField] private ParticleSystem lightningParticles;
        [SerializeField] private AudioClip thunderSound;
        [SerializeField] private AudioClip rainSound;
        [SerializeField] private AudioClip windSound;
        
        private List<RegionalWeather> regions = new List<RegionalWeather>();
        private List<WeatherEvent> activeEvents = new List<WeatherEvent>();
        private WeatherCondition globalWeather = new WeatherCondition();
        private float weatherTimer = 0f;
        private float timeMultiplier = 1f;
        
        public TimeOfDay CurrentTimeOfDay => GetTimeOfDay();
        public Season CurrentSeason => currentSeason;
        public float CurrentTemperature => globalWeather.temperature;
        public float CurrentVisibility => globalWeather.visibility;
        public WeatherType CurrentWeatherType => globalWeather.type;
        
        public event Action<WeatherCondition> OnWeatherChanged;
        public event Action<TimeOfDay> OnTimeOfDayChanged;
        public event Action<Season> OnSeasonChanged;
        public event Action<WeatherEvent> OnWeatherEventStarted;
        public event Action<WeatherEvent> OnWeatherEventEnded;
        public event Action OnLightningStrike;
        
        private TimeOfDay lastTimeOfDay;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeRegions();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void InitializeRegions()
        {
            // Create world regions with different climates
            regions.Add(CreateRegion("europe", "Central Europe", new Vector2(0, 0), 5000f, 15f, WeatherType.PartlyCloudy));
            regions.Add(CreateRegion("arctic", "Arctic Circle", new Vector2(0, 10000), 3000f, -20f, WeatherType.Snow));
            regions.Add(CreateRegion("sahara", "Sahara Desert", new Vector2(3000, -5000), 4000f, 35f, WeatherType.Clear));
            regions.Add(CreateRegion("pacific", "Pacific Ocean", new Vector2(-8000, 0), 6000f, 25f, WeatherType.LightRain));
            regions.Add(CreateRegion("atlantic", "Atlantic Ocean", new Vector2(5000, 2000), 5000f, 18f, WeatherType.Overcast));
            regions.Add(CreateRegion("siberia", "Siberia", new Vector2(-5000, 8000), 4000f, -10f, WeatherType.Snow));
            regions.Add(CreateRegion("amazon", "Amazon Basin", new Vector2(-4000, -6000), 3000f, 28f, WeatherType.HeavyRain));
            regions.Add(CreateRegion("australia", "Australian Outback", new Vector2(8000, -8000), 4000f, 32f, WeatherType.Clear));
        }
        
        private RegionalWeather CreateRegion(string id, string name, Vector2 center, float radius, float baseTemp, WeatherType common)
        {
            var region = new RegionalWeather(id, name, center, radius);
            region.baseTemperature = baseTemp;
            region.commonWeatherTypes.Add(common);
            region.currentWeather.temperature = baseTemp;
            region.currentWeather.type = common;
            return region;
        }
        
        private void Update()
        {
            UpdateTime();
            UpdateWeather();
            UpdateVisualEffects();
            UpdateWeatherEvents();
        }
        
        private void UpdateTime()
        {
            float hoursPerSecond = 24f / (dayLengthMinutes * 60f);
            currentTime += hoursPerSecond * Time.deltaTime * timeMultiplier;
            
            if (currentTime >= 24f)
            {
                currentTime -= 24f;
                currentDay++;
                
                // Season change every 7 days
                if (currentDay % 7 == 0)
                {
                    AdvanceSeason();
                }
            }
            
            UpdateLighting();
            
            TimeOfDay newTimeOfDay = GetTimeOfDay();
            if (newTimeOfDay != lastTimeOfDay)
            {
                lastTimeOfDay = newTimeOfDay;
                OnTimeOfDayChanged?.Invoke(newTimeOfDay);
            }
        }
        
        private void AdvanceSeason()
        {
            currentSeason = (Season)(((int)currentSeason + 1) % 4);
            OnSeasonChanged?.Invoke(currentSeason);
            
            // Adjust regional temperatures
            foreach (var region in regions)
            {
                float seasonMod = GetSeasonTemperatureModifier(currentSeason);
                region.currentWeather.temperature = region.baseTemperature + seasonMod;
            }
        }
        
        private float GetSeasonTemperatureModifier(Season season)
        {
            switch (season)
            {
                case Season.Winter: return -10f;
                case Season.Spring: return 0f;
                case Season.Summer: return 10f;
                case Season.Autumn: return 0f;
                default: return 0f;
            }
        }
        
        private void UpdateLighting()
        {
            if (sunLight != null)
            {
                // Sun position based on time
                float sunAngle = (currentTime / 24f) * 360f - 90f;
                sunLight.transform.rotation = Quaternion.Euler(sunAngle, 170f, 0f);
                
                // Intensity based on time
                float normalizedTime = currentTime / 24f;
                if (sunIntensityCurve != null && sunIntensityCurve.length > 0)
                {
                    sunLight.intensity = sunIntensityCurve.Evaluate(normalizedTime);
                }
                else
                {
                    // Default intensity curve
                    sunLight.intensity = Mathf.Clamp01(Mathf.Sin(normalizedTime * Mathf.PI));
                }
                
                // Color based on time
                if (dayNightGradient != null)
                {
                    sunLight.color = dayNightGradient.Evaluate(normalizedTime);
                }
                
                // Weather affects intensity
                sunLight.intensity *= globalWeather.visibility;
            }
            
            if (moonLight != null)
            {
                float moonAngle = (currentTime / 24f) * 360f + 90f;
                moonLight.transform.rotation = Quaternion.Euler(moonAngle, 170f, 0f);
                moonLight.intensity = (currentTime >= 20f || currentTime <= 6f) ? 0.1f : 0f;
            }
        }
        
        private void UpdateWeather()
        {
            weatherTimer += Time.deltaTime;
            
            if (weatherTimer >= weatherUpdateInterval)
            {
                weatherTimer = 0f;
                GenerateNewWeather();
            }
            
            // Transition weather smoothly
            foreach (var region in regions)
            {
                if (region.transitionProgress < 1f)
                {
                    region.transitionProgress += weatherTransitionSpeed * Time.deltaTime;
                    LerpWeather(region.currentWeather, region.forecastWeather, region.transitionProgress);
                }
            }
        }
        
        private void GenerateNewWeather()
        {
            foreach (var region in regions)
            {
                if (UnityEngine.Random.value < 0.3f) // 30% chance to change weather
                {
                    region.forecastWeather = GenerateWeatherForRegion(region);
                    region.transitionProgress = 0f;
                }
            }
            
            // Random severe weather events
            if (UnityEngine.Random.value < stormChance)
            {
                GenerateWeatherEvent();
            }
            
            // Update global weather (average of all regions)
            UpdateGlobalWeather();
        }
        
        private WeatherCondition GenerateWeatherForRegion(RegionalWeather region)
        {
            var weather = new WeatherCondition();
            
            // Pick a weather type based on region and season
            List<WeatherType> possibleTypes = new List<WeatherType>(region.commonWeatherTypes);
            
            // Add seasonal variations
            switch (currentSeason)
            {
                case Season.Winter:
                    if (region.baseTemperature < 10f)
                    {
                        possibleTypes.Add(WeatherType.Snow);
                        possibleTypes.Add(WeatherType.Blizzard);
                    }
                    break;
                case Season.Summer:
                    if (region.baseTemperature > 25f)
                    {
                        possibleTypes.Add(WeatherType.Clear);
                        possibleTypes.Add(WeatherType.Thunderstorm);
                    }
                    break;
                case Season.Spring:
                case Season.Autumn:
                    possibleTypes.Add(WeatherType.LightRain);
                    possibleTypes.Add(WeatherType.Fog);
                    break;
            }
            
            weather.type = possibleTypes[UnityEngine.Random.Range(0, possibleTypes.Count)];
            weather.intensity = UnityEngine.Random.Range(0.3f, 1f);
            weather.temperature = region.baseTemperature + UnityEngine.Random.Range(-region.temperatureVariation, region.temperatureVariation);
            weather.windSpeed = UnityEngine.Random.Range(0f, 30f);
            weather.windDirection = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f)).normalized;
            weather.humidity = UnityEngine.Random.Range(0.2f, 0.9f);
            
            // Set visibility and other parameters based on weather type
            ApplyWeatherTypeParameters(weather);
            
            return weather;
        }
        
        private void ApplyWeatherTypeParameters(WeatherCondition weather)
        {
            switch (weather.type)
            {
                case WeatherType.Clear:
                    weather.visibility = 1f;
                    weather.cloudCoverage = 0.1f;
                    weather.precipitation = 0f;
                    break;
                case WeatherType.PartlyCloudy:
                    weather.visibility = 0.95f;
                    weather.cloudCoverage = 0.4f;
                    weather.precipitation = 0f;
                    break;
                case WeatherType.Overcast:
                    weather.visibility = 0.8f;
                    weather.cloudCoverage = 0.8f;
                    weather.precipitation = 0f;
                    break;
                case WeatherType.LightRain:
                    weather.visibility = 0.7f;
                    weather.cloudCoverage = 0.7f;
                    weather.precipitation = 0.3f;
                    break;
                case WeatherType.HeavyRain:
                    weather.visibility = 0.4f;
                    weather.cloudCoverage = 0.9f;
                    weather.precipitation = 0.8f;
                    break;
                case WeatherType.Thunderstorm:
                    weather.visibility = 0.3f;
                    weather.cloudCoverage = 1f;
                    weather.precipitation = 0.9f;
                    weather.windSpeed = Mathf.Max(weather.windSpeed, 20f);
                    break;
                case WeatherType.Fog:
                    weather.visibility = 0.2f;
                    weather.cloudCoverage = 0.6f;
                    weather.precipitation = 0.1f;
                    break;
                case WeatherType.Snow:
                    weather.visibility = 0.5f;
                    weather.cloudCoverage = 0.8f;
                    weather.precipitation = 0.5f;
                    weather.temperature = Mathf.Min(weather.temperature, 0f);
                    break;
                case WeatherType.Blizzard:
                    weather.visibility = 0.15f;
                    weather.cloudCoverage = 1f;
                    weather.precipitation = 1f;
                    weather.windSpeed = Mathf.Max(weather.windSpeed, 30f);
                    weather.temperature = Mathf.Min(weather.temperature, -10f);
                    break;
                case WeatherType.Sandstorm:
                    weather.visibility = 0.1f;
                    weather.cloudCoverage = 0.3f;
                    weather.precipitation = 0f;
                    weather.windSpeed = Mathf.Max(weather.windSpeed, 25f);
                    break;
                case WeatherType.Hail:
                    weather.visibility = 0.35f;
                    weather.cloudCoverage = 0.9f;
                    weather.precipitation = 0.7f;
                    break;
                case WeatherType.VolcanicAsh:
                    weather.visibility = 0.05f;
                    weather.cloudCoverage = 1f;
                    weather.precipitation = 0f;
                    break;
            }
        }
        
        private void LerpWeather(WeatherCondition current, WeatherCondition target, float t)
        {
            current.intensity = Mathf.Lerp(current.intensity, target.intensity, t);
            current.visibility = Mathf.Lerp(current.visibility, target.visibility, t);
            current.windSpeed = Mathf.Lerp(current.windSpeed, target.windSpeed, t);
            current.windDirection = Vector3.Lerp(current.windDirection, target.windDirection, t);
            current.temperature = Mathf.Lerp(current.temperature, target.temperature, t);
            current.humidity = Mathf.Lerp(current.humidity, target.humidity, t);
            current.cloudCoverage = Mathf.Lerp(current.cloudCoverage, target.cloudCoverage, t);
            current.precipitation = Mathf.Lerp(current.precipitation, target.precipitation, t);
            
            if (t > 0.5f)
            {
                current.type = target.type;
            }
        }
        
        private void UpdateGlobalWeather()
        {
            if (regions.Count == 0) return;
            
            float totalVisibility = 0f;
            float totalWindSpeed = 0f;
            Vector3 totalWindDir = Vector3.zero;
            float totalTemp = 0f;
            float totalHumidity = 0f;
            float totalClouds = 0f;
            float totalPrecip = 0f;
            
            foreach (var region in regions)
            {
                totalVisibility += region.currentWeather.visibility;
                totalWindSpeed += region.currentWeather.windSpeed;
                totalWindDir += region.currentWeather.windDirection;
                totalTemp += region.currentWeather.temperature;
                totalHumidity += region.currentWeather.humidity;
                totalClouds += region.currentWeather.cloudCoverage;
                totalPrecip += region.currentWeather.precipitation;
            }
            
            int count = regions.Count;
            globalWeather.visibility = totalVisibility / count;
            globalWeather.windSpeed = totalWindSpeed / count;
            globalWeather.windDirection = (totalWindDir / count).normalized;
            globalWeather.temperature = totalTemp / count;
            globalWeather.humidity = totalHumidity / count;
            globalWeather.cloudCoverage = totalClouds / count;
            globalWeather.precipitation = totalPrecip / count;
            
            // Determine most common weather type
            Dictionary<WeatherType, int> typeCounts = new Dictionary<WeatherType, int>();
            foreach (var region in regions)
            {
                if (!typeCounts.ContainsKey(region.currentWeather.type))
                    typeCounts[region.currentWeather.type] = 0;
                typeCounts[region.currentWeather.type]++;
            }
            
            WeatherType mostCommon = WeatherType.Clear;
            int maxCount = 0;
            foreach (var kvp in typeCounts)
            {
                if (kvp.Value > maxCount)
                {
                    maxCount = kvp.Value;
                    mostCommon = kvp.Key;
                }
            }
            globalWeather.type = mostCommon;
            
            OnWeatherChanged?.Invoke(globalWeather);
        }
        
        private void GenerateWeatherEvent()
        {
            WeatherType eventType;
            string eventName;
            
            if (UnityEngine.Random.value < extremeWeatherChance)
            {
                // Extreme weather
                WeatherType[] extremeTypes = { WeatherType.Blizzard, WeatherType.Thunderstorm, WeatherType.Sandstorm, WeatherType.VolcanicAsh };
                eventType = extremeTypes[UnityEngine.Random.Range(0, extremeTypes.Length)];
                eventName = $"Severe {eventType}";
            }
            else
            {
                // Regular storm
                WeatherType[] stormTypes = { WeatherType.HeavyRain, WeatherType.Snow, WeatherType.Fog };
                eventType = stormTypes[UnityEngine.Random.Range(0, stormTypes.Length)];
                eventName = $"{eventType} Front";
            }
            
            Vector2 randomPos = new Vector2(
                UnityEngine.Random.Range(-10000f, 10000f),
                UnityEngine.Random.Range(-10000f, 10000f)
            );
            
            var weatherEvent = new WeatherEvent(eventName, eventType, randomPos, UnityEngine.Random.Range(1000f, 5000f));
            weatherEvent.movementDirection = new Vector2(
                UnityEngine.Random.Range(-1f, 1f),
                UnityEngine.Random.Range(-1f, 1f)
            ).normalized;
            weatherEvent.movementSpeed = UnityEngine.Random.Range(5f, 20f);
            weatherEvent.duration = UnityEngine.Random.Range(900f, 3600f);
            
            activeEvents.Add(weatherEvent);
            OnWeatherEventStarted?.Invoke(weatherEvent);
        }
        
        private void UpdateWeatherEvents()
        {
            for (int i = activeEvents.Count - 1; i >= 0; i--)
            {
                var evt = activeEvents[i];
                evt.elapsedTime += Time.deltaTime;
                
                // Move the event
                evt.position += evt.movementDirection * evt.movementSpeed * Time.deltaTime;
                
                // Lightning for thunderstorms
                if (evt.weatherType == WeatherType.Thunderstorm && 
                    UnityEngine.Random.value < 0.001f * evt.intensity)
                {
                    TriggerLightning(new Vector3(evt.position.x, 0, evt.position.y));
                }
                
                // End event
                if (evt.elapsedTime >= evt.duration)
                {
                    evt.isActive = false;
                    OnWeatherEventEnded?.Invoke(evt);
                    activeEvents.RemoveAt(i);
                }
            }
        }
        
        private void TriggerLightning(Vector3 position)
        {
            if (lightningParticles != null)
            {
                lightningParticles.transform.position = position + Vector3.up * 500f;
                lightningParticles.Emit(1);
            }
            
            if (thunderSound != null)
            {
                AudioSource.PlayClipAtPoint(thunderSound, position);
            }
            
            OnLightningStrike?.Invoke();
        }
        
        private void UpdateVisualEffects()
        {
            // Rain particles
            if (rainParticles != null)
            {
                var emission = rainParticles.emission;
                bool shouldRain = globalWeather.type == WeatherType.LightRain || 
                                 globalWeather.type == WeatherType.HeavyRain ||
                                 globalWeather.type == WeatherType.Thunderstorm;
                emission.enabled = shouldRain;
                if (shouldRain)
                {
                    emission.rateOverTime = globalWeather.precipitation * 1000f;
                }
            }
            
            // Snow particles
            if (snowParticles != null)
            {
                var emission = snowParticles.emission;
                bool shouldSnow = globalWeather.type == WeatherType.Snow || 
                                 globalWeather.type == WeatherType.Blizzard;
                emission.enabled = shouldSnow;
                if (shouldSnow)
                {
                    emission.rateOverTime = globalWeather.precipitation * 500f;
                }
            }
            
            // Fog
            if (fogParticles != null)
            {
                var emission = fogParticles.emission;
                emission.enabled = globalWeather.type == WeatherType.Fog;
            }
            
            // Ambient fog (Unity fog)
            RenderSettings.fog = globalWeather.visibility < 0.7f;
            RenderSettings.fogDensity = (1f - globalWeather.visibility) * 0.05f;
            RenderSettings.fogColor = Color.Lerp(Color.gray, Color.white, globalWeather.visibility);
        }
        
        public TimeOfDay GetTimeOfDay()
        {
            if (currentTime >= 5f && currentTime < 7f) return TimeOfDay.Dawn;
            if (currentTime >= 7f && currentTime < 11f) return TimeOfDay.Morning;
            if (currentTime >= 11f && currentTime < 14f) return TimeOfDay.Noon;
            if (currentTime >= 14f && currentTime < 17f) return TimeOfDay.Afternoon;
            if (currentTime >= 17f && currentTime < 20f) return TimeOfDay.Dusk;
            if (currentTime >= 20f || currentTime < 0f) return TimeOfDay.Night;
            return TimeOfDay.Midnight;
        }
        
        public WeatherCondition GetWeatherAtPosition(Vector3 worldPosition)
        {
            Vector2 pos2D = new Vector2(worldPosition.x, worldPosition.z);
            
            // Check weather events first
            foreach (var evt in activeEvents)
            {
                float dist = Vector2.Distance(pos2D, evt.position);
                if (dist < evt.radius)
                {
                    var eventWeather = new WeatherCondition();
                    eventWeather.type = evt.weatherType;
                    eventWeather.intensity = evt.intensity * (1f - dist / evt.radius);
                    ApplyWeatherTypeParameters(eventWeather);
                    return eventWeather;
                }
            }
            
            // Find nearest region
            RegionalWeather nearestRegion = null;
            float nearestDist = float.MaxValue;
            
            foreach (var region in regions)
            {
                float dist = Vector2.Distance(pos2D, region.regionCenter);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearestRegion = region;
                }
            }
            
            return nearestRegion?.currentWeather ?? globalWeather;
        }
        
        public WeatherEffects GetWeatherEffects(Vector3 position)
        {
            var weather = GetWeatherAtPosition(position);
            var effects = new WeatherEffects();
            
            // Apply weather effects
            switch (weather.type)
            {
                case WeatherType.Clear:
                    // No modifiers
                    break;
                case WeatherType.LightRain:
                    effects.liftModifier = 0.98f;
                    effects.radarRangeModifier = 0.9f;
                    break;
                case WeatherType.HeavyRain:
                    effects.liftModifier = 0.95f;
                    effects.dragModifier = 1.05f;
                    effects.radarRangeModifier = 0.7f;
                    effects.weaponAccuracyModifier = 0.9f;
                    break;
                case WeatherType.Thunderstorm:
                    effects.liftModifier = 0.9f;
                    effects.dragModifier = 1.1f;
                    effects.controlSensitivityModifier = 0.85f;
                    effects.radarRangeModifier = 0.5f;
                    effects.weaponAccuracyModifier = 0.8f;
                    effects.lightningPossible = true;
                    break;
                case WeatherType.Fog:
                    effects.radarRangeModifier = 0.8f;
                    break;
                case WeatherType.Snow:
                    effects.liftModifier = 0.97f;
                    effects.icingPossible = true;
                    break;
                case WeatherType.Blizzard:
                    effects.liftModifier = 0.92f;
                    effects.dragModifier = 1.08f;
                    effects.controlSensitivityModifier = 0.8f;
                    effects.radarRangeModifier = 0.3f;
                    effects.icingPossible = true;
                    effects.fuelConsumptionModifier = 1.15f;
                    break;
                case WeatherType.Sandstorm:
                    effects.enginePowerModifier = 0.9f;
                    effects.radarRangeModifier = 0.2f;
                    effects.weaponAccuracyModifier = 0.7f;
                    break;
                case WeatherType.VolcanicAsh:
                    effects.enginePowerModifier = 0.7f;
                    effects.radarRangeModifier = 0.1f;
                    break;
            }
            
            // Wind effects
            float windFactor = weather.windSpeed / 30f;
            effects.controlSensitivityModifier *= (1f - windFactor * 0.2f);
            effects.dragModifier *= (1f + windFactor * 0.1f);
            
            // Temperature effects (engine efficiency)
            if (weather.temperature > 35f)
            {
                effects.enginePowerModifier *= 0.95f;
            }
            else if (weather.temperature < -20f)
            {
                effects.fuelConsumptionModifier *= 1.1f;
                effects.icingPossible = true;
            }
            
            return effects;
        }
        
        public Vector3 GetWindVector()
        {
            return globalWeather.windDirection * globalWeather.windSpeed;
        }
        
        public float GetCurrentTime()
        {
            return currentTime;
        }
        
        public void SetTimeMultiplier(float multiplier)
        {
            timeMultiplier = Mathf.Clamp(multiplier, 0f, 100f);
        }
        
        public void SetTime(float hours)
        {
            currentTime = Mathf.Clamp(hours, 0f, 24f);
        }
        
        public void SetSeason(Season season)
        {
            currentSeason = season;
            OnSeasonChanged?.Invoke(currentSeason);
        }
        
        public void ForceWeather(WeatherType type, float duration = 300f)
        {
            var weather = new WeatherCondition();
            weather.type = type;
            weather.intensity = 1f;
            ApplyWeatherTypeParameters(weather);
            
            foreach (var region in regions)
            {
                region.forecastWeather = weather.Clone();
                region.transitionProgress = 0f;
            }
        }
        
        public List<WeatherEvent> GetActiveWeatherEvents()
        {
            return new List<WeatherEvent>(activeEvents);
        }
        
        public string GetWeatherDescription()
        {
            string temp = $"{globalWeather.temperature:F1}°C";
            string wind = $"Wind: {globalWeather.windSpeed:F0} m/s";
            string visibility = $"Visibility: {globalWeather.visibility * 100:F0}%";
            return $"{globalWeather.type} - {temp}, {wind}, {visibility}";
        }
    }
}
