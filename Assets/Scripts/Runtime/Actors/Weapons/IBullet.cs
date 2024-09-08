using UnityEngine;

namespace Starblast.Actors.Weapons
{
    public interface IBullet
    {
        void Shoot(Vector3 direction, Vector2 shipVelocity);
    }
}