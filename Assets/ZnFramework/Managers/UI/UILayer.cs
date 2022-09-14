using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZnFramework
{
    /// <summary>
    /// UI层级管理
    /// </summary>
    public class UILayer
    {
        private Dictionary<byte, ushort> m_UILayerDic;

        public UILayer()
        {
            m_UILayerDic = new Dictionary<byte, ushort>();
        }

        /// <summary>
        /// 初始化基础排序
        /// </summary>
        /// <param name="groups"></param>
        internal void Init(UIGroup[] groups)
        {
            var len = groups.Length;
            for (int i = 0; i < len; i++)
            {
                var group = groups[i];
                m_UILayerDic[group.Id] = group.BaseOrder;
            }
        }

        /// <summary>
        /// 设置层级
        /// </summary>
        /// <param name="formBase">UI界面</param>
        /// <param name="isAdd">是否添加层级</param>
        internal void SetSortingOrder(UIFormBase formBase, bool isAdd)
        {
            if (m_UILayerDic.TryGetValue(formBase.GroupId, out _))
            {
                if (isAdd)
                {
                    m_UILayerDic[formBase.GroupId] += 10;
                }
                else
                {
                    m_UILayerDic[formBase.GroupId] -= 10;
                }
                formBase.currCanvas.overrideSorting = true;
                formBase.currCanvas.sortingOrder = m_UILayerDic[formBase.GroupId];
            }
        }
    }
}