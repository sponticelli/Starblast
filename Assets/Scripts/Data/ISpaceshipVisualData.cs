using UnityEngine;

namespace Starblast.Data
{
    public interface ISpaceshipVisualData
    {
        
        public Sprite BodySprite { get; }
        public Sprite EngineSprite { get; }
        
        public float MaxRotationAngle { get; }
        public float RotationSpeed { get; }
        float ReturnToZeroFactor { get; }
    }
}