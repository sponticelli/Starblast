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
        
        private IBoundaryManager boundaryManager;
        private AsteroidFactory asteroidFactory;
        
        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            Initialize();
        }
        public void Initialize()
        {
            if (IsInitialized) return;
            boundaryManager = ServiceLocator.Main.Get<IBoundaryManager>();
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
            var minRadius = boundaryManager.GetZoneRadius(ZoneType.SafeZone) / 2;
            var maxRadius = boundaryManager.GetZoneRadius(ZoneType.SafeZone);
            
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