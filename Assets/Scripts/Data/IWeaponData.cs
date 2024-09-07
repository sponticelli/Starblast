using Starblast.Actors.Weapons;
using UnityEngine;

namespace Starblast.Data
{
    public interface IWeaponData
    {
        Vector2 MuzzleOffset { get; set; }
        int AmmoCapacity { get; set; }
        bool AutomaticFire { get; set; }
        float WeaponDelay { get; set; }
        float SpreadAngle { get; set; }
        Bullet BulletPrefab { get; set; }
        int GetBulletCountToSpawn { get; }
    }
}