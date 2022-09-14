
//===================================================
//作    者：ZnArycs
//备    注：此代码为工具生成 请勿手工修改
//===================================================
using System.Collections;
using System.Collections.Generic;
using System;
using ZnFramework;

/// <summary>
/// DTSys_UIForm数据管理
/// </summary>
public partial class DTSys_UIFormDBModel : DataTableDBModelBase<DTSys_UIFormDBModel, DTSys_UIFormEntity>
{
    /// <summary>
    /// 文件名称
    /// </summary>
    public override string DataTableName { get { return "DTSys_UIForm"; } }

    /// <summary>
    /// 加载列表
    /// </summary>
    protected override void LoadList(ZnMemoryStream ms)
    {
        int rows = ms.ReadInt();
        int columns = ms.ReadInt();

        for (int i = 0; i < rows; i++)
        {
            DTSys_UIFormEntity entity = new DTSys_UIFormEntity();
            entity.Id = ms.ReadInt();
            entity.Desc = ms.ReadUTF8String();
            entity.Name = ms.ReadUTF8String();
            entity.UIGroupId = (byte)ms.ReadByte();
            entity.DisableUILayer = ms.ReadInt();
            entity.IsLock = ms.ReadInt();
            entity.AssetPath_Chinese = ms.ReadUTF8String();
            entity.AssetPath_English = ms.ReadUTF8String();
            entity.CanMulit = ms.ReadBool();
            entity.ShowMode = (byte)ms.ReadByte();

            m_List.Add(entity);
            m_Dic[entity.Id] = entity;
        }
    }
}