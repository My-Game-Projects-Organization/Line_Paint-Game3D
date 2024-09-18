using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/Creadte LevelData", order = 1)]
public class LevelScriptableData : ScriptableObject
{
    public int width, height;
    public Vector2Int brushStartCoords;
    public List<Conection> completePattern = new List<Conection>();
    public bool unlocked;

    public static LevelScriptableData CreateInstance(int width, int height, Vector2Int brushStartCoords, List<Conection> completePattern)
    {
        LevelScriptableData instance = ScriptableObject.CreateInstance<LevelScriptableData>();

        instance.width = width;
        instance.height = height;
        instance.brushStartCoords = brushStartCoords;
        instance.completePattern = completePattern;

        return instance;
    }
}
