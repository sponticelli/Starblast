using UnityEngine;

namespace Starblast.Data
{
    [CreateAssetMenu(fileName = "BodyData", menuName = "Starblast/Data/Body Data")]
    public class BodyDataSO : ScriptableObject, IBodyData
    {
        [SerializeField] private float _rotationSpeed = 180f;
        
        [Header("Auto-Brake Settings")]
        [SerializeField] private float _forwardAutoBrakeFactor = 0.15f;
        [SerializeField] private float _orthogonalAutoBrakeFactor = 20f;
        [SerializeField] private float _autoBrakeThreshold = 1.1f;
        
        [Header("Physics Settings")]
        [SerializeField] private float _mass = 1f;
        [SerializeField] private float _momentOfInertia = 1f;
        
        
        public float RotationSpeed => _rotationSpeed;
        public float ForwardAutoBrakeFactor => _forwardAutoBrakeFactor;
        public float OrthogonalAutoBrakeFactor => _orthogonalAutoBrakeFactor;
        public float AutoBrakeThreshold => _autoBrakeThreshold;
        public float Mass => _mass;
        public float MomentOfInertia => _momentOfInertia;
    }
    
    
}