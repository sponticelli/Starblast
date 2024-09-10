using UnityEngine;

namespace Starblast.Actors.Movements
{
    public interface IPhysicsApplier
    {
        void ApplyMovement(Vector2 velocity, float deltaTime);
        void ApplyRotation(float rotation);
    }
}