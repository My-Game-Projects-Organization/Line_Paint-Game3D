using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private Image[] startsArray;
    [SerializeField] private Color lockColor, unlockColor;
    [SerializeField] private Text levelStatusText;
    [SerializeField] private GameObject overPanel;

    public void GameOver(int startCount)
    {
        if(startCount > 0)
        {
            levelStatusText.text = "Level " + (LevelSystemManager.Instance.CurrentLevel + 1) + " Completed";
            //LevelSystemManager.Instance.LevelComplete();
        }
        else
        {
            levelStatusText.text = "Level " + (LevelSystemManager.Instance.CurrentLevel + 1) + " Failed";
        }
        //SetStart(startCount);
        //overPanel.SetActive(true);
    }

    public void ObBtn()
    {
        SceneManager.LoadScene(0);
    }

    private void SetStart(int startAchieved)
    {
        for (int i = 0; i < startsArray.Length; i++)
        {
            if (i < startAchieved)
            {
                startsArray[i].color = unlockColor;
            }
            else
            {
                startsArray[i].color = lockColor;
            }
        }
    }
}
