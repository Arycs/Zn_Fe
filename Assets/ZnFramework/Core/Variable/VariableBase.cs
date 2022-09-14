using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZnFramework
{
    /// <summary>
    /// 变量基类
    /// </summary>
    public abstract class VariableBase
    {
        /// <summary>
        /// 获取变量类型
        /// </summary>
        public abstract Type type { get; }
        
        /// <summary>
        /// 引用计数
        /// </summary>
        private byte ReferenceCount { get; set; }

        /// <summary>
        /// 保留对象,  如果Var变量是在携程中进行使用,则需要在携程开始时要调用一次,防止同步方法中变量被释放掉.
        /// </summary>
        public void Retain()
        {
            ReferenceCount++;
        }

        /// <summary>
        /// 释放对象, 使用过之后一定要释放,也就是和Retain / Alloc 这两个一定要成对出现
        /// </summary>
        public void Release()
        {
            ReferenceCount--;
            if (ReferenceCount < 1)
            {
                GameEntry.Pool.EnqueueVarObject(this);
            }
        }
    }
}