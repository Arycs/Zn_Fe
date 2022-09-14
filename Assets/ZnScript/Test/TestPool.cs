using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZnFramework;

public class TestPool : MonoBehaviour
{
    public Transform cube;

    // private int i = 0;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
     void Update()

    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            // GameEntry.Pool.GameObjectSpawn(1, cube);
            // transform.gameObject.SetActive(true);
            // transform.position = new Vector3(i, i, i);
            //
            // GameEntry.Time.CreateTimeAction().Init(loop: 5,
            //     onStartAction: () => { Debug.Log("AAA"); }, onUpdateAction: (i) => { Debug.Log($"{i}"); },
            //     onCompleteAction: (() => { GameEntry.Pool.GameObjectDespawn(1, transform); })).Run();
            // i++;
        }
    }
}