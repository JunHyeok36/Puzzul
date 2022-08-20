using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds.Api;

public class AdmobManager : MonoBehaviour
{
    static AdmobManager instance = null;
	public static AdmobManager Instance {
		get {
			if(instance == null){
				instance = (AdmobManager)FindObjectOfType(typeof(AdmobManager));
			}
			return instance;
		}
	}

    bool isTestMode = false;
    bool getReward = false, rewardAvailable = false;

    public delegate void RewardMethod();
    RewardMethod rewardMethod;
    
    void Start() {
        var requestConfiguration = new RequestConfiguration
            .Builder()
            .build();

        MobileAds.SetRequestConfiguration(requestConfiguration);

        LoadFrontAd();
        LoadRewardAd();
    }

    AdRequest GetAdRequest() {
        return new AdRequest.Builder().Build();
    }

    #region BannerAD

    const string bannerTestID = "ca-app-pub-3940256099942544/6300978111";
    const string bannerID = "";
    BannerView bannerAd;

    void LoadBannerAd() {
        bannerAd = new BannerView(isTestMode ? bannerTestID : bannerID, AdSize.SmartBanner, AdPosition.Bottom);
        bannerAd.LoadAd(GetAdRequest());
        ToggleBannerAd(false);
    }

    public void ToggleBannerAd(bool b) {
        if (b) bannerAd.Show();
        else bannerAd.Hide();
    }

    #endregion

    #region FrontAD

    const string frontTestID = "ca-app-pub-3940256099942544/1033173712";
    const string frontID = "ca-app-pub-4753604301213739/9169794929";
    InterstitialAd frontAd;


    void LoadFrontAd() {
        frontAd = new InterstitialAd(isTestMode ? frontTestID : frontID);
        frontAd.LoadAd(GetAdRequest());
        frontAd.OnAdLoaded += HandleRewardedAdLoaded;
        frontAd.OnAdClosed += (sender, e) => {
            ListManager.Instance.ContinueBGM();
            rewardMethod();
        };
    }

    public void ShowFrontAd(RewardMethod rewardMethod) {
        this.rewardMethod = rewardMethod;
        
        frontAd.Show();
        LoadFrontAd();
    }

    #endregion

    #region RewardAD

    const string rewardTestID = "ca-app-pub-3940256099942544/5224354917";
    const string rewardID = "ca-app-pub-4753604301213739/9077058486";
    bool endVideo = false;
    bool curVideoCompleteReward = false;
    RewardedAd rewardAd;

    void LoadRewardAd() {
        rewardAd = new RewardedAd(isTestMode ? rewardTestID : rewardID);
        rewardAd.LoadAd(GetAdRequest());
        rewardAd.OnAdLoaded += HandleRewardedAdLoaded;
        rewardAd.OnUserEarnedReward += HandleUserEarnedReward;
        rewardAd.OnAdClosed += HandleRewardedAdClosed;
    }

    public void ShowRewardAd(RewardMethod rewardMethod) {
        this.rewardMethod = rewardMethod;

        rewardAd.Show();
    }

    void Update()
    {
        if(endVideo) {
            endVideo = false;
            if(curVideoCompleteReward) {
                rewardMethod();
                rewardMethod = null;
                curVideoCompleteReward = false;
            }
        }
    }

    private void HandleRewardedAdLoaded(object sender, EventArgs args) {
        ListManager.Instance.StopBGM();
    }

    private void HandleUserEarnedReward(object sender, Reward args)
    {
        curVideoCompleteReward = true;
    }

    private void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        LoadRewardAd();
        endVideo = true;
        ListManager.Instance.ContinueBGM();
    }

    #endregion
}