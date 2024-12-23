using Starblast.Services;
using UnityEngine;

namespace Starblast.Managers
{
    [DefaultExecutionOrder(ExecutionOrder.TransientServiceRegister)]
    public class GameManagerRegisterer : ATransientServiceRegisterer<GameManager>
    {
        [SerializeField] private GameManager _service;
        
        public override GameManager Service => _service;
    }
}