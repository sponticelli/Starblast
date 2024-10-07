using Starblast.Services;
using TMPro;
using UnityEngine;

namespace Starblast.Environments.Boundaries
{
    /// <summary>
    /// It teleports objects to the other side of the camera when they are far enough from the player.
    /// </summary>
    [DefaultExecutionOrder(ExecutionOrder.Services)]
    public class LevelBounds : MonoBehaviour, IInitializable
    {
        [Header("Settings")]
        [SerializeField] private float _innerRadius = 100f;
        [SerializeField] private float _textDistance = 1f;
        [SerializeField] private float _outerRadius = 120f;
        
        [SerializeField] private Color _innerColor = Color.red;

        [Header("References")]
        [SerializeField] private CircleOutline _circleOutline;
        [SerializeField] private Transform _playerTransform;
        [SerializeField] private TextMeshPro _text;
        
        
        public float InnerRadius => _innerRadius;
        public float OuterRadius => _outerRadius;
        
        public bool IsInitialized { get; private set; }
        
        private void Start()
        {
            Initialize();
        }
        
        public void Initialize()
        {
            Debug.Log("Initializing LevelBounds");
            
            if (IsInitialized) return;
            _circleOutline.SetRadius(_innerRadius);
            _circleOutline.SetColor(_innerColor);
            IsInitialized = true;
            
            Debug.Log("LevelBounds initialized");
        }

        private void OnDrawGizmos()
        {
            // orange
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f); 
            Gizmos.DrawWireSphere(Vector3.zero, _innerRadius);
            
            // red
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
            Gizmos.DrawWireSphere(Vector3.zero, _outerRadius);
        }
        
        private void Update()
        {
            _text.transform.position =  _playerTransform.position.normalized * (_textDistance + _innerRadius);
            // Rotate the text on z-axis, so it is pependicular to the vector from the center to the player
            Vector3 direction = _playerTransform.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            _text.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        
    }
}