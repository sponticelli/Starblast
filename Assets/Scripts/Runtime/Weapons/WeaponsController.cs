using Starblast.Inputs;
using UnityEngine;

namespace Starblast.Weapons
{
    public class WeaponsController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Weapon[] _weapons;
        
        private IActorInputHandler _inputHandler;
        
        public void Initialize(WeaponDataSO weaponData)
        {
            foreach (var weapon in _weapons)
            {
                weapon.Initialize(weaponData);
            }
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
            foreach (var weapon in _weapons)
            {
                weapon.TryShooting();
            }
        }

        public void StopShooting()
        {
            foreach (var weapon in _weapons)
            {
                weapon.StopShooting();
            }
        }

        public void OnVelocityChange(Vector2 velocity)
        {
            foreach (var weapon in _weapons)
            {
                weapon.SetRelativeVelocity(velocity);
            }
        }
    }
}