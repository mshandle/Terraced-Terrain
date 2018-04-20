using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LayerTerrainObject : ScriptableObject {

    [SerializeField]
    private string _name;

    [SerializeField]
    LayerTerrainColor Colors = new LayerTerrainColor();

    public LayerTerrainColor GetColors
    {
        get
        {
            return Colors;
        }
    }

    [SerializeField]
    LayerTerrainGroups groups = new LayerTerrainGroups();

    public LayerTerrainGroups GetGroups
    {
        get
        {
            return groups;
        }
    }

}
