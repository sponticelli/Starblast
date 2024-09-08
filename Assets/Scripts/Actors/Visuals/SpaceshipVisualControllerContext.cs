using Starblast.Data;
using Starblast.Data.Spaceships.Visuals;
using Starblast.Inputs;

namespace Starblast.Actors.Visuals
{
    public class SpaceshipVisualControllerContext : ISpaceshipVisualControllerContext
    {
        public IActorInputHandler ActorInputHandler { get; }
        public ISpaceshipVisualDataProvider VisualDataProvider { get; }

        public SpaceshipVisualControllerContext(IActorInputHandler actorInputHandler, 
            ISpaceshipVisualDataProvider visualDataProvider)
        {
            ActorInputHandler = actorInputHandler;
            VisualDataProvider = visualDataProvider;
        }

    }
}