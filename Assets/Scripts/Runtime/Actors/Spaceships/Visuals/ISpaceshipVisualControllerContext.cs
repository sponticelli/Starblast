using Starblast.Data.Spaceships.Visuals;
using Starblast.Inputs;

namespace Starblast.Actors.Visuals
{
    public interface ISpaceshipVisualControllerContext
    {
        public IActorInputHandler ActorInputHandler { get; }
        
        public ISpaceshipVisualData VisualData { get; }
    }
}