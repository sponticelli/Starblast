using Starblast.Data;
using UnityEngine;

namespace Starblast.Actors.Movements
{
    [CreateAssetMenu(menuName = "Starblast/Data/Kinematic Movement Component Factory", 
        fileName = "NewKinematicMovementComponentFactory")]
    public class KinematicMovementComponentFactory : AMovementComponentFactory
    {
        public override IPhysicsApplier CreatePhysicsApplier(Rigidbody2D rigidbody)
        {
            return new KinematicPhysicsApplier(rigidbody);
        }

        public override IMovementCalculator CreateMovementCalculator(IBodyData bodyData, IPropulsorData propulsorData)
        {
            return new KinematicMovementCalculator(bodyData, propulsorData);
        }
    }
}