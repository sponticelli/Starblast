using UnityEngine;

namespace Starblast.Weapons
{
    public interface IBullet
    {
        void Shoot(Vector3 direction, Vector2 shipVelocity);
    }
}