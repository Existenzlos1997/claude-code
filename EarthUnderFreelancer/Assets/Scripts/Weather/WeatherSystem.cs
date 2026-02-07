using UnityEngine;

namespace EarthUnderFreelancer.Weather
{
    // Iteration 13: Dynamic weather system
    public class WeatherSystem : MonoBehaviour
    {
        public enum WeatherType { Clear, Rain, Storm, Fog, Snow }
        
        [SerializeField] private WeatherType currentWeather = WeatherType.Clear;
        [SerializeField] private float weatherIntensity = 0.5f;
        
        private ParticleSystem rainParticles;
        private ParticleSystem snowParticles;
        
        private void Start()
        {
            SetWeather(currentWeather);
        }
        
        public void SetWeather(WeatherType weather)
        {
            currentWeather = weather;
            Debug.Log($"[Weather] Changed to: {weather}");
            
            switch (weather)
            {
                case WeatherType.Clear:
                    RenderSettings.fogDensity = 0.0001f;
                    break;
                case WeatherType.Fog:
                    RenderSettings.fogDensity = 0.01f;
                    break;
                case WeatherType.Storm:
                    RenderSettings.fogDensity = 0.005f;
                    break;
            }
        }
        
        public WeatherType GetCurrentWeather() => currentWeather;
    }
}
