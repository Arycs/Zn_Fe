namespace ZnFramework
{
    /// <summary>
    /// String变量
    /// </summary>
    public class VarString : Variable<string>
    {
        /// <summary>
        /// 分配一个对象
        /// </summary>
        /// <returns></returns>
        public static VarString Alloc()
        {
            var var = GameEntry.Pool.DequeueVarObject<VarString>();
            var.Value = string.Empty; //对其进行初始化,防止其他对象数据回池没清空
            var.Retain();
            return var;
        }

        /// <summary>
        /// 分配一个对象, Alloc在同步情况下 与Release是成对出现的, Alloc要最开始声明, Release要在结束声明
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static VarString Alloc(string value)
        {
            var var = Alloc();
            var.Value = value;
            return var;
        }

        /// <summary>
        /// 重写运算符 VarString -> string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator string(VarString value)
        {
            return value.Value;
        }
    }
}