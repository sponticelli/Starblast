using System.Collections;
using Starblast.Entities.Asteroids;
using Starblast.Environments.Boundaries;
using Starblast.Services;
using UnityEngine;

namespace Starblast.Environments
{
    [DefaultExecutionOrder(ExecutionOrder.Services)]
    public class GameManager : MonoBehaviour, IInitializable
    {
        [Header("Game Settings")]
        [SerializeField] private int initialAsteroidCount = 20;
        
        
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
            var minRadius = levelBounds.InnerRadius / 2;
            var maxRadius = levelBounds.InnerRadius;
            
            var randomAngle = Random.Range(0, 360);
            var randomRadius = Random.Range(minRadius, maxRadius);
            
            var spawnPosition = new Vector3(
                Mathf.Cos(randomAngle) * randomRadius,
                Mathf.Sin(randomAngle) * randomRadius,
                0
            );
            
            
            return spawnPosition;
        }
    }
}