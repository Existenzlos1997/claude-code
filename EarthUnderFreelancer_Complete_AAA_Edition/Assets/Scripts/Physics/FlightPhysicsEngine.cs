using UnityEngine;

namespace EarthUnderFreelancer.Physics
{
    /// <summary>
    /// Realistic flight physics engine with aerodynamic forces, 6-DOF movement, and damage effects.
    /// Implements full flight dynamics including lift, drag, thrust, and control surfaces.
    /// </summary>
    public class FlightPhysicsEngine : MonoBehaviour
    {
        [Header("Aircraft Properties")]
        public float mass = 15000f; // kg (e.g., F-16)
        public float wingArea = 27.87f; // m² (F-16)
        public float maxThrust = 76000f; // N (with afterburner)
        public float minThrust = 20000f; // N (idle)
        
        [Header("Aerodynamic Coefficients")]
        public float liftCoefficient = 1.2f;
        public float dragCoefficient = 0.025f;
        public float stallAngle = 18f; // degrees
        
        [Header("Control Surfaces")]
        [Range(-1f, 1f)] public float aileronInput = 0f; // Roll
        [Range(-1f, 1f)] public float elevatorInput = 0f; // Pitch
        [Range(-1f, 1f)] public float rudderInput = 0f; // Yaw
        [Range(0f, 1f)] public float flapsExtension = 0f;
        
        [Header("Engine")]
        [Range(0f, 1f)] public float throttle = 0f;
        public bool afterburnerActive = false;
        
        [Header("Damage")]
        [Range(0f, 1f)] public float structuralIntegrity = 1f; // 1 = perfect, 0 = destroyed
        
        // Internal state
        private Rigidbody rb;
        private Vector3 velocity;
        private Vector3 angularVelocity;
        private float atmosphericDensity = 1.225f; // kg/m³ at sea level
        
        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
                rb.mass = mass;
                rb.useGravity = true;
            }
        }
        
        private void FixedUpdate()
        {
            CalculateAtmosphericDensity();
            ApplyAerodynamicForces();
            ApplyEngineThrust();
            ApplyControlSurfaces();
            ApplyDamageEffects();
        }
        
        /// <summary>
        /// Calculate atmospheric density based on altitude (simplified model)
        /// </summary>
        private void CalculateAtmosphericDensity()
        {
            float altitude = transform.position.y;
            // Exponential atmosphere model
            atmosphericDensity = 1.225f * Mathf.Exp(-altitude / 8500f);
        }
        
        /// <summary>
        /// Apply lift and drag forces
        /// </summary>
        private void ApplyAerodynamicForces()
        {
            velocity = rb.velocity;
            float speed = velocity.magnitude;
            
            if (speed < 0.1f) return; // Avoid division by zero
            
            // Calculate angle of attack
            Vector3 velocityDirection = velocity.normalized;
            Vector3 forward = transform.forward;
            float angleOfAttack = Vector3.Angle(forward, velocityDirection);
            
            // Stall condition
            bool isStalled = angleOfAttack > stallAngle;
            float effectiveLiftCoeff = isStalled ? liftCoefficient * 0.3f : liftCoefficient;
            
            // Lift force (perpendicular to velocity)
            float dynamicPressure = 0.5f * atmosphericDensity * speed * speed;
            float liftMagnitude = dynamicPressure * wingArea * effectiveLiftCoeff * (1f + flapsExtension * 0.5f);
            Vector3 liftDirection = Vector3.Cross(Vector3.Cross(velocityDirection, transform.right), velocityDirection).normalized;
            Vector3 liftForce = liftDirection * liftMagnitude;
            
            // Drag force (opposite to velocity)
            float dragMagnitude = dynamicPressure * wingArea * (dragCoefficient + flapsExtension * 0.1f);
            Vector3 dragForce = -velocityDirection * dragMagnitude;
            
            // Apply forces
            rb.AddForce(liftForce);
            rb.AddForce(dragForce);
        }
        
        /// <summary>
        /// Apply engine thrust
        /// </summary>
        private void ApplyEngineThrust()
        {
            float currentThrust = Mathf.Lerp(minThrust, maxThrust, throttle);
            if (afterburnerActive && throttle > 0.9f)
            {
                currentThrust *= 1.3f; // 30% boost with afterburner
            }
            
            Vector3 thrustForce = transform.forward * currentThrust;
            rb.AddForce(thrustForce);
        }
        
        /// <summary>
        /// Apply control surface effects (ailerons, elevator, rudder)
        /// </summary>
        private void ApplyControlSurfaces()
        {
            float speed = rb.velocity.magnitude;
            float controlAuthority = Mathf.Clamp01(speed / 50f); // More authority at higher speeds
            
            // Roll (ailerons)
            float rollTorque = aileronInput * 50000f * controlAuthority;
            rb.AddRelativeTorque(Vector3.forward * rollTorque);
            
            // Pitch (elevator)
            float pitchTorque = elevatorInput * 80000f * controlAuthority;
            rb.AddRelativeTorque(Vector3.right * pitchTorque);
            
            // Yaw (rudder)
            float yawTorque = rudderInput * 60000f * controlAuthority;
            rb.AddRelativeTorque(Vector3.up * yawTorque);
        }
        
        /// <summary>
        /// Apply damage effects to flight performance
        /// </summary>
        private void ApplyDamageEffects()
        {
            if (structuralIntegrity < 1f)
            {
                // Reduced control authority
                float damageMultiplier = Mathf.Clamp01(structuralIntegrity);
                
                // Add instability
                float instability = (1f - structuralIntegrity) * 20000f;
                Vector3 randomTorque = new Vector3(
                    Random.Range(-instability, instability),
                    Random.Range(-instability, instability),
                    Random.Range(-instability, instability)
                );
                rb.AddTorque(randomTorque);
                
                // Increased drag from damage
                float extraDrag = (1f - structuralIntegrity) * 5000f;
                rb.AddForce(-rb.velocity.normalized * extraDrag);
            }
        }
        
        /// <summary>
        /// Apply damage to the aircraft
        /// </summary>
        public void TakeDamage(float damageAmount)
        {
            structuralIntegrity = Mathf.Clamp01(structuralIntegrity - damageAmount);
        }
        
        /// <summary>
        /// Get current G-force being experienced
        /// </summary>
        public float GetGForce()
        {
            Vector3 acceleration = (rb.velocity - velocity) / Time.fixedDeltaTime;
            return acceleration.magnitude / 9.81f;
        }
        
        /// <summary>
        /// Check if aircraft is in stall condition
        /// </summary>
        public bool IsStalled()
        {
            if (rb.velocity.magnitude < 0.1f) return false;
            
            Vector3 velocityDirection = rb.velocity.normalized;
            float angleOfAttack = Vector3.Angle(transform.forward, velocityDirection);
            return angleOfAttack > stallAngle;
        }
    }
}
