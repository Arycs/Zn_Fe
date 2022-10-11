using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zn_Fe.Maps;

public class AutoGenerateMap : MonoBehaviour
{
    //TODO 调色盘应该有多个, 在预制体上进行选择即可
    public List<TileBase> Palette_1;
    public Tilemap tilemap;
    
    private List<int> mapdataList = new List<int>();
    [Header("从FEB中导出的xml地图文件")]
    public TextAsset mapXmlSourceFile;
    private int Row;
    private int Col;

    [Button("生成地图")]
    public void OnInspectorGUI()
    {
        ParseXML();
        tilemap.ClearAllTiles();
        for (int i = 0; i < Col; i++)
        {
            for (int j = 0; j < Row; j++)
            {
                var pos = new Vector3Int(j, -i, 0);
                var tile = mapdataList[i * 15 + j];
                tilemap.SetTile(pos,Palette_1[tile]);
            }
        }
        Debug.Log("地图生成完毕, 请查看地图, 确保色块属性设置正确");
        Debug.Log("TODO 图块属性设置比较麻烦, 后续优化");
    }

    void ParseXML()
    {
        mapdataList.Clear();
        XmlDocument xml = new XmlDocument();
        xml.LoadXml(mapXmlSourceFile.text);
        XmlNode list = xml.LastChild;
        Row = int.Parse(list.Attributes["width"].Value);
        Col = int.Parse(list.Attributes["height"].Value);
        var mapdata = list.LastChild["data"];
        foreach (XmlElement temp in mapdata)
        {
            var a = temp.Attributes["gid"];
            mapdataList.Add(int.Parse(a.Value) - 1);
        }
    }
}
