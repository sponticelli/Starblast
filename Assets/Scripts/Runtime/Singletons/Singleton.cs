using UnityEngine;

namespace Starblast.Singletons
{
    public class MonoSingleton<T> : MonoBehaviour where T : Component
    {
        [SerializeField] private bool _persistent = true;
        
        
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = FindObjectOfType<T>();
                if (_instance != null) return _instance;
                GameObject obj = new GameObject
                {
                    name = typeof(T).Name
                };
                _instance = obj.AddComponent<T>();
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                if (_persistent) DontDestroyOnLoad(this.gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
}