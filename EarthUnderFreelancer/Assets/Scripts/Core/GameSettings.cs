using UnityEngine;
using System;

namespace EarthUnderFreelancer.Core
{
    /// <summary>
    /// Game settings class for storing all user preferences
    /// </summary>
    [Serializable]
    public class GameSettings
    {
        [Range(0f, 1f)]
        public float masterVolume = 1f;
        [Range(0f, 1f)]
        public float musicVolume = 0.7f;
        [Range(0f, 1f)]
        public float sfxVolume = 1f;
        [Range(0f, 1f)]
        public float voiceVolume = 1f;

        public int qualityLevel = 2;
        public bool fullscreen = true;
        public int resolutionIndex = 0;
        public bool vSync = true;
        public int targetFrameRate = 60;
        public bool postProcessing = true;
        public bool shadows = true;
        public bool antiAliasing = true;

        public float mouseSensitivity = 1f;
        public float gamepadSensitivity = 1f;
        public bool invertYAxis = false;
        public bool invertXAxis = false;
        public bool vibration = true;

        public float touchSensitivity = 1f;
        public bool useGyroscope = false;
        public bool autoFire = false;
        public float joystickSize = 1f;
        public int controlScheme = 0;

        public bool showTutorials = true;
        public bool showDamageNumbers = true;
        public bool showMinimap = true;
        public bool autoAim = false;
        public float autoAimStrength = 0.5f;
        public bool showFPS = false;
        public bool showPing = true;

        public bool colorBlindMode = false;
        public int colorBlindType = 0;
        public float uiScale = 1f;
        public bool screenShake = true;
        public float screenShakeIntensity = 1f;
        public bool subtitles = true;

        public string preferredRegion = "auto";
        public bool showPlayerNames = true;
        public bool allowVoiceChat = true;
        public bool pushToTalk = true;

        public bool allowAnalytics = true;
        public bool personalizedAds = true;

        public void ApplyGraphicsSettings()
        {
            QualitySettings.SetQualityLevel(qualityLevel);
            Screen.fullScreen = fullscreen;
            QualitySettings.vSyncCount = vSync ? 1 : 0;
            Application.targetFrameRate = vSync ? -1 : targetFrameRate;
        }

        public void ApplyAudioSettings()
        {
            AudioListener.volume = masterVolume;
        }

        public void ResetToDefaults()
        {
            masterVolume = 1f;
            musicVolume = 0.7f;
            sfxVolume = 1f;
            voiceVolume = 1f;
            qualityLevel = 2;
            fullscreen = true;
            vSync = true;
            targetFrameRate = 60;
            postProcessing = true;
            shadows = true;
            antiAliasing = true;
            mouseSensitivity = 1f;
            gamepadSensitivity = 1f;
            invertYAxis = false;
            invertXAxis = false;
            vibration = true;
            touchSensitivity = 1f;
            useGyroscope = false;
            autoFire = false;
            joystickSize = 1f;
            controlScheme = 0;
            showTutorials = true;
            showDamageNumbers = true;
            showMinimap = true;
            autoAim = false;
            autoAimStrength = 0.5f;
            showFPS = false;
            showPing = true;
            colorBlindMode = false;
            colorBlindType = 0;
            uiScale = 1f;
            screenShake = true;
            screenShakeIntensity = 1f;
            subtitles = true;
            preferredRegion = "auto";
            showPlayerNames = true;
            allowVoiceChat = true;
            pushToTalk = true;
            allowAnalytics = true;
            personalizedAds = true;
        }
    }
}
