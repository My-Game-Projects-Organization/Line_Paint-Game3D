using UnityEngine;
using UnityEditor;
using UnityEditor.MemoryProfiler;
using System.Collections.Generic;
using System;
using static UnityEngine.GraphicsBuffer;

public class LevelEditorWindow : EditorWindow
{
    private int width = 3;
    private int height = 3;
    private Vector2 brushCoord = new Vector2(0, 0);
    private Vector2Int previousCoord = new Vector2Int(0, 0);
    private bool unlocked = true; // Thêm biến bool unlocked

    private GameObject parent;
    private List<Conection> connections = new List<Conection>(); // Danh sách lưu các Connection
    private GameObject brushObject;
    private bool isDragging = false;

    private static int indexLine = 0;
    private static int countLevel = 0;

    GameObject cylinder;
    public static LevelEditorWindow Instance { get; private set; }

    [MenuItem("Tools/Level Editor")]
    private static void ShowWindow()
    {
        var window = GetWindow<LevelEditorWindow>();
        window.titleContent = new GUIContent("Level Editor");
        window.width = 200;
        window.height = 200;
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Create a New Level Grid", EditorStyles.boldLabel);

        // Fields to input width, height, brush coordinates, and unlocked state
        width = EditorGUILayout.IntField("Width", width);
        height = EditorGUILayout.IntField("Height", height);
        brushCoord = EditorGUILayout.Vector2Field("Brush Coordinates", brushCoord);
        unlocked = EditorGUILayout.Toggle("Unlocked", unlocked); // Trường bool unlocked

        if (GUILayout.Button("Create Grid"))
        {
            CreateGrid(width, height);
        }

        if (GUILayout.Button("Clear Grid"))
        {
            ResetConnections();
        }

        EditorGUILayout.LabelField("Brush movement panel", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(10);
        if (GUILayout.Button("Top Left", GUILayout.Width(100)))
            HandleKeyCodeInput("Top Left");
        if (GUILayout.Button("Top", GUILayout.Width(100)))
            HandleKeyCodeInput("Top");
        if (GUILayout.Button("Top Right", GUILayout.Width(100)))
            HandleKeyCodeInput("Top Right");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(10);
        if (GUILayout.Button("Left", GUILayout.Width(100)))
            HandleKeyCodeInput("Left");
        if (GUILayout.Button("None", GUILayout.Width(100)))
            Debug.Log("None");
        if (GUILayout.Button("Right", GUILayout.Width(100)))
            HandleKeyCodeInput("Right");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(10);
        if (GUILayout.Button("Bottom Left", GUILayout.Width(100)))
            HandleKeyCodeInput("Bottom Left");
        if (GUILayout.Button("Bottom", GUILayout.Width(100)))
            HandleKeyCodeInput("Bottom");
        if (GUILayout.Button("Bottom Right", GUILayout.Width(100)))
            HandleKeyCodeInput("Bottom Right");
        EditorGUILayout.EndHorizontal();

        if(GUILayout.Button("Show List Connection"))
        {
            foreach(Conection conection in connections)
            {
                Debug.Log(conection.ToString());
            }
        }

        if (GUILayout.Button("Create Level"))
        {
            LevelScriptableData levelScriptableData = ScriptableObject.CreateInstance<LevelScriptableData>();

            levelScriptableData.width = width;
            levelScriptableData.height = height;
            levelScriptableData.unlocked = false;

            levelScriptableData.brushStartCoords = new Vector2Int(Convert.ToInt32(brushCoord.x), Convert.ToInt32(brushCoord.y));
            levelScriptableData.completePattern = connections;

            countLevel++;
            AssetDatabase.CreateAsset(levelScriptableData, "Assets/Resources/Level_" + countLevel +".asset");

        }


        if(GUILayout.Button("Show Count level"))
        {
            LevelScriptableData[] resourcesLevel = Resources.LoadAll<LevelScriptableData>("");

            if (resourcesLevel != null && resourcesLevel.Length > 0)
            {
                int count = 0;
                foreach (LevelScriptableData resource in resourcesLevel)
                {
                    // Check the type if necessary
                    if (resource is LevelScriptableData)
                    {
                        count++;
                    }
                }

                Debug.Log("Number of ScriptableObjects: " + count);
                countLevel = count;
            }
            else
            {
                Debug.Log("No ScriptableObjects found in Resources");
            }
        }
    }

    private void HandleKeyCodeInput(string BtnName)
    {
        switch (BtnName)
        {
            case "Top":
                Debug.Log(BtnName);
                Vector3 temp = new Vector3(cylinder.transform.position.x,
                    cylinder.transform.position.y,
                    cylinder.transform.position.z);
                cylinder.transform.position = new Vector3(cylinder.transform.position.x,
                cylinder.transform.position.y,
                cylinder.transform.position.z + 1);

                indexLine += 1;
                DrawLine(indexLine,temp,cylinder.transform.position);
                AddConnection(temp.x, temp.z, cylinder.transform.position.x, cylinder.transform.position.z);
                break;
            case "Top Left":
                Debug.Log(BtnName);
                Vector3 temp1 = new Vector3(cylinder.transform.position.x,
                    cylinder.transform.position.y,
                    cylinder.transform.position.z);
                cylinder.transform.position = new Vector3(cylinder.transform.position.x-1,
                cylinder.transform.position.y,
                cylinder.transform.position.z + 1);

                indexLine += 1;
                DrawLine(indexLine, temp1, cylinder.transform.position);
                AddConnection(temp1.x, temp1.z, cylinder.transform.position.x, cylinder.transform.position.z);
                break;
            case "Top Right":
                Debug.Log(BtnName);
                Vector3 temp2 = new Vector3(cylinder.transform.position.x,
                    cylinder.transform.position.y,
                    cylinder.transform.position.z);
                cylinder.transform.position = new Vector3(cylinder.transform.position.x + 1,
                cylinder.transform.position.y,
                cylinder.transform.position.z + 1);

                indexLine += 1;
                DrawLine(indexLine, temp2, cylinder.transform.position);
                AddConnection(temp2.x, temp2.z, cylinder.transform.position.x, cylinder.transform.position.z);
                break;
            case "Right":
                Debug.Log(BtnName);
                Vector3 temp3 = new Vector3(cylinder.transform.position.x,
                    cylinder.transform.position.y,
                    cylinder.transform.position.z);
                cylinder.transform.position = new Vector3(cylinder.transform.position.x+1,
                cylinder.transform.position.y,
                cylinder.transform.position.z);

                indexLine += 1;
                DrawLine(indexLine, temp3, cylinder.transform.position);
                AddConnection(temp3.x, temp3.z, cylinder.transform.position.x, cylinder.transform.position.z);
                break;
            case "Left":
                Debug.Log(BtnName);
                Vector3 temp4 = new Vector3(cylinder.transform.position.x,
                    cylinder.transform.position.y,
                    cylinder.transform.position.z);
                cylinder.transform.position = new Vector3(cylinder.transform.position.x-1,
                cylinder.transform.position.y,
                cylinder.transform.position.z);

                indexLine += 1;
                DrawLine(indexLine, temp4, cylinder.transform.position);
                AddConnection(temp4.x, temp4.z, cylinder.transform.position.x, cylinder.transform.position.z);
                break;
            case "Bottom Right":
                Debug.Log(BtnName);
                Vector3 temp5 = new Vector3(cylinder.transform.position.x,
                    cylinder.transform.position.y,
                    cylinder.transform.position.z);
                cylinder.transform.position = new Vector3(cylinder.transform.position.x + 1,
                cylinder.transform.position.y,
                cylinder.transform.position.z - 1);

                indexLine += 1;
                DrawLine(indexLine, temp5, cylinder.transform.position);
                AddConnection(temp5.x, temp5.z, cylinder.transform.position.x, cylinder.transform.position.z);
                break;
            case "Bottom Left":
                Debug.Log(BtnName);
                Vector3 temp6 = new Vector3(cylinder.transform.position.x,
                    cylinder.transform.position.y,
                    cylinder.transform.position.z);
                cylinder.transform.position = new Vector3(cylinder.transform.position.x - 1,
                cylinder.transform.position.y,
                cylinder.transform.position.z - 1);

                indexLine += 1;
                DrawLine(indexLine, temp6, cylinder.transform.position);
                AddConnection(temp6.x, temp6.z, cylinder.transform.position.x, cylinder.transform.position.z);
                break;
            case "Bottom":
                Debug.Log(BtnName);
                Vector3 temp7 = new Vector3(cylinder.transform.position.x,
                    cylinder.transform.position.y,
                    cylinder.transform.position.z);
                cylinder.transform.position = new Vector3(cylinder.transform.position.x,
                cylinder.transform.position.y,
                cylinder.transform.position.z - 1);

                indexLine += 1;
                DrawLine(indexLine, temp7, cylinder.transform.position);
                AddConnection(temp7.x, temp7.z, cylinder.transform.position.x, cylinder.transform.position.z);
                break;
        }
    }

    private void AddConnection(float startPosX, float startPosY, float endPosX, float endPosY)
    {
        connections.Add(new Conection(new Vector2Int(Convert.ToInt32(startPosX),Convert.ToInt32(startPosY)), 
            new Vector2Int(Convert.ToInt32(endPosX), Convert.ToInt32(endPosY))));
    }

    void DrawLine(int idx, Vector3 startPos, Vector3 endPos)
    {
        string s = "Line" + idx;
        GameObject lineGO = new GameObject(s);
        LineRenderer lineRenderer = lineGO.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
        lineGO.transform.SetParent(parent.transform);

    }

    private void CreateGrid(int width, int height)
    {
        parent  = new GameObject("LevelGrid");

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = new Vector3(x, 0, y);
                cube.transform.parent = parent.transform;


                GridCell cell = cube.AddComponent<GridCell>();
                cell.SetCoordinates(new Vector2Int(x, y));

                // Create a small sphere at the center of the cube
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                // Adjust the size as needed
                sphere.transform.position = cube.transform.position + Vector3.up * 0.5f; // Position at the center of the cube, slightly above
                sphere.transform.parent = cube.transform; // Make the sphere a child of the cube
                                                          // Assuming you have a reference to the renderer component
                cell.SetCoordinatesCenter(sphere.transform.position);

                Renderer renderer = sphere.GetComponent<Renderer>();

                // Create a new material and assign it to the shared material property
                Material newMaterial = new Material(renderer.sharedMaterial);
                newMaterial.color = Color.black; // Set the color to black
                renderer.sharedMaterial = newMaterial;


                // Check if the current position matches the brush coordinates
                if (x == (int)brushCoord.x && y == (int)brushCoord.y)
                {
                    cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    cylinder.transform.position = new Vector3(x, 1, y); // 1 unit above the grid
                    cylinder.transform.parent = parent.transform;
                    cylinder.tag = "Brush";
                    cylinder.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

                    Renderer renderer1 = cylinder.GetComponent<Renderer>();

                    // Create a new material and assign it to the shared material property
                    Material newMaterial1 = new Material(renderer1.sharedMaterial);
                    newMaterial1.color = Color.yellow; // Set the color to black
                    renderer1.sharedMaterial = newMaterial1;
                }
            }
        }
       

    }
    private void HandleMouseInput()
    {
        Event e = Event.current;

        if (e.type == EventType.MouseDown && e.button == 0)
        {
            Vector2 mousePos = e.mousePosition;

            brushCoord = new Vector2(Mathf.Floor(mousePos.x / 20), Mathf.Floor(mousePos.y / 20));
            isDragging = true;
        }

        if(e.type == EventType.MouseDrag && isDragging)
        {
            Vector2 newBrushCoord = new Vector2(Mathf.Floor(e.mousePosition.x / 20), Mathf.Floor(e.mousePosition.y / 20));

            if (newBrushCoord != brushCoord)
            {
                //Add a new connection
                connections.Add(new Conection(Vector2Int.FloorToInt(brushCoord), Vector2Int.FloorToInt(newBrushCoord)));
                brushCoord = newBrushCoord;
            }
        }

        if(e.type == EventType.MouseUp && e.button == 0)
        {
            isDragging = false;
        }
    }


    private void ResetConnections()
    {
        GameObject target = GameObject.Find("LevelGrid");
        if (target != null)
        {
            DestroyImmediate(target);
        }
            connections.Clear();
            Debug.Log("Connections reset.");
        }
    }


