using UnityEngine;

namespace Starblast.Actors.Asteroids
{
    [DefaultExecutionOrder(Consts.OrderInitializer)]
    public class AsteroidInitializer : MonoBehaviour
    {
        [Header("Component References")]
        [SerializeField] private ActorController _actorController;
        
    }
}