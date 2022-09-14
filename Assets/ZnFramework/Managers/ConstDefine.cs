using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZnFramework
{
    public class ConstDefine
    {
        /// <summary>
        /// 版本文件名称
        /// </summary>
        public const string VersionFileName = "VersionFile.bytes";

        /// <summary>
        /// 资源信息文件名称
        /// </summary>
        public const string AssetInfoName = "AssetInfo.bytes";
        
        /// <summary>
        /// 类对象池的释放间隔
        /// </summary>
        public const string Pool_ReleaseClassObjectInterval = "Pool_ReleaseClassObjectInterval";

        /// <summary>
        /// AssetBundle池的释放间隔
        /// </summary>
        public const string Pool_ReleaseAssetBundleInterval = "Pool_ReleaseAssetBundleInterval";

        /// <summary>
        /// Asset池的释放间隔
        /// </summary>
        public const string Pool_ReleaseAssetInterval = "Pool_ReleaseAssetInterval";
        
        /// <summary>
        /// UI池中最大数量
        /// </summary>
        public const string UI_PoolMaxCount = "UI_PoolMaxCount";

        /// <summary>
        /// UI过期时间
        /// </summary>
        public const string UI_Exprie = "UI_Exprie";

        /// <summary>
        /// UI清理间隔
        /// </summary>
        public const string UI_ClearInterval = "UI_ClearInterval";
    }
}