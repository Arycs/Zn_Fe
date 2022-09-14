using System;
using System.Collections.Generic;

namespace ZnFramework
{
    /// <summary>
    /// 资源管理器
    /// </summary>
    public class ResourceManager
    {
        /// <summary>
        /// StreamingAssets(只读区)资源管理器
        /// </summary>
        public StreamingAssetsManager StreamingAssetsManager { get; private set; }

        public ResourceManager()
        {
            StreamingAssetsManager = new StreamingAssetsManager();
        }

        public void Init()
        {
            
        }

        public void Dispose()
        {
            m_StreamingAssetsVersionDic.Clear();
        }

        #region 只读区

        /// <summary>
        /// 只读区版本号
        /// </summary>
        private string m_StreamingAssetsVersion;

        /// <summary>
        /// 只读区AssetBundle信息
        /// </summary>
        private Dictionary<string, AssetBundleInfoEntity> m_StreamingAssetsVersionDic;

        /// <summary>
        /// 是否存在只读区AssetBundle信息
        /// </summary>
        private bool m_IsExistsStreamingAssetsBundleInfo = false;

        #region InitStreamingAssetsBundleInfo 初始化只读区资源包信息

        /// <summary>
        /// 初始化只读区资源包信息
        /// </summary>
        public void InitStreamingAssetsBundle()
        {
            ReadStreamingAssetsBundle(ConstDefine.VersionFileName, (byte[] buffer) =>
            {
                if (buffer == null)
                {
                    GameEntry.LogInfo(LogCategory.Resource,$"{ConstDefine.VersionFileName}版本文件丢失,请检查");
                }
                else
                {
                    m_IsExistsStreamingAssetsBundleInfo = true;
                    m_StreamingAssetsVersionDic = GetAssetBundleVersionList(buffer, ref m_StreamingAssetsVersion);
                    GameEntry.Procedure.ChangeState(ProcedureState.PreLoad);
                }
            });
        }

        #endregion

        #region ReadStreamingAssetsBundle 读取只读区的资源包

        private void ReadStreamingAssetsBundle(string fileUrl, Action<byte[]> onComplete)
        {
            StreamingAssetsManager.ReadAssetBundle(fileUrl,onComplete);
        }

        #endregion

        #endregion

        #region 可写区

        //TODO 目前是单机框架 资源随包出 有需要后续添加

        #endregion

        #region CDN

        //TODO 目前是单机框架 资源随包出 有需要后续添加

        #endregion
        
        #region GetAssetBundleVersionList 根据字节数组获取资源包版本信息

        /// <summary>
        /// 根据字节数组获取资源包版本信息
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public static Dictionary<string, AssetBundleInfoEntity> GetAssetBundleVersionList(byte[] buffer,
            ref string version)
        {
            buffer = ZlibUtil.DeCompressBytes(buffer);
            var dic = new Dictionary<string, AssetBundleInfoEntity>();
            var ms = new ZnMemoryStream(buffer);
            var len = ms.ReadInt();
            for (int i = 0; i < len; i++)
            {
                if (i == 0)
                {
                    version = ms.ReadUTF8String().Trim();
                }
                else
                {
                    var entity = new AssetBundleInfoEntity()
                    {
                        AssetBundleName = ms.ReadUTF8String(),
                        MD5 = ms.ReadUTF8String(),
                        Size = ms.ReadULong(),
                        IsFirstData = ms.ReadByte() == 1,
                        IsEncrypt = ms.ReadByte() == 1,
                    };
                    dic[entity.AssetBundleName] = entity;
                }
            }

            return dic;
        }

        #endregion

        #region GetAssetBundleInfo 获取资源包信息

        /// <summary>
        /// 获取资源包信息
        /// </summary>
        /// <param name="assetBundlePath"></param>
        /// <returns></returns>
        public AssetBundleInfoEntity GetAssetBundleInfo(string assetBundlePath)
        {
            m_StreamingAssetsVersionDic.TryGetValue(assetBundlePath, out var entity);
            return entity;
        }

        #endregion
    }
}