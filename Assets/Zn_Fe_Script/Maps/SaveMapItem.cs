using System.Collections.Generic;

namespace Zn_Fe.Maps
{
    public class SaveMapItem
    {
        public int levelId;
        public int col;
        public int row;
        public int[] walkArea;
        public Dictionary<TerrainType, string[]> allTiles;
        public Dictionary<TerrainType, List<TileOffset>> allTilesOffset;

        public static SaveMapItem GetDefaultSaveMapItem(int col, int row)
        {
            List<TerrainType> terrainTypes = MapEditorUtils.GetListFormEnum<TerrainType>();
            var saveMapItem = new SaveMapItem
            {
                levelId = 1,
                col = col,
                row = row,
                walkArea = new int[col * row],
                allTiles = GetDefaultAllTiles(col, row, terrainTypes),
                allTilesOffset = GetDefaultAllOffsets(terrainTypes)
            };
            return saveMapItem;
        }
        
        private static Dictionary<TerrainType, string[]> GetDefaultAllTiles(int col, int row, List<TerrainType> terrainTypes)
        {
            var result = new Dictionary<TerrainType, string[]>();
            foreach (var item in terrainTypes)
            {
                result.Add(item, new string[col * row]);
            }

            return result;
        }
        
        
        private static Dictionary<TerrainType, List<TileOffset>> GetDefaultAllOffsets(List<TerrainType> terrainTypes)
        {
            var result = new Dictionary<TerrainType, List<TileOffset>>();
            foreach (var item in terrainTypes)
            {
                result.Add(item,new List<TileOffset>());
            }

            return result;
        }
    }
}