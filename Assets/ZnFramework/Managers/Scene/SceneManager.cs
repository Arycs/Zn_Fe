using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZnFramework
{
    /// <summary>
    /// 场景管理器
    /// </summary>
    public class ZnSceneManager
    {
        /// <summary>
        /// 场景加载器链表
        /// </summary>
        private LinkedList<SceneLoaderRoutine> m_SceneLoaderList;

        /// <summary>
        /// 当前加载的场景编号
        /// </summary>
        private int m_CurrLoadSceneId;

        /// <summary>
        /// 场景数据实体
        /// </summary>
        private DTSys_SceneEntity m_CurrSceneEntity;

        /// <summary>
        /// 场景明细列表
        /// </summary>
        private List<DTSys_SceneDetailEntity> m_CurrSceneDetailList;

        /// <summary>
        /// 需要加载或卸载的明细数量
        /// </summary>
        private int m_NeedLoadOrUnLoadSceneDetailCount = 0;

        /// <summary>
        /// 当前已经加载或卸载的明细数量
        /// </summary>
        private int m_CurrLoadOrUnLoadSceneDetailCount = 0;

        /// <summary>
        /// 场景是否加载中
        /// </summary>
        private bool m_CurrSceneIsLoading;

        /// <summary>
        /// 当前进度及
        /// </summary>
        private float m_CurrProgress = 0;

        /// <summary>
        /// 目标的进度字典
        /// </summary>
        private Dictionary<int, float> m_TargetProgressDic;

        /// <summary>
        /// 加载场景的参数
        /// </summary>
        private BaseParams m_CurrLoadingParam;
        
        /// <summary>
        /// 加载完成委托
        /// </summary>
        private Action m_OnComplete = null;

        public ZnSceneManager()
        {
            m_SceneLoaderList = new LinkedList<SceneLoaderRoutine>();
            m_TargetProgressDic = new Dictionary<int, float>();
        }

        public void Init()
        {
            
        }
        
        public void OnUpdate()
        {
            if (m_CurrSceneIsLoading)
            {
                var curr = m_SceneLoaderList.First;
                while (curr != null)
                {
                    curr.Value.OnUpdate();
                    curr = curr.Next;
                }

                var currTarget = GetCurrTotalProgress();
                var finalTarget = 0.9f * m_NeedLoadOrUnLoadSceneDetailCount;
                if (currTarget >= finalTarget)
                {
                    currTarget = m_NeedLoadOrUnLoadSceneDetailCount;
                }

                if (m_CurrProgress < m_NeedLoadOrUnLoadSceneDetailCount && m_CurrProgress <= currTarget)
                {
                    m_CurrProgress = m_CurrProgress + Time.deltaTime * m_NeedLoadOrUnLoadSceneDetailCount * 1;
                    m_CurrLoadingParam.IntParam1 = (int) LoadingType.ChangeScene;
                    m_CurrLoadingParam.FloatParam1 = (m_CurrProgress / m_NeedLoadOrUnLoadSceneDetailCount);
                    GameEntry.Event.CommonEvent.Dispatch(SysEventId.LoadingProgressChange, m_CurrLoadingParam);
                    //GameEntry.LogInfo(LogCategory.Scene,$"当前场景加载进度 ===> {m_CurrProgress / m_NeedLoadOrUnLoadSceneDetailCount}");
                }else if (m_CurrProgress >= m_NeedLoadOrUnLoadSceneDetailCount)
                {
                    GameEntry.LogInfo(LogCategory.Scene,"场景加载完毕");
                    //TODO 播放BGM
                    //GameEntry.Audio.PlayBGM(m_CurrSceneEntity.BGMId);
                    m_NeedLoadOrUnLoadSceneDetailCount = 0;
                    m_CurrLoadOrUnLoadSceneDetailCount = 0;
                    m_CurrSceneIsLoading = false;
                    GameEntry.UI.CloseUIForm(UIFormId.UI_Loading);
                    m_CurrLoadingParam.Reset();
                    GameEntry.Pool.EnqueueClassObject(m_CurrLoadingParam);
                    m_OnComplete?.Invoke();
                    
                }
            }
        }

        #region LoadScene 加载场景
        
        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="sceneId">场景ID</param>
        /// <param name="showLoadingForm">是否显示加载界面</param>
        /// <param name="onComplete">回调</param>
        public void LoadScene(int sceneId, bool showLoadingForm = false, Action onComplete = null)
        {
            if (m_CurrSceneIsLoading)
            {
                GameEntry.LogInfo(LogCategory.Scene, $"场景ID : {m_CurrLoadSceneId}正在加载中");
                return;
            }

            if (m_CurrLoadSceneId == sceneId)
            {
                GameEntry.LogInfo(LogCategory.Scene, $"正在重复加载场景{sceneId}");
                return;
            }

            //TODO 这里需要处理停掉BGM
            // GameEntry.Audio.StopBGM();

            m_CurrLoadingParam = GameEntry.Pool.DequeueClassObject<BaseParams>();
            m_OnComplete = onComplete;

            if (showLoadingForm)
            {
                GameEntry.UI.OpenUIForm(UIFormId.UI_Loading,onOpen: (form) =>
                {
                    DoLoadScene(sceneId);
                });
            }
            else
            {
                DoLoadScene(sceneId);
            }
        }

        #endregion

        #region DoLoadScene 执行加载场景

        /// <summary>
        /// 执行加载场景
        /// </summary>
        /// <param name="sceneId"></param>
        private void DoLoadScene(int sceneId)
        {
            m_CurrProgress = 0;
            m_TargetProgressDic .Clear();
            
            m_CurrSceneIsLoading = true;
            m_CurrLoadSceneId = sceneId;
            //先卸载当前场景
            UnLoadCurrScene();
        }

        #endregion

        #region UnLoadCurrScene 卸载当前场景

        /// <summary>
        /// 卸载当前场景
        /// </summary>
        private void UnLoadCurrScene()
        {
            if (m_CurrSceneEntity != null)
            {
                m_NeedLoadOrUnLoadSceneDetailCount = m_CurrSceneDetailList.Count;
                for (int i = 0; i < m_NeedLoadOrUnLoadSceneDetailCount; i++)
                {
                    var routine = GameEntry.Pool.DequeueClassObject<SceneLoaderRoutine>();
                    m_SceneLoaderList.AddLast(routine);
                    routine.UnLoadScene(m_CurrSceneDetailList[i].ScenePath,OnUnLoadSceneComplete);
                }
            }
            else
            {
                LoadNewScene();
            }
        }

        #endregion

        #region LoadNewScene 加载新场景

        /// <summary>
        /// 加载新场景
        /// </summary>
        private void LoadNewScene()
        {
            m_CurrSceneEntity = GameEntry.DataTable.Sys_SceneDBModel.Get(m_CurrLoadSceneId);
            //TODO 这里加载场景的等级 根据机型来获取
            m_CurrSceneDetailList =
                GameEntry.DataTable.Sys_SceneDetailDBModel.GetListBySceneId(m_CurrSceneEntity.Id, 1);
            m_NeedLoadOrUnLoadSceneDetailCount = m_CurrSceneDetailList.Count;
            for (int i = 0; i < m_NeedLoadOrUnLoadSceneDetailCount; i++)
            {
                var routine = GameEntry.Pool.DequeueClassObject<SceneLoaderRoutine>();
                m_SceneLoaderList.AddLast(routine);
                var entity = m_CurrSceneDetailList[i];
                routine.LoadScene(entity.Id,entity.ScenePath, OnLoadSceneProgressUpdate, OnLoadSceneComplete);
            }
        }

        #endregion

        #region OnLoadSceneComplete 加载场景完毕

        /// <summary>
        /// 加载场景完毕
        /// </summary>
        /// <param name="routine"></param>
        private void OnLoadSceneComplete(SceneLoaderRoutine routine)
        {
            m_SceneLoaderList.Remove(routine);
            GameEntry.Pool.EnqueueClassObject(routine);
        }

        #endregion

        #region OnLoadSceneProgressUpdate 加载场景进度更新

        /// <summary>
        /// 加载场景进度更新
        /// </summary>
        /// <param name="sceneDetailId"></param>
        /// <param name="progress"></param>
        private void OnLoadSceneProgressUpdate(int sceneDetailId, float progress)
        {
            m_TargetProgressDic[sceneDetailId] = progress;
            GameEntry.LogInfo(LogCategory.Scene, $"SceneDetailId = {sceneDetailId}");
            GameEntry.LogInfo(LogCategory.Scene, $"progress = {progress}");
        }

        #endregion

        #region OnUnLoadSceneComplete 卸载场景完毕

        /// <summary>
        /// 卸载场景完毕
        /// </summary>
        /// <param name="routine"></param>
        private void OnUnLoadSceneComplete(SceneLoaderRoutine routine)
        {
            m_SceneLoaderList.Remove(routine);
            GameEntry.Pool.EnqueueClassObject(routine);

            m_CurrLoadOrUnLoadSceneDetailCount++;
            if (m_CurrLoadOrUnLoadSceneDetailCount == m_NeedLoadOrUnLoadSceneDetailCount)
            {
                Resources.UnloadUnusedAssets();
                m_NeedLoadOrUnLoadSceneDetailCount = 0;
                m_CurrLoadOrUnLoadSceneDetailCount = 0;
                LoadNewScene();
            }
        }

        #endregion

        #region GetCurrTotalProgrtess 获取当前加载的总进度值

        /// <summary>
        /// 获取当前加载的总进度值
        /// </summary>
        /// <returns></returns>
        private float GetCurrTotalProgress()
        {
            float progress = 0;
            var lst = m_TargetProgressDic.GetEnumerator();
            while (lst.MoveNext())
            {
                progress += lst.Current.Value;
            }

            return progress;
        }

        #endregion
    }
}