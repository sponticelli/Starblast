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
        private IBoundaryManager _boundaryManager;
        private BoundaryVisualEffectController.ZoneSettings _currentZoneSettings;

        public Vector3 Target { set; get; }
        
        public void SetBoundaryManagerAndZoneSettings(IBoundaryManager boundaryManager, BoundaryVisualEffectController.ZoneSettings zoneSettings)
        {
            _boundaryManager = boundaryManager;
            _currentZoneSettings = zoneSettings;
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
            // Rotate 2d towards player
            if (_player != null)
            {
                HandleBoundaryMovement();
                RotateTowardsPlayer();
            }
        }

        private void HandleBoundaryMovement()
        {
            if (_currentZoneSettings.KrakenSettings.IsEnabled)
            {
                var krakenPosition = transform.position;
                var distance = krakenPosition.magnitude;
                var zoneRadius = _boundaryManager.GetZoneRadius(_currentZoneSettings.ZoneType);

                if (distance > zoneRadius)
                {
                    Target = _player.transform.position;
                }
                else
                {
                    var target = transform.position;
                    target += krakenPosition.normalized * zoneRadius;
                    Target = target;
                }
            }
            else
            {
                if ((transform.position - Target).magnitude < 0.1f)
                {
                    gameObject.SetActive(false);
                }
            }

            MoveTowardsTarget();
        }
        
        private void MoveTowardsTarget()
        {
            if (Target == null) return;

            Vector3 direction = Target - transform.position;
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