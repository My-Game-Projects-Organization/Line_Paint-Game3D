using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    [SerializeField] private Color[] colors;
    [SerializeField] private Material cubeMat;
    [SerializeField] private CameraZoom gameCamera;
    [SerializeField] private CameraZoom solutionCamera;
    [SerializeField] private Cell blockPrefabs;
    [SerializeField] private BrushController brush;
    [SerializeField] private LinePaintScript linePaintPrefab;
    [SerializeField] private float cellSize;
    [SerializeField] private List<LevelScriptableData> levelScriptables;
    [SerializeField] private Vector3 gridOriginPos;
    [SerializeField] private UIManager uiManager;

    private int width;
    private int height;
    private Cell[,] cells;
    private Grid grid;
    public SwipeController swipeController;
    private BrushController currentBrush;
    private List<Conection> inProgress = new List<Conection>();
    private List<LinePaintScript> connectedLinePaint = new List<LinePaintScript>();

    private bool checkOnShopState = false;

    public bool CheckOnShopState { get => checkOnShopState; set => checkOnShopState = value; }

    private void Start()
    {
        levelScriptables = new List<LevelScriptableData>(LevelSystemManager.Instance.LevelData.levelScriptableDatas);
        if(levelScriptables.Count <= 0 || levelScriptables == null)
        {
            LevelScriptableData[] resourcesLevel = Resources.LoadAll<LevelScriptableData>("");

            if (resourcesLevel != null && resourcesLevel.Length > 0)
            {
                levelScriptables = new List<LevelScriptableData>(resourcesLevel);
            }
            else
            {
                Debug.Log("No ScriptableObjects found in Resources");
            }
        }

        GameManager.gameState = GameState.Playing;

        GameManager.currentLevel = PlayerPrefs.GetInt("CurrentLevel", 0);
        //uiManager.TotalDiamond.text = PlayerPrefs.GetInt(PrefKey.TotalDiamonds.ToString(), 0) + "";
        uiManager.UpdateDiamondText();
        uiManager.LevelText.text = "Level " + (GameManager.currentLevel + 1);

        swipeController = new SwipeController();
        swipeController.SetLevelManager(this);
        grid = new Grid();

        width = levelScriptables[GameManager.currentLevel].width;
        height = levelScriptables[GameManager.currentLevel].height;

        CompleteBoard();

        grid.Initialize(width, height, cellSize, Vector3.zero);
        cells = new Cell[width, height];

        CreateGrid(Vector3.zero);

        InitBrushCoord();
        gameCamera.ZoomPerspectiveCamera(width, height);
    }

    public void CheckExistBrushCoord()
    {
        BrushController brushController = GameObject.FindGameObjectWithTag("Brush").GetComponent<BrushController>();
        if(brushController != null)
        {
            Destroy(brushController.gameObject);
        }
    }
    public void InitBrushCoord()
    {
        BrushController character = GameDataManager.GetSelectedCharacter().pencilModel;
        if (character != null)
        {
            currentBrush = Instantiate(character, grid.GetCellWorldPosition(levelScriptables[GameManager.currentLevel].brushStartCoords.x,
                levelScriptables[GameManager.currentLevel].brushStartCoords.y), Quaternion.identity);

            currentBrush.coords = levelScriptables[GameManager.currentLevel].brushStartCoords;
            currentBrush.transform.rotation = Quaternion.Euler(-40, -23, 170);
        }
        else
        {
            currentBrush = Instantiate(brush, grid.GetCellWorldPosition(levelScriptables[GameManager.currentLevel].brushStartCoords.x,
                levelScriptables[GameManager.currentLevel].brushStartCoords.y), Quaternion.identity);

            currentBrush.coords = levelScriptables[GameManager.currentLevel].brushStartCoords;
            currentBrush.transform.rotation = Quaternion.Euler(-40, -23, 170);
        }
    }

    private void CreateGrid(Vector3 origionPos)
    {
        origionPos = new Vector3(origionPos.x, origionPos.y - 0.4f, origionPos.z);
        for (int x = 0; x < grid.GridArray.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GridArray.GetLength(1); y++)
            {
                cells[x, y] = CreateCell(x, y, origionPos);
            }
        }
    }

    private Cell CreateCell(int x, int y, Vector3 origionPos)
    {
        Cell cell = Instantiate(blockPrefabs);
        cell.Coords = new Vector2Int(x, y);
        cell.transform.localScale = new Vector3(cellSize,0.25f,cellSize);
        cell.transform.position = origionPos + grid.GetCellWorldPosition(x, y);

        return cell;
    }

    public void MoveBrush(Swipe direction)
    {
        if (uiManager.CheckStateShopBtn())
        {
            uiManager.ChangeStateButton(true);
        }
        Vector2Int newCoords = grid.GetCellXZBySwipe(currentBrush.coords.x, currentBrush.coords.y, direction);

        if(newCoords != new Vector2Int(-1, -1))
        {
            SoudManager.Ins.PlayFx(FxType.BrushMove);
            Vector3 finalPos = grid.GetCellWorldPosition(newCoords.x,newCoords.y);
            
            if(ConnectionAlreadyDone(currentBrush.coords,newCoords,true) == false)
            {
                inProgress.Add(new Conection(currentBrush.coords, newCoords));
                cells[currentBrush.coords.x, currentBrush.coords.y].CellCenter.gameObject.SetActive(true);
                cells[currentBrush.coords.x, currentBrush.coords.y].CellCenter.material.color = colors[GameManager.currentLevel % colors.Length];   

                LinePaintScript linePaint = Instantiate(linePaintPrefab, new Vector3(0, -0.1f, 0), Quaternion.identity);
                linePaint.SetRendererPosition(currentBrush.transform.position + new Vector3(0, -0.1f, 0),
                    finalPos + new Vector3(0, -0.1f, 0), cells[currentBrush.coords.x, currentBrush.coords.y].CellCenter.material.color);
                linePaint.SetConnectionCoords(currentBrush.coords, newCoords);
                connectedLinePaint.Add(linePaint);
            }
            else
            {
                RemoveConnectionLinePaint(currentBrush.coords,newCoords);
            }

            if (levelScriptables[GameManager.currentLevel].completePattern.Count <= inProgress.Count)
            {
                if (IsLevelComplete())
                {
                    CompleteLevel();
                }
            }

            currentBrush.transform.position = finalPos;
            currentBrush.coords = newCoords;
        }
    }

    public void CompleteLevel()
    {
        SoudManager.Ins.PlayFx(FxType.Victory);
        GameManager.gameState = GameState.Complete;
        LevelSystemManager.Instance.LevelCompleteNew(GameManager.currentLevel);
        GameManager.currentLevel++;
        if (GameManager.currentLevel > levelScriptables.Count - 1)
        {
            GameManager.currentLevel = 0;
        }
        PlayerPrefs.SetInt("CurrentLevel", GameManager.currentLevel);
        //int earnedDiamond = PlayerPrefs.GetInt(PrefKey.TotalDiamonds.ToString(), 0) + 15;
        GameDataManager.AddDiamonds(15);
        //PlayerPrefs.SetInt(PrefKey.TotalDiamonds.ToString(), earnedDiamond);
        uiManager.levelComplete();

        Debug.Log("LevelCompleted");
    }

    private bool ConnectionAlreadyDone(Vector2Int startCoord, Vector2Int endCoord, bool removeConnection)
    {
        bool connected = false;

        for (int i = 0; i < inProgress.Count; i++)
        {
            if (inProgress[i].startCoord == startCoord && inProgress[i].endCoord == endCoord
                || inProgress[i].startCoord == endCoord && inProgress[i].endCoord == startCoord)
            {
                if (removeConnection)
                {
                    inProgress.RemoveAt(i);
                }

                connected = true;
                break;
            }

        }
        return connected;
    }

    private void RemoveConnectionLinePaint(Vector2Int startCoord, Vector2Int endCoord)
    {
        for (int i = 0; i < connectedLinePaint.Count; i++)
        {
            if (connectedLinePaint[i].StartCoords == startCoord && connectedLinePaint[i].EndCoords == endCoord
                || connectedLinePaint[i].StartCoords == endCoord && connectedLinePaint[i].EndCoords == startCoord)
            {
                LinePaintScript line = connectedLinePaint[i];
                connectedLinePaint.RemoveAt(i);
                Destroy(line.gameObject);

                cells[endCoord.x,endCoord.y].CellCenter.gameObject.SetActive(false);
            }
        }
    }

    private bool IsLevelComplete()
    {
        if (levelScriptables[GameManager.currentLevel].completePattern.Count != inProgress.Count)
        {
            return false;
        }

        for (int i = 0; i < levelScriptables[GameManager.currentLevel].completePattern.Count; i++)
        {
            if (!ConnectionAlreadyDone(levelScriptables[GameManager.currentLevel].completePattern[i].startCoord,
                levelScriptables[GameManager.currentLevel].completePattern[i].endCoord, false))
                { return false; }
        }
        return true;
    }
    private void CompleteBoard()
    {
        grid.Initialize(width, height, cellSize, gridOriginPos);

        Vector3 offset = new Vector3((levelScriptables[GameManager.currentLevel].width - cellSize) / 2,5f, (levelScriptables[GameManager.currentLevel].height - cellSize) / 2);
        solutionCamera.transform.position += offset;

        solutionCamera.ZoomOrthographicSizeCamera(levelScriptables[GameManager.currentLevel].width, levelScriptables[GameManager.currentLevel].height);

        for (int i = 0; i < levelScriptables[GameManager.currentLevel].completePattern.Count; i++)
        {
            Vector3 startPos = grid.GetCellWorldPosition(levelScriptables[GameManager.currentLevel].completePattern[i].startCoord.x,
                levelScriptables[GameManager.currentLevel].completePattern[i].startCoord.y);
            
            Vector3 endPos = grid.GetCellWorldPosition(levelScriptables[GameManager.currentLevel].completePattern[i].endCoord.x,
                levelScriptables[GameManager.currentLevel].completePattern[i].endCoord.y);

            LinePaintScript linePaint = Instantiate(linePaintPrefab, new Vector3(0, 0.2f, 0), Quaternion.identity);
            linePaint.SetRendererPosition(startPos, endPos, colors[GameManager.currentLevel % colors.Length]);
        }
    }
    private void Update()
    {
        if (swipeController != null && GameManager.gameState == GameState.Playing)
            swipeController.OnUpdate();
    }
}
