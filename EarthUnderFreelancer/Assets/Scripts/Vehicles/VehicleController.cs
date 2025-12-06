using UnityEngine;
using EarthUnderFreelancer.Core;

namespace EarthUnderFreelancer.Vehicles
{
    /// <summary>
    /// Base vehicle controller for all flyable vehicles
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class VehicleController : MonoBehaviour
    {
        [Header("Vehicle Stats")]
        [SerializeField] private VehicleStats stats;

        [Header("Movement Settings")]
        [SerializeField] private float thrustForce = 1000f;
        [SerializeField] private float maxSpeed = 100f;
        [SerializeField] private float boostMultiplier = 1.5f;
        [SerializeField] private float brakeForce = 500f;
        [SerializeField] private float pitchSpeed = 50f;
        [SerializeField] private float yawSpeed = 30f;
        [SerializeField] private float rollSpeed = 60f;

        [Header("Physics")]
        [SerializeField] private float drag = 1f;
        [SerializeField] private float angularDrag = 3f;
        [SerializeField] private float stability = 0.3f;

        [Header("Effects")]
        [SerializeField] private ParticleSystem[] engineParticles;
        [SerializeField] private TrailRenderer[] trailRenderers;
        [SerializeField] private Light[] engineLights;

        private Rigidbody rb;
        private HealthSystem healthSystem;

        private Vector2 movementInput;
        private Vector2 lookInput;
        private bool isBoosting;
        private bool isBraking;

        private float currentSpeed;
        private float currentThrottle;
        private bool isControllable = true;

        public float CurrentSpeed => currentSpeed;
        public float MaxSpeed => maxSpeed;
        public bool IsBoosting => isBoosting;
        public VehicleStats Stats => stats;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            healthSystem = GetComponent<HealthSystem>();

            rb.useGravity = false;
            rb.linearDamping = drag;
            rb.angularDamping = angularDrag;
        }

        private void Start()
        {
            if (stats != null) ApplyStats();
        }

        private void ApplyStats()
        {
            thrustForce = stats.enginePower;
            maxSpeed = stats.maxSpeed;
            pitchSpeed = stats.maneuverability;
            yawSpeed = stats.maneuverability * 0.6f;
            rollSpeed = stats.maneuverability * 1.2f;
        }

        private void FixedUpdate()
        {
            if (!isControllable) return;

            ApplyThrust();
            ApplyRotation();
            ApplyStabilization();
            UpdateEffects();

            currentSpeed = rb.linearVelocity.magnitude;
        }

        private void ApplyThrust()
        {
            float targetThrottle = movementInput.y;
            
            if (isBoosting && currentThrottle > 0)
            {
                targetThrottle *= boostMultiplier;
            }

            currentThrottle = Mathf.Lerp(currentThrottle, targetThrottle, Time.fixedDeltaTime * 3f);

            if (currentThrottle > 0)
            {
                Vector3 thrust = transform.forward * thrustForce * currentThrottle;
                rb.AddForce(thrust, ForceMode.Force);
            }

            if (isBraking)
            {
                rb.AddForce(-rb.linearVelocity.normalized * brakeForce, ForceMode.Force);
            }

            float effectiveMaxSpeed = isBoosting ? maxSpeed * boostMultiplier : maxSpeed;
            if (rb.linearVelocity.magnitude > effectiveMaxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * effectiveMaxSpeed;
            }
        }

        private void ApplyRotation()
        {
            float pitch = -lookInput.y * pitchSpeed * Time.fixedDeltaTime;
            float yaw = lookInput.x * yawSpeed * Time.fixedDeltaTime;
            float roll = -movementInput.x * rollSpeed * Time.fixedDeltaTime;

            rb.AddRelativeTorque(pitch, yaw, roll, ForceMode.VelocityChange);
        }

        private void ApplyStabilization()
        {
            if (Mathf.Abs(movementInput.x) < 0.1f)
            {
                float rollAngle = transform.eulerAngles.z;
                if (rollAngle > 180) rollAngle -= 360;
                
                float stabilizingTorque = -rollAngle * stability;
                rb.AddRelativeTorque(0, 0, stabilizingTorque, ForceMode.Force);
            }
        }

        private void UpdateEffects()
        {
            float throttleNormalized = Mathf.Clamp01(currentThrottle);
            float boostEffect = isBoosting ? 1.5f : 1f;

            if (engineParticles != null)
            {
                foreach (var ps in engineParticles)
                {
                    if (ps != null)
                    {
                        var emission = ps.emission;
                        emission.rateOverTime = throttleNormalized * 50f * boostEffect;
                        
                        var main = ps.main;
                        main.startSpeed = 10f + throttleNormalized * 20f * boostEffect;
                    }
                }
            }

            if (trailRenderers != null)
            {
                foreach (var trail in trailRenderers)
                {
                    if (trail != null)
                    {
                        trail.emitting = throttleNormalized > 0.5f || isBoosting;
                    }
                }
            }

            if (engineLights != null)
            {
                foreach (var light in engineLights)
                {
                    if (light != null)
                    {
                        light.intensity = throttleNormalized * 2f * boostEffect;
                    }
                }
            }
        }

        public void SetMovementInput(Vector2 input) => movementInput = input;
        public void SetLookInput(Vector2 input) => lookInput = input;
        public void SetBoost(bool boost) => isBoosting = boost;
        public void SetBrake(bool brake) => isBraking = brake;

        public void SetControllable(bool controllable)
        {
            isControllable = controllable;
            if (!controllable)
            {
                movementInput = Vector2.zero;
                lookInput = Vector2.zero;
                isBoosting = false;
                isBraking = false;
            }
        }

        public void ApplyDamageForce(Vector3 force) => rb.AddForce(force, ForceMode.Impulse);
        public void SetStats(VehicleStats newStats) { stats = newStats; ApplyStats(); }
        public float GetThrottlePercent() => Mathf.Clamp01(currentThrottle);

        public float GetSpeedPercent()
        {
            float effectiveMax = isBoosting ? maxSpeed * boostMultiplier : maxSpeed;
            return currentSpeed / effectiveMax;
        }
    }

    [System.Serializable]
    public class VehicleStats
    {
        public string vehicleId;
        public string vehicleName;
        public VehicleType vehicleType;

        [Header("Performance")]
        public float maxSpeed = 100f;
        public float enginePower = 1000f;
        public float maneuverability = 50f;
        public float acceleration = 10f;

        [Header("Combat")]
        public float maxHealth = 100f;
        public float maxShield = 50f;
        public float shieldRegenRate = 5f;
        public int weaponSlots = 2;
        public int missileSlots = 1;

        [Header("Economy")]
        public int purchasePrice = 10000;
        public int upgradeBaseCost = 500;
    }
}
