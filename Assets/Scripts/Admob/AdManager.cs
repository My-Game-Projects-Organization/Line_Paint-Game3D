using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdManager : MonoBehaviour
{
    private RewardedAd _rewardedAd;

    public RewardedAd GetRewardAd
    {

    get { return _rewardedAd; } }

    private void Start()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            // This callback is called once the MobileAds SDK is initialized.
        });


    }

    // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
    private string _adUnitId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
  private string _adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
  private string _adUnitId = "unused";
#endif

    private void HandleAdClosed()
    {
        // Hủy quảng cáo và giải phóng tài nguyên
        _rewardedAd.Destroy();
        _rewardedAd = null;
        Debug.Log("Quảng cáo đã đóng.");
    }
    public void ShowRewardedAd(int typeOfBtnShowAd)
    {
        //const string rewardMsg =  "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            _rewardedAd.Show((Reward reward) =>
            {
                // TODO: Reward the user.
                //Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
                if(typeOfBtnShowAd == 0)
                {
                    LevelManager lvManageObj = GameObject.Find("LevelManager").GetComponent<LevelManager>();
                    if (lvManageObj != null)
                    {
                        lvManageObj.CompleteLevel();
                    }
                    Debug.Log("Skip Level!");
                }else if( typeOfBtnShowAd == 1)
                {
                    GameDataManager.AddDiamonds(100);
                    UIManager uiManageObj = GameObject.Find("UIManager").GetComponent<UIManager>();
                    if (uiManageObj != null)
                    {
                        uiManageObj.UpdateDiamondText();
                    }
                    Debug.Log("You get 100 diamond");
                }
               
            });
        }
    }
    
    public void LoadRewardedAd()
    {
        // Clean up the old ad before loading a new one.
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        Debug.Log("Loading the rewarded ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

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

                _rewardedAd = ad;
            });
    }
}
