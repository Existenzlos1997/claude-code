using UnityEngine;
using System;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Audio
{
    /// <summary>
    /// Advanced dynamic music system with layered tracks, adaptive transitions, and mood-based composition
    /// </summary>
    public class DynamicMusicSystem : MonoBehaviour
    {
        public static DynamicMusicSystem Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource primarySource;
        [SerializeField] private AudioSource secondarySource;
        [SerializeField] private AudioSource[] layerSources;
        [SerializeField] private AudioSource stingerSource;

        [Header("Music Settings")]
        [SerializeField] private float crossfadeDuration = 2f;
        [SerializeField] private float layerFadeDuration = 1f;
        [SerializeField] private float maxVolume = 0.7f;
        [SerializeField] private bool syncToBeat = true;
        [SerializeField] private float bpm = 120f;

        [Header("Mood Tracks")]
        [SerializeField] private MusicTrack[] peacefulTracks;
        [SerializeField] private MusicTrack[] explorationTracks;
        [SerializeField] private MusicTrack[] tensionTracks;
        [SerializeField] private MusicTrack[] combatTracks;
        [SerializeField] private MusicTrack[] victoryTracks;
        [SerializeField] private MusicTrack[] defeatTracks;
        [SerializeField] private MusicTrack[] bossTrack;

        [Header("Stingers")]
        [SerializeField] private AudioClip combatStartStinger;
        [SerializeField] private AudioClip combatEndStinger;
        [SerializeField] private AudioClip victoryStinger;
        [SerializeField] private AudioClip defeatStinger;
        [SerializeField] private AudioClip levelUpStinger;
        [SerializeField] private AudioClip discoveryStinger;

        // State
        private MusicMood currentMood = MusicMood.Peaceful;
        private MusicMood targetMood = MusicMood.Peaceful;
        private MusicTrack currentTrack;
        private bool isPrimaryActive = true;
        private float moodTransitionProgress;
        private float combatIntensity;
        private Dictionary<string, float> layerVolumes = new Dictionary<string, float>();
        private float beatTimer;
        private int currentBeat;

        // Events
        public event Action<MusicMood> OnMoodChanged;
        public event Action<int> OnBeat;
        public event Action<int> OnBar; // Every 4 beats

        public enum MusicMood
        {
            Silent,
            Peaceful,
            Exploration,
            Tension,
            Combat,
            CombatIntense,
            Boss,
            Victory,
            Defeat,
            Cinematic
        }

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
            // Create audio sources if not assigned
            if (primarySource == null)
            {
                primarySource = CreateAudioSource("PrimaryMusic");
            }
            if (secondarySource == null)
            {
                secondarySource = CreateAudioSource("SecondaryMusic");
            }
            if (stingerSource == null)
            {
                stingerSource = CreateAudioSource("Stinger");
                stingerSource.volume = 1f;
            }

            // Create layer sources
            if (layerSources == null || layerSources.Length == 0)
            {
                layerSources = new AudioSource[4];
                for (int i = 0; i < 4; i++)
                {
                    layerSources[i] = CreateAudioSource($"MusicLayer{i}");
                    layerSources[i].volume = 0f;
                }
            }
        }

        private AudioSource CreateAudioSource(string name)
        {
            var go = new GameObject(name);
            go.transform.SetParent(transform);
            var source = go.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = true;
            source.volume = 0f;
            return source;
        }

        private void Update()
        {
            UpdateBeatTracking();
            UpdateMoodTransition();
            UpdateCombatIntensityLayers();
        }

        private void UpdateBeatTracking()
        {
            if (!syncToBeat) return;

            float beatDuration = 60f / bpm;
            beatTimer += Time.deltaTime;

            if (beatTimer >= beatDuration)
            {
                beatTimer -= beatDuration;
                currentBeat++;
                OnBeat?.Invoke(currentBeat);

                if (currentBeat % 4 == 0)
                {
                    OnBar?.Invoke(currentBeat / 4);
                }
            }
        }

        private void UpdateMoodTransition()
        {
            if (currentMood != targetMood)
            {
                moodTransitionProgress += Time.deltaTime / crossfadeDuration;

                if (moodTransitionProgress >= 1f)
                {
                    moodTransitionProgress = 0f;
                    currentMood = targetMood;
                    FinishMoodTransition();
                }
            }
        }

        private void UpdateCombatIntensityLayers()
        {
            if (currentMood != MusicMood.Combat && currentMood != MusicMood.CombatIntense) return;

            // Adjust layer volumes based on combat intensity
            if (layerSources.Length >= 3)
            {
                // Layer 0: Base rhythm (always on in combat)
                SetLayerVolume(0, 1f);

                // Layer 1: Melody (intensity > 0.3)
                SetLayerVolume(1, combatIntensity > 0.3f ? 1f : 0f);

                // Layer 2: Heavy percussion (intensity > 0.6)
                SetLayerVolume(2, combatIntensity > 0.6f ? 1f : 0f);

                // Layer 3: Full orchestra (intensity > 0.85)
                SetLayerVolume(3, combatIntensity > 0.85f ? 1f : 0f);
            }
        }

        private void SetLayerVolume(int layerIndex, float targetVolume)
        {
            if (layerIndex < 0 || layerIndex >= layerSources.Length) return;

            var source = layerSources[layerIndex];
            if (source != null)
            {
                source.volume = Mathf.Lerp(source.volume, targetVolume * maxVolume, Time.deltaTime / layerFadeDuration);
            }
        }

        public void SetMood(MusicMood mood)
        {
            if (targetMood == mood) return;

            targetMood = mood;
            moodTransitionProgress = 0f;

            StartMoodTransition(mood);
            OnMoodChanged?.Invoke(mood);
        }

        private void StartMoodTransition(MusicMood mood)
        {
            MusicTrack[] tracks = GetTracksForMood(mood);
            if (tracks == null || tracks.Length == 0) return;

            // Select random track from mood
            currentTrack = tracks[UnityEngine.Random.Range(0, tracks.Length)];

            // Crossfade to new track
            if (isPrimaryActive)
            {
                FadeToSecondary(currentTrack);
            }
            else
            {
                FadeToPrimary(currentTrack);
            }

            // Play stinger if transitioning to/from combat
            if (mood == MusicMood.Combat || mood == MusicMood.CombatIntense)
            {
                PlayStinger(combatStartStinger);
            }
            else if (currentMood == MusicMood.Combat || currentMood == MusicMood.CombatIntense)
            {
                if (mood == MusicMood.Victory)
                {
                    PlayStinger(victoryStinger);
                }
                else if (mood == MusicMood.Defeat)
                {
                    PlayStinger(defeatStinger);
                }
                else
                {
                    PlayStinger(combatEndStinger);
                }
            }
        }

        private void FinishMoodTransition()
        {
            // Stop inactive source
            var inactiveSource = isPrimaryActive ? secondarySource : primarySource;
            inactiveSource.Stop();
            inactiveSource.volume = 0f;
        }

        private void FadeToPrimary(MusicTrack track)
        {
            if (track.mainClip != null)
            {
                primarySource.clip = track.mainClip;
                primarySource.Play();
            }

            StartCoroutine(CrossfadeCoroutine(secondarySource, primarySource));
            isPrimaryActive = true;

            LoadLayersForTrack(track);
        }

        private void FadeToSecondary(MusicTrack track)
        {
            if (track.mainClip != null)
            {
                secondarySource.clip = track.mainClip;
                secondarySource.Play();
            }

            StartCoroutine(CrossfadeCoroutine(primarySource, secondarySource));
            isPrimaryActive = false;

            LoadLayersForTrack(track);
        }

        private System.Collections.IEnumerator CrossfadeCoroutine(AudioSource fadeOut, AudioSource fadeIn)
        {
            float elapsed = 0f;
            float startVolumeOut = fadeOut.volume;
            float startVolumeIn = fadeIn.volume;

            while (elapsed < crossfadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / crossfadeDuration;

                fadeOut.volume = Mathf.Lerp(startVolumeOut, 0f, t);
                fadeIn.volume = Mathf.Lerp(startVolumeIn, maxVolume, t);

                yield return null;
            }

            fadeOut.volume = 0f;
            fadeIn.volume = maxVolume;
        }

        private void LoadLayersForTrack(MusicTrack track)
        {
            // Load layer clips if available
            if (track.layers != null)
            {
                for (int i = 0; i < layerSources.Length && i < track.layers.Length; i++)
                {
                    if (track.layers[i] != null)
                    {
                        layerSources[i].clip = track.layers[i];
                        layerSources[i].Play();

                        // Sync to main track
                        if (isPrimaryActive)
                        {
                            layerSources[i].time = primarySource.time;
                        }
                        else
                        {
                            layerSources[i].time = secondarySource.time;
                        }
                    }
                }
            }
        }

        private MusicTrack[] GetTracksForMood(MusicMood mood)
        {
            switch (mood)
            {
                case MusicMood.Peaceful: return peacefulTracks;
                case MusicMood.Exploration: return explorationTracks;
                case MusicMood.Tension: return tensionTracks;
                case MusicMood.Combat:
                case MusicMood.CombatIntense: return combatTracks;
                case MusicMood.Boss: return bossTrack;
                case MusicMood.Victory: return victoryTracks;
                case MusicMood.Defeat: return defeatTracks;
                default: return peacefulTracks;
            }
        }

        public void SetCombatIntensity(float intensity)
        {
            combatIntensity = Mathf.Clamp01(intensity);

            // Auto-escalate mood based on intensity
            if (currentMood == MusicMood.Combat && combatIntensity > 0.8f)
            {
                SetMood(MusicMood.CombatIntense);
            }
            else if (currentMood == MusicMood.CombatIntense && combatIntensity < 0.5f)
            {
                SetMood(MusicMood.Combat);
            }
        }

        public void PlayStinger(AudioClip stinger)
        {
            if (stinger == null || stingerSource == null) return;

            stingerSource.PlayOneShot(stinger);
        }

        public void PlayLevelUpStinger()
        {
            PlayStinger(levelUpStinger);
        }

        public void PlayDiscoveryStinger()
        {
            PlayStinger(discoveryStinger);
        }

        public void FadeOut(float duration = 2f)
        {
            StartCoroutine(FadeOutCoroutine(duration));
        }

        private System.Collections.IEnumerator FadeOutCoroutine(float duration)
        {
            float elapsed = 0f;
            float startVolumePrimary = primarySource.volume;
            float startVolumeSecondary = secondarySource.volume;
            float[] startVolumeLayers = new float[layerSources.Length];
            for (int i = 0; i < layerSources.Length; i++)
            {
                startVolumeLayers[i] = layerSources[i].volume;
            }

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                primarySource.volume = Mathf.Lerp(startVolumePrimary, 0f, t);
                secondarySource.volume = Mathf.Lerp(startVolumeSecondary, 0f, t);

                for (int i = 0; i < layerSources.Length; i++)
                {
                    layerSources[i].volume = Mathf.Lerp(startVolumeLayers[i], 0f, t);
                }

                yield return null;
            }

            StopAll();
        }

        public void FadeIn(float duration = 2f)
        {
            StartCoroutine(FadeInCoroutine(duration));
        }

        private System.Collections.IEnumerator FadeInCoroutine(float duration)
        {
            // Start playing if not already
            if (!primarySource.isPlaying && !secondarySource.isPlaying)
            {
                SetMood(currentMood);
            }

            float elapsed = 0f;
            var activeSource = isPrimaryActive ? primarySource : secondarySource;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                activeSource.volume = Mathf.Lerp(0f, maxVolume, t);

                yield return null;
            }
        }

        public void StopAll()
        {
            primarySource.Stop();
            secondarySource.Stop();
            foreach (var layer in layerSources)
            {
                layer.Stop();
            }
        }

        public void SetVolume(float volume)
        {
            maxVolume = Mathf.Clamp01(volume);

            var activeSource = isPrimaryActive ? primarySource : secondarySource;
            activeSource.volume = maxVolume;
        }

        public MusicMood GetCurrentMood() => currentMood;
        public float GetCombatIntensity() => combatIntensity;
    }

    [Serializable]
    public class MusicTrack
    {
        public string trackName;
        public AudioClip mainClip;
        public AudioClip[] layers;
        public float intensityThreshold;
        public MusicMood[] appropriateMoods;
    }
}
