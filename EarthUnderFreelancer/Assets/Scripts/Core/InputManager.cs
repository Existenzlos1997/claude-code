using UnityEngine;
using System;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Core
{
    /// <summary>
    /// Centralized input manager with rebindable controls
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private float mouseSensitivity = 1f;
        [SerializeField] private bool invertY = false;
        [SerializeField] private float deadzone = 0.1f;

        // Input state
        private Dictionary<string, KeyCode> keyBindings = new Dictionary<string, KeyCode>();
        private Vector2 moveInput;
        private Vector2 lookInput;
        private bool isMobile;

        // Input Actions
        public Vector2 MoveInput => moveInput;
        public Vector2 LookInput => lookInput;
        public float MouseSensitivity => mouseSensitivity;

        // Events
        public event Action OnFirePressed;
        public event Action OnFireReleased;
        public event Action OnSecondaryFirePressed;
        public event Action OnSecondaryFireReleased;
        public event Action OnBoostPressed;
        public event Action OnBoostReleased;
        public event Action OnBrakePressed;
        public event Action OnBrakeReleased;
        public event Action OnPausePressed;
        public event Action OnTargetCycle;
        public event Action OnWeaponCycle;
        public event Action OnInteract;

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

#if UNITY_ANDROID || UNITY_IOS
            isMobile = true;
#else
            isMobile = false;
#endif
        }

        private void InitializeDefaultBindings()
        {
            keyBindings["Fire"] = KeyCode.Mouse0;
            keyBindings["SecondaryFire"] = KeyCode.Mouse1;
            keyBindings["Boost"] = KeyCode.LeftShift;
            keyBindings["Brake"] = KeyCode.Space;
            keyBindings["Pause"] = KeyCode.Escape;
            keyBindings["TargetCycle"] = KeyCode.Tab;
            keyBindings["WeaponCycle"] = KeyCode.R;
            keyBindings["Interact"] = KeyCode.E;
            keyBindings["MoveUp"] = KeyCode.W;
            keyBindings["MoveDown"] = KeyCode.S;
            keyBindings["MoveLeft"] = KeyCode.A;
            keyBindings["MoveRight"] = KeyCode.D;
            keyBindings["RollLeft"] = KeyCode.Q;
            keyBindings["RollRight"] = KeyCode.E;
            keyBindings["Afterburner"] = KeyCode.LeftControl;
        }

        private void Update()
        {
            if (isMobile) return; // Mobile input handled by MobileInputManager

            ProcessMovementInput();
            ProcessLookInput();
            ProcessActionInput();
        }

        private void ProcessMovementInput()
        {
            float horizontal = 0f;
            float vertical = 0f;

            if (Input.GetKey(keyBindings["MoveRight"])) horizontal += 1f;
            if (Input.GetKey(keyBindings["MoveLeft"])) horizontal -= 1f;
            if (Input.GetKey(keyBindings["MoveUp"])) vertical += 1f;
            if (Input.GetKey(keyBindings["MoveDown"])) vertical -= 1f;

            // Also support axis input
            horizontal += Input.GetAxis("Horizontal");
            vertical += Input.GetAxis("Vertical");

            moveInput = new Vector2(
                Mathf.Clamp(horizontal, -1f, 1f),
                Mathf.Clamp(vertical, -1f, 1f)
            );

            // Apply deadzone
            if (moveInput.magnitude < deadzone)
            {
                moveInput = Vector2.zero;
            }
        }

        private void ProcessLookInput()
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            if (invertY) mouseY = -mouseY;

            lookInput = new Vector2(mouseX, mouseY);
        }

        private void ProcessActionInput()
        {
            // Fire
            if (Input.GetKeyDown(keyBindings["Fire"]))
                OnFirePressed?.Invoke();
            if (Input.GetKeyUp(keyBindings["Fire"]))
                OnFireReleased?.Invoke();

            // Secondary Fire
            if (Input.GetKeyDown(keyBindings["SecondaryFire"]))
                OnSecondaryFirePressed?.Invoke();
            if (Input.GetKeyUp(keyBindings["SecondaryFire"]))
                OnSecondaryFireReleased?.Invoke();

            // Boost
            if (Input.GetKeyDown(keyBindings["Boost"]))
                OnBoostPressed?.Invoke();
            if (Input.GetKeyUp(keyBindings["Boost"]))
                OnBoostReleased?.Invoke();

            // Brake
            if (Input.GetKeyDown(keyBindings["Brake"]))
                OnBrakePressed?.Invoke();
            if (Input.GetKeyUp(keyBindings["Brake"]))
                OnBrakeReleased?.Invoke();

            // Other actions
            if (Input.GetKeyDown(keyBindings["Pause"]))
                OnPausePressed?.Invoke();
            if (Input.GetKeyDown(keyBindings["TargetCycle"]))
                OnTargetCycle?.Invoke();
            if (Input.GetKeyDown(keyBindings["WeaponCycle"]))
                OnWeaponCycle?.Invoke();
            if (Input.GetKeyDown(keyBindings["Interact"]))
                OnInteract?.Invoke();
        }

        public bool GetButton(string action)
        {
            if (keyBindings.TryGetValue(action, out KeyCode key))
            {
                return Input.GetKey(key);
            }
            return false;
        }

        public bool GetButtonDown(string action)
        {
            if (keyBindings.TryGetValue(action, out KeyCode key))
            {
                return Input.GetKeyDown(key);
            }
            return false;
        }

        public bool GetButtonUp(string action)
        {
            if (keyBindings.TryGetValue(action, out KeyCode key))
            {
                return Input.GetKeyUp(key);
            }
            return false;
        }

        public void SetKeyBinding(string action, KeyCode key)
        {
            keyBindings[action] = key;
            SaveBindings();
        }

        public KeyCode GetKeyBinding(string action)
        {
            if (keyBindings.TryGetValue(action, out KeyCode key))
            {
                return key;
            }
            return KeyCode.None;
        }

        public void SetMouseSensitivity(float sensitivity)
        {
            mouseSensitivity = Mathf.Clamp(sensitivity, 0.1f, 5f);
        }

        public void SetInvertY(bool invert)
        {
            invertY = invert;
        }

        public void ResetToDefaults()
        {
            InitializeDefaultBindings();
            mouseSensitivity = 1f;
            invertY = false;
        }

        private void SaveBindings()
        {
            foreach (var binding in keyBindings)
            {
                PlayerPrefs.SetInt($"KeyBinding_{binding.Key}", (int)binding.Value);
            }
            PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivity);
            PlayerPrefs.SetInt("InvertY", invertY ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void LoadBindings()
        {
            foreach (var key in new List<string>(keyBindings.Keys))
            {
                if (PlayerPrefs.HasKey($"KeyBinding_{key}"))
                {
                    keyBindings[key] = (KeyCode)PlayerPrefs.GetInt($"KeyBinding_{key}");
                }
            }

            if (PlayerPrefs.HasKey("MouseSensitivity"))
                mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity");
            if (PlayerPrefs.HasKey("InvertY"))
                invertY = PlayerPrefs.GetInt("InvertY") == 1;
        }

        public Dictionary<string, KeyCode> GetAllBindings()
        {
            return new Dictionary<string, KeyCode>(keyBindings);
        }
    }
}
