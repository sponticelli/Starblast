using Starblast.Actors.Movements;
using Starblast.Actors.Weapons;
using Starblast.Data;
using Starblast.Inputs;
using UnityEngine;
using UnityEngine.Serialization;

namespace Starblast.Actors
{
    [DefaultExecutionOrder(Consts.OrderInitializer)]
    public class PlayerShipInitializer : MonoBehaviour
    {
        [Header("Component References")]
        [SerializeField] private ActorController _actorController;
        [SerializeField] private PlayerInputHandler _actorInput;
        [SerializeField] private MovementController _movementController;
        [SerializeField] private WeaponsController _weaponsController;

        [Header("Data")]
        [SerializeField] private BodyDataProvider _bodyDataProvider;
        [SerializeField] private EngineDataProvider _engineDataProvider;
        [SerializeField] private WeaponDataProvider _weaponDataProvider;
        
        
        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            var movementControllerContext = new MovementControllerContext(_bodyDataProvider, _engineDataProvider, 
                _actorController, _actorInput);
            _movementController.Initialize(movementControllerContext);
            
            var weaponControllerContext = new WeaponsControllerContext(_movementController, _actorInput, _weaponDataProvider);
            _weaponsController.Initialize(weaponControllerContext);
        }
    }
}