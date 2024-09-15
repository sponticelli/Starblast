using UnityEngine;
using UnityEngine.Pool;

namespace Starblast.Weapons
{
    public class BulletPoolManager : MonoBehaviour
    {
        private WeaponDataSO _weaponData;
        private IObjectPool<Bullet> _bulletPool;
        

        public void Initialize(WeaponDataSO weaponData)
        {
            _weaponData = weaponData;

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
            Bullet bulletPrefab = _weaponData.BulletPrefab;
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