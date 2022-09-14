using System;
using System.Collections;
using System.Collections.Generic;
using PathologicalGames;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ZnFramework
{
    /// <summary>
    /// 游戏物体对象池
    /// </summary>
    public class GameObjectPool : IDisposable
    {
        /// <summary>
        /// 游戏物体对象池字典
        /// </summary>
        private Dictionary<byte, GameObjectPoolEntity> m_SpawnPoolDic;

        /// <summary>
        /// 实例ID对应对象池ID
        /// </summary>
        private Dictionary<int, byte> m_InstanceIdPoolIdDic;

        /// <summary>
        /// 空闲预设池队列, 相当于对这个预设池再加了一层池
        /// </summary>
        private Queue<PrefabPool> m_PrefabPoolQueue;


        public GameObjectPool()
        {
            m_SpawnPoolDic = new Dictionary<byte, GameObjectPoolEntity>();
            m_InstanceIdPoolIdDic = new Dictionary<int, byte>();
            m_PrefabPoolQueue = new Queue<PrefabPool>();

            InstanceHandler.InstantiateDelegates += this.InstantiateDelegate;
            InstanceHandler.DestroyDelegates += this.DestroyDelegate;
        }

        public void Dispose()
        {
            m_SpawnPoolDic.Clear();
        }

        /// <summary>
        /// 当对象池物体创建时候
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <param name="userData"></param>
        /// <returns></returns>
        public GameObject InstantiateDelegate(GameObject prefab, Vector3 pos, Quaternion rot, object userData)
        {
            ResourceEntity resourceEntity = userData as ResourceEntity;

            if (resourceEntity != null)
            {
                Debug.LogError("resourceEntity= " + resourceEntity.ResourceName);
            }

            GameObject obj = UnityEngine.Object.Instantiate(prefab, pos, rot) as GameObject;

            //注册
            GameEntry.Pool.RegisterInstanceResource(obj.GetInstanceID(), resourceEntity);
            return obj;
        }

        /// <summary>
        /// 当对象池物体销毁的时候
        /// </summary>
        /// <param name="instance"></param>
        public void DestroyDelegate(GameObject instance)
        {
            UnityEngine.Object.Destroy(instance);
            GameEntry.Resource.ResourceLoaderManager.UnLoadGameObject(instance);
        }

        /// <summary>
        /// 切换场景的时候 销毁需要时间,因此使用协程
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public IEnumerator Init(GameObjectPoolEntity[] arr, Transform parent)
        {
            int len = arr.Length;
            for (int i = 0; i < len; i++)
            {
                GameObjectPoolEntity entity = arr[i];
                if (entity.Pool != null)
                {
                    Object.Destroy(entity.Pool.gameObject);
                    yield return null;
                    entity.Pool = null;
                }

                //创建对象池
                SpawnPool pool = PathologicalGames.PoolManager.Pools.Create(entity.PoolName);
                pool.group.parent = parent;
                pool.group.localPosition = Vector3.zero;
                entity.Pool = pool;

                m_SpawnPoolDic[entity.PoolId] = entity;
            }
        }

        /// <summary>
        /// 加载中的预设池
        /// </summary>
        private Dictionary<int, HashSet<Action<SpawnPool, Transform, ResourceEntity>>> m_LoadingPrefabPoolDic =
            new Dictionary<int, HashSet<Action<SpawnPool, Transform, ResourceEntity>>>();

        /// <summary>
        /// 从对象池中获取对象
        /// </summary>
        /// <param name="prefabId"></param>
        /// <param name="onComplete"></param>
        public void Spawn(int prefabId, Action<Transform, bool> onComplete)
        {
            //拿到预设表数据
            DTSys_PrefabEntity entity = GameEntry.DataTable.Sys_PrefabDBModel.Get(prefabId);
            if (entity == null)
            {
                Debug.LogError("预设数据不存在");
                return;
            }

            Spawn(entity.Id, entity.PoolId, entity.AssetCategory, entity.AssetPath, entity.CullDespawned == 1,
                entity.CullAbove, entity.CullDelay, entity.CullMaxPerPass, onComplete);
        }

        public void Spawn(string assetPath, Action<Transform, bool> onComplete)
        {
            int prefabId = assetPath.GetHashCode();
            byte poolId = 1;
            Spawn(prefabId, poolId, (int) AssetCategory.EffectSources, assetPath, true, 0, 10, 2, onComplete);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefabId"></param>
        /// <param name="poolId"></param>
        /// <param name="assetCategory"></param>
        /// <param name="assetPath"></param>
        /// <param name="cullDespawned"></param>
        /// <param name="cullAbove"></param>
        /// <param name="cullDelay"></param>
        /// <param name="cullMaxPerPass"></param>
        /// <param name="onComplete"></param>
        public void Spawn(int prefabId, byte poolId, int assetCategory, string assetPath, bool cullDespawned,
            int cullAbove, int cullDelay, int cullMaxPerPass, Action<Transform, bool> onComplete)
        {
            //拿到对象池
            GameObjectPoolEntity gameObjectPoolEntity = m_SpawnPoolDic[poolId];

            //使用预设编号 当做池ID
            //Debug.LogError($"prefabId={prefabId} Name{entity.Name}");
            PrefabPool prefabPool = gameObjectPoolEntity.Pool.GetPrefabPool(prefabId);
            if (prefabPool != null)
            {
                //拿到一个实例 激活一个已有的
                Transform retTrans = prefabPool.TrySpawnInstance();
                if (retTrans != null)
                {
                    //Debug.LogError($"prefabId={prefabId} Name{entity.Name} 拿到一个实例 激活一个已有的");
                    int instanceID = retTrans.gameObject.GetInstanceID();
                    m_InstanceIdPoolIdDic[instanceID] = poolId;
                    onComplete?.Invoke(retTrans, false);
                    return;
                }
            }

            if (m_LoadingPrefabPoolDic.TryGetValue(prefabId, out var lst))
            {
                //进行拦截
                //如果存在加载中的asset 把委托加入对应的链表 然后直接返回
                lst.Add((_SpawnPool, _Transform, _ResourceEntity) =>
                {
                    //拿到一个实例
                    bool isNewInstance = false;
                    Transform retTrans = _SpawnPool.Spawn(_Transform, ref isNewInstance, _ResourceEntity, poolId);
                    int instanceID = retTrans.gameObject.GetInstanceID();
                    m_InstanceIdPoolIdDic[instanceID] = poolId;
                    onComplete?.Invoke(retTrans, isNewInstance);
                });
                return;
            }

            //这里说明是加载在第一个
            lst = GameEntry.Pool.DequeueClassObject<HashSet<Action<SpawnPool, Transform, ResourceEntity>>>();
            lst.Add((_SpawnPool, _Transform, _ResourceEntity) =>
            {
                //拿到一个实例
                bool isNewInstance = false;
                Transform retTrans = _SpawnPool.Spawn(_Transform, ref isNewInstance, _ResourceEntity, poolId);
                int instanceID = retTrans.gameObject.GetInstanceID();
                m_InstanceIdPoolIdDic[instanceID] = poolId;
                onComplete?.Invoke(retTrans, isNewInstance);
            });
            m_LoadingPrefabPoolDic[prefabId] = lst;

            GameEntry.Resource.ResourceLoaderManager.LoadMainAsset((AssetCategory) assetCategory, assetPath,
                (ResourceEntity resourceEntity) =>
                {
                    Transform prefab = ((GameObject) resourceEntity.Target).transform;

                    PrefabPool prefabPoolInner = gameObjectPoolEntity.Pool.GetPrefabPool(prefabId);
                    if (prefabPoolInner == null)
                    {
                        #region 实例化池

                        //先去队列里找 空闲的池
                        if (m_PrefabPoolQueue.Count > 0)
                        {
                            prefabPoolInner = m_PrefabPoolQueue.Dequeue();

                            prefabPoolInner.PrefabPoolId = prefabId; //设置预设池编号
                            gameObjectPoolEntity.Pool.AddPrefabPool(prefabPoolInner);

                            //Debug.LogError("先去队列里找 空闲的池="+prefab.gameObject.name);
                            prefabPoolInner.prefab = prefab;
                            prefabPoolInner.prefabGO = prefab.gameObject;
                            prefabPoolInner.AddPrefabToDic(prefab.name, prefab);
                        }
                        else
                        {
                            //Debug.LogError("new PrefabPool="+prefab.gameObject.name);
                            prefabPoolInner = new PrefabPool(prefab, prefabId);
                            gameObjectPoolEntity.Pool.CreatePrefabPool(prefabPoolInner, resourceEntity);
                        }

                        prefabPoolInner.OnPrefabPoolClear = (PrefabPool pool) =>
                        {
                            //Debug.LogError("nPrefabPoolClear="+pool.prefabGO.name);
                            //预设池加入队列
                            pool.PrefabPoolId = 0;
                            gameObjectPoolEntity.Pool.RemovePrefabPool(pool);
                            m_PrefabPoolQueue.Enqueue(pool);
                        };

                        //这些属性要从表格中读取
                        prefabPoolInner.cullDespawned = cullDespawned;
                        prefabPoolInner.cullAbove = cullAbove;
                        prefabPoolInner.cullDelay = cullDelay;
                        prefabPoolInner.cullMaxPerPass = cullMaxPerPass;

                        #endregion
                    }

                    var enumerator = lst.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current?.Invoke(gameObjectPoolEntity.Pool, prefab, resourceEntity);
                    }

                    m_LoadingPrefabPoolDic.Remove(prefabId);
                    lst.Clear(); //一定要清空
                    GameEntry.Pool.EnqueueClassObject(lst);
                });
        }

        #region Despawn 对象回池

        /// <summary>
        /// 对象回池
        /// </summary>
        /// <param name="poolId"></param>
        /// <param name="instance"></param>
        public void Despawn(byte poolId, Transform instance)
        {
            GameObjectPoolEntity entity = m_SpawnPoolDic[poolId];
            instance.SetParent(entity.Pool.transform); //重置到原始对象池节点下
            entity.Pool.Despawn(instance);
        }

        /// <summary>
        /// 对象回池
        /// </summary>
        /// <param name="instance"></param>
        public void Despawn(Transform instance)
        {
            int instanceID = instance.gameObject.GetInstanceID();
            byte poolId = m_InstanceIdPoolIdDic[instanceID];
            m_InstanceIdPoolIdDic.Remove(instanceID);
            Despawn(poolId, instance);
        }

        #endregion
    }
}