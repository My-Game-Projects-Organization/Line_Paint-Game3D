using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    public Vector2Int coordinates; // Lưu tọa độ của ô
    public Vector3 coordinatesCenters; // Lưu tọa độ của ô

    public void SetCoordinates(Vector2Int coords)
    {
        coordinates = coords;

    }
    public void SetCoordinatesCenter(Vector3 coordsCenter)
    {
        coordinatesCenters = coordsCenter;

    }
}
