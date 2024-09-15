using Starblast.Inputs;
using UnityEngine;

namespace Starblast.Weapons
{
    public class WeaponsController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Weapon _weapon;
        
        private IActorInputHandler _inputHandler;
        
        public void Initialize(WeaponDataSO weaponData)
        {
            _weapon.Initialize(weaponData);
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
        
        public void Shoot()
        {
            if (_weapon == null) return;
            _weapon.TryShooting();
        }

        public void StopShooting()
        {
            if (_weapon == null) return;
            _weapon.StopShooting();
        }

        public void OnVelocityChange(Vector2 velocity)
        {
            _weapon.SetRelativeVelocity(velocity);
        }
    }
}