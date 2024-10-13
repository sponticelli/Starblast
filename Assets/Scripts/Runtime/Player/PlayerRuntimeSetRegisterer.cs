using Starblast.Services;
using UnityEngine;

namespace Starblast.Player
{
    [DefaultExecutionOrder(ExecutionOrder.TransientServiceRegister)]
    public class PlayerRuntimeSetRegisterer : ATransientServiceRegisterer<PlayerRuntimeSet>
    {
        [SerializeField] private PlayerRuntimeSet _service;
        
        public override PlayerRuntimeSet Service => _service;
    }
}