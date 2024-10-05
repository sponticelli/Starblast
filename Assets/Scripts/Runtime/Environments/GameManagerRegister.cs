using Starblast.Services;
using UnityEngine;

namespace Starblast.Environments
{
    [DefaultExecutionOrder(ExecutionOrder.TransientServiceRegister)]
    public class GameManagerRegister : ATransientServiceRegister<GameManager>
    {
        [SerializeField] private GameManager _service;
        
        public override GameManager Service => _service;
    }
}