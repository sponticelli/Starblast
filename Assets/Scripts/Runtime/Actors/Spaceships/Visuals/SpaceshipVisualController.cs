using Starblast.Inputs;
using UnityEngine;

namespace Starblast.Actors.Visuals
{
    public class SpaceshipVisualController : MonoBehaviour, ISpaceshipVisualController
    {
        [Header("References")]
        [SerializeField] private SpriteRenderer bodyRenderer;
        [SerializeField] private SpriteRenderer engineRenderer;
        
        private ISpaceshipVisualControllerContext _context;
        private IActorInputHandler _actorInputHandler;
        
        private float _currentRotation;
        private float _rotationInput;
        
        #region cached values
        private Transform _cachedTransform;
        private float _cachedMaxAngle;
        private float _cachedRotationSpeed;
        private float _cachedReturnToZeroFactor;
        private float _cachedReturnSpeed;
        #endregion
        
        public void Initialize(ISpaceshipVisualControllerContext context)
        {
            this._context = context;
            bodyRenderer.sprite = context.VisualDataProvider.Data.BodySprite;
            engineRenderer.sprite = context.VisualDataProvider.Data.EngineSprite;
            
            StopListeningInput();
            _actorInputHandler = context.ActorInputHandler;
            StartListeningInput();
            
            _cachedTransform = transform;
            _cachedMaxAngle = context.VisualDataProvider.Data.MaxRotationAngle;
            _cachedRotationSpeed = context.VisualDataProvider.Data.RotationSpeed;
            _cachedReturnToZeroFactor = context.VisualDataProvider.Data.ReturnToZeroFactor;
            _cachedReturnSpeed = _cachedRotationSpeed * _cachedReturnToZeroFactor; 
        }
        
        private void OnEnable()
        {
            StartListeningInput();
        }
        
        private void OnDisable()
        {
            StopListeningInput();
        }
        
        private void StartListeningInput()
        {
            if (_actorInputHandler == null) return;
            _actorInputHandler.OnRotate += OnRotate;
        }
        
        private void StopListeningInput()
        {
            if (_actorInputHandler == null) return;
            _actorInputHandler.OnRotate -= OnRotate;
        }

        private void OnRotate(float value)
        {
            _rotationInput = value;
        }
        
        private void Update()
        {
            HandleRotation();
        }

        private void HandleRotation()
        {
            if (_rotationInput != 0)
            {
                // Calculate rotation change based on input
                float rotationChange = _rotationInput * _cachedRotationSpeed * Time.deltaTime;

                // Update current rotation
                _currentRotation += rotationChange;

                // Clamp the rotation to the allowed range
                _currentRotation = Mathf.Clamp(_currentRotation, -_cachedMaxAngle, _cachedMaxAngle);
            }
            else
            {
                // Rotate back to 0 when there's no input
                _currentRotation = Mathf.MoveTowards(_currentRotation, 0, _cachedReturnSpeed * Time.deltaTime);
            }

            // Apply the rotation only to the Y-axis
            _cachedTransform.localRotation = Quaternion.Euler(0f, _currentRotation, 0f);
        }
    }
}