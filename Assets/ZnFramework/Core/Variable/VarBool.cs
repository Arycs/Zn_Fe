namespace ZnFramework
{
    /// <summary>
    /// bool变量
    /// </summary>
    public class VarBool : Variable<bool>
    {
        /// <summary>
        /// 分配一个对象
        /// </summary>
        /// <returns></returns>
        public static VarBool Alloc()
        {
            var var = GameEntry.Pool.DequeueVarObject<VarBool>();
            var.Value = false; //对其进行初始化,防止其他对象数据回池没清空
            var.Retain();
            return var;
        }

        /// <summary>
        /// 分配一个对象, Alloc在同步情况下 与Release是成对出现的, Alloc要最开始声明, Release要在结束声明
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static VarBool Alloc(bool value)
        {
            var var = Alloc();
            var.Value = value;
            return var;
        }

        /// <summary>
        /// 重写运算符 VarBool -> bool
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator bool(VarBool value)
        {
            return value.Value;
        }
    }
}