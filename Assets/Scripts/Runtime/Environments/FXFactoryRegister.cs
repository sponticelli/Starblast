using Starblast.Services;
using UnityEngine;

namespace Starblast.Environments
{
    [DefaultExecutionOrder(ExecutionOrder.TransientServiceRegister)]
    public class FXFactoryRegister : ATransientServiceRegister<FXFactory>
    {
        [SerializeField] private FXFactory _service;
        
        public override FXFactory Service => _service;
    }
}