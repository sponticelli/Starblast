namespace Starblast.Data.Spaceships.Visuals
{
    public class SpaceshipVisualDataProvider : ISpaceshipVisualDataProvider
    {
        public SpaceshipVisualDataProvider(ISpaceshipVisualData spaceshipVisualData)
        {
            Data = spaceshipVisualData;
        }

        public ISpaceshipVisualData Data { get; }
    }
}