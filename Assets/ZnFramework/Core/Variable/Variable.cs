using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZnFramework
{
    /// <summary>
    /// 变量泛型基类,因为有些属性需要定义成泛型
    /// TODO 理论上游戏中需要用到的类型都应该封装一层,防止在传递过程中有拆装箱的损耗
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Variable<T> : VariableBase
    {
        /// <summary>
        /// 变量类型
        /// </summary>
        public override Type type => typeof(T);

        /// <summary>
        /// 当前真实值
        /// </summary>
        public T Value;
    }
}