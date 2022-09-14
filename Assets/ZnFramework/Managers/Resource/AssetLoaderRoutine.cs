using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ZnFramework
{
    /// <summary>
    /// 资源加载器
    /// </summary>
    public class AssetLoaderRoutine
    {
        /// <summary>
        /// 资源加载请求
        /// </summary>
        private AssetBundleRequest m_CurrAssetBundleRequest;

        /// <summary>
        /// 资源请求更新
        /// </summary>
        public Action<float> OnAssetUpdate;

        /// <summary>
        /// 加载资源完毕
        /// </summary>
        public Action<Object> OnLoadAssetComplete;

        /// <summary>
        /// 更新
        /// </summary>
        public void OnUpdate()
        {
            UpdateAssetBundleRequest();
        }

        /// <summary>
        /// 重置
        /// </summary>
        private void Reset()
        {
            m_CurrAssetBundleRequest = null;
        }
        
        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="assetBundle"></param>
        public void LoadAsset(string assetName, AssetBundle assetBundle)
        {
            m_CurrAssetBundleRequest = assetBundle.LoadAssetAsync(assetName);
        }

        private void UpdateAssetBundleRequest()
        {
            if (m_CurrAssetBundleRequest != null)
            {
                if (!m_CurrAssetBundleRequest.isDone) return;
                var obj = m_CurrAssetBundleRequest.asset;
                if (obj !=null)
                {
                    GameEntry.LogInfo(LogCategory.Resource,$"资源=> {obj.name} 加载完毕");
                    Reset();
                    OnLoadAssetComplete?.Invoke(obj);
                }
                else
                {
                    GameEntry.LogInfo(LogCategory.Resource,$"资源=> {obj.name} 加载失败");
                    Reset();
                    OnLoadAssetComplete?.Invoke(null);
                }
            }
            else
            {
                OnAssetUpdate?.Invoke(m_CurrAssetBundleRequest.progress);
            }
        }

    }
}