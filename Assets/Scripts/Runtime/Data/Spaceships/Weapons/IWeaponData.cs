using Starblast.Actors.Weapons;
using UnityEngine;

namespace Starblast.Data.Spaceships.Weapons
{
    public interface IWeaponData : IData
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