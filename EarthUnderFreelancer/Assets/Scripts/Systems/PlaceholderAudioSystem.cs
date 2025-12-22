using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Systems
{
    /// <summary>
    /// Placeholder Audio System
    /// Generates procedural audio placeholders until real audio is added
    /// Uses Unity's AudioClip.Create for simple tones
    /// </summary>
    public class PlaceholderAudioSystem : MonoBehaviour
    {
        private static PlaceholderAudioSystem _instance;
        public static PlaceholderAudioSystem Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("PlaceholderAudioSystem");
                    _instance = go.AddComponent<PlaceholderAudioSystem>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        private Dictionary<string, AudioClip> _cachedClips = new Dictionary<string, AudioClip>();

        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);

            GeneratePlaceholderAudio();
        }

        private void GeneratePlaceholderAudio()
        {
            Debug.Log("Generating placeholder audio clips...");

            // Engine sounds (different frequencies for different aircraft types)
            _cachedClips["JetEngine"] = GenerateTone(200f, 1f, 0.3f);
            _cachedClips["PropEngine"] = GenerateTone(100f, 1f, 0.2f);
            _cachedClips["TurbopropEngine"] = GenerateTone(150f, 1f, 0.25f);
            
            // Weapon sounds
            _cachedClips["MachineGun"] = GenerateNoise(0.1f, 0.5f);
            _cachedClips["Cannon"] = GenerateNoise(0.15f, 0.7f);
            _cachedClips["MissileLaunch"] = GenerateSweep(100f, 300f, 0.5f, 0.6f);
            
            // Explosions
            _cachedClips["ExplosionSmall"] = GenerateNoise(0.3f, 0.8f);
            _cachedClips["ExplosionLarge"] = GenerateNoise(0.5f, 1.0f);
            
            // UI sounds
            _cachedClips["ButtonClick"] = GenerateTone(800f, 0.05f, 0.3f);
            _cachedClips["MenuOpen"] = GenerateSweep(400f, 600f, 0.1f, 0.2f);
            _cachedClips["MenuClose"] = GenerateSweep(600f, 400f, 0.1f, 0.2f);
            _cachedClips["Achievement"] = GenerateChord(new float[] {523f, 659f, 784f}, 0.5f, 0.4f);
            
            // Warnings
            _cachedClips["WarningBeep"] = GenerateTone(1000f, 0.2f, 0.5f);
            _cachedClips["LockOnWarning"] = GenerateTone(880f, 0.15f, 0.6f);
            _cachedClips["LowFuel"] = GenerateTone(440f, 0.3f, 0.4f);

            Debug.Log($"✓ Generated {_cachedClips.Count} placeholder audio clips");
        }

        /// <summary>
        /// Get a placeholder audio clip by name
        /// </summary>
        public AudioClip GetClip(string clipName)
        {
            if (_cachedClips.TryGetValue(clipName, out AudioClip clip))
            {
                return clip;
            }
            Debug.LogWarning($"Placeholder audio clip '{clipName}' not found. Generating default.");
            return GenerateTone(440f, 0.5f, 0.3f); // Default A4 tone
        }

        /// <summary>
        /// Play a placeholder sound at a position
        /// </summary>
        public void PlaySound(string clipName, Vector3 position, float volume = 1f)
        {
            AudioClip clip = GetClip(clipName);
            if (clip != null)
            {
                AudioSource.PlayClipAtPoint(clip, position, volume);
            }
        }

        /// <summary>
        /// Generate a simple sine wave tone
        /// </summary>
        private AudioClip GenerateTone(float frequency, float duration, float volume)
        {
            int sampleRate = 44100;
            int sampleCount = Mathf.CeilToInt(sampleRate * duration);
            AudioClip clip = AudioClip.Create($"Tone_{frequency}Hz", sampleCount, 1, sampleRate, false);
            
            float[] samples = new float[sampleCount];
            for (int i = 0; i < sampleCount; i++)
            {
                float t = i / (float)sampleRate;
                samples[i] = Mathf.Sin(2f * Mathf.PI * frequency * t) * volume;
                
                // Apply fade out in last 10%
                if (i > sampleCount * 0.9f)
                {
                    float fade = 1f - ((i - sampleCount * 0.9f) / (sampleCount * 0.1f));
                    samples[i] *= fade;
                }
            }
            
            clip.SetData(samples, 0);
            return clip;
        }

        /// <summary>
        /// Generate white noise (for explosions, weapon fire)
        /// </summary>
        private AudioClip GenerateNoise(float duration, float volume)
        {
            int sampleRate = 44100;
            int sampleCount = Mathf.CeilToInt(sampleRate * duration);
            AudioClip clip = AudioClip.Create($"Noise_{duration}s", sampleCount, 1, sampleRate, false);
            
            float[] samples = new float[sampleCount];
            for (int i = 0; i < sampleCount; i++)
            {
                samples[i] = (Random.value * 2f - 1f) * volume;
                
                // Exponential decay
                float decay = Mathf.Exp(-5f * i / sampleCount);
                samples[i] *= decay;
            }
            
            clip.SetData(samples, 0);
            return clip;
        }

        /// <summary>
        /// Generate a frequency sweep (for missiles, sirens)
        /// </summary>
        private AudioClip GenerateSweep(float startFreq, float endFreq, float duration, float volume)
        {
            int sampleRate = 44100;
            int sampleCount = Mathf.CeilToInt(sampleRate * duration);
            AudioClip clip = AudioClip.Create($"Sweep_{startFreq}-{endFreq}Hz", sampleCount, 1, sampleRate, false);
            
            float[] samples = new float[sampleCount];
            for (int i = 0; i < sampleCount; i++)
            {
                float t = i / (float)sampleRate;
                float progress = i / (float)sampleCount;
                float freq = Mathf.Lerp(startFreq, endFreq, progress);
                samples[i] = Mathf.Sin(2f * Mathf.PI * freq * t) * volume;
            }
            
            clip.SetData(samples, 0);
            return clip;
        }

        /// <summary>
        /// Generate a chord (multiple frequencies)
        /// </summary>
        private AudioClip GenerateChord(float[] frequencies, float duration, float volume)
        {
            int sampleRate = 44100;
            int sampleCount = Mathf.CeilToInt(sampleRate * duration);
            AudioClip clip = AudioClip.Create($"Chord", sampleCount, 1, sampleRate, false);
            
            float[] samples = new float[sampleCount];
            for (int i = 0; i < sampleCount; i++)
            {
                float t = i / (float)sampleRate;
                float sample = 0f;
                
                foreach (float freq in frequencies)
                {
                    sample += Mathf.Sin(2f * Mathf.PI * freq * t);
                }
                
                samples[i] = (sample / frequencies.Length) * volume;
                
                // Fade out
                if (i > sampleCount * 0.7f)
                {
                    float fade = 1f - ((i - sampleCount * 0.7f) / (sampleCount * 0.3f));
                    samples[i] *= fade;
                }
            }
            
            clip.SetData(samples, 0);
            return clip;
        }

        /// <summary>
        /// Helper method to create an AudioSource with a placeholder clip
        /// </summary>
        public AudioSource CreateAudioSource(GameObject target, string clipName, bool loop = false, float volume = 1f)
        {
            AudioSource source = target.AddComponent<AudioSource>();
            source.clip = GetClip(clipName);
            source.loop = loop;
            source.volume = volume;
            source.spatialBlend = 1f; // 3D sound
            source.minDistance = 10f;
            source.maxDistance = 500f;
            return source;
        }
    }
}
