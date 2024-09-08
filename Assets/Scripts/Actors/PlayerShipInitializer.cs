using Starblast.Actors.Movements;
using Starblast.Actors.Visuals;
using Starblast.Actors.Weapons;
using Starblast.Data;
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
        [SerializeField] private MovementController _movementController;
        [SerializeField] private WeaponsController _weaponsController;
        [SerializeField] private SpaceshipVisualController _spaceshipVisualController;
        
        [Header("Data")]
        [SerializeField] private MBSpaceshipDataProvider _spaceshipDataProvider;
        
        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            var spaceshipData = _spaceshipDataProvider.Data;
            
            var movementControllerContext = new MovementControllerContext(
                new SpaceshipBodyDataProvider(spaceshipData.SpaceshipBodyData), 
                new SpaceshipEngineDataProvider(spaceshipData.SpaceshipEngineData), 
                _actorController, _actorInput);
            _movementController.Initialize(movementControllerContext);

            var weaponControllerContext = new WeaponsControllerContext(
                _movementController, _actorInput, 
                new WeaponDataProvider(spaceshipData.WeaponData));
            _weaponsController.Initialize(weaponControllerContext);

            var visualControllerContext = new SpaceshipVisualControllerContext(
                _actorInput, 
                new SpaceshipVisualDataProvider(spaceshipData.VisualData ));
            _spaceshipVisualController.Initialize(visualControllerContext);
            
        }
    }
}