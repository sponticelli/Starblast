using UnityEngine;

namespace Starblast.Services
{
    [DefaultExecutionOrder(ExecutionOrder.TransientServiceRegister)]
    public abstract class ATransientServiceRegisterer<T> : MonoBehaviour where T : IInitializable
    {
        public virtual T Service { get; private set; }
        
        protected virtual void Awake()
        {
            ServiceLocator.Main.Register<T>(Service);
            Debug.Log($"Registered {typeof(T).Name}");
        }

        protected virtual void OnDestroy()
        {
            if (ServiceLocator.Main == null) return;
            ServiceLocator.Main.Unregister<T>();
            Debug.Log($"Unregistered {typeof(T).Name}");
        }
    }
}