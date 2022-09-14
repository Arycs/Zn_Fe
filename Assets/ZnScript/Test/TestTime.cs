using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZnFramework;

public class TestTime : MonoBehaviour
{
    private TimeAction timeAction1 = null;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            timeAction1 = GameEntry.Time.CreateTimeAction().Init("定时器A",loop: 10,
                
                onUpdateAction: (i) => { Debug.Log($"定时器A 剩余执行次数{i}"); },
                onCompleteAction: () => { Debug.Log("定时器A 执行完毕"); }
            );
            timeAction1.Run();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            timeAction1?.Pause();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            timeAction1?.Resume();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            GameEntry.Time.RemoveTimeActionByName("定时器A");
            timeAction1 = null;
        }
    }
}