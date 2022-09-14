using UnityEngine;

namespace ZnFramework
{
    /// <summary>
    /// color变量
    /// </summary>
    public class VarColor : Variable<Color>
    {
        /// <summary>
        /// 分配一个对象
        /// </summary>
        /// <returns></returns>
        public static VarColor Alloc()
        {
            var var = GameEntry.Pool.DequeueVarObject<VarColor>();
            var.Value = Vector4.zero; //对其进行初始化,防止其他对象数据回池没清空
            var.Retain();
            return var;
        }

        /// <summary>
        /// 分配一个对象, Alloc在同步情况下 与Release是成对出现的, Alloc要最开始声明, Release要在结束声明
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static VarColor Alloc(Color value)
        {
            var var = Alloc();
            var.Value = value;
            return var;
        }

        /// <summary>
        /// 重写运算符 VarColor -> color
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Color(VarColor value)
        {
            return value.Value;
        }
    }
}