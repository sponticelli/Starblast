using Starblast.Inputs;
using UnityEngine;

namespace Starblast.Actors.Weapons
{
    public class WeaponsController : MonoBehaviour, IWeaponsController
    {
        [Header("References")]
        [SerializeField] private Weapon _weapon;
        
        private IWeaponsControllerContext _context;
        private IActorInputHandler _inputHandler;
        
        public void Initialize(IWeaponsControllerContext context)
        {
            _context = context;
            _weapon.Initialize(context.WeaponData, context.VelocityProvider);
            StopListeningInput();
            _inputHandler = context.ActorInputHandler;
            StartListeningInput();
        }

        private void OnEnable()
        {
            StartListeningInput();
        }
        
        private void OnDisable()
        {
            StopListeningInput();
        }

        private void StartListeningInput()
        {
            if (_inputHandler == null) return;
            _inputHandler.OnFirePressed += Shoot;
            _inputHandler.OnFireReleased += StopShooting;
        }
        
        private void StopListeningInput()
        {
            if (_inputHandler == null) return;
            _inputHandler.OnFirePressed -= Shoot;
            _inputHandler.OnFireReleased -= StopShooting;
        }
        
        private void Shoot()
        {
            if (_weapon == null) return;
            _weapon.TryShooting();
        }

        private void StopShooting()
        {
            if (_weapon == null) return;
            _weapon.StopShooting();
        }
    }
}