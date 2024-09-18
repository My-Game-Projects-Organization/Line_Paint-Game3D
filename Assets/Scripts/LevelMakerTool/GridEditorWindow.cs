using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GridEditorWindow : EditorWindow
{
    private int gridWidth = 10;
    private int gridHeight = 10;

    [MenuItem("Window/Grid Editor")]
    public static void ShowWindow()
    {
        GetWindow<GridEditorWindow>("Grid Editor");
    }

    private void OnGUI()
    {
        DrawGrid(gridWidth, gridHeight);
    }

    private void DrawGrid(int width, int height)
    {
        GUI.backgroundColor = Color.red;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Rect rect = new Rect(i * 20, j * 20, 20, 20);
                GUI.Box(rect, GUIContent.none);

                Handles.BeginGUI();
                Handles.color = Color.black;

                // Vẽ đường dọc bên phải
                if (i < width - 1)
                {
                    Handles.DrawLine(new Vector3(rect.xMax, rect.yMin, 0), new Vector3(rect.xMax, rect.yMax, 0));
                }

                // Vẽ đường ngang bên dưới
                if (j < height - 1)
                {
                    Handles.DrawLine(new Vector3(rect.xMin, rect.yMax, 0), new Vector3(rect.xMax, rect.yMax, 0));
                }

                Handles.EndGUI();
            }
        }
    }
}
