using Starblast.Data;
using UnityEngine;

namespace Starblast.Actors.Movements
{
    [CreateAssetMenu(menuName = "Starblast/Data/Dynamic Movement Component Factory", 
        fileName = "NewDynamicMovementComponentFactory")]
    public class DynamicMovementComponentFactory : AMovementComponentFactory
    {
        public override IPhysicsApplier CreatePhysicsApplier(Rigidbody2D rigidbody)
        {
            return new DynamicPhysicsApplier(rigidbody);
        }

        public override IMovementCalculator CreateMovementCalculator(IBodyData bodyData, IPropulsorData propulsorData)
        {
            return new DynamicMovementCalculator(bodyData, propulsorData);
        }
    }
}