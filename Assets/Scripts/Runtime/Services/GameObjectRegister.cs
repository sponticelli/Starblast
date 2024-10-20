using UnityEngine;

namespace Starblast.Services
{
    [DefaultExecutionOrder(ExecutionOrder.RegisterToRuntimeSet)]
    public class GameObjectRegister<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] private T _object;

        private void Start()
        {
            GameObjectRegistry registry = ServiceLocator.Main.Get<GameObjectRegistry>();
            registry.Register(_object);
        }
    }
}