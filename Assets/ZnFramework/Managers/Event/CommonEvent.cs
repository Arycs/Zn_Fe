using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZnFramework
{
    /// <summary>
    /// 通用事件
    /// </summary>
    public class CommonEvent
    {
        public delegate void OnActionHandler(object userdata);

        private Dictionary<int, LinkedList<OnActionHandler>> dic =
            new Dictionary<int, LinkedList<OnActionHandler>>();

        #region AddEventListener 添加事件监听

        /// <summary>
        /// 添加事件监听
        /// </summary>
        /// <param name="key"></param>
        /// <param name="handler"></param>
        public void AddEventListener(int key, OnActionHandler handler)
        {
            dic.TryGetValue(key, out var lstHandler);
            if (lstHandler == null)
            {
                lstHandler = new LinkedList<OnActionHandler>();
                dic[key] = lstHandler;
            }

            lstHandler.AddLast(handler);
        }

        #endregion

        #region RemoveEventListener 移除事件监听

        /// <summary>
        /// 移除事件监听
        /// </summary>
        /// <param name="key"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener(int key, OnActionHandler handler)
        {
            dic.TryGetValue(key, out var lstHandler);
            if (lstHandler != null)
            {
                lstHandler.Remove(handler);
                if (lstHandler.Count == 0)
                {
                    dic.Remove(key);
                }
            }
        }

        public void RemoveEventListener(int key)
        {
            dic.TryGetValue(key, out var lstHandler);
            if (lstHandler != null)
            {
                lstHandler.Clear();
                dic.Remove(key);
            }
        }

        #endregion

        #region Dispatch 派发事件监听
        
        /// <summary>
        /// 派发事件
        /// </summary>
        /// <param name="key">事件Key</param>
        /// <param name="userdata">事件参数</param>
        public void Dispatch(int key, object userdata = null)
        {
            dic.TryGetValue(key, out var lstHandler);
            if (lstHandler != null && lstHandler.Count > 0)
            {
                for (var curr = lstHandler.First; curr != null; curr = curr.Next)
                {
                    curr.Value?.Invoke(userdata);
                }
            }
        }

        #endregion
    }
}