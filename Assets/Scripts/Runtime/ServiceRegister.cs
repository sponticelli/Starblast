using UnityEngine;

namespace Starblast
{
    public abstract class AServiceRegister<T> : MonoBehaviour 
    {
        protected abstract T GetService();
        
        protected virtual void Awake()
        {
            if (ServiceLocator.Main != null)
            {
                Destroy(gameObject);
                return;
            }

            Register();
        }
        
        protected void OnDestroy()
        {
            Unregister();
        }

        private void Unregister()
        {
            BeforeUnregister();
            ServiceLocator.Main.Unregister<T>();
        }

        private void Register()
        {
            BeforeRegister();
            ServiceLocator.Main.Register<T>(GetService());
            AfterRegister();
        }

        protected virtual void BeforeRegister() { }
        protected virtual void AfterRegister() { }
        protected virtual void BeforeUnregister() { }
    }

}