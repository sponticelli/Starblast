using UnityEngine;

namespace Starblast.Actors.Movements
{
    public interface IMovementCalculator
    {
        Vector2 CalculateVelocity(Vector2 currentVelocity, float thrustInput, Vector2 forwardDirection, float deltaTime);
        float CalculateRotation(float rotationInput, float currentSpeed, float deltaTime);
    }
    
}