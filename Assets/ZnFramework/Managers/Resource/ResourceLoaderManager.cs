using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ZnFramework
{
    /// <summary>
    /// 资源加载管理器
    /// </summary>
    public class ResourceLoaderManager
    {
        private Dictionary<AssetCategory, Dictionary<string, AssetEntity>> m_AssetInfoDic;

        private LinkedList<AssetBundleLoaderRoutine> m_AssetBundleLoaderList;

        private LinkedList<AssetLoaderRoutine> m_AssetLoaderList;

        public ResourceLoaderManager()
        {
            m_AssetInfoDic = new Dictionary<AssetCategory, Dictionary<string, AssetEntity>>();
            var enumerator = Enum.GetValues(typeof(AssetCategory)).GetEnumerator();
            while (enumerator.MoveNext())
            {
                var assetCategory = (AssetCategory) enumerator.Current;
                m_AssetInfoDic[assetCategory] = new Dictionary<string, AssetEntity>();
            }

            m_AssetBundleLoaderList = new LinkedList<AssetBundleLoaderRoutine>();
            m_AssetLoaderList = new LinkedList<AssetLoaderRoutine>();
        }

        public void Init()
        {
            
        }

        public void OnUpdate()
        {
            for (var curr = m_AssetBundleLoaderList.First; curr != null; curr = curr.Next)
            {
                curr.Value?.OnUpdate();
            }

            for (var curr = m_AssetLoaderList.First; curr != null; curr = curr.Next)
            {
                curr.Value?.OnUpdate();
            }
        }

        public void Dispose()
        {
            m_AssetInfoDic.Clear();
            m_AssetBundleLoaderList.Clear();
            m_AssetLoaderList.Clear();
        }


        #region InitAssetInfo 初始化资源信息

        /// <summary>
        /// 初始化资源信息
        /// </summary>
        public void InitAssetInfo()
        {
           GameEntry.Resource.ResourceManager.StreamingAssetsManager.ReadAssetBundle(ConstDefine.AssetInfoName,
                InitAssetInfo);
        }

        private void InitAssetInfo(byte[] buffer)
        {
            buffer = ZlibUtil.DeCompressBytes(buffer);
            var ms = new ZnMemoryStream(buffer);
            var len = ms.ReadInt();
            for (int i = 0; i < len; i++)
            {
                var entity = new AssetEntity()
                {
                    Category = (AssetCategory) ms.ReadByte(),
                    AssetFullName = ms.ReadUTF8String(),
                    AssetBundleName = ms.ReadUTF8String()
                };
                var depLen = ms.ReadInt();
                if (depLen > 0)
                {
                    entity.DependsAssetList = new List<AssetDependEntity>();
                    for (int j = 0; j < depLen; j++)
                    {
                        var assetDepends = new AssetDependEntity()
                        {
                            Category = (AssetCategory) ms.ReadByte(),
                            AssetFullName = ms.ReadUTF8String(),
                        };
                        entity.DependsAssetList.Add(assetDepends);
                    }
                }

                m_AssetInfoDic[entity.Category][entity.AssetFullName] = entity;
            }
        }

        #endregion

        #region GetAssetEntity 根据资源分类和资源路径获取资源信息

        /// <summary>
        /// 根据资源分类和资源路径获取资源信息
        /// </summary>
        /// <param name="assetCategory"></param>
        /// <param name="assetFullName"></param>
        /// <returns></returns>
        public AssetEntity GetAssetEntity(AssetCategory assetCategory, string assetFullName)
        {
            if (m_AssetInfoDic.TryGetValue(assetCategory, out var dicCategory))
            {
                if (dicCategory.TryGetValue(assetFullName, out var entity))
                {
                    return entity;
                }
            }

            GameEntry.LogInfo(LogCategory.Resource, $"assetFullName =>{assetFullName} 不存在");
            return null;
        }

        #endregion

        #region 加载主资源

        /// <summary>
        /// 加载主资源
        /// </summary>
        /// <param name="assetCategory"></param>
        /// <param name="assetFullName"></param>
        /// <param name="onComplete"></param>
        public void LoadMainAsset(AssetCategory assetCategory, string assetFullName,
            Action<ResourceEntity> onComplete = null)
        {
            var routine = GameEntry.Pool.DequeueClassObject<MainAssetLoaderRoutine>();
            routine.Load(assetCategory, assetFullName, (resEntity) =>
            {
                onComplete?.Invoke(resEntity);
            });
        }

        #endregion
        
        #region LoadAssetBundle 加载资源包

        private Dictionary<string, LinkedList<Action<AssetBundle>>> m_LoadingAssetBundle =
            new Dictionary<string, LinkedList<Action<AssetBundle>>>();

        /// <summary>
        /// 加载AssetBundle包
        /// </summary>
        /// <param name="assetBundlePath">路径</param>
        /// <param name="onUpdate">加载中事件</param>
        /// <param name="onComplete">完成事件</param>
        public void LoadAssetBundle(string assetBundlePath, Action<float> onUpdate = null,
            Action<AssetBundle> onComplete = null)
        {
            //1. 判断资源是否在AssetBundlePool
            var assetBundleEntity = GameEntry.Pool.AssetBundlePool.Spawn(assetBundlePath);
            if (assetBundleEntity != null)
            {
                var assetBundle = assetBundleEntity.Target as AssetBundle;
                onComplete?.Invoke(assetBundle);
                return;
            }

            if (m_LoadingAssetBundle.TryGetValue(assetBundlePath, out var lst))
            {
                //正在加载中的Bundle, 把委托加入对应的链表, 然后直接返回
                lst.AddLast(onComplete);
                return;
            }
            else
            {
                lst = GameEntry.Pool.DequeueClassObject<LinkedList<Action<AssetBundle>>>();
                lst.AddLast(onComplete);
                m_LoadingAssetBundle[assetBundlePath] = lst;
            }

            var routine = GameEntry.Pool.DequeueClassObject<AssetBundleLoaderRoutine>();

            //加入链表开始循环
            m_AssetBundleLoaderList.AddLast(routine);
            routine.LoadAssetBundle(assetBundlePath);
            routine.OnAssetBundleCreateUpdate = (progress) => { onUpdate?.Invoke(progress); };

            routine.OnLoadAssetBundleComplete = (assetBundle) =>
            {
                //把资源注册到资源池
                assetBundleEntity = GameEntry.Pool.DequeueClassObject<ResourceEntity>();
                assetBundleEntity.ResourceName = assetBundlePath;
                assetBundleEntity.IsAssetBundle = true;
                assetBundleEntity.Target = assetBundle;
                GameEntry.Pool.AssetBundlePool.Register(assetBundleEntity);

                for (var curr = lst.First; curr != null; curr = curr.Next)
                {
                    curr.Value?.Invoke(assetBundle);
                }

                lst.Clear();
                GameEntry.Pool.EnqueueClassObject(lst);

                m_LoadingAssetBundle.Remove(assetBundlePath); //资源加载完毕

                m_AssetBundleLoaderList.Remove(routine);
                GameEntry.Pool.EnqueueClassObject(routine);
            };
        }

        #endregion

        #region LoadAsset 从资源包中加载资源

        /// <summary>
        /// 加载中的资源
        /// </summary>
        private Dictionary<string, LinkedList<Action<Object>>> m_LoadingAsset =
            new Dictionary<string, LinkedList<Action<Object>>>();

        /// <summary>
        /// 从AssetBundle包中加载资源
        /// </summary>
        /// <param name="assetName">资源名称</param>
        /// <param name="assetBundle">AssetBundle名称</param>
        /// <param name="onUpdate">加载中事件</param>
        /// <param name="onComplete">加载完成事件</param>
        public void LoadAsset(string assetName, AssetBundle assetBundle, Action<float> onUpdate = null,
            Action<Object> onComplete = null)
        {
            if (m_LoadingAsset.TryGetValue(assetName, out var lst))
            {
                //如果正在加载,则委托加入对应链表,直接返回
                lst.AddLast(onComplete);
                return;
            }
            else
            {
                lst = GameEntry.Pool.DequeueClassObject<LinkedList<Action<Object>>>();
                lst.AddLast(onComplete);
                m_LoadingAsset[assetName] = lst;
            }

            var routine = GameEntry.Pool.DequeueClassObject<AssetLoaderRoutine>();
            m_AssetLoaderList.AddLast(routine);
            
            routine.LoadAsset(assetName,assetBundle);
            routine.OnAssetUpdate = (progress) =>
            {
                onUpdate?.Invoke(progress);
            };

            routine.OnLoadAssetComplete = (obj) =>
            {
                for (var curr = lst.First; curr  != null; curr = curr.Next)
                {
                    curr.Value?.Invoke(obj);
                }
                lst.Clear();
                GameEntry.Pool.EnqueueClassObject(lst);
                m_LoadingAsset.Remove(assetName); //资源加载完毕,从加载中字典移除

                m_AssetLoaderList.Remove(routine);
                GameEntry.Pool.EnqueueClassObject(routine);
            };
        }

        #endregion

        #region UnLoadGameObject 释放资源

        /// <summary>
        /// 释放资源,通过LoadMainAsset加载出来的,不用的时候 调用这个方法,防止内存占用
        /// </summary>
        /// <param name="gameObject"></param>
        public void UnLoadGameObject(GameObject gameObject)
        {
            GameEntry.Pool.ReleaseInstanceResource(gameObject.GetInstanceID());
        }

        #endregion
        
    }
}