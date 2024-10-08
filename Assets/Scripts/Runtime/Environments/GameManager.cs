using System.Collections;
using Starblast.Entities.Asteroids;
using Starblast.Services;
using UnityEngine;

namespace Starblast.Environments
{
    [DefaultExecutionOrder(ExecutionOrder.Services)]
    public class GameManager : MonoBehaviour, IInitializable
    {
        [Header("Game Settings")]
        [SerializeField] private int initialAsteroidCount = 20;
        [SerializeField] private float minRadius = 2f;
        
        
        public bool IsInitialized { get; private set;  }
        
        private LevelBounds levelBounds;
        private AsteroidFactory asteroidFactory;
        
        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            Initialize();
        }
        public void Initialize()
        {
            if (IsInitialized) return;
            levelBounds = ServiceLocator.Main.Get<LevelBounds>();
            asteroidFactory = ServiceLocator.Main.Get<AsteroidFactory>();
            SpawnInitialAsteroids();
            IsInitialized = true;
        }
        
        private void SpawnInitialAsteroids()
        {
            for (int i = 0; i < initialAsteroidCount; i++)
            {
                var spawnPosition = GetRandomSpawnPosition();
                asteroidFactory.Spawn(AsteroidSize.Big, spawnPosition);
            }
        }
        
        private Vector3 GetRandomSpawnPosition()
        {
            var spawnPosition = new Vector3(Random.Range(levelBounds.Mins.x, levelBounds.Maxs.x), 
                Random.Range(levelBounds.Mins.y, levelBounds.Maxs.y), 0);
            var direction = spawnPosition;
            direction.Normalize();
            spawnPosition += direction * minRadius;
            return spawnPosition;
        }
    }
}