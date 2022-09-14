using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

public class ZnEditor : OdinMenuEditorWindow
{
    [MenuItem("ZnTools/ZnEditor")]
    private static void OpenEditor()
    {
        var window = GetWindow<ZnEditor>();
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(700, 700);
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree();
        tree.AddAssetAtPath("MacroSettings", "ZnFramework/ZnAssets/MacroSettings.asset").AddIcon(EditorIcons.Airplane);
        tree.AddAssetAtPath("AssetbundleSettings", "ZnFramework/ZnAssets/AssetBundleSettings.asset").AddIcon(EditorIcons.Bell);
        tree.AddAssetAtPath("ParamsSettings", "ZnFramework/ZnAssets/ParamsSettings.asset").AddIcon(EditorIcons.SettingsCog);
        tree.AddAssetAtPath("PoolAnalyze/ClassObjectPool", "ZnFramework/ZnAssets/PoolAnalyze_ClassObjectPool.asset").AddIcon(EditorIcons.FileCabinet);
        tree.AddAssetAtPath("PoolAnalyze/AssetPool", "ZnFramework/ZnAssets/PoolAnalyze_AssetPool.asset").AddIcon(EditorIcons.FileCabinet);
        tree.AddAssetAtPath("PoolAnalyze/AssetBundlePool", "ZnFramework/ZnAssets/PoolAnalyze_AssetBundlePool.asset").AddIcon(EditorIcons.FileCabinet);
        return tree;
    }
}
