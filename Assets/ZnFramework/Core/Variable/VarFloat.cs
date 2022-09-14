namespace ZnFramework
{
    /// <summary>
    /// float变量
    /// </summary>
    public class VarFloat : Variable<float>
    {
        /// <summary>
        /// 分配一个对象
        /// </summary>
        /// <returns></returns>
        public static VarFloat Alloc()
        {
            var var = GameEntry.Pool.DequeueVarObject<VarFloat>();
            var.Value = 0; //对其进行初始化,防止其他对象数据回池没清空
            var.Retain();
            return var;
        }

        /// <summary>
        /// 分配一个对象, Alloc在同步情况下 与Release是成对出现的, Alloc要最开始声明, Release要在结束声明
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static VarFloat Alloc(float value)
        {
            var var = Alloc();
            var.Value = value;
            return var;
        }

        /// <summary>
        /// 重写运算符 VarFloat -> float
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator float(VarFloat value)
        {
            return value.Value;
        }
    }
}