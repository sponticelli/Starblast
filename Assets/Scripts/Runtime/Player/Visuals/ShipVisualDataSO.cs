using UnityEngine;

namespace Starblast.Player.Visuals
{
    [CreateAssetMenu(fileName = "ShipVisualData", menuName = "Starblast/Data/Player/Visual Data")]
    public class ShipVisualDataSO : ScriptableObject
    {
        [field: SerializeField] 
        public float MaxRotationAngle { get; private set; }
        [field: SerializeField]
        public float RotationSpeed { get; private set; }
        [field: SerializeField]
        public float ReturnToZeroFactor { get; private set; }
    }
}