namespace Starblast.Data.Spaceships.Visuals
{
    public class SpaceshipVisualDataProvider : ISpaceshipVisualDataProvider
    {
        private readonly ISpaceshipVisualData _spaceshipVisualData;

        public SpaceshipVisualDataProvider(ISpaceshipVisualData spaceshipVisualData)
        {
            _spaceshipVisualData = spaceshipVisualData;
        }

        public ISpaceshipVisualData Data => _spaceshipVisualData;
    }
}