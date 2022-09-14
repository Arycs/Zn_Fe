using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZnFramework
{
    /// <summary>
    /// 资源组件
    /// </summary>
    public class AddressableManager
    {
        /// <summary>
        /// 本地文件路径
        /// </summary>
        public string LocalFilePath;

        /// <summary>
        /// 资源管理器
        /// </summary>
        public ResourceManager ResourceManager { get; private set; }

        /// <summary>
        /// 资源加载管理器
        /// </summary>
        public ResourceLoaderManager ResourceLoaderManager { get; private set; }

        public AddressableManager()
        {
            ResourceManager = new ResourceManager();
            ResourceLoaderManager = new ResourceLoaderManager();
#if DISABLE_ASSETBUNDLE
            LocalFilePath = Application.dataPath;
#else
            LocalFilePath = Application.persistentDataPath;
#endif
        }

        public void Init()
        {
            ResourceManager.Init();
            ResourceLoaderManager.Init();
        }

        public void OnUpdate()
        {
            ResourceLoaderManager.OnUpdate();
        }

        public void Dispose()
        {
            ResourceManager.Dispose();
            ResourceLoaderManager.Dispose();
        }

        #region InitStreamingAssetsBundleInfo 初始化只读区资源包信息

        /// <summary>
        /// 初始化只读区资源包信息
        /// </summary>
        public void InitStreamingAssetsBundleInfo()
        {
            ResourceManager.InitStreamingAssetsBundle();
        }

        #endregion

        #region InitAssetInfo 初始化资源信息

        /// <summary>
        /// 初始化资源信息
        /// </summary>
        public void InitAssetInfo()
        {
            ResourceLoaderManager.InitAssetInfo();
        }

        #endregion

        #region GetLastPathName 获取路径的最后名称

        /// <summary>
        /// 获取路径的最后名称
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string GetLastPathName(string path)
        {
            return path.IndexOf('/') == -1 ? path : path.Substring(path.LastIndexOf('/') + 1);
        }

        #endregion

        #region GetSceneAssetBundlePath 获取场景的资源包路径

        /// <summary>
        /// 获取场景的资源包路径
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        public string GetSceneAssetBundlePath(string sceneName)
        {
            return $"download/scenes/normalscene/{sceneName.ToLower()}.assetbundle";
        }

        #endregion
    }
}