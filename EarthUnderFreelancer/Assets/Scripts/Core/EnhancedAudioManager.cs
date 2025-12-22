using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Core
{
    /// <summary>
    /// Enhanced audio system with 3D positional audio, music layers, and ambience
    /// </summary>
    public class EnhancedAudioManager : MonoBehaviour
    {
        public static EnhancedAudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource ambienceSource;
        [SerializeField] private AudioSource uiSource;
        [SerializeField] private int sfxPoolSize = 20;

        [Header("Volume Settings")]
        [SerializeField] [Range(0f, 1f)] private float masterVolume = 1f;
        [SerializeField] [Range(0f, 1f)] private float musicVolume = 0.7f;
        [SerializeField] [Range(0f, 1f)] private float sfxVolume = 1f;
        [SerializeField] [Range(0f, 1f)] private float ambienceVolume = 0.5f;
        [SerializeField] [Range(0f, 1f)] private float uiVolume = 0.8f;

        [Header("Music Tracks")]
        [SerializeField] private AudioClip menuMusic;
        [SerializeField] private AudioClip explorationMusic;
        [SerializeField] private AudioClip combatMusic;
        [SerializeField] private AudioClip stationMusic;
        [SerializeField] private AudioClip victoryMusic;
        [SerializeField] private AudioClip defeatMusic;

        [Header("Ambience")]
        [SerializeField] private AudioClip spaceAmbience;
        [SerializeField] private AudioClip nebulaAmbience;
        [SerializeField] private AudioClip stationAmbience;
        [SerializeField] private AudioClip combatAmbience;

        [Header("Sound Effects Library")]
        [SerializeField] private SoundLibrary soundLibrary;

        [Header("Fade Settings")]
        [SerializeField] private float musicFadeDuration = 2f;
        [SerializeField] private float ambienceFadeDuration = 1f;

        private List<AudioSource> sfxPool = new List<AudioSource>();
        private MusicState currentMusicState = MusicState.None;
        private Coroutine musicFadeCoroutine;
        private Coroutine ambienceFadeCoroutine;

        public float MasterVolume
        {
            get => masterVolume;
            set
            {
                masterVolume = Mathf.Clamp01(value);
                UpdateAllVolumes();
                PlayerPrefs.SetFloat("MasterVolume", masterVolume);
            }
        }

        public float MusicVolume
        {
            get => musicVolume;
            set
            {
                musicVolume = Mathf.Clamp01(value);
                UpdateMusicVolume();
                PlayerPrefs.SetFloat("MusicVolume", musicVolume);
            }
        }

        public float SFXVolume
        {
            get => sfxVolume;
            set
            {
                sfxVolume = Mathf.Clamp01(value);
                PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
            }
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeAudioSources();
                LoadVolumeSettings();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeAudioSources()
        {
            // Create music source if not assigned
            if (musicSource == null)
            {
                GameObject musicObj = new GameObject("MusicSource");
                musicObj.transform.SetParent(transform);
                musicSource = musicObj.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
            }

            // Create ambience source if not assigned
            if (ambienceSource == null)
            {
                GameObject ambienceObj = new GameObject("AmbienceSource");
                ambienceObj.transform.SetParent(transform);
                ambienceSource = ambienceObj.AddComponent<AudioSource>();
                ambienceSource.loop = true;
                ambienceSource.playOnAwake = false;
            }

            // Create UI source if not assigned
            if (uiSource == null)
            {
                GameObject uiObj = new GameObject("UISource");
                uiObj.transform.SetParent(transform);
                uiSource = uiObj.AddComponent<AudioSource>();
                uiSource.playOnAwake = false;
            }

            // Create SFX pool
            GameObject poolContainer = new GameObject("SFXPool");
            poolContainer.transform.SetParent(transform);

            for (int i = 0; i < sfxPoolSize; i++)
            {
                GameObject sfxObj = new GameObject($"SFXSource_{i}");
                sfxObj.transform.SetParent(poolContainer.transform);
                AudioSource source = sfxObj.AddComponent<AudioSource>();
                source.playOnAwake = false;
                source.spatialBlend = 1f; // 3D sound
                sfxPool.Add(source);
            }
        }

        private void LoadVolumeSettings()
        {
            masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
            sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            ambienceVolume = PlayerPrefs.GetFloat("AmbienceVolume", 0.5f);
            uiVolume = PlayerPrefs.GetFloat("UIVolume", 0.8f);
            
            UpdateAllVolumes();
        }

        private void UpdateAllVolumes()
        {
            UpdateMusicVolume();
            UpdateAmbienceVolume();
        }

        private void UpdateMusicVolume()
        {
            if (musicSource != null)
            {
                musicSource.volume = musicVolume * masterVolume;
            }
        }

        private void UpdateAmbienceVolume()
        {
            if (ambienceSource != null)
            {
                ambienceSource.volume = ambienceVolume * masterVolume;
            }
        }

        #region Music

        public void PlayMusic(MusicState state, bool instant = false)
        {
            if (currentMusicState == state) return;
            currentMusicState = state;

            AudioClip clip = GetMusicClip(state);
            if (clip == null) return;

            if (instant)
            {
                musicSource.clip = clip;
                musicSource.volume = musicVolume * masterVolume;
                musicSource.Play();
            }
            else
            {
                if (musicFadeCoroutine != null)
                {
                    StopCoroutine(musicFadeCoroutine);
                }
                musicFadeCoroutine = StartCoroutine(CrossfadeMusic(clip));
            }
        }

        private AudioClip GetMusicClip(MusicState state)
        {
            switch (state)
            {
                case MusicState.Menu: return menuMusic;
                case MusicState.Exploration: return explorationMusic;
                case MusicState.Combat: return combatMusic;
                case MusicState.Station: return stationMusic;
                case MusicState.Victory: return victoryMusic;
                case MusicState.Defeat: return defeatMusic;
                default: return null;
            }
        }

        private System.Collections.IEnumerator CrossfadeMusic(AudioClip newClip)
        {
            float startVolume = musicSource.volume;
            float targetVolume = musicVolume * masterVolume;

            // Fade out
            float elapsed = 0f;
            while (elapsed < musicFadeDuration / 2)
            {
                elapsed += Time.unscaledDeltaTime;
                musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / (musicFadeDuration / 2));
                yield return null;
            }

            // Switch track
            musicSource.clip = newClip;
            musicSource.Play();

            // Fade in
            elapsed = 0f;
            while (elapsed < musicFadeDuration / 2)
            {
                elapsed += Time.unscaledDeltaTime;
                musicSource.volume = Mathf.Lerp(0f, targetVolume, elapsed / (musicFadeDuration / 2));
                yield return null;
            }

            musicSource.volume = targetVolume;
        }

        public void StopMusic(bool instant = false)
        {
            if (instant)
            {
                musicSource.Stop();
            }
            else
            {
                StartCoroutine(FadeOutMusic());
            }
            currentMusicState = MusicState.None;
        }

        private System.Collections.IEnumerator FadeOutMusic()
        {
            float startVolume = musicSource.volume;
            float elapsed = 0f;

            while (elapsed < musicFadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / musicFadeDuration);
                yield return null;
            }

            musicSource.Stop();
        }

        #endregion

        #region Ambience

        public void PlayAmbience(AmbienceType type)
        {
            AudioClip clip = GetAmbienceClip(type);
            if (clip == null) return;

            if (ambienceFadeCoroutine != null)
            {
                StopCoroutine(ambienceFadeCoroutine);
            }
            ambienceFadeCoroutine = StartCoroutine(CrossfadeAmbience(clip));
        }

        private AudioClip GetAmbienceClip(AmbienceType type)
        {
            switch (type)
            {
                case AmbienceType.Space: return spaceAmbience;
                case AmbienceType.Nebula: return nebulaAmbience;
                case AmbienceType.Station: return stationAmbience;
                case AmbienceType.Combat: return combatAmbience;
                default: return null;
            }
        }

        private System.Collections.IEnumerator CrossfadeAmbience(AudioClip newClip)
        {
            float startVolume = ambienceSource.volume;
            float targetVolume = ambienceVolume * masterVolume;

            float elapsed = 0f;
            while (elapsed < ambienceFadeDuration / 2)
            {
                elapsed += Time.unscaledDeltaTime;
                ambienceSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / (ambienceFadeDuration / 2));
                yield return null;
            }

            ambienceSource.clip = newClip;
            ambienceSource.Play();

            elapsed = 0f;
            while (elapsed < ambienceFadeDuration / 2)
            {
                elapsed += Time.unscaledDeltaTime;
                ambienceSource.volume = Mathf.Lerp(0f, targetVolume, elapsed / (ambienceFadeDuration / 2));
                yield return null;
            }

            ambienceSource.volume = targetVolume;
        }

        #endregion

        #region Sound Effects

        public void PlaySFX(string soundId, Vector3? position = null, float pitchVariation = 0f)
        {
            AudioClip clip = GetSFXClip(soundId);
            if (clip == null) return;

            AudioSource source = GetAvailableSFXSource();
            if (source == null) return;

            source.clip = clip;
            source.volume = sfxVolume * masterVolume;
            source.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);

            if (position.HasValue)
            {
                source.transform.position = position.Value;
                source.spatialBlend = 1f;
            }
            else
            {
                source.spatialBlend = 0f;
            }

            source.Play();
        }

        public void PlaySFX(AudioClip clip, Vector3? position = null, float volume = 1f)
        {
            if (clip == null) return;

            AudioSource source = GetAvailableSFXSource();
            if (source == null) return;

            source.clip = clip;
            source.volume = volume * sfxVolume * masterVolume;

            if (position.HasValue)
            {
                source.transform.position = position.Value;
                source.spatialBlend = 1f;
            }
            else
            {
                source.spatialBlend = 0f;
            }

            source.Play();
        }

        private AudioClip GetSFXClip(string soundId)
        {
            if (soundLibrary == null) return null;

            foreach (var entry in soundLibrary.sounds)
            {
                if (entry.soundId == soundId)
                    return entry.clip;
            }
            return null;
        }

        private AudioSource GetAvailableSFXSource()
        {
            foreach (var source in sfxPool)
            {
                if (!source.isPlaying)
                    return source;
            }

            // All sources are busy, return the first one (it will interrupt)
            return sfxPool.Count > 0 ? sfxPool[0] : null;
        }

        #endregion

        #region UI Sounds

        public void PlayUISound(string soundId)
        {
            AudioClip clip = GetSFXClip(soundId);
            if (clip == null) return;

            uiSource.PlayOneShot(clip, uiVolume * masterVolume);
        }

        public void PlayButtonClick()
        {
            PlayUISound("ui_click");
        }

        public void PlayButtonHover()
        {
            PlayUISound("ui_hover");
        }

        #endregion

        public void SaveSettings()
        {
            PlayerPrefs.SetFloat("MasterVolume", masterVolume);
            PlayerPrefs.SetFloat("MusicVolume", musicVolume);
            PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
            PlayerPrefs.SetFloat("AmbienceVolume", ambienceVolume);
            PlayerPrefs.SetFloat("UIVolume", uiVolume);
            PlayerPrefs.Save();
        }
    }

    [System.Serializable]
    public class SoundLibrary
    {
        public List<SoundEntry> sounds = new List<SoundEntry>();
    }

    [System.Serializable]
    public class SoundEntry
    {
        public string soundId;
        public AudioClip clip;
        public float defaultVolume = 1f;
    }

    public enum MusicState
    {
        None,
        Menu,
        Exploration,
        Combat,
        Station,
        Victory,
        Defeat,
        Boss
    }

    public enum AmbienceType
    {
        None,
        Space,
        Nebula,
        Station,
        Combat,
        AsteroidField
    }
}
