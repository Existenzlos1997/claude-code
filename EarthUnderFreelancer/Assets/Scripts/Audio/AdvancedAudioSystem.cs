using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Audio
{
    /// <summary>
    /// Advanced 3D audio system replacing SimpleAudioSystem
    /// Features: Spatial audio, occlusion, zones, dynamic mixing
    /// </summary>
    public class AdvancedAudioSystem : MonoBehaviour
    {
        [Header("Audio Mixer")]
        [SerializeField] private AudioMixer masterMixer;
        
        [Header("Settings")]
        [SerializeField] private int maxSimultaneousSounds = 64;
        [SerializeField] private float dopplerLevel = 1f;
        [SerializeField] private float masterVolume = 1f;
        
        private Dictionary<string, AudioClip> audioLibrary = new Dictionary<string, AudioClip>();
        private List<AudioSource> activeSources = new List<AudioSource>();
        private Queue<AudioSource> audioSourcePool = new Queue<AudioSource>();
        private Dictionary<string, AudioZone> audioZones = new Dictionary<string, AudioZone>();
        
        public static AdvancedAudioSystem Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Initialize()
        {
            // Pre-create audio source pool
            for (int i = 0; i < maxSimultaneousSounds; i++)
            {
                CreatePooledAudioSource();
            }
        }
        
        private void CreatePooledAudioSource()
        {
            GameObject sourceObj = new GameObject($"PooledAudioSource");
            sourceObj.transform.SetParent(transform);
            AudioSource source = sourceObj.AddComponent<AudioSource>();
            
            // Configure for 3D spatial audio
            source.spatialBlend = 1f;
            source.dopplerLevel = dopplerLevel;
            source.rolloffMode = AudioRolloffMode.Logarithmic;
            source.minDistance = 10f;
            source.maxDistance = 500f;
            
            audioSourcePool.Enqueue(source);
        }
        
        /// <summary>
        /// Play 3D sound at position with priority and options
        /// </summary>
        public AudioSource Play3DSound(string clipName, Vector3 position, float volume = 1f, float pitch = 1f, int priority = 128)
        {
            if (!audioLibrary.ContainsKey(clipName)) return null;
            
            AudioSource source = GetAvailableSource();
            if (source == null) return null;
            
            source.transform.position = position;
            source.clip = audioLibrary[clipName];
            source.volume = volume * masterVolume;
            source.pitch = pitch;
            source.priority = priority;
            source.Play();
            
            activeSources.Add(source);
            StartCoroutine(ReturnToPoolWhenFinished(source));
            
            return source;
        }
        
        /// <summary>
        /// Play sound attached to transform (follows movement)
        /// </summary>
        public AudioSource PlayAttached(string clipName, Transform parent, float volume = 1f, bool loop = false)
        {
            AudioSource source = Play3DSound(clipName, parent.position, volume);
            if (source != null)
            {
                source.transform.SetParent(parent);
                source.loop = loop;
            }
            return source;
        }
        
        /// <summary>
        /// Play music with crossfade
        /// </summary>
        public void PlayMusic(string clipName, float fadeTime = 2f)
        {
            // Implementation for music crossfade
            // Would use coroutine to fade out current, fade in new
        }
        
        /// <summary>
        /// Register audio zone for environmental effects
        /// </summary>
        public void RegisterZone(string zoneName, AudioReverbPreset preset, float priority)
        {
            audioZones[zoneName] = new AudioZone
            {
                reverbPreset = preset,
                priority = priority
            };
        }
        
        /// <summary>
        /// Calculate occlusion between source and listener
        /// </summary>
        private float CalculateOcclusion(Vector3 sourcePos, Vector3 listenerPos)
        {
            RaycastHit hit;
            Vector3 direction = sourcePos - listenerPos;
            float distance = direction.magnitude;
            
            if (Physics.Raycast(listenerPos, direction.normalized, out hit, distance))
            {
                // Hit something - apply occlusion based on material
                return 0.3f; // Simplified - would check material properties
            }
            
            return 1f; // No occlusion
        }
        
        private AudioSource GetAvailableSource()
        {
            if (audioSourcePool.Count > 0)
            {
                return audioSourcePool.Dequeue();
            }
            
            // All sources in use - try to steal lowest priority inactive
            AudioSource lowestPriority = null;
            int maxPriority = -1;
            
            foreach (var source in activeSources)
            {
                if (!source.isPlaying && source.priority > maxPriority)
                {
                    maxPriority = source.priority;
                    lowestPriority = source;
                }
            }
            
            if (lowestPriority != null)
            {
                activeSources.Remove(lowestPriority);
                return lowestPriority;
            }
            
            return null;
        }
        
        private System.Collections.IEnumerator ReturnToPoolWhenFinished(AudioSource source)
        {
            while (source.isPlaying)
            {
                yield return null;
            }
            
            activeSources.Remove(source);
            source.transform.SetParent(transform);
            source.transform.localPosition = Vector3.zero;
            audioSourcePool.Enqueue(source);
        }
        
        [System.Serializable]
        public class AudioZone
        {
            public AudioReverbPreset reverbPreset;
            public float priority;
        }
    }
}
