using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SimTerrain
{
    /// <summary>
    /// 地形文件，读取二进制的地形文件
    /// </summary>
    /// 
    [System.Serializable]
    public class TerrainFile
    {
        public string name;
        /// <summary>
        /// 地形文件的 分辨率;
        /// 2^n+1 :33 65 129 257 513 最小值33
        /// </summary>
        public int terrainWidth = 257;

        /// <summary>
        /// 地形文件的 分辨率;
        /// 2^n+1 :33 65 129 257 513 最小值33
        /// </summary>
        /// 
        
        public int terrainLength = 257;
        /// <summary>
        /// 地形的坐标
        /// </summary>
        public Vector3 worldPositon = new Vector3(0,0,0);
        /// <summary>
        /// 这块地形的大小，Z轴表示地形高度 32层 32m
        /// </summary>
        public Vector3 worldSize = new Vector3(256.0f,256.0F,33.0F);

        /// <summary>
        /// 二维数组，表示高度文件
        /// </summary>
        /// 
        [HideInInspector]
        public ushort[] data = null;
        public ushort[] Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
            }
        }
        [HideInInspector]
        public ushort[] surfaceData = null;

        [HideInInspector]
        public ushort[] CullfaceData = null;

        [HideInInspector]
        [System.NonSerialized]
        public ushort[] CopyData = null;
        [HideInInspector]
        [System.NonSerialized]
        public ushort[] CopySurfaceData = null;

        [HideInInspector]
        [NonSerialized]
        public ushort[] copyCullfaceData = null;

        [HideInInspector]
        [System.NonSerialized]
        public ushort[] HeigthFlagData = null;
        public ushort[] HeigtMinFlagData = null;


        public bool[] FilterFlag = null;


        public void Refresh()
        {
            //copy data to Copy Data;
            CopyData = new ushort[data.Length];
            CopySurfaceData = new ushort[data.Length];
            copyCullfaceData = new ushort[data.Length];
            HeigthFlagData = new ushort[data.Length];
            HeigtMinFlagData = new ushort[data.Length];
            FilterFlag = new bool[data.Length];

            if (surfaceData == null)
            {
                surfaceData = new ushort[data.Length];
                Array.Copy(data, surfaceData, data.Length);
            }

            if (CullfaceData == null)
            {
                CullfaceData = new ushort[data.Length];
                Array.Copy(data, CullfaceData, data.Length);
            }

            Array.Clear(HeigtMinFlagData, 0, HeigthFlagData.Length);
            Array.Clear(HeigthFlagData, 0, HeigthFlagData.Length);
            Array.Copy(data, CopyData, data.Length);
            Array.Copy(surfaceData, CopySurfaceData, surfaceData.Length);
            Array.Copy(CullfaceData, copyCullfaceData, CullfaceData.Length);
            Array.Clear(FilterFlag, 0, FilterFlag.Length);
        }

        public void SaveData()
        {

            Array.Copy(CopyData, data, CopyData.Length);
            Array.Copy(CopySurfaceData, surfaceData, CopySurfaceData.Length);
            Array.Copy(copyCullfaceData, CullfaceData, copyCullfaceData.Length);

        }
        /// <summary>
        /// 
        /// </summary>
        public TerrainFile()
        {

        }
        /// <summary>
        /// 初始化地形文件类
        /// </summary>
        /// <param name="indata"></param>
        /// <returns></returns>
        public bool Init (Byte[] indata)
        {
            int dataLenght = indata.Length;
            if(dataLenght < 33 * 33)
            {
                UnityEngine.Debug.LogErrorFormat("[TerrainFile:Init] data lenght less  minvalue 33*33");
                return false;
            }
            terrainWidth = Mathf.FloorToInt(Mathf.Sqrt(dataLenght));
            terrainLength = terrainWidth;

            //拷贝一份，后面读取写入比较方便，不需要通过 BinaryReader 或者 BinaryWirter
            try
            {
                data = new ushort[terrainWidth * terrainLength];
            }
            catch(Exception e)
            {
                Debug.LogError(e.ToString());
            }

            
            for (int idy = 0; idy < terrainLength; ++idy)
            {
                for(int idx = 0; idx < terrainWidth; ++idx)
                {
                    data[idy * terrainWidth + idx] = indata[idy * terrainWidth + idx];
                }
            }
            
            return true;
        }
        /// <summary>
        /// 根据索引取数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int GetValue(int index)
        {
            return CopyData[index];
        }

        public int GetValue(int x, int y)
        {
            int index = Mathf.Min(y * terrainWidth + x, (terrainWidth * terrainLength) - 1);
            return CopyData[index];
        }

        public float GetRealHeight(int x,int y)
        {
            int index = Mathf.Min(y * terrainWidth + x, (terrainWidth * terrainLength) - 1);
			float step = (float)GetValue(index) / ushort.MaxValue;
            return step * worldSize.z;
        }

        public float GetRealHeight(int index)
        {
            float step = (float)GetValue(index) / 65535.0f;
            return step * worldSize.z;
        }
        /// <summary>
        /// 根据索引写入数据
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void SetValue(int index, ushort value)
        {
            CopyData[index] = value;
        }

        public void SetValue(int x,int y,ushort value)
        {
            int index = Mathf.Min(y * terrainWidth + x, (terrainWidth * terrainLength) - 1);
            SetValue(index, value);
        }

        public void SetValue(int x,int y, float realheigth)
        {
			ushort value =(ushort)((realheigth / worldSize.z) * ushort.MaxValue);
            SetValue(x, y, value);
        }

        public void SetSurfaceValue(int index, ushort value)
        {
            CopySurfaceData[index] = value;
        }

        public void SetSurfaceValue(int index,float realHeight)
        {
            ushort value = (ushort)((realHeight / worldSize.z) * ushort.MaxValue);
            SetSurfaceValue(index, value);
        }

        public void SetSurfaceValue(int x, int y, float realheigth)
        {
            int index = Mathf.Min(y * terrainWidth + x, (terrainWidth * terrainLength) - 1);
            SetSurfaceValue(index, realheigth);
        }
        public int GetSurfaceValue(int x, int y)
        {
            int index = Mathf.Min(y * terrainWidth + x, (terrainWidth * terrainLength) - 1);
            return CopySurfaceData[index];
        }

        public float GetSurfaceRealValue(int x, int y)
        {
            int index = Mathf.Min(y * terrainWidth + x, (terrainWidth * terrainLength) - 1);
            return  (CopySurfaceData[index] / 65535.0f) * worldSize.z;
        }

        public ushort GetHeightIndex(int x,int y)
        {
            int index = Mathf.Min(y * terrainWidth + x, (terrainWidth * terrainLength) - 1);
            return GetHeightIndex(index);
        }

        public ushort GetHeightIndex(int index)
        {
            return HeigthFlagData[index];
        }
        public ushort GetMinHeightIndex(int x, int y)
        {
            int index = Mathf.Min(y * terrainWidth + x, (terrainWidth * terrainLength) - 1);
            return GetMinHeightIndex(index);
        }

        public ushort GetMinHeightIndex(int index)
        {
            return HeigtMinFlagData[index];
        }


        private int GetHeigthFlag(int x,int y)
        {
            if (x < 0 || x >= terrainWidth)
                return 0;
            if (y < 0 || y >= terrainLength)
                return 0;
            return Mathf.FloorToInt(GetSurfaceRealValue(x, y));
        }


        public ushort GetCullFace(int x, int y)
        {
            int index = Mathf.Min(y * terrainWidth + x, (terrainWidth * terrainLength) - 1);
            return GetCullFace(index);
        }

        public ushort GetCullFace(int index)
        {
            return copyCullfaceData[index];
        }

        public void SetCullFace(int x, int y,float cullface)
        {
            ushort value = (ushort)((cullface / worldSize.z) * ushort.MaxValue);
            int index = Mathf.Min(y * terrainWidth + x, (terrainWidth * terrainLength) - 1);
            copyCullfaceData[index] = value;
        }

        public void ClearFilterFlag()
        {
            Array.Clear(FilterFlag, 0, FilterFlag.Length);
        }
        public bool GetFilterFlagValue(int x, int y)
        {
            int index = Mathf.Min(y * terrainWidth + x, (terrainWidth * terrainLength) - 1);
            return FilterFlag[index];
        }

        public void SetFilterFlagValue(int x,int y,bool value)
        {
            int index = Mathf.Min(y * terrainWidth + x, (terrainWidth * terrainLength) - 1);
             FilterFlag[index] = value;
        }


        public void CalculateHeightFlag(int index)
        {
            //center 
            int x = index % terrainWidth;
            int y = Mathf.FloorToInt(index / terrainWidth);

            int center = GetHeigthFlag(x, y);

            int left = GetHeigthFlag(x - 1, y);
            int right = GetHeigthFlag(x + 1, y);
            int top = GetHeigthFlag(x, y + 1);
            int bottom = GetHeigthFlag(x, y - 1);

            int leftTop = GetHeigthFlag(x - 1, y + 1);
            int leftbottom = GetHeigthFlag(x - 1, y - 1);
            int rightTop = GetHeigthFlag(x + 1, y + 1);
            int rightbottom = GetHeigthFlag(x + 1, y - 1);

            int max = Mathf.Max(center, left, right, top, bottom, leftTop, leftbottom, rightTop , rightbottom);
            int min = Mathf.Min(center, left, right, top, bottom, leftTop, leftbottom, rightTop , rightbottom);
            HeigthFlagData[index] = (ushort)max;
            HeigtMinFlagData[index] = (ushort)min;
        }

    }
}
