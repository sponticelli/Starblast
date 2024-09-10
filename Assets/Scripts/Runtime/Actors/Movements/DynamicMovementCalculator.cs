using Starblast.Data;
using UnityEngine;

namespace Starblast.Actors.Movements
{
    public class DynamicMovementCalculator : IMovementCalculator
    {
        private readonly IBodyData _bodyData;
        private readonly IPropulsorData _propulsorData;

        public DynamicMovementCalculator(IBodyData bodyData, IPropulsorData propulsorData)
        {
            _bodyData = bodyData;
            _propulsorData = propulsorData;
        }

        public Vector2 CalculateVelocity(Vector2 currentVelocity, float thrustInput, Vector2 forwardDirection, float deltaTime)
        {
            // Calculate thrust force
            Vector2 thrustForce = forwardDirection * (thrustInput * _propulsorData.Acceleration);

            // Calculate new velocity based on acceleration
            Vector2 acceleration = thrustForce / _bodyData.Mass; // Assuming we add a Mass property to IBodyData
            Vector2 velocityChange = acceleration * deltaTime;
            Vector2 newVelocity = currentVelocity + velocityChange;

            // Apply auto-braking when not thrusting
            if (Mathf.Abs(thrustInput) < _bodyData.AutoBrakeThreshold)
            {
                newVelocity = ApplyAutoBrake(newVelocity, forwardDirection, deltaTime);
            }

            // Clamp to max speed
            return Vector2.ClampMagnitude(newVelocity, _propulsorData.MaxSpeed);
        }

        public float CalculateRotation(float rotationInput, float currentSpeed, float deltaTime)
        {
            float speedFactor = currentSpeed / _propulsorData.MaxSpeed;
            float maneuverability = _propulsorData.SpeedToManeuverability.Evaluate(speedFactor);
            
            // Calculate torque instead of direct rotation
            float torque = -rotationInput * _bodyData.RotationSpeed * maneuverability;
            
            // Convert torque to angular acceleration (assuming we add a MomentOfInertia property to IBodyData)
            float angularAcceleration = torque / _bodyData.MomentOfInertia;
            
            // Calculate change in angular velocity
            float angularVelocityChange = angularAcceleration * deltaTime;
            
            return angularVelocityChange;
        }

        private Vector2 ApplyAutoBrake(Vector2 velocity, Vector2 forwardDirection, float deltaTime)
        {
            Vector2 forwardVelocity = Vector2.Dot(velocity, forwardDirection) * forwardDirection;
            Vector2 orthogonalVelocity = velocity - forwardVelocity;

            forwardVelocity = ApplyBrakeToComponent(forwardVelocity, _bodyData.ForwardAutoBrakeFactor, deltaTime);
            orthogonalVelocity = ApplyBrakeToComponent(orthogonalVelocity, _bodyData.OrthogonalAutoBrakeFactor, deltaTime);

            return forwardVelocity + orthogonalVelocity;
        }

        private Vector2 ApplyBrakeToComponent(Vector2 velocityComponent, float brakeFactor, float deltaTime)
        {
            float brakeMagnitude = brakeFactor * velocityComponent.magnitude * deltaTime;
            if (brakeMagnitude > velocityComponent.magnitude)
            {
                return Vector2.zero;
            }
            return velocityComponent.normalized * (velocityComponent.magnitude - brakeMagnitude);
        }
    }
}