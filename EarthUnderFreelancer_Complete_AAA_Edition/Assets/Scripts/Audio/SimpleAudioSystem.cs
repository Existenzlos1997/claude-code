using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Audio
{
    /// <summary>
    /// Simple audio system for game sounds
    /// Implements VERBESSERUNGSPLAN Priority 2 - Audio System
    /// </summary>
    public class SimpleAudioSystem : MonoBehaviour
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource engineSource;
        
        [Header("Settings")]
        [SerializeField] private float masterVolume = 1f;
        [SerializeField] private float musicVolume = 0.7f;
        [SerializeField] private float sfxVolume = 1f;
        [SerializeField] private float engineVolume = 0.8f;
        
        private Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();
        
        public static SimpleAudioSystem Instance { get; private set; }
        
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
                musicObj.transform.parent = transform;
                musicSource = musicObj.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
            }
            
            if (sfxSource == null)
            {
                GameObject sfxObj = new GameObject("SFXSource");
                sfxObj.transform.parent = transform;
                sfxSource = sfxObj.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
            }
            
            if (engineSource == null)
            {
                GameObject engineObj = new GameObject("EngineSource");
                engineObj.transform.parent = transform;
                engineSource = engineObj.AddComponent<AudioSource>();
                engineSource.loop = true;
                engineSource.playOnAwake = false;
            }
            
            UpdateVolumes();
        }
        
        public void PlaySFX(string soundName, float volume = 1f)
        {
            if (sfxSource == null) return;
            
            // In real implementation, would load from Resources or audio clips
            // For now, just log
            Debug.Log($"[Audio] Playing SFX: {soundName}");
            sfxSource.PlayOneShot(null, volume * sfxVolume * masterVolume);
        }
        
        public void PlayMusic(string musicName)
        {
            if (musicSource == null) return;
            
            Debug.Log($"[Audio] Playing Music: {musicName}");
            // Would load and play music clip
            musicSource.Play();
        }
        
        public void StopMusic()
        {
            if (musicSource != null)
            {
                musicSource.Stop();
            }
        }
        
        public void SetEngineVolume(float throttlePercent)
        {
            if (engineSource == null) return;
            
            float volume = Mathf.Lerp(0.3f, 1f, throttlePercent);
            engineSource.volume = volume * engineVolume * masterVolume;
            
            float pitch = Mathf.Lerp(0.8f, 1.5f, throttlePercent);
            engineSource.pitch = pitch;
        }
        
        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
        }
        
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
        }
        
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
        }
        
        private void UpdateVolumes()
        {
            if (musicSource != null)
                musicSource.volume = musicVolume * masterVolume;
            if (sfxSource != null)
                sfxSource.volume = sfxVolume * masterVolume;
            if (engineSource != null)
                engineSource.volume = engineVolume * masterVolume;
        }
    }
}
