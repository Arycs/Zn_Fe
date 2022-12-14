using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZnFramework
{
    /// <summary>
    /// 数据表的管理基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="P"></typeparam>
    public abstract class DataTableDBModelBase<T, P>
        where T : class, new() 
        where P: DataTableEntityBase

    {
        protected List<P> m_List;
        protected Dictionary<int, P> m_Dic;

        public DataTableDBModelBase()
        {
            m_List = new List<P>();
            m_Dic = new Dictionary<int, P>();
        }

        #region 需要子类实现的属性或方法

        /// <summary>
        /// 数据表名
        /// </summary>
        public abstract string DataTableName { get; }

        /// <summary>
        /// 加载数据列表
        /// </summary>
        /// <param name="ms"></param>
        protected abstract void LoadList(ZnMemoryStream ms);

        #endregion

        #region 加载数据表数据

        /// <summary>
        /// 加载数据表数据
        /// </summary>
        public void LoadData()
        {
            GameEntry.DataTable.TotalTableCount++;

            GameEntry.DataTable.GetDataTableBuffer(DataTableName, (buffer =>
            {
                using (ZnMemoryStream ms = new ZnMemoryStream(buffer))
                {
                    LoadList(ms);
                }
                GameEntry.Event.CommonEvent.Dispatch(SysEventId.LoadOneDataTableComplete,DataTableName);
            }));
        }

        #endregion

        #region GetList 获取集合

        /// <summary>
        /// 获取集合
        /// </summary>
        /// <returns></returns>
        public List<P> GetList()
        {
            return m_List;
        }

        #endregion
        
        #region Get 根据编号获取实体
        
        /// <summary>
        /// 根据编号获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public P Get(int id)
        {
            if (m_Dic.ContainsKey(id))
            {
                return m_Dic[id];
            }
            return null;
        }

        #endregion
        
        public void Clear()
        {
            m_List.Clear();
            m_Dic.Clear();
        }
    }
}