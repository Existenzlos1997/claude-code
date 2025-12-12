using UnityEngine;
using UnityEngine.UI;
using EarthUnderFreelancer.Core;

namespace EarthUnderFreelancer.UI
{
    /// <summary>
    /// Settings menu UI controller
    /// </summary>
    public class SettingsUI : MonoBehaviour
    {
        [Header("Audio")]
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Slider voiceVolumeSlider;

        [Header("Graphics")]
        [SerializeField] private Dropdown qualityDropdown;
        [SerializeField] private Dropdown resolutionDropdown;
        [SerializeField] private Toggle fullscreenToggle;
        [SerializeField] private Toggle vSyncToggle;
        [SerializeField] private Toggle postProcessToggle;
        [SerializeField] private Toggle shadowsToggle;

        [Header("Controls")]
        [SerializeField] private Slider mouseSensitivitySlider;
        [SerializeField] private Toggle invertYToggle;
        [SerializeField] private Toggle invertXToggle;
        [SerializeField] private Toggle vibrationToggle;

        [Header("Mobile Controls")]
        [SerializeField] private Slider touchSensitivitySlider;
        [SerializeField] private Toggle gyroscopeToggle;
        [SerializeField] private Toggle autoFireToggle;
        [SerializeField] private Dropdown controlSchemeDropdown;

        [Header("Gameplay")]
        [SerializeField] private Toggle tutorialsToggle;
        [SerializeField] private Toggle damageNumbersToggle;
        [SerializeField] private Toggle minimapToggle;
        [SerializeField] private Toggle autoAimToggle;
        [SerializeField] private Toggle fpsToggle;

        [Header("Buttons")]
        [SerializeField] private Button applyButton;
        [SerializeField] private Button resetButton;
        [SerializeField] private Button backButton;

        private GameSettings settings;

        private void Start()
        {
            settings = GameManager.Instance?.gameSettings ?? new GameSettings();
            LoadSettings();
            SetupListeners();
        }

        private void SetupListeners()
        {
            applyButton?.onClick.AddListener(ApplySettings);
            resetButton?.onClick.AddListener(ResetSettings);
            backButton?.onClick.AddListener(OnBack);

            // Audio sliders
            masterVolumeSlider?.onValueChanged.AddListener(v => { settings.masterVolume = v; AudioManager.Instance?.SetMusicVolume(v); });
            musicVolumeSlider?.onValueChanged.AddListener(v => settings.musicVolume = v);
            sfxVolumeSlider?.onValueChanged.AddListener(v => settings.sfxVolume = v);

            // Graphics
            qualityDropdown?.onValueChanged.AddListener(v => settings.qualityLevel = v);
            fullscreenToggle?.onValueChanged.AddListener(v => settings.fullscreen = v);
            vSyncToggle?.onValueChanged.AddListener(v => settings.vSync = v);

            // Controls
            mouseSensitivitySlider?.onValueChanged.AddListener(v => settings.mouseSensitivity = v);
            invertYToggle?.onValueChanged.AddListener(v => settings.invertYAxis = v);
            invertXToggle?.onValueChanged.AddListener(v => settings.invertXAxis = v);

            // Gameplay
            tutorialsToggle?.onValueChanged.AddListener(v => settings.showTutorials = v);
            damageNumbersToggle?.onValueChanged.AddListener(v => settings.showDamageNumbers = v);
            minimapToggle?.onValueChanged.AddListener(v => settings.showMinimap = v);
            autoAimToggle?.onValueChanged.AddListener(v => settings.autoAim = v);
        }

        private void LoadSettings()
        {
            if (masterVolumeSlider != null) masterVolumeSlider.value = settings.masterVolume;
            if (musicVolumeSlider != null) musicVolumeSlider.value = settings.musicVolume;
            if (sfxVolumeSlider != null) sfxVolumeSlider.value = settings.sfxVolume;
            if (voiceVolumeSlider != null) voiceVolumeSlider.value = settings.voiceVolume;

            if (qualityDropdown != null) qualityDropdown.value = settings.qualityLevel;
            if (fullscreenToggle != null) fullscreenToggle.isOn = settings.fullscreen;
            if (vSyncToggle != null) vSyncToggle.isOn = settings.vSync;
            if (postProcessToggle != null) postProcessToggle.isOn = settings.postProcessing;
            if (shadowsToggle != null) shadowsToggle.isOn = settings.shadows;

            if (mouseSensitivitySlider != null) mouseSensitivitySlider.value = settings.mouseSensitivity;
            if (invertYToggle != null) invertYToggle.isOn = settings.invertYAxis;
            if (invertXToggle != null) invertXToggle.isOn = settings.invertXAxis;
            if (vibrationToggle != null) vibrationToggle.isOn = settings.vibration;

            if (touchSensitivitySlider != null) touchSensitivitySlider.value = settings.touchSensitivity;
            if (gyroscopeToggle != null) gyroscopeToggle.isOn = settings.useGyroscope;
            if (autoFireToggle != null) autoFireToggle.isOn = settings.autoFire;

            if (tutorialsToggle != null) tutorialsToggle.isOn = settings.showTutorials;
            if (damageNumbersToggle != null) damageNumbersToggle.isOn = settings.showDamageNumbers;
            if (minimapToggle != null) minimapToggle.isOn = settings.showMinimap;
            if (autoAimToggle != null) autoAimToggle.isOn = settings.autoAim;
            if (fpsToggle != null) fpsToggle.isOn = settings.showFPS;
        }

        private void ApplySettings()
        {
            AudioManager.Instance?.PlayButtonClick();
            
            settings.ApplyGraphicsSettings();
            settings.ApplyAudioSettings();
            
            if (GameManager.Instance != null)
            {
                GameManager.Instance.gameSettings = settings;
                GameManager.Instance.SaveGame();
            }
        }

        private void ResetSettings()
        {
            AudioManager.Instance?.PlayButtonClick();
            settings.ResetToDefaults();
            LoadSettings();
            ApplySettings();
        }

        private void OnBack()
        {
            AudioManager.Instance?.PlayButtonClick();
            ApplySettings();
        }
    }
}
