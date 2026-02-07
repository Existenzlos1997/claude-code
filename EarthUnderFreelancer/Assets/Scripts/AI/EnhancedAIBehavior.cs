using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.AI
{
    /// <summary>
    /// Enhanced AI behavior system with multiple personalities and tactics
    /// Implements VERBESSERUNGSPLAN Priority 3 - AI Improvements
    /// </summary>
    public class EnhancedAIBehavior : MonoBehaviour
    {
        [Header("AI Personality")]
        [SerializeField] private AIPersonality personality = AIPersonality.Balanced;
        [SerializeField] private float skillLevel = 0.5f; // 0-1, affects aim, reaction time
        
        [Header("Combat Behavior")]
        [SerializeField] private float aggressionLevel = 0.5f;
        [SerializeField] private float cautiousness = 0.5f;
        [SerializeField] private bool useAdvancedManeuvers = true;
        
        [Header("Formation Flying")]
        [SerializeField] private bool enableFormationFlying = false;
        [SerializeField] private FormationType formationType = FormationType.VFormation;
        [SerializeField] private Transform formationLeader;
        [SerializeField] private int positionInFormation = 0;
        
        public enum AIPersonality
        {
            Aggressive,     // High aggression, low caution
            Defensive,      // Low aggression, high caution
            Balanced,       // Moderate both
            Ace,           // Skilled, uses advanced maneuvers
            Rookie,        // Less skilled, basic maneuvers
            Kamikaze       // Extremely aggressive, no self-preservation
        }
        
        public enum FormationType
        {
            VFormation,
            LineAbreast,
            Echelon,
            Trail,
            Finger4
        }
        
        private Vector3 targetPosition;
        private Quaternion targetRotation;
        private float currentThrottle = 0.5f;
        
        // Advanced maneuver state
        private bool isPerformingManeuver = false;
        private float maneuverTimer = 0f;
        private ManeuverType currentManeuver = ManeuverType.None;
        
        private enum ManeuverType
        {
            None,
            BarrelRoll,
            Immelman,
            SplitS,
            YoYo,
            Scissors
        }
        
        private void Start()
        {
            ApplyPersonalitySettings();
        }
        
        private void Update()
        {
            if (enableFormationFlying && formationLeader != null)
            {
                MaintainFormation();
            }
            
            if (isPerformingManeuver)
            {
                ExecuteManeuver();
            }
        }
        
        private void ApplyPersonalitySettings()
        {
            switch (personality)
            {
                case AIPersonality.Aggressive:
                    aggressionLevel = 0.9f;
                    cautiousness = 0.2f;
                    skillLevel = 0.6f;
                    break;
                    
                case AIPersonality.Defensive:
                    aggressionLevel = 0.3f;
                    cautiousness = 0.8f;
                    skillLevel = 0.5f;
                    break;
                    
                case AIPersonality.Balanced:
                    aggressionLevel = 0.5f;
                    cautiousness = 0.5f;
                    skillLevel = 0.5f;
                    break;
                    
                case AIPersonality.Ace:
                    aggressionLevel = 0.7f;
                    cautiousness = 0.6f;
                    skillLevel = 0.9f;
                    useAdvancedManeuvers = true;
                    break;
                    
                case AIPersonality.Rookie:
                    aggressionLevel = 0.4f;
                    cautiousness = 0.3f;
                    skillLevel = 0.2f;
                    useAdvancedManeuvers = false;
                    break;
                    
                case AIPersonality.Kamikaze:
                    aggressionLevel = 1.0f;
                    cautiousness = 0.0f;
                    skillLevel = 0.4f;
                    break;
            }
        }
        
        private void MaintainFormation()
        {
            if (formationLeader == null) return;
            
            Vector3 formationOffset = CalculateFormationOffset();
            targetPosition = formationLeader.position + formationLeader.TransformDirection(formationOffset);
            targetRotation = formationLeader.rotation;
            
            // Smooth movement to formation position
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 2f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2f);
        }
        
        private Vector3 CalculateFormationOffset()
        {
            float spacing = 50f;
            
            switch (formationType)
            {
                case FormationType.VFormation:
                    // V formation - aircraft spread out in V shape
                    int side = positionInFormation % 2 == 0 ? 1 : -1;
                    int row = (positionInFormation + 1) / 2;
                    return new Vector3(side * row * spacing, 0, -row * spacing);
                    
                case FormationType.LineAbreast:
                    // Line abreast - aircraft side by side
                    return new Vector3((positionInFormation - 1) * spacing, 0, 0);
                    
                case FormationType.Echelon:
                    // Echelon - diagonal line
                    return new Vector3(positionInFormation * spacing, 0, -positionInFormation * spacing * 0.5f);
                    
                case FormationType.Trail:
                    // Trail - line astern
                    return new Vector3(0, 0, -positionInFormation * spacing);
                    
                case FormationType.Finger4:
                    // Finger-four formation (2x2)
                    int pair = positionInFormation / 2;
                    int inPair = positionInFormation % 2;
                    return new Vector3((inPair * 2 - 1) * spacing, 0, -pair * spacing * 1.5f);
                    
                default:
                    return Vector3.zero;
            }
        }
        
        public void PerformEvasiveManeuver()
        {
            if (!useAdvancedManeuvers || isPerformingManeuver) return;
            
            // Choose maneuver based on skill and personality
            ManeuverType[] availableManeuvers = new ManeuverType[]
            {
                ManeuverType.BarrelRoll,
                ManeuverType.Immelman,
                ManeuverType.SplitS
            };
            
            if (skillLevel > 0.7f)
            {
                availableManeuvers = new ManeuverType[]
                {
                    ManeuverType.BarrelRoll,
                    ManeuverType.Immelman,
                    ManeuverType.SplitS,
                    ManeuverType.YoYo,
                    ManeuverType.Scissors
                };
            }
            
            currentManeuver = availableManeuvers[Random.Range(0, availableManeuvers.Length)];
            isPerformingManeuver = true;
            maneuverTimer = 0f;
            
            Debug.Log($"[AI] {gameObject.name} performing {currentManeuver}");
        }
        
        private void ExecuteManeuver()
        {
            maneuverTimer += Time.deltaTime;
            
            switch (currentManeuver)
            {
                case ManeuverType.BarrelRoll:
                    ExecuteBarrelRoll();
                    if (maneuverTimer > 2f) isPerformingManeuver = false;
                    break;
                    
                case ManeuverType.Immelman:
                    ExecuteImmelman();
                    if (maneuverTimer > 3f) isPerformingManeuver = false;
                    break;
                    
                case ManeuverType.SplitS:
                    ExecuteSplitS();
                    if (maneuverTimer > 3f) isPerformingManeuver = false;
                    break;
                    
                case ManeuverType.YoYo:
                    ExecuteYoYo();
                    if (maneuverTimer > 4f) isPerformingManeuver = false;
                    break;
                    
                case ManeuverType.Scissors:
                    ExecuteScissors();
                    if (maneuverTimer > 3f) isPerformingManeuver = false;
                    break;
            }
        }
        
        private void ExecuteBarrelRoll()
        {
            // Roll along forward axis
            float rollSpeed = 180f * skillLevel;
            transform.Rotate(Vector3.forward, rollSpeed * Time.deltaTime);
        }
        
        private void ExecuteImmelman()
        {
            // Half loop and roll - reverses direction while gaining altitude
            if (maneuverTimer < 1.5f)
            {
                transform.Rotate(Vector3.right, 60f * Time.deltaTime);
            }
            else
            {
                transform.Rotate(Vector3.forward, 180f * Time.deltaTime);
            }
        }
        
        private void ExecuteSplitS()
        {
            // Roll and dive - opposite of Immelman
            if (maneuverTimer < 0.5f)
            {
                transform.Rotate(Vector3.forward, 180f * Time.deltaTime);
            }
            else
            {
                transform.Rotate(Vector3.right, -60f * Time.deltaTime);
            }
        }
        
        private void ExecuteYoYo()
        {
            // Vertical turning maneuver
            float phase = Mathf.Sin(maneuverTimer * Mathf.PI * 0.5f);
            transform.Rotate(Vector3.right, 45f * phase * Time.deltaTime);
        }
        
        private void ExecuteScissors()
        {
            // Defensive weaving maneuver
            float weave = Mathf.Sin(maneuverTimer * Mathf.PI * 2f);
            transform.Rotate(Vector3.up, weave * 30f * Time.deltaTime);
        }
        
        public void BreakFormation()
        {
            enableFormationFlying = false;
            formationLeader = null;
        }
        
        public void JoinFormation(Transform leader, int position, FormationType type)
        {
            formationLeader = leader;
            positionInFormation = position;
            formationType = type;
            enableFormationFlying = true;
        }
        
        // Public API
        public float GetAggressionLevel() => aggressionLevel;
        public float GetCautiousness() => cautiousness;
        public float GetSkillLevel() => skillLevel;
        public AIPersonality GetPersonality() => personality;
        public bool IsInFormation() => enableFormationFlying && formationLeader != null;
    }
}
