
//===================================================
//作    者：ZnArycs
//备    注：此代码为工具生成 请勿手工修改
//===================================================
using System.Collections;
using System.Collections.Generic;
using System;
using ZnFramework;

/// <summary>
/// DTSys_Scene数据管理
/// </summary>
public partial class DTSys_SceneDBModel : DataTableDBModelBase<DTSys_SceneDBModel, DTSys_SceneEntity>
{
    /// <summary>
    /// 文件名称
    /// </summary>
    public override string DataTableName { get { return "DTSys_Scene"; } }

    /// <summary>
    /// 加载列表
    /// </summary>
    protected override void LoadList(ZnMemoryStream ms)
    {
        int rows = ms.ReadInt();
        int columns = ms.ReadInt();

        for (int i = 0; i < rows; i++)
        {
            DTSys_SceneEntity entity = new DTSys_SceneEntity();
            entity.Id = ms.ReadInt();
            entity.Desc = ms.ReadUTF8String();
            entity.Name = ms.ReadUTF8String();
            entity.SceneName = ms.ReadUTF8String();
            entity.BGMId = ms.ReadInt();

            m_List.Add(entity);
            m_Dic[entity.Id] = entity;
        }
    }
}