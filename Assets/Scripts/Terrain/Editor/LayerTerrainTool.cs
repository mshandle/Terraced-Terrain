using UnityEngine;
using UnityEditor.ProjectWindowCallback;
using System.IO;
using UnityEditor;

namespace SimTerrain
{
    public class LayerTerrainFactory
    {
        [MenuItem("GameObject/3D Object/LayerTerrain", false, 0)]
        static void AddLayerTerrain()
        {
            GameObject[] objects = Selection.gameObjects;
            GameObject newLayerTerrain = new GameObject("LayerTerrain");
            newLayerTerrain.AddComponent<LayerTerrain>();
            if (objects.Length == 1)
            {
                newLayerTerrain.transform.parent = objects[0].transform;
            }
        }

        [MenuItem("Assets/Create/LayerTerrain", priority = 200)]
        static void MenuCreateLayerTerrainProfile()
        {
            var icon = EditorGUIUtility.FindTexture("ScriptableObject Icon");
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<DoCreateLayerTerrainProfile>(), "New LayerTerrain.asset", icon, null);
        }

        internal static LayerTerrainObject CreatePostProcessingProfileAtPath(string path)
        {
            var profile = ScriptableObject.CreateInstance<LayerTerrainObject>();
            profile.name = Path.GetFileName(path);
            AssetDatabase.CreateAsset(profile, path);
            return profile;
        }
    }

    class DoCreateLayerTerrainProfile : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            LayerTerrainObject profile = LayerTerrainFactory.CreatePostProcessingProfileAtPath(pathName);
            ProjectWindowUtil.ShowCreatedAsset(profile);
        }
    }
}