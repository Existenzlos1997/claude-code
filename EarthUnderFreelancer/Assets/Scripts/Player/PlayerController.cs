using UnityEngine;
using EarthUnderFreelancer.Core;
using EarthUnderFreelancer.Vehicles;
using EarthUnderFreelancer.Combat;

namespace EarthUnderFreelancer.Player
{
    /// <summary>
    /// Main player controller that handles input and delegates to vehicle control
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private VehicleController vehicleController;
        [SerializeField] private WeaponController weaponController;
        [SerializeField] private CameraController cameraController;

        [Header("Input Settings")]
        [SerializeField] private bool useMobileControls = false;
        [SerializeField] private float inputDeadzone = 0.1f;

        private Vector2 movementInput;
        private Vector2 lookInput;
        private bool fireInput;
        private bool secondaryFireInput;
        private bool boostInput;
        private bool brakeInput;
        private bool isControlEnabled = true;

        private void Start()
        {
#if UNITY_ANDROID || UNITY_IOS
            useMobileControls = true;
#else
            useMobileControls = false;
#endif

            if (vehicleController == null)
                vehicleController = GetComponent<VehicleController>();
            if (weaponController == null)
                weaponController = GetComponent<WeaponController>();
        }

        private void Update()
        {
            if (!isControlEnabled) return;

            if (useMobileControls)
            {
                HandleMobileInput();
            }
            else
            {
                HandleKeyboardMouseInput();
            }

            ApplyInput();
        }

        private void HandleKeyboardMouseInput()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            movementInput = new Vector2(horizontal, vertical);

            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            lookInput = new Vector2(mouseX, mouseY);

            fireInput = Input.GetButton("Fire1");
            secondaryFireInput = Input.GetButton("Fire2");
            boostInput = Input.GetKey(KeyCode.LeftShift);
            brakeInput = Input.GetKey(KeyCode.Space);

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (weaponController != null)
                    weaponController.CycleTarget();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                if (weaponController != null)
                    weaponController.CycleWeapon();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (GameManager.Instance != null)
                {
                    if (GameManager.Instance.CurrentState == GameManager.GameState.Playing)
                        GameManager.Instance.SetGameState(GameManager.GameState.Paused);
                    else if (GameManager.Instance.CurrentState == GameManager.GameState.Paused)
                        GameManager.Instance.SetGameState(GameManager.GameState.Playing);
                }
            }
        }

        private void HandleMobileInput()
        {
            if (MobileInputManager.Instance != null)
            {
                movementInput = MobileInputManager.Instance.MovementInput;
                lookInput = MobileInputManager.Instance.LookInput;
                fireInput = MobileInputManager.Instance.FireInput;
                secondaryFireInput = MobileInputManager.Instance.SecondaryFireInput;
                boostInput = MobileInputManager.Instance.BoostInput;
                brakeInput = MobileInputManager.Instance.BrakeInput;
            }
        }

        private void ApplyInput()
        {
            if (movementInput.magnitude < inputDeadzone)
                movementInput = Vector2.zero;
            if (lookInput.magnitude < inputDeadzone)
                lookInput = Vector2.zero;

            if (vehicleController != null)
            {
                vehicleController.SetMovementInput(movementInput);
                vehicleController.SetLookInput(lookInput);
                vehicleController.SetBoost(boostInput);
                vehicleController.SetBrake(brakeInput);
            }

            if (weaponController != null)
            {
                if (fireInput)
                    weaponController.FirePrimary();
                if (secondaryFireInput)
                    weaponController.FireSecondary();
            }
        }

        public void EnableControls()
        {
            isControlEnabled = true;
        }

        public void DisableControls()
        {
            isControlEnabled = false;
            movementInput = Vector2.zero;
            lookInput = Vector2.zero;
            fireInput = false;
            secondaryFireInput = false;
            boostInput = false;
            brakeInput = false;

            if (vehicleController != null)
            {
                vehicleController.SetMovementInput(Vector2.zero);
                vehicleController.SetLookInput(Vector2.zero);
            }
        }

        public void SetMobileControls(bool enabled)
        {
            useMobileControls = enabled;
        }
    }
}
