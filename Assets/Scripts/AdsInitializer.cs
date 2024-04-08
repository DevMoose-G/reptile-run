using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;
using GoogleMobileAds.Api;

public class AdsInitializer : MonoBehaviour
{
    // [SerializeField] string _androidGameId;
    // [SerializeField] string _iOSGameId;
    // [SerializeField] bool _testMode = true;
    private string _gameId;

    // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
        private string _adUnitId = "ca-app-pub-5931616599429277/3428048774";
        private string _interstitialAdUnitId = "ca-app-pub-5931616599429277/2893541742";
#elif UNITY_IPHONE
        private string _adUnitId = "ca-app-pub-5931616599429277/6307501160";
#else
    private string _adUnitId = "unused";
    private string _interstitialAdUnitId = "unused";
    #endif

    public UpgradeNode data_for_BoughtUpgrade;

    public void Start()
    {
        // When true all events raised by GoogleMobileAds will be raised
        // on the Unity main thread. The default value is false.
        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(initStatus => { });
    }

    

    // Loads the rewarded ad.
    public void LoadInterstitialAd()
    {
        // Clean up the old ad before loading a new one.
        if (AdsManager.interstitialAd != null)
        {
            AdsManager.interstitialAd.Destroy();
            AdsManager.interstitialAd = null;
        }

        Debug.Log("Loading the interstitial ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();
        // adRequest.Keywords.Add("unity-admob-sample");

        // send the request to load the ad.
        InterstitialAd.Load(_adUnitId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Interstitial ad loaded with response : "
                          + ad.GetResponseInfo());

                AdsManager.interstitialAd = ad;

                RegisterEventHandlers(ad);
            });
    }

    public void ShowInterstitialAd()
    {
        if (AdsManager.interstitialAd != null && AdsManager.interstitialAd.CanShowAd())
        {
            AdsManager.interstitialAd.Show();
        } else
        {
            Debug.LogError("Interstitial ad is not ready yet.");
        }
    }

    private void RegisterEventHandlers(InterstitialAd ad)
    {

        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Interstitial ad was clicked.");
            SceneManager.LoadSceneAsync(0);
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial ad was closed.");
            SceneManager.LoadSceneAsync(0);
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content with error : "
                + error);
            SceneManager.LoadSceneAsync(0);
        };
    }

    // Loads the rewarded ad.
    public void LoadRewardedAd()
    {
        // Clean up the old ad before loading a new one.
        if (AdsManager.rewardedAd != null)
        {
            AdsManager.rewardedAd.Destroy();
            AdsManager.rewardedAd = null;
        }

        Debug.Log("Loading the rewarded ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();
        // adRequest.Keywords.Add("unity-admob-sample");

        // send the request to load the ad.
        RewardedAd.Load(_adUnitId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Rewarded ad loaded with response : "
                          + ad.GetResponseInfo());

                AdsManager.rewardedAd = ad;

                RegisterReloadHandler(ad);

                GameObject.Find("UIDocument").GetComponent<UI>().UpdateUpgrades();
            });
    }

    public void ShowRewardedAd()
    {
        const string rewardMsg =
            "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

        if (AdsManager.rewardedAd != null && AdsManager.rewardedAd.CanShowAd())
        {
            AdsManager.rewardedAd.Show((Reward reward) =>
            {
                Debug.Log(System.String.Format(rewardMsg, reward.Type, reward.Amount));
                // Grant a reward.
                GameObject.Find("UIDocument").GetComponent<UI>().adUpgradeCounter++;
                GameObject.Find("UIDocument").GetComponent<UI>().ApplyUpgrade(null, data_for_BoughtUpgrade);
            });
        }
    }

    private void RegisterReloadHandler(RewardedAd ad)
    {
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () => {
            Debug.Log("Rewarded Ad full screen content closed.");

            // Reload the ad so that we can show another as soon as possible.
            LoadRewardedAd();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content " +
                           "with error : " + error);

            // Reload the ad so that we can show another as soon as possible.
            LoadRewardedAd();
        };

        ad.OnAdPaid += (AdValue val) =>
        {
            if (AdsManager.rewardedAd.CanShowAd() == false)
            {
                LoadRewardedAd();
            }
        };
    }

    void OnDestroy()
    {
        
    }

    /*
    void Awake()
    {
        InitializeAds();
    }

    public void InitializeAds()
    {
        #if UNITY_IOS
            _gameId = _iOSGameId;
        #elif UNITY_ANDROID
            _gameId = _androidGameId;
        #elif UNITY_EDITOR
            _gameId = _androidGameId; //Only for testing the functionality in the Editor
        #endif
        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(_gameId, _testMode, this);
        }
    }


    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
        rewardedAdsButton.LoadAd();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }
    */
}