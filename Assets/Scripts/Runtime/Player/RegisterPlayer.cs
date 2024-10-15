using Starblast.Services;
using UnityEngine;

namespace Starblast.Player
{
    [DefaultExecutionOrder(ExecutionOrder.RegisterToRuntimeSet)]
    public class RegisterPlayer : MonoBehaviour
    {
        [SerializeField] private ShipController _player;

        private void Start()
        {
            GameObjectRegistry registry = ServiceLocator.Main.Get<GameObjectRegistry>();
            registry.Register(_player);
        }
    }
}