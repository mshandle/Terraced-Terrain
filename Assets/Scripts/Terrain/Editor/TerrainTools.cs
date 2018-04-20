using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class TerrainTools : SceneView
{
    TerrainTools()
    {
        this.titleContent = new GUIContent("SceneExport");
    }


    static void OpenWindows()
    {
        // EditorWindow windows = EditorWindow.GetWindowWithRect<SceneExport>(new Rect(0, 0, 400, 300));
        TerrainTools windows = EditorWindow.GetWindow<TerrainTools>();
        windows.minSize = new Vector2(400, 300);
        windows.maxSize = new Vector2(400, 800);
        windows.Show();
        //
        //Tool.View = ViewTool.None;
    }

    void Add()
    {
        TerrainTools.onSceneGUIDelegate += OnSceneGUI;
        
    }
    void OnSceneGUI(SceneView scene)
    {
        Event e = Event.current;
        Vector2 mousePosition = e.mousePosition;
        Debug.Log(e.type);
        // View point to world point translation function in my game.
        //this._mousePosition = SceneScreenToWorldPoint(mousePosition);

        // Block SceneView's built-in behavior
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        // ------------------------------
        // Your Custom OnGUI Logic
        // ------------------------------

        if (Event.current.type == EventType.MouseDown) Event.current.Use();
        if (Event.current.type == EventType.MouseMove) Event.current.Use();
    }

    private Vector3 SceneScreenToWorldPoint(Vector3 sceneScreenPoint)
    {
        Camera sceneCamera = this.camera;
        float screenHeight = sceneCamera.orthographicSize * 2f;
        float screenWidth = screenHeight * sceneCamera.aspect;

        Vector3 worldPos = new Vector3(
            (sceneScreenPoint.x / sceneCamera.pixelWidth) * screenWidth - screenWidth * 0.5f,
            ((-(sceneScreenPoint.y) / sceneCamera.pixelHeight) * screenHeight + screenHeight * 0.5f),
            0f);

        worldPos += sceneCamera.transform.position;
        worldPos.z = 0f;

        return worldPos;
    }

}
