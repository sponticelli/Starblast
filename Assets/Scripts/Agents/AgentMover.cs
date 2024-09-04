using UnityEngine;
using UnityEngine.Events;

namespace Starblast.Agents
{
    public class AgentMover : MonoBehaviour
    {
        [System.Serializable]
        public class SpaceshipEvent : UnityEvent<float> { }

        [Header("References")]
        public Rigidbody2D rb;
        
        [Header("Movement Settings")] 
        public float maxSpeed = 10f;
        public float acceleration = 5f;
        public float rotationSpeed = 180f;
        public float brakingForce = 5f;
        public AnimationCurve speedToManeuverability;

        [Header("Auto-Brake Settings")]
        public float forwardAutoBrakeFactor = 0.3f;
        public float orthogonalAutoBrakeFactor = 0.7f;
        public float autoBrakeThreshold = 0.1f;

        [Header("Boost Settings")] 
        public float boostMultiplier = 2f;
        public float boostDuration = 2f;
        public float boostCooldown = 5f;

        private Vector2 velocity;
        private float currentRotation;
        
        private float thrustInput;
        private float rotationInput;

        public void SetRotationInput(float input)
        {
            rotationInput = input;
        }

        public void SetThrustInput(float input)
        {
            thrustInput = input;
        }
        
        private void Start()
        {
            rb.isKinematic = true;
        }

        private void Update()
        {
            HandleInput();
        }

        private void FixedUpdate()
        {
            ApplyMovement();
            ApplyRotation();
            ApplyDirectionalAutoBrake();
        }

        private void HandleInput()
        {
            if (thrustInput > 0)
            {
                ApplyThrust(thrustInput);
            }
            else if (thrustInput < 0)
            {
                ApplyBrake(-thrustInput);
            }

            currentRotation = rotationInput * rotationSpeed;
        }

        private void ApplyThrust(float input)
        {
            float accelerationThisFrame = acceleration * input;
            velocity += (Vector2)transform.up * (accelerationThisFrame * Time.fixedDeltaTime);
        }

        private void ApplyBrake(float input)
        {
            velocity -= velocity.normalized * (brakingForce * input * Time.fixedDeltaTime);
        }

        private void ApplyDirectionalAutoBrake()
        {
            if (Mathf.Abs(thrustInput) < autoBrakeThreshold)
            {
                Vector2 forwardDirection = transform.up;
                Vector2 orthogonalDirection = Vector2.Perpendicular(forwardDirection);

                // Decompose velocity into forward and orthogonal components
                Vector2 forwardVelocity = Vector2.Dot(velocity, forwardDirection) * forwardDirection;
                Vector2 orthogonalVelocity = Vector2.Dot(velocity, orthogonalDirection) * orthogonalDirection;

                // Apply auto-brake to each component separately
                forwardVelocity *= (1f - forwardAutoBrakeFactor * Time.fixedDeltaTime);
                orthogonalVelocity *= (1f - orthogonalAutoBrakeFactor * Time.fixedDeltaTime);

                // Recombine the velocities
                velocity = forwardVelocity + orthogonalVelocity;
            }
        }

        private void ApplyMovement()
        {
            velocity = Vector2.ClampMagnitude(velocity, maxSpeed);
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }

        private void ApplyRotation()
        {
            float speedFactor = velocity.magnitude / maxSpeed;
            float maneuverability = speedToManeuverability.Evaluate(speedFactor);
            float rotationThisFrame = currentRotation * maneuverability * Time.fixedDeltaTime;
            rb.MoveRotation(rb.rotation - rotationThisFrame);
        }

        public Vector2 GetVelocity() => velocity;
        
        private void OnCollisionEnter2D(Collision2D collision)
        {
            // Handle collisions here
            velocity = Vector2.zero;
        }
    }
}