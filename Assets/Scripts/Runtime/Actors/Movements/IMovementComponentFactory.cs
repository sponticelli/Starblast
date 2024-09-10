using Starblast.Data;
using UnityEngine;

namespace Starblast.Actors.Movements
{
    public interface IMovementComponentFactory
    {
        IPhysicsApplier CreatePhysicsApplier(Rigidbody2D rigidbody);
        IMovementCalculator CreateMovementCalculator(IBodyData bodyData, IPropulsorData propulsorData);
    }
}