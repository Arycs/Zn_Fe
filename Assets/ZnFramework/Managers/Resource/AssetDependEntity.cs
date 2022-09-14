using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZnFramework
{
    /// <summary>
    /// 资源依赖项实体
    /// </summary>
    public class AssetDependEntity
    {
        /// <summary>
        /// 资源分类
        /// </summary>
        public AssetCategory Category;

        /// <summary>
        /// 资源完整名称
        /// </summary>
        public string AssetFullName;
        
        /// <summary>
        /// 所属AssetBundle名称
        /// </summary>
        public string AssetBundleName;
    }
}