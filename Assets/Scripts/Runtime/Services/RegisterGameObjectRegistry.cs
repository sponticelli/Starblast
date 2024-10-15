using UnityEngine;

namespace Starblast.Services
{
    [DefaultExecutionOrder(ExecutionOrder.RegisterToRuntimeSet)]
    public class RegisterGameObjectRegistry : ATransientServiceRegisterer<GameObjectRegistry>
    {
        [SerializeField] private GameObjectRegistry _service;
        
        public override GameObjectRegistry Service => _service;
    }
}