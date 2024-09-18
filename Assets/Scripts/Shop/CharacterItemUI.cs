using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CharacterItemUI : MonoBehaviour
{
    [SerializeField] Color itemNotSelectedColor;
    [SerializeField] Color itemSelectedColor;

    [SerializeField] Color itemBlinkingColor;
    [SerializeField] Color itemNotBlinkingColor;


    [SerializeField] GameObject unlockObj, lockObj;
    [SerializeField] Image skinImg;
    [SerializeField] Image itemImgOutline;
    [SerializeField] Image imgBlink;

    [Space(20f)]
    [SerializeField] Button itemBtn;

    private float blinkSpeed = 0.2f;
    private float delayStart = 0f;

    private Coroutine blinkCoroutine;
    private bool isBlinking = false;

    public void StartBlinking(float delay)
    {
        delayStart = delay;
        blinkCoroutine = StartCoroutine(BlinkCoroutine());
    }

    IEnumerator BlinkCoroutine()
    {
        yield return new WaitForSeconds(delayStart);
        isBlinking = true;
        while (isBlinking)
        {
            imgBlink.color = itemBlinkingColor;
            yield return new WaitForSeconds(blinkSpeed);
            imgBlink.color = itemNotBlinkingColor;
            yield return new WaitForSeconds(blinkSpeed);
        }
    }

    public void StopBlinking()
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(BlinkCoroutine());
        }
        isBlinking = false;
    }

    public void SetCharacterImage(Sprite sprite)
    {
        skinImg.sprite = sprite;
    }

    public void SetCharacterAsPurchased()
    {
        //TODO: Change item Color
        itemBtn.interactable = true;
    }

    public void SetStateSkinCharacter(bool isUnlocked)
    {
        if (isUnlocked)
        {
            unlockObj.SetActive(true);
            lockObj.SetActive(false);
            itemBtn.interactable = true;
        }
        else
        {
            unlockObj.SetActive(false);
            lockObj.SetActive(true);
            itemBtn.interactable = false;
        }
    }

    public void OnItemSelect(int itemIndex, UnityAction<int> action)
    {
        itemBtn.interactable = true;
        itemBtn.onClick.RemoveAllListeners();
        itemBtn.onClick.AddListener(() => action.Invoke(itemIndex));    
    }

    public void SelectItem()
    {
        itemImgOutline.color = itemSelectedColor;
        itemBtn.interactable = false;
    }
    public void DeSelectItem()
    {
        itemImgOutline.color = itemNotSelectedColor;
        itemBtn.interactable = true;
    }
}
