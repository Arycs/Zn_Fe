using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "EditorAssets/MacroSettings")]
public class MacroSettings : ScriptableObject
{
    private string m_Macro;

    [BoxGroup("MacroSettings")]
    [TableList(ShowIndexLabels = true,AlwaysExpanded = true)]
    [HideLabel]
    public MacroData[] Settings;

    [Button]
    public void SaveMacro()
    {
#if UNITY_EDITOR
        m_Macro = string.Empty;
        foreach (var item in Settings)
        {
            if (item.Enable)
            {
                m_Macro += string.Format($"{item.Macro};");
            }

            if (!item.Macro.Equals("DISABLE_ASSETBUNDLE", StringComparison.CurrentCultureIgnoreCase)) continue;
            var arrScene = EditorBuildSettings.scenes;
            foreach (var scene in arrScene)
            {
                if (scene.path.IndexOf("download",StringComparison.CurrentCultureIgnoreCase) > -1)
                {
                    scene.enabled = item.Enable;
                }
            }

            EditorBuildSettings.scenes = arrScene;
        }
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, m_Macro);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, m_Macro);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, m_Macro);
        Debug.LogError("Save Macro Success");
#endif
    }

    private void OnEnable()
    {
#if UNITY_EDITOR
        m_Macro = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
        foreach (var setting in Settings)
        {
            if (!string.IsNullOrEmpty(m_Macro) && m_Macro.IndexOf(setting.Macro, StringComparison.Ordinal) != -1)
            {
                setting.Enable = true;
            }
            else
            {
                setting.Enable = false;
            }
        }
#endif
    }
}

[Serializable]
public class MacroData
{
    [TableColumnWidth(80,Resizable = false)]
    public bool Enable;

    public string Name;

    public string Macro;
}