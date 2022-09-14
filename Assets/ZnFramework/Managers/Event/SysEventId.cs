using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZnFramework
{
    public class SysEventId
    {
        /// <summary>
        /// 加载表格完毕
        /// </summary>
        public const ushort LoadDataTableComplete = 1001;

        /// <summary>
        /// 加载单一表格完毕
        /// </summary>
        public const ushort LoadOneDataTableComplete = 1002;

        /// <summary>
        /// 加载进度条更新事件
        /// </summary>
        public const ushort LoadingProgressChange = 1101;
    }
}