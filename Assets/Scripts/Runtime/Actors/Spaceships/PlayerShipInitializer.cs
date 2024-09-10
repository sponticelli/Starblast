using Starblast.Actors.Movements;
using Starblast.Actors.Visuals;
using Starblast.Actors.Weapons;
using Starblast.Data.Spaceships;
using Starblast.Data.Spaceships.Visuals;
using Starblast.Data.Spaceships.Weapons;
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
        [SerializeField] private SpaceshipMovementController spaceshipMovementController;
        [SerializeField] private WeaponsController _weaponsController;
        [SerializeField] private SpaceshipVisualController _spaceshipVisualController;
        
        [Header("Data")]
        [SerializeField] private AMovementComponentFactory _movementComponentFactory;
        [SerializeField] private SpaceshipDataSO _spaceshipData;
        
        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            var movementControllerContext = new SpaceshipMovementControllerContext(
                _spaceshipData.BodyData, 
                _spaceshipData.PropulsorData, 
                _actorController.GetRigidbody2D(), _actorInput);
            spaceshipMovementController.Initialize(movementControllerContext, 
                _movementComponentFactory.CreatePhysicsApplier(_actorController.GetRigidbody2D()), 
                _movementComponentFactory.CreateMovementCalculator(_spaceshipData.BodyData, _spaceshipData.PropulsorData)
                );

            var weaponControllerContext = new WeaponsControllerContext(
                spaceshipMovementController, _actorInput, 
                _spaceshipData.WeaponData);
            _weaponsController.Initialize(weaponControllerContext);

            var visualControllerContext = new SpaceshipVisualControllerContext(
                _actorInput, 
                _spaceshipData.VisualData);
            _spaceshipVisualController.Initialize(visualControllerContext);
            
        }
    }
}