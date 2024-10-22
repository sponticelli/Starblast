using System;
using Starblast.Player;
using Starblast.Services;
using UnityEngine;

namespace Starblast.Environments.Boundaries
{
    public class PlayerBoundaryHandler : MonoBehaviour
    {
        private IBoundaryEffectController[] _effectControllers;
        private IBoundaryManager _boundaryManager;
        private PlayerController _player;
        
        private ZoneType _currentZoneType;
        private bool _playerExists;
        
        private void Start()
        {
            _boundaryManager = ServiceLocator.Main.Get<IBoundaryManager>();
            _effectControllers = GetComponentsInChildren<IBoundaryEffectController>();
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
            float intensity = 1f - _boundaryManager.NormalizedPositionInZone(_player.transform.position,
                _currentZoneType);
            
            foreach (var effectController in _effectControllers)
            {
                effectController.SetEffectIntensity(intensity);
            }
        }

        private void HandleZoneChange(ZoneType oldZoneType, ZoneType newZoneType)
        {
            _currentZoneType = newZoneType;
            
            foreach (var effectController in _effectControllers)
            {
                effectController.OnExitZone(oldZoneType);
                effectController.OnEnterZone(newZoneType);
            }
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