using Starblast.Pools;
using Starblast.Services;
using UnityEngine;
using UnityEngine.Events;

namespace Starblast.Entities.Asteroids
{
    public class Asteroid : MonoBehaviour, IDamageable
    {
        [Header("Data")]
        [SerializeField] private AsteroidData asteroidData;
        
        [Header("Components")]
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private AsteroidMovement movement;
        
        [Header("Events")]
        [SerializeField] private UnityEvent onDestroyed;
        [SerializeField] private UnityEvent onDamaged;
        
        private float currentHealth;
        private PooledObject _pooledObject;
        
        
        private void OnEnable()
        {
            rb.mass = asteroidData.mass;
            if (movement != null)
            {
                movement.Initialize(rb,  asteroidData.speed, asteroidData.rotationSpeed);
            }
            currentHealth = asteroidData.health;
            if (_pooledObject == null) _pooledObject = GetComponent<PooledObject>();
        }

        public void TakeDamage(float damage)
        {
            if (currentHealth <= 0)
            {
                return;
            }
            
            currentHealth -= damage;
            onDamaged?.Invoke();
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            onDestroyed?.Invoke();
            if (asteroidData.size != AsteroidSize.Mini)
            {
                AsteroidFactory factory = ServiceLocator.Main.Get<AsteroidFactory>();

                foreach (var splittingInfo in asteroidData.splittingInfo)
                {
                    var splitCount = Random.Range(splittingInfo.minSplitCount, splittingInfo.maxSplitCount);
                    for (int i = 0; i < splitCount; i++)
                    {
                        var spawnPosition = transform.position +
                                            (Vector3)Random.insideUnitCircle * splittingInfo.spawnRadius;
                        factory.Spawn(splittingInfo.size, spawnPosition);
                    }
                }
            }

            if (_pooledObject == null)
            {
                Destroy(gameObject);
                return;
            }
            _pooledObject.ReturnToPool();
        }
    }
}