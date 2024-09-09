using Starblast.Actors.Movements;
using Starblast.Data.Spaceships.Bodies;
using Starblast.Data.Spaceships.Engines;
using UnityEngine;

namespace Starblast.Actors.Spaceships.Movements
{
    public class SpaceshipMovementCalculator : IMovementCalculator
    {
        private readonly ISpaceshipBodyData _bodyData;
        private readonly ISpaceshipEngineData _engineData;

        public SpaceshipMovementCalculator(ISpaceshipBodyData bodyData, ISpaceshipEngineData engineData)
        {
            _bodyData = bodyData;
            _engineData = engineData;
        }

        public Vector2 CalculateVelocity(Vector2 currentVelocity, float thrustInput, Vector2 forwardDirection, float deltaTime)
        {
            Vector2 newVelocity = currentVelocity;

            if (thrustInput > 0)
            {
                newVelocity += ApplyThrust(thrustInput, forwardDirection, deltaTime);
            }
            else if (thrustInput < 0)
            {
                newVelocity += ApplyBrake(newVelocity, -thrustInput, deltaTime);
            }

            newVelocity = ApplyDirectionalAutoBrake(newVelocity, thrustInput, forwardDirection, deltaTime);
            return Vector2.ClampMagnitude(newVelocity, _engineData.MaxSpeed);
        }

        public float CalculateRotation(float rotationInput, float currentSpeed, float deltaTime)
        {
            var speedFactor = currentSpeed / _engineData.MaxSpeed;
            var maneuverability = _engineData.SpeedToManeuverability.Evaluate(speedFactor);
            return -rotationInput * _bodyData.RotationSpeed * maneuverability * deltaTime;
        }

        private Vector2 ApplyThrust(float input, Vector2 forwardDirection, float deltaTime)
        {
            var accelerationThisFrame = _engineData.Acceleration * input;
            return forwardDirection * (accelerationThisFrame * deltaTime);
        }

        private Vector2 ApplyBrake(Vector2 currentVelocity, float input, float deltaTime)
        {
            return -currentVelocity.normalized * (_engineData.BrakingForce * input * deltaTime);
        }

        private Vector2 ApplyDirectionalAutoBrake(Vector2 currentVelocity, float thrustInput, Vector2 forwardDirection, float deltaTime)
        {
            if (Mathf.Abs(thrustInput) >= _bodyData.AutoBrakeThreshold)
            {
                return currentVelocity;
            }

            var orthogonalDirection = Vector2.Perpendicular(forwardDirection);

            var forwardVelocity = Vector2.Dot(currentVelocity, forwardDirection) * forwardDirection;
            var orthogonalVelocity = Vector2.Dot(currentVelocity, orthogonalDirection) * orthogonalDirection;

            forwardVelocity *= (1f - _bodyData.ForwardAutoBrakeFactor * deltaTime);
            orthogonalVelocity *= (1f - _bodyData.OrthogonalAutoBrakeFactor * deltaTime);

            return forwardVelocity + orthogonalVelocity;
        }
    }
}