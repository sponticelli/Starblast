using System;
using System.Collections;
using Starblast.Pools;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Starblast.Weapons
{
    /// <summary>
    /// Represents a weapon that can be attached to a spaceship.
    /// </summary>
    public class Weapon : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _muzzle = null!;

        [Header("Settings")]
        [SerializeField] private WeaponDataSO _weaponData = null!;
        
        [Header("Events")]
        [SerializeField] private UnityEvent OnShoot = null!;
        [SerializeField] private UnityEvent OnShootNoAmmo = null!;
        
        

        private int _ammo;
        private bool _isShooting;
        private bool _isReloading;
        private Vector2 _relativeVelocity;
        
        private IPoolManager _poolManager = null!;

        public int Ammo
        {
            get => _ammo;
            set => _ammo = Mathf.Clamp(value, 0, _weaponData?.AmmoCapacity ?? 0);
        }

        public bool AmmoEmpty => Ammo <= 0;
        public bool AmmoFull => Ammo >= _weaponData?.AmmoCapacity;

        private void Start()
        {
            if (_weaponData == null)
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
        public void Initialize(WeaponDataSO weaponData)
        {
            _weaponData = weaponData ? weaponData : throw new ArgumentNullException(nameof(weaponData));
            _poolManager = ServiceLocator.Main.PoolManager;
            if (_muzzle == null)
            {
                Debug.LogError($"Muzzle transform not assigned to the Weapon on GameObject: {gameObject.name}");
                enabled = false;
                return;
            }
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
            var bulletGO = _poolManager.GetPooledObject(_weaponData.BulletPrefab);
            var bullet = bulletGO.GetComponent<Bullet>();
            bullet.transform.SetPositionAndRotation(_muzzle.position, _muzzle.rotation);

            var direction = Quaternion.Euler(0, 0, 
                Random.Range(-_weaponData.SpreadAngle, _weaponData.SpreadAngle)) * _muzzle.up;
            bullet.Shoot(direction, _relativeVelocity);
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

        public void SetRelativeVelocity(Vector2 velocity)
        {
            _relativeVelocity = velocity;
        }
    }
}