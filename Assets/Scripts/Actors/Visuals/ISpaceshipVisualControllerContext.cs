using Starblast.Data;
using Starblast.Inputs;

namespace Starblast.Actors.Visuals
{
    public interface ISpaceshipVisualControllerContext
    {
        public IActorInputHandler ActorInputHandler { get; }
        
        public ISpaceshipVisualDataProvider VisualDataProvider { get; }
    }
}