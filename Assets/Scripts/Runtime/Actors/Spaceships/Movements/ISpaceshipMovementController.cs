namespace Starblast.Actors.Movements
{
    public interface ISpaceshipMovementController
    {
        void Initialize(ISpaceshipMovementControllerContext context,  
            IPhysicsApplier physicsApplier,
            IMovementCalculator movementCalculator);
    }
}