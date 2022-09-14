using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZnFramework
{
    public class ProcedureLaunch : ProcedureBase
    {
        public override void OnEnter()
        {
            base.OnEnter();
            GameEntry.LogInfo(LogCategory.Procedure,"进入 ProcedureLaunch 流程");
            GameEntry.Procedure.ChangeState(ProcedureState.CheckVersion);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            GameEntry.LogInfo(LogCategory.Procedure,"循环 ProcedureLaunch 流程");
        }

        public override void OnLeave()
        {
            base.OnLeave();
            GameEntry.LogInfo(LogCategory.Procedure,"离开 ProcedureLaunch 流程");
        }
    }
}