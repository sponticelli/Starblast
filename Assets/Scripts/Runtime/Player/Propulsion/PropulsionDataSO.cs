using UnityEngine;

namespace Starblast.Player.Propulsion
{
    [CreateAssetMenu(fileName = "PropulsionData", menuName = "Starblast/Data/Player/Propulsion Data")]
    public class PropulsionDataSO : ScriptableObject
    {
        [Header("Propulsion Settings")]
        [SerializeField] private float _maxSpeed = 20f;
        [SerializeField] private float _acceleration = 4f;
        [SerializeField] private float _brakingForce = 1f;
       
        [Header("Maneuverability Settings")]
        [SerializeField] private AnimationCurve _speedToManeuverability = AnimationCurve.Linear(0f, 1f, 1f, 0.75f);
        [SerializeField] private float _rotationSpeed = 180f;
        
        [Header("Auto-Brake Settings")]
        [SerializeField] private float _forwardAutoBrakeFactor = 0.15f;
        [SerializeField] private float _orthogonalAutoBrakeFactor = 20f;
        [SerializeField] private float _autoBrakeThreshold = 1.1f;
        
        
        
        public float RotationSpeed => _rotationSpeed;
        public float ForwardAutoBrakeFactor => _forwardAutoBrakeFactor;
        public float OrthogonalAutoBrakeFactor => _orthogonalAutoBrakeFactor;
        public float AutoBrakeThreshold => _autoBrakeThreshold;
        
        public float MaxSpeed => _maxSpeed;
        public float Acceleration => _acceleration;
        public float BrakingForce => _brakingForce;
        public AnimationCurve SpeedToManeuverability => _speedToManeuverability;
    }
}