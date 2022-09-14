using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZnFramework
{
    /// <summary>
    /// 定时器
    /// </summary>
    public class TimeAction
    {
        public string TimeActionName { get; private set; }
        public bool IsRunning { get; private set; }
        private bool m_IsPause = false;
        private float m_CurrRunTime;
        private int m_CurrLoop;
        private float m_DelayTime;
        private float m_Interval; //间隔时间 (秒)
        private int m_Loop; //循环次数 (参数中设置, -1表示无限循环, 0也会循环一次, 1以上设置多少循环多少)
        private float m_LastPauseTime;
        private float m_PauseTime;

        #region 回调事件

        public Action OnStartAction { get; private set; }
        public Action<int> OnUpdateAction { get; private set; } //运行中,参数表示剩余次数
        public Action OnCompleteAction { get; private set; }

        #endregion

        #region Init 初始化定时器

        /// <summary>
        /// 初始化定时器
        /// </summary>
        /// <param name="timeActionName">定时器名称</param>
        /// <param name="delayTime">延迟时间</param>
        /// <param name="interval">间隔时间</param>
        /// <param name="loop">循环次数</param>
        /// <param name="onStartAction">开始回调</param>
        /// <param name="onUpdateAction">运行回调</param>
        /// <param name="onCompleteAction">结束回调</param>
        public TimeAction Init(string timeActionName = null, float delayTime = 0, float interval = 1, int loop = 1,
            Action onStartAction = null, Action<int> onUpdateAction = null, Action onCompleteAction = null)
        {
            TimeActionName = timeActionName;
            m_DelayTime = delayTime;
            m_Interval = interval;
            m_Loop = loop;

            OnStartAction = onStartAction;
            OnUpdateAction = onUpdateAction;
            OnCompleteAction = onCompleteAction;
            return this;
        }

        #endregion

        #region Run 运行定时器

        public void Run()
        {
            // 首先先将自身 注册到TimeManager 链表中
            GameEntry.Time.RegisterTimeAction(this);
            m_CurrRunTime = Time.time;
            m_IsPause = false;
        }

        #endregion

        #region Pause 暂停定时器,调用时应做判空处理

        public void Pause()
        {
            m_LastPauseTime = Time.time;
            m_IsPause = true;
            GameEntry.LogInfo(LogCategory.Time,"暂停运行");
        }

        #endregion

        #region Resume 恢复定时器,调用时应做判空处理

        public void Resume()
        {
            m_IsPause = false;

            m_PauseTime = Time.time - m_LastPauseTime;
            GameEntry.LogInfo(LogCategory.Time,$"{TimeActionName}定时器恢复运行,暂停了{m_PauseTime}秒");
        }

        #endregion

        #region Stop 停止定时器,调用时应做判空处理

        private void Stop()
        {
            OnCompleteAction?.Invoke();
            IsRunning = false;
            GameEntry.Time.RemoveTimeAction(this);
        }

        #endregion

        #region OnUpdate 每帧执行

        public void OnUpdate()
        {
            // 1. 是否暂停
            if (m_IsPause)
            {
                return;
            }

            // 2. 判断当前时间 是否是否满足  运行时间 + 延迟时间 + 暂停时间
            if (Time.time > m_CurrRunTime + m_DelayTime + m_PauseTime)
            {
                if (!IsRunning)
                {
                    //当程序执行到这里的时候,表示已经第一次过了延迟时间
                    m_CurrRunTime = Time.time;
                    m_PauseTime = 0;
                    
                    OnStartAction?.Invoke();
                }

                IsRunning = true;
            }
            
            // 3. 这里是 有延迟时间 未到达延迟时间 不可执行
            if (!IsRunning)
            {
                return;
            }
        
            // 4. 满足条件, 开始循环, m_Interval 是默认间隔时间
            if (Time.time > m_CurrRunTime + m_PauseTime)
            {
                m_CurrRunTime = Time.time + m_Interval;
                m_PauseTime = 0;
                //以下代码 间隔m_Interval 时间执行一次
                OnUpdateAction?.Invoke(m_Loop - m_CurrLoop);

                if (m_Loop > -1)
                {
                    m_CurrLoop++;
                    if (m_CurrLoop >= m_Loop)
                    {
                        Stop();
                    }
                }
            }
        }

        #endregion
    }
}