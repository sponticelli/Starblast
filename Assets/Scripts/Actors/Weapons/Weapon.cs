using System;
using System.Collections;
using System.Collections.Generic;
using Starblast.Data;
using Starblast.Data.Spaceships.Weapons;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;
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
        private IWeaponDataProvider _weaponDataProvider = null!;

        private int _ammo;
        private bool _isShooting;
        private bool _isReloading;

        public int Ammo
        {
            get => _ammo;
            set => _ammo = Mathf.Clamp(value, 0, _weaponDataProvider?.Data.AmmoCapacity ?? 0);
        }

        public bool AmmoEmpty => Ammo <= 0;
        public bool AmmoFull => Ammo >= _weaponDataProvider?.Data.AmmoCapacity;

        private void Start()
        {
            if (_weaponDataProvider == null || _velocityProvider == null)
            {
                Debug.LogError("Weapon not properly initialized. Please call Initialize before use.");
                enabled = false;
                return;
            }

            Ammo = _weaponDataProvider.Data.AmmoCapacity;
        }

        private void Update() => UseWeapon();

        /// <summary>
        /// Initializes the weapon with necessary dependencies.
        /// </summary>
        public void Initialize(IWeaponDataProvider weaponDataProvider, IVelocityProvider velocityProvider)
        {
            _weaponDataProvider = weaponDataProvider ?? throw new ArgumentNullException(nameof(weaponDataProvider));
            _velocityProvider = velocityProvider ?? throw new ArgumentNullException(nameof(velocityProvider));
            
            if (_bulletPoolManager == null)
            {
                Debug.LogError($"BulletPoolManager not assigned to the Weapon on GameObject: {gameObject.name}");
                enabled = false;
                return;
            }
            
            _bulletPoolManager.Initialize(_weaponDataProvider);

            if (_muzzle == null)
            {
                Debug.LogError($"Muzzle transform not assigned to the Weapon on GameObject: {gameObject.name}");
                enabled = false;
                return;
            }

            _muzzle.localPosition = _weaponDataProvider.Data.MuzzleOffset;
        }
        

        public void TryShooting() => _isShooting = true;

        public void StopShooting() => _isShooting = false;

        public void Reload(int ammo) => Ammo += ammo;

        private void UseWeapon()
        {
            if (!_isShooting || _isReloading || AmmoEmpty) return;

            Ammo--;
            OnShoot?.Invoke();

            for (int i = 0; i < _weaponDataProvider.Data.GetBulletCountToSpawn; i++)
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

            var direction = Quaternion.Euler(0, 0, Random.Range(-_weaponDataProvider.Data.SpreadAngle, _weaponDataProvider.Data.SpreadAngle)) * _muzzle.up;
            bullet.Shoot(direction, _velocityProvider.GetVelocity());
        }

        private void FinishShooting()
        {
            StartCoroutine(DelayNextShootCoroutine());
            if (!_weaponDataProvider.Data.AutomaticFire)
            {
                _isShooting = false;
            }
        }

        private IEnumerator DelayNextShootCoroutine()
        {
            _isReloading = true;
            yield return new WaitForSeconds(_weaponDataProvider.Data.WeaponDelay);
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