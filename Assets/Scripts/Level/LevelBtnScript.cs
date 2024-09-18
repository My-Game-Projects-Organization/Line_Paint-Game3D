using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelBtnScript : MonoBehaviour
{
    [SerializeField]private GameObject lockObj, unlockObj;
    [SerializeField]private Image[] startsArray;
    [SerializeField]private Text levelIndexText;
    [SerializeField]private Color lockColor, unlockColor;
    [SerializeField]private Button btn;
    [SerializeField]private GameObject activeLevelIndicator;

    private int levelIndex;

    private void Start()
    {
        btn.onClick.AddListener(() => OnClick());
    }

    public void SetLevelButton(LevelScriptableData value, int index, bool activeLevel)
    {
        if (value.unlocked) { 
            activeLevelIndicator.SetActive(activeLevel);
            levelIndex = index + 1;
            btn.interactable = true;
            lockObj.SetActive(false);
            unlockObj.SetActive(true);
            levelIndexText.text = "" + levelIndex;
        }
        else
        {
            btn.interactable = false;
            lockObj.SetActive(true);
            unlockObj.SetActive(false);
        }
    }

    private void OnClick()
    {
        LevelSystemManager.Instance.CurrentLevel = levelIndex - 1;
        PlayerPrefs.SetInt("CurrentLevel", levelIndex - 1);
        SceneManager.LoadScene("GameScene");

    }
}
