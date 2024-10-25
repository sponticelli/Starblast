using Starblast.Pools;
using UnityEngine;

namespace Starblast.Entities
{
    public class DestroyAfterDelay : MonoBehaviour
    {
        [SerializeField] private float delay;
        private PooledObject _pooledObject;
        
        private void OnEnable()
        {
            if (_pooledObject == null) _pooledObject = GetComponent<PooledObject>();
            Invoke(nameof(DestroyObject), delay);
        }

        private void DestroyObject()
        {
            if (_pooledObject == null)
            {
                Debug.Log("Destroying object " + gameObject.name);
                Destroy(gameObject);
                return;
            }

            _pooledObject.ReturnToPool();
        }
        
        private void OnDestroy()
        {
            Debug.Log("Destroyed object " + gameObject.name);
        }
    }
}