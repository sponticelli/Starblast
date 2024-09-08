namespace Starblast.Data.Spaceships.Bodies
{
    public class SpaceshipBodyDataProvider : ISpaceshipBodyDataProvider
    {
        public ISpaceshipBodyData Data { get; }

        public SpaceshipBodyDataProvider(ISpaceshipBodyData spaceshipBodyData)
        {
            Data = spaceshipBodyData;
        }
    }
}