using Starblast.Actors.Spaceships.Movements;
using Starblast.Data.Spaceships.Bodies;
using Starblast.Data.Spaceships.Engines;
using Starblast.Inputs;
using TMPro.EditorUtilities;
using UnityEngine;

namespace Starblast.Actors.Movements
{
    public class SpaceshipMovementController : MonoBehaviour, IVelocityProvider, ISpaceshipMovementController
    {
        private Vector2 _velocity;
        private float _currentRotation;
        
        private IActorInputHandler _inputHandler;
        
        private float _thrustInput;
        private float _rotationInput;
        
        private ISpaceshipBodyData _spaceshipBodyData;
        private ISpaceshipEngineData _spaceshipEngineData;

        // Cache for frequently used components
        private Rigidbody2D _cachedRigidbody;
        private Transform _cachedTransform;
        
        private IMovementCalculator _spaceshipMovementCalculator;

        public void Initialize(ISpaceshipMovementControllerContext context)
        {
            _spaceshipMovementCalculator = new SpaceshipMovementCalculator(context.SpaceshipBodyData, context.SpaceshipEngineData);
            
            _spaceshipBodyData = context.SpaceshipBodyData;
            _spaceshipEngineData = context.SpaceshipEngineData;
            
            _inputHandler = context.InputHandler;
            
            // Cache the Rigidbody2D and Transform components
            _cachedRigidbody = context.Rigidbody2D;
            _cachedTransform = transform;

            StopListeningToInput();
            _inputHandler = context.InputHandler;
            ListenToInput();
        }
        
        private void OnEnable()
        {
            ListenToInput();
        }
        
        private void OnDisable()
        {
            StopListeningToInput();
        }
        
        public Vector2 GetVelocity()
        {
            return _velocity;
        }
        
        private void Update()
        {
            HandleInput();
        }

        private void FixedUpdate()
        {
            ApplyMovement();
            ApplyRotation();
        }
        
        private void HandleInput()
        {
            switch (_thrustInput)
            {
                case > 0:
                    ApplyThrust(_thrustInput);
                    break;
                case < 0:
                    ApplyBrake(-_thrustInput);
                    break;
            }

            _currentRotation = _rotationInput * _spaceshipBodyData.RotationSpeed;
        }

        private void ApplyThrust(float input)
        {
            var accelerationThisFrame = _spaceshipEngineData.Acceleration * input;
            _velocity += (Vector2)_cachedTransform.up * (accelerationThisFrame * Time.fixedDeltaTime);
        }

        private void ApplyBrake(float input)
        {
            _velocity -= _velocity.normalized * (_spaceshipEngineData.BrakingForce * input * Time.fixedDeltaTime);
        }

        private void ApplyMovement()
        {
            _velocity = _spaceshipMovementCalculator.CalculateVelocity(_velocity, _thrustInput, _cachedTransform.up, Time.fixedDeltaTime);
            _cachedRigidbody.MovePosition(_cachedRigidbody.position + _velocity * Time.fixedDeltaTime);
        }

        private void ApplyRotation()
        {
            float rotationThisFrame = _spaceshipMovementCalculator.CalculateRotation(_rotationInput, _velocity.magnitude, Time.fixedDeltaTime);
            _cachedRigidbody.MoveRotation(_cachedRigidbody.rotation + rotationThisFrame);
        }
        
        #region Input Handling
        private InputEvent<float> OnRotate
        {
            get { return value => _rotationInput = value; }
        }
        
        private InputEvent<float> OnThrust
        {
            get { return value => _thrustInput = value; }
        }
        
        private void ListenToInput()
        {
            if (_inputHandler == null) return;
            _inputHandler.OnRotate += OnRotate;
            _inputHandler.OnThrust += OnThrust;
        }

        private void StopListeningToInput()
        {
            if (_inputHandler == null) return;
            _inputHandler.OnRotate -= OnRotate;
            _inputHandler.OnThrust -= OnThrust;
        }
        #endregion
    }
}