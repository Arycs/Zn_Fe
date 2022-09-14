using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZnFramework
{
    public class EventManager : IDisposable
    {
        /// <summary>
        /// 通用事件
        /// </summary>
        public CommonEvent CommonEvent { get; private set; }

        public EventManager()
        {
            CommonEvent = new CommonEvent();
        }
        
        public void Dispose()
        {
        }
    }
}