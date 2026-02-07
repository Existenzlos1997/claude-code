using UnityEngine;

namespace EarthUnderFreelancer.Weather
{
    public class WeatherPhysicsSystem : MonoBehaviour
    {
        [Header("Wind")]
        public Vector3 windDirection = Vector3.right;
        public float windStrength = 10f;
        public float turbulenceIntensity = 5f;
        
        [Header("Precipitation")]
        public float rainIntensity = 0f; // 0-1
        public float iceFormationRate = 0f; // 0-1
        
        [Header("Visibility")]
        public float fogDensity = 0f; // 0-1
        public float visibilityRange = 10000f; // meters
        
        private void FixedUpdate()
        {
            ApplyWeatherPhysics();
        }
        
        private void ApplyWeatherPhysics()
        {
            // Apply to all aircraft
            FlightPhysicsEngine[] aircraft = FindObjectsOfType<FlightPhysicsEngine>();
            foreach (var plane in aircraft)
            {
                // Wind drift
                if (windStrength > 0)
                {
                    Vector3 windForce = windDirection.normalized * windStrength * 100f;
                    Rigidbody rb = plane.GetComponent<Rigidbody>();
                    if (rb != null) rb.AddForce(windForce);
                }
                
                // Turbulence
                if (turbulenceIntensity > 0)
                {
                    Vector3 turbulence = new Vector3(
                        Random.Range(-turbulenceIntensity, turbulenceIntensity),
                        Random.Range(-turbulenceIntensity, turbulenceIntensity),
                        Random.Range(-turbulenceIntensity, turbulenceIntensity)
                    ) * 1000f;
                    Rigidbody rb = plane.GetComponent<Rigidbody>();
                    if (rb != null) rb.AddForce(turbulence);
                }
                
                // Ice formation (reduces control)
                if (iceFormationRate > 0)
                {
                    float iceDamage = iceFormationRate * Time.fixedDeltaTime * 0.01f;
                    plane.TakeDamage(iceDamage);
                }
            }
        }
        
        public void SetWeather(float rain, float wind, float fog)
        {
            rainIntensity = Mathf.Clamp01(rain);
            windStrength = wind;
            fogDensity = Mathf.Clamp01(fog);
            
            // Update rendering
            RenderSettings.fogDensity = fogDensity * 0.01f;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
        }
    }
}
