using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZnFramework
{
    /// <summary>
    /// 只读区资源管理器
    /// </summary>
    public class StreamingAssetsManager
    {
        private string m_StreamingAssetsPath;

        public StreamingAssetsManager()
        {
            m_StreamingAssetsPath = "file://" + Application.streamingAssetsPath;
#if !UNITY_EDITOR
            m_StreamingAssetsPath = Application.streamingAssetsPath;
#endif
        }

        #region ReadAssetBundle 读取只读区资源包

        /// <summary>
        /// 读取只读区资源包
        /// </summary>
        /// <param name="fileUrl"></param>
        /// <param name="onComplete"></param>
        public void ReadAssetBundle(string fileUrl, Action<byte[]> onComplete)
        {
            GameEntry.Instance.StartCoroutine(ReadStreamingAsset($"{m_StreamingAssetsPath}/AssetBundles/{fileUrl}",
                onComplete));
        }

        #endregion

        #region ReadStreamingAsset 读取SteamingAssets下的资源

        /// <summary>
        /// 读取只读区下的资源
        /// </summary>
        /// <param name="url"></param>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        private IEnumerator ReadStreamingAsset(string url, Action<byte[]> onComplete)
        {
            var www = new WWW(url);
            yield return www;
            onComplete?.Invoke(www.error == null ? www.bytes : null);
        }

        #endregion
    }
}