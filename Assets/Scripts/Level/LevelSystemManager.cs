using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSystemManager : MonoBehaviour
{
    private static LevelSystemManager instance;

    private LevelData levelData;

    private int currentLevel;
    public static LevelSystemManager Instance { get => instance;}
    public int CurrentLevel { get => currentLevel; set => currentLevel = value; }
    public LevelData LevelData { get => levelData;}

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        List<LevelScriptableData> list_LevelData = LevelData.levelScriptableDatas;
        if(list_LevelData == null || list_LevelData.Count == 0)
        {
            Debug.Log("Level đã lỗi");
            SaveLoadData.Instance.GetAllLevelDataFromResources();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SaveLoadData.Instance.SaveData();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            SaveLoadData.Instance.ClearData();
        }
    }

    private void OnEnable()
    {
        SaveLoadData.Instance.Initialize();
    }
    public void InitLevelData()
    {
        levelData = new LevelData();
        levelData.lastUnlockedLevel = 0;
        levelData.levelScriptableDatas = new List<LevelScriptableData>();
    }
    //public void LevelComplete(int startAchieved)
    //{
    //    //levelData.levelItemArray[currentLevel].startAchieved = startAchieved;
    //    if(levelData.lastUnlockedLevel < (currentLevel + 1))
    //    {
    //        levelData.lastUnlockedLevel = currentLevel + 1;
    //        //levelData.levelItemArray[levelData.lastUnlockedLevel].unlocked = true;
    //        levelData.levelScriptableDatas[levelData.lastUnlockedLevel].unlocked = true;
    //    }

    //}


    public void LevelCompleteNew(int curLevel)
    {
        if (LevelData.lastUnlockedLevel < (curLevel + 1))
        {
            LevelData.lastUnlockedLevel = curLevel + 1;
            if (LevelData.lastUnlockedLevel > LevelData.levelScriptableDatas.Count - 1)
                LevelData.lastUnlockedLevel = 0;
            LevelData.levelScriptableDatas[LevelData.lastUnlockedLevel].unlocked = true;
        }
        SaveLoadData.Instance.SaveData();
    }
}
