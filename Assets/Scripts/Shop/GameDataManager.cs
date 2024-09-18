using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Shop Data Holder
[System.Serializable]
public class CharacterShopData
{
    public List<int> purchasedCharactersIndexes = new List<int>();
}

//Player Data Holder
[System.Serializable]
public class PlayerData
{
    public int diamonds = 0;
    public int selectedCharacterInedx = 0;
    public int diamondNeedToSpin = 500;
}
public static class GameDataManager
{
    static PlayerData playerData = new PlayerData();
    static CharacterShopData characterShopData = new CharacterShopData();
    static Character selectedCharacter;
    
    static GameDataManager()
    {
        LoadPlayerData();
        LoadCharacterShopData();
    }

    //PlayerData methods
    public static Character GetSelectedCharacter()
    {
        return selectedCharacter;
    }
    public static int GetSelectedCharacterIndex()
    {
        return playerData.selectedCharacterInedx;
    }

    public static void SetSelectedCharacter(Character character, int index)
    {
        selectedCharacter = character;
        playerData.selectedCharacterInedx = index;
        SavePlayerData();
    }
    public static int GetDiamondNeedToSpin()
    {
        return playerData.diamondNeedToSpin;
    }
    public static void ResetDiamondNeedToSpin()
    {
        playerData.diamondNeedToSpin = 500;
    }
    public static int GetDiamonds()
    {
        return playerData.diamonds;
    }
    public static void AddDiamonds(int amount)
    {
        playerData.diamonds += amount;
        SavePlayerData();
    }
    public static void IncrementDiamond()
    {
        playerData.diamondNeedToSpin += 500;
        SavePlayerData();
    }

    public static bool CanSpendDiamonds(int amount)
    {
        return (playerData.diamonds >= amount);
    }
    public static void SpendDiamonds(int amount)
    {
        playerData.diamonds -= amount;
        SavePlayerData();
    }
    static void SavePlayerData()
    {
        string playerDataString = JsonUtility.ToJson(playerData);

        try
        {
            System.IO.File.WriteAllText(Application.persistentDataPath + "/PlayerData.json", playerDataString);
            Debug.Log("<color=green>[PlayerData] Saved.</color>");
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }
    static void LoadPlayerData()
    {
        try
        {
            Debug.Log(Application.persistentDataPath);
            string playerDataString = System.IO.File.ReadAllText(Application.persistentDataPath + "/PlayerData.json");
            UnityEngine.Debug.Log("<color=green>[PlayerData] Loaded.</color>");
            playerData = JsonUtility.FromJson<PlayerData>(playerDataString);
            if (playerData == null)
            {
                throw new MissingReferenceException("Data is null");
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }

    }

    //Characters Shop Data Methods
    public static void AddPurchasedCharacter(int characterIndex)
    {
        characterShopData.purchasedCharactersIndexes.Add(characterIndex);
        SaveCharacterShopData();
    }

    public static List<int> GetAllPurchasedCharacter()
    {
        return characterShopData.purchasedCharactersIndexes;
    }
    
    public static int GetPurchasedCharacter(int index)
    {
        return characterShopData.purchasedCharactersIndexes[index];
    }
    public static void SaveCharacterShopData()
    {
        string characterShopDataString = JsonUtility.ToJson(characterShopData);

        try
        {
            System.IO.File.WriteAllText(Application.persistentDataPath + "/CharacterShopData.json", characterShopDataString);
            Debug.Log("<color=green>[CharacterShopData] Saved.</color>");
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    static void LoadCharacterShopData()
    {
        try
        {
            Debug.Log(Application.persistentDataPath);
            string characterShopDataString = System.IO.File.ReadAllText(Application.persistentDataPath + "/CharacterShopData.json");
            UnityEngine.Debug.Log("<color=green>[CharacterShopData] Loaded.</color>");
            characterShopData = JsonUtility.FromJson<CharacterShopData>(characterShopDataString);
            if (characterShopData == null)
            {
                throw new MissingReferenceException("Data is null");
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }

    }
}
