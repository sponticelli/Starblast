using Starblast.Actors.Weapons;
using UnityEngine;

namespace Starblast.Data
{
    [CreateAssetMenu(fileName = "NewWeaponData", menuName = "Starblast/Spaceship/Weapon Data")]
    public class WeaponDataSO : ScriptableObject, IWeaponData
    {
        [field: SerializeField]
        public Vector2 MuzzleOffset { get; set; }
        [field: SerializeField]
        [field: Range(1, 100)]
        public int AmmoCapacity { get; set; } = 10; 
        
        [field: SerializeField]
        public bool AutomaticFire { get; set; } = false;
        [field: SerializeField]
        [field: Range(0.01f, 2f)]
        public float WeaponDelay { get; set; } = 0.1f;
        
        [field: SerializeField]
        [field: Range(0f, 10f)]
        public float SpreadAngle { get; set; } = 5f;
        
        [SerializeField]
        private bool _multiBulletShoot = false;
        [SerializeField]
        [Range(1, 10)]
        private int _bulletCountToSpawn = 1;
        
        [field: SerializeField]
        public Bullet BulletPrefab { get; set; }

        public int GetBulletCountToSpawn => _multiBulletShoot ? _bulletCountToSpawn : 1;
    }
}