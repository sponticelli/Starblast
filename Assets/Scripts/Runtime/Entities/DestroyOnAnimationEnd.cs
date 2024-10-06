using Starblast.Pools;
using UnityEngine;

namespace Starblast.Entities
{
    public class DestroyOnAnimationEnd : MonoBehaviour
    {
        private PooledObject _pooledObject;
        
        private void OnEnable()
        {
            if (_pooledObject == null) _pooledObject = GetComponent<PooledObject>();
        }
        
        public void OnAnimationEnd()
        {
            if (_pooledObject == null)
            {
                Destroy(gameObject);
                return;
            }

            _pooledObject.ReturnToPool();
        }
    }
}