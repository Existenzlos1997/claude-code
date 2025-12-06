using UnityEngine;
using System;

namespace EarthUnderFreelancer.Core
{
    /// <summary>
    /// Handles all audio playback in the game
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource ambientSource;
        [SerializeField] private AudioSource voiceSource;

        [Header("Music Tracks")]
        [SerializeField] private AudioClip menuMusic;
        [SerializeField] private AudioClip combatMusic;
        [SerializeField] private AudioClip ambientSpace;
        [SerializeField] private AudioClip victoryMusic;
        [SerializeField] private AudioClip defeatMusic;

        [Header("UI Sound Effects")]
        [SerializeField] private AudioClip buttonClick;
        [SerializeField] private AudioClip buttonHover;
        [SerializeField] private AudioClip purchaseSuccess;
        [SerializeField] private AudioClip purchaseFail;
        [SerializeField] private AudioClip notification;

        [Header("Combat Sound Effects")]
        [SerializeField] private AudioClip laserFire;
        [SerializeField] private AudioClip missileFire;
        [SerializeField] private AudioClip explosion;
        [SerializeField] private AudioClip shieldHit;
        [SerializeField] private AudioClip hullHit;
        [SerializeField] private AudioClip missileAlert;

        [Header("Vehicle Sound Effects")]
        [SerializeField] private AudioClip engineStart;
        [SerializeField] private AudioClip engineLoop;
        [SerializeField] private AudioClip boost;
        [SerializeField] private AudioClip afterburner;

        private float musicVolume = 0.7f;
        private float sfxVolume = 1f;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeAudioSources();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeAudioSources()
        {
            if (musicSource == null)
            {
                GameObject musicObj = new GameObject("MusicSource");
                musicObj.transform.SetParent(transform);
                musicSource = musicObj.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
            }

            if (sfxSource == null)
            {
                GameObject sfxObj = new GameObject("SFXSource");
                sfxObj.transform.SetParent(transform);
                sfxSource = sfxObj.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
            }

            if (ambientSource == null)
            {
                GameObject ambientObj = new GameObject("AmbientSource");
                ambientObj.transform.SetParent(transform);
                ambientSource = ambientObj.AddComponent<AudioSource>();
                ambientSource.loop = true;
                ambientSource.playOnAwake = false;
            }

            if (voiceSource == null)
            {
                GameObject voiceObj = new GameObject("VoiceSource");
                voiceObj.transform.SetParent(transform);
                voiceSource = voiceObj.AddComponent<AudioSource>();
                voiceSource.playOnAwake = false;
            }
        }

        public void SetMusicVolume(float volume)
        {
            musicVolume = volume;
            if (musicSource != null)
                musicSource.volume = volume;
            if (ambientSource != null)
                ambientSource.volume = volume * 0.5f;
        }

        public void SetSFXVolume(float volume)
        {
            sfxVolume = volume;
            if (sfxSource != null)
                sfxSource.volume = volume;
        }

        public void PlayMenuMusic() => PlayMusic(menuMusic);
        public void PlayCombatMusic() => PlayMusic(combatMusic);
        public void PlayVictoryMusic() => PlayMusic(victoryMusic, false);
        public void PlayDefeatMusic() => PlayMusic(defeatMusic, false);

        public void PlayMusic(AudioClip clip, bool loop = true)
        {
            if (musicSource != null && clip != null)
            {
                musicSource.clip = clip;
                musicSource.loop = loop;
                musicSource.volume = musicVolume;
                musicSource.Play();
            }
        }

        public void StopMusic()
        {
            if (musicSource != null) musicSource.Stop();
        }

        public void FadeOutMusic(float duration = 1f)
        {
            StartCoroutine(FadeOutMusicCoroutine(duration));
        }

        private System.Collections.IEnumerator FadeOutMusicCoroutine(float duration)
        {
            float startVolume = musicSource.volume;
            float timer = 0f;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                musicSource.volume = Mathf.Lerp(startVolume, 0f, timer / duration);
                yield return null;
            }

            musicSource.Stop();
            musicSource.volume = startVolume;
        }

        public void PlaySFX(AudioClip clip, float volumeScale = 1f)
        {
            if (sfxSource != null && clip != null)
            {
                sfxSource.PlayOneShot(clip, sfxVolume * volumeScale);
            }
        }

        public void PlaySFXAtPoint(AudioClip clip, Vector3 position, float volumeScale = 1f)
        {
            if (clip != null)
            {
                AudioSource.PlayClipAtPoint(clip, position, sfxVolume * volumeScale);
            }
        }

        public void PlayButtonClick() => PlaySFX(buttonClick);
        public void PlayButtonHover() => PlaySFX(buttonHover, 0.5f);
        public void PlayPurchaseSuccess() => PlaySFX(purchaseSuccess);
        public void PlayPurchaseFail() => PlaySFX(purchaseFail);
        public void PlayNotification() => PlaySFX(notification);

        public void PlayLaserFire() => PlaySFX(laserFire);
        public void PlayMissileFire() => PlaySFX(missileFire);
        public void PlayExplosion() => PlaySFX(explosion);
        public void PlayShieldHit() => PlaySFX(shieldHit);
        public void PlayHullHit() => PlaySFX(hullHit);
        public void PlayMissileAlert() => PlaySFX(missileAlert);

        public void PlayEngineStart() => PlaySFX(engineStart);
        public void PlayBoost() => PlaySFX(boost);
        public void PlayAfterburner() => PlaySFX(afterburner);

        public void PlaySpaceAmbient()
        {
            if (ambientSource != null && ambientSpace != null)
            {
                ambientSource.clip = ambientSpace;
                ambientSource.volume = musicVolume * 0.5f;
                ambientSource.Play();
            }
        }

        public void StopAmbient()
        {
            if (ambientSource != null) ambientSource.Stop();
        }

        public void PlayVoice(AudioClip clip)
        {
            if (voiceSource != null && clip != null)
            {
                voiceSource.Stop();
                voiceSource.clip = clip;
                voiceSource.Play();
            }
        }
    }
}
