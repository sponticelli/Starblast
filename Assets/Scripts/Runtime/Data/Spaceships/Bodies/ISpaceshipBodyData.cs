namespace Starblast.Data.Spaceships.Bodies
{
    public interface ISpaceshipBodyData : IData
    {
        float RotationSpeed { get; }
        float ForwardAutoBrakeFactor { get; }
        float OrthogonalAutoBrakeFactor { get; }
        float AutoBrakeThreshold { get; }
    }
}