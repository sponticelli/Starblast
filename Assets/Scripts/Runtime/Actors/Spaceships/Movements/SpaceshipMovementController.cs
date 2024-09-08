using Starblast.Data.Spaceships.Bodies;
using Starblast.Data.Spaceships.Engines;
using Starblast.Inputs;
using UnityEngine;

namespace Starblast.Actors.Movements
{
    public class SpaceshipMovementController : MonoBehaviour, IVelocityProvider, ISpaceshipMovementController
    {
        private Vector2 _velocity;
        private float _currentRotation;
        
        private IRigidbody2DProvider _rbProvider;
        private IActorInputHandler _inputHandler;
        
        private float _thrustInput;
        private float _rotationInput;
        
        private ISpaceshipBodyDataProvider _spaceshipBodyDataProvider;
        private ISpaceshipEngineDataProvider _spaceshipEngineDataProvider;

        // Cache for frequently used components
        private Rigidbody2D _cachedRigidbody;
        private Transform _cachedTransform;

        public void Initialize(ISpaceshipMovementControllerContext context)
        {
            _spaceshipBodyDataProvider = context.SpaceshipBodyDataProvider;
            _spaceshipEngineDataProvider = context.SpaceshipEngineDataProvider;
            
            _rbProvider = context.Rigidbody2DProvider;
            _inputHandler = context.InputHandler;
            
            // Cache the Rigidbody2D and Transform components
            _cachedRigidbody = _rbProvider.GetRigidbody2D();
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
            ApplyDirectionalAutoBrake();
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

            _currentRotation = _rotationInput * _spaceshipBodyDataProvider.Data.RotationSpeed;
        }

        private void ApplyThrust(float input)
        {
            var accelerationThisFrame = _spaceshipEngineDataProvider.Data.Acceleration * input;
            _velocity += (Vector2)_cachedTransform.up * (accelerationThisFrame * Time.fixedDeltaTime);
        }

        private void ApplyBrake(float input)
        {
            _velocity -= _velocity.normalized * (_spaceshipEngineDataProvider.Data.BrakingForce * input * Time.fixedDeltaTime);
        }

        private void ApplyDirectionalAutoBrake()
        {
            if (!(Mathf.Abs(_thrustInput) < _spaceshipBodyDataProvider.Data.AutoBrakeThreshold)) return;
            
            Vector2 forwardDirection = _cachedTransform.up;
            var orthogonalDirection = Vector2.Perpendicular(forwardDirection);

            // Decompose velocity into forward and orthogonal components
            var forwardVelocity = Vector2.Dot(_velocity, forwardDirection) * forwardDirection;
            var orthogonalVelocity = Vector2.Dot(_velocity, orthogonalDirection) * orthogonalDirection;

            // Apply auto-brake to each component separately
            forwardVelocity *= (1f - _spaceshipBodyDataProvider.Data.ForwardAutoBrakeFactor * Time.fixedDeltaTime);
            orthogonalVelocity *= (1f - _spaceshipBodyDataProvider.Data.OrthogonalAutoBrakeFactor * Time.fixedDeltaTime);

            // Recombine the velocities
            _velocity = forwardVelocity + orthogonalVelocity;
        }

        private void ApplyMovement()
        {
            _velocity = Vector2.ClampMagnitude(_velocity, _spaceshipEngineDataProvider.Data.MaxSpeed);
            _cachedRigidbody.MovePosition(_cachedRigidbody.position + _velocity * Time.fixedDeltaTime);
        }

        private void ApplyRotation()
        {
            var engineData = _spaceshipEngineDataProvider.Data;
            var speedFactor = _velocity.magnitude / engineData.MaxSpeed;
            var maneuverability = engineData.SpeedToManeuverability.Evaluate(speedFactor);
            var rotationThisFrame = _currentRotation * maneuverability * Time.fixedDeltaTime;
            _cachedRigidbody.MoveRotation(_cachedRigidbody.rotation - rotationThisFrame);
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