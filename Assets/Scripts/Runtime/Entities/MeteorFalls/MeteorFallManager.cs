using Starblast.Pools;
using Starblast.Services;
using Starblast.Utils;
using UnityEngine;

namespace Starblast.Entities.MeteorFalls
{
    public class MeteorFallManager : MonoBehaviour, IInitializable
    {
        [Header("Settings")]
        [SerializeField] private Meteor[] _meteorPrefabs;
        [SerializeField] private float _radius;
        [SerializeField] private Vector2 _spawnCenter;
        
        [SerializeField] private IntRange _poolSize;
        
        
        [Header("Meteor Defaults")]
        [SerializeField] private FloatRange _numberOfMeteor;
        [SerializeField] private FloatRange _lifeTimeRange;
        [SerializeField] private FloatRange _speedRange;

        private IPoolManager poolManager;
        
        public bool IsInitialized { get; private set; }
        
        private void Start()
        {
            Initialize();
        }
        
        public void Initialize()
        {
            if (IsInitialized) return;
            poolManager = ServiceLocator.Main.Get<IPoolManager>();
            CreateMeteorPools();
            IsInitialized = true;
        }

        private void CreateMeteorPools()
        {
            foreach (var prefab in _meteorPrefabs)
            {
                poolManager.CreatePool(prefab.gameObject, _poolSize.Min, _poolSize.Max);
            }
        }

        public void Spawn(Vector2 position, Vector2 direction)
        {
            _spawnCenter = position;
            var numberOfMeteor = Random.Range(_numberOfMeteor.Min, _numberOfMeteor.Max);
            for (int i = 0; i < numberOfMeteor; i++)
            {
                var meteor = poolManager.GetPooledObject(_meteorPrefabs[Random.Range(0, _meteorPrefabs.Length)].gameObject);
                var spawnPosition = _spawnCenter + Random.insideUnitCircle * _radius;
                
                meteor.transform.position = spawnPosition;
                meteor.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
                meteor.GetComponent<Meteor>().Initialize(new Meteor.MeteorSettings
                {
                    Direction = direction,
                    Speed = Random.Range(_speedRange.Min, _speedRange.Max),
                    useLifeTime = true,
                    LifeTime = Random.Range(_lifeTimeRange.Min, _lifeTimeRange.Max)
                });
            }
        }
        
        public void Spawn(Vector3 position, Vector2 direction, FloatRange speed, FloatRange lifeTime, FloatRange quantity)
        {
            _spawnCenter = position;
            var numberOfMeteor = quantity.RandomValue();
            for (int i = 0; i < numberOfMeteor; i++)
            {
                var meteor = poolManager.GetPooledObject(_meteorPrefabs[Random.Range(0, _meteorPrefabs.Length)].gameObject);
                meteor.transform.parent = transform;
                var spawnPosition = _spawnCenter + Random.insideUnitCircle * _radius;

                meteor.transform.position = spawnPosition;
                meteor.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
                meteor.GetComponent<Meteor>().Initialize(new Meteor.MeteorSettings
                {
                    Direction = direction,
                    Speed = speed.RandomValue(),
                    useLifeTime = true,
                    LifeTime = speed.RandomValue()
                });
            }
        }
        
        
        #if UNITY_EDITOR
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_spawnCenter, _radius);
        }
        
        #endif

        
    }
}