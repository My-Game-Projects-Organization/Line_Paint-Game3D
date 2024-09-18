using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.MemoryProfiler;
using UnityEngine;

[System.Serializable]
public class Conection 
{
    [JsonConverter(typeof(Vector2IntJsonConverter))]
    public Vector2Int startCoord;
    [JsonConverter(typeof(Vector2IntJsonConverter))]
    public Vector2Int endCoord;

    public Conection(Vector2Int startCoord, Vector2Int endCoord)
    {
        this.startCoord = startCoord;
        this.endCoord = endCoord;
    }

    public override string ToString()
    {
        return "Start (X: " + startCoord.x + " Y: " + startCoord.y + ") End (X: " + endCoord.x + " Y: " + endCoord.y + ")";
    }
}
