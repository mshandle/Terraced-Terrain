using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SimTerrain
{
    /// <summary>
    /// 一个地形组，可以认为一个高度图文件对应一个地形组，
    /// 地下划分多个地形Chunk.s
    /// </summary>
    /// 
    public class TerrainGroup : MonoBehaviour
    {
        private List<TerrainChunk> Chunks = new List<TerrainChunk>();
        GameObject GroupObject = null;
        TerrainFile filedata = null;
        LayerTerrainGroups.Chunk chunkData;

        int ChunkColumn = -1;
        int ChunkRow = -1;
        int ChunkCount = -1;
        private void Awake()
        {

        }


        public bool Init(LayerTerrainColor color, LayerTerrainGroups.Chunk data, TerrainFile file,Material mat)
        {
            chunkData = data;
            filedata = file;
            ChunkColumn = (file.terrainWidth / data.ChunkWidth);
            //2 *2 or 3*3 4*4
            ChunkRow = (file.terrainLength / data.ChunkLenght);
            ChunkCount = ChunkColumn * ChunkRow;
            //create Chunk
            Chunks.Capacity = ChunkCount;
            float WorldChunkSize = file.worldSize.x / ChunkColumn;



            for (int idy = 0; idy < ChunkRow; ++idy)
            {
                for (int idx = 0; idx < ChunkColumn; ++idx)
                {
                    GameObject Chunk = new GameObject(this.gameObject.name + "Chunk[" + idy.ToString() + ":" + idx.ToString() + "]");
                    Chunk.transform.parent = transform;
                    Chunk.tag = "Terrain";
                    Chunk.layer =  8;
                    MeshFilter meshfilter =  Chunk.AddComponent<MeshFilter>();
                    meshfilter.hideFlags = HideFlags.HideAndDontSave;
                    MeshRenderer render =  Chunk.AddComponent<MeshRenderer>();
                    render.hideFlags = HideFlags.DontSave;

                    TerrainChunk terrainChunkItem = Chunk.AddComponent<TerrainChunk>();
                    terrainChunkItem.hideFlags = HideFlags.HideAndDontSave;

                    Vector2Int index = new Vector2Int();
                    index.x = idx;
                    index.y = idy;

                    Vector3 offset = new Vector3();
                    offset.x = WorldChunkSize * idx;
                    offset.z = WorldChunkSize * idy;
                    offset.y = 0.0f;

                    terrainChunkItem.Init(new Vector2Int(idx, idy), offset,
                                          new Vector2(WorldChunkSize, WorldChunkSize),
                                          file, color, data,mat);
                    Chunks.Add(terrainChunkItem);

                }
            }
            return true;
        }
        void BuildGroup()
        {
            for (int idx = 0; idx < Chunks.Count; ++idx)
            {
                Chunks[idx].BuildChunk();
            }
        }

        IEnumerator BuildGroupAsy()
        {
            for (int idx = 0; idx < Chunks.Count; ++idx)
            {
                Chunks[idx].BuildChunk();
                yield return new WaitForEndOfFrame();
            }
            yield return null;
        }

        private void Start()
        {
            //TODO:
        }
        private GameObject flag = null;
        public bool CastGroup(Ray ray, TerrainBrush brush, List<TerrainChunk> colliedChunks,out Vector3 HitPoint)
        {
            RaycastHit info = default(RaycastHit);
            bool Collided = false;
            HitPoint = Vector3.zero;

            List<TerrainChunk> CopyChunks = Chunks;
            int nChildCount = CopyChunks.Count;

            int nCopyChunkCount = CopyChunks.Count;
            for (int idx = 0; idx < nCopyChunkCount; ++idx)
            {
                TerrainChunk Curchunk = CopyChunks[idx];

                if (Curchunk.GetMeshCollider().Raycast(ray, out info, 1000))
                {
                    if (flag != null)
                        flag.transform.position = info.point;

                  
                        Brush(info.point, Curchunk, brush);
//                         Brush(info.point + new Vector3(0.0f,0.0f,0.2f), Curchunk, brush);
//                         Brush(info.point + new Vector3(0.0f, 0.0f, -0.2f), Curchunk, brush);
//                         Brush(info.point + new Vector3(-0.2f, 0.0f, 0.0f), Curchunk, brush);
//                         Brush(info.point + new Vector3(0.2f, 0.0f, 0.0f), Curchunk, brush);

                    //return info.point;
                    HitPoint = info.point;
                    Collided = true;
                    break;
                }
            }
            float brushLenght = (brush.Radius + brush.BlurRadius) + 2.0f;
            Circle brushCircle = new Circle(new Vector2(info.point.x, info.point.z), brushLenght);

            colliedChunks.Clear();
            if (Collided)
            {
                for (int idx = 0; idx < nCopyChunkCount; ++idx)
                {
                    if (Circle.ColliderRect(brushCircle, CopyChunks[idx]._boxRect))
                    {
                        colliedChunks.Add(CopyChunks[idx]);
                    }
                }
            }
            int nColliderChunks = colliedChunks.Count;
            for (int idx = 0; idx < nColliderChunks; ++idx)
            {
                colliedChunks[idx].BuildChunk();
            }
            return Collided;
        }

        public bool TouchInGround(Ray ray, out int layerIndex)
        {
            RaycastHit info;
            for (int idx = 0; idx < transform.childCount; ++idx)
            {
                TerrainChunk Curchunk = transform.GetChild(idx).GetComponent<TerrainChunk>();
                if (Curchunk == null) continue;
                if (Curchunk.GetComponent<MeshCollider>().Raycast(ray, out info, 5000.0f))
                {
                    layerIndex = Mathf.RoundToInt(info.point.y / chunkData.LayerHeight) + 1;
                    return true;
                }
            }
            layerIndex = 0;
            return false;
        }


        private void Update()
        {

        }

        private float GetDistancePower(float distance ,bool withBlur,TerrainBrush brush)
        {
            float num = 1.0f;
            if(withBlur && brush.BlurRadius > 0.0f)
            {
                num = Mathf.InverseLerp(brush.Radius + brush.BlurRadius, brush.Radius, distance);
            }
            float blurPower = brush.BlurPower;
            if(blurPower > 0.0f)
            {
                num = Mathf.Pow(num, blurPower);
            }else if(blurPower < 0.0f)
            {
                num = Mathf.Pow(num, 1.0f / (1.0f - blurPower));
            }
            return num;
        }
        private void BrushRaiseEx(Vector3 point, TerrainBrush brush, TerrainChunk chunk)
        {
            #region v1.0
            /*

            Vector3 localPoint = point - transform.position;
            Vector2 localIdx = new Vector2(localPoint.x, localPoint.z);

            localIdx.x = Mathf.RoundToInt((localIdx.x / chunk.groupInfo.worldSize.x) *
                chunk.groupInfo.terrainWidth);
            localIdx.y = Mathf.RoundToInt((localIdx.y / chunk.groupInfo.worldSize.y) *
          chunk.groupInfo.terrainLength);

            float radio = (float)brush.BrushRange;
            float BrushOcaptiy = brush.BrushOpacity;
            float cell = chunk.groupInfo.worldSize.x / (chunk.groupInfo.terrainWidth - 1);

            
            float maxX = localPoint.x + (radio + 2.0f * cell);
            float minX = localPoint.x - (radio + 2.0f * cell);
            float maxY = localPoint.z + (radio + 2.0f * cell);
            float minY = localPoint.z - (radio + 2.0f * cell);
            int iMinx = Mathf.FloorToInt((minX / chunk.groupInfo.worldSize.x) *
                chunk.groupInfo.terrainWidth);


            float fMaxX = (maxX / chunk.groupInfo.worldSize.x) * chunk.groupInfo.terrainWidth;
            int iMaxx = Mathf.CeilToInt(fMaxX);
            int iMiny = Mathf.FloorToInt((minY / chunk.groupInfo.worldSize.y) *
                chunk.groupInfo.terrainLength);
            int iMaxy = Mathf.CeilToInt((maxY / chunk.groupInfo.worldSize.y) *
                chunk.groupInfo.terrainLength);

            Vector2 TestPoint = new Vector2();
            for(int idy = iMiny; idy <= iMaxy; ++idy)
            {
                for(int idx = iMinx; idx <= iMaxx; ++idx)
                {
                    TestPoint.x = idx;
                    TestPoint.y = idy;
                    float distance = Vector2.Distance(TestPoint, localIdx);
                    float weight =1.0f -  (distance / radio);
                    if(weight < 0.0f)
                    {
                        weight = 0.0f;
                    }
                    if(weight > 1.0f)
                    {
                        weight = 1.0f;
                    }
                    weight = Mathf.Sin(weight * (Mathf.PI / 2.0f));
                    float curValue = chunk.groupInfo.GetRealHeight(idx, idy);
                    float newValue = (curValue * (1.0f - weight)) + BrushOcaptiy * weight;
                    chunk.groupInfo.SetValue(idx, idy, newValue);
                }
            }
            
            */
            #endregion v1.0

            #region v1.1
            /*
            Vector3 localPoint = point - transform.position;
            int BrushSize = (int)brush.BrushRange;
            float BrushOcaptiy = brush.Targetheight + 1.0f;
            float powValue = brush.BrushPow;

            int idx = Mathf.RoundToInt((localPoint.x / chunk.groupInfo.worldSize.x) * chunk.groupInfo.terrainWidth);
            int idy = Mathf.RoundToInt((localPoint.z / chunk.groupInfo.worldSize.y) * chunk.groupInfo.terrainLength);

            int minX = Mathf.Max(0, idx - BrushSize);
            int maxX = Mathf.Min(chunk.groupInfo.terrainWidth, idx + BrushSize);

            int minY = Mathf.Max(0, idy - BrushSize);
            int maxY = Mathf.Min(chunk.groupInfo.terrainLength, idy + BrushSize);

            Vector2 center = new Vector2(idx, idy);
            Vector2 testPoint = new Vector2(maxX, maxY);
            float maxDistance = Vector2.Distance(center, testPoint);
            for (int y = minY; y <= maxY; ++y)
            {
                for (int x = minX; x <= maxX; ++x)
                {
                    float curValue = chunk.groupInfo.GetRealHeight(x, y);
                    testPoint.x = x;
                    testPoint.y = y;
                    float distance = Vector2.Distance(center, testPoint);
                    //if (distance > brush.BrushRange) continue;
                    float Distancevalue = (distance / maxDistance);
                    float num1 = Mathf.Pow(Distancevalue, powValue);
                    float num2 = (1.0F - num1);
                    if (num2 > 1.0f) num2 = 1.0f;
                    if (num2 < 0.0f) num2 = 0.0f;
                    float newvalue = BrushOcaptiy * num2;
                    
                    if(newvalue > BrushOcaptiy - 1.0f)
                    {
                        newvalue = BrushOcaptiy - 1.0F;
                    }
                    newvalue = Mathf.Max(curValue, newvalue);
                    chunk.groupInfo.SetValue(x, y, newvalue);
                }
            }
            */
            #endregion v1.1

            float radius = brush.Radius;
            float num = brush.Strenght;
            float blurRange = brush.BlurRadius;
            float num3 = 1.0f - Mathf.Pow(brush.BlurPower, 4.0f);
            num3 = 1.0f - Mathf.Pow(1.0f - num3, 1.1f);

            //Height
            Vector2 localPoint =new Vector2( point.x - transform.position.x,point.z - transform.position.z);
            localPoint.x = Mathf.RoundToInt((localPoint.x / chunk.groupInfo.worldSize.x) * (chunk.groupInfo.terrainWidth - 1));
            localPoint.y = Mathf.RoundToInt((localPoint.y / chunk.groupInfo.worldSize.y) * (chunk.groupInfo.terrainLength- 1));

            float Minx = Mathf.Max(0, localPoint.x - (radius + blurRange));
            float Maxx = Mathf.Min(chunk.groupInfo.terrainWidth - 1, localPoint.x + (radius + blurRange));
            float MinY = Mathf.Max(0, localPoint.y - (radius + blurRange));
            float Maxy = Mathf.Min(chunk.groupInfo.terrainLength - 1, localPoint.y + (radius + blurRange));
            Vector2 CenterPoint = new Vector2(Minx, MinY);
            float localsqrDis = (radius + blurRange) * (radius + blurRange);
            for (int y = (int)MinY; y <= Maxy; ++y)
            {
                for (int x =(int) Minx; x <= Maxx; ++x)
                {
                    //每个点的 高低的切面
                    int heightflag = chunk.groupInfo.GetHeightIndex(x, y);
                    int heightMinFlag = chunk.groupInfo.GetMinHeightIndex(x, y);

                    //float curValue = chunk.groupInfo.GetRealHeight(x, y);
                    CenterPoint.x = x;
                    CenterPoint.y = y;
                    float sqr =  Vector2.SqrMagnitude(localPoint - CenterPoint);
                    if(sqr < localsqrDis)
                    {
                        float distancePower =  GetDistancePower(Mathf.Sqrt(sqr), true, brush);
                        int targetFlag = Mathf.FloorToInt(brush.Targetheight - 0.001f);

                        // float  curValue = chunk.groupInfo.GetRealHeight(x, y);

                        //做一次 Filter 整形，不需要重复整形。用Flag标志位表示
                        /* if (heightflag == heightMinFlag && heightMinFlag < targetFlag)
                         {
                             if (chunk.groupInfo.GetFilterFlagValue(x, y) == false)
                             {

                                 chunk.groupInfo.SetValue(x,y, heightMinFlag);
                                 chunk.groupInfo.SetSurfaceValue(x, y, heightMinFlag);
                                 chunk.groupInfo.SetFilterFlagValue(x, y, true);
                                 //curValue = heightflag;
                             }
                         }
                         */
						float curValue = chunk.groupInfo.GetSurfaceRealValue(x, y);
                        float newvalue = Mathf.Lerp(curValue, brush.Targetheight, num * distancePower);
                      
                        //防止拓扑面畸变，补面
                        if (heightflag > targetFlag  )
                        {
                            if(heightMinFlag <= targetFlag)
                            {
								float surfacevalue = Mathf.Max (chunk.groupInfo.GetSurfaceRealValue (x, y), brush.Targetheight - 0.5f);
								chunk.groupInfo.SetSurfaceValue(x, y,surfacevalue);
                            }
                            continue;
                        }
                        if (heightMinFlag >= targetFlag)
                        {
                            continue;
                        }
                        chunk.groupInfo.SetCullFace(x, y, newvalue);
                        chunk.groupInfo.SetValue(x, y, newvalue);
                        chunk.groupInfo.SetSurfaceValue(x, y, newvalue);
                      
                    }

                }
            }
        }


        private void BrushRaise(Vector3 point, TerrainBrush brush, TerrainChunk chunk)
        {
            Vector3 localPoint = point - transform.position;
            int BrushSize = (int)brush.Radius;
            float BrushOcaptiy = brush.Targetheight;
            float powValue = brush.Strenght;

            int idx = Mathf.RoundToInt((localPoint.x / chunk.groupInfo.worldSize.x) * chunk.groupInfo.terrainWidth);
            int idy = Mathf.RoundToInt((localPoint.z / chunk.groupInfo.worldSize.y) * chunk.groupInfo.terrainLength);

            int minX = Mathf.Max(0, idx - BrushSize);
            int maxX = Mathf.Min(chunk.groupInfo.terrainWidth, idx + BrushSize);

            int minY = Mathf.Max(0, idy - BrushSize);
            int maxY = Mathf.Min(chunk.groupInfo.terrainLength, idy + BrushSize);

            Vector2 center = new Vector2(idx, idy);
            Vector2 testPoint = new Vector2(maxX , maxY );
            float maxDistance = Vector2.Distance(center,testPoint);
            for (int y = minY; y <= maxY; ++y)
            {
                for (int x = minX; x <= maxX; ++x)
                {
                    float curValue = chunk.groupInfo.GetRealHeight(x, y);
                    testPoint.x = x;
                    testPoint.y = y;
                    float distance = Vector2.Distance(center, testPoint);
                    float Distancevalue = (distance / maxDistance);
                    float num1 = Mathf.Pow(Distancevalue, powValue);
                    float num2 = (1.0F - num1);
                    if (num2 > 1.0f) num2 = 1.0f;
                    if (num2 < 0.0f) num2 = 0.0f;
                    float newvalue = BrushOcaptiy * num2;
                    newvalue = Mathf.Max(newvalue, curValue);
                   
                    chunk.groupInfo.SetValue(x, y, newvalue);
                    chunk.groupInfo.SetSurfaceValue(x, y, newvalue);
                    if(chunk.groupInfo.GetValue(x,y) != chunk.groupInfo.GetSurfaceValue(x, y))
                    {

                        Debug.LogError("Set Value  Error");
                        chunk.groupInfo.SetValue(x, y, newvalue);
                        chunk.groupInfo.SetSurfaceValue(x, y, newvalue);
                    }
                }
            }
        }
        private void BrushDown(Vector3 point, TerrainBrush brush, TerrainChunk chunk)
        {

            float radius = brush.Radius;
            float num = brush.Strenght;
            float blurRange = brush.BlurRadius;
            float num3 = 1.0f - Mathf.Pow(brush.BlurPower, 4.0f);
            num3 = 1.0f - Mathf.Pow(1.0f - num3, 1.1f);
            float target = brush.Targetheight;
            target -= 1.0f;
            //Height
            Vector2 localPoint = new Vector2(point.x - transform.position.x, point.z - transform.position.z);
            localPoint.x = Mathf.RoundToInt((localPoint.x / chunk.groupInfo.worldSize.x) * (chunk.groupInfo.terrainWidth - 1));
            localPoint.y = Mathf.RoundToInt((localPoint.y / chunk.groupInfo.worldSize.y) * (chunk.groupInfo.terrainLength - 1));

            float Minx = Mathf.Max(0, localPoint.x - (radius + blurRange));
            float Maxx = Mathf.Min(chunk.groupInfo.terrainWidth - 1, localPoint.x + (radius + blurRange));
            float MinY = Mathf.Max(0, localPoint.y - (radius + blurRange));
            float Maxy = Mathf.Min(chunk.groupInfo.terrainLength - 1, localPoint.y + (radius + blurRange));
            Vector2 CenterPoint = new Vector2(Minx, MinY);
            float localsqrDis = (radius + blurRange) * (radius + blurRange);
            for (int y = (int)MinY; y <= Maxy; ++y)
            {
                for (int x = (int)Minx; x <= Maxx; ++x)
                {
                    int heightflag = chunk.groupInfo.GetHeightIndex(x, y);
                    int heightMinFlag = chunk.groupInfo.GetMinHeightIndex(x, y);
                    int targetFlag = Mathf.FloorToInt(brush.Targetheight - 0.001f);

                    if (heightMinFlag < targetFlag)
                    {
                        if (heightflag > targetFlag) {
                            chunk.groupInfo.SetCullFace(x, y, targetFlag);
                        }
                        continue;
                    }
                       

                    float curValue = chunk.groupInfo.GetRealHeight(x, y);
                    CenterPoint.x = x;
                    CenterPoint.y = y;
                    float sqr = Vector2.SqrMagnitude(localPoint - CenterPoint);
                    if (sqr < localsqrDis)
                    {
                        float distancePower = GetDistancePower(Mathf.Sqrt(sqr), true, brush);
                        float newvalue = Mathf.Lerp(curValue, target, num * distancePower);
                        newvalue = Mathf.Min(curValue, newvalue);
                        newvalue = Mathf.Max(target + 0.001f, newvalue);
                        chunk.groupInfo.SetValue(x, y, newvalue);
                        chunk.groupInfo.SetSurfaceValue(x, y, newvalue);
                        chunk.groupInfo.SetCullFace(x, y,newvalue);
                    }

                }
            }
        }

        private void BrushLock(Vector3 point, TerrainBrush brush, TerrainChunk chunk)
        {
            float radius = brush.Radius;
            float num = brush.Strenght;
            float blurRange = brush.BlurRadius;
            float num3 = 1.0f - Mathf.Pow(brush.BlurPower, 4.0f);
            num3 = 1.0f - Mathf.Pow(1.0f - num3, 1.1f);

            //Height
            Vector2 localPoint = new Vector2(point.x - transform.position.x, point.z - transform.position.z);
            localPoint.x = Mathf.RoundToInt((localPoint.x / chunk.groupInfo.worldSize.x) * (chunk.groupInfo.terrainWidth - 1));
            localPoint.y = Mathf.RoundToInt((localPoint.y / chunk.groupInfo.worldSize.y) * (chunk.groupInfo.terrainLength - 1));

            float Minx = Mathf.Max(0, localPoint.x - (radius));
            float Maxx = Mathf.Min(chunk.groupInfo.terrainWidth - 1, localPoint.x + (radius));
            float MinY = Mathf.Max(0, localPoint.y - (radius ));
            float Maxy = Mathf.Min(chunk.groupInfo.terrainLength - 1, localPoint.y + (radius ));
            Vector2 CenterPoint = new Vector2(Minx, MinY);
            float localsqrDis = (radius + blurRange) * (radius + blurRange);
            for (int y = (int)MinY; y <= Maxy; ++y)
            {
                for (int x = (int)Minx; x <= Maxx; ++x)
                {
                    //每个点的 高低的切面
                    int heightflag = chunk.groupInfo.GetHeightIndex(x, y);
                    int heightMinFlag = chunk.groupInfo.GetMinHeightIndex(x, y);

                    //float curValue = chunk.groupInfo.GetRealHeight(x, y);
                    CenterPoint.x = x;
                    CenterPoint.y = y;
                    float sqr = Vector2.SqrMagnitude(localPoint - CenterPoint);
                    if (sqr < localsqrDis)
                    {
                        float distancePower = GetDistancePower(Mathf.Sqrt(sqr), true, brush);
                        int targetFlag = Mathf.FloorToInt(brush.Targetheight - 0.001f);
                        float curValue = chunk.groupInfo.GetSurfaceRealValue(x, y);
                        // float  curValue = chunk.groupInfo.GetRealHeight(x, y);

                        //做一次 Filter 整形，不需要重复整形。用Flag标志位表示
                        if (heightflag == heightMinFlag && heightMinFlag < targetFlag)
                        {
                            if (chunk.groupInfo.GetFilterFlagValue(x, y) == false)
                            {

                                chunk.groupInfo.SetValue(x, y, heightMinFlag);
                                chunk.groupInfo.SetSurfaceValue(x, y, heightMinFlag);
                                chunk.groupInfo.SetFilterFlagValue(x, y, true);
                                curValue = heightflag;
                            }
                        }


                        float newvalue = Mathf.Lerp(curValue, brush.Targetheight, num * distancePower);

                        //防止拓扑面畸变，补面
                        if (heightflag > targetFlag)
                        {
                            if (heightMinFlag < targetFlag)
                            {
                                chunk.groupInfo.SetSurfaceValue(x, y, brush.Targetheight - 0.1f);
                            }
                            continue;
                        }
                        if (heightMinFlag > targetFlag)
                        {
                            continue;
                        }

                        chunk.groupInfo.SetValue(x, y, newvalue);
                        chunk.groupInfo.SetSurfaceValue(x, y, newvalue);

                    }

                }
            }
        }
        public void Brush(Vector3 point, TerrainChunk chunk, TerrainBrush brush)
        {
            switch (brush.brushType)
            {
                case TerrainBrush.BrushType.BT_ADD:
                    {
                        BrushRaiseEx(point, brush, chunk);
                    }
                    break;
                case TerrainBrush.BrushType.BT_DOWN:
                    {
                        BrushDown(point, brush, chunk);
                    }
                    break;
                case TerrainBrush.BrushType.LOCK:
                    {
                        BrushRaiseEx(point, brush, chunk);
                    }
                    break;
            }
        }

        TerrainBox GetBox(int x, int y)
        {
            int ChunkX = Mathf.FloorToInt(x / 32.0f);
            int ChunkY = Mathf.FloorToInt(y / 32.0f);

            TerrainChunk chunk = Chunks[ChunkY * ChunkColumn + ChunkX];

            int BoxX = x % 32;
            int BoxY = y % 32;

            TerrainBox[] boxs = chunk.boxs;

            return boxs[BoxY * 32 + BoxX];
        }
    }
}


