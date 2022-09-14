using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZnFramework;

public class UITips2 : UIFormBase
{
    protected override void OnInit(object userData)
    {
        base.OnInit(userData);
        Debug.Log("UITips2 初始化");
    }

    protected override void OnOpen(object userData)
    {
        base.OnOpen(userData);
        Debug.Log("UITips2 打开");
    }

    protected override void OnClose()
    {
        base.OnClose();
        Debug.Log("UITips2 关闭");
    }

    protected override void OnBeforeDestroy()
    {
        base.OnBeforeDestroy();
    }
}
