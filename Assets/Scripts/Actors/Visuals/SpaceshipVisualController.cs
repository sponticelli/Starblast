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
        
        public void Initialize(ISpaceshipVisualControllerContext context)
        {
            this._context = context;
            bodyRenderer.sprite = context.VisualDataProvider.VisualData.BodySprite;
            engineRenderer.sprite = context.VisualDataProvider.VisualData.EngineSprite;
            
            StopListeningInput();
            _actorInputHandler = context.ActorInputHandler;
            StartListeningInput();
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
            float maxAngle = _context.VisualDataProvider.VisualData.MaxRotationAngle;
            float rotationSpeed = _context.VisualDataProvider.VisualData.RotationSpeed;

            if (_rotationInput != 0)
            {
                // Calculate rotation change based on input
                float rotationChange = _rotationInput * rotationSpeed * Time.deltaTime;

                // Update current rotation
                _currentRotation += rotationChange;

                // Clamp the rotation to the allowed range
                _currentRotation = Mathf.Clamp(_currentRotation, -maxAngle, maxAngle);
            }
            else
            {
                // Rotate back to 0 when there's no input
                float returnSpeed = rotationSpeed * _context.VisualDataProvider.VisualData.ReturnToZeroFactor; 
                _currentRotation = Mathf.MoveTowards(_currentRotation, 0, returnSpeed * Time.deltaTime);
            }

            // Apply the rotation only to the Y-axis
            transform.localRotation = Quaternion.Euler(0f, _currentRotation, 0f);
        }
    }
}