using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    public static GameState gameState;
    public static int currentLevel = 0;
    public static int totalDiamonds;

}

public enum GameState
{
    Playing,
    Stop,
    Complete
}

public enum PrefKey
{
    TotalDiamonds
}
