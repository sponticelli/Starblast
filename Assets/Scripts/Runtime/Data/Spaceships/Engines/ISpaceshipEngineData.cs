using UnityEngine;

namespace Starblast.Data.Spaceships.Engines
{
    public interface ISpaceshipEngineData : IData
    {
        float MaxSpeed { get; }
        float Acceleration { get; }
        float BrakingForce { get; }
        AnimationCurve SpeedToManeuverability { get; }
    }
}