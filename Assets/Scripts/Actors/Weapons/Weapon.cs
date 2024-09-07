using System.Collections;
using Starblast.Data;
using UnityEngine;
using UnityEngine.Events;

namespace Starblast.Actors.Weapons
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private Transform _muzzle;
        [SerializeField] private int _ammo = 10;
        
        [field: SerializeField]
        public UnityEvent OnShoot { get; set; }
        
        [field: SerializeField]
        public UnityEvent OnShootNoAmmo { get; set; }
        
        public int Ammo
        {
            get => _ammo;
            set
            {
                _ammo = Mathf.Clamp(value, 0, _weaponDataProvider.WeaponData.AmmoCapacity);
            }
        }
        
        public bool AmmoEmpty => _ammo <= 0;
        public bool AmmoFull => _ammo >= _weaponDataProvider.WeaponData.AmmoCapacity;
        
        private bool _isShooting;
        private bool _reloadCoroutine = false;
        
        private IVelocityProvider _velocityProvider;
        private IWeaponDataProvider _weaponDataProvider;

        private void Start()
        {
            Ammo = _weaponDataProvider.WeaponData.AmmoCapacity;
        }
        
        private void Update()
        {
            UseWeapon();
        }
        public void TryShooting()
        {
            _isShooting = true;
        }
        
        public void StopShooting()
        {
            _isShooting = false;
        }
        
        public void Reload(int ammo)
        {
            Ammo += ammo;
        }
        
        private void UseWeapon()
        {
            if (_isShooting && !_reloadCoroutine)
            {
                if (!AmmoEmpty)
                {
                    Ammo--;
                    OnShoot?.Invoke();
                    for(var i = 0; i < _weaponDataProvider.WeaponData.GetBulletCountToSpawn; i++)
                    {
                        ShootBullet();
                    }
                }
                else
                {
                    _isShooting = false;
                    OnShootNoAmmo?.Invoke();
                    return;
                }
                FinishShooting();
            }
        }

        private void ShootBullet()
        {
            var bullet = Instantiate(_weaponDataProvider.WeaponData.BulletPrefab, _muzzle.position, _muzzle.rotation);
            var direction = _muzzle.up;
            // Add spread angle to the direction
            direction = Quaternion.Euler(0, 0, Random.Range(-_weaponDataProvider.WeaponData.SpreadAngle, _weaponDataProvider.WeaponData.SpreadAngle)) * direction;
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
            _reloadCoroutine = true;
            yield return new WaitForSeconds(_weaponDataProvider.WeaponData.WeaponDelay);
            _reloadCoroutine = false;
        }
        
        private void OnDrawGizmos()
        {
            if (_muzzle == null) return;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_muzzle.transform.position, 0.1f);
        }

        public void Initialize(IWeaponDataProvider weaponDataProvider, IVelocityProvider velocityProvider)
        {
            _velocityProvider = velocityProvider;
            _weaponDataProvider = weaponDataProvider;
            
            _muzzle.localPosition = _weaponDataProvider.WeaponData.MuzzleOffset;
        }

    }
}