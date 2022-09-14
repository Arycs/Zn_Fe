using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZnFramework
{
    public enum LogCategory
    {
        Normal,
        Event,
        Time,
        Pool,
        Fsm,
        Procedure,
        Resource,
        Scene,
        UI,
        GameLogic,
    }

    /// <summary>
    /// 资源分类
    /// </summary>
    public enum AssetCategory
    {
        None = 0,
        Audio,
        CusShaders,
        DataTable,
        EffectSources,
        Scenes,
        UIFont,
        UIPrefab,
        UIRes,
        XLuaLogic,
    }
    
    /// <summary>
    /// UI窗口的显示类型
    /// </summary>
    public enum UIFormShowMode
    {
        /// <summary>
        /// 普通
        /// </summary>
        Normal = 0,

        /// <summary>
        /// 反向
        /// </summary>
        ReverseChange = 1,
    }
        
    /// <summary>
    /// Loading类型
    /// </summary>
    public enum LoadingType
    {
        /// <summary>
        /// 检查更新
        /// </summary>
        CheckVersion = 0,

        /// <summary>
        /// 切换场景
        /// </summary>
        ChangeScene = 1
    }
    
}