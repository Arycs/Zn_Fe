using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZnFramework
{
    /// <summary>
    /// 资源信息实体
    /// </summary>
    public class AssetEntity
    {
        /// <summary>
        /// 资源类型
        /// </summary>
        public AssetCategory Category;

        /// <summary>
        /// 资源名称
        /// </summary>
        public string AssetName;

        /// <summary>
        /// 资源完整名称
        /// </summary>
        public string AssetFullName;

        /// <summary>
        /// 所属AssetBundle名称
        /// </summary>
        public string AssetBundleName;

        /// <summary>
        /// 依赖资源
        /// </summary>
        public List<AssetDependEntity> DependsAssetList;

        /// <summary>
        /// 引用资源
        /// </summary>
        public List<AssetReferenceEntity> RefrenceAssetList;
    }
}