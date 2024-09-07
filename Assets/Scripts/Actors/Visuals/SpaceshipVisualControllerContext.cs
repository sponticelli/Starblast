using Starblast.Data;
using Starblast.Inputs;

namespace Starblast.Actors.Visuals
{
    public class SpaceshipVisualControllerContext : ISpaceshipVisualControllerContext
    {
        public IActorInputHandler ActorInputHandler { get; }
        public ISpaceshipVisualDataProvider VisualDataProvider { get; }

        public SpaceshipVisualControllerContext(IActorInputHandler actorInputHandler, SpaceshipVisualDataProvider visualDataProvider)
        {
            ActorInputHandler = actorInputHandler;
            VisualDataProvider = visualDataProvider;
        }

    }
}