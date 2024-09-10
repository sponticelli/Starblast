using UnityEngine;

namespace Starblast.Data
{
    public interface IPropulsorData : IData
    {
        float MaxSpeed { get; }
        float Acceleration { get; }
        float BrakingForce { get; }
        AnimationCurve SpeedToManeuverability { get; }
    }
}