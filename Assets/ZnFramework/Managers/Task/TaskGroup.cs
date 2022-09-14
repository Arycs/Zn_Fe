using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZnFramework
{
    /// <summary>
    /// 任务组
    /// </summary>
    public class TaskGroup : IDisposable
    {
        /// <summary>
        /// 任务列表
        /// </summary>
        private LinkedList<TaskRoutine> m_TaskRoutineList;

        /// <summary>
        /// 任务组完成
        /// </summary>
        public Action OnComplete;

        /// <summary>
        /// 是否并发执行
        /// </summary>
        private bool m_IsConcurrency = false;

        public TaskGroup()
        {
            m_TaskRoutineList = new LinkedList<TaskRoutine>();
        }

        public void Dispose()
        {
            m_TaskRoutineList.Clear();
            OnComplete = null;
        }

        public void AddTask(TaskRoutine routine)
        {
            m_TaskRoutineList.AddLast(routine);
        }

        /// <summary>
        /// 清空所有任务
        /// </summary>
        public void CreateAllTask()
        {
            LinkedListNode<TaskRoutine> routine = m_TaskRoutineList.First;
            while (routine != null)
            {
                var next = routine.Next;
                routine.Value.StopTask?.Invoke();
                GameEntry.Pool.EnqueueClassObject(routine);
                m_TaskRoutineList.Remove(routine);
                routine = next;
            }
        }

        public void OnUpdate()
        {
            LinkedListNode<TaskRoutine> routine = m_TaskRoutineList.First;
            while (routine != null)
            {
                routine.Value.OnUpdate();
                routine = routine.Next;
            }
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        public void Run(bool isConcurrency = false)
        {
            m_IsConcurrency = isConcurrency;

            if (m_IsConcurrency)
            {
                ConcurrencyTask();
            }
            else
            {
                //按照顺序执行任务
                CheckTask();
            }
        }

        /// <summary>
        /// 检查任务
        /// </summary>
        private void CheckTask()
        {
            LinkedListNode<TaskRoutine> curr = m_TaskRoutineList.First;
            if (curr != null)
            {
                curr.Value.OnComplete = () =>
                {
                    m_TaskRoutineList.Remove(curr);
                    CheckTask();
                };
                curr.Value.Enter();
            }
            else
            {
                OnComplete?.Invoke();
                Dispose();
                GameEntry.Task.RemoveTaskGroup(this);
                GameEntry.Pool.EnqueueClassObject(this);
            }
        }

        private int m_TotalCount = 0;
        private int m_CurrCount = 0;

        /// <summary>
        /// 并发执行任务
        /// </summary>
        private void ConcurrencyTask()
        {
            m_TotalCount = m_TaskRoutineList.Count;
            m_CurrCount = 0;

            LinkedListNode<TaskRoutine> routine = m_TaskRoutineList.First;
            while (routine != null)
            {
                LinkedListNode<TaskRoutine> next = routine.Next;
                routine.Value.Enter();
                var routine1 = routine;
                routine.Value.OnComplete = () => { CheckConcurrencyTaskComplete(); };
                routine = next;
            }
        }

        private void CheckConcurrencyTaskComplete()
        {
            m_CurrCount++;
            //Debug.LogError("m_CurrCount=" + m_CurrCount);
            //Debug.LogError("m_TotalCount=" + m_TotalCount);
            if (m_CurrCount == m_TotalCount)
            {
                OnComplete?.Invoke();
                Dispose();
                GameEntry.Task.RemoveTaskGroup(this);
                GameEntry.Pool.EnqueueClassObject(this);
            }
        }
    }
}