using Starblast.Services;
using UnityEngine;

namespace Starblast.Entities.Asteroids
{
    public class Asteroid : MonoBehaviour, IDamageable
    {
        [Header("Data")]
        [SerializeField] private AsteroidData asteroidData;
        
        [Header("Components")]
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private AsteroidMovement movement;
        
        private float currentHealth;
        
        
        private void OnEnable()
        {
            rb.mass = asteroidData.mass;
            if (movement != null)
            {
                movement.Initialize(rb,  asteroidData.speed, asteroidData.rotationSpeed);
            }
            currentHealth = asteroidData.health;
        }

        public void TakeDamage(float damage)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
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

            Destroy(gameObject);
        }
    }
}