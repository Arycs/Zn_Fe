using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ZnFramework
{
    public class DataTableManager : IDisposable
    {
        public int TotalTableCount = 0;
        public int CurrLoadTableCount = 0;

        #region 此处应该添加每个数据表的管理类

        /// <summary>
        /// 系统预置体表
        /// </summary>
        public DTSys_PrefabDBModel Sys_PrefabDBModel { get; private set; }
        
        /// <summary>
        /// UI表
        /// </summary>
        public DTSys_UIFormDBModel Sys_UIFormDBModel { get; private set; }
        
        /// <summary>
        /// 场景表
        /// </summary>
        public DTSys_SceneDBModel Sys_SceneDBModel { get; private set; }
        
        /// <summary>
        /// 系统场景详情表
        /// </summary>
        public DTSys_SceneDetailDBModel Sys_SceneDetailDBModel { get; private set; }
        
        /// <summary>
        /// 本地化表
        /// </summary>
        public LocalizationDBModel LocalizationDBModel { get; private set; }
        
        #endregion
        
        public DataTableManager()
        {
            InitDBModel();
        }


        public void Dispose()
        {
            Clear();
        }
        
        /// <summary>
        /// 初始化DBModel
        /// </summary>
        private void InitDBModel()
        {
            //每个表都需要new一下
            Sys_PrefabDBModel = new DTSys_PrefabDBModel();
            Sys_UIFormDBModel = new DTSys_UIFormDBModel();
            Sys_SceneDBModel = new DTSys_SceneDBModel();
            Sys_SceneDetailDBModel = new DTSys_SceneDetailDBModel();
            LocalizationDBModel = new LocalizationDBModel();
        }

        /// <summary>
        /// 加载表格
        /// </summary>
        private void LoadDataTable()
        {
            //每个表都需要LoadData
            Sys_PrefabDBModel.LoadData();
            Sys_UIFormDBModel.LoadData();
            Sys_SceneDBModel.LoadData();
            Sys_SceneDetailDBModel.LoadData();
            LocalizationDBModel.LoadData();
        }
        
        private void Clear()
        {
            //每个表都Clear
            Sys_PrefabDBModel.Clear();
            Sys_UIFormDBModel.Clear();
            Sys_SceneDBModel.Clear();
            Sys_SceneDetailDBModel.Clear();
            LocalizationDBModel.Clear();
        }

        /// <summary>
        /// 表格资源包
        /// </summary>
        private AssetBundle m_DataTableBundle;

        /// <summary>
        /// 异步加载表格
        /// </summary>
        public void LoadDataTableAsync()
        {
#if DISABLE_ASSETBUNDLE
            LoadDataTable();
#else
            GameEntry.Resource.ResourceLoaderManager.LoadAssetBundle("download/datatable.assetbundle",onComplete: (
                AssetBundle bundle) =>
            {
                m_DataTableBundle = bundle;
                GameEntry.LogInfo(LogCategory.Resource, "LoadDataTableAsync 拿到了 Bundle");
                LoadDataTable();
            });
#endif
        }

        /// <summary>
        /// 获取表格的字节数组
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="onComplete"></param>
        public void GetDataTableBuffer(string tableName, Action<byte[]> onComplete)
        {
#if DISABLE_ASSETBUNDLE
            GameEntry.Time.Yield(() =>
            {
                var buffer =
                    IOUtil.GetFileBuffer($"{GameEntry.Resource.LocalFilePath}/download/DataTable/{tableName}.bytes");
                onComplete?.Invoke(buffer);
            });
#else
            GameEntry.Resource.ResourceLoaderManager.LoadAsset(GameEntry.Resource.GetLastPathName(tableName),m_DataTableBundle,onComplete:
                (Object obj) =>
                {
                    var asset = obj as TextAsset;
                    onComplete?.Invoke(asset.bytes);
                });
#endif
        }
    }
}