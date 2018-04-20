using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimTerrain
{
    public class TerrainBox
    {

        public int v0Index = -1;
        public int v1Index = -1;
        public int v2Index = -1;
        public int v3Index = -1;

        public int v0value = -1;
        public int v1value = -1;
        public int v2value = -1;
        public int v3value = -1;

		public Vector3 ChunkOffSet;

        public TerrainFile groupInfo;
        [System.NonSerialized]
        public LayerTerrainColor colorConfig;
        [System.NonSerialized]
        public LayerTerrainGroups.Chunk chunkConfig;


        public int verticesCount = 0;
        public int verticesStart = 0;
        public int IndicesStart = 0;
        public int indicesCount = 0;

		/// <summary>
		/// 0 defual 1 red 2 green
		/// </summary>
		public int TestCode = 0;


        public Vector3 _v0 = new Vector3();
        public Vector3 _v1 = new Vector3();
        public Vector3 _v2 = new Vector3();
        public Vector3 _v3 = new Vector3();
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

        public TerrainBox(TerrainFile file, LayerTerrainColor _colorConfig, LayerTerrainGroups.Chunk chunkdata)
        {
            groupInfo = file;
            colorConfig = _colorConfig;
            chunkConfig = chunkdata;
        }

		public void BrushBox(TerrainBrush brush, Vector2 point)
		{
			Vector2 localPoint = new Vector2(point.x - ChunkOffSet.x,point.y - ChunkOffSet.z);
			float x = (_v0.x + _v3.x) / 2.0f;
			float y = (_v0.z + _v2.z) / 2.0f;

			float maxx = 0.0f;
			if (localPoint.x > ((_v0.x + _v3.x) / 2.0f)) {
				maxx = _v0.x;		
			} else {
				maxx = _v3.x;
			}

			float maxy = 0.0f;
			if (localPoint.y > (_v0.z + _v1.z) / 2.0f) {

				maxy = _v0.z;
			} else {
				maxy = _v1.z;
			}



			float distance = Vector2.Distance (new Vector2 (maxx, maxy), localPoint);
			if (distance  < brush.Radius) {
				TestCode = 2;

			} else 
			{

				/*
					（x - a）^2 + (y-b)^2 = r^2;
				*/
				float r2 = brush.Radius * brush.Radius;
				// 计算 是否跟上边相交
				float topysubb2 = (_v2.z - localPoint.y) * (_v2.z - localPoint.y);
				float topsqrx = Mathf.Sqrt (r2 - topysubb2);
				float Topleftx = topsqrx + localPoint.x;
				float Toprightx = -topsqrx + localPoint.x;

				int topCross = 0;
				float topCrossValue = -1;
				if (Toprightx >= _v0.x && Toprightx <= _v2.x) 
				{
					topCrossValue = Toprightx;
					topCross++;	
				}

				if (Topleftx >= _v0.x && Topleftx <= _v2.x) 
				{
					topCrossValue = Topleftx;
					topCross++;
				}
				if (topCross == 2) {
					Debug.LogError (2);
				}
				float bottomySubb2 = (_v0.z - localPoint.y) * (_v0.z - localPoint.y);
				float bottomsqr = Mathf.Sqrt (r2 - bottomySubb2);
				float bottomleftx = bottomsqr + localPoint.x;
				float bottomrightx = -bottomsqr + localPoint.x;

				int bottomCross = 0;
				float bottomCrossValue = -1;
				if (bottomrightx >= _v0.x && bottomrightx <= _v2.x) 
				{
					bottomCrossValue = bottomrightx;
					bottomCross++;	
				}

				if (bottomleftx >= _v0.x && bottomleftx <= _v2.x) 
				{
					bottomCrossValue = bottomleftx;
					bottomCross++;
				}

				if (bottomCross == 2) {
					Debug.LogError (2);
				}
				float leftXSub2 = (_v0.x - localPoint.x) * (_v0.x - localPoint.x);
				float leftsqr = Mathf.Sqrt (r2 - leftXSub2);
				float lefttop = leftsqr + localPoint.y;
				float leftbottom = -leftsqr + localPoint.y;

				int leftCross = 0;
				if (lefttop >= _v0.z && lefttop <= _v1.z) {
					leftCross++;
				}

				if (leftbottom >= _v0.z && leftbottom <= _v1.z) {
					leftCross++;
				}
				if (leftbottom == 2) {
					Debug.LogError (2);
				}
				float rightXSub2 = (_v3.x - localPoint.x) * (_v3.x - localPoint.x);
				float rightsqr = Mathf.Sqrt (r2 - rightXSub2);
				float rightTop = rightsqr + localPoint.y;
				float rightbottom = -rightsqr + localPoint.y;

				int rightcross = 0;

				if (rightTop >= _v0.z && rightTop <= _v1.z) {
					rightcross++;
				}

				if (rightbottom >= _v0.z && rightbottom <= _v1.z) {
					rightcross++;
				}
				if (rightbottom == 2) {
					Debug.LogError (2);
				}
				if ((topCross + bottomCross + leftCross + rightcross) == 2) {
					if (topCross == 1 && leftCross == 1) {
					
					} else if (topCross == 1 && rightcross == 1) {
						
					} else if (bottomCross == 1 && leftCross == 1) {
						
					} else if (bottomCross == 1 && rightcross == 1) {
						
					} else if (topCross == 1 && bottomCross == 1) {
						
					} else if (leftCross == 1 && rightcross == 1) {
					
					}else {
						Debug.LogError ("Circle Cross Error");
					}
					TestCode = 1;
				}
				else {
					TestCode = 0;
				}
				//if(
			}
				//
		}

		public bool RebuildTriangle(TerrainChunk chunk, TerrainMeshPool pool, ref TerrainMeshPool.MemoryFlag flag,int boxindex)
        {
            int value0 = groupInfo.CopyData[v0Index];
            int value1 = groupInfo.CopyData[v1Index];
            int value2 = groupInfo.CopyData[v2Index];
            int value3 = groupInfo.CopyData[v3Index];

            int valueSurface0 = groupInfo.CopySurfaceData[v0Index];
            int valueSurface1 = groupInfo.CopySurfaceData[v1Index];
            int valueSurface2 = groupInfo.CopySurfaceData[v2Index];
            int valueSurface3 = groupInfo.CopySurfaceData[v3Index];

            int valueCull0 = groupInfo.copyCullfaceData[v0Index];
            int valueCull1 = groupInfo.copyCullfaceData[v1Index];
            int valueCull2 = groupInfo.copyCullfaceData[v2Index];
            int valueCull3 = groupInfo.copyCullfaceData[v3Index];


            bool hasShowSurface = false;

            if (value0 != valueSurface0)
            {
                hasShowSurface = true;
            }

            if (value1 != valueSurface1)
            {
                hasShowSurface = true;
            }

            if (value2 != valueSurface2)
            {
                hasShowSurface = true;
            }

			if (value3 != valueSurface3)
			{
				hasShowSurface = true;
			}

            bool hasCullFace = false;

            if (value0 != valueCull0)
            {
                hasCullFace = true;
            }

            if (value1 != valueCull1)
            {
                hasCullFace = true;
            }

            if (value2 != valueCull2)
            {
                hasCullFace = true;
            }

            if (value3 != valueCull3)
            {
                hasCullFace = true;
            }


            if (value0 == v0value && value1 == v1value && value2 == v2value && value3 == v3value && hasShowSurface == false && hasCullFace == false)
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
                if (flag.flagOldPoint == false)
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
                v3value = value3;
            }

            float maxValue = (float)ushort.MaxValue;
            _v0.y = (value0 / maxValue) * groupInfo.worldSize.z;
            _v1.y = (value1 / maxValue) * groupInfo.worldSize.z;
            _v2.y = (value2 / maxValue) * groupInfo.worldSize.z;
            _v3.y = (value3 / maxValue) * groupInfo.worldSize.z;

			if (hasCullFace) {
			
				valueCull0 =  (int)((valueCull0 / maxValue) * groupInfo.worldSize.z);
				valueCull1 =  (int)((valueCull1 / maxValue) * groupInfo.worldSize.z);
				valueCull2 =  (int)((valueCull2 / maxValue) * groupInfo.worldSize.z);
				valueCull3 =  (int)((valueCull3 / maxValue) * groupInfo.worldSize.z);
			}

			verticesStart = pool.vertticeIndex;
			verticesCount = 0;
			IndicesStart = pool.IndicesIndex;
			indicesCount = 0;
            //Debug.Log (chunk.name +  v0Index + ":" + v1Index + ":" + v2Index + ":" + v3Index);
            /*switch (TestCode) 
			{
			case 1:
				{
					colorConfig.testSurface = true;
					colorConfig.Red.r = 255;
					colorConfig.Red.b = 0;
					colorConfig.Red.g = 0;
				}break;
		   case 2:
				{
					colorConfig.testSurface = true;
					colorConfig.Red.r = 0;
					colorConfig.Red.b = 0;
					colorConfig.Red.g = 255;
				}
				break;

			}

            */
            groupInfo.CalculateHeightFlag(v0Index);
            groupInfo.CalculateHeightFlag(v1Index);
            groupInfo.CalculateHeightFlag(v2Index);
            groupInfo.CalculateHeightFlag(v3Index);

            //colorConfig.testSurface = hasCullFace;


            if (Mathf.Max(_v0.y, _v2.y) > Mathf.Max(_v1.y, _v3.y))
            {
				int CullFaca = -1;
				if (hasCullFace) {
					CullFaca =  Mathf.Max (valueCull0, valueCull1, valueCull3);
				}

				buildTriangleLeft(pool, _v0, _v1, _v3,CullFaca);
				if (hasCullFace) {
					CullFaca =  Mathf.Max (valueCull2, valueCull1, valueCull3);
				}
				buildTriangleLeft(pool, _v3, _v1, _v2,CullFaca);

                if (hasShowSurface)
                {

                    int minSurfaceValue = Mathf.Min(valueSurface0, valueSurface1, valueSurface3);
                    _v0.y = (minSurfaceValue / maxValue) * groupInfo.worldSize.z;
                    _v1.y = (minSurfaceValue / maxValue) * groupInfo.worldSize.z;
                    _v3.y = (minSurfaceValue / maxValue) * groupInfo.worldSize.z;
                    buildTriangleLeft(pool, _v0, _v1, _v3);
                    minSurfaceValue = Mathf.Min(valueSurface2, valueSurface1, valueSurface3);
                    _v2.y = (minSurfaceValue / maxValue) * groupInfo.worldSize.z;
                    _v1.y = (minSurfaceValue / maxValue) * groupInfo.worldSize.z;
                    _v3.y = (minSurfaceValue / maxValue) * groupInfo.worldSize.z;
                    buildTriangleLeft(pool, _v3, _v1, _v2);
                }

            }
            else
            {
				int CullFaca = -1;
				if (hasCullFace) {
					CullFaca =  Mathf.Max (valueCull0, valueCull1, valueCull2);
				}
				buildTriangleLeft(pool, _v0, _v1, _v2,CullFaca);
				if (hasCullFace) {
					CullFaca =  Mathf.Max (valueCull0, valueCull2, valueCull3);
				}
					        
				buildTriangleLeft(pool, _v0, _v2, _v3,CullFaca);

                if (hasShowSurface)
                {
                    int minSurfaceValue = Mathf.Min(valueSurface0, valueSurface1, valueSurface2);
                    _v0.y = (minSurfaceValue / maxValue) * groupInfo.worldSize.z;
                    _v1.y = (minSurfaceValue / maxValue) * groupInfo.worldSize.z;
                    _v2.y = (minSurfaceValue / maxValue) * groupInfo.worldSize.z;
                    buildTriangleLeft(pool, _v0, _v1, _v2);

                    minSurfaceValue = Mathf.Min(valueSurface0, valueSurface2, valueSurface3);
                    _v0.y = (minSurfaceValue / maxValue) * groupInfo.worldSize.z;
                    _v2.y = (minSurfaceValue / maxValue) * groupInfo.worldSize.z;
                    _v3.y = (minSurfaceValue / maxValue) * groupInfo.worldSize.z;

                    buildTriangleLeft(pool, _v0, _v1, _v2);
                    buildTriangleLeft(pool, _v0, _v2, _v3);

                }
            }
           // colorConfig.testSurface = false;
            pool.vertticeIndex += verticesCount;
			pool.IndicesIndex += indicesCount;

            return true;
        }

		private bool buildTriangleLeft(TerrainMeshPool pool, Vector3 v1, Vector3 v2, Vector3 v3,int CullFaceLayer = -1)
		{
			Vector3[] verticesAry = pool.verticesAry;
			Vector2[] uv0Ary = pool.uvAry;
			Color32[] colorsAry = pool.ColorsAry;
			int[] indicesAry = pool.indicesBuffer;


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
			/*if (CullFaceLayer > 0 && CullFaceLayer < h_max) {
				h_max = Mathf.Max (h_min, CullFaceLayer);
			}*/
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
							colorsAry[verIndex + 2] = colorConfig.GetBottomColor(Spil);
							colorsAry[verIndex + 3] = colorConfig.GetBottomColor(Spil);


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
							colorsAry[verIndex + 2] = colorConfig.GetBottomColor(Spil);
							colorsAry[verIndex + 3] = colorConfig.GetBottomColor(Spil);


							verticesCount += 4;
						}

					}

				}

			}

			return true;
		}

    }

}