using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainBrush  {

    public enum BrushType
    {
        BT_ADD,
        BT_DOWN,
        LOCK,
        VIEW,
    }

    public BrushType brushType = BrushType.BT_ADD;

    public float Targetheight = 2.0f;

    public float Radius = 3;

    public float Strenght = 0.06f;

    public float BlurRadius = 5.0f;

    public float BlurPower = 0.6f;

}
