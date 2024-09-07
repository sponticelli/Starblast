using Starblast.Data;
using UnityEngine;
using UnityEngine.Pool;

namespace Starblast.Actors.Weapons
{
    public class BulletPoolManager : MonoBehaviour
    {
        private IWeaponDataProvider _weaponDataProvider;
        private IObjectPool<Bullet> _bulletPool;
        

        public void Initialize(IWeaponDataProvider weaponDataProvider)
        {
            _weaponDataProvider = weaponDataProvider;

            _bulletPool?.Clear();

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
            Bullet bulletPrefab = _weaponDataProvider.WeaponData.BulletPrefab;
            Bullet bullet = Instantiate(bulletPrefab, transform);
            bullet.Initialize(_bulletPool);
            return bullet;
        }

        public Bullet GetBullet()
        {
            return _bulletPool.Get();
        }

        public void ReleaseBullet(Bullet bullet)
        {
            _bulletPool.Release(bullet);
        }
    }
}