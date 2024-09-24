using UnityEngine;

namespace Starblast
{
    [DefaultExecutionOrder(ExecutionOrder.TransientServiceRegister)]
    public abstract class ATransientServiceRegister<T> : MonoBehaviour where T : IInitializable
    {
        public virtual T Service { get; private set; }
        
        protected virtual void Awake()
        {
            ServiceLocator.Main.Register<T>(Service);
        }

        protected virtual void OnDestroy()
        {
            ServiceLocator.Main.Unregister<T>();
        }
    }
}