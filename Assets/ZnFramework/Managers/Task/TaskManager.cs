using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZnFramework
{
    /// <summary>
    /// 任务管理器
    /// </summary>
    public class TaskManager: IDisposable
    {
        /// <summary>
        /// 任务组列表
        /// </summary>
        private LinkedList<TaskGroup> m_TaskGroupList;

        public TaskManager()
        {
            m_TaskGroupList = new LinkedList<TaskGroup>();
        }
        
        public void Init()
        {
            
        }

        public void OnUpdate()
        {
            LinkedListNode<TaskGroup> taskGroup = m_TaskGroupList.First;
            while (taskGroup != null)
            {
                taskGroup.Value.OnUpdate();
                taskGroup = taskGroup.Next;
            }
        }

        /// <summary>
        /// 创建一个任务组
        /// </summary>
        /// <returns></returns>
        public TaskGroup CreateTaskGroup()
        {
            TaskGroup taskGroup = GameEntry.Pool.DequeueClassObject<TaskGroup>();
            m_TaskGroupList.AddLast(taskGroup);
            return taskGroup;
        }

        /// <summary>
        /// 移除任务组
        /// </summary>
        /// <param name="taskGroup"></param>
        public void RemoveTaskGroup(TaskGroup taskGroup)
        {
            m_TaskGroupList.Remove(taskGroup);
        }

        /// <summary>
        /// 创建任务执行器
        /// </summary>
        /// <returns></returns>
        public TaskRoutine CreateTaskRoutine()
        {
            return GameEntry.Pool.DequeueClassObject<TaskRoutine>();
        }

        public void Dispose()
        {
            
        }
    }
}