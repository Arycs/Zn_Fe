// using System.Collections.Generic;
// using System.IO;
// using System.Text;
// using System.Text.RegularExpressions;
// using LitJson;
// using Sirenix.OdinInspector;
// using Sirenix.OdinInspector.Demos.RPGEditor;
// using Sirenix.OdinInspector.Editor;
// using Sirenix.OdinInspector.Editor.Examples;
// using Sirenix.Utilities;
// using Sirenix.Utilities.Editor;
// using UnityEditor;
// using UnityEngine;
// using UnityEngine.UI;
// using Zn_Fe.Maps;
// using Zn_Fe_Script.Utils;
//
// // [CreateAssetMenu(menuName = "EditorMap/Map")]
// public class ZnMapEditor : OdinEditorWindow
// {
//     // [SerializeField]
//
//     [MenuItem("ZnTools/FE/Map Editor")]
//     private static void OpenWindow()
//     {
//         var window = GetWindow<ZnMapEditor>();
//
//         // Nifty little trick to quickly position the window in the middle of the editor.
//         window.position = GUIHelper.GetEditorWindowRect().AlignCenter(1300, 700);
//     }
//
//
//     [TableMatrix(DrawElementMethod = "DrawColoredEnumElement", SquareCells = true, HideColumnIndices = true,
//         HideRowIndices = true, ResizableColumns = false)]
//     [HorizontalGroup("Split", 0.5f, LabelWidth = 20)]
//     [BoxGroup("Split/图块画板")]
//     public SRPGTile[,] MapDrawingBoard = new SRPGTile[1, 1];
//
//     [HorizontalGroup("Split", 0.1f, LabelWidth = 20)] [BoxGroup("Split/图块信息"), LabelText("当前图块"), LabelWidth(50)]
//     public Sprite currSelectSrpgTileSprite;
//
//
//     [BoxGroup("Split/图块信息"), LabelText("当前地形"), LabelWidth(100), OnValueChanged("ChangeSRPGTileProperty")]
//     public TerrainType currSelectSrpgTileTerrainType;
//
//     [BoxGroup("Split/图块信息"), LabelText("当前图块闪避"), LabelWidth(100), OnValueChanged("ChangeSRPGTileProperty")]
//     public int currSelectSrpgTileAvo;
//
//     [BoxGroup("Split/图块信息"), LabelText("当前图块恢复"), LabelWidth(100), OnValueChanged("ChangeSRPGTileProperty")]
//     public int currSelectSrpgTileHeal;
//
//     private int currSelectOffsetX;
//     private int currSelectOffsetY;
//
//     [HorizontalGroup("Split", 0.4f, LabelWidth = 20)]
//     [TableMatrix(DrawElementMethod = "DrawColoredEnumElement1", SquareCells = true, HideColumnIndices = true,
//         HideRowIndices = true, ResizableColumns = false)]
//     [BoxGroup("Split/制作新地图")]
//     public SRPGTile[,] MapEditor = new SRPGTile[1, 1];
//
//     [BoxGroup("Split/图块信息/新建地图"), LabelText("新建地图行数"), LabelWidth(100)]
//     public int newMapCol;
//
//     [BoxGroup("Split/图块信息/新建地图"), LabelText("新建地图列数"), LabelWidth(100)]
//     public int newMapRow;
//
//     [BoxGroup("Split/图块信息/新建地图"), Button("创建新地图")]
//     private void CreateNewMap()
//     {
//         MapEditor = new SRPGTile[newMapCol, newMapRow];
//     }
//
//
//     [BoxGroup("Split/图块信息/新建地图"), Button("保存地图信息到本地")]
//     private void SaveMapToJson()
//     {
//         if (MapEditor != null)
//         {
//             var mapData = new Map("序章", MapEditor.GetLength(0), MapEditor.GetLength(1));
//             mapData.Buildings = MapEditor;
//             var jsonData = JsonUtility.ToJson(mapData, true);
//             IOUtil.CreateTextFile(Application.dataPath + "/Download/MapJson/序章.json", jsonData);
//             AssetDatabase.Refresh();
//         }
//     }
//
//     [BoxGroup("Split/图块信息/加载地图"), LabelText("地图文件"), LabelWidth(100)]
//     public TextAsset mapFile;
//
//     [BoxGroup("Split/图块信息/加载地图"), Button("加载地图信息")]
//     private void LoadMapByFile()
//     {
//         var jsonData = mapFile.text;
//         var col = int.Parse(RegexUtils.RegexMapX(jsonData));
//         var row = int.Parse(RegexUtils.RegexMapY(jsonData));
//         var mapName = RegexUtils.RegexMapName(jsonData);
//         MapEditor = new SRPGTile[col, row];
//         Map map = new Map(mapName, col, row);
//         JsonUtility.FromJsonOverwrite(jsonData, map);
//         MapEditor = map.Buildings;
//     }
//
//     [BoxGroup("Split/图块信息/画板"), LabelText("原画板图"), LabelWidth(100)]
//     public Texture2D sourceMapTexture;
//
//     [BoxGroup("Split/图块信息/画板"), Button("创建画板")]
//     private void CreateData()
//     {
//         if (sourceMapTexture == null)
//         {
//             MapDrawingBoard = new SRPGTile[1, 1];
//             return;
//         }
//
//         Texture2D image = sourceMapTexture;
//         string path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(image)) + "/" + image.name + ".PNG"; //图片路径名称
//         TextureImporter texImp = AssetImporter.GetAtPath(path) as TextureImporter;
//         Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(path);
//         int len = (int) Mathf.Sqrt(sprites.Length);
//         MapDrawingBoard = new SRPGTile[len, len];
//         for (int i = 1; i < sprites.Length; i++)
//         {
//             var names = sprites[i].name.Split('_');
//             var index = int.Parse(names[1]);
//             var row = index / len;
//             var col = index % len;
//             MapDrawingBoard[col, row].sprite = sprites[i] as Sprite;
//             MapDrawingBoard[col, row].offsetX = col;
//             MapDrawingBoard[col, row].offsetY = row;
//         }
//     }
//
//     [BoxGroup("Split/图块信息/画板"), Button("保存画板信息")]
//     private void SaveDrawingBoardToJson()
//     {
//         if (MapDrawingBoard != null)
//         {
//             var SaveMap = new Map("鲁内斯沦陷", MapDrawingBoard.GetLength(0), MapDrawingBoard.GetLength(1))
//             {
//                 Buildings = MapDrawingBoard
//             };
//
//             var jsonData = JsonUtility.ToJson(SaveMap, true);
//             Debug.Log(jsonData);
//             IOUtil.CreateTextFile(Application.dataPath + "/Download/MapDrawBordJson/鲁内斯沦陷.json", jsonData);
//             AssetDatabase.Refresh();
//         }
//     }
//
//     [BoxGroup("Split/图块信息/加载已有画板"), LabelText("地图画板路径"), LabelWidth(100)]
//     public TextAsset drawingBoardFile;
//
//     [BoxGroup("Split/图块信息/加载已有画板"), Button("加载已有画板")]
//     private void LoadMapDrawingBoard()
//     {
//         var jsonData = drawingBoardFile.text;
//         var col = int.Parse(RegexUtils.RegexMapX(jsonData));
//         var row = int.Parse(RegexUtils.RegexMapY(jsonData));
//         var mapName = RegexUtils.RegexMapName(jsonData);
//
//         Map map = new Map(mapName, col, row);
//         JsonUtility.FromJsonOverwrite(jsonData, map);
//         MapDrawingBoard = map.Buildings;
//     }
//
//     private SRPGTile DrawColoredEnumElement1(Rect rect, SRPGTile value)
//     {
//         if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
//         {
//             //点击左键
//             if (Event.current.button == 0)
//             {
//                 if (currSelectSrpgTileSprite != null)
//                 {
//                     value.sprite = currSelectSrpgTileSprite;
//                     value.terrainType = currSelectSrpgTileTerrainType;
//                     value.avo = currSelectSrpgTileAvo;
//                     value.heal = currSelectSrpgTileHeal;
//
//                     //EditorGUI.DrawRect(rect.Padding(1), new Color(0, 0, 0, 0.5f));
//                 }
//             }
//             else if (Event.current.button == 1)
//             {
//                 Debug.Log("aa");
//                 value.sprite = null;
//                 value.terrainType = TerrainType.None;
//                 value.avo = 0;
//                 value.heal = 0;
//             }
//
//             GUI.changed = true;
//             Event.current.Use();
//         }
//
//         if (value.sprite != null)
//         {
//             var targetTex = new Texture2D((int) value.sprite.rect.width, (int) value.sprite.rect.height);
//             var pixels = value.sprite.texture.GetPixels(
//                 (int) value.sprite.textureRect.x,
//                 (int) value.sprite.textureRect.y,
//                 (int) value.sprite.textureRect.width,
//                 (int) value.sprite.textureRect.height);
//             targetTex.SetPixels(pixels);
//             targetTex.Apply();
//             EditorGUI.DrawPreviewTexture(rect.Padding(1), targetTex);
//         }
//         else
//         {
//             EditorGUI.DrawRect(rect.Padding(1), new Color(0, 0, 0, 0.5f)); // Enables dragging of the ItemSlot
//         }
//
//         return value;
//     }
//
//     private SRPGTile DrawColoredEnumElement(Rect rect, SRPGTile value)
//     {
//         if (value.sprite != null)
//         {
//             var targetTex = new Texture2D((int) value.sprite.rect.width, (int) value.sprite.rect.height);
//             var pixels = value.sprite.texture.GetPixels(
//                 (int) value.sprite.textureRect.x,
//                 (int) value.sprite.textureRect.y,
//                 (int) value.sprite.textureRect.width,
//                 (int) value.sprite.textureRect.height);
//             targetTex.SetPixels(pixels);
//             targetTex.Apply();
//
//             EditorGUI.DrawPreviewTexture(rect.Padding(1), targetTex);
//             if (TerrainType.None == value.terrainType)
//             {
//                 EditorGUI.DrawRect(rect.Padding(1), new Color(0, 0, 0, 0.5f)); // Enables dragging of the ItemSlot
//             }
//         }
//         else
//         {
//             EditorGUI.DrawRect(rect.Padding(1), new Color(0, 0, 0, 0.5f)); // Enables dragging of the ItemSlot
//         }
//
//         if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
//         {
//             if (value.sprite != null)
//             {
//                 currSelectSrpgTileSprite = value.sprite;
//                 currSelectSrpgTileTerrainType = value.terrainType;
//                 currSelectSrpgTileAvo = value.avo;
//                 currSelectSrpgTileHeal = value.heal;
//
//                 currSelectOffsetX = value.offsetX;
//                 currSelectOffsetY = value.offsetY;
//             }
//             else
//             {
//                 currSelectSrpgTileSprite = null;
//                 currSelectSrpgTileTerrainType = TerrainType.None;
//                 currSelectSrpgTileAvo = 0;
//                 currSelectSrpgTileHeal = 0;
//                 currSelectOffsetX = 0;
//                 currSelectOffsetY = 0;
//             }
//
//             GUI.changed = true;
//             Event.current.Use();
//         }
//
//         return value;
//     }
//
//     public void ChangeSRPGTileProperty()
//     {
//         if (currSelectSrpgTileSprite == null)
//         {
//             return;
//         }
//
//         MapDrawingBoard[currSelectOffsetX, currSelectOffsetY].terrainType = currSelectSrpgTileTerrainType;
//         MapDrawingBoard[currSelectOffsetX, currSelectOffsetY].avo = currSelectSrpgTileAvo;
//         MapDrawingBoard[currSelectOffsetX, currSelectOffsetY].heal = currSelectSrpgTileHeal;
//     }
// }