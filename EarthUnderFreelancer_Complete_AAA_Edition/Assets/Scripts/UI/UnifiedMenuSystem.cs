using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace EarthUnderFreelancer.UI
{
    /// <summary>
    /// Unified Menu System - Central hub for all menu management
    /// Manages Main Menu, Pause Menu, Settings Menu with state machine
    /// </summary>
    public class UnifiedMenuSystem : MonoBehaviour
    {
        [Header("Menu Panels")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject pauseMenuPanel;
        [SerializeField] private GameObject settingsMenuPanel;
        [SerializeField] private GameObject creditsMenuPanel;
        [SerializeField] private GameObject multiplayerLobbyPanel;
        [SerializeField] private GameObject characterSelectPanel;

        [Header("UI Elements - Main Menu")]
        [SerializeField] private Button startGameButton;
        [SerializeField] private Button multiplayerButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button creditsButton;
        [SerializeField] private Button quitButton;

        [Header("UI Elements - Pause Menu")]
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button pauseSettingsButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button pauseQuitButton;

        [Header("Settings")]
        [SerializeField] private float transitionDuration = 0.3f;
        [SerializeField] private bool useAnimations = true;
        [SerializeField] private AudioClip menuOpenSound;
        [SerializeField] private AudioClip menuCloseSound;
        [SerializeField] private AudioClip buttonClickSound;

        // State Management
        public enum MenuState
        {
            MainMenu,
            PauseMenu,
            SettingsMenu,
            CreditsMenu,
            MultiplayerLobby,
            CharacterSelect,
            InGame,
            None
        }

        private MenuState currentState = MenuState.None;
        private MenuState previousState = MenuState.None;
        private Stack<MenuState> menuHistory = new Stack<MenuState>();

        // Events
        public event Action<MenuState> OnMenuStateChanged;
        public event Action OnGameStarted;
        public event Action OnReturnToMainMenu;

        private GUIManager guiManager;
        private bool isInitialized = false;

        void Awake()
        {
            guiManager = GetComponent<GUIManager>();
            if (guiManager == null)
            {
                Debug.LogError("[UnifiedMenuSystem] GUIManager component required!");
            }
        }

        void Start()
        {
            Initialize();
        }

        void Update()
        {
            // ESC key handling
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                HandleEscapeKey();
            }
        }

        /// <summary>
        /// Initialize the menu system
        /// </summary>
        public void Initialize()
        {
            if (isInitialized) return;

            // Setup button listeners
            SetupButtonListeners();

            // Determine starting state based on scene
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if (sceneName.Contains("MainMenu"))
            {
                ShowMainMenu();
            }
            else
            {
                currentState = MenuState.InGame;
                HideAllMenus();
            }

            isInitialized = true;
            Debug.Log("[UnifiedMenuSystem] Initialized in state: " + currentState);
        }

        /// <summary>
        /// Setup all button click listeners
        /// </summary>
        private void SetupButtonListeners()
        {
            // Main Menu buttons
            if (startGameButton != null)
                startGameButton.onClick.AddListener(OnStartGameClicked);
            if (multiplayerButton != null)
                multiplayerButton.onClick.AddListener(OnMultiplayerClicked);
            if (settingsButton != null)
                settingsButton.onClick.AddListener(OnSettingsClicked);
            if (creditsButton != null)
                creditsButton.onClick.AddListener(OnCreditsClicked);
            if (quitButton != null)
                quitButton.onClick.AddListener(OnQuitClicked);

            // Pause Menu buttons
            if (resumeButton != null)
                resumeButton.onClick.AddListener(OnResumeClicked);
            if (pauseSettingsButton != null)
                pauseSettingsButton.onClick.AddListener(OnSettingsClicked);
            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(OnMainMenuClicked);
            if (pauseQuitButton != null)
                pauseQuitButton.onClick.AddListener(OnQuitClicked);
        }

        #region State Management

        /// <summary>
        /// Change menu state with history tracking
        /// </summary>
        public void ChangeState(MenuState newState, bool addToHistory = true)
        {
            if (currentState == newState) return;

            if (addToHistory && currentState != MenuState.None)
            {
                menuHistory.Push(currentState);
            }

            previousState = currentState;
            currentState = newState;

            UpdateMenuVisibility();
            OnMenuStateChanged?.Invoke(currentState);

            Debug.Log($"[UnifiedMenuSystem] State changed: {previousState} -> {currentState}");
        }

        /// <summary>
        /// Go back to previous menu state
        /// </summary>
        public void GoBack()
        {
            if (menuHistory.Count > 0)
            {
                MenuState previousState = menuHistory.Pop();
                ChangeState(previousState, false);
            }
            else
            {
                Debug.LogWarning("[UnifiedMenuSystem] No menu history to go back to");
            }
        }

        /// <summary>
        /// Clear menu history
        /// </summary>
        public void ClearHistory()
        {
            menuHistory.Clear();
        }

        #endregion

        #region Menu Display Control

        /// <summary>
        /// Update visibility of all menus based on current state
        /// </summary>
        private void UpdateMenuVisibility()
        {
            // Hide all first
            HideAllMenus();

            // Show appropriate menu
            switch (currentState)
            {
                case MenuState.MainMenu:
                    ShowPanel(mainMenuPanel);
                    Time.timeScale = 1f;
                    break;

                case MenuState.PauseMenu:
                    ShowPanel(pauseMenuPanel);
                    Time.timeScale = 0f;
                    break;

                case MenuState.SettingsMenu:
                    ShowPanel(settingsMenuPanel);
                    break;

                case MenuState.CreditsMenu:
                    ShowPanel(creditsMenuPanel);
                    break;

                case MenuState.MultiplayerLobby:
                    ShowPanel(multiplayerLobbyPanel);
                    break;

                case MenuState.CharacterSelect:
                    ShowPanel(characterSelectPanel);
                    break;

                case MenuState.InGame:
                    Time.timeScale = 1f;
                    break;
            }
        }

        /// <summary>
        /// Hide all menu panels
        /// </summary>
        private void HideAllMenus()
        {
            HidePanel(mainMenuPanel);
            HidePanel(pauseMenuPanel);
            HidePanel(settingsMenuPanel);
            HidePanel(creditsMenuPanel);
            HidePanel(multiplayerLobbyPanel);
            HidePanel(characterSelectPanel);
        }

        /// <summary>
        /// Show a panel with optional animation
        /// </summary>
        private void ShowPanel(GameObject panel)
        {
            if (panel == null) return;

            panel.SetActive(true);

            if (useAnimations && guiManager != null)
            {
                guiManager.ShowPanel(panel, transitionDuration);
            }

            PlaySound(menuOpenSound);
        }

        /// <summary>
        /// Hide a panel with optional animation
        /// </summary>
        private void HidePanel(GameObject panel)
        {
            if (panel == null) return;

            if (useAnimations && guiManager != null)
            {
                guiManager.HidePanel(panel, transitionDuration, () =>
                {
                    panel.SetActive(false);
                });
            }
            else
            {
                panel.SetActive(false);
            }

            PlaySound(menuCloseSound);
        }

        #endregion

        #region Public Menu Methods

        /// <summary>
        /// Show main menu
        /// </summary>
        public void ShowMainMenu()
        {
            ClearHistory();
            ChangeState(MenuState.MainMenu, false);
        }

        /// <summary>
        /// Show pause menu (from in-game)
        /// </summary>
        public void ShowPauseMenu()
        {
            if (currentState == MenuState.InGame)
            {
                ChangeState(MenuState.PauseMenu);
            }
        }

        /// <summary>
        /// Hide pause menu and resume game
        /// </summary>
        public void HidePauseMenu()
        {
            if (currentState == MenuState.PauseMenu)
            {
                ChangeState(MenuState.InGame, false);
                ClearHistory();
            }
        }

        /// <summary>
        /// Toggle pause menu
        /// </summary>
        public void TogglePauseMenu()
        {
            if (currentState == MenuState.InGame)
            {
                ShowPauseMenu();
            }
            else if (currentState == MenuState.PauseMenu)
            {
                HidePauseMenu();
            }
        }

        /// <summary>
        /// Show settings menu
        /// </summary>
        public void ShowSettings()
        {
            ChangeState(MenuState.SettingsMenu);
        }

        /// <summary>
        /// Show credits menu
        /// </summary>
        public void ShowCredits()
        {
            ChangeState(MenuState.CreditsMenu);
        }

        /// <summary>
        /// Show multiplayer lobby
        /// </summary>
        public void ShowMultiplayerLobby()
        {
            ChangeState(MenuState.MultiplayerLobby);
        }

        /// <summary>
        /// Show character selection
        /// </summary>
        public void ShowCharacterSelect()
        {
            ChangeState(MenuState.CharacterSelect);
        }

        #endregion

        #region Button Click Handlers

        private void OnStartGameClicked()
        {
            PlaySound(buttonClickSound);
            Debug.Log("[UnifiedMenuSystem] Start Game clicked");

            // Show character select or load game directly
            if (characterSelectPanel != null)
            {
                ShowCharacterSelect();
            }
            else
            {
                StartGame();
            }
        }

        private void OnMultiplayerClicked()
        {
            PlaySound(buttonClickSound);
            Debug.Log("[UnifiedMenuSystem] Multiplayer clicked");
            ShowMultiplayerLobby();
        }

        private void OnSettingsClicked()
        {
            PlaySound(buttonClickSound);
            Debug.Log("[UnifiedMenuSystem] Settings clicked");
            ShowSettings();
        }

        private void OnCreditsClicked()
        {
            PlaySound(buttonClickSound);
            Debug.Log("[UnifiedMenuSystem] Credits clicked");
            ShowCredits();
        }

        private void OnQuitClicked()
        {
            PlaySound(buttonClickSound);
            Debug.Log("[UnifiedMenuSystem] Quit clicked");

            if (guiManager != null)
            {
                guiManager.ShowConfirmDialog(
                    "Quit Game",
                    "Are you sure you want to quit?",
                    () => QuitGame(),
                    null
                );
            }
            else
            {
                QuitGame();
            }
        }

        private void OnResumeClicked()
        {
            PlaySound(buttonClickSound);
            Debug.Log("[UnifiedMenuSystem] Resume clicked");
            HidePauseMenu();
        }

        private void OnMainMenuClicked()
        {
            PlaySound(buttonClickSound);
            Debug.Log("[UnifiedMenuSystem] Main Menu clicked");

            if (guiManager != null)
            {
                guiManager.ShowConfirmDialog(
                    "Return to Main Menu",
                    "Unsaved progress will be lost. Continue?",
                    () => ReturnToMainMenu(),
                    null
                );
            }
            else
            {
                ReturnToMainMenu();
            }
        }

        #endregion

        #region Game Flow Methods

        /// <summary>
        /// Start the game
        /// </summary>
        public void StartGame()
        {
            Debug.Log("[UnifiedMenuSystem] Starting game...");

            ChangeState(MenuState.InGame, false);
            ClearHistory();

            OnGameStarted?.Invoke();

            // Load game scene
            LoadGameScene();
        }

        /// <summary>
        /// Return to main menu from game
        /// </summary>
        public void ReturnToMainMenu()
        {
            Debug.Log("[UnifiedMenuSystem] Returning to main menu...");

            Time.timeScale = 1f;
            OnReturnToMainMenu?.Invoke();

            // Load main menu scene
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }

        /// <summary>
        /// Quit the game
        /// </summary>
        public void QuitGame()
        {
            Debug.Log("[UnifiedMenuSystem] Quitting game...");

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        /// <summary>
        /// Load game scene
        /// </summary>
        private void LoadGameScene()
        {
            string gameSceneName = "GameScene";

            if (UnityEngine.SceneManagement.SceneManager.GetSceneByName(gameSceneName).isLoaded)
            {
                Debug.LogWarning("[UnifiedMenuSystem] GameScene already loaded");
                return;
            }

            Debug.Log($"[UnifiedMenuSystem] Loading scene: {gameSceneName}");
            UnityEngine.SceneManagement.SceneManager.LoadScene(gameSceneName);
        }

        #endregion

        #region Input Handling

        /// <summary>
        /// Handle ESC key press
        /// </summary>
        private void HandleEscapeKey()
        {
            switch (currentState)
            {
                case MenuState.InGame:
                    ShowPauseMenu();
                    break;

                case MenuState.PauseMenu:
                    HidePauseMenu();
                    break;

                case MenuState.SettingsMenu:
                case MenuState.CreditsMenu:
                case MenuState.MultiplayerLobby:
                case MenuState.CharacterSelect:
                    GoBack();
                    break;

                case MenuState.MainMenu:
                    // ESC on main menu could show quit confirmation
                    OnQuitClicked();
                    break;
            }
        }

        #endregion

        #region Audio

        /// <summary>
        /// Play a UI sound effect
        /// </summary>
        private void PlaySound(AudioClip clip)
        {
            if (clip == null) return;

            // Use AdvancedAudioSystem if available
            var audioSystem = FindObjectOfType<Audio.AdvancedAudioSystem>();
            if (audioSystem != null)
            {
                audioSystem.PlaySound2D(clip.name, 1.0f, 1.0f);
            }
            else
            {
                // Fallback to simple AudioSource
                AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, 0.5f);
            }
        }

        #endregion

        #region Getters

        public MenuState CurrentState => currentState;
        public MenuState PreviousState => previousState;
        public bool IsInGame => currentState == MenuState.InGame;
        public bool IsPaused => currentState == MenuState.PauseMenu;

        #endregion
    }
}
