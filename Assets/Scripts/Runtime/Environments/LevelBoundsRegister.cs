using Starblast.Services;
using UnityEngine;

namespace Starblast.Environments
{
    [DefaultExecutionOrder(ExecutionOrder.TransientServiceRegister)]
    public class LevelBoundsRegister : ATransientServiceRegister<LevelBounds>
    {
        [SerializeField] private LevelBounds _service;
        
        public override LevelBounds Service => _service;
    }
}