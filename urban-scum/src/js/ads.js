/**
 * Urban Scum - AdMob Integration
 * Handles Interstitial and Rewarded Ads
 */

const Ads = {
    // AdMob App ID (replace with your actual App ID)
    appId: 'ca-app-pub-XXXXXXXXXXXXXXXX~XXXXXXXXXX',
    
    // Ad Unit IDs (replace with your actual Ad Unit IDs)
    interstitialId: 'ca-app-pub-XXXXXXXXXXXXXXXX/XXXXXXXXXX',
    rewardedId: 'ca-app-pub-XXXXXXXXXXXXXXXX/XXXXXXXXXX',
    
    // Test Ad IDs (use these for development)
    testInterstitialId: 'ca-app-pub-3940256099942544/1033173712',
    testRewardedId: 'ca-app-pub-3940256099942544/5224354917',
    
    // State
    isInitialized: false,
    isInterstitialLoaded: false,
    isRewardedLoaded: false,
    useTestAds: true, // Set to false for production
    
    // Callbacks
    onRewardedComplete: null,
    
    // Initialize AdMob
    init: () => {
        // Check if running in Cordova/Capacitor environment
        if (typeof admob !== 'undefined') {
            Ads.initAdMob();
        } else if (typeof AdMob !== 'undefined') {
            Ads.initCapacitorAdMob();
        } else {
            console.log('AdMob not available - running in browser mode');
            Ads.isInitialized = true;
        }
    },
    
    // Initialize for Cordova admob-free or admob-plus
    initAdMob: () => {
        const adUnitIds = {
            interstitial: Ads.useTestAds ? Ads.testInterstitialId : Ads.interstitialId,
            rewarded: Ads.useTestAds ? Ads.testRewardedId : Ads.rewardedId
        };
        
        // Initialize interstitial
        admob.interstitial.config({
            id: adUnitIds.interstitial,
            isTesting: Ads.useTestAds,
            autoShow: false
        });
        
        // Initialize rewarded
        admob.rewardVideo.config({
            id: adUnitIds.rewarded,
            isTesting: Ads.useTestAds,
            autoShow: false
        });
        
        // Event listeners
        document.addEventListener('admob.interstitial.load', () => {
            Ads.isInterstitialLoaded = true;
            console.log('Interstitial ad loaded');
        });
        
        document.addEventListener('admob.interstitial.close', () => {
            Ads.isInterstitialLoaded = false;
            Ads.prepareInterstitial();
        });
        
        document.addEventListener('admob.rewardVideo.load', () => {
            Ads.isRewardedLoaded = true;
            console.log('Rewarded ad loaded');
        });
        
        document.addEventListener('admob.rewardVideo.reward', () => {
            console.log('User earned reward');
            if (Ads.onRewardedComplete) {
                Ads.onRewardedComplete();
                Ads.onRewardedComplete = null;
            }
        });
        
        document.addEventListener('admob.rewardVideo.close', () => {
            Ads.isRewardedLoaded = false;
            Ads.prepareRewarded();
        });
        
        // Prepare ads
        Ads.prepareInterstitial();
        Ads.prepareRewarded();
        
        Ads.isInitialized = true;
        console.log('AdMob initialized (Cordova)');
    },
    
    // Initialize for Capacitor AdMob
    initCapacitorAdMob: () => {
        AdMob.initialize({
            requestTrackingAuthorization: true,
            testingDevices: Ads.useTestAds ? ['YOUR_DEVICE_ID'] : [],
            initializeForTesting: Ads.useTestAds
        }).then(() => {
            Ads.isInitialized = true;
            console.log('AdMob initialized (Capacitor)');
            
            // Prepare ads
            Ads.prepareInterstitial();
            Ads.prepareRewarded();
            
            // Setup listeners
            AdMob.addListener('interstitialAdLoaded', () => {
                Ads.isInterstitialLoaded = true;
            });
            
            AdMob.addListener('interstitialAdDismissed', () => {
                Ads.isInterstitialLoaded = false;
                Ads.prepareInterstitial();
            });
            
            AdMob.addListener('rewardVideoAdLoaded', () => {
                Ads.isRewardedLoaded = true;
            });
            
            AdMob.addListener('rewardVideoAdRewarded', () => {
                if (Ads.onRewardedComplete) {
                    Ads.onRewardedComplete();
                    Ads.onRewardedComplete = null;
                }
            });
            
            AdMob.addListener('rewardVideoAdDismissed', () => {
                Ads.isRewardedLoaded = false;
                Ads.prepareRewarded();
            });
        });
    },
    
    // Prepare interstitial ad
    prepareInterstitial: () => {
        if (typeof admob !== 'undefined') {
            admob.interstitial.prepare();
        } else if (typeof AdMob !== 'undefined') {
            AdMob.prepareInterstitial({
                adId: Ads.useTestAds ? Ads.testInterstitialId : Ads.interstitialId
            });
        }
    },
    
    // Prepare rewarded ad
    prepareRewarded: () => {
        if (typeof admob !== 'undefined') {
            admob.rewardVideo.prepare();
        } else if (typeof AdMob !== 'undefined') {
            AdMob.prepareRewardVideoAd({
                adId: Ads.useTestAds ? Ads.testRewardedId : Ads.rewardedId
            });
        }
    },
    
    // Show interstitial ad
    showInterstitial: () => {
        if (!Ads.isInitialized) {
            console.log('AdMob not initialized');
            return;
        }
        
        if (typeof admob !== 'undefined' && Ads.isInterstitialLoaded) {
            admob.interstitial.show();
        } else if (typeof AdMob !== 'undefined' && Ads.isInterstitialLoaded) {
            AdMob.showInterstitial();
        } else {
            console.log('Interstitial ad not ready');
            // For browser testing, simulate ad
            if (!Ads.isNativeApp()) {
                Ads.simulateInterstitial();
            }
        }
    },
    
    // Show rewarded ad
    showRewarded: (onComplete) => {
        Ads.onRewardedComplete = onComplete;
        
        if (!Ads.isInitialized) {
            console.log('AdMob not initialized');
            // For browser testing, simulate reward
            if (!Ads.isNativeApp() && onComplete) {
                Ads.simulateRewarded(onComplete);
            }
            return;
        }
        
        if (typeof admob !== 'undefined' && Ads.isRewardedLoaded) {
            admob.rewardVideo.show();
        } else if (typeof AdMob !== 'undefined' && Ads.isRewardedLoaded) {
            AdMob.showRewardVideoAd();
        } else {
            console.log('Rewarded ad not ready');
            // For browser testing, simulate reward
            if (!Ads.isNativeApp() && onComplete) {
                Ads.simulateRewarded(onComplete);
            }
        }
    },
    
    // Check if running as native app
    isNativeApp: () => {
        return typeof admob !== 'undefined' || typeof AdMob !== 'undefined';
    },
    
    // Simulate interstitial for browser testing
    simulateInterstitial: () => {
        const adContainer = document.getElementById('ad-container');
        adContainer.classList.remove('hidden');
        adContainer.innerHTML = `
            <div style="text-align: center; padding: 20px;">
                <p>📺 Interstitial Ad Placeholder</p>
                <button onclick="Ads.closeSimulatedAd()" style="padding: 10px 20px; margin-top: 10px; cursor: pointer;">Close</button>
            </div>
        `;
        
        // Auto close after 3 seconds
        setTimeout(() => {
            Ads.closeSimulatedAd();
        }, 3000);
    },
    
    // Simulate rewarded for browser testing
    simulateRewarded: (onComplete) => {
        const adContainer = document.getElementById('ad-container');
        adContainer.classList.remove('hidden');
        adContainer.style.height = '200px';
        adContainer.innerHTML = `
            <div style="text-align: center; padding: 20px;">
                <p>🎬 Rewarded Ad Placeholder</p>
                <p>Watch to earn reward!</p>
                <div id="reward-timer" style="font-size: 24px; margin: 10px 0;">5</div>
            </div>
        `;
        
        let countdown = 5;
        const timer = setInterval(() => {
            countdown--;
            document.getElementById('reward-timer').textContent = countdown;
            
            if (countdown <= 0) {
                clearInterval(timer);
                adContainer.classList.add('hidden');
                adContainer.style.height = '60px';
                if (onComplete) {
                    onComplete();
                }
            }
        }, 1000);
    },
    
    // Close simulated ad
    closeSimulatedAd: () => {
        const adContainer = document.getElementById('ad-container');
        adContainer.classList.add('hidden');
        adContainer.style.height = '60px';
    }
};

// Export
window.Ads = Ads;
