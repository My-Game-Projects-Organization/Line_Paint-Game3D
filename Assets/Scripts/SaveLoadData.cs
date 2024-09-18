using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;


public class SaveLoadData : MonoBehaviour
{
    private static SaveLoadData instance;

    public static SaveLoadData Instance {  get { return instance; } }

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

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveData();
        }
    }
    private void OnApplicationQuit()
    {
        SaveData();
    }

    private bool CheckFirstTimeStartGame()
    {
        foreach(LevelScriptableData level in LevelSystemManager.Instance.LevelData.levelScriptableDatas)
        {
            if(level == null)
            {
                return false;
            }
        }
        return true;
    }

    public void Initialize()
    {
        if(PlayerPrefs.GetInt("GameStartFirstTime")  == 1)
        {
            Debug.Log("State 1");
            LoadData();
        }
        else
        {
            Debug.Log("State 0");
            GetAllLevelDataFromResources();
            SaveData();
            PlayerPrefs.SetInt("GameStartFirstTime", 1);
        }
    }

    public void SaveData()
    {
        string levelDataString = 
        JsonConvert.SerializeObject(LevelSystemManager.Instance.LevelData, Formatting.Indented, new Vector2IntJsonConverter());
        try
        {
            System.IO.File.WriteAllText(Application.persistentDataPath + "/LevelData.json", levelDataString);
            GetAllStateLevelData();
            Debug.Log("<color=green>[Level Data] Saved.</color>");
        }
        catch(System.Exception e) 
        {
            Debug.Log("Error Saving data" + e);
            throw;
        }
    }
    public void GetAllLevelDataFromResources()
    {
        LevelScriptableData[] resourcesLevel = Resources.LoadAll<LevelScriptableData>("");
        LevelScriptableData[] resourcesLevelAfterOrder = resourcesLevel.OrderBy(level => ExtractLevelNumber(level.name)).ToArray();
        //foreach (var level in resourcesLevelAfterOrder)
        //{
        //    Debug.Log(level.name);
        //}
        if (resourcesLevelAfterOrder != null && resourcesLevelAfterOrder.Length > 0)
        {
            int count = 0;
            foreach (LevelScriptableData resource in resourcesLevelAfterOrder)
            {
                // Check the type if necessary
                if (resource is LevelScriptableData)
                {
                    count++;
                    Debug.Log(count + " - " + resource.unlocked);
                }
            }
            LevelSystemManager.Instance.InitLevelData();
            LevelSystemManager.Instance.LevelData.levelScriptableDatas = new List<LevelScriptableData>(resourcesLevelAfterOrder);

            Debug.Log("Number of ScriptableObjects In Resource: " + count);
        }
        else
        {
            Debug.Log("No ScriptableObjects found in Resources");
        }
    }
    // Phương pháp để trích xuất số từ tên level
    private int ExtractLevelNumber(string name)
    {
        // Sử dụng biểu thức chính quy để tìm số trong tên
        var match = Regex.Match(name, @"\d+");
        return match.Success ? int.Parse(match.Value) : 0;
    }
    void GetAllStateLevelData()
    {
        List<LevelScriptableData> list = LevelSystemManager.Instance.LevelData.levelScriptableDatas;
        if (list != null && list.Count > 0)
        {
            int count = 0;
            foreach (LevelScriptableData resource in list)
            {
                // Check the type if necessary
                if (resource is LevelScriptableData)
                {
                    count++;
                    Debug.Log(count + " - " + resource.unlocked);
                }
            }
            Debug.Log("Number of ScriptableObjects In Resource: " + count);
        }
        else
        {
            Debug.Log("No ScriptableObjects found in Resources");
        }
    }
    void GetAllStateLevelDataByListExist(List<LevelScriptableData> listLevelExist)
    {
        List<LevelScriptableData> list = LevelSystemManager.Instance.LevelData.levelScriptableDatas;
        if (list != null && list.Count > 0)
        {
            int count = 0;
            foreach (LevelScriptableData resource in list)
            {
                // Check the type if necessary
                if (resource is LevelScriptableData)
                {
                    count++;
                    Debug.Log(count + " - " + resource.unlocked);
                }
            }
            Debug.Log("Number of ScriptableObjects In Resource: " + count);
        }
        else
        {
            Debug.Log("No ScriptableObjects found in Resources");
        }
    }

    private void LoadData()
    {
        try
        {
            string levelDataString = System.IO.File.ReadAllText(Application.persistentDataPath + "/LevelData.json");
            Debug.Log("Log json: " +levelDataString);
            LevelData levelData = JsonConvert.DeserializeObject<LevelData>(levelDataString, new Vector2IntJsonConverter());
            if (levelData != null)
            {
                for (int i = 0; i < levelData.levelScriptableDatas.Count; i++)
                {
                    if (i > levelData.lastUnlockedLevel)
                        break;
                    else
                        levelData.levelScriptableDatas[i].unlocked = true;

                }
                LevelSystemManager.Instance.InitLevelData();
                LevelSystemManager.Instance.LevelData.levelScriptableDatas = new List<LevelScriptableData>(levelData.levelScriptableDatas);
                GetAllStateLevelDataByListExist(levelData.levelScriptableDatas);
                LevelSystemManager.Instance.LevelData.lastUnlockedLevel = levelData.lastUnlockedLevel;
                Debug.Log(LevelSystemManager.Instance.LevelData.lastUnlockedLevel + "");
            }
            Debug.Log("<color=green>[Level Data] Loaded.</color>");
            GetAllStateLevelData();
        }
        catch (MissingReferenceException e)
        {
            Debug.Log("Error Loading Data" + e);
        }catch (NullReferenceException e1)
        {
            Debug.Log("Level is null" + e1);
        }
    }

    public void ClearData()
    {
        Debug.Log("Data Cleared");
        GetAllLevelDataFromResources();
        LevelSystemManager.Instance.LevelData.lastUnlockedLevel = 0;
        for (int i = 1; i < LevelSystemManager.Instance.LevelData.levelScriptableDatas.Count; i++) {
            LevelSystemManager.Instance.LevelData.levelScriptableDatas[i].unlocked = false;
        }
        SaveData();
        PlayerPrefs.SetInt("GameStartFirstTime", 0);
        //PlayerPrefs.SetInt(PrefKey.TotalDiamonds.ToString(), 0);
        GameDataManager.SpendDiamonds(GameDataManager.GetDiamonds());
    }
}
