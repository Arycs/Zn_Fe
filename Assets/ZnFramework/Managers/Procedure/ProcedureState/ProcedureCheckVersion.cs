using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZnFramework
{
    /// <summary>
    /// 检查版本流程
    /// </summary>
    public class ProcedureCheckVersion : ProcedureBase
    {
        public override void OnEnter()
        {
            base.OnEnter();
            GameEntry.LogInfo(LogCategory.Procedure,"进入 ProcedureCheckVersion 流程");
            
#if DISABLE_ASSETBUNDLE
            GameEntry.Procedure.ChangeState(ProcedureState.Preload);
#else
            GameEntry.Resource.InitStreamingAssetsBundleInfo();
#endif
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        public override void OnLeave()
        {
            base.OnLeave();
            Debug.Log("OnLeave ProcedureCheckVersion");
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}