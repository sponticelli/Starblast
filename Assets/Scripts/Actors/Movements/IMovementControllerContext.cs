using Starblast.Data;
using Starblast.Data.Spaceships.Bodies;
using Starblast.Data.Spaceships.Engines;
using Starblast.Inputs;

namespace Starblast.Actors.Movements
{
    public interface IMovementControllerContext
    {
        ISpaceshipBodyDataProvider SpaceshipBodyDataProvider { get; }
        ISpaceshipEngineDataProvider SpaceshipEngineDataProvider { get; }
        
        
        IRigidbody2DProvider Rigidbody2DProvider { get; }
        IActorInputHandler InputHandler { get; }
    }
}