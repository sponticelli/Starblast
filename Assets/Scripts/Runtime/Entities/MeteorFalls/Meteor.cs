using System;
using Starblast.Pools;
using UnityEngine;
using UnityEngine.Events;

namespace Starblast.Entities.MeteorFalls
{
    public class Meteor : MonoBehaviour
    {
        [Serializable]
        public class MeteorSettings
        {
            public float Speed;
            public Vector3 Direction;
            public bool useLifeTime;
            public float LifeTime;
            
        }


        [Header("Settings")]
        [SerializeField] private MeteorSettings _settings;

        [Header("Events")]
        [SerializeField] private UnityEvent onDestroyed;
        
        private float _lifeTime;
        private PooledObject _pooledObject;


        public void Initialize(MeteorSettings settings)
        {
            _settings = settings;
            _lifeTime = settings.LifeTime;
        }
        
        private void OnEnable()
        {
            if (_pooledObject == null) _pooledObject = GetComponent<PooledObject>();
        }

        private void Update()
        {
            if (_settings.useLifeTime)
            {
                _lifeTime -= Time.deltaTime;
                if (_lifeTime <= 0)
                {
                    Die();
                    return;
                }
            }

            transform.position += _settings.Direction * (_settings.Speed * Time.deltaTime);
        }
        
        private void Die()
        {
            onDestroyed?.Invoke();
            if (_pooledObject == null)
            {
                Debug.Log("Destroying " + gameObject.name);
                Destroy(gameObject);
                return;
            }
            _pooledObject.ReturnToPool();
        }

        private void OnDestroy()
        {
            Debug.Log("Destroyed " + gameObject.name);
        }
    }
}