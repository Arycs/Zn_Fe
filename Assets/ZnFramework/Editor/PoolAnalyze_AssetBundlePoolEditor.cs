using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEngine;
using ZnFramework;

[CustomEditor(typeof(PoolAnalyze_AssetBundlePool))]
public class PoolAnalyze_AssetBundlePoolEditor :Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        GUILayout.Space(10);

        var titleStyle = new GUIStyle() {normal = {textColor = new Color(102 / 255f, 232 / 255f, 255 / 255f, 1)}};
        if (GameEntry.Pool != null)
        {
            GUILayout.BeginHorizontal("box");
            GUILayout.Label("下次释放剩余时间: " + Mathf.Abs(Time.time - (GameEntry.Pool.ReleaseAssetBundleNextRunTime + GameEntry.Pool.ReleaseAssetInterval)),titleStyle);
            GUILayout.EndHorizontal();
        }
        GUILayout.Space(10);
        GUILayout.BeginVertical("box");
        GUILayout.BeginHorizontal("box");
        GUILayout.Label("资源包");
        GUILayout.Label("剩余时间", GUILayout.Width(50));
        GUILayout.EndHorizontal();

        if (GameEntry.Pool != null)
        {
            foreach (var item in GameEntry.Pool.AssetBundlePool.InspectorDic)
            {
                GUILayout.BeginHorizontal("box");
                GUILayout.Label(item.Key);
                var remain = Mathf.Max(0,
                    GameEntry.Pool.ReleaseAssetBundleInterval - (Time.time - item.Value.LastUseTime));
                titleStyle.fixedWidth = 50;
                GUILayout.Label(remain.ToString(CultureInfo.InvariantCulture),titleStyle);
                GUILayout.EndHorizontal();
            }
        }
        
        GUILayout.EndVertical();
        serializedObject.ApplyModifiedProperties();
        //重绘
        Repaint();
    }
}
