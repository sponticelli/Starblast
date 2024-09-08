namespace Starblast.Data.Asteroids.Movements
{
    public class AsteroidMovementDataProvider : IAsteroidMovementDataProvider
    {
        public AsteroidMovementDataProvider(IAsteroidMovementData data)
        {
            Data = data;
        }

        public IAsteroidMovementData Data { get; }
    }
}