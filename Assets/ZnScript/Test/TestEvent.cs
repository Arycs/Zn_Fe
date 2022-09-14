using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZnFramework;

public class TestEvent : MonoBehaviour
{

    private string m_Content;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    private void OnTestEvent1(object userdata)
    {
        Debug.Log($"OnTestEvent1 : 派发参数:{userdata}");
    }
    private void OnTestEvent2(object userdata)
    {
        Debug.Log($"OnTestEvent2 :派发参数:{userdata}");

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("添加 TestEventID , 监听事件 OnTestEvent1");
            GameEntry.Event.CommonEvent.AddEventListener(CommonEventID.TestEventID, OnTestEvent1);
        }
        
        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("添加 TestEventID , 监听事件 OnTestEvent2");        
            GameEntry.Event.CommonEvent.AddEventListener(CommonEventID.TestEventID, OnTestEvent2);

        }
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            m_Content = "Zn_Arycs";
            GameEntry.Event.CommonEvent.Dispatch(CommonEventID.TestEventID,(object)m_Content);
        }
        
        if (Input.GetKeyDown(KeyCode.D))
        {
            m_Content = "移除OnTestEvent1";
            GameEntry.Event.CommonEvent.RemoveEventListener(CommonEventID.TestEventID,OnTestEvent1);
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            m_Content = "移除OnTestEvent2";
            GameEntry.Event.CommonEvent.RemoveEventListener(CommonEventID.TestEventID,OnTestEvent2);
        }
        
        
        
    }
}
