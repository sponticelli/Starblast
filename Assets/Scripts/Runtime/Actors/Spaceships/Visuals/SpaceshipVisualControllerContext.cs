using Starblast.Data.Spaceships.Visuals;
using Starblast.Inputs;

namespace Starblast.Actors.Visuals
{
    public class SpaceshipVisualControllerContext : ISpaceshipVisualControllerContext
    {
        public IActorInputHandler ActorInputHandler { get; }
        public ISpaceshipVisualData VisualData { get; }

        public SpaceshipVisualControllerContext(IActorInputHandler actorInputHandler, 
            ISpaceshipVisualData visualDataProvider)
        {
            ActorInputHandler = actorInputHandler;
            VisualData = visualDataProvider;
        }

    }
}