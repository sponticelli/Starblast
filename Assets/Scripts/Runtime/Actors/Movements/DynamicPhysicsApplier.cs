using UnityEngine;

namespace Starblast.Actors.Movements
{
    public class DynamicPhysicsApplier : IPhysicsApplier
    {
        private readonly Rigidbody2D _rigidbody;

        public DynamicPhysicsApplier(Rigidbody2D rigidbody)
        {
            _rigidbody = rigidbody;
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            _rigidbody.gravityScale = 0f;
        }

        public void ApplyMovement(Vector2 velocityChange, float deltaTime)
        {
            _rigidbody.velocity += velocityChange;
        }

        public void ApplyRotation(float angularVelocityChange)
        {
            _rigidbody.angularVelocity += angularVelocityChange;
        }
    }
}