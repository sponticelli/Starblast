using Starblast.Services;
using UnityEngine;

namespace Starblast.Entities.MeteorFalls
{
    [DefaultExecutionOrder(ExecutionOrder.TransientServiceRegister)]
    public class MeteorFallManagerRegisterer : ATransientServiceRegisterer<MeteorFallManager>
    {
        [SerializeField] private MeteorFallManager _service;
        
        public override MeteorFallManager Service => _service;
    }
}