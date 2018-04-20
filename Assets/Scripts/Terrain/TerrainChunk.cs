using System.Collections.Generic;
using UnityEngine;
using System;
namespace SimTerrain {
    /// <summary>
    /// Chunk 是一个渲染的单元，包含了最小的碰撞测试单元 
    /// </summary>
    /// 
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class TerrainChunk : MonoBehaviour
    {
        /// <summary>
        /// Chunk 在Group的索引值，从左下角开始
        /// </summary>
        public  Vector2Int nChunkId;

        /// <summary>
        /// 一个Chunk的真实大小 20.0m * 20.0m ...
        /// </summary>
        public Vector2 worldSize;

        /// <summary>
        /// 一个Chunk的Box 大小 16*16 32*32....
        /// </summary>
        public Vector2Int boxSize;

        public TerrainFile groupInfo;

        public Rect _boxRect;

        /// <summary>
        /// 
        /// </summary>
        //private List<TerrainTriangle> triangles = new List<TerrainTriangle>();

		private TerrainTriangle[] triangles = new TerrainTriangle[2048];

		public TerrainBox[] boxs = new TerrainBox[1024];
        /// <summary>
        /// 在Group中位置偏移量
        /// </summary>
        private Vector3 offset;

        public Vector3[] verticesAry = new Vector3[1];

        public Color32[] ColorsAry = new Color32[1];

        public Vector2[] uvAry = new Vector2[1];

        public int[] indicesAry = new int[1];

        public List<int> IndexBuff = new List<int>();

        private MeshRenderer meshRender = null;
        private MeshFilter meshFilter = null;

        public MeshFilter GetMeshFilter()
        {
            return   meshFilter; 
        }
        private MeshCollider meshCollider = null;
        public MeshCollider GetMeshCollider()
        {
            return meshCollider;
        }

        public TerrainChunk()
        {
            
        }

        private void Awake()
        {

        }

        private void Start()
        {
            //Debug.Log("start");
        }


        private void BuildTriangles(LayerTerrainColor colorConfig, LayerTerrainGroups.Chunk chunkConfig)
        {

            float cellx = worldSize.x / boxSize.x;
            float celly = worldSize.y / boxSize.y;
            //triangles.Capacity = boxSize.x * boxSize.y * 2;
            int heightDataIndex = nChunkId.y * (groupInfo.terrainWidth * boxSize.y) + nChunkId.x * boxSize.x;
            //int valueTest = 0;

            ///  V1-----V2
            ///  |      |
            ///  V0-----V3
            ///  

            Vector3 v0 = new Vector3();
            Vector3 v1 = new Vector3();
            Vector3 v2 = new Vector3();
            Vector3 v3 = new Vector3();

            Vector3 vCellx = new Vector3(cellx, 0, 0);
            Vector3 vCelly = new Vector3(0, 0, celly);
			int triangleIndex = 0;
			int boxIndex = 0;
            for (int idy = 0; idy < boxSize.y; ++idy)
            {
                for (int idx = 0; idx < boxSize.x; ++idx)
                {
                    v0.x = idx * cellx;
                    v0.z = idy * celly;
                    v1 = v0 + vCelly;
                    v2 = v0 + vCellx + vCelly;
                    v3 = v0 + vCellx;
                    int BoxIndex = heightDataIndex + (idy * groupInfo.terrainWidth) + idx;
                    //Triangle 1
                    TerrainTriangle triangle1 = new TerrainTriangle();
					triangle1.groupInfo = groupInfo;
					triangle1.colorConfig = colorConfig;
					triangle1.chunkConfig = chunkConfig;
                    
                    triangle1.chunkIndex = new TerrainTriangle.HeightDataIndex(BoxIndex, BoxIndex + groupInfo.terrainWidth, BoxIndex + groupInfo.terrainWidth + 1);
                    triangle1._v0 = v0;
                    triangle1._v1 = v1;
                    triangle1._v2 = v2;
					triangles[triangleIndex++] = triangle1;

                    //Triangle 2
					TerrainTriangle triangle2 = new TerrainTriangle();
					triangle2.groupInfo = groupInfo;
					triangle2.colorConfig = colorConfig;
					triangle2.chunkConfig = chunkConfig;

                    triangle2.chunkIndex = new TerrainTriangle.HeightDataIndex(BoxIndex, BoxIndex + groupInfo.terrainWidth + 1, BoxIndex + 1);
                    triangle2._v0 = v0;
                    triangle2._v1 = v2;
                    triangle2._v2 = v3;
					triangles[triangleIndex++] = triangle2;

                    TerrainBox box = new TerrainBox(groupInfo, colorConfig, chunkConfig);
					box.ChunkOffSet = offset;
                    box.v0Index = BoxIndex;
                    box.v1Index = BoxIndex + groupInfo.terrainWidth;
                    box.v2Index = BoxIndex + groupInfo.terrainWidth + 1;
                    box.v3Index = BoxIndex + 1;
					box._v0 = v0;
					box._v1 = v1;
					box._v2 = v2;
					box._v3 = v3;

					box.groupInfo = groupInfo;
					box.colorConfig = colorConfig;
					box.chunkConfig = chunkConfig;

					boxs[boxIndex++] = box;
				
                }
            }
        }

        public bool Init(Vector2Int ChunkId, Vector3 vOffset, Vector2 vRealSize, TerrainFile _groupInfo ,LayerTerrainColor colorConfig,LayerTerrainGroups.Chunk chunkConfig,Material mat)
        {
            _boxRect = new Rect(new Vector2(vOffset.x, vOffset.z), vRealSize);

            // Material matrial = Resources.Load<Material>("Terrain/TerainShader");
            //matrial.
            //LayerTerrain.terrainMatrial;
            meshFilter = this.gameObject.GetComponent<MeshFilter>();
            if (meshFilter.sharedMesh == null)
            {
                meshFilter.mesh = new Mesh();
                meshFilter.sharedMesh.MarkDynamic();
            }

            meshRender = gameObject.GetComponent<MeshRenderer>();

#if UNITY_ANDROID
            meshRender.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
#endif
            meshRender.receiveShadows = true;
            meshRender.sharedMaterial = mat;

       
            meshCollider = gameObject.AddComponent<MeshCollider>();
            meshCollider.hideFlags = HideFlags.HideAndDontSave;
            meshCollider.cookingOptions = MeshColliderCookingOptions.CookForFasterSimulation | MeshColliderCookingOptions.EnableMeshCleaning | MeshColliderCookingOptions.InflateConvexMesh | MeshColliderCookingOptions.WeldColocatedVertices;
            meshCollider.sharedMesh = meshFilter.sharedMesh;


            nChunkId = ChunkId;
            offset = vOffset;
            boxSize = new Vector2Int(chunkConfig.ChunkWidth,chunkConfig.ChunkLenght);
            worldSize = vRealSize;
            groupInfo = _groupInfo;

            //build triangles
            BuildTriangles(colorConfig, chunkConfig);
            BuildChunk();
            BrushChunkEnd();
            transform.localPosition = offset;
            return true;
        }
        public void BuildChunk()
        {
            TerrainMeshPool pool = TerrainMeshPool.Instance;
            TerrainMeshPool.MemoryFlag flag = pool.memoryFlag;

            flag.reset();
            pool.reset();

			/*int trianglesCount = triangles.Length;
            
            for (int idx = 0; idx < trianglesCount; ++idx)
            {
                if(triangles[idx].RebuildTriangle(this, pool,ref flag ,idx))
                {
                    if(flag.verticeCount !=0 || flag.indiceCount != 0)
                    {
                        pool.ChunkMemoryCopy(this);
                        flag.flagOldPoint = false;
                        flag.verticeCount = 0;
                        flag.indiceCount = 0;
                    }
                    flag.verticeNewStart = pool.vertticeIndex;
                    flag.indiceNewStart = pool.IndicesIndex;
                }
            }

            if (flag.verticeCount != 0 || flag.indiceCount != 0)
            {
                pool.ChunkMemoryCopy(this);
            }*/

			int boxCount = boxs.Length;
			for (int idx = 0; idx < boxCount; ++idx)
			{
				if(boxs[idx].RebuildTriangle(this, pool,ref flag ,idx))
				{
					if(flag.verticeCount !=0 || flag.indiceCount != 0)
					{
						pool.ChunkMemoryCopy(this);
						flag.flagOldPoint = false;
						flag.verticeCount = 0;
						flag.indiceCount = 0;
					}
					flag.verticeNewStart = pool.vertticeIndex;
					flag.indiceNewStart = pool.IndicesIndex;
				}
			}
			if (flag.verticeCount != 0 || flag.indiceCount != 0)
			{
				pool.ChunkMemoryCopy(this);
			}

            Flush();
        }
        public void BrushChunkEnd()
        {
            MeshFilter filter = gameObject.GetComponent<MeshFilter>();
            MeshCollider collider = gameObject.GetComponent<MeshCollider>();
            collider.sharedMesh = filter.sharedMesh;

        }
        public void Flush()
        {

            TerrainMeshPool.Instance.BuildMesh(this);
          /*
            IndexBuff.Clear();
            Mesh mesh = meshFilter.sharedMesh;
            mesh.Clear();

      
            int verterDesIndex = 0;
            int trianglesCount = triangles.Count;
            for (int idx = 0; idx < trianglesCount; ++idx)
            {
                int IndexBuffOff = verterDesIndex;
                TerrainTriangle triangle = triangles[idx];

                int Copycount = triangle.verticesCount;
                Array.Copy(triangle.verticesAry, 0, verticesAry, verterDesIndex, triangle.verticesCount);
                Array.Copy(triangle.colorsAry, 0, ColorsAry, verterDesIndex, triangle.verticesCount);
                Array.Copy(triangle.uv0Ary, 0, uvAry, verterDesIndex, triangle.verticesCount);
                verterDesIndex += Copycount;

                //int indexBufferCount = triangle.indexBuffer.Count;
                ///for (int idxBuff = 0; idxBuff < indexBufferCount; ++idxBuff)
                //{
                //    IndexBuff.Add(triangle.indexBuffer[idxBuff] + IndexBuffOff);
                //}
                IndexBuff.AddRange(triangle.indexBuffer);
            }

            mesh.vertices = verticesAry;
            mesh.uv = uvAry;
            mesh.colors32 = ColorsAry;

            mesh.SetTriangles(IndexBuff, 0);
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            
            */
        }
    }

}

