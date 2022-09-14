using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZnFramework
{
    public class FsmManager : IDisposable
    {
        private Dictionary<int, FsmBase> m_FsmDic;
        private int m_TempFsmId = 0;

        public FsmManager()
        {
            m_FsmDic = new Dictionary<int, FsmBase>();
        }

        public void Dispose()
        {
            var enumerator = m_FsmDic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.Value.ShutDown();
            }

            m_FsmDic.Clear();
        }

        #region CreateFsm 创建状态机

        /// <summary>
        /// 创建状态机
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="states"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Fsm<T> CreateFsm<T>(T owner, FsmState<T>[] states) where T : class
        {
            return Create(m_TempFsmId++, owner, states);
        }

        /// <summary>
        /// 创建状态机
        /// </summary>
        /// <param name="fsmId">状态机编号</param>
        /// <param name="owner">拥有者</param>
        /// <param name="states">状态</param>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <returns></returns>
        private Fsm<T> Create<T>(int fsmId, T owner, FsmState<T>[] states) where T : class
        {
            var fsm = new Fsm<T>(fsmId, owner, states);
            m_FsmDic[fsmId] = fsm;
            GameEntry.LogInfo(LogCategory.Fsm, $"创建状态机完成,拥有者为 : {owner}");
            return fsm;
        }

        #endregion

        #region DestroyFsm 销毁状态机

        /// <summary>
        /// 销毁状态机
        /// </summary>
        /// <param name="fsmId"></param>
        public void DestroyFsm(int fsmId)
        {
            if (!m_FsmDic.TryGetValue(fsmId, out var fsm)) return;
            fsm.ShutDown();
            m_FsmDic.Remove(fsmId);
        }

        #endregion
    }
}