using System;
using System.Collections;
using Starblast.Data.Spaceships.Weapons;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Starblast.Actors.Weapons
{
    /// <summary>
    /// Represents a weapon that can be attached to a spaceship.
    /// </summary>
    public class Weapon : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _muzzle = null!;
        [SerializeField] private BulletPoolManager _bulletPoolManager = null!;

        [Header("Events")]
        [SerializeField] private UnityEvent OnShoot = null!;
        [SerializeField] private UnityEvent OnShootNoAmmo = null!;
        
        private IVelocityProvider _velocityProvider = null!;
        private IWeaponData _weaponData = null!;

        private int _ammo;
        private bool _isShooting;
        private bool _isReloading;

        public int Ammo
        {
            get => _ammo;
            set => _ammo = Mathf.Clamp(value, 0, _weaponData?.AmmoCapacity ?? 0);
        }

        public bool AmmoEmpty => Ammo <= 0;
        public bool AmmoFull => Ammo >= _weaponData?.AmmoCapacity;

        private void Start()
        {
            if (_weaponData == null || _velocityProvider == null)
            {
                Debug.LogError("Weapon not properly initialized. Please call Initialize before use.");
                enabled = false;
                return;
            }

            Ammo = _weaponData.AmmoCapacity;
        }

        private void Update() => UseWeapon();

        /// <summary>
        /// Initializes the weapon with necessary dependencies.
        /// </summary>
        public void Initialize(IWeaponData weaponData, IVelocityProvider velocityProvider)
        {
            _weaponData = weaponData ?? throw new ArgumentNullException(nameof(weaponData));
            _velocityProvider = velocityProvider ?? throw new ArgumentNullException(nameof(velocityProvider));
            
            if (_bulletPoolManager == null)
            {
                Debug.LogError($"BulletPoolManager not assigned to the Weapon on GameObject: {gameObject.name}");
                enabled = false;
                return;
            }
            
            _bulletPoolManager.Initialize(_weaponData);

            if (_muzzle == null)
            {
                Debug.LogError($"Muzzle transform not assigned to the Weapon on GameObject: {gameObject.name}");
                enabled = false;
                return;
            }

            _muzzle.localPosition = _weaponData.MuzzleOffset;
        }
        

        public void TryShooting() => _isShooting = true;

        public void StopShooting() => _isShooting = false;

        public void Reload(int ammo) => Ammo += ammo;

        private void UseWeapon()
        {
            if (!_isShooting || _isReloading || AmmoEmpty) return;

            Ammo--;
            OnShoot?.Invoke();

            for (int i = 0; i < _weaponData.GetBulletCountToSpawn; i++)
            {
                ShootBullet();
            }

            if (AmmoEmpty)
            {
                _isShooting = false;
                OnShootNoAmmo?.Invoke();
            }
            else
            {
                FinishShooting();
            }
        }

        private void ShootBullet()
        {
            var bullet = _bulletPoolManager.GetBullet();
            bullet.transform.SetPositionAndRotation(_muzzle.position, _muzzle.rotation);

            var direction = Quaternion.Euler(0, 0, 
                Random.Range(-_weaponData.SpreadAngle, _weaponData.SpreadAngle)) * _muzzle.up;
            bullet.Shoot(direction, _velocityProvider.GetVelocity());
        }

        private void FinishShooting()
        {
            StartCoroutine(DelayNextShootCoroutine());
            if (!_weaponData.AutomaticFire)
            {
                _isShooting = false;
            }
        }

        private IEnumerator DelayNextShootCoroutine()
        {
            _isReloading = true;
            yield return new WaitForSeconds(_weaponData.WeaponDelay);
            _isReloading = false;
        }

        private void OnDrawGizmos()
        {
            if (_muzzle == null) return;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_muzzle.position, 0.1f);
        }
    }
}