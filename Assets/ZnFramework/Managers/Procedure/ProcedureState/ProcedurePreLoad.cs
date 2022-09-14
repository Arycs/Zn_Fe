using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZnFramework;

public class ProcedurePreLoad : ProcedureBase
{
    /// <summary>
    /// 目标进度
    /// </summary>
    private float m_TargetProgress = 0;

    /// <summary>
    /// 当前进度
    /// </summary>
    private float m_CurrProgress = 0;

    /// <summary>
    /// 预加载参数
    /// </summary>
    private BaseParams m_PreloadParams;

    public override void OnEnter()
    {
        base.OnEnter();
        GameEntry.LogInfo(LogCategory.Procedure,"On Enter ProcedurePreLoad");
        
        GameEntry.Event.CommonEvent.AddEventListener(SysEventId.LoadDataTableComplete,OnLoadDataTableComplete);
        GameEntry.Event.CommonEvent.AddEventListener(SysEventId.LoadOneDataTableComplete,OnLoadOneDataTableComplete);
        
        GameEntry.LogInfo(LogCategory.Procedure, "预加载流程开始");
        m_PreloadParams = GameEntry.Pool.DequeueClassObject<BaseParams>();
        m_PreloadParams.Reset();
        m_TargetProgress = 99;
        
#if !DISABLE_ASSETBUNDLE
        GameEntry.Resource.InitAssetInfo();
#endif
        GameEntry.DataTable.LoadDataTableAsync();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (m_CurrProgress < m_TargetProgress || m_TargetProgress < 100)
        {
            m_CurrProgress = m_CurrProgress + Time.deltaTime * 200; //根据实际速度调节速度
            m_PreloadParams.FloatParam1 = m_CurrProgress;
            //TODO 如果此处有对应UI界面则通过事件发送过去进度即可 m_PreloadParams 参数
        }else if (m_CurrProgress >= 100)
        {
            m_CurrProgress = 100;
            GameEntry.LogInfo(LogCategory.Procedure,"预加载完毕");
            GameEntry.Pool.EnqueueClassObject(m_PreloadParams);
            //TODO 切换下一个流程
            GameEntry.Procedure.ChangeState(ProcedureState.MainMenu);
        }
    }

    public override void OnLeave()
    {
        base.OnLeave();
        GameEntry.LogInfo(LogCategory.Procedure,"On Leave ProcedurePreLoad");
        
        GameEntry.Event.CommonEvent.RemoveEventListener(SysEventId.LoadDataTableComplete,OnLoadDataTableComplete);
        GameEntry.Event.CommonEvent.RemoveEventListener(SysEventId.LoadOneDataTableComplete,OnLoadOneDataTableComplete);

    }

    /// <summary>
    /// 加载单一表格完毕回调 
    /// </summary>
    /// <param name="userdata"></param>
    private void OnLoadOneDataTableComplete(object userdata)
    {
        GameEntry.DataTable.CurrLoadTableCount++;
        if (GameEntry.DataTable.CurrLoadTableCount == GameEntry.DataTable.TotalTableCount)
        {
            GameEntry.Event.CommonEvent.Dispatch(SysEventId.LoadDataTableComplete);
        }
    }

    /// <summary>
    /// 加载所有表格完成回调
    /// </summary>
    /// <param name="userdata"></param>
    private void OnLoadDataTableComplete(object userdata)
    {
        GameEntry.LogInfo(LogCategory.Procedure,"加载所有表格完毕");
        //TODO 如果后续有需要加载的资源对应添加在这里 . 回调的时候设置Progress = 100 即可
        
        m_TargetProgress = 100;
    }
}
