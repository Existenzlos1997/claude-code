using UnityEngine;
using System;
using EarthUnderFreelancer.Core;

namespace EarthUnderFreelancer.Monetization
{
    /// <summary>
    /// Manages Unity Ads integration
    /// </summary>
    public class AdsManager : MonoBehaviour
    {
        public static AdsManager Instance { get; private set; }

        [Header("Ad Unit IDs")]
        [SerializeField] private string androidGameId = "YOUR_ANDROID_GAME_ID";
        [SerializeField] private string iosGameId = "YOUR_IOS_GAME_ID";
        [SerializeField] private string interstitialAdUnitId = "Interstitial_Android";
        [SerializeField] private string rewardedAdUnitId = "Rewarded_Android";
        [SerializeField] private string bannerAdUnitId = "Banner_Android";

        [Header("Settings")]
        [SerializeField] private bool testMode = true;
        [SerializeField] private bool showBannerOnStart = false;
        [SerializeField] private int interstitialFrequency = 3; // Show every N missions

        private bool isInitialized = false;
        private int missionsSinceLastAd = 0;
        private Action<bool> rewardedCallback;

        public event Action OnInterstitialShown;
        public event Action OnInterstitialFailed;
        public event Action<int> OnRewardedCompleted;
        public event Action OnRewardedFailed;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            InitializeAds();
        }

        public void InitializeAds()
        {
            // Unity Ads initialization would go here
            // For actual implementation, use:
            // UnityAds.Initialize(gameId, testMode);
            
            string gameId = Application.platform == RuntimePlatform.Android ? androidGameId : iosGameId;
            Debug.Log($"Initializing Unity Ads with Game ID: {gameId}, Test Mode: {testMode}");
            
            isInitialized = true;
            
            if (showBannerOnStart)
            {
                ShowBanner();
            }
        }

        #region Interstitial Ads

        public void ShowInterstitial()
        {
            if (!isInitialized || !ShouldShowInterstitial())
            {
                Debug.Log("Interstitial not ready or not time to show");
                return;
            }

            // Unity Ads show interstitial would go here
            // UnityAds.Show(interstitialAdUnitId);
            
            Debug.Log("Showing Interstitial Ad");
            missionsSinceLastAd = 0;
            
            // Simulate ad shown
            OnInterstitialShown?.Invoke();
        }

        public void IncrementMissionCount()
        {
            missionsSinceLastAd++;
        }

        private bool ShouldShowInterstitial()
        {
            // Check if ads are allowed
            if (GameManager.Instance?.gameSettings?.personalizedAds == false)
                return false;

            return missionsSinceLastAd >= interstitialFrequency;
        }

        public bool IsInterstitialReady()
        {
            return isInitialized;
        }

        #endregion

        #region Rewarded Ads

        public void ShowRewardedAd(Action<bool> callback, int rewardAmount = 100)
        {
            if (!isInitialized)
            {
                Debug.LogWarning("Ads not initialized");
                callback?.Invoke(false);
                return;
            }

            rewardedCallback = callback;
            
            // Unity Ads show rewarded would go here
            // UnityAds.Show(rewardedAdUnitId, new ShowOptions { resultCallback = HandleShowResult });
            
            Debug.Log("Showing Rewarded Ad");
            
            // Simulate successful reward (in real implementation, this would be in the callback)
            SimulateRewardedAdComplete(rewardAmount);
        }

        private void SimulateRewardedAdComplete(int amount)
        {
            // Simulate ad completion
            Debug.Log($"Rewarded Ad completed, granting {amount} credits");
            
            if (GameManager.Instance != null)
            {
                GameManager.Instance.playerData.credits += amount;
                GameManager.Instance.SaveGame();
            }

            OnRewardedCompleted?.Invoke(amount);
            EventManager.TriggerEvent(GameEvents.AD_WATCHED, amount);
            EventManager.TriggerEvent(GameEvents.REWARD_EARNED, amount);
            
            rewardedCallback?.Invoke(true);
            rewardedCallback = null;
        }

        public bool IsRewardedAdReady()
        {
            return isInitialized;
        }

        public void WatchAdForCredits()
        {
            ShowRewardedAd((success) =>
            {
                if (success)
                {
                    Debug.Log("Player earned credits from ad");
                }
            }, 500);
        }

        public void WatchAdForDoubleReward()
        {
            ShowRewardedAd((success) =>
            {
                if (success)
                {
                    // Double the last mission reward
                    Debug.Log("Player doubled their reward");
                }
            });
        }

        public void WatchAdForRevive()
        {
            ShowRewardedAd((success) =>
            {
                if (success)
                {
                    // Revive player
                    Debug.Log("Player revived from ad");
                }
            });
        }

        #endregion

        #region Banner Ads

        public void ShowBanner()
        {
            if (!isInitialized) return;
            
            // Unity Ads banner would go here
            // BannerLoadOptions options = new BannerLoadOptions { loadCallback = OnBannerLoaded, errorCallback = OnBannerError };
            // Advertisement.Banner.Load(bannerAdUnitId, options);
            
            Debug.Log("Showing Banner Ad");
        }

        public void HideBanner()
        {
            // Advertisement.Banner.Hide();
            Debug.Log("Hiding Banner Ad");
        }

        #endregion

        public void SetTestMode(bool enabled)
        {
            testMode = enabled;
        }

        public void SetPersonalizedAds(bool enabled)
        {
            // Set consent for personalized ads
            Debug.Log($"Personalized ads: {enabled}");
        }
    }
}
