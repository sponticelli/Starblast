using System;
using System.Collections.Generic;
using Starblast.Entities.Tentacles;
using Starblast.Player;
using Starblast.Services;
using UnityEngine;

namespace Starblast.Entities.Krakens
{
    public class Kraken : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private List<Tentacle> tentacles = new List<Tentacle>();
        
        [Header("Settings")]
        [SerializeField] private float _attackRange = 10f;
        
        private PlayerController _player;
        
        private bool _tentaclesActive = true; 

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
                Vector3 direction = _player.transform.position - transform.position;
                float angle = 90f + Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                
                // Check if player is in range
                if (direction.magnitude <= _attackRange)
                {
                    SwitchTentacles(true);
                }
                else
                {
                    SwitchTentacles(false);
                }
            }
        }
    }
}