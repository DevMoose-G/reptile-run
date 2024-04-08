using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;

public static class AdsManager
{
    public static RewardedAd rewardedAd { get; set; }
    public static InterstitialAd interstitialAd { get; set; }

    public static float AD_TIMER = 3.0f * 60.0f; // required interstitial ad
    public static float timeTillAd = AD_TIMER;

    public static float AD_OFFER_TIMER = 0.4f * 60.0f;
    public static float timeTillAdOffer = AD_OFFER_TIMER;

}
