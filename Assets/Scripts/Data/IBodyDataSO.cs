namespace Starblast.Data
{
    public interface IBodyData
    {
        float RotationSpeed { get; }
        float ForwardAutoBrakeFactor { get; }
        float OrthogonalAutoBrakeFactor { get; }
        float AutoBrakeThreshold { get; }
    }
}