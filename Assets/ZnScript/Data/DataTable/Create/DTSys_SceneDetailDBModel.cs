
//===================================================
//作    者：ZnArycs
//备    注：此代码为工具生成 请勿手工修改
//===================================================
using System.Collections;
using System.Collections.Generic;
using System;
using ZnFramework;

/// <summary>
/// DTSys_SceneDetail数据管理
/// </summary>
public partial class DTSys_SceneDetailDBModel : DataTableDBModelBase<DTSys_SceneDetailDBModel, DTSys_SceneDetailEntity>
{
    /// <summary>
    /// 文件名称
    /// </summary>
    public override string DataTableName { get { return "DTSys_SceneDetail"; } }

    /// <summary>
    /// 加载列表
    /// </summary>
    protected override void LoadList(ZnMemoryStream ms)
    {
        int rows = ms.ReadInt();
        int columns = ms.ReadInt();

        for (int i = 0; i < rows; i++)
        {
            DTSys_SceneDetailEntity entity = new DTSys_SceneDetailEntity();
            entity.Id = ms.ReadInt();
            entity.SceneId = ms.ReadInt();
            entity.ScenePath = ms.ReadUTF8String();
            entity.SceneGrade = ms.ReadInt();

            m_List.Add(entity);
            m_Dic[entity.Id] = entity;
        }
    }
}