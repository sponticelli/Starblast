using System;
using System.Collections.Generic;
using Starblast.Entities.Tentacles;
using Starblast.Environments.Boundaries;
using Starblast.Player;
using UnityEngine;

namespace Starblast.Entities.Krakens
{
    public class KrakenController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private List<Tentacle> tentacles = new List<Tentacle>();
        
        [Header("Settings")]
        [SerializeField] private float _attackRange = 10f;
        [SerializeField] private float _movementSpeed = 2f;
        [SerializeField] private float _rotationSpeed = 2f;
        
        public float MovementSpeed => _movementSpeed;
        public float RotationSpeed => _rotationSpeed;
        
        private PlayerController _player;
        
        private bool _tentaclesActive = true;

        private bool _isEnabled;
        private float _zoneRadius;

        private Vector3 _target;
        
        public void SetBoundaryManagerAndZoneSettings(
            bool krakenSettingsIsEnabled,
            float getZoneRadius)
        {
            _isEnabled = krakenSettingsIsEnabled;
            _zoneRadius = getZoneRadius;
        }

        public void OnPlayerAdded(PlayerController player)
        {
            _player = player;
            Rigidbody2D playerRigidbody = player.GetComponent<Rigidbody2D>();
            foreach (var tentacle in tentacles)
            {
                tentacle.TargetRigidbody = playerRigidbody;
            }
        }
        
        private void SwitchTentacles(bool active)
        {
            if (_tentaclesActive == active) return;
            
            foreach (var tentacle in tentacles)
            {
                tentacle.gameObject.SetActive(active);
            }
            _tentaclesActive = active;
        }

        private void Update()
        {
            if (_player == null) return;
            HandleBoundaryMovement();
            RotateTowardsPlayer();
        }

        private void HandleBoundaryMovement()
        {
            if (_isEnabled)
            {
                var krakenPosition = transform.position;
                var distance = krakenPosition.magnitude;
                
                if (distance > _zoneRadius)
                {
                    _target = _player.transform.position;
                }
                else
                {
                    var target = transform.position;
                    target += krakenPosition.normalized * _zoneRadius;
                    _target = target;
                }
            }
            else
            {
                if ((transform.position - _target).magnitude < 0.1f)
                {
                    gameObject.SetActive(false);
                }
            }

            MoveTowardsTarget();
        }
        
        private void MoveTowardsTarget()
        {
            Vector3 direction = _target - transform.position;
            transform.position += direction.normalized * (_movementSpeed * Time.deltaTime);
        }

        private void RotateTowardsPlayer()
        {
            Vector3 direction = _player.transform.position - transform.position;
            float angle = 90f + Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                
            // Check if player is in range
            SwitchTentacles(direction.magnitude <= _attackRange);
        }

      
    }
}