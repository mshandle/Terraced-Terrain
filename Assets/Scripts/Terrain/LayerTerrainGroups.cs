using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimTerrain;
[System.Serializable]
public class LayerTerrainGroups
{
    [System.Serializable]
    public class Chunk
    {
        [Tooltip("Number of sample points, which affects quality and performance.")]
        [Range(0.0f, 5.0f)]
        public float LayerHeight = 0.2F;
        [Space(2.0f)]
        public Vector2 uvReslution = new Vector2(14.0f, 14.0f);
        public int ChunkWidth = 32;
        public int ChunkLenght = 32;
    }
    public Chunk chunk = new Chunk();
    public List<TerrainFile> datas  = new List<TerrainFile>();

}
