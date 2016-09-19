using System;
using UnityEngine;

using GoogleMobileAds.Api;

public class GoogleMobileAdsManager : MonoBehaviour
{
    [Header("Android ====== ")]
    public string bannerAdsId_Android = "INSERT_ANDROID_BANNER_AD_UNIT_ID_HERE";
    public string interstitalAdsId_Android = "INSERT_ANDROID_INTERSTITIAL_AD_UNIT_ID_HERE";
    public string reward_Android = "INSERT_REWARD_AD_UNIT_ID_HERE";

    public string myDeviceID;    

    BannerView bannerView;
    InterstitialAd interstitial;
    RewardBasedVideoAd rewardBasedVideo;

    public event System.Action<Reward> ApplyRewardByAds;

    public bool debug = true;

    public static GoogleMobileAdsManager instance { get; private set; }

    void Awake()
    {
        instance = this;

#if UNITY_EDITOR || UNITY_STANDALONE
        return;
#endif  
        InitAdmob();
    }

    // =================================================================

    void InitAdmob()
    {
#if UNITY_ANDROID
        //===================================================== Init For InterstitalAds
    #if UNITY_ANDROID
            string adUnitInterstialId = interstitalAdsId_Android;
    #else
            string adUnitInterstialId = "unexpected_platform";
    #endif

            // Initialize an InterstitialAd.
            interstitial = new InterstitialAd(adUnitInterstialId);
		    RequestInterstitial();

        //===================================================== Init For RewardBasedVideoAds
            rewardBasedVideo = RewardBasedVideoAd.Instance;
            RequestRewardVideo();
        //===================================================== Init For BannerAds
    #if UNITY_ANDROID
        string adUnitId = bannerAdsId_Android;
    #else
        string adUnitId = "unexpected_platform";
    #endif
        bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Top);
        //=====================================================
#endif
    }

    AdRequest GetAdRequest()
    {
        // Create an empty ad request.            
        AdRequest.Builder builder = new AdRequest.Builder();
        if (debug)
        {
            builder.AddTestDevice(AdRequest.TestDeviceSimulator);
            
            myDeviceID = GetCurrentDeviceID();
            builder.AddTestDevice(myDeviceID);
            Debug.Log("==== My Device ID =================== " + myDeviceID);
        }

        AdRequest request = builder.Build();
        return request;
    }

    public void RequestInterstitial()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        return;
#endif  
		
        if (requestInterstitialAds == false)
        {
            requestInterstitialAds = true;

            AdRequest request = GetAdRequest();

            // Load the interstitial with the request.
            interstitial.LoadAd(request);
        }
    }

    public void ShowInterstitial()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        return;
#endif         

        if (isInterstitialAdsReady)
        {
            if (interstitial.IsLoaded())
            {
                interstitial.Show();
            }

            requestInterstitialAds = false;
            isInterstitialAdsReady = false;
        }        
    }

    public void RequestSmartBanner()
    {
        AdRequest request = GetAdRequest();

        Debug.Log(bannerView);
        Debug.Log(request);

        // Load the banner with the request.
        bannerView.LoadAd(request);
        bannerView.Show();
    }

    public bool createBanner = false;
    public void ShowSmartBanner()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        return;
#endif
        if (createBanner == false)
        {
            RequestSmartBanner();
            createBanner = true;
        }
        else
        {
            bannerView.Show();
        }
    }

    public void HideSmartBanner()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        return;
#endif
        bannerView.Hide();
    }

    public void RequestRewardVideo()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        return;
#endif

        if (requestRewardVideoAds == false)
        {
            requestRewardVideoAds = true;

            AdRequest request = GetAdRequest();

#if UNITY_ANDROID
            string adReward = reward_Android;
#elif UNITY_IPHONE
            string adReward = reward_iOS;
#else
            string adReward = "unexpected_platform";
#endif

            // Load the interstitial with the request.
            rewardBasedVideo.LoadAd(request, adReward);
        }
    }

    public void ShowRewardVideo()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        return;
#endif         

        if (isRewardVideoAdsReady)
        {
            if (rewardBasedVideo.IsLoaded())
            {
                rewardBasedVideo.Show();
            }

            requestRewardVideoAds = false;
            isRewardVideoAdsReady = false;
        }
    }

    void OnEnable()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        return;
#endif
        if (interstitial != null)
        {
            interstitial.OnAdLoaded += interstitialReceivedAdEvent;
            interstitial.OnAdFailedToLoad += interstitialFailedToReceiveAdEvent;
        }

        if (bannerView != null)
        {
            bannerView.OnAdLoaded += bannerReceivedAdEvent;
        }

        if (rewardBasedVideo != null)
        {
            rewardBasedVideo.OnAdLoaded += HandleRewardBasedVideoLoaded;
            rewardBasedVideo.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;
            rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;
        }
    }

    void OnDisable()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        return;
#endif

        if (interstitial != null)
        {
            interstitial.OnAdLoaded -= interstitialReceivedAdEvent;
            interstitial.OnAdFailedToLoad -= interstitialFailedToReceiveAdEvent;
        }

        if (bannerView != null)
        {
            bannerView.OnAdLoaded -= bannerReceivedAdEvent;
        }

        if (rewardBasedVideo != null)
        {
            rewardBasedVideo.OnAdLoaded -= HandleRewardBasedVideoLoaded;
            rewardBasedVideo.OnAdFailedToLoad -= HandleRewardBasedVideoFailedToLoad;
            rewardBasedVideo.OnAdRewarded -= HandleRewardBasedVideoRewarded;
        }
    }

    bool firstBanner = true;
    void bannerReceivedAdEvent(object sender, EventArgs args)
    {
        //if (firstBanner)
        //{
        //    firstBanner = false;
        //    HideSmartBanner();
        //}
    }

    bool requestInterstitialAds = false;
    bool isInterstitialAdsReady = false;
    void interstitialReceivedAdEvent(object sender, EventArgs args)
    {
        isInterstitialAdsReady = interstitial.IsLoaded();
    }


    void interstitialFailedToReceiveAdEvent(object sender, AdFailedToLoadEventArgs args)
    {
        requestInterstitialAds = false;
    }

    public bool IsReadyInterstial()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        return true;
#endif

        return interstitial.IsLoaded();
    }



    bool requestRewardVideoAds = false;
    bool isRewardVideoAdsReady = false;
    void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
    {
        Debug.Log("====== HandleRewardBasedVideoLoaded "+ " ====" + rewardBasedVideo.IsLoaded());
        isRewardVideoAdsReady = rewardBasedVideo.IsLoaded();
    }


    void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        Debug.Log("====== HandleRewardBasedVideoFailedToLoad " + " =!===" + args.Message);
        requestRewardVideoAds = false;
    }

    public void HandleRewardBasedVideoRewarded(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;

        Debug.Log("==== Load Reward " + type + " = " + amount);
        if (ApplyRewardByAds != null)
            ApplyRewardByAds(args);
    }

    public bool IsReadyRewardVideo()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        return true;
#endif

        return rewardBasedVideo.IsLoaded();
    }

    #region DEVICE_ID   
    public static string GetCurrentDeviceID()
    {
    #if UNITY_ANDROID
        return GetAndroidAdMobID();
    #elif UNITY_IOS
        return GetIOSAdMobID();
    #endif

        return null;        
    }

    #if UNITY_ANDROID
    public static string GetAndroidAdMobID()
    {
        UnityEngine.AndroidJavaClass up = new UnityEngine.AndroidJavaClass("com.unity3d.player.UnityPlayer");
        UnityEngine.AndroidJavaObject currentActivity = up.GetStatic<UnityEngine.AndroidJavaObject>("currentActivity");
        UnityEngine.AndroidJavaObject contentResolver = currentActivity.Call<UnityEngine.AndroidJavaObject>("getContentResolver");
        UnityEngine.AndroidJavaObject secure = new UnityEngine.AndroidJavaObject("android.provider.Settings$Secure");
        string deviceID = secure.CallStatic<string>("getString", contentResolver, "android_id");
        return Md5Sum(deviceID).ToUpper();
    }
    #endif

    #if UNITY_IOS
     public static string GetIOSAdMobID() {
         return Md5Sum(UnityEngine.iOS.Device.advertisingIdentifier);
     }
    #endif

    public static string Md5Sum(string strToEncrypt)
    {
        System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
        byte[] bytes = ue.GetBytes(strToEncrypt);

        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] hashBytes = md5.ComputeHash(bytes);

        string hashString = "";
        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }

        return hashString.PadLeft(32, '0');
    }
#endregion
}
