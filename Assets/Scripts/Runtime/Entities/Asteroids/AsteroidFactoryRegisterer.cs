using Starblast.Services;
using UnityEngine;

namespace Starblast.Entities.Asteroids
{
    [DefaultExecutionOrder(ExecutionOrder.TransientServiceRegister)]
    public class AsteroidFactoryRegisterer : ATransientServiceRegisterer<AsteroidFactory>
    {
        [SerializeField] private AsteroidFactory _service;
        
        public override AsteroidFactory Service => _service;
    }
}