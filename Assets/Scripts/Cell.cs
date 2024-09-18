using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField] private MeshRenderer cellCenter;
    [HideInInspector] private Vector2Int coords;

    public MeshRenderer CellCenter { get => cellCenter;}
    public Vector2Int Coords { get => coords; set => coords = value; }
}
