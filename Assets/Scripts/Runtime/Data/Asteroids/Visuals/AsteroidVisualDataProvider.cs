namespace Starblast.Data.Asteroids.Visuals
{
    public class AsteroidVisualDataProvider : IAsteroidVisualDataProvider
    {
        public AsteroidVisualDataProvider(IAsteroidVisualData spaceshipVisualData)
        {
            Data = spaceshipVisualData;
        }

        public IAsteroidVisualData Data { get; }
    }
}