using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZnFramework;

public class UITips : UIFormBase
{
    protected override void OnInit(object userData)
    {
        base.OnInit(userData);
        Debug.Log("UITips 初始化");
    }

    protected override void OnOpen(object userData)
    {
        base.OnOpen(userData);
        Debug.Log("UITips 打开");
    }

    protected override void OnClose()
    {
        base.OnClose();
        Debug.Log("UITips 关闭");
    }

    protected override void OnBeforeDestroy()
    {
        base.OnBeforeDestroy();
    }
}
