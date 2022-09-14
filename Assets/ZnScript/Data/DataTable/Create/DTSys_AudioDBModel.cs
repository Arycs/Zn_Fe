
//===================================================
//作    者：ZnArycs
//备    注：此代码为工具生成 请勿手工修改
//===================================================
using System.Collections;
using System.Collections.Generic;
using System;
using ZnFramework;

/// <summary>
/// DTSys_Audio数据管理
/// </summary>
public partial class DTSys_AudioDBModel : DataTableDBModelBase<DTSys_AudioDBModel, DTSys_AudioEntity>
{
    /// <summary>
    /// 文件名称
    /// </summary>
    public override string DataTableName { get { return "DTSys_Audio"; } }

    /// <summary>
    /// 加载列表
    /// </summary>
    protected override void LoadList(ZnMemoryStream ms)
    {
        int rows = ms.ReadInt();
        int columns = ms.ReadInt();

        for (int i = 0; i < rows; i++)
        {
            DTSys_AudioEntity entity = new DTSys_AudioEntity();
            entity.Id = ms.ReadInt();
            entity.Desc = ms.ReadUTF8String();
            entity.AssetPath = ms.ReadUTF8String();
            entity.Is3D = ms.ReadInt();
            entity.volume = ms.ReadFloat();

            m_List.Add(entity);
            m_Dic[entity.Id] = entity;
        }
    }
}