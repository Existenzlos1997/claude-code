using UnityEngine;
using EarthUnderFreelancer.Data;
using System;

namespace EarthUnderFreelancer.Vehicles
{
    /// <summary>
    /// Realistic flight physics controller based on War Thunder flight model
    /// Simulates authentic WWII/Korean War era aircraft behavior
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class RealisticFlightController : MonoBehaviour
    {
        public static event Action<RealisticFlightController> OnAircraftSpawned;
        public static event Action<RealisticFlightController> OnAircraftDestroyed;

        [Header("Aircraft Data")]
        [SerializeField] private string aircraftId = "p51d_mustang";
        private AircraftSpecs specs;

        [Header("Control Surfaces")]
        [SerializeField] private Transform elevator;
        [SerializeField] private Transform aileronLeft;
        [SerializeField] private Transform aileronRight;
        [SerializeField] private Transform rudder;
        [SerializeField] private Transform[] flaps;
        [SerializeField] private Transform[] airbrakes;
        [SerializeField] private Transform[] landingGear;
        [SerializeField] private float controlSurfaceDeflection = 25f;

        [Header("Engine")]
        [SerializeField] private Transform[] propellers;
        [SerializeField] private ParticleSystem[] exhaustParticles;
        [SerializeField] private float propellerRPM = 2400f;

        [Header("Visual Effects")]
        [SerializeField] private TrailRenderer[] wingtipVortices;
        [SerializeField] private ParticleSystem[] engineSmoke;
        [SerializeField] private ParticleSystem[] damageSmoke;
        [SerializeField] private ParticleSystem[] damageFire;

        [Header("Audio")]
        [SerializeField] private AudioSource engineAudioSource;
        [SerializeField] private AudioSource windAudioSource;
        [SerializeField] private AudioClip engineStartSound;
        [SerializeField] private AudioClip engineLoopSound;
        [SerializeField] private AudioClip engineShutdownSound;
        [SerializeField] private AudioClip stallWarningSound;
        [SerializeField] private AudioClip gForceSound;

        // Components
        private Rigidbody rb;
        private HealthSystem healthSystem;

        // Flight state
        private Vector3 controlInput; // pitch, yaw, roll
        private float throttle;
        private bool wepActive;
        private float wepTimeRemaining;
        private bool gearDeployed = true;
        private bool flapsDeployed;
        private bool airbrakesDeployed;
        private float fuelRemaining;
        private bool engineRunning;
        private bool isStalling;
        private float currentGForce;
        private bool hasAirbrake;

        // Physics values
        private float currentIAS; // Indicated Airspeed in m/s
        private float currentTAS; // True Airspeed
        private float currentAltitude;
        private float currentMach;
        private float angleOfAttack;
        private float slipAngle;
        private float liftCoefficient;
        private float dragCoefficient;
        private Vector3 lastVelocity;

        // Constants
        private const float AIR_DENSITY_SEA_LEVEL = 1.225f; // kg/m³
        private const float GRAVITY = 9.81f;
        private const float SPEED_OF_SOUND = 343f; // m/s at sea level

        // Properties
        public float Throttle => throttle;
        public float CurrentSpeedKmh => currentIAS * 3.6f;
        public float CurrentSpeedKnots => currentIAS * 1.944f;
        public float CurrentAltitudeM => currentAltitude;
        public float CurrentAltitudeFt => currentAltitude * 3.281f;
        public float CurrentMach => currentMach;
        public float AngleOfAttack => angleOfAttack;
        public float GForce => currentGForce;
        public bool IsStalling => isStalling;
        public bool IsWEPActive => wepActive;
        public float FuelPercent => fuelRemaining / (specs?.fuelCapacityL ?? 1f);
        public bool GearDeployed => gearDeployed;
        public bool FlapsDeployed => flapsDeployed;
        public AircraftSpecs Specs => specs;
        public string AircraftName => specs?.name ?? "Unknown";

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            healthSystem = GetComponent<HealthSystem>();
            
            LoadAircraftSpecs();
            InitializeAircraft();
        }

        private void Start()
        {
            OnAircraftSpawned?.Invoke(this);
        }

        private void OnDestroy()
        {
            OnAircraftDestroyed?.Invoke(this);
        }

        private void LoadAircraftSpecs()
        {
            specs = RealAircraftDatabase.GetAircraftById(aircraftId);
            if (specs == null)
            {
                Debug.LogError($"[Flight] Aircraft specs not found for: {aircraftId}");
                specs = RealAircraftDatabase.P51D_Mustang; // Default fallback
            }
        }

        private void InitializeAircraft()
        {
            // Set mass
            rb.mass = specs.emptyWeightKg;
            rb.linearDamping = 0f;
            rb.angularDamping = 1f;
            rb.useGravity = true;
            rb.interpolation = RigidbodyInterpolation.Interpolate;

            // Initialize fuel
            fuelRemaining = specs.fuelCapacityL;
            wepTimeRemaining = specs.engine.wepDurationSeconds;

            // Start engine
            StartEngine();

            Debug.Log($"[Flight] Initialized {specs.name} - {specs.engine.horsePower}hp");
        }

        private void FixedUpdate()
        {
            if (!engineRunning && throttle < 0.01f) return;

            UpdatePhysicsState();
            ApplyAerodynamicForces();
            ApplyEngineForces();
            ApplyControlSurfaces();
            CheckFlightLimits();
            UpdateFuel();
            UpdateEffects();
        }

        #region Physics Calculations

        private void UpdatePhysicsState()
        {
            // Calculate speeds
            Vector3 localVelocity = transform.InverseTransformDirection(rb.linearVelocity);
            currentIAS = rb.linearVelocity.magnitude;
            currentAltitude = transform.position.y;
            
            // Air density decreases with altitude
            float airDensity = CalculateAirDensity(currentAltitude);
            currentTAS = currentIAS * Mathf.Sqrt(AIR_DENSITY_SEA_LEVEL / airDensity);
            currentMach = currentIAS / SPEED_OF_SOUND;

            // Angle of attack
            if (currentIAS > 1f)
            {
                angleOfAttack = Mathf.Atan2(-localVelocity.y, localVelocity.z) * Mathf.Rad2Deg;
                slipAngle = Mathf.Atan2(localVelocity.x, localVelocity.z) * Mathf.Rad2Deg;
            }

            // G-Force calculation
            Vector3 acceleration = (rb.linearVelocity - lastVelocity) / Time.fixedDeltaTime;
            currentGForce = (acceleration.y + GRAVITY) / GRAVITY;
            lastVelocity = rb.linearVelocity;

            // Stall detection
            float stallAoA = 15f + (flapsDeployed ? 5f : 0f);
            isStalling = Mathf.Abs(angleOfAttack) > stallAoA || currentIAS < (specs.stallSpeedKmh / 3.6f);
        }

        private float CalculateAirDensity(float altitude)
        {
            // International Standard Atmosphere approximation
            float temperature = 288.15f - 0.0065f * altitude;
            float pressure = 101325f * Mathf.Pow(temperature / 288.15f, 5.256f);
            return pressure / (287.05f * temperature);
        }

        private void ApplyAerodynamicForces()
        {
            if (currentIAS < 0.1f) return;

            float airDensity = CalculateAirDensity(currentAltitude);
            float dynamicPressure = 0.5f * airDensity * currentIAS * currentIAS;
            float wingArea = specs.wingAreaM2;

            // Lift coefficient varies with AoA
            liftCoefficient = CalculateLiftCoefficient(angleOfAttack);
            
            // Flaps increase lift and drag
            if (flapsDeployed)
            {
                liftCoefficient *= 1.4f;
            }

            // Calculate lift force
            float liftMagnitude = liftCoefficient * dynamicPressure * wingArea;
            Vector3 liftDirection = Vector3.Cross(rb.linearVelocity.normalized, transform.right).normalized;
            Vector3 liftForce = liftDirection * liftMagnitude;

            // Drag coefficient
            dragCoefficient = CalculateDragCoefficient(liftCoefficient);
            
            // Additional drag sources
            if (gearDeployed) dragCoefficient += 0.02f;
            if (flapsDeployed) dragCoefficient += 0.04f;
            if (airbrakesDeployed) dragCoefficient += 0.08f;

            // Calculate drag force
            float dragMagnitude = dragCoefficient * dynamicPressure * wingArea;
            Vector3 dragForce = -rb.linearVelocity.normalized * dragMagnitude;

            // Apply forces
            rb.AddForce(liftForce, ForceMode.Force);
            rb.AddForce(dragForce, ForceMode.Force);

            // Stall handling
            if (isStalling)
            {
                // Reduce lift during stall
                rb.AddForce(-liftForce * 0.5f, ForceMode.Force);
                
                // Add random buffeting
                rb.AddTorque(UnityEngine.Random.insideUnitSphere * 500f, ForceMode.Force);
            }
        }

        private float CalculateLiftCoefficient(float aoa)
        {
            // Simplified lift curve
            float aoaRad = aoa * Mathf.Deg2Rad;
            float clSlope = 5.7f; // Lift slope per radian
            float cl = clSlope * aoaRad;
            
            // Stall at high AoA
            float stallAoA = 15f * Mathf.Deg2Rad;
            if (Mathf.Abs(aoaRad) > stallAoA)
            {
                float stallFactor = 1f - (Mathf.Abs(aoaRad) - stallAoA) / (10f * Mathf.Deg2Rad);
                cl *= Mathf.Clamp01(stallFactor);
            }

            return Mathf.Clamp(cl, -1.5f, 1.8f);
        }

        private float CalculateDragCoefficient(float cl)
        {
            // Parasitic + Induced drag
            float cd0 = 0.021f; // Zero-lift drag coefficient (clean aircraft)
            float k = 0.04f; // Induced drag factor
            float oswald = 0.8f; // Oswald efficiency
            float aspectRatio = (specs.wingspanM * specs.wingspanM) / specs.wingAreaM2;
            
            float inducedDrag = (cl * cl) / (Mathf.PI * aspectRatio * oswald);
            
            return cd0 + k * inducedDrag;
        }

        private void ApplyEngineForces()
        {
            if (!engineRunning || fuelRemaining <= 0) return;

            float airDensity = CalculateAirDensity(currentAltitude);
            float densityRatio = airDensity / AIR_DENSITY_SEA_LEVEL;

            // Engine power varies with altitude and throttle
            float basePower = specs.engine.horsePower;
            if (wepActive && wepTimeRemaining > 0)
            {
                basePower = specs.engine.horsePowerWEP;
                wepTimeRemaining -= Time.fixedDeltaTime;
            }

            // Power decreases with altitude (supercharger partially compensates)
            float altitudeFactor = Mathf.Lerp(1f, 0.3f, currentAltitude / 10000f);
            float effectivePower = basePower * throttle * altitudeFactor * densityRatio;

            // Convert HP to Watts (1 HP = 745.7 W)
            float powerWatts = effectivePower * 745.7f;

            // Thrust = Power / Velocity (with efficiency factor)
            float propEfficiency = 0.8f;
            float thrust;
            if (currentIAS > 10f)
            {
                thrust = (powerWatts * propEfficiency) / currentIAS;
            }
            else
            {
                // Static thrust approximation
                thrust = powerWatts * propEfficiency * 0.1f;
            }

            // Apply thrust
            rb.AddForce(transform.forward * thrust, ForceMode.Force);
        }

        private void ApplyControlSurfaces()
        {
            // Control effectiveness increases with airspeed
            float controlEffectiveness = Mathf.Clamp01(currentIAS / 50f);
            
            // But decreases at very high speeds (control stiffening)
            if (currentIAS > specs.maxSpeedKmh / 3.6f * 0.9f)
            {
                float highSpeedFactor = (specs.maxSpeedKmh / 3.6f - currentIAS) / (specs.maxSpeedKmh / 3.6f * 0.1f);
                controlEffectiveness *= Mathf.Clamp01(highSpeedFactor);
            }

            // Calculate control torques
            float pitch = controlInput.x * specs.turnTime360 / 20f * 100f;
            float yaw = controlInput.y * specs.turnTime360 / 30f * 50f;
            float roll = controlInput.z * specs.rollRate * 0.5f;

            // Apply torques
            Vector3 torque = new Vector3(pitch, yaw, roll) * controlEffectiveness;
            rb.AddRelativeTorque(torque, ForceMode.Force);

            // Update visual control surface positions
            UpdateControlSurfaceVisuals();
        }

        private void UpdateControlSurfaceVisuals()
        {
            if (elevator != null)
            {
                elevator.localRotation = Quaternion.Euler(-controlInput.x * controlSurfaceDeflection, 0, 0);
            }
            if (aileronLeft != null)
            {
                aileronLeft.localRotation = Quaternion.Euler(-controlInput.z * controlSurfaceDeflection, 0, 0);
            }
            if (aileronRight != null)
            {
                aileronRight.localRotation = Quaternion.Euler(controlInput.z * controlSurfaceDeflection, 0, 0);
            }
            if (rudder != null)
            {
                rudder.localRotation = Quaternion.Euler(0, controlInput.y * controlSurfaceDeflection, 0);
            }
        }

        private void CheckFlightLimits()
        {
            // Wing overload (G-force limit)
            if (Mathf.Abs(currentGForce) > specs.maxGForce)
            {
                float overload = (Mathf.Abs(currentGForce) - specs.maxGForce) / specs.maxGForce;
                if (overload > 0.2f)
                {
                    // Wing stress damage
                    healthSystem?.TakeDamage(overload * 10f * Time.fixedDeltaTime, DamageSource.StructuralFailure);
                    Debug.Log($"[Flight] Wing stress! G-Force: {currentGForce:F1}");
                }
            }

            // Speed limit (wing rip)
            if (CurrentSpeedKmh > specs.wingRipSpeedKmh)
            {
                float overspeed = (CurrentSpeedKmh - specs.wingRipSpeedKmh) / 100f;
                healthSystem?.TakeDamage(overspeed * 50f * Time.fixedDeltaTime, DamageSource.StructuralFailure);
                Debug.Log($"[Flight] Overspeed! Wings failing!");
            }

            // Ground collision check
            if (currentAltitude < 0.1f && !gearDeployed && currentIAS > 30f)
            {
                // Belly landing damage
                healthSystem?.TakeDamage(100f, DamageSource.Collision);
            }
        }

        private void UpdateFuel()
        {
            if (!engineRunning) return;

            // Fuel consumption rate based on throttle
            float consumptionRate = specs.fuelCapacityL / (specs.fuelCapacityL * 60f); // Rough estimate
            consumptionRate *= throttle;
            if (wepActive) consumptionRate *= 1.5f;

            fuelRemaining -= consumptionRate * Time.fixedDeltaTime;
            fuelRemaining = Mathf.Max(0, fuelRemaining);

            if (fuelRemaining <= 0)
            {
                StopEngine();
            }
        }

        #endregion

        #region Controls

        public void SetControlInput(Vector3 input)
        {
            controlInput = new Vector3(
                Mathf.Clamp(input.x, -1f, 1f), // Pitch
                Mathf.Clamp(input.y, -1f, 1f), // Yaw
                Mathf.Clamp(input.z, -1f, 1f)  // Roll
            );
        }

        public void SetThrottle(float value)
        {
            throttle = Mathf.Clamp01(value);
        }

        public void SetWEP(bool active)
        {
            if (wepTimeRemaining > 0)
            {
                wepActive = active;
            }
            else
            {
                wepActive = false;
            }
        }

        public void ToggleGear()
        {
            // Don't allow gear toggle at high speed
            if (currentIAS > 400f / 3.6f)
            {
                Debug.Log("[Flight] Too fast to toggle gear!");
                return;
            }

            gearDeployed = !gearDeployed;
            AnimateGear();
        }

        public void ToggleFlaps()
        {
            // Don't allow flaps at high speed
            if (currentIAS > 350f / 3.6f && !flapsDeployed)
            {
                Debug.Log("[Flight] Too fast for flaps!");
                return;
            }

            flapsDeployed = !flapsDeployed;
            AnimateFlaps();
        }

        public void ToggleAirbrakes()
        {
            if (!hasAirbrake) return;
            airbrakesDeployed = !airbrakesDeployed;
            AnimateAirbrakes();
        }

        public void StartEngine()
        {
            if (fuelRemaining <= 0) return;

            engineRunning = true;
            if (engineAudioSource != null && engineStartSound != null)
            {
                engineAudioSource.PlayOneShot(engineStartSound);
            }
        }

        public void StopEngine()
        {
            engineRunning = false;
            throttle = 0;
            if (engineAudioSource != null && engineShutdownSound != null)
            {
                engineAudioSource.PlayOneShot(engineShutdownSound);
            }
        }

        #endregion

        #region Visual Effects

        private void UpdateEffects()
        {
            // Propeller rotation
            if (propellers != null && engineRunning)
            {
                float rpm = propellerRPM * throttle;
                foreach (var prop in propellers)
                {
                    if (prop != null)
                    {
                        prop.Rotate(Vector3.forward, rpm * 6f * Time.fixedDeltaTime);
                    }
                }
            }

            // Exhaust particles
            if (exhaustParticles != null)
            {
                foreach (var ps in exhaustParticles)
                {
                    if (ps != null)
                    {
                        var emission = ps.emission;
                        emission.rateOverTime = throttle * 50f;
                    }
                }
            }

            // Wingtip vortices at high AoA
            if (wingtipVortices != null)
            {
                bool showVortices = Mathf.Abs(angleOfAttack) > 5f && currentIAS > 50f;
                foreach (var trail in wingtipVortices)
                {
                    if (trail != null)
                    {
                        trail.emitting = showVortices;
                    }
                }
            }

            // Engine audio
            if (engineAudioSource != null && engineRunning)
            {
                engineAudioSource.pitch = 0.8f + throttle * 0.6f;
                engineAudioSource.volume = 0.5f + throttle * 0.5f;
            }

            // Wind audio
            if (windAudioSource != null)
            {
                windAudioSource.volume = Mathf.Clamp01(currentIAS / 200f);
                windAudioSource.pitch = 0.5f + (currentIAS / 300f);
            }
        }

        private void AnimateGear()
        {
            // Would use DOTween or Animation for smooth movement
            foreach (var gear in landingGear)
            {
                if (gear != null)
                {
                    // Simple toggle visibility for now
                    gear.gameObject.SetActive(gearDeployed);
                }
            }
        }

        private void AnimateFlaps()
        {
            foreach (var flap in flaps)
            {
                if (flap != null)
                {
                    float angle = flapsDeployed ? 30f : 0f;
                    flap.localRotation = Quaternion.Euler(angle, 0, 0);
                }
            }
        }

        private void AnimateAirbrakes()
        {
            foreach (var brake in airbrakes)
            {
                if (brake != null)
                {
                    float angle = airbrakesDeployed ? 45f : 0f;
                    brake.localRotation = Quaternion.Euler(angle, 0, 0);
                }
            }
        }

        #endregion

        #region Damage Handling

        public void ApplyDamage(float damage, Vector3 hitPoint, DamageSource source)
        {
            if (healthSystem != null)
            {
                healthSystem.TakeDamage(damage, source);

                // Visual damage effects
                if (healthSystem.HealthPercent < 0.5f)
                {
                    EnableDamageEffects(true);
                }
                if (healthSystem.HealthPercent < 0.25f)
                {
                    EnableFireEffects(true);
                }
            }
        }

        private void EnableDamageEffects(bool enable)
        {
            if (damageSmoke != null)
            {
                foreach (var ps in damageSmoke)
                {
                    if (ps != null)
                    {
                        if (enable) ps.Play();
                        else ps.Stop();
                    }
                }
            }
        }

        private void EnableFireEffects(bool enable)
        {
            if (damageFire != null)
            {
                foreach (var ps in damageFire)
                {
                    if (ps != null)
                    {
                        if (enable) ps.Play();
                        else ps.Stop();
                    }
                }
            }
        }

        #endregion

        #region Debug

        private void OnGUI()
        {
            if (!Debug.isDebugBuild) return;

            GUILayout.BeginArea(new Rect(10, 10, 250, 300));
            GUILayout.Label($"Aircraft: {specs?.name ?? "Unknown"}");
            GUILayout.Label($"IAS: {CurrentSpeedKmh:F0} km/h ({CurrentSpeedKnots:F0} kts)");
            GUILayout.Label($"Altitude: {CurrentAltitudeM:F0} m ({CurrentAltitudeFt:F0} ft)");
            GUILayout.Label($"Mach: {CurrentMach:F2}");
            GUILayout.Label($"AoA: {angleOfAttack:F1}°");
            GUILayout.Label($"G-Force: {currentGForce:F1} G");
            GUILayout.Label($"Throttle: {throttle * 100:F0}%{(wepActive ? " [WEP]" : "")}");
            GUILayout.Label($"Fuel: {FuelPercent * 100:F0}%");
            GUILayout.Label($"Gear: {(gearDeployed ? "DOWN" : "UP")}");
            GUILayout.Label($"Flaps: {(flapsDeployed ? "DOWN" : "UP")}");
            GUILayout.Label($"Stalling: {(isStalling ? "YES!" : "No")}");
            GUILayout.EndArea();
        }

        #endregion
    }

    public enum DamageSource
    {
        Projectile,
        Explosion,
        Collision,
        StructuralFailure,
        Fire,
        Crash
    }
}
