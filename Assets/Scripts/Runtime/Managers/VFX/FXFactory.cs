using Starblast.Pools;
using Starblast.Services;
using UnityEngine;

namespace Starblast.Environments
{
    [DefaultExecutionOrder(ExecutionOrder.Services)]
    public class FXFactory : MonoBehaviour, IInitializable
    {
        [SerializeField] private FXInfo[] fxInfos;
        
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
            InitializeFXPools();
            IsInitialized = true;
        }

        private void InitializeFXPools()
        {
            foreach (var info in fxInfos)
            {
                for (int i = 0; i < info.prefabs.Length; i++)
                {
                    poolManager.CreatePool(info.prefabs[i].gameObject, info.initialPoolSize, info.maxPoolSize);
                }
            }
        }

        private void OnDestroy()
        {
            IsInitialized = false;
            if (poolManager == null) return;
            foreach (var info in fxInfos)
            {
                for (int i = 0; i < info.prefabs.Length; i++)
                {
                    poolManager.DestroyPool(info.prefabs[i].gameObject);
                }
            }
        }

        public void Spawn(FXTypes type, Vector3 spawnPosition, Quaternion rotation)
        {
            FXInfo info = GetFXInfo(type);
            if (info == null) return;
            
            GameObject prefab = info.prefabs[Random.Range(0, info.prefabs.Length)].gameObject;
            GameObject fx = poolManager.GetPooledObject(prefab);
            fx.transform.position = spawnPosition;
            fx.transform.rotation = rotation;
            fx.SetActive(true);
        }
        
        private FXInfo GetFXInfo(FXTypes type)
        {
            foreach (var info in fxInfos)
            {
                if (info.type == type)
                {
                    return info;
                }
            }

            return null;
        }

        
    }
}