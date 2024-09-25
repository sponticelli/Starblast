using UnityEngine;
using UnityEngine.Events;

namespace Starblast.Player.Propulsion
{
    [AddComponentMenu("Starblast/Player/Ship Propulsion")]
    public class ShipPropulsion : MonoBehaviour
    {
        
        [Header("Events")]
        [SerializeField] private UnityEvent OnThrusting = null!;
        [SerializeField] private UnityEvent OnStopThrusting = null!;
        [SerializeField] private UnityEvent OnBraking = null!;
        [SerializeField] private UnityEvent OnStopBraking = null!;
        
        private Rigidbody2D _rigidbody2D;
        private PropulsionDataSO _propulsionData;
        
        private float _thrustInput;
        private float _rotationInput;
        
        private Vector2 _velocity;
        private float _currentRotation;
        private Transform _cachedTransform;
        private bool _isThrusting;
        private bool _isBraking;
        
        public delegate void VelocityChange(Vector2 newVelocity);
        public event VelocityChange OnVelocityChange;
        
        public void Initialize(Rigidbody2D rigidbody2D, PropulsionDataSO propulsionData)
        {
            _rigidbody2D = rigidbody2D;
            _propulsionData = propulsionData;
            _cachedTransform = _rigidbody2D.transform;
        }

        
        private void Update()
        {
            HandleInput();
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
                case 0:
                    _isBraking = false;
                    _isThrusting = false;
                    OnStopThrusting?.Invoke();
                    OnStopBraking?.Invoke();
                    break;
            }

            _currentRotation = _rotationInput * _propulsionData.RotationSpeed;
        }
        
        private void FixedUpdate()
        {
            ApplyMovement(Time.fixedDeltaTime);
            ApplyRotation(Time.fixedDeltaTime);
        }

        public void OnRotate(float value)
        {
            _rotationInput = value;
        }

        public void OnThrust(float value)
        {
            _thrustInput = value;
        }
        
        private void ApplyThrust(float input)
        {
            var accelerationThisFrame = _propulsionData.Acceleration * input;
            _velocity += (Vector2)_cachedTransform.up * (accelerationThisFrame * Time.fixedDeltaTime);
            if (!_isThrusting) OnThrusting?.Invoke();
            _isThrusting = true;
            _isBraking = false;
        }

        private void ApplyBrake(float input)
        {
            _velocity -= _velocity.normalized * (_propulsionData.BrakingForce * input * Time.fixedDeltaTime);
            if (!_isBraking) OnBraking?.Invoke();
            _isBraking = true;
            _isThrusting = false;
        }
        
        #region Physic Calculations
        public Vector2 CalculateVelocity(Vector2 currentVelocity, float thrustInput, Vector2 forwardDirection, float deltaTime)
        {
            Vector2 newVelocity = currentVelocity;

            if (thrustInput > 0)
            {
                newVelocity += ApplyThrust(thrustInput, forwardDirection, deltaTime);
            }
            else if (thrustInput < 0)
            {
                newVelocity += ApplyBrake(newVelocity, -thrustInput, deltaTime);
            }

            newVelocity = ApplyDirectionalAutoBrake(newVelocity, thrustInput, forwardDirection, deltaTime);
            return Vector2.ClampMagnitude(newVelocity, _propulsionData.MaxSpeed);
        }

        public float CalculateRotation(float rotationInput, float currentSpeed, float deltaTime)
        {
            var speedFactor = currentSpeed / _propulsionData.MaxSpeed;
            var maneuverability = _propulsionData.SpeedToManeuverability.Evaluate(speedFactor);
            return -rotationInput * _propulsionData.RotationSpeed * maneuverability * deltaTime;
        }

        private Vector2 ApplyThrust(float input, Vector2 forwardDirection, float deltaTime)
        {
            var accelerationThisFrame = _propulsionData.Acceleration * input;
            return forwardDirection * (accelerationThisFrame * deltaTime);
        }

        private Vector2 ApplyBrake(Vector2 currentVelocity, float input, float deltaTime)
        {
            return -currentVelocity.normalized * (_propulsionData.BrakingForce * input * deltaTime);
        }

        private Vector2 ApplyDirectionalAutoBrake(Vector2 currentVelocity, float thrustInput, Vector2 forwardDirection, float deltaTime)
        {
            if (Mathf.Abs(thrustInput) >= _propulsionData.AutoBrakeThreshold)
            {
                return currentVelocity;
            }

            var orthogonalDirection = Vector2.Perpendicular(forwardDirection);

            var forwardVelocity = Vector2.Dot(currentVelocity, forwardDirection) * forwardDirection;
            var orthogonalVelocity = Vector2.Dot(currentVelocity, orthogonalDirection) * orthogonalDirection;

            forwardVelocity *= (1f - _propulsionData.ForwardAutoBrakeFactor * deltaTime);
            orthogonalVelocity *= (1f - _propulsionData.OrthogonalAutoBrakeFactor * deltaTime);

            return forwardVelocity + orthogonalVelocity;
        }
    
        #endregion
        
        # region Apply Physics
        private void ApplyMovement(float deltaTime)
        {
            _velocity = CalculateVelocity(_velocity, _thrustInput, _cachedTransform.up, deltaTime);
            OnVelocityChange?.Invoke(_velocity);
            _rigidbody2D.MovePosition(_rigidbody2D.position + _velocity * deltaTime);
        }

       

        private void ApplyRotation(float deltaTime)
        {
            float rotationThisFrame = CalculateRotation(_rotationInput, _velocity.magnitude, deltaTime);
            _rigidbody2D.MoveRotation(_rigidbody2D.rotation + rotationThisFrame);
        }
        # endregion
    }
}