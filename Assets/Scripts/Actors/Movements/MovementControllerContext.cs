using Starblast.Data;
using Starblast.Inputs;

namespace Starblast.Actors.Movements
{
    public class MovementControllerContext : IMovementControllerContext
    {
        public IBodyDataProvider BodyDataProvider { get; private set; }
        public IEngineDataProvider EngineDataProvider { get; private set; }
        
        public IRigidbody2DProvider Rigidbody2DProvider { get; private set; }
        public IActorInputHandler InputHandler { get; private set; }
        
        public MovementControllerContext(IBodyDataProvider bodyDataProvider,
            IEngineDataProvider engineDataProvider,
            IRigidbody2DProvider rigidbody2DProvider,
            IActorInputHandler inputHandler)
        {
            BodyDataProvider = bodyDataProvider;
            EngineDataProvider = engineDataProvider;
            Rigidbody2DProvider = rigidbody2DProvider;
            InputHandler = inputHandler;
        }
    }
}