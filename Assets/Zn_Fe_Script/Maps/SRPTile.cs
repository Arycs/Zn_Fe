using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Zn_Fe.Maps
{
    /// <summary>
    /// 自定义Tile
    /// </summary>
    public class SRPGTile
    {
        //public Sprite sprite;
        public TerrainType terrainType;
        public int avo;
        public int heal;
        public int offsetX;
        public int offsetY;
    }

    public class Map : ISerializationCallbackReceiver
    {
        public int x;
        public int y;
        public string mapName;
        public SRPGTile[,] Buildings;
        public List<SRPGTile> tempList = new List<SRPGTile>();
        
        public Map(string mapName ,int col, int row)
        {
            this.mapName = mapName;
            x = col;
            y = row;
            Buildings = new SRPGTile[col, row];
        }

        public void OnBeforeSerialize()
        {
            tempList.Clear();
            for (int i = 0; i < Buildings.GetLength(0); i++)
            {
                for (int j = 0; j < Buildings.GetLength(1); j++)
                {
                    tempList.Add(Buildings[i, j]);
                }
            }
        }

        public void OnAfterDeserialize()
        {
            foreach (SRPGTile baseBuilding in tempList)
            {
                Buildings[baseBuilding.offsetX, baseBuilding.offsetY] = baseBuilding;
            }
        }
    }
}