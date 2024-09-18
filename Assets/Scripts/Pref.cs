using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pref
{
     public static int totalDiamond
    {
        set
        {
            if(value >= 0)
            {
                PlayerPrefs.SetInt(PrefKey.TotalDiamonds.ToString(), value);
            }
        }
        get => PlayerPrefs.GetInt(PrefKey.TotalDiamonds.ToString(),0);
    }
}
