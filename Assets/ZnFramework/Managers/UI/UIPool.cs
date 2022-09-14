using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZnFramework
{
    public class UIPool
    {
        /// <summary>
        /// 对象池中的列表
        /// </summary>
        private LinkedList<UIFormBase> m_UIFormList;

        public UIPool()
        {
            m_UIFormList = new LinkedList<UIFormBase>();
        }

        /// <summary>
        /// 从UI对象池中获取UI
        /// </summary>
        /// <param name="uiFormId"></param>
        /// <returns></returns>
        internal UIFormBase Dequeue(int uiFormId)
        {
            for (var curr = m_UIFormList.First; curr != null; curr = curr.Next)
            {
                if (curr.Value.UIFormId != uiFormId) continue;
                m_UIFormList.Remove(curr.Value);
                return curr.Value;
            }

            return null;
        }

        /// <summary>
        /// 回到对象池
        /// </summary>
        /// <param name="formBase"></param>
        internal void Enqueue(UIFormBase formBase)
        {
            m_UIFormList.AddLast(formBase);
        }


        /// <summary>
        /// 检查是否可以释放
        /// </summary>
        internal void CheckClear()
        {
            for (var curr = m_UIFormList.First; curr != null;)
            {
                if (!curr.Value.IsLock && Time.time > (curr.Value.CloseTime + GameEntry.UI.UIExpire))
                {
                    Object.Destroy(curr.Value.gameObject);
                    GameEntry.Pool.ReleaseInstanceResource(curr.Value.gameObject.GetInstanceID());
                    
                    var next = curr.Next;
                    m_UIFormList.Remove(curr.Value);
                    curr = next;
                }
                else
                {
                    curr = curr.Next;
                }
            }
        }


        /// <summary>
        /// 打开UI的时候判断对象池中是否有释放
        /// </summary>
        internal void CheckByOpenUI()
        {
            if (m_UIFormList.Count <= GameEntry.UI.UIPoolMaxCount)
            {
                return;
            }

            for (var curr = m_UIFormList.First; curr != null;)
            {
                if (m_UIFormList.Count <= GameEntry.UI.UIPoolMaxCount + 1)
                {
                    //池中数量 在指定数量以内 则不在继续销毁
                    break;
                }

                if (!curr.Value.IsLock)
                {
                    Object.Destroy(curr.Value.gameObject);
                    GameEntry.Pool.ReleaseInstanceResource(curr.Value.gameObject.GetInstanceID());

                    var next = curr.Next;
                    m_UIFormList.Remove(curr.Value);
                    curr = next;
                }
                else
                {
                    curr = curr.Next;
                }
            }
        }
    }
}