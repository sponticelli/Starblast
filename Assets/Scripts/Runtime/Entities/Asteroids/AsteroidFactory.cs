using System.Linq;
using Starblast.Pools;
using Starblast.Services;
using UnityEngine;

namespace Starblast.Entities.Asteroids
{
    [DefaultExecutionOrder(ExecutionOrder.Services)]
    public class AsteroidFactory : MonoBehaviour, IInitializable
    {
        [SerializeField] private AsteroidInfo[] asteroidInfo;
        
        private IPoolManager poolManager;
        
        
        private void Start()
        {
            Initialize();
        }
        
        public bool IsInitialized { get; private set; }
        public void Initialize()
        {
            if (IsInitialized) return;
            poolManager = ServiceLocator.Main.Get<IPoolManager>();
            CreateAsteroidPools();
            IsInitialized = true;
        }

        private void CreateAsteroidPools()
        {
            foreach (var info in asteroidInfo)
            {
                for (int i = 0; i < info.prefabs.Length; i++)
                {
                    poolManager.CreatePool(info.prefabs[i].gameObject, info.initialPoolSize, info.maxPoolSize);
                }
            }
        }

        private void OnDestroy()
        {
            if (poolManager == null) return;
            foreach (var info in asteroidInfo)
            {
                for (int i = 0; i < info.prefabs.Length; i++)
                {
                    poolManager.DestroyPool(info.prefabs[i].gameObject);
                }
            }
        }

        public void Spawn(AsteroidSize size, Vector3 spawnPosition)
        {
            var info = GetAsteroidInfo(size);
            if (info == null) return;
            
            Asteroid prefab = info.prefabs[UnityEngine.Random.Range(0, info.prefabs.Length)];
            var asteroid = poolManager.GetPooledObject(prefab.gameObject);
            asteroid.transform.parent = transform;
            asteroid.transform.position = spawnPosition;
            asteroid.SetActive(true);
        }

        private AsteroidInfo GetAsteroidInfo(AsteroidSize size)
        {
            return asteroidInfo.FirstOrDefault(info => info.size == size);
        }
    }
}