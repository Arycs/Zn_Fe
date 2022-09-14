using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZnFramework;

public class ProcedureMainMenu : ProcedureBase
{
    public override void OnEnter()
    {
        base.OnEnter();
        GameEntry.Scene.LoadScene(SysScene.MainMenu,false,(() =>
        {
            Debug.Log("加载完第一个场景了");
        }));
    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (Input.GetKeyDown(KeyCode.A))
        {
            GameEntry.UI.OpenUIForm(SysUIFormId.UI_Tips);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            GameEntry.UI.OpenUIForm(SysUIFormId.UI_Tips2);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            GameEntry.UI.CloseUIForm(SysUIFormId.UI_Tips);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            GameEntry.UI.CloseUIForm(SysUIFormId.UI_Tips2);
        }
    }

    public override void OnLeave()
    {
        base.OnLeave();
    }

}
