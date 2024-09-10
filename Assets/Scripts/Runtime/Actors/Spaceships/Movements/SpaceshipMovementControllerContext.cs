using Starblast.Data;
using Starblast.Inputs;
using UnityEngine;

namespace Starblast.Actors.Movements
{
    public class SpaceshipMovementControllerContext : ISpaceshipMovementControllerContext
    {
        public IBodyData BodyData { get; private set; }
        public IPropulsorData PropulsorData { get; private set; }
        
        public Rigidbody2D Rigidbody2D { get; private set; }
        public IActorInputHandler InputHandler { get; private set; }
        
        public SpaceshipMovementControllerContext(IBodyData bodyData,
            IPropulsorData propulsorData,
            Rigidbody2D rigidbody2D,
            IActorInputHandler inputHandler)
        {
            BodyData = bodyData;
            PropulsorData = propulsorData;
            Rigidbody2D = rigidbody2D;
            InputHandler = inputHandler;
        }
    }
}