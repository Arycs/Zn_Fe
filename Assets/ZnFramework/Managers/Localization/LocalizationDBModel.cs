using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZnFramework;

/// <summary>
/// Localization数据管理
/// </summary>
public partial class LocalizationDBModel : DataTableDBModelBase<LocalizationDBModel, DataTableEntityBase>
{
    /// <summary>
    /// 文件名称
    /// </summary>
    public override string DataTableName => "Localization/"+GameEntry.Localization.CurrLanguage.ToString();

    /// <summary>
    /// 当前语言字典
    /// </summary>
    public Dictionary<string,string> LocalizationDic = new Dictionary<string, string>();
    
    /// <summary>
    /// 加载列表
    /// </summary>
    protected override void LoadList(ZnMemoryStream ms)
    {
        int rows = ms.ReadInt();
        int columns = ms.ReadInt();

        for (int i = 0; i < rows; i++)
        {
            LocalizationDic[ms.ReadUTF8String()] =  ms.ReadUTF8String();
        }
    }
}