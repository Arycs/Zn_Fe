using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZnFramework
{
    /// <summary>
    /// 资源实体 == AssetBundle == / == Asset ==
    /// </summary>
    public class ResourceEntity
    {
        public ResourceEntity()
        {
            DependsResourceList = new LinkedList<ResourceEntity>();
        }

        /// <summary>
        /// 资源名字
        /// </summary>
        public string ResourceName;

        /// <summary>
        /// 资源分类(用于Asset)
        /// </summary>
        public AssetCategory Category;

        /// <summary>
        /// 是否AssetBundle
        /// </summary>
        public bool IsAssetBundle;

        /// <summary>
        /// 关联目标
        /// </summary>
        public object Target;

        /// <summary>
        /// 上次使用时间
        /// </summary>
        public float LastUseTime;

        /// <summary>
        /// 引用计数
        /// </summary>
        public int ReferenceCount { get; private set; }
        
        /// <summary>
        /// 依赖的资源实体链表
        /// </summary>
        public LinkedList<ResourceEntity> DependsResourceList { private set; get; }


        /// <summary>
        /// 对象取池
        /// </summary>
        public void Spawn()
        {
            LastUseTime = Time.time;
            if (!IsAssetBundle)
            {
                ReferenceCount++;
            }
            else
            {
                ReferenceCount = 1;
            }
        }
        
        /// <summary>
        /// 对象回池
        /// </summary>
        public void UnSpawn()
        {
            LastUseTime = Time.time;
            ReferenceCount--;
            if (ReferenceCount < 0)
            {
                ReferenceCount = 0;
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Release()
        {
            ResourceName = null;
            ReferenceCount = 0;
            if (IsAssetBundle)
            {
                var bundle = Target as AssetBundle;
                bundle.Unload(false);
            }

            Target = null;
            DependsResourceList.Clear();//把自己依赖的资源实体清空
            GameEntry.Pool.EnqueueClassObject(this); //把这个资源实体回池
        }

        /// <summary>
        /// 对象是否可以释放
        /// </summary>
        /// <returns></returns>
        public bool GetCanRelease()
        {
            return ReferenceCount == 0 && Time.time - LastUseTime > GameEntry.Pool.ReleaseAssetBundleInterval;
        }

    }
}