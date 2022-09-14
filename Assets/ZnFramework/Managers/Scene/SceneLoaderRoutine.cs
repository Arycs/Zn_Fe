using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ZnFramework
{
    /// <summary>
    /// 场景加载和卸载器
    /// </summary>
    public class SceneLoaderRoutine
    {
        private AsyncOperation m_CurrAsync = null;

        /// <summary>
        /// 进度更新 
        /// </summary>
        private Action<int, float> OnProgressUpdate;

        /// <summary>
        /// 加载场景完毕回调
        /// </summary>
        private Action<SceneLoaderRoutine> OnLoadSceneComplete;

        /// <summary>
        /// 卸载场景完毕
        /// </summary>
        private Action<SceneLoaderRoutine> OnUnLoadSceneComplete;

        /// <summary>
        /// 场景明细编号
        /// </summary>
        private int m_SceneDetailId;

        /// <summary>
        /// 更新
        /// </summary>
        public void OnUpdate()
        {
            if (m_CurrAsync == null)
            {
                return;
            }

            if (!m_CurrAsync.isDone)
            {
                if (m_CurrAsync.progress >= 0.9f)
                {
                    OnProgressUpdate?.Invoke(m_SceneDetailId,m_CurrAsync.progress);
                    m_CurrAsync.allowSceneActivation = true;
                    m_CurrAsync = null;
                
                    OnLoadSceneComplete?.Invoke(this);
                }
                else
                {
                    OnProgressUpdate?.Invoke(m_SceneDetailId,m_CurrAsync.progress);
                }
            }
            else
            {
                m_CurrAsync = null;
                OnUnLoadSceneComplete?.Invoke(this);
            }

        }
        

        #region LoadScene 加载场景
        
        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="sceneDetailId"></param>
        /// <param name="sceneName"></param>
        /// <param name="onProgressUpdate"></param>
        /// <param name="onLoadSceneComplete"></param>
        public void LoadScene(int sceneDetailId, string sceneName, Action<int, float> onProgressUpdate,
            Action<SceneLoaderRoutine> onLoadSceneComplete)
        {
            Reset();

            m_SceneDetailId = sceneDetailId;
            OnProgressUpdate = onProgressUpdate;
            OnLoadSceneComplete = onLoadSceneComplete;
#if DISABLE_ASSETBUNDLE
            m_CurrAsync = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            m_CurrAsync.allowSceneActivation = false;
            if (m_CurrAsync == null)
            {
                OnLoadSceneComplete?.Invoke(this);

            }
#else
            GameEntry.Resource.ResourceLoaderManager.LoadAssetBundle(GameEntry.Resource.GetSceneAssetBundlePath(sceneName),onComplete:
                (AssetBundle bundle2) =>
                {
                    m_CurrAsync = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                    if (m_CurrAsync == null)
                    {
                        OnLoadSceneComplete?.Invoke(this);
                    }
                });
            
#endif
        }

        #endregion

        #region UnLoadScene 卸载场景

        /// <summary>
        /// 卸载场景
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="onUnLoadSceneComplete"></param>
        public void UnLoadScene(string sceneName, Action<SceneLoaderRoutine> onUnLoadSceneComplete)
        {
            Reset();

            OnUnLoadSceneComplete = onUnLoadSceneComplete;
            m_CurrAsync = SceneManager.UnloadSceneAsync(sceneName);
            if (m_CurrAsync == null)
            {
                OnUnLoadSceneComplete?.Invoke(this);
            }
        }

        #endregion
        
        /// <summary>
        /// 重置
        /// </summary>
        private void Reset()
        {
            m_CurrAsync = null;
            OnProgressUpdate = null;
            OnLoadSceneComplete = null;
            OnUnLoadSceneComplete = null;
        }


    }
}