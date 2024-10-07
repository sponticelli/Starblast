using Starblast.Environments;
using Starblast.Inputs;
using Starblast.Player.Propulsion;
using Starblast.Player.Visuals;
using Starblast.Services;
using Starblast.Weapons;
using UnityEngine;

namespace Starblast.Player
{
    [AddComponentMenu("Starblast/Player/Ship Controller")]
    public class ShipController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private PlayerShipInputHandler _inputHandler;
        
        [Header("Propulsion")]
        [SerializeField] private ShipPropulsion _propulsion;
        [SerializeField] private PropulsionDataSO _propulsionData;

        [Header("Visuals")]
        [SerializeField] private ShipVisuals _visuals;
        [SerializeField] private ShipVisualDataSO _visualData;
        
        [Header("Weapons")]
        [SerializeField] private WeaponsController _weaponController;
        [SerializeField] private WeaponDataSO _weaponData;
        
        public Rigidbody2D Rigidbody2D => _rigidbody2D;
        
        public LevelBounds _levelBounds;
        
        private void Awake()
        {
            _levelBounds = ServiceLocator.Main.Get<LevelBounds>();
            
            _propulsion.Initialize(_rigidbody2D, _propulsionData);
            _visuals.Initialize(_visualData);
            _weaponController.Initialize(_weaponData);
        }
        
        private void OnEnable()
        {
            ListenToInput();
        }
        
        private void OnDisable()
        {
            StopListeningToInput();
        }


        private void ListenToInput()
        {
            if (_inputHandler == null) return;
            _inputHandler.OnRotate += _propulsion.OnRotate;
            _inputHandler.OnThrust += _propulsion.OnThrust;
            _inputHandler.OnRotate += _visuals.OnRotate;
            _inputHandler.OnFirePressed += _weaponController.Shoot;
            _inputHandler.OnFireReleased += _weaponController.StopShooting;
            _propulsion.OnVelocityChange += _weaponController.OnVelocityChange;
        }

        private void StopListeningToInput()
        {
            if (_inputHandler == null) return;
            _inputHandler.OnRotate -= _propulsion.OnRotate;
            _inputHandler.OnThrust -= _propulsion.OnThrust;
            _inputHandler.OnRotate -= _visuals.OnRotate;
            _inputHandler.OnFirePressed -= _weaponController.Shoot;
            _inputHandler.OnFireReleased -= _weaponController.StopShooting;
            _propulsion.OnVelocityChange -= _weaponController.OnVelocityChange;
        }
    }
}