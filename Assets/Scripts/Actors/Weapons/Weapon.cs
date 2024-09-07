using System.Collections;
using System.Collections.Generic;
using Starblast.Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

namespace Starblast.Actors.Weapons
{
    /// <summary>
    /// Represents a weapon that can be attached to a spaceship.
    /// </summary>
    public class Weapon : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _muzzle = null!;

        [Header("Events")]
        [SerializeField] private UnityEvent OnShoot = null!;
        [SerializeField] private UnityEvent OnShootNoAmmo = null!;

        private IObjectPool<Bullet> _bulletPool = null!;
        private IVelocityProvider _velocityProvider = null!;
        private IWeaponDataProvider _weaponDataProvider = null!;

        private int _ammo;
        private bool _isShooting;
        private bool _isReloading;

        public int Ammo
        {
            get => _ammo;
            set => _ammo = Mathf.Clamp(value, 0, _weaponDataProvider?.WeaponData.AmmoCapacity ?? 0);
        }

        public bool AmmoEmpty => Ammo <= 0;
        public bool AmmoFull => Ammo >= _weaponDataProvider?.WeaponData.AmmoCapacity;

        private void Start()
        {
            if (_weaponDataProvider == null || _velocityProvider == null)
            {
                Debug.LogError("Weapon not properly initialized. Please call Initialize before use.");
                enabled = false;
                return;
            }

            Ammo = _weaponDataProvider.WeaponData.AmmoCapacity;
            InitializeBulletPool();
        }

        private void Update() => UseWeapon();

        /// <summary>
        /// Initializes the weapon with necessary dependencies.
        /// </summary>
        public void Initialize(IWeaponDataProvider weaponDataProvider, IVelocityProvider velocityProvider)
        {
            _weaponDataProvider = weaponDataProvider ?? throw new System.ArgumentNullException(nameof(weaponDataProvider));
            _velocityProvider = velocityProvider ?? throw new System.ArgumentNullException(nameof(velocityProvider));

            if (_muzzle != null)
            {
                _muzzle.localPosition = _weaponDataProvider.WeaponData.MuzzleOffset;
            }
            else
            {
                Debug.LogError("Muzzle transform not assigned to the Weapon.");
            }
        }

        private void InitializeBulletPool()
        {
            _bulletPool = new ObjectPool<Bullet>(
                createFunc: CreateBullet,
                actionOnGet: bullet => bullet.gameObject.SetActive(true),
                actionOnRelease: bullet => bullet.gameObject.SetActive(false),
                actionOnDestroy: bullet => Destroy(bullet.gameObject),
                collectionCheck: false,
                defaultCapacity: 20,
                maxSize: 100
            );
        }

        private Bullet CreateBullet()
        {
            var bullet = Instantiate(_weaponDataProvider.WeaponData.BulletPrefab, _muzzle.position, _muzzle.rotation);
            bullet.Initialize(_bulletPool);
            return bullet;
        }

        public void TryShooting() => _isShooting = true;

        public void StopShooting() => _isShooting = false;

        public void Reload(int ammo) => Ammo += ammo;

        private void UseWeapon()
        {
            if (!_isShooting || _isReloading || AmmoEmpty) return;

            Ammo--;
            OnShoot?.Invoke();

            for (int i = 0; i < _weaponDataProvider.WeaponData.GetBulletCountToSpawn; i++)
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
            var bullet = _bulletPool.Get();
            bullet.transform.SetPositionAndRotation(_muzzle.position, _muzzle.rotation);

            var direction = Quaternion.Euler(0, 0, Random.Range(-_weaponDataProvider.WeaponData.SpreadAngle, _weaponDataProvider.WeaponData.SpreadAngle)) * _muzzle.up;
            bullet.Shoot(direction, _velocityProvider.GetVelocity());
        }

        private void FinishShooting()
        {
            StartCoroutine(DelayNextShootCoroutine());
            if (!_weaponDataProvider.WeaponData.AutomaticFire)
            {
                _isShooting = false;
            }
        }

        private IEnumerator DelayNextShootCoroutine()
        {
            _isReloading = true;
            yield return new WaitForSeconds(_weaponDataProvider.WeaponData.WeaponDelay);
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