using UnityEngine;

namespace Starblast.Pools
{
    public class PooledObject : MonoBehaviour
    {
        private IPoolManager _poolManager;
        private GameObject _prefab;

        public void SetPool(IPoolManager manager, GameObject original)
        {
            Debug.Assert(manager != null, "PoolManager reference is missing.");
            _poolManager = manager;
            _prefab = original;
        }

        public void ReturnToPool()
        {
            if (_poolManager != null)
            {
                _poolManager.ReturnPooledObject(_prefab, gameObject);
            }
            else
            {
                Debug.LogWarning("PoolManager reference is missing. Object will be destroyed.");
                Destroy(gameObject);
            }
        }
    }
}