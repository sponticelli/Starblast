using UnityEngine;

namespace Starblast.Actors.Movements
{
    public class KinematicPhysicsApplier : IPhysicsApplier
    {
        private readonly Rigidbody2D _rigidbody;

        public KinematicPhysicsApplier(Rigidbody2D rigidbody)
        {
            _rigidbody = rigidbody;
        }

        public void ApplyMovement(Vector2 velocity, float deltaTime)
        {
            _rigidbody.MovePosition(_rigidbody.position + velocity * deltaTime);
        }

        public void ApplyRotation(float rotation)
        {
            _rigidbody.MoveRotation(_rigidbody.rotation + rotation);
        }
    }
}