using Starblast.Services;
using UnityEngine;

namespace Starblast.Managers.Starfields
{
    [DefaultExecutionOrder(ExecutionOrder.TransientServiceRegister)]
    public class StarfieldManagerRegisterer : ATransientServiceRegisterer<StarfieldManager>
    {
        [SerializeField] private StarfieldManager _service;
        
        public override StarfieldManager Service => _service;
    }
}