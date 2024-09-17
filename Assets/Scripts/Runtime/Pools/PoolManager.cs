using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

namespace Starblast.Pools
{
    [DefaultExecutionOrder(ExecutionOrder.PoolManager)]
    public class PoolManager : MonoBehaviour, IPoolManager
    {
        [System.Serializable]
        public class PooledObjectInfo
        {
            public GameObject prefab;
            public int initialPoolSize;
            public int maxPoolSize;
            [HideInInspector] public Transform poolParent;
        }

        public List<PooledObjectInfo> pooledObjects;

        private Dictionary<GameObject, ObjectPool<GameObject>> pools;
        private Dictionary<GameObject, List<GameObject>> activeObjects;

        public bool IsInitialized { get; private set; }

        public void Initialize()
        {
            InitializePools();
        }

        private void InitializePools()
        {
            if (IsInitialized) return;
            IsInitialized = true;
            pools = new Dictionary<GameObject, ObjectPool<GameObject>>();
            activeObjects = new Dictionary<GameObject, List<GameObject>>();

            foreach (var obj in pooledObjects)
            {
                obj.poolParent = new GameObject(obj.prefab.name + "Pool").transform;
                obj.poolParent.SetParent(transform);

                ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
                    createFunc: () =>
                    {
                        GameObject newObj = Instantiate(obj.prefab, obj.poolParent);
                        newObj.name = obj.prefab.name + " " + obj.poolParent.childCount;
                        PooledObject pooledObj = newObj.AddComponent<PooledObject>();
                        pooledObj.SetPool(this, obj.prefab);
                        newObj.SetActive(false);
                        return newObj;
                    },
                    actionOnGet: (GameObject pooledObject) =>
                    {
                        pooledObject.SetActive(true);
                        if (!activeObjects.ContainsKey(obj.prefab))
                        {
                            activeObjects[obj.prefab] = new List<GameObject>();
                        }

                        activeObjects[obj.prefab].Add(pooledObject);
                    },
                    actionOnRelease: (GameObject pooledObject) =>
                    {
                        pooledObject.SetActive(false);
                        pooledObject.transform.SetParent(obj.poolParent);
                        if (activeObjects.ContainsKey(obj.prefab))
                        {
                            activeObjects[obj.prefab].Remove(pooledObject);
                        }
                    },
                    actionOnDestroy: (GameObject pooledObject) => Destroy(pooledObject),
                    collectionCheck: false,
                    defaultCapacity: obj.initialPoolSize,
                    maxSize: obj.maxPoolSize
                );

                pools.Add(obj.prefab, pool);
                
                
                GameObject[] _items = new GameObject[obj.initialPoolSize];
                for (int i = 0; i < obj.initialPoolSize; i++)
                {
                    _items[i] = pool.Get();
                }

                for (int i = 0; i < obj.initialPoolSize; i++)
                {
                    pool.Release(_items[i]);
                }
            }
        }

        public GameObject GetPooledObject(GameObject prefab)
        {
            if (pools.TryGetValue(prefab, out ObjectPool<GameObject> pool))
            {
                GameObject obj = pool.Get();
                obj.transform.SetParent(null);
                return obj;
            }

            Debug.LogWarning($"Pool for prefab {prefab.name} doesn't exist.");
            return null;
        }

        public void ReturnPooledObject(GameObject prefab, GameObject obj)
        {
            if (pools.TryGetValue(prefab, out ObjectPool<GameObject> pool))
            {
                pool.Release(obj);
            }
            else
            {
                Debug.LogWarning($"Pool for prefab {prefab.name} doesn't exist. Object will be destroyed.");
                Destroy(obj);
            }
        }

        public void ReturnAllPooledObjects()
        {
            foreach (var kvp in activeObjects)
            {
                GameObject prefab = kvp.Key;
                List<GameObject>
                    objects = new List<GameObject>(kvp.Value); // Create a copy to avoid modification during iteration
                foreach (GameObject obj in objects)
                {
                    ReturnPooledObject(prefab, obj);
                }
            }
        }

        private void OnDestroy()
        {
            CleanupPools();
        }

        public void CleanupPools()
        {
            foreach (var pool in pools.Values)
            {
                pool.Clear();
            }

            foreach (var obj in pooledObjects)
            {
                if (obj.poolParent != null)
                {
                    Destroy(obj.poolParent.gameObject);
                }
            }

            pools.Clear();
            activeObjects.Clear();
        }

        
    }
}