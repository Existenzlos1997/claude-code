using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace EarthUnderFreelancer.Player
{
    /// <summary>
    /// Handles mobile/touch input for the game
    /// </summary>
    public class MobileInputManager : MonoBehaviour
    {
        public static MobileInputManager Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private RectTransform movementJoystick;
        [SerializeField] private RectTransform movementKnob;
        [SerializeField] private RectTransform aimJoystick;
        [SerializeField] private RectTransform aimKnob;
        [SerializeField] private Button fireButton;
        [SerializeField] private Button secondaryFireButton;
        [SerializeField] private Button boostButton;
        [SerializeField] private Button brakeButton;

        [Header("Settings")]
        [SerializeField] private float joystickRange = 100f;
        [SerializeField] private float deadzone = 0.1f;
        [SerializeField] private bool useGyroscope = false;
        [SerializeField] private float gyroSensitivity = 1f;

        public Vector2 MovementInput { get; private set; }
        public Vector2 LookInput { get; private set; }
        public bool FireInput { get; private set; }
        public bool SecondaryFireInput { get; private set; }
        public bool BoostInput { get; private set; }
        public bool BrakeInput { get; private set; }

        private int movementTouchId = -1;
        private int aimTouchId = -1;
        private Vector2 movementStartPos;
        private Vector2 aimStartPos;
        private bool isFirePressed = false;
        private bool isSecondaryFirePressed = false;
        private bool isBoostPressed = false;
        private bool isBrakePressed = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            SetupButtonListeners();

            if (useGyroscope && SystemInfo.supportsGyroscope)
            {
                Input.gyro.enabled = true;
            }
        }

        private void SetupButtonListeners()
        {
            SetupButton(fireButton, ref isFirePressed);
            SetupButton(secondaryFireButton, ref isSecondaryFirePressed);
            SetupButton(boostButton, ref isBoostPressed);
            SetupButton(brakeButton, ref isBrakePressed);
        }

        private void SetupButton(Button button, ref bool pressedState)
        {
            if (button == null) return;
            
            bool localPressed = false;
            EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();
            
            EventTrigger.Entry pointerDown = new EventTrigger.Entry();
            pointerDown.eventID = EventTriggerType.PointerDown;
            pointerDown.callback.AddListener((data) => { localPressed = true; });
            trigger.triggers.Add(pointerDown);

            EventTrigger.Entry pointerUp = new EventTrigger.Entry();
            pointerUp.eventID = EventTriggerType.PointerUp;
            pointerUp.callback.AddListener((data) => { localPressed = false; });
            trigger.triggers.Add(pointerUp);
        }

        private void Update()
        {
            HandleTouchInput();
            UpdateButtonInputs();

            if (useGyroscope && SystemInfo.supportsGyroscope)
            {
                HandleGyroscopeInput();
            }
        }

        private void HandleTouchInput()
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                bool isLeftSide = touch.position.x < Screen.width / 2;

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        if (isLeftSide && movementTouchId == -1)
                        {
                            movementTouchId = touch.fingerId;
                            movementStartPos = touch.position;
                            if (movementJoystick != null)
                            {
                                movementJoystick.position = touch.position;
                                movementJoystick.gameObject.SetActive(true);
                            }
                        }
                        else if (!isLeftSide && aimTouchId == -1 && !IsOverUIButton(touch.position))
                        {
                            aimTouchId = touch.fingerId;
                            aimStartPos = touch.position;
                            if (aimJoystick != null)
                            {
                                aimJoystick.position = touch.position;
                                aimJoystick.gameObject.SetActive(true);
                            }
                        }
                        break;

                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        if (touch.fingerId == movementTouchId)
                        {
                            Vector2 delta = touch.position - movementStartPos;
                            MovementInput = Vector2.ClampMagnitude(delta / joystickRange, 1f);
                            
                            if (movementKnob != null)
                            {
                                movementKnob.anchoredPosition = MovementInput * joystickRange;
                            }
                        }
                        else if (touch.fingerId == aimTouchId)
                        {
                            Vector2 delta = touch.position - aimStartPos;
                            LookInput = Vector2.ClampMagnitude(delta / joystickRange, 1f);
                            
                            if (aimKnob != null)
                            {
                                aimKnob.anchoredPosition = LookInput * joystickRange;
                            }
                        }
                        break;

                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        if (touch.fingerId == movementTouchId)
                        {
                            movementTouchId = -1;
                            MovementInput = Vector2.zero;
                            if (movementJoystick != null) movementJoystick.gameObject.SetActive(false);
                            if (movementKnob != null) movementKnob.anchoredPosition = Vector2.zero;
                        }
                        else if (touch.fingerId == aimTouchId)
                        {
                            aimTouchId = -1;
                            LookInput = Vector2.zero;
                            if (aimJoystick != null) aimJoystick.gameObject.SetActive(false);
                            if (aimKnob != null) aimKnob.anchoredPosition = Vector2.zero;
                        }
                        break;
                }
            }

            if (MovementInput.magnitude < deadzone) MovementInput = Vector2.zero;
            if (LookInput.magnitude < deadzone) LookInput = Vector2.zero;
        }

        private void HandleGyroscopeInput()
        {
            Vector3 gyroRotation = Input.gyro.rotationRateUnbiased;
            LookInput = new Vector2(gyroRotation.y, -gyroRotation.x) * gyroSensitivity;
        }

        private void UpdateButtonInputs()
        {
            FireInput = isFirePressed;
            SecondaryFireInput = isSecondaryFirePressed;
            BoostInput = isBoostPressed;
            BrakeInput = isBrakePressed;
        }

        private bool IsOverUIButton(Vector2 position)
        {
            if (fireButton != null && RectTransformUtility.RectangleContainsScreenPoint(
                fireButton.GetComponent<RectTransform>(), position)) return true;
            if (secondaryFireButton != null && RectTransformUtility.RectangleContainsScreenPoint(
                secondaryFireButton.GetComponent<RectTransform>(), position)) return true;
            if (boostButton != null && RectTransformUtility.RectangleContainsScreenPoint(
                boostButton.GetComponent<RectTransform>(), position)) return true;
            if (brakeButton != null && RectTransformUtility.RectangleContainsScreenPoint(
                brakeButton.GetComponent<RectTransform>(), position)) return true;

            return false;
        }

        public void SetGyroscope(bool enabled)
        {
            useGyroscope = enabled;
            if (SystemInfo.supportsGyroscope) Input.gyro.enabled = enabled;
        }

        public void SetDeadzone(float value) => deadzone = value;
        public void SetJoystickRange(float range) => joystickRange = range;
    }
}
