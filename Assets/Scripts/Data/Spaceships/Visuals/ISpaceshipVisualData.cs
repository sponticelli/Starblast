using UnityEngine;

namespace Starblast.Data.Spaceships.Visuals
{
    public interface ISpaceshipVisualData : IData
    {
        
        public Sprite BodySprite { get; }
        public Sprite EngineSprite { get; }
        
        public float MaxRotationAngle { get; }
        public float RotationSpeed { get; }
        float ReturnToZeroFactor { get; }
    }
}