using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class CharacterShopUI : MonoBehaviour
{
    [SerializeField] Color enabledButtonColor;
    [SerializeField] Color enabledIconColor;
    [SerializeField] Color disabledButtonColor;

    [Space (20f)]
    [Header("UI elements")]
    [SerializeField] GameObject itemPrefab;
    [SerializeField] Transform shopItemBtnGridHolder;

    [Space(20f)]
    [Header("Shop Events")]
    [SerializeField] GameObject shopUI;
    [SerializeField] Button closeShopBtn;
    [SerializeField] Button unlockItemBtn;
    [SerializeField] GameObject diamondIconUnlockBtn;
    [SerializeField] Button earnDiamondsBtn;
    [SerializeField] Text priceTextToSpin;
    [SerializeField] UIManager uiManager;

    [Space(20f)]
    [SerializeField] CharacterShopDatabase characterDB;

    int newSelectedItemIndex = 0;
    int previousSelectedItemIndex = 0;

    private List <CharacterItemUI> itemUIArrays = new List<CharacterItemUI>();


    private void Start()
    {
        AddShopEvents();

        GenerateShopItemUI();

        //Set selected character in the playerDataManager
        SetSelectedCharacter();

        //Select UI item
        SelectItemUI(GameDataManager.GetSelectedCharacterIndex());

        UpdateStateUnlockBtn();
    }

    private void Update()
    {
        ReStart();
    }

    void SetSelectedCharacter()
    {
        //Get save index
        int index = GameDataManager.GetSelectedCharacterIndex();

        //Set selected character
        GameDataManager.SetSelectedCharacter(characterDB.GetCharacter(index),index);
    }

    void GenerateShopItemUI()
    {
        // Loop throw save purchased character items and make them as purchased in the database arrays
        for(int i = 0; i< GameDataManager.GetAllPurchasedCharacter().Count; i++)
        {
            int purchasedCharacterIndex = GameDataManager.GetPurchasedCharacter(i);
            characterDB.PurchaseCharacter(purchasedCharacterIndex);
        }

        Transform firstItem = shopItemBtnGridHolder.transform.Find("LevelButton1");
        if(firstItem != null)
        {
            Destroy(firstItem.gameObject);
        }
        Destroy(shopItemBtnGridHolder.GetChild(0).gameObject);
        shopItemBtnGridHolder.DetachChildren();

        // Generate Item
        for (int i = 0; i < characterDB.CharactersCount; i++)
        {
            Character character = characterDB.GetCharacter(i);
            CharacterItemUI uiItem = Instantiate(itemPrefab, shopItemBtnGridHolder).GetComponent<CharacterItemUI>();

            uiItem.gameObject.name = "item " + i;
            uiItem.SetCharacterImage(character.characterImg);
            uiItem.SetStateSkinCharacter(character.isPurchased);
            if (character.isPurchased)
            {
                // Character is Unlocked
                uiItem.OnItemSelect(i, OnItemSelected);
            }
            else
            {
                itemUIArrays.Add(uiItem);
            }

        }
    }

    private void OnItemSelected(int index)
    {
        //Select item in the UI
        SelectItemUI(index);

        //Save Data
        GameDataManager.SetSelectedCharacter(characterDB.GetCharacter(index), index);
    }

    private void OnItemUnlocked(int index)
    {
        Character character = characterDB.GetCharacter(index);
        CharacterItemUI uiItem = GetItem(index);

        //Proceed with the purchase operation
        GameDataManager.SpendDiamonds(GameDataManager.GetDiamondNeedToSpin());

        //Update Coins UI Text
        //GameSharedUI.Instance.UpdateCoinsUIText();
        uiManager.UpdateDiamondText();

        characterDB.PurchaseCharacter(index);

        uiItem.SetCharacterAsPurchased();
        uiItem.OnItemSelect(index, OnItemSelected);

        //Add purchased item to Shop Data
        GameDataManager.AddPurchasedCharacter(index);

        GameDataManager.IncrementDiamond();
        UpdateStateUnlockBtn();

    }

    private void UpdateStateUnlockBtn()
    {
        if (GameDataManager.CanSpendDiamonds(GameDataManager.GetDiamondNeedToSpin()))
        {
            unlockItemBtn.interactable = true;
            unlockItemBtn.GetComponent<Image>().color = enabledButtonColor;
            diamondIconUnlockBtn.GetComponent<Image>().color = enabledIconColor;
        }
        else
        {
            unlockItemBtn.interactable = false;
            unlockItemBtn.GetComponent<Image>().color = disabledButtonColor;
            diamondIconUnlockBtn.GetComponent<Image>().color = disabledButtonColor;
        }

        priceTextToSpin.text = GameDataManager.GetDiamondNeedToSpin().ToString();
    }

    void SelectItemUI(int index)
    {
        previousSelectedItemIndex = newSelectedItemIndex;
        newSelectedItemIndex = index;

        CharacterItemUI newItemUI = GetItem(newSelectedItemIndex);
        CharacterItemUI previousItemUI = GetItem(previousSelectedItemIndex);

        previousItemUI.DeSelectItem();
        Debug.Log("deselect " + previousSelectedItemIndex);
        newItemUI.SelectItem();
        Debug.Log("select " + newSelectedItemIndex);

        
    }

    private CharacterItemUI GetItem(int index)
    {
        return shopItemBtnGridHolder.GetChild(index).GetComponent<CharacterItemUI>();
    }

    void AddShopEvents()
    {
        closeShopBtn.onClick.RemoveAllListeners();
        closeShopBtn.onClick.AddListener(CloseShop);

        unlockItemBtn.onClick.RemoveAllListeners();
        unlockItemBtn.onClick.AddListener(SpinItem);

        earnDiamondsBtn.onClick.RemoveAllListeners();
        earnDiamondsBtn.onClick.AddListener(EarnDiamondByAdmob);
    }

    private void EarnDiamondByAdmob()
    {
        AdManager adManager = GameObject.Find("AdManager").GetComponent<AdManager>();
        if (adManager != null)
        {
            adManager.LoadRewardedAd();
            adManager.ShowRewardedAd(1);
            if (adManager.GetRewardAd != null)
            {
                adManager.GetRewardAd.Destroy();
            }
            uiManager.UpdateDiamondText();
        }
        
    }

    void SpinItem()
    {
        StartCoroutine(SpinningCoroutine());
    }

    void CloseShop()
    {
        shopUI.gameObject.SetActive(false);
        uiManager.UpdateDiamondText();
        LevelManager lvManageObj = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        if(lvManageObj != null )
        {
            lvManageObj.CheckExistBrushCoord();
            lvManageObj.InitBrushCoord();
            lvManageObj.CheckOnShopState = false;
            lvManageObj.swipeController.OnShopPanelToggle(false);
        }

        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator SpinningCoroutine()
    {
        float delayIncrement = 0.2f;
        for(int i = 0; i < itemUIArrays.Count; i++)
        {
            itemUIArrays[i].StartBlinking(delayIncrement * i);
        }

        yield return new WaitForSeconds(1.5f); // Thời gian quay

        // Chọn ngẫu nhiên một ô để dừng
        int randomIndex = UnityEngine.Random.Range(0, itemUIArrays.Count);
        for (int i = 0; i < itemUIArrays.Count; i++)
        {
            itemUIArrays[i].StopBlinking();
        }
        Debug.Log(randomIndex + "index random");
        // Hiển thị hình ảnh của ô đó
        itemUIArrays[randomIndex].SetStateSkinCharacter(true); // Hoặc thay đổi sprite
        int index = GetItemIndex(itemUIArrays[randomIndex].name);
        if(index >= 0)
        {
            OnItemUnlocked(index);
        }
        else
        {
            Debug.Log("Lỗi spin");
        }
    }
    public static int GetItemIndex(string itemName)
    {
        // Loại bỏ phần "item " khỏi đầu tên
        string numberString = itemName.Replace("item ", "");

        // Kiểm tra xem phần còn lại có phải là một số nguyên không
        if (int.TryParse(numberString, out int itemIndex))
        {
            return itemIndex;
        }
        else
        {
            Debug.Log("Tên GameObject không hợp lệ: " + itemName);
            return -1; // Trả về -1 nếu không thể chuyển đổi
        }
    }

    void ReStart()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            // Loop throw save purchased character items and make them as purchased in the database arrays
            for (int i = 0; i < GameDataManager.GetAllPurchasedCharacter().Count; i++)
            {
                int purchasedCharacterIndex = GameDataManager.GetPurchasedCharacter(i);
                characterDB.UnPurchaseCharacter(purchasedCharacterIndex);
            }
            GameDataManager.GetAllPurchasedCharacter().Clear();
            GameDataManager.AddPurchasedCharacter(0);
            GameDataManager.ResetDiamondNeedToSpin();
            GameDataManager.SetSelectedCharacter(characterDB.GetCharacter(0), 0);
        }
    }
}
