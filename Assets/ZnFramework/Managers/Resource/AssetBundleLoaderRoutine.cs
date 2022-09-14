using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZnFramework
{
    /// <summary>
    /// AssetBundle 资源加载器
    /// </summary>
    public class AssetBundleLoaderRoutine
    {
        /// <summary>
        /// 当前的资源包信息
        /// </summary>
        private AssetBundleInfoEntity m_CurrAssetBundleInfo;

        /// <summary>
        /// 资源包创建请求
        /// </summary>
        private AssetBundleCreateRequest m_CurrAssetBundleCreateRequest;

        /// <summary>
        /// 资源包创建请求更新
        /// </summary>
        public Action<float> OnAssetBundleCreateUpdate;

        /// <summary>
        /// 加载资源包完毕
        /// </summary>
        public Action<AssetBundle> OnLoadAssetBundleComplete;
        
        /// <summary>
        /// 重置
        /// </summary>
        private void Reset()
        {
            m_CurrAssetBundleCreateRequest = null;
        }

        
        public void OnUpdate()
        {
            UpdateAssetBundleCreateRequest();
        }

        #region LoadAssetBundle 加载资源包

        /// <summary>
        /// 加载资源包
        /// </summary>
        /// <param name="assetBundlePath"></param>
        public void LoadAssetBundle(string assetBundlePath)
        {
            m_CurrAssetBundleInfo = GameEntry.Resource.ResourceManager.GetAssetBundleInfo(assetBundlePath);
            GameEntry.Resource.ResourceManager.StreamingAssetsManager.ReadAssetBundle(assetBundlePath,
                LoadAssetBundleAsync);
        }

        #endregion

        #region LoadAssetBundleAsync 异步加载资源包

        /// <summary>
        /// 异步加载资源包
        /// </summary>
        /// <param name="buffer"></param>
        private void LoadAssetBundleAsync(byte[] buffer)
        {
            if (m_CurrAssetBundleInfo.IsEncrypt)
            {
                buffer = SecurityUtil.Xor(buffer);
            }

            m_CurrAssetBundleCreateRequest = AssetBundle.LoadFromMemoryAsync(buffer);
        }

        #endregion

        #region UpdateAssetBundleCreateRequest 更新资源包请求

        /// <summary>
        /// 更新资源包请求
        /// </summary>
        private void UpdateAssetBundleCreateRequest()
        {
            if (m_CurrAssetBundleCreateRequest == null) return;
            if (m_CurrAssetBundleCreateRequest.isDone)
            {
                var assetBundle = m_CurrAssetBundleCreateRequest.assetBundle;
                if (assetBundle != null)
                {
                    GameEntry.LogInfo(LogCategory.Resource, $"资源包=> {m_CurrAssetBundleInfo.AssetBundleName} 加载完毕");
                    Reset();
                    OnLoadAssetBundleComplete?.Invoke(assetBundle);
                }
                else
                {
                    GameEntry.LogInfo(LogCategory.Resource, $"资源包=> {m_CurrAssetBundleInfo.AssetBundleName} 加载失败");
                    Reset();
                    OnLoadAssetBundleComplete?.Invoke(null);
                }
            }
            else
            {
                OnAssetBundleCreateUpdate?.Invoke(m_CurrAssetBundleCreateRequest.progress);
            }
        }

        #endregion

        
    }
}