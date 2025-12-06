using UnityEngine;
using UnityEngine.UI;
using EarthUnderFreelancer.Core;

namespace EarthUnderFreelancer.UI
{
    /// <summary>
    /// Pause menu with game options
    /// </summary>
    public class PauseMenuUI : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject confirmPanel;
        [SerializeField] private GameObject optionsPanel;

        [Header("Buttons")]
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button loadButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button quitButton;

        [Header("Confirm Dialog")]
        [SerializeField] private Text confirmText;
        [SerializeField] private Button confirmYesButton;
        [SerializeField] private Button confirmNoButton;

        [Header("Settings")]
        [SerializeField] private KeyCode pauseKey = KeyCode.Escape;

        private System.Action pendingAction;
        private bool isPaused = false;

        public bool IsPaused => isPaused;

        public event System.Action OnGamePaused;
        public event System.Action OnGameResumed;

        private void Start()
        {
            InitializeButtons();
            HideAllPanels();
        }

        private void InitializeButtons()
        {
            if (resumeButton != null)
                resumeButton.onClick.AddListener(Resume);
            
            if (optionsButton != null)
                optionsButton.onClick.AddListener(ShowOptions);
            
            if (saveButton != null)
                saveButton.onClick.AddListener(SaveGame);
            
            if (loadButton != null)
                loadButton.onClick.AddListener(LoadGame);
            
            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(() => ShowConfirm("Return to Main Menu?", GoToMainMenu));
            
            if (quitButton != null)
                quitButton.onClick.AddListener(() => ShowConfirm("Quit to Desktop?", QuitGame));

            if (confirmYesButton != null)
                confirmYesButton.onClick.AddListener(ConfirmAction);
            
            if (confirmNoButton != null)
                confirmNoButton.onClick.AddListener(CancelConfirm);
        }

        private void Update()
        {
            if (Input.GetKeyDown(pauseKey))
            {
                if (isPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
        }

        public void Pause()
        {
            isPaused = true;
            Time.timeScale = 0f;
            
            if (pausePanel != null)
                pausePanel.SetActive(true);
            
            // Show cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            OnGamePaused?.Invoke();

            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetGameState(GameManager.GameState.Paused);
            }
        }

        public void Resume()
        {
            isPaused = false;
            Time.timeScale = 1f;
            
            HideAllPanels();
            
            // Hide cursor for gameplay
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            OnGameResumed?.Invoke();

            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetGameState(GameManager.GameState.Playing);
            }
        }

        private void ShowOptions()
        {
            if (pausePanel != null)
                pausePanel.SetActive(false);
            if (optionsPanel != null)
                optionsPanel.SetActive(true);
        }

        public void HideOptions()
        {
            if (optionsPanel != null)
                optionsPanel.SetActive(false);
            if (pausePanel != null)
                pausePanel.SetActive(true);
        }

        private void SaveGame()
        {
            if (SaveLoadManager.Instance != null)
            {
                SaveLoadManager.Instance.SaveGame("quicksave");
                ShowNotification("Game Saved!");
            }
        }

        private void LoadGame()
        {
            ShowConfirm("Load last save? Unsaved progress will be lost.", () =>
            {
                if (SaveLoadManager.Instance != null)
                {
                    SaveLoadManager.Instance.LoadGame("quicksave");
                    Resume();
                }
            });
        }

        private void ShowConfirm(string message, System.Action action)
        {
            pendingAction = action;
            
            if (confirmText != null)
                confirmText.text = message;
            
            if (pausePanel != null)
                pausePanel.SetActive(false);
            if (confirmPanel != null)
                confirmPanel.SetActive(true);
        }

        private void ConfirmAction()
        {
            if (confirmPanel != null)
                confirmPanel.SetActive(false);
            
            pendingAction?.Invoke();
            pendingAction = null;
        }

        private void CancelConfirm()
        {
            if (confirmPanel != null)
                confirmPanel.SetActive(false);
            if (pausePanel != null)
                pausePanel.SetActive(true);
            
            pendingAction = null;
        }

        private void GoToMainMenu()
        {
            Time.timeScale = 1f;
            isPaused = false;
            
            if (SceneLoader.Instance != null)
            {
                SceneLoader.Instance.LoadScene("MainMenu");
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
            }
        }

        private void QuitGame()
        {
            // Save before quitting
            if (SaveLoadManager.Instance != null)
            {
                SaveLoadManager.Instance.SaveGame("autosave");
            }

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void HideAllPanels()
        {
            if (pausePanel != null)
                pausePanel.SetActive(false);
            if (confirmPanel != null)
                confirmPanel.SetActive(false);
            if (optionsPanel != null)
                optionsPanel.SetActive(false);
        }

        private void ShowNotification(string message)
        {
            // Could integrate with UIManager for notifications
            Debug.Log($"[Pause Menu] {message}");
        }

        private void OnDestroy()
        {
            // Ensure time scale is reset
            Time.timeScale = 1f;
        }
    }
}
