using Starblast.Data.Asteroids.Movements;
using Starblast.Data.Asteroids.Visuals;

namespace Starblast.Data.Asteroids
{
    public interface IAsteroidData : IData
    {
        IAsteroidVisualData VisualData { get; }
        IAsteroidMovementData MovementData { get; }
    }
}