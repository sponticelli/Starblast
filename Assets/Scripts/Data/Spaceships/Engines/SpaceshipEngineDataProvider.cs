namespace Starblast.Data.Spaceships.Engines
{
    public class SpaceshipEngineDataProvider : ISpaceshipEngineDataProvider
    {
        private readonly ISpaceshipEngineData _data;

        public SpaceshipEngineDataProvider(ISpaceshipEngineData data)
        {
            _data = data;
        }

        public ISpaceshipEngineData Data => _data;
    }
}