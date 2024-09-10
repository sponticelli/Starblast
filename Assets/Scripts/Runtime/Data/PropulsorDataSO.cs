using UnityEngine;

namespace Starblast.Data
{
    [CreateAssetMenu(fileName = "PropulsorData", menuName = "Starblast/Data/Propulsor Data")]
    public class PropulsorDataSO : ScriptableObject, IPropulsorData
    {
        [SerializeField] private float _maxSpeed = 20f;
        [SerializeField] private float _acceleration = 4f;
        [SerializeField] private float _brakingForce = 1f;
        [SerializeField] private AnimationCurve _speedToManeuverability = AnimationCurve.Linear(0f, 1f, 1f, 0.75f);
        
        public float MaxSpeed => _maxSpeed;
        public float Acceleration => _acceleration;
        public float BrakingForce => _brakingForce;
        public AnimationCurve SpeedToManeuverability => _speedToManeuverability;
    }
    

}