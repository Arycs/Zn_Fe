using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine;

namespace Zn_Fe.Maps
{
    public class CreateSRPGTileEditor :Editor
    {
        [MenuItem("ZnTools/CreateTile")]
        public static void ShowSelectionSprite()
        {
            var selection = Selection.objects;
            for (int i = 0; i < selection.Length; i++)
            {
                if (selection[i] != null)
                {
                    var tile = CreateInstance<ZnTile>(); 
                    tile.sprite = selection[i] as Sprite;
                    AssetDatabase.CreateAsset(tile, @"Assets/Download/ZnTiles/Palette1_Tiles/" + selection[i].name + ".asset");//在传入的路径中创建资源
                    AssetDatabase.SaveAssets(); //存储资源
                }
            }
            AssetDatabase.Refresh(); //刷新
        }

        public static void ShowA()
        {
            
        }
    }
}