namespace Starblast.Data
{
    public interface IBodyData : IData
    {
        float RotationSpeed { get; }
        float ForwardAutoBrakeFactor { get; }
        float OrthogonalAutoBrakeFactor { get; }
        float AutoBrakeThreshold { get; }
        
        float Mass { get; }
        float MomentOfInertia { get; }
    }
}