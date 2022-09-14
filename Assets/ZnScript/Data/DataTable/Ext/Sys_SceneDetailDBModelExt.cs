using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZnFramework;

public partial class DTSys_SceneDetailDBModel
{
    private List<DTSys_SceneDetailEntity> m_retList = new List<DTSys_SceneDetailEntity>();

    /// <summary>
    /// 根据场景编号获取场景明细
    /// </summary>
    /// <param name="sceneId">场景ID</param>
    /// <param name="sceneGrade">场景等级</param>
    /// <returns></returns>
    public List<DTSys_SceneDetailEntity> GetListBySceneId(int sceneId, int sceneGrade)
    {
        m_retList.Clear();
        var lst = this.GetList();
        var len = lst.Count;
        for (var i = 0; i < len; i++)
        {
            var entity = lst[i];
            if (entity.SceneId == sceneId && entity.SceneGrade <= sceneGrade)
            {
                m_retList.Add(entity);
            }
        }

        return m_retList;
    }
}
