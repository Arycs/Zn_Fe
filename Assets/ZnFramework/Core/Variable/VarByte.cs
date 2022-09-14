namespace ZnFramework
{
    /// <summary>
    /// byte变量
    /// </summary>
    public class VarByte : Variable<byte>
    {
        /// <summary>
        /// 分配一个对象
        /// </summary>
        /// <returns></returns>
        public static VarByte Alloc()
        {
            var var = GameEntry.Pool.DequeueVarObject<VarByte>();
            var.Value = 0; //对其进行初始化,防止其他对象数据回池没清空
            var.Retain();
            return var;
        }

        /// <summary>
        /// 分配一个对象, Alloc在同步情况下 与Release是成对出现的, Alloc要最开始声明, Release要在结束声明
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static VarByte Alloc(byte value)
        {
            var var = Alloc();
            var.Value = value;
            return var;
        }

        /// <summary>
        /// 重写运算符 VarByte -> byte
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator byte(VarByte value)
        {
            return value.Value;
        }
    }
}