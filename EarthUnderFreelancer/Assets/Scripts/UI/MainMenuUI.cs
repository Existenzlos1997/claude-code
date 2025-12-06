using UnityEngine;
using UnityEngine.UI;
using EarthUnderFreelancer.Core;

namespace EarthUnderFreelancer.UI
{
    /// <summary>
    /// Main menu controller
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject playPanel;
        [SerializeField] private GameObject hangarPanel;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject shopPanel;
        [SerializeField] private GameObject profilePanel;

        [Header("Main Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button hangarButton;
        [SerializeField] private Button shopButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button profileButton;
        [SerializeField] private Button quitButton;

        [Header("Player Info")]
        [SerializeField] private Text playerNameText;
        [SerializeField] private Text playerLevelText;
        [SerializeField] private Text creditsText;
        [SerializeField] private Slider experienceBar;

        [Header("Version")]
        [SerializeField] private Text versionText;

        private void Start()
        {
            SetupButtons();
            UpdatePlayerInfo();
            ShowMainPanel();
            
            AudioManager.Instance?.PlayMenuMusic();
            
            if (versionText != null)
                versionText.text = $"v{Application.version}";
        }

        private void SetupButtons()
        {
            playButton?.onClick.AddListener(OnPlayClicked);
            hangarButton?.onClick.AddListener(OnHangarClicked);
            shopButton?.onClick.AddListener(OnShopClicked);
            settingsButton?.onClick.AddListener(OnSettingsClicked);
            profileButton?.onClick.AddListener(OnProfileClicked);
            quitButton?.onClick.AddListener(OnQuitClicked);
        }

        public void UpdatePlayerInfo()
        {
            PlayerData data = GameManager.Instance?.playerData;
            if (data == null) return;

            if (playerNameText != null)
                playerNameText.text = data.playerName;
            if (playerLevelText != null)
                playerLevelText.text = $"Level {data.playerLevel}";
            if (creditsText != null)
                creditsText.text = $"{data.credits:N0} Credits";
            if (experienceBar != null)
                experienceBar.value = (float)data.experiencePoints / data.GetExperienceForNextLevel();
        }

        private void ShowMainPanel()
        {
            HideAllPanels();
            mainPanel?.SetActive(true);
        }

        private void HideAllPanels()
        {
            mainPanel?.SetActive(false);
            playPanel?.SetActive(false);
            hangarPanel?.SetActive(false);
            settingsPanel?.SetActive(false);
            shopPanel?.SetActive(false);
            profilePanel?.SetActive(false);
        }

        private void OnPlayClicked()
        {
            AudioManager.Instance?.PlayButtonClick();
            HideAllPanels();
            playPanel?.SetActive(true);
        }

        private void OnHangarClicked()
        {
            AudioManager.Instance?.PlayButtonClick();
            HideAllPanels();
            hangarPanel?.SetActive(true);
        }

        private void OnShopClicked()
        {
            AudioManager.Instance?.PlayButtonClick();
            HideAllPanels();
            shopPanel?.SetActive(true);
        }

        private void OnSettingsClicked()
        {
            AudioManager.Instance?.PlayButtonClick();
            HideAllPanels();
            settingsPanel?.SetActive(true);
        }

        private void OnProfileClicked()
        {
            AudioManager.Instance?.PlayButtonClick();
            HideAllPanels();
            profilePanel?.SetActive(true);
        }

        private void OnQuitClicked()
        {
            AudioManager.Instance?.PlayButtonClick();
            GameManager.Instance?.QuitGame();
        }

        public void OnBackClicked()
        {
            AudioManager.Instance?.PlayButtonClick();
            ShowMainPanel();
        }

        public void StartSinglePlayer()
        {
            AudioManager.Instance?.PlayButtonClick();
            SceneLoader.Instance?.LoadMission("patrol_01");
        }

        public void StartMultiplayer()
        {
            AudioManager.Instance?.PlayButtonClick();
            SceneLoader.Instance?.LoadLobby();
        }
    }
}
