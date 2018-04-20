using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TerrainMeshPool : Singleton<TerrainMeshPool> {

    public Vector3[] verticesAry = new Vector3[65000];
    public Color32[] ColorsAry = new Color32[65000];
    public Vector2[] uvAry = new Vector2[65000];
    public int[] indicesBuffer = new int[100000];
    public int vertticeIndex = 0;
    public int IndicesIndex = 0;

    public class MemoryFlag
    {
        public int verticeOldStart = 0;
        public int verticeNewStart = 0;
        public int verticeCount = 0;

        public int indiceOldStart  = 0;
        public int indiceNewStart = 0;
        public int indiceCount  = 0;

        public bool flagOldPoint = false;

        public void reset()
        {
            verticeOldStart = 0;
            verticeCount = 0;
            verticeNewStart = 0;
            indiceOldStart = 0;
            indiceNewStart = 0;
            indiceCount = 0;
            flagOldPoint = false;

        }
    }
    public MemoryFlag memoryFlag  = new MemoryFlag();

    public void reset()
    {
        vertticeIndex = 0;
        IndicesIndex = 0;
    }

    public void ChunkMemoryCopy(SimTerrain.TerrainChunk chunk)
    {
        Array.Copy(chunk.verticesAry, memoryFlag.verticeOldStart, verticesAry, memoryFlag.verticeNewStart, memoryFlag.verticeCount);
        Array.Copy(chunk.ColorsAry, memoryFlag.verticeOldStart, ColorsAry, memoryFlag.verticeNewStart, memoryFlag.verticeCount);
        Array.Copy(chunk.uvAry, memoryFlag.verticeOldStart, uvAry, memoryFlag.verticeNewStart, memoryFlag.verticeCount);
        Array.Copy(chunk.indicesAry, memoryFlag.indiceOldStart, indicesBuffer, memoryFlag.indiceNewStart, memoryFlag.indiceCount);
    }

    public void BuildMesh(SimTerrain.TerrainChunk chunk/*, List<SimTerrain.TerrainTriangle> triangles*/)
    {

        Mesh mesh = chunk.GetMeshFilter().sharedMesh;
        mesh.Clear();



        if(vertticeIndex > chunk.verticesAry.Length)
        {
            int newValue = Math.Min(65000, vertticeIndex + 10000);
            chunk.verticesAry = new Vector3[newValue];
            chunk.uvAry = new Vector2[newValue];
            chunk.ColorsAry = new Color32[newValue];
        }

        if(IndicesIndex > chunk.indicesAry.Length)
        {
            chunk.indicesAry = new int[IndicesIndex+2000*3];
        }
       

        Array.Clear(chunk.indicesAry, 0, chunk.indicesAry.Length);
        Array.Copy(indicesBuffer, chunk.indicesAry, IndicesIndex);
        int copyCount = Math.Min(65000, vertticeIndex);
        Array.Copy(verticesAry, chunk.verticesAry, copyCount);
        Array.Copy(ColorsAry, chunk.ColorsAry, copyCount);
        Array.Copy(uvAry, chunk.uvAry, copyCount);
        
      
        mesh.vertices = chunk.verticesAry;
        mesh.uv = chunk.uvAry;
        mesh.colors32 = chunk.ColorsAry;
        mesh.SetIndices(chunk.indicesAry, MeshTopology.Triangles, 0);
        mesh.RecalculateNormals();
    }
}
