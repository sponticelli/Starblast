using UnityEngine;

namespace Starblast.Agents
{
    public class AgentMover : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float _rotSpeed = 320;
        [SerializeField] private float _maxSpeed = 40;
        [SerializeField] private float _acceleration = 10;
        [SerializeField] private float _deceleration = 5;
        [SerializeField] private float _thrustForce = 2.3f;
        [SerializeField] private float _rotFactorWhileAccelerating = 0.3f;
        
        
        [Header("References")]
        [SerializeField] private Rigidbody2D _rigidbody;
        
        private float rotInput = 0;
        private float thrustInput = 0;
        
        protected float curThrust = 0;
        private float maxThrust = 1;
        
        public void SetRotationInput(float input)
        {
            rotInput = input;
        }

        public void SetThrustInput(float input)
        {
            thrustInput = input;
        }
        
        private void FixedUpdate()
        {
            HandleRotation();
            HandleThrust();
        }
        
        private void HandleRotation()
        {
            if (rotInput != 0)
            {
                var new_rotation = _rigidbody.rotation - _rotSpeed * GetRotPower() * Time.fixedDeltaTime * rotInput;
                new_rotation = NormalizeAngle(new_rotation);
                _rigidbody.MoveRotation(new_rotation);
            }

        }
        
        private void HandleThrust()
        {
            if (IsAccelerating())
            {
                curThrust = Mathf.Min(curThrust + _thrustForce * Time.fixedDeltaTime, maxThrust);
                _rigidbody.AddForce(GetAimDir() * (_acceleration * curThrust), ForceMode2D.Impulse);
            }
            else
            {
                curThrust = 0;
                if (IsDecelerating())
                {
                    // Movement in the opposite direction
                    var velocity = _rigidbody.velocity;
                    var oppositeDir = -velocity.normalized;
                    // Apply the brake but not stronger than the current velocity
                    _rigidbody.AddForce(oppositeDir * Mathf.Min(_deceleration, velocity.magnitude), ForceMode2D.Impulse);
                }
            }
        }
        
        protected virtual float GetRotPower()
        {
            if (IsAccelerating())
                return _rotFactorWhileAccelerating;
            return 1;
        }

        public bool IsAccelerating()
        {
            return thrustInput > 0;
        }
        
        public bool IsDecelerating()
        {
            return thrustInput < 0;
        }
        
        protected float NormalizeAngle(float ang)
        {
            ang = (ang + 180) % 360;
            if (ang < 0)
                ang += 360;
            return ang - 180;
        }
        
        public Vector2 GetVelDir()
        {
            return _rigidbody.velocity.normalized;
        }

        public Vector2 GetAimDir()
        {
            return _rigidbody.transform.up;
        }

    }
}