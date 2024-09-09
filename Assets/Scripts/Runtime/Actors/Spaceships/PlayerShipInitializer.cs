using Starblast.Actors.Movements;
using Starblast.Actors.Visuals;
using Starblast.Actors.Weapons;
using Starblast.Data.Spaceships;
using Starblast.Data.Spaceships.Bodies;
using Starblast.Data.Spaceships.Engines;
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
        [FormerlySerializedAs("_movementController")] [SerializeField] private SpaceshipMovementController spaceshipMovementController;
        [SerializeField] private WeaponsController _weaponsController;
        [SerializeField] private SpaceshipVisualController _spaceshipVisualController;
        
        [Header("Data")]
        [SerializeField] private SpaceshipDataSO _spaceshipData;
        
        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            
            var movementControllerContext = new SpaceshipMovementControllerContext(
                _spaceshipData.SpaceshipBodyData, 
                _spaceshipData.SpaceshipEngineData, 
                _actorController.GetRigidbody2D(), _actorInput);
            spaceshipMovementController.Initialize(movementControllerContext);

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