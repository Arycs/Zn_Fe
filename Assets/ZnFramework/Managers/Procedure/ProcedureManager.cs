using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZnFramework
{
    /// <summary>
    /// 流程状态
    /// TODO 此处流程需后续根据具体游戏逻辑进行添加.目前给出的是预设,需要后续根据游戏逻辑更改
    /// TODO Max 的值需要一直为数量最大 + 1
    /// </summary>
    public enum ProcedureState
    {
        Launch = 0,
        CheckVersion = 1,
        PreLoad = 2,
        MainMenu = 3,
        Max = 4,
    }

    /// <summary>
    /// 流程管理器
    /// </summary>
    public class ProcedureManager : IDisposable
    {
        public Fsm<ProcedureManager> CurrFsm { get; private set; }

        public ProcedureState CurrProcedureState => (ProcedureState) CurrFsm.CurrStateType;

        public FsmState<ProcedureManager> CurrProcedure => CurrFsm.GetState(CurrFsm.CurrStateType);

        public void Init()
        {
            var states = new FsmState<ProcedureManager>[(int) ProcedureState.Max];
            states[0] = new ProcedureLaunch();
            states[1] = new ProcedureCheckVersion();
            states[2] = new ProcedurePreLoad();
            states[3] = new ProcedureMainMenu();

            CurrFsm = GameEntry.Fsm.CreateFsm(this, states);
            CurrFsm.ChangeState((sbyte) ProcedureState.Launch);
        }

        public void OnUpdate()
        {
            CurrFsm.OnUpdate();
        }

        public void Dispose()
        {
        }

        #region ChangeState 切换状态

        /// <summary>
        /// 切换状态
        /// </summary>
        /// <param name="state"></param>
        public void ChangeState(ProcedureState state)
        {
            CurrFsm.ChangeState((sbyte) state);
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
            CurrFsm.SetData(key, value);
        }

        /// <summary>
        /// 获取参数值
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="TData"></typeparam>
        /// <returns></returns>
        public TData GetData<TData>(string key) where TData : VariableBase
        {
            return CurrFsm.GetData<TData>(key);
        }

        #endregion
    }
}