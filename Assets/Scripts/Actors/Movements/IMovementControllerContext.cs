using Starblast.Data;
using Starblast.Inputs;

namespace Starblast.Actors.Movements
{
    public interface IMovementControllerContext
    {
        IBodyDataProvider BodyDataProvider { get; }
        IEngineDataProvider EngineDataProvider { get; }
        
        
        IRigidbody2DProvider Rigidbody2DProvider { get; }
        IActorInputHandler InputHandler { get; }
    }
}