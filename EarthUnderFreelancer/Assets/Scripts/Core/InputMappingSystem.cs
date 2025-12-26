using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Core
{
    /// <summary>
    /// Advanced input mapping system with full rebinding support
    /// Replaces hard-coded controls with flexible, rebindable system
    /// </summary>
    public class InputMappingSystem : MonoBehaviour
    {
        public enum InputContext
        {
            Flight,
            Combat,
            Menu,
            Trading,
            Map
        }
        
        [System.Serializable]
        public class InputBinding
        {
            public string actionName;
            public KeyCode primaryKey;
            public KeyCode secondaryKey;
            public int mouseButton = -1; // -1 = not mouse
            public string gamepadButton;
            public bool requiresModifier;
            public KeyCode modifierKey;
            public InputContext context;
        }
        
        [Header("Input Contexts")]
        [SerializeField] private InputContext currentContext = InputContext.Flight;
        
        private Dictionary<string, InputBinding> bindings = new Dictionary<string, InputBinding>();
        private Dictionary<string, float> axisValues = new Dictionary<string, float>();
        private Dictionary<KeyCode, string> keyToAction = new Dictionary<KeyCode, string>();
        
        public static InputMappingSystem Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeDefaultBindings();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void InitializeDefaultBindings()
        {
            // Flight controls
            AddBinding("Throttle_Up", KeyCode.W, InputContext.Flight);
            AddBinding("Throttle_Down", KeyCode.S, InputContext.Flight);
            AddBinding("Yaw_Left", KeyCode.A, InputContext.Flight);
            AddBinding("Yaw_Right", KeyCode.D, InputContext.Flight);
            AddBinding("Boost", KeyCode.LeftShift, InputContext.Flight);
            AddBinding("Brake", KeyCode.Space, InputContext.Flight);
            
            // Combat controls
            AddBinding("Fire_Primary", KeyCode.Mouse0, InputContext.Combat, mouseButton: 0);
            AddBinding("Fire_Secondary", KeyCode.Mouse1, InputContext.Combat, mouseButton: 1);
            AddBinding("Target_Next", KeyCode.Tab, InputContext.Combat);
            AddBinding("Target_Prev", KeyCode.Tab, InputContext.Combat, requiresModifier: true, modifierKey: KeyCode.LeftShift);
            AddBinding("Weapon_Switch", KeyCode.R, InputContext.Combat);
            
            // Power management
            AddBinding("Power_Weapons", KeyCode.Alpha1, InputContext.Flight);
            AddBinding("Power_Shields", KeyCode.Alpha2, InputContext.Flight);
            AddBinding("Power_Engines", KeyCode.Alpha3, InputContext.Flight);
            AddBinding("Power_Balanced", KeyCode.Alpha4, InputContext.Flight);
            
            // UI/System
            AddBinding("Menu_Toggle", KeyCode.Escape, InputContext.Menu);
            AddBinding("Map_Toggle", KeyCode.M, InputContext.Map);
            AddBinding("Inventory", KeyCode.I, InputContext.Menu);
            
            RebuildKeyMappings();
        }
        
        private void AddBinding(string actionName, KeyCode key, InputContext context, 
            int mouseButton = -1, bool requiresModifier = false, KeyCode modifierKey = KeyCode.None)
        {
            bindings[actionName] = new InputBinding
            {
                actionName = actionName,
                primaryKey = key,
                mouseButton = mouseButton,
                requiresModifier = requiresModifier,
                modifierKey = modifierKey,
                context = context
            };
        }
        
        private void RebuildKeyMappings()
        {
            keyToAction.Clear();
            foreach (var binding in bindings.Values)
            {
                if (!keyToAction.ContainsKey(binding.primaryKey))
                {
                    keyToAction[binding.primaryKey] = binding.actionName;
                }
            }
        }
        
        private void Update()
        {
            // Update axis values (for analog inputs like joysticks)
            UpdateAxisValues();
        }
        
        /// <summary>
        /// Check if action button was pressed this frame
        /// </summary>
        public bool GetButtonDown(string actionName)
        {
            if (!bindings.ContainsKey(actionName)) return false;
            
            InputBinding binding = bindings[actionName];
            if (binding.context != currentContext) return false;
            
            bool modifierPressed = !binding.requiresModifier || Input.GetKey(binding.modifierKey);
            
            if (binding.mouseButton >= 0)
            {
                return modifierPressed && Input.GetMouseButtonDown(binding.mouseButton);
            }
            
            return modifierPressed && Input.GetKeyDown(binding.primaryKey);
        }
        
        /// <summary>
        /// Check if action button is currently held
        /// </summary>
        public bool GetButton(string actionName)
        {
            if (!bindings.ContainsKey(actionName)) return false;
            
            InputBinding binding = bindings[actionName];
            if (binding.context != currentContext) return false;
            
            bool modifierPressed = !binding.requiresModifier || Input.GetKey(binding.modifierKey);
            
            if (binding.mouseButton >= 0)
            {
                return modifierPressed && Input.GetMouseButton(binding.mouseButton);
            }
            
            return modifierPressed && Input.GetKey(binding.primaryKey);
        }
        
        /// <summary>
        /// Get axis value for analog inputs
        /// </summary>
        public float GetAxis(string axisName)
        {
            if (axisValues.ContainsKey(axisName))
            {
                return axisValues[axisName];
            }
            return 0f;
        }
        
        /// <summary>
        /// Rebind an action to a new key
        /// </summary>
        public bool RebindAction(string actionName, KeyCode newKey)
        {
            if (!bindings.ContainsKey(actionName)) return false;
            
            // Check for conflicts
            if (keyToAction.ContainsKey(newKey) && keyToAction[newKey] != actionName)
            {
                Debug.LogWarning($"Key {newKey} already bound to {keyToAction[newKey]}");
                return false;
            }
            
            bindings[actionName].primaryKey = newKey;
            RebuildKeyMappings();
            SaveBindings();
            return true;
        }
        
        /// <summary>
        /// Switch input context (e.g., from Flight to Menu)
        /// </summary>
        public void SetContext(InputContext newContext)
        {
            currentContext = newContext;
        }
        
        private void UpdateAxisValues()
        {
            // Mouse axes
            axisValues["Mouse_X"] = Input.GetAxis("Mouse X");
            axisValues["Mouse_Y"] = Input.GetAxis("Mouse Y");
            
            // Joystick axes (if connected)
            axisValues["Joystick_X"] = Input.GetAxis("Horizontal");
            axisValues["Joystick_Y"] = Input.GetAxis("Vertical");
        }
        
        private void SaveBindings()
        {
            // Save to PlayerPrefs or file
            // Implementation would serialize bindings dictionary
        }
        
        private void LoadBindings()
        {
            // Load from PlayerPrefs or file
        }
    }
}
