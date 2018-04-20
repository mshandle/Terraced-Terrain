using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SimTerrain;
public class AddGroupWindows : EditorWindow {

    List<TerrainFile> containers = null;
    public LayerTerrainObject objRef = null;
    TerrainFile newFile;
    public List<TerrainFile> Containers
    {
        get
        {
            return containers;
        }
        set
        {
            containers = value;
        }
    }

    public static void OpenWindow(LayerTerrainObject Refobject,List<TerrainFile> container)
    {
        AddGroupWindows windows = EditorWindow.GetWindow<AddGroupWindows>();
        windows.minSize = new Vector2(400, 300);
        windows.maxSize = new Vector2(400, 800);
        windows.Show();
        windows.objRef = Refobject;
        windows.Containers = container;
    }
    private void OnEnable()
    {
        newFile = new TerrainFile();

    }

    private void OnGUI()
    {
        // EditorGUILayout.
        newFile.name = EditorGUILayout.TextField("name", newFile.name);
        newFile.terrainWidth = EditorGUILayout.IntField("TerrainWidth", newFile.terrainWidth);
        newFile.terrainLength = EditorGUILayout.IntField("TerrainLenght", newFile.terrainLength);
        newFile.worldPositon = EditorGUILayout.Vector3Field("worldPositon", newFile.worldPositon);
        newFile.worldSize = EditorGUILayout.Vector3Field("worldSize", newFile.worldSize);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Ok"))
        {
            newFile.Data = new ushort[newFile.terrainWidth * newFile.terrainLength];
            TextAsset defulatData =  Resources.Load<TextAsset>("Terrain/data1");
            for(int idx = 0;idx < newFile.terrainWidth * newFile.terrainLength; ++idx)
            {
                //newFile.Data[idx] = defulatData.bytes[idx];
                newFile.Data[idx] =0;
            }
            EditorUtility.SetDirty(objRef);
            containers.Add(newFile);
            Close();
        }
        if (GUILayout.Button("Canle"))
        {
            Close();
        }
        EditorGUILayout.EndHorizontal();
    }
}
public class DelGroupWindows : EditorWindow
{
    List<TerrainFile> containers = null;
    private int index = 0;
    public List<TerrainFile> Containers
    {
        get
        {
            return containers;
        }
        set
        {
            containers = value;
        }
    }

    public static void OpenWindow(List<TerrainFile> container)
    {
        DelGroupWindows windows = EditorWindow.GetWindow<DelGroupWindows>();
        windows.minSize = new Vector2(400, 300);
        windows.maxSize = new Vector2(400, 800);
        windows.Show();
        windows.Containers = container;
    }
    private void OnEnable()
    {
        index = 0;
    }

    private void OnGUI()
    {

        index = EditorGUILayout.IntSlider("Index", index, 0, containers.Count - 1);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Ok"))
        {
            containers.RemoveAt(index);
            Close();
        }
        if (GUILayout.Button("Canle"))
        {
            Close();
        }
        EditorGUILayout.EndHorizontal();
    }
}
