using Starblast.Data.Spaceships.Bodies;
using Starblast.Data.Spaceships.Engines;
using Starblast.Inputs;
using UnityEngine;

namespace Starblast.Actors.Movements
{
    public class SpaceshipMovementControllerContext : ISpaceshipMovementControllerContext
    {
        public ISpaceshipBodyData SpaceshipBodyData { get; private set; }
        public ISpaceshipEngineData SpaceshipEngineData { get; private set; }
        
        public Rigidbody2D Rigidbody2D { get; private set; }
        public IActorInputHandler InputHandler { get; private set; }
        
        public SpaceshipMovementControllerContext(ISpaceshipBodyData spaceshipBodyData,
            ISpaceshipEngineData spaceshipEngineData,
            Rigidbody2D rigidbody2D,
            IActorInputHandler inputHandler)
        {
            SpaceshipBodyData = spaceshipBodyData;
            SpaceshipEngineData = spaceshipEngineData;
            Rigidbody2D = rigidbody2D;
            InputHandler = inputHandler;
        }
    }
}