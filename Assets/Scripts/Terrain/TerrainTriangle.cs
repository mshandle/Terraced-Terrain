using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace SimTerrain
{
    public class TerrainTriangle
    {
        /// <summary>
        /// 
        /// </summary>
        [System.NonSerialized]
        public TerrainFile groupInfo;
        [System.NonSerialized]
        public LayerTerrainColor colorConfig;
        [System.NonSerialized]
        public LayerTerrainGroups.Chunk chunkConfig;
        /// <summary>
        /// box 在Chunk层的数据索引,不会改变！
        /// </summary>
        public struct HeightDataIndex
        {
            public HeightDataIndex(int v0, int v1, int v2)
            {
                v0Index = v0;
                v1Index = v1;
                v2Index = v2;
            }
            public int v0Index;
            public int v1Index;
            public int v2Index;

        }
        //Cache Value,same no rebuild;
        public int v0value = -1;
        public int v1value = -1;
        public int v2value = -1;
        /// <summary>
        /// box 在Chunk层的数据索引,不会改变！
        /// </summary>
        public HeightDataIndex chunkIndex = new HeightDataIndex();

        public Vector3 _v0 = new Vector3();
        public Vector3 _v1 = new Vector3();
        public Vector3 _v2 = new Vector3();

        /// <summary>
        ///在一个平面的Chunk中，半个BOX的排序。一个Box由个三角形构成。从左下角开始排。
        /// </summary>
        public int halfBoxIndex = 0;
        public MapIndex TraingleIndex = new MapIndex(0, 0);

        public int verticesCount = 0;
        public int verticesStart = 0;
        public int IndicesStart = 0;
        public int indicesCount = 0;

        //Temp Vector

        //切割面
        Vector3 v1_c = new Vector3();
        Vector3 v2_c = new Vector3();
        Vector3 v3_c = new Vector3();
        //底下一个面
        Vector3 v1_b = new Vector3();
        Vector3 v2_b = new Vector3();
        Vector3 v3_b = new Vector3();

        Vector3 v1_c_n = new Vector3();
        Vector3 v1_b_n = new Vector3();


        Vector3 v2_c_n = new Vector3();
        Vector3 v2_b_n = new Vector3();

        public TerrainTriangle()
        {

        }
        public void clear()
        {
           // indexBuffer.Clear();
        }

 

        /// <summary>
        /// 根据高度图，重建三角面
        /// </summary>
        public bool RebuildTriangle(TerrainChunk chunk,TerrainMeshPool pool,ref TerrainMeshPool.MemoryFlag flag, int index)
        {
            if (groupInfo != null)
            {
                int value0 = groupInfo.CopyData[chunkIndex.v0Index];
                int value1 = groupInfo.CopyData[chunkIndex.v1Index];
                int value2 = groupInfo.CopyData[chunkIndex.v2Index];

                int valueSurface0 = groupInfo.CopySurfaceData[chunkIndex.v0Index];
                int valueSurface1 = groupInfo.CopySurfaceData[chunkIndex.v1Index];
                int valueSurface2 = groupInfo.CopySurfaceData[chunkIndex.v2Index];

                bool hasShowSurface = false;

                if(value0 != valueSurface0)
                {
                    hasShowSurface = true;
                }

                if(value1 != valueSurface1)
                {
                    hasShowSurface = true;
                }

                if(value2 != valueSurface2)
                {
                    hasShowSurface = true;
                }

                if (value0 == v0value && value1 == v1value && value2 == v2value && hasShowSurface == false)
                {
                    if (pool.vertticeIndex != verticesStart)
                    {
                        int det = pool.vertticeIndex - verticesStart;
                        int indexCount = indicesCount;
                        for (int idx = 0; idx < indexCount; ++idx)
                        {
                            chunk.indicesAry[IndicesStart + idx] = chunk.indicesAry[IndicesStart + idx] + det;
                        }
                    }

                    if(flag.flagOldPoint == false)
                    {
                        flag.verticeOldStart = verticesStart;
                        flag.indiceOldStart = IndicesStart;
                        flag.flagOldPoint = true;
                    }
                    flag.verticeCount += verticesCount;
                    flag.indiceCount += indicesCount; 

                    verticesStart = pool.vertticeIndex;
                    IndicesStart = pool.IndicesIndex;
                    pool.IndicesIndex += indicesCount;
                    pool.vertticeIndex += verticesCount;
                    return false;
                }
                else
                {
                    v0value = value0;
                    v1value = value1;
                    v2value = value2;
                }

                float maxValue = (float)ushort.MaxValue;
                _v0.y = (value0 / maxValue) * groupInfo.worldSize.z;
                _v1.y = (value1 / maxValue) * groupInfo.worldSize.z;
                _v2.y = (value2 / maxValue) * groupInfo.worldSize.z;

                groupInfo.CalculateHeightFlag(chunkIndex.v0Index);
                groupInfo.CalculateHeightFlag(chunkIndex.v1Index);
                groupInfo.CalculateHeightFlag(chunkIndex.v2Index);
                /*if (hasShowSurface)
                {
                    colorConfig.testSurface = true;
                }*/
               
                buildTriangle(pool,_v0,_v1,_v2);
               
                // buildTriangle(pool, _v0, _v1, _v2);

                if (hasShowSurface)
                {
                    int minSurfaceValue = Mathf.Min(valueSurface0, valueSurface1, valueSurface2);
                    _v0.y = (minSurfaceValue / maxValue) * groupInfo.worldSize.z;
                    _v1.y = (minSurfaceValue / maxValue) * groupInfo.worldSize.z;
                    _v2.y = (minSurfaceValue / maxValue) * groupInfo.worldSize.z;
                    buildTriangle(pool, _v0, _v1, _v2);
                }
                //colorConfig.testSurface = false;
                return true;
            }
            return false;
            
        }
        private bool buildTriangle(TerrainMeshPool pool, Vector3 v1, Vector3 v2, Vector3 v3)
        {
            Vector3[] verticesAry = pool.verticesAry;
            Vector2[] uv0Ary = pool.uvAry;
            Color32[] colorsAry = pool.ColorsAry;
            int[] indicesAry = pool.indicesBuffer;

            verticesStart = pool.vertticeIndex;
            verticesCount = 0;
            IndicesStart = pool.IndicesIndex;
            indicesCount = 0;


            /* Vector3 v1 = _v0;
             Vector3 v2 = _v1;
             Vector3 v3 = _v2;*/

            int h_min = (int)Mathf.Floor(Mathf.Min( Mathf.Min(v1.y, v2.y), v3.y));
            int h_max = (int)Mathf.Floor(Mathf.Max( Mathf.Max(v1.y, v2.y), v3.y));

            //groupInfo.SetHeightFlag()
            float h1 = v1.y;
            float h2 = v2.y;
            float h3 = v3.y;
            int detSpil = 1;
            //bool isMultiLayer = (h_max - h_min) > 3;
            for (int Spil = h_min; Spil <= h_max; Spil += detSpil)
            {
                int point_above = 0;

                if (h1 < Spil)
                {
                    if (h2 < Spil)
                    {
                        if (h3 < Spil)
                        {
                            //所有点都在分割面下部,直接压在最底部
                        }
                        else
                        {
                            //v3 在 上面，根据下面,lerp公式，不需要改变v3的位置
                            //
                            //  t1 = (h1 - h) / (h1 - h3)  // Interpolation value for v1 and v3
                            // t2 = (h2 - h) / (h2 - h3)  // Interpolation value for v2 and v3
                            point_above = 1;
                        }
                    }
                    else
                    {
                        if (h3 < Spil)
                        {
                            point_above = 1;
                            //v2 在 上面，根据下面,交换 v3 和v2 还得保持三角面顺时针的渲染顺序
                            // 比如 正常是 v1->v2->v3 现在交换v2 v3 => v1 v3 v2 变成逆时针了。 v3 v1 v2 只能把v3 放在v1位置
                            //保持三角面的 顺时针的渲染顺序
                            //  t1 = (h1 - h) / (h1 - h3)  // Interpolation value for v1 and v3
                            // t2 = (h2 - h) / (h2 - h3)  // Interpolation value for v2 and v3
                            Vector3 v1Temp = v1;
                            Vector3 v2Temp = v2;
                            Vector3 v3Temp = v3;
                            v1 = v3Temp;
                            v2 = v1Temp;
                            v3 = v2Temp;
                        }
                        else
                        {
                            //v2 v3 都在上面,交换v1 v3
                            point_above = 2;
                            Vector3 v1Temp = v1;
                            Vector3 v2Temp = v2;
                            Vector3 v3Temp = v3;

                            v1 = v2Temp;
                            v2 = v3Temp;
                            v3 = v1Temp;
                        }
                    }
                }
                else
                {
                    if (h2 < Spil)
                    {
                        if (h3 < Spil)
                        {
                            //v1 高于切割面，交换 v1 v3

                            point_above = 1;
                            Vector3 v1Temp = v1;
                            Vector3 v2Temp = v2;
                            Vector3 v3Temp = v3;
                            v1 = v2Temp;
                            v2 = v3Temp;
                            v3 = v1Temp;

                        }
                        else
                        {
                            //v1 v3 高于切割面,交换 v3 v2
                            point_above = 2;

                            Vector3 v1Temp = v1;
                            Vector3 v2Temp = v2;
                            Vector3 v3Temp = v3;
                            v1 = v3Temp;
                            v2 = v1Temp;
                            v3 = v2Temp;
                        }
                    }
                    else
                    {
                        if (h3 < Spil)
                        {
                            //v1 v2 高于切割面 不用交换
                            point_above = 2;
                        }
                        else
                        {
                            point_above = 3;
                            //不需要切割 都高于所有平面
                        }
                    }
                }
                float spilValue = Spil * chunkConfig.LayerHeight;
                //切割面
                v1_c.x = v1.x;
                v1_c.y = spilValue;
                v1_c.z = v1.z;

                v2_c.x = v2.x;
                v2_c.y = spilValue;
                v2_c.z = v2.z;
                //v2_c = new Vector3(v2.x, spilValue, v2.z);
                v3_c.x = v3.x;
                v3_c.y = spilValue;
                v3_c.z = v3.z;
                // v3_c =  new Vector3(v3.x, spilValue, v3.z);
                //底下一个面
                v1_b.x = v1.x;
                v1_b.y = (Spil - detSpil) * chunkConfig.LayerHeight;
                v1_b.z = v1.z;

                //v1_b =  new Vector3(v1.x, (Spil - detSpil) * chunkConfig.LayerHeight, v1.z);
                v2_b.x = v2.x;
                v2_b.y = (Spil - detSpil) * chunkConfig.LayerHeight;
                v2_b.z = v2.z;
                //v2_b =  new Vector3(v2.x, (Spil - detSpil) * chunkConfig.LayerHeight, v2.z);

                v3_b.x = v3.x;
                v3_b.y = (Spil - detSpil) * chunkConfig.LayerHeight;
                v3_b.z = v3.z;
                //v3_b =  new Vector3(v3.x, (Spil - detSpil) * chunkConfig.LayerHeight, v3.z);

                //因为交换过点 重新计算
                h1 = v1.y;
                h2 = v2.y;
                h3 = v3.y;

                if (point_above == 3)
                {
                    int verIndex = verticesStart + verticesCount;
                    int indicesIndex = IndicesStart + indicesCount;

                    indicesAry[indicesIndex + 0] = verIndex + 0;
                    indicesAry[indicesIndex + 1] = verIndex + 1;
                    indicesAry[indicesIndex + 2] = verIndex + 2;
                    indicesCount += 3;

                    verticesAry[verIndex + 0] = v1_c;
                    verticesAry[verIndex + 1] = v2_c;
                    verticesAry[verIndex + 2] = v3_c;

                    colorsAry[verIndex + 0] = colorConfig.GetColorWithLayer(Spil);
                    colorsAry[verIndex + 1] = colorConfig.GetColorWithLayer(Spil);
                    colorsAry[verIndex + 2] = colorConfig.GetColorWithLayer(Spil);

                    uv0Ary[verIndex + 0].x = v1_c.x / chunkConfig.uvReslution.x;
                    uv0Ary[verIndex + 0].y = v1_c.z / chunkConfig.uvReslution.y;

                    //uv0Ary[verIndex + 0] = new Vector2(v1_c.x / chunkConfig.uvReslution.x, v1_c.z / chunkConfig.uvReslution.y);

                    uv0Ary[verIndex + 1].x = v2_c.x / chunkConfig.uvReslution.x;
                    uv0Ary[verIndex + 1].y = v2_c.z / chunkConfig.uvReslution.y;

                    //uv0Ary[verIndex + 1] = new Vector2(v2_c.x / chunkConfig.uvReslution.x, v2_c.z / chunkConfig.uvReslution.y);
                    uv0Ary[verIndex + 2].x = v3_c.x / chunkConfig.uvReslution.x;
                    uv0Ary[verIndex + 2].y = v3_c.z / chunkConfig.uvReslution.y;

                    //uv0Ary[verIndex + 2] = new Vector2(v3_c.x / chunkConfig.uvReslution.x, v3_c.z / chunkConfig.uvReslution.y);

                    verticesCount += 3;


                }
                else if (point_above == 2 || point_above == 1)
                {
                    //continue;
                    //插值计算出 v1---v3 这条线跟平面的焦点

                    float t1 = (h1 - Spil) / (h1 - h3);
                    v1_c_n.x = v1_c.x * (1.0f - t1) + v3_c.x * t1;
                    v1_c_n.y = v1_c.y * (1.0f - t1) + v3_c.y * t1;
                    v1_c_n.z = v1_c.z * (1.0f - t1) + v3_c.z * t1;

                    v1_b_n.x = v1_b.x * (1.0f - t1) + v3_b.x * t1;
                    v1_b_n.y = v1_b.y * (1.0f - t1) + v3_b.y * t1;
                    v1_b_n.z = v1_b.z * (1.0f - t1) + v3_b.z * t1;

                    //Vector3 v1_b_n = Vector3.Lerp(v1_b, v3_b, t1);

                    //插值计算出 v2---v3 这条线跟平面的焦点
                    float t2 = (h2 - Spil) / (h2 - h3);
                    v2_c_n.x = v2_c.x * (1.0f - t2) + v3_c.x * t2;
                    v2_c_n.y = v2_c.y * (1.0f - t2) + v3_c.y * t2;
                    v2_c_n.z = v2_c.z * (1.0f - t2) + v3_c.z * t2;

                    //Vector3 v2_c_n = Vector3.Lerp(v2_c, v3_c, t2);
                    v2_b_n.x = v2_b.x * (1.0f - t2) + v3_b.x * t2;
                    v2_b_n.y = v2_b.y * (1.0f - t2) + v3_b.y * t2;
                    v2_b_n.z = v2_b.z * (1.0f - t2) + v3_b.z * t2;

                    //Vector3 v2_b_n = Vector3.Lerp(v2_b, v3_b, t2);


                    if (point_above == 1)
                    {

                        {
                            int verIndex = verticesStart + verticesCount;
                            int indicesIndex = IndicesStart + indicesCount;

                            indicesAry[indicesIndex + 0] = verIndex + 0;
                            indicesAry[indicesIndex + 1] = verIndex + 1;
                            indicesAry[indicesIndex + 2] = verIndex + 2;
                            indicesCount += 3;

                            //添加一个上部的被切平面

                            verticesAry[verIndex + 0] = v3_c;
                            verticesAry[verIndex + 1] = v1_c_n;
                            verticesAry[verIndex + 2] = v2_c_n;

                            uv0Ary[verIndex + 0].x = v3_c.x / chunkConfig.uvReslution.x;
                            uv0Ary[verIndex + 0].y = v3_c.z / chunkConfig.uvReslution.y;

                            //uv0Ary[verIndex + 0] = new Vector2(v3_c.x / chunkConfig.uvReslution.x, v3_c.z / chunkConfig.uvReslution.y);
                            uv0Ary[verIndex + 1].x = v1_c_n.x / chunkConfig.uvReslution.x;
                            uv0Ary[verIndex + 1].y = v1_c_n.z / chunkConfig.uvReslution.y;
                            //uv0Ary[verIndex + 1] = new Vector2(v1_c_n.x / chunkConfig.uvReslution.x, v1_c_n.z / chunkConfig.uvReslution.y);
                            uv0Ary[verIndex + 2].x = v2_c_n.x / chunkConfig.uvReslution.x;
                            uv0Ary[verIndex + 2].y = v2_c_n.z / chunkConfig.uvReslution.y;
                            //uv0Ary[verIndex + 2] = new Vector2(v2_c_n.x / chunkConfig.uvReslution.x, v2_c_n.z / chunkConfig.uvReslution.y);



                            colorsAry[verIndex + 0] = colorConfig.GetColorWithLayer(Spil);
                            colorsAry[verIndex + 1] = colorConfig.GetColorWithLayer(Spil);
                            colorsAry[verIndex + 2] = colorConfig.GetColorWithLayer(Spil);


                            verticesCount += 3;
                        }

                        //添加一堵墙

                        {
                            int verIndex = verticesStart + verticesCount;
                            int indicesIndex = IndicesStart + indicesCount;

                            indicesAry[indicesIndex + 0] = verIndex + 0;
                            indicesAry[indicesIndex + 1] = verIndex + 1;
                            indicesAry[indicesIndex + 2] = verIndex + 3;

                            indicesAry[indicesIndex + 3] = verIndex + 1;
                            indicesAry[indicesIndex + 4] = verIndex + 2;
                            indicesAry[indicesIndex + 5] = verIndex + 3;
                            indicesCount += 6;

                            verticesAry[verIndex + 0] = v2_c_n;
                            verticesAry[verIndex + 1] = v1_c_n;
                            verticesAry[verIndex + 2] = v1_b_n;
                            verticesAry[verIndex + 3] = v2_b_n;

                            uv0Ary[verIndex + 0].x = v2_c_n.x / chunkConfig.uvReslution.x;
                            uv0Ary[verIndex + 0].y = v2_c_n.z / chunkConfig.uvReslution.y;
                            //uv0Ary[verIndex + 0] = new Vector2(v2_c_n.x / chunkConfig.uvReslution.x, v2_c_n.z / chunkConfig.uvReslution.y);
                            uv0Ary[verIndex + 1].x = v1_c_n.x / chunkConfig.uvReslution.x;
                            uv0Ary[verIndex + 1].y = v1_c_n.z / chunkConfig.uvReslution.y;
                            //uv0Ary[verIndex + 1] = new Vector2(v1_c_n.x / chunkConfig.uvReslution.x, v1_c_n.z / chunkConfig.uvReslution.y);
                            uv0Ary[verIndex + 2].x = v1_b_n.x / chunkConfig.uvReslution.x;
                            uv0Ary[verIndex + 2].y = v1_b_n.z / chunkConfig.uvReslution.y;
                            //uv0Ary[verIndex + 2] = new Vector2(v1_b_n.x / chunkConfig.uvReslution.x, v1_b_n.z / chunkConfig.uvReslution.y);
                            uv0Ary[verIndex + 3].x = v2_b_n.x / chunkConfig.uvReslution.x;
                            uv0Ary[verIndex + 3].y = v2_b_n.z / chunkConfig.uvReslution.y;
                            //uv0Ary[verIndex + 3] = new Vector2(v2_b_n.x / chunkConfig.uvReslution.x, v2_b_n.z / chunkConfig.uvReslution.y);



                            colorsAry[verIndex + 0] = colorConfig.GetColorWithLayer(Spil);
                            colorsAry[verIndex + 1] = colorConfig.GetColorWithLayer(Spil);
                            colorsAry[verIndex + 2] = colorConfig.bottomColor;
                            colorsAry[verIndex + 3] = colorConfig.bottomColor;


                            verticesCount += 4;

                        }
                    }
                    else
                    {
                        {

                            int verIndex = verticesStart + verticesCount;
                            int indicesIndex = IndicesStart + indicesCount;

                            indicesAry[indicesIndex + 0] = verIndex + 0;
                            indicesAry[indicesIndex + 1] = verIndex + 1;
                            indicesAry[indicesIndex + 2] = verIndex + 2;
                            indicesAry[indicesIndex + 3] = verIndex + 2;
                            indicesAry[indicesIndex + 4] = verIndex + 3;
                            indicesAry[indicesIndex + 5] = verIndex + 0;
                            indicesCount += 6;


                            verticesAry[verIndex + 0] = v1_c;
                            verticesAry[verIndex + 1] = v2_c;
                            verticesAry[verIndex + 2] = v2_c_n;
                            verticesAry[verIndex + 3] = v1_c_n;

                            uv0Ary[verIndex + 0].x = v1_c.x / chunkConfig.uvReslution.x;
                            uv0Ary[verIndex + 0].y = v1_c.z / chunkConfig.uvReslution.y;
                            //uv0Ary[verIndex + 0] = new Vector2(v1_c.x / chunkConfig.uvReslution.x, v1_c.z / chunkConfig.uvReslution.y);
                            uv0Ary[verIndex + 1].x = v2_c.x / chunkConfig.uvReslution.x;
                            uv0Ary[verIndex + 1].y = v2_c.z / chunkConfig.uvReslution.y;
                            //uv0Ary[verIndex + 1] = new Vector2(v2_c.x / chunkConfig.uvReslution.x, v2_c.z / chunkConfig.uvReslution.y);
                            uv0Ary[verIndex + 2].x = v2_c_n.x / chunkConfig.uvReslution.x;
                            uv0Ary[verIndex + 2].y = v2_c_n.z / chunkConfig.uvReslution.y;
                            //uv0Ary[verIndex + 2] = new Vector2(v2_c_n.x / chunkConfig.uvReslution.x, v2_c_n.z / chunkConfig.uvReslution.y);
                            uv0Ary[verIndex + 3].x = v1_c_n.x / chunkConfig.uvReslution.x;
                            uv0Ary[verIndex + 3].y = v1_c_n.z / chunkConfig.uvReslution.y;
                            //uv0Ary[verIndex + 3] = new Vector2(v1_c_n.x / chunkConfig.uvReslution.x, v1_c_n.z / chunkConfig.uvReslution.y);



                            colorsAry[verIndex + 0] = colorConfig.GetColorWithLayer(Spil);
                            colorsAry[verIndex + 1] = colorConfig.GetColorWithLayer(Spil);
                            colorsAry[verIndex + 2] = colorConfig.GetColorWithLayer(Spil);
                            colorsAry[verIndex + 3] = colorConfig.GetColorWithLayer(Spil);


                            verticesCount += 4;

                        }

                        //添加一堵墙
                        {
                            int verIndex = verticesStart + verticesCount;
                            int indicesIndex = IndicesStart + indicesCount;


                            indicesAry[indicesIndex + 0] = verIndex + 0;
                            indicesAry[indicesIndex + 1] = verIndex + 1;
                            indicesAry[indicesIndex + 2] = verIndex + 2;
                            indicesAry[indicesIndex + 3] = verIndex + 0;
                            indicesAry[indicesIndex + 4] = verIndex + 2;
                            indicesAry[indicesIndex + 5] = verIndex + 3;
                            indicesCount += 6;


                            verticesAry[verIndex + 0] = v1_c_n;
                            verticesAry[verIndex + 1] = v2_c_n;
                            verticesAry[verIndex + 2] = v2_b_n;
                            verticesAry[verIndex + 3] = v1_b_n;

                            uv0Ary[verIndex + 0].x = v1_c_n.x / chunkConfig.uvReslution.x;
                            uv0Ary[verIndex + 0].y = v1_c_n.z / chunkConfig.uvReslution.y;
                            //uv0Ary[verIndex + 0] = new Vector2(v1_c_n.x / chunkConfig.uvReslution.x, v1_c_n.z / chunkConfig.uvReslution.y);
                            uv0Ary[verIndex + 1].x = v2_c_n.x / chunkConfig.uvReslution.x;
                            uv0Ary[verIndex + 1].y = v2_c_n.z / chunkConfig.uvReslution.y;
                            //uv0Ary[verIndex + 1] = new Vector2(v2_c_n.x / chunkConfig.uvReslution.x, v2_c_n.z / chunkConfig.uvReslution.y);
                            uv0Ary[verIndex + 2].x = v2_b_n.x / chunkConfig.uvReslution.x;
                            uv0Ary[verIndex + 2].y = v2_b_n.z / chunkConfig.uvReslution.y;
                            //uv0Ary[verIndex + 2] = new Vector2(v2_b_n.x / chunkConfig.uvReslution.x, v2_b_n.z / chunkConfig.uvReslution.y);
                            uv0Ary[verIndex + 3].x = v1_b_n.x / chunkConfig.uvReslution.x;
                            uv0Ary[verIndex + 3].y = v1_b_n.z / chunkConfig.uvReslution.y;
                            //uv0Ary[verIndex + 3] = new Vector2(v1_b_n.x / chunkConfig.uvReslution.x, v1_b_n.z / chunkConfig.uvReslution.y);
                            colorsAry[verIndex + 0] = colorConfig.GetColorWithLayer(Spil);
                            colorsAry[verIndex + 1] = colorConfig.GetColorWithLayer(Spil);
                            colorsAry[verIndex + 2] = colorConfig.bottomColor;
                            colorsAry[verIndex + 3] = colorConfig.bottomColor;


                            verticesCount += 4;
                        }

                    }

                }

            }
            pool.vertticeIndex += verticesCount;
            pool.IndicesIndex += indicesCount;
            return true;
        }
        public bool RebuildTriangleEx(TerrainChunk chunk, TerrainMeshPool pool, ref TerrainMeshPool.MemoryFlag flag)
        {

            Vector3[] verticesAry = pool.verticesAry;
            Vector2[] uv0Ary = pool.uvAry;
            Color32[] colorsAry = pool.ColorsAry;
            int[] indicesAry = pool.indicesBuffer;

            float value0 = groupInfo.GetValue(chunkIndex.v0Index);
            float value1 = groupInfo.GetValue(chunkIndex.v1Index);
            float value2 = groupInfo.GetValue(chunkIndex.v2Index);
            float maxValue = (float)ushort.MaxValue;
            _v0.y = (value0 / maxValue) * groupInfo.worldSize.z;
            _v1.y = (value1 / maxValue) * groupInfo.worldSize.z;
            _v2.y = (value2 / maxValue) * groupInfo.worldSize.z;

            verticesStart = pool.vertticeIndex;
            verticesCount = 0;
            IndicesStart = pool.IndicesIndex;
            indicesCount = 0;



            verticesAry[verticesStart] = (_v0);
            verticesAry[verticesStart + 1] = (_v1);
            verticesAry[verticesStart + 2] = (_v2);

            colorsAry[verticesStart + 0] = colorConfig.bottomColor;
            colorsAry[verticesStart + 1] = colorConfig.bottomColor;
            colorsAry[verticesStart + 2] = colorConfig.bottomColor;

            uv0Ary[verticesStart + 0] = (new Vector2(_v0.x / chunkConfig.uvReslution.x, _v0.z / chunkConfig.uvReslution.y));
            uv0Ary[verticesStart + 1] = (new Vector2(_v1.x / chunkConfig.uvReslution.x, _v1.z / chunkConfig.uvReslution.y));
            uv0Ary[verticesStart + 2] = (new Vector2(_v2.x / chunkConfig.uvReslution.x, _v2.z / chunkConfig.uvReslution.y));


            indicesAry[IndicesStart + 0] = verticesStart;
            indicesAry[IndicesStart + 1] = verticesStart + 1;
            indicesAry[IndicesStart + 2] = verticesStart + 2;

            verticesCount = 3;
            indicesCount = 3;

            pool.vertticeIndex += 3;
            pool.IndicesIndex += 3;
            return true;
            /*
            clear();
            //TODO:
            if (groupInfo != null)
            {

                float value0 = groupInfo.GetValue(chunkIndex.v0Index);
                float value1 = groupInfo.GetValue(chunkIndex.v1Index);
                float value2 = groupInfo.GetValue(chunkIndex.v2Index);
				float maxValue  = (float)ushort.MaxValue;
				_v0.y = (value0 / maxValue) * groupInfo.worldSize.z;
				_v1.y = (value1 / maxValue) * groupInfo.worldSize.z;
				_v2.y = (value2 / maxValue) * groupInfo.worldSize.z;

                verticesAry[0] = (_v0);
                verticesAry[1] = (_v1);
                verticesAry[2] = (_v2);

                colorsAry[0] = colorConfig.bottomColor;
                colorsAry[1] = colorConfig.bottomColor;
                colorsAry[2] = colorConfig.bottomColor;

                uv0Ary[0] = (new Vector2(_v0.x / chunkConfig.uvReslution.x, _v0.z / chunkConfig.uvReslution.y));
                uv0Ary[1] = (new Vector2(_v1.x / chunkConfig.uvReslution.x, _v1.z / chunkConfig.uvReslution.y));
                uv0Ary[2] = (new Vector2(_v2.x / chunkConfig.uvReslution.x, _v2.z / chunkConfig.uvReslution.y));


                indexBuffer.Add(IndexBufferIndex + 0);
                indexBuffer.Add(IndexBufferIndex + 1);
                indexBuffer.Add(IndexBufferIndex + 2);



                verticesCount = 3;

    

               return IndexBufferIndex + 3;
            }

            return IndexBufferIndex;
            */
        }

    }

}

