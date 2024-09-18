using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinePaintScript : MonoBehaviour
{
    [SerializeField]private LineRenderer lineRenderer;

    private Vector2Int _startCoords, _endCoords;

    public Vector2Int EndCoords { get => _endCoords; }
    public Vector2Int StartCoords { get => _startCoords;}

    public void SetConnectionCoords(Vector2Int startCoord, Vector2Int endCoord)
    {
        _startCoords = startCoord;
        _endCoords = endCoord;
    }

    public void SetRendererPosition(Vector3 startPos, Vector3 endPos, Color color)
    {
        lineRenderer.material.color = color;    
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0,startPos);
        lineRenderer.SetPosition(1,endPos);
    }
}
