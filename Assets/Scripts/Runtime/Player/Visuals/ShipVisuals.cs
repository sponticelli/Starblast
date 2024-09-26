using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Starblast.Player.Visuals
{
    [AddComponentMenu("Starblast/Player/Ship Visuals")]
    public class ShipVisuals : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TrailRenderer _trailRenderer;
        
        
        private float _rotationInput;
        private float _currentRotation;
        
        private Vector3[] _trailPositions;
        
        #region cached values
        private Transform _cachedTransform;
        private float _cachedMaxAngle;
        private float _cachedRotationSpeed;
        private float _cachedReturnToZeroFactor;
        private float _cachedReturnSpeed;
        #endregion
        
        public void Initialize(ShipVisualDataSO visualData)
        {
            _cachedTransform = transform;
            _cachedMaxAngle = visualData.MaxRotationAngle;
            _cachedRotationSpeed = visualData.RotationSpeed;
            _cachedReturnToZeroFactor = visualData.ReturnToZeroFactor;
            _cachedReturnSpeed = _cachedRotationSpeed * _cachedReturnToZeroFactor; 
        }
        
        public void OnRotate(float value)
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
        
        
        
        public void OnBeforeBoundsTeleport(Vector3 position, Vector3 newPosition)
        {
            if (_trailPositions == null || _trailPositions.Length != _trailRenderer.positionCount)
                _trailPositions = new Vector3[_trailRenderer.positionCount];
            _trailRenderer.GetPositions(_trailPositions);
        }
        
        public void OnAfterBoundsTeleport(Vector3 position, Vector3 newPosition)
        {
            var deltaPosition = newPosition - position;
            for(var i=0; i<_trailPositions.Length; i++)
            {
                _trailPositions[i] += deltaPosition;
            }
            _trailRenderer.SetPositions(_trailPositions);
        }
        
        
    }
}