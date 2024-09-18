using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [SerializeField]private Text levelText, totalDiamondInGame, totalDiamondInShop, diamondEarned;
    [SerializeField]private GameObject mainMenu, completeMenu, extraBtnHolder, soundBtnOff, commonInfo, shopUI;
    [SerializeField] private Button nextBtn, retryBtn, settingsBtn, soundBtn, vibrationBtn, openShopBtn, SkipLevelBtn;

    public Text LevelText { get => levelText; }
    public Text TotalDiamondInGame { get => totalDiamondInGame;}
    public Text TotalDiamondInShop { get => totalDiamondInShop;}
    public GameObject ShopUI { get => shopUI; }

    private void Start()
    {
        soundBtnOff.SetActive(PlayerPrefs.GetInt("SoundOn",1) == 0 ? true : false);
        AudioListener.volume = PlayerPrefs.GetInt("SoundOn", 1);

        nextBtn.onClick.AddListener(() => OnClick(nextBtn));
        retryBtn.onClick.AddListener(() => OnClick(retryBtn));
        settingsBtn.onClick.AddListener(() => OnClick(settingsBtn));
        soundBtn.onClick.AddListener(() => OnClick(soundBtn));
        openShopBtn.onClick.AddListener(() => OnClick(openShopBtn));
        SkipLevelBtn.onClick.AddListener(() => OnClick(SkipLevelBtn));  
    }


    private void OnClick(Button btn)
    {
        SoudManager.Ins.PlayFx(FxType.Button);
        switch (btn.name)
        {
            case "NextButton":
            case "RetryButton":
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                ChangeStateButton(false);
                break;
            case "SettingsBtn":
                extraBtnHolder.SetActive(!extraBtnHolder.activeInHierarchy);
                break;
            case "SoundBtn":
                PlayerPrefs.SetInt("SoundOn", PlayerPrefs.GetInt("SoundOn", 1) == 0 ? 1 : 0);
                soundBtnOff.SetActive(PlayerPrefs.GetInt("SoundOn",1) == 0 ? true : false);
                break;
            case "ShopBtn":
                ShopUI.SetActive(true);
                LevelManager lvManageObj = GameObject.Find("LevelManager").GetComponent<LevelManager>();
                if (lvManageObj != null)
                {
                    lvManageObj.CheckOnShopState = true;
                    lvManageObj.swipeController.OnShopPanelToggle(true);
                }
                break;
            case "SkipLevelBtn":
                AdManager adManager = GameObject.Find("AdManager").GetComponent<AdManager>();
                if(adManager != null)
                {
                    adManager.LoadRewardedAd();
                    adManager.ShowRewardedAd(0);
                    if (adManager.GetRewardAd != null)
                    {
                        adManager.GetRewardAd.Destroy();
                    }
                }
                break;
        }
    }

    public bool CheckStateShopBtn()
    {
        return openShopBtn.IsActive() ? true : false;
    }

    public void ChangeStateButton(bool active)
    {
        retryBtn.gameObject.SetActive(active);
        openShopBtn.gameObject.SetActive(!active);
    }

    public void ShowMenu()
    {
        mainMenu.SetActive(true);
        commonInfo.SetActive(true);
    }

    public void levelComplete()
    {
        mainMenu.SetActive(false);
        completeMenu.SetActive(true);
        commonInfo.SetActive(false);

       // totalDiamond.text = GameManager.totalDiamonds + "";
    }

    public void UpdateDiamondText()
    {
        totalDiamondInGame.text = GameDataManager.GetDiamonds().ToString();
        totalDiamondInShop.text = GameDataManager.GetDiamonds().ToString();
    }
}
