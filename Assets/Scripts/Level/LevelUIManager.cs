using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUIManager : MonoBehaviour
{
    [SerializeField] private GameObject leveBtnGridHolder;
    [SerializeField] private LevelBtnScript levelBtnPrefab;
    [SerializeField] private GameObject mainMenuUI, levelMenuUI;
    [SerializeField] private Button startBtn;

    private void Start()
    {
        startBtn.onClick.RemoveAllListeners();
        startBtn.onClick.AddListener(() =>
        {
            mainMenuUI.SetActive(false);
            levelMenuUI.SetActive(true);
            InitializeUI();
        });


    }

    public void InitializeUI()
    {
        List<LevelScriptableData> levelScriptableDatas = LevelSystemManager.Instance.LevelData.levelScriptableDatas;
        for (int i = 0; i < levelScriptableDatas.Count; i++)
        {
            LevelBtnScript levelBtn = Instantiate(levelBtnPrefab, leveBtnGridHolder.transform);
            levelBtn.SetLevelButton(levelScriptableDatas[i], i, i == LevelSystemManager.Instance.LevelData.lastUnlockedLevel);
        }
    }
}
