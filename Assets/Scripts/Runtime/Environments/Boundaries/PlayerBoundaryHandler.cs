using System;
using Starblast.Player;
using Starblast.Services;
using UnityEngine;

namespace Starblast.Environments.Boundaries
{
    public class PlayerBoundaryHandler : MonoBehaviour
    {
        private IBoundaryVisualEffectController _visualEffectController;
        private IBoundaryAudioEffectController _audioEffectController;
        private IBoundaryWarningController _warningController;
        
        private IBoundaryManager _boundaryManager;
        
        private PlayerController _player;

        
        private ZoneType _currentZoneType;
        private bool _playerExists;
        
        private void Start()
        {
            _boundaryManager = ServiceLocator.Main.Get<IBoundaryManager>();
            _visualEffectController = GetComponentInChildren<IBoundaryVisualEffectController>();
            _audioEffectController = GetComponentInChildren<IBoundaryAudioEffectController>();
            _warningController = GetComponentInChildren<IBoundaryWarningController>();
        }
        
        private void Update()
        {
            if (_playerExists == false)
            {
                return;
            }
            
            ZoneType newZoneType = _boundaryManager.GetZoneType(_player.transform.position);
            if (newZoneType != _currentZoneType)
            {
                
                HandleZoneChange(_currentZoneType, newZoneType);
            }
            
            HandleZoneEffects();
        }

        
        private void HandleZoneEffects()
        {
            float intensity = 1f - _boundaryManager.NormalizedPositionInZone(_player.transform.position, _currentZoneType);
            
            _visualEffectController.SetEffectIntensity(intensity);
            _audioEffectController.SetEffectIntensity(intensity);
            _warningController.SetEffectIntensity(intensity);
        }

        private void HandleZoneChange(ZoneType oldZoneType, ZoneType newZoneType)
        {
            _currentZoneType = newZoneType;
            
            _visualEffectController.OnExitZone(oldZoneType);
            _audioEffectController.OnExitZone(oldZoneType);
            _warningController.OnExitZone(oldZoneType);
            
            _visualEffectController.OnEnterZone(newZoneType);
            _audioEffectController.OnEnterZone(newZoneType);
            _warningController.OnEnterZone(newZoneType);
            
        }
        
        public void OnPlayerAdded(PlayerController player)
        {
            _player = player;
            _playerExists = true;
        }

        
        public void OnPlayerRemoved(PlayerController player)
        {
            _player = null;
            _playerExists = false;
        }
        
        
    }
}