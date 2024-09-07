using UnityEngine;

namespace Starblast.Data
{
    public interface IEngineData
    {
        float MaxSpeed { get; }
        float Acceleration { get; }
        float BrakingForce { get; }
        AnimationCurve SpeedToManeuverability { get; }
    }
}