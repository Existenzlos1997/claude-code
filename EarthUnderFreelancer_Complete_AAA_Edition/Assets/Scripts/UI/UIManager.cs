using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace EarthUnderFreelancer.UI
{
    /// <summary>
    /// Central UI Manager for all game UI
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Screen References")]
        [SerializeField] private GameObject mainMenuScreen;
        [SerializeField] private GameObject gameHUDScreen;
        [SerializeField] private GameObject pauseMenuScreen;
        [SerializeField] private GameObject settingsScreen;
        [SerializeField] private GameObject inventoryScreen;
        [SerializeField] private GameObject tradingScreen;
        [SerializeField] private GameObject missionScreen;
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private GameObject deathScreen;

        [Header("HUD Elements")]
        [SerializeField] private Slider healthBar;
        [SerializeField] private Slider shieldBar;
        [SerializeField] private Slider boostBar;
        [SerializeField] private TextMeshProUGUI speedText;
        [SerializeField] private TextMeshProUGUI creditsText;
        [SerializeField] private TextMeshProUGUI targetNameText;
        [SerializeField] private TextMeshProUGUI targetDistanceText;
        [SerializeField] private Slider targetHealthBar;
        [SerializeField] private Image crosshair;
        [SerializeField] private Image leadIndicator;
        [SerializeField] private Image lockOnIndicator;
        [SerializeField] private TextMeshProUGUI ammoText;
        [SerializeField] private TextMeshProUGUI missileCountText;

        [Header("Notifications")]
        [SerializeField] private Transform notificationContainer;
        [SerializeField] private GameObject notificationPrefab;
        [SerializeField] private float notificationDuration = 3f;

        [Header("Minimap")]
        [SerializeField] private RawImage minimapImage;
        [SerializeField] private Camera minimapCamera;

        private UIScreen currentScreen = UIScreen.None;
        private Stack<UIScreen> screenHistory = new Stack<UIScreen>();

        public UIScreen CurrentScreen => currentScreen;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            HideAllScreens();
            ShowScreen(UIScreen.MainMenu);

            // Subscribe to events
            if (Core.GameManager.Instance != null)
            {
                Core.GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
            }
        }

        private void Update()
        {
            if (currentScreen == UIScreen.GameHUD)
            {
                UpdateHUD();
            }
        }

        private void OnGameStateChanged(Core.GameManager.GameState newState)
        {
            switch (newState)
            {
                case Core.GameManager.GameState.MainMenu:
                    ShowScreen(UIScreen.MainMenu);
                    break;
                case Core.GameManager.GameState.Playing:
                    ShowScreen(UIScreen.GameHUD);
                    break;
                case Core.GameManager.GameState.Paused:
                    ShowScreen(UIScreen.PauseMenu);
                    break;
                case Core.GameManager.GameState.Loading:
                    ShowScreen(UIScreen.Loading);
                    break;
                case Core.GameManager.GameState.GameOver:
                    ShowScreen(UIScreen.Death);
                    break;
            }
        }

        private void UpdateHUD()
        {
            UpdatePlayerStats();
            UpdateTargetInfo();
            UpdateCrosshair();
            UpdateCredits();
        }

        private void UpdatePlayerStats()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;

            var health = player.GetComponent<Vehicles.HealthSystem>();
            if (health != null)
            {
                if (healthBar != null)
                {
                    healthBar.value = health.CurrentHealth / health.MaxHealth;
                }
                if (shieldBar != null)
                {
                    shieldBar.value = health.CurrentShield / health.MaxShield;
                }
            }

            var vehicle = player.GetComponent<Vehicles.VehicleController>();
            if (vehicle != null)
            {
                if (speedText != null)
                {
                    speedText.text = $"{Mathf.RoundToInt(vehicle.CurrentSpeed)} m/s";
                }
                if (boostBar != null)
                {
                    boostBar.value = vehicle.BoostAmount;
                }
            }

            var weapons = player.GetComponent<Combat.WeaponController>();
            if (weapons != null)
            {
                if (missileCountText != null)
                {
                    missileCountText.text = weapons.MissileCount.ToString();
                }
            }
        }

        private void UpdateTargetInfo()
        {
            var targeting = Combat.TargetingSystem.Instance;
            if (targeting == null) return;

            Transform target = targeting.CurrentTarget;

            bool hasTarget = target != null;

            if (targetNameText != null) targetNameText.gameObject.SetActive(hasTarget);
            if (targetDistanceText != null) targetDistanceText.gameObject.SetActive(hasTarget);
            if (targetHealthBar != null) targetHealthBar.gameObject.SetActive(hasTarget);

            if (!hasTarget) return;

            if (targetNameText != null)
            {
                targetNameText.text = target.name;
            }

            if (targetDistanceText != null)
            {
                targetDistanceText.text = $"{Mathf.RoundToInt(targeting.DistanceToTarget)} m";
            }

            // Get target health
            var aiShip = target.GetComponent<AI.AIShipController>();
            if (aiShip != null && targetHealthBar != null)
            {
                targetHealthBar.value = aiShip.CurrentHull / aiShip.MaxHull;
            }
        }

        private void UpdateCrosshair()
        {
            var targeting = Combat.TargetingSystem.Instance;
            if (targeting == null || leadIndicator == null) return;

            if (targeting.HasValidLead && Camera.main != null)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(targeting.LeadPosition);
                if (screenPos.z > 0)
                {
                    leadIndicator.gameObject.SetActive(true);
                    leadIndicator.transform.position = screenPos;
                }
                else
                {
                    leadIndicator.gameObject.SetActive(false);
                }
            }
            else
            {
                leadIndicator.gameObject.SetActive(false);
            }
        }

        private void UpdateCredits()
        {
            if (creditsText != null && Systems.EconomyManager.Instance != null)
            {
                creditsText.text = $"${Systems.EconomyManager.Instance.PlayerCredits:N0}";
            }
        }

        public void ShowScreen(UIScreen screen)
        {
            if (currentScreen != UIScreen.None)
            {
                screenHistory.Push(currentScreen);
            }

            HideAllScreens();
            currentScreen = screen;

            switch (screen)
            {
                case UIScreen.MainMenu:
                    if (mainMenuScreen != null) mainMenuScreen.SetActive(true);
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    break;
                case UIScreen.GameHUD:
                    if (gameHUDScreen != null) gameHUDScreen.SetActive(true);
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    break;
                case UIScreen.PauseMenu:
                    if (pauseMenuScreen != null) pauseMenuScreen.SetActive(true);
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    break;
                case UIScreen.Settings:
                    if (settingsScreen != null) settingsScreen.SetActive(true);
                    break;
                case UIScreen.Inventory:
                    if (inventoryScreen != null) inventoryScreen.SetActive(true);
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    break;
                case UIScreen.Trading:
                    if (tradingScreen != null) tradingScreen.SetActive(true);
                    break;
                case UIScreen.Mission:
                    if (missionScreen != null) missionScreen.SetActive(true);
                    break;
                case UIScreen.Loading:
                    if (loadingScreen != null) loadingScreen.SetActive(true);
                    break;
                case UIScreen.Death:
                    if (deathScreen != null) deathScreen.SetActive(true);
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    break;
            }
        }

        public void HideScreen(UIScreen screen)
        {
            switch (screen)
            {
                case UIScreen.MainMenu: if (mainMenuScreen != null) mainMenuScreen.SetActive(false); break;
                case UIScreen.GameHUD: if (gameHUDScreen != null) gameHUDScreen.SetActive(false); break;
                case UIScreen.PauseMenu: if (pauseMenuScreen != null) pauseMenuScreen.SetActive(false); break;
                case UIScreen.Settings: if (settingsScreen != null) settingsScreen.SetActive(false); break;
                case UIScreen.Inventory: if (inventoryScreen != null) inventoryScreen.SetActive(false); break;
                case UIScreen.Trading: if (tradingScreen != null) tradingScreen.SetActive(false); break;
                case UIScreen.Mission: if (missionScreen != null) missionScreen.SetActive(false); break;
                case UIScreen.Loading: if (loadingScreen != null) loadingScreen.SetActive(false); break;
                case UIScreen.Death: if (deathScreen != null) deathScreen.SetActive(false); break;
            }
        }

        public void HideAllScreens()
        {
            if (mainMenuScreen != null) mainMenuScreen.SetActive(false);
            if (gameHUDScreen != null) gameHUDScreen.SetActive(false);
            if (pauseMenuScreen != null) pauseMenuScreen.SetActive(false);
            if (settingsScreen != null) settingsScreen.SetActive(false);
            if (inventoryScreen != null) inventoryScreen.SetActive(false);
            if (tradingScreen != null) tradingScreen.SetActive(false);
            if (missionScreen != null) missionScreen.SetActive(false);
            if (loadingScreen != null) loadingScreen.SetActive(false);
            if (deathScreen != null) deathScreen.SetActive(false);
        }

        public void GoBack()
        {
            if (screenHistory.Count > 0)
            {
                currentScreen = UIScreen.None;
                ShowScreen(screenHistory.Pop());
            }
        }

        public void ShowNotification(string message, NotificationType type = NotificationType.Info)
        {
            if (notificationContainer == null || notificationPrefab == null) return;

            GameObject notif = Instantiate(notificationPrefab, notificationContainer);
            TextMeshProUGUI text = notif.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null) text.text = message;

            Image bg = notif.GetComponent<Image>();
            if (bg != null)
            {
                switch (type)
                {
                    case NotificationType.Info: bg.color = new Color(0.2f, 0.4f, 0.8f, 0.9f); break;
                    case NotificationType.Warning: bg.color = new Color(0.8f, 0.6f, 0.2f, 0.9f); break;
                    case NotificationType.Error: bg.color = new Color(0.8f, 0.2f, 0.2f, 0.9f); break;
                    case NotificationType.Success: bg.color = new Color(0.2f, 0.8f, 0.3f, 0.9f); break;
                }
            }

            Destroy(notif, notificationDuration);
        }

        public void SetLoadingProgress(float progress, string message = "")
        {
            // Update loading screen progress bar if exists
        }

        // Button handlers
        public void OnPlayButtonClicked()
        {
            Core.GameManager.Instance?.StartGame();
        }

        public void OnSettingsButtonClicked()
        {
            ShowScreen(UIScreen.Settings);
        }

        public void OnQuitButtonClicked()
        {
            Core.GameManager.Instance?.QuitGame();
        }

        public void OnResumeButtonClicked()
        {
            Core.GameManager.Instance?.SetGameState(Core.GameManager.GameState.Playing);
        }

        public void OnMainMenuButtonClicked()
        {
            Core.GameManager.Instance?.ReturnToMainMenu();
        }

        public void OnRestartButtonClicked()
        {
            Core.GameManager.Instance?.RestartGame();
        }
    }

    public enum UIScreen
    {
        None,
        MainMenu,
        GameHUD,
        PauseMenu,
        Settings,
        Inventory,
        Trading,
        Mission,
        Loading,
        Death
    }

    public enum NotificationType
    {
        Info,
        Warning,
        Error,
        Success
    }
}
