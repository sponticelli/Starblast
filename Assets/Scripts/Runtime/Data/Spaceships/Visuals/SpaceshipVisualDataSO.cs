using UnityEngine;

namespace Starblast.Data.Spaceships.Visuals
{
    [CreateAssetMenu(fileName = "NewSpaceshipVisualData", menuName = "Starblast/Spaceship/Data/Visual Data")]
    public class SpaceshipVisualDataSO : ScriptableObject, ISpaceshipVisualData
    {
        [field: SerializeField] public Sprite BodySprite { get; private set; }
        [field: SerializeField] public Sprite EngineSprite { get; private set; }
       
        [field: SerializeField][field: Range(0, 60)] public float MaxRotationAngle { get; private set; }
        [field: SerializeField] public float RotationSpeed { get; private set; }
        [field: SerializeField] public float ReturnToZeroFactor { get; private set; }
    }
}