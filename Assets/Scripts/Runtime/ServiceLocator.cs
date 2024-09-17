using System;
using System.Collections.Generic;
using Starblast.Audio;
using Starblast.Pools;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Starblast
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    [DefaultExecutionOrder(ExecutionOrder.ServiceLocator)]
    public class ServiceLocator : MonoBehaviour, IInitializable
    {
        public static ServiceLocator Main { get; private set; }
        
        public bool IsInitialized { get; private set; }
        
        // Services Shortcuts
        public IPoolManager PoolManager { get; private set; }
        public IMusicManager MusicManager { get; private set; }
        
        /*
        public IMusicManager AudioManager { get; private set; }
        public IFadeToBlack FadeToBlack { get; private set; }
        public ISceneLoader SceneLoader { get; private set; }
        public ICommandFactory CommandFactory { get; private set; }
        */
        
        
        private Dictionary<Type, object> _services = new Dictionary<Type, object>();
        
        static ServiceLocator()
        {
            Main = null;
        }
        
        private void Awake()
        {
            if (Main != null)
            {
                Destroy(gameObject);
                return;
            }

            Main = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        
        public void Initialize()
        {
            if (IsInitialized) return;
            _services = new Dictionary<Type, object>();
            LoadServices();
            InitializeServices();
            IsInitialized = true;
        }
        
        private void LoadServices()
        {
            PoolManager = GetComponentInChildren<IPoolManager>();
            MusicManager = GetComponentInChildren<IMusicManager>();
            Register<IPoolManager>(PoolManager);
            Register<IMusicManager>(MusicManager);
            
            /*
            AudioManager = GetComponentInChildren<IMusicManager>();
            
            SoundManager = GetComponentInChildren<ISoundManager>();
            FadeToBlack = GetComponentInChildren<IFadeToBlack>();
            SceneLoader = GetComponentInChildren<ISceneLoader>();
            CommandFactory = GetComponentInChildren<ICommandFactory>();
            */
        }
        
        private void InitializeServices()
        {
            PoolManager.Initialize();
            MusicManager.Initialize();
            /*
            AudioManager.Initialize();
            SoundManager.Initialize();
            CommandFactory.Initialize();
            FadeToBlack.Initialize();
            SceneLoader.Initialize();
            */
        }


        public void Register<T>(T service)
        {
            if (_services.ContainsKey(typeof(T)))
            {
                Debug.LogWarning($"Service of type {typeof(T).Name} is already registered.");
                return;
            }
            _services.Add(typeof(T), service);
        }

        public void Unregister<T>()
        {
            if (!_services.ContainsKey(typeof(T)))
            {
                Debug.LogWarning($"Service of type {typeof(T).Name} is not registered.");
                return;
            }
            _services.Remove(typeof(T));
        }
    }
}