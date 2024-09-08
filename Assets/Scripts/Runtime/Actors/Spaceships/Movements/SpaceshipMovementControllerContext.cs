using Starblast.Data.Spaceships.Bodies;
using Starblast.Data.Spaceships.Engines;
using Starblast.Inputs;

namespace Starblast.Actors.Movements
{
    public class SpaceshipMovementControllerContext : ISpaceshipMovementControllerContext
    {
        public ISpaceshipBodyDataProvider SpaceshipBodyDataProvider { get; private set; }
        public ISpaceshipEngineDataProvider SpaceshipEngineDataProvider { get; private set; }
        
        public IRigidbody2DProvider Rigidbody2DProvider { get; private set; }
        public IActorInputHandler InputHandler { get; private set; }
        
        public SpaceshipMovementControllerContext(ISpaceshipBodyDataProvider spaceshipBodyDataProvider,
            ISpaceshipEngineDataProvider spaceshipEngineDataProvider,
            IRigidbody2DProvider rigidbody2DProvider,
            IActorInputHandler inputHandler)
        {
            SpaceshipBodyDataProvider = spaceshipBodyDataProvider;
            SpaceshipEngineDataProvider = spaceshipEngineDataProvider;
            Rigidbody2DProvider = rigidbody2DProvider;
            InputHandler = inputHandler;
        }
    }
}