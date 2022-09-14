using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZnFramework
{
    public class Fsm<T> : FsmBase where T : class
    {
        private FsmState<T> m_CurrState;
        private Dictionary<sbyte, FsmState<T>> m_StateDic;
        private Dictionary<string, VariableBase> m_ParamDic;

        public Fsm(int fsmId, T owner, FsmState<T>[] states) : base(fsmId)
        {
            m_StateDic = new Dictionary<sbyte, FsmState<T>>();
            var len = states.Length;
            for (var i = 0; i < len; i++)
            {
                var state = states[i];
                state.curFsm = this;
                m_StateDic[(sbyte) i] = state;
            }

            CurrStateType = -1;
        }

        public void OnUpdate()
        {
            m_CurrState?.OnUpdate();
        }

        #region GetState 获取状态机状态

        /// <summary>
        /// 获取状态机状态
        /// </summary>
        /// <param name="stateType"></param>
        /// <returns></returns>
        public FsmState<T> GetState(sbyte stateType)
        {
            m_StateDic.TryGetValue(stateType, out var state);
            return state;
        }

        #endregion

        #region ChangeState 改变状态

        /// <summary>
        /// 改变状态
        /// </summary>
        /// <param name="newState"></param>
        public void ChangeState(sbyte newState)
        {
            if (CurrStateType == newState)
            {
                return;
            }

            m_CurrState?.OnLeave();
            CurrStateType = newState;
            m_CurrState = m_StateDic[CurrStateType];
            m_CurrState?.OnEnter();
        }

        #endregion

        #region ShutDown 关闭状态机

        /// <summary>
        /// 关闭状态机
        /// </summary>
        public override void ShutDown()
        {
            m_CurrState?.OnLeave();
            foreach (var state in m_StateDic)
            {
                state.Value.OnDestroy();
            }

            m_StateDic.Clear();
            m_ParamDic.Clear();
        }

        #endregion

        #region SetData 设置状态机参数

        /// <summary>
        /// 设置状态机参数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <typeparam name="TData">泛型的类型</typeparam>
        public void SetData<TData>(string key, TData value) where TData : VariableBase
        {
            if (m_ParamDic.TryGetValue(key, out var itemBase))
            {
                if (!(itemBase is Variable<TData> item)) return;
                item.Value = value;
                m_ParamDic[key] = item;
                GameEntry.LogInfo(LogCategory.Fsm,$"修改已有值{key}-{value}");
            }
            else
            {
                GameEntry.LogInfo(LogCategory.Fsm,$"参数不存在新实例化{key}-{value}");
                var item = new Variable<TData> {Value = value};
                m_ParamDic.Add(key, item);
            }
        }

        /// <summary>
        /// 获取参数值
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="TData"></typeparam>
        /// <returns></returns>
        public TData GetData<TData>(string key) where TData : VariableBase
        {
            if (!m_ParamDic.TryGetValue(key, out var itemBase)) return default;
            if (itemBase is Variable<TData> item) return item.Value;
            return default;
        }

        #endregion
    }
}