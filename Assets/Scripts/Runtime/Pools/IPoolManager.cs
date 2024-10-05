using Starblast.Services;
using UnityEngine;

namespace Starblast.Pools
{
    public interface IPoolManager : IInitializable
    {
        GameObject GetPooledObject(GameObject prefab);
        void ReturnPooledObject(GameObject prefab, GameObject obj);
        void ReturnAllPooledObjects();
        void CleanupPools();
        
        void CreatePool(GameObject prefab, int initialPoolSize, int maxPoolSize);
        void DestroyPool(GameObject prefab);
    }
}