using Starblast.Services;
using UnityEngine;

namespace Starblast.Environments.Boundaries
{
    [DefaultExecutionOrder(ExecutionOrder.TransientServiceRegister)]
    public class BoundaryManagerRegisterer : ATransientServiceRegisterer<IBoundaryManager>
    {
        [SerializeField] private BoundaryManager _service;
        
        public override IBoundaryManager Service => _service;
    }
}