using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZnFramework
{
    public class TimeManager : IDisposable
    {
        /// <summary>
        /// 定时器链表
        /// </summary>
        private LinkedList<TimeAction> m_TimeActionList;

        public TimeManager()
        {
            m_TimeActionList = new LinkedList<TimeAction>();
        }

        public void OnUpdate()
        {
            for (var curr = m_TimeActionList.First; curr != null; curr = curr.Next)
            {
                if (curr.Value.OnStartAction !=null && (curr.Value.OnStartAction.Target == null || curr.Value.OnStartAction.Target.ToString() == "null"))
                {
                    m_TimeActionList.Remove(curr);
                    continue;
                }

                if (curr.Value.OnUpdateAction != null && (curr.Value.OnUpdateAction.Target == null || curr.Value.OnUpdateAction.Target.ToString() == "null"))
                {
                    m_TimeActionList.Remove(curr);
                    continue;
                }

                if (curr.Value.OnCompleteAction != null && (curr.Value.OnCompleteAction.Target == null || curr.Value.OnCompleteAction.Target.ToString() == "null"))
                {
                    m_TimeActionList.Remove(curr);
                    continue;
                }
                curr.Value.OnUpdate();
            }
        }

        public void Dispose()
        {
            m_TimeActionList.Clear();
        }

        /// <summary>
        /// 创建一个定时器
        /// </summary>
        /// <returns></returns>
        public TimeAction CreateTimeAction()
        {
            return new TimeAction();
        }

        /// <summary>
        /// 注册定时器
        /// </summary>
        /// <param name="action"></param>
        internal void RegisterTimeAction(TimeAction action)
        {
            m_TimeActionList.AddLast(action);
        }

        /// <summary>
        /// 移除定时器
        /// </summary>
        /// <param name="action"></param>
        internal void RemoveTimeAction(TimeAction action)
        {
            m_TimeActionList.Remove(action);
        }

        /// <summary>
        /// 根据定时器名字删除定时器
        /// </summary>
        /// <param name="timeName"></param>
        public void RemoveTimeActionByName(string timeName)
        {
            LinkedListNode<TimeAction> curr = m_TimeActionList.First;
            while (curr != null)
            {
                if (curr.Value.TimeActionName.Equals(timeName, StringComparison.CurrentCultureIgnoreCase))
                {
                    m_TimeActionList.Remove(curr);
                    break;
                }

                curr = curr.Next;
            }
        }

        #region Yield 等一帧

        public void Yield(Action onComplete)
        {
            GameEntry.Instance.StartCoroutine(YieldCoroutine(onComplete));
        }

        private IEnumerator YieldCoroutine(Action onComplete)
        {
            yield return null;
            if (onComplete != null)
            {
                onComplete();
            }
        }

        #endregion
    }
}