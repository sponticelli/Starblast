using Starblast.Data;
using UnityEngine;

namespace Starblast.Actors.Movements
{
    public abstract class AMovementComponentFactory : ScriptableObject, IMovementComponentFactory
    {
        public abstract IPhysicsApplier CreatePhysicsApplier(Rigidbody2D rigidbody);
        public abstract IMovementCalculator CreateMovementCalculator(IBodyData bodyData, IPropulsorData propulsorData);
    }
}