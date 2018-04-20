using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SimTerrain;

[CustomEditor(typeof(LayerTerrain))]
public class LayerTerrainEditor : Editor
{

    private SerializedObject obj;
    [HideInInspector]
    static public LayerTerrain targetObject;
    private LayerTerrainObject objRef;
    static public SerializedProperty DataFile;
    static public SerializedProperty terrainMatiera;
    private SerializedObject DataRefObject;
    private SerializedProperty colors;

    private LayerTerrainGroups groupsRefObject;
    private SerializedProperty groups;

    bool hasConfige = false;

    private int GroupSelectIndex = 0;
    private GUIContent[] icons = new GUIContent[3];
    private GUIContent[] Brushicons = new GUIContent[4];
    private GUIContent delIcon;
    private GUIContent addIcon;
    private GUIContent refreshIcon;
    private GUIContent savaIcon;
    private string[] iconpath = new string[] { "TerrainInspector.TerrainToolSetHeight", "PreTextureRGB", "SettingsIcon" };

    private string[] Brushiconpath = new string[] { "TerrainInspector.TerrainToolRaise", "TerrainInspector.TerrainToolSmoothHeight", "ClothInspector.PaintValue","animationvisibilitytoggleon" };
    private Action[] groupsActions = new Action[3];

    /*enum PageIndex
    {
        BRUSH   = 0,
        COLOR   = 1,
        SETTING = 2,
    }
    private PageIndex page = PageIndex.BRUSH;*/
    /// <summary>
    /// flag mouse Down for Brush
    /// </summary>
    static bool IsMouseDown = false;
    static bool IsMouseDrage = false;
    private int BrushSelectIndex = 0;
    private float BrushSize = 5;
    static private float BrushOpactiy = 0;
    private float BrushStrenght = 5.0f;

    //Blur
    private float BlurRadius = 10.0f;
    private float BlurPower = 1.0f;
    private void OnEnable()
    {
        //Debug.Log("OnEnable");

        targetObject = target as LayerTerrain;
        obj = new SerializedObject(target);
        DataFile = obj.FindProperty("configObject");
        terrainMatiera = obj.FindProperty("terrainMatrial");
        BrushSelectIndex =(int) targetObject.brush.brushType;
        BrushSize = targetObject.brush.Radius;
        BrushOpactiy = targetObject.brush.Targetheight;
        BrushStrenght = targetObject.brush.Strenght;
        BlurRadius = targetObject.brush.BlurRadius;
        BlurPower = targetObject.brush.BlurPower;
        SceneView.onSceneGUIDelegate += OnSceneGUI;

        for (int idx = 0; idx < iconpath.Length; ++idx)
        {
            icons[idx] = EditorGUIUtility.IconContent(iconpath[idx]);
        }

        for (int idx = 0; idx < Brushiconpath.Length; ++idx)
        {
            Brushicons[idx] = EditorGUIUtility.IconContent(Brushiconpath[idx]);
        }

        groupsActions[0] = DrawBrushGroup;
        groupsActions[1] = DrawColorGroup;
        groupsActions[2] = DrawSettingGroup;
        delIcon = EditorGUIUtility.IconContent("TreeEditor.Trash");
        addIcon = EditorGUIUtility.IconContent("Toolbar Plus");
        refreshIcon = EditorGUIUtility.IconContent("TreeEditor.Refresh");
        savaIcon = EditorGUIUtility.IconContent("BuildSettings.SelectedIcon");
    }

    private void OnDisable()
    {
        //Debug.Log("OnDisable");
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
    }
    override public void OnInspectorGUI()
    {
        
        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(DataFile);
        if (DataFile != null)
        {
            hasConfige = DataFile.objectReferenceValue != null;
            if (hasConfige)
            {
                DataRefObject = new SerializedObject(DataFile.objectReferenceValue);
                LayerTerrainObject TerrainObject = DataFile.objectReferenceValue as LayerTerrainObject;
                objRef = TerrainObject;
                groupsRefObject = TerrainObject.GetGroups;
                colors = DataRefObject.FindProperty("Colors");
            }
        }
        if (!hasConfige) return;
        EditorGUILayout.PropertyField(terrainMatiera);
        GroupSelectIndex = GUILayout.Toolbar(GroupSelectIndex, icons, EditorStyles.toolbarButton);
        drawGroup(GroupSelectIndex);
        if (GUILayout.Button(refreshIcon))
        {
            targetObject.Refresh();

        }

        if (GUILayout.Button(savaIcon))
        {
            targetObject.saveData();
            EditorUtility.SetDirty(DataFile.objectReferenceValue);
        }
        if (EditorGUI.EndChangeCheck())
        {
            obj.ApplyModifiedProperties();
        }
    }

    private void drawGroup(int idx)
    {
        //for()
       // page = (PageIndex)idx;
        groupsActions[idx].Invoke();
    }

    private void DrawBrushGroup()
    {
        EditorGUILayout.BeginVertical();
        GUILayout.Label("Brush");
        EditorGUI.BeginChangeCheck();
        BrushSelectIndex = GUILayout.Toolbar(BrushSelectIndex, Brushicons, EditorStyles.toolbarButton);

        BrushSize = EditorGUILayout.Slider("Brush Size",BrushSize, 0, 20);
        BrushOpactiy = (float)EditorGUILayout.IntSlider("TargetHeight", (int)BrushOpactiy, 1, 32);
        BrushStrenght = EditorGUILayout.Slider("Strength", BrushStrenght, 0.0f, 1.0f);

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        BlurRadius = EditorGUILayout.Slider("BlurRadius", BlurRadius, 0.0f, 10.0f);
        BlurPower = EditorGUILayout.Slider("BlurPower", BlurPower, -2.0f, 2.0f);

        if (EditorGUI.EndChangeCheck())
        {
            targetObject.brush.Radius = BrushSize;
            targetObject.brush.Targetheight = BrushOpactiy;
            targetObject.brush.Strenght = BrushStrenght;
            targetObject.brush.BlurRadius = BlurRadius;
            targetObject.brush.BlurPower = BlurPower;
            targetObject.brush.brushType = (TerrainBrush.BrushType)BrushSelectIndex;
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawColorGroup()
    {
        if(DataRefObject != null)
        {
            DataRefObject.Update();
        }
        
        EditorGUILayout.BeginVertical();
        GUILayout.Label("Colors");
        EditorGUI.BeginChangeCheck();

        if (colors != null )
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(colors, true);
            if (EditorGUI.EndChangeCheck())
            {
                //Debug.Log("Change Colors");
                targetObject.Refresh();
            }
            //EditorGUILayout.PropertyField(bottomColor);
        }

        if (EditorGUI.EndChangeCheck() && DataRefObject != null)
        {
            DataRefObject.ApplyModifiedProperties();
        }
     
        EditorGUILayout.EndVertical();
    }

    private void DrawSettingGroup()
    {
        EditorGUILayout.BeginVertical();
        GUILayout.Label("Setting");
  
        groupsRefObject.chunk.ChunkLenght = EditorGUILayout.IntField("Chunk Lenght",groupsRefObject.chunk.ChunkLenght);

        groupsRefObject.chunk.ChunkWidth = EditorGUILayout.IntField("Chunk Width", groupsRefObject.chunk.ChunkWidth);

        groupsRefObject.chunk.uvReslution = EditorGUILayout.Vector2Field("UV Reslution", groupsRefObject.chunk.uvReslution);

        groupsRefObject.chunk.LayerHeight = EditorGUILayout.FloatField("Layer Height", groupsRefObject.chunk.LayerHeight);

        for (int idx = 0; idx < groupsRefObject.datas.Count; ++idx)
        {
            EditorGUILayout.LabelField("--------------------------------------------------------------------------------------------");
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            GUILayout.Label("Index:" + idx.ToString());
            TerrainFile FileRef = groupsRefObject.datas[idx];
            EditorGUI.BeginChangeCheck();
            FileRef.name = EditorGUILayout.TextField("name", FileRef.name);
            FileRef.terrainWidth = EditorGUILayout.IntField("TerrainWidth", FileRef.terrainWidth);
            FileRef.terrainLength = EditorGUILayout.IntField("TerrainLenght", FileRef.terrainLength);
            if (EditorGUI.EndChangeCheck())
            {
                EditorGUILayout.HelpBox("修改尺寸，会重置地图数据", MessageType.Warning,true);
            }
            FileRef.worldPositon = EditorGUILayout.Vector3Field("worldPositon", FileRef.worldPositon);
            FileRef.worldSize = EditorGUILayout.Vector3Field("worldSize", FileRef.worldSize);
        }
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(delIcon))
        {
            if (groupsRefObject.datas.Count <= 0)
                return;
            DelGroupWindows.OpenWindow(groupsRefObject.datas);
           
        }

        if (GUILayout.Button(addIcon))
        {
            AddGroupWindows.OpenWindow(objRef, groupsRefObject.datas);
        }


        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    static void OnSceneGUI(SceneView sceneView)
    {
        //if (page != PageIndex.BRUSH) return;

        Event e = Event.current;
        Vector2 mousePosition = e.mousePosition;

        // View point to world point translation function in my game.
        //this._mousePosition = SceneScreenToWorldPoint(mousePosition);

        // Block SceneView's built-in behavior
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        bool HandleMsg = false;
         if (e.isMouse)
        {
            // Debug.Log(e.button.ToString() + ":"+ e.type.ToString());
        }
        if(e.button == 0 && e.type == EventType.MouseDown)
        {
            HandleMsg = false;
            RaycastHit hit;
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            int terrainMask = LayerMask.NameToLayer("Terrain");
            int layerIndex = 0;
            if (targetObject.TouchInTerrain(ray, out layerIndex))
            {
                targetObject.BrushTerrainBegin();

                if ((targetObject.brush.brushType) == TerrainBrush.BrushType.VIEW)
                {
                    targetObject.brush.Targetheight = layerIndex;
                    BrushOpactiy = layerIndex;
                }
                else if ((targetObject.brush.brushType) == TerrainBrush.BrushType.LOCK)
                {
                    targetObject.brush.Targetheight = layerIndex;
                    BrushOpactiy = layerIndex;
                    IsMouseDown = true;
                }
                else
                {
                     IsMouseDown = true;
                }
               
            }
        }

        if (e.button == 0 && e.type == EventType.MouseDrag && IsMouseDown)
        {
            HandleMsg = true;
            RaycastHit hit;
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            targetObject.BrushTerrain(ray);
            IsMouseDrage = true;
        }

        if (e.button == 0 && e.type == EventType.MouseUp)
        {
            IsMouseDown = false;
            if (IsMouseDrage)
            {
                targetObject.BrushTerrainEnd();
                IsMouseDrage = false;
            }
            HandleMsg = true;
            
        }

        /*if(e.control && e.keyCode == KeyCode.S)
        {
            if(DataFile != null)
            {
                Debug.Log("Sava data");
                targetObject.saveData();
                EditorUtility.SetDirty(DataFile.objectReferenceValue);
            }
           
        }*/
        // ------------------------------
        // Your Custom OnGUI Logic
        // ------------------------------
 
        if (HandleMsg == true)
        {
            if (Event.current.type == EventType.MouseDown) Event.current.Use();
            if (Event.current.type == EventType.MouseMove) Event.current.Use();
        }

    }
}
