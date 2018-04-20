using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LayerTerrainColor {
    public List<Color32> colors = new List<Color32>();
    public Color32 bottomColor = new Color32();
    [HideInInspector]
    public bool testSurface = false;
    public float bottomValue = 0.45f;
    public Color32 testColor = new Color32(255, 0, 0, 255);
    public Color32 GetColorWithLayer(int layer)
    {
        if (testSurface)
        {
            return new Color32(255, 0, 0, 255);
        }
        return colors[layer];
    }

    public Color32 GetBottomColor(int layer)
    {
        Color32 color = colors[layer];
        color.a = (byte)Mathf.FloorToInt(color.a * bottomValue);
        color.b = (byte)Mathf.FloorToInt(color.b * bottomValue);
        color.g = (byte)Mathf.FloorToInt(color.g * bottomValue);
        color.r = (byte)Mathf.FloorToInt(color.r * bottomValue);
        return color;
    }
}


