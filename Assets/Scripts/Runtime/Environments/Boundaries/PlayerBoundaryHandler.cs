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
        
        private ShipController _player;
        private GameObjectRegistry _gameObjectRegistry;
        
        private ZoneType _currentZoneType;
        private bool _playerExists;
        
        private void Start()
        {
            _boundaryManager = ServiceLocator.Main.Get<IBoundaryManager>();
            _visualEffectController = GetComponentInChildren<IBoundaryVisualEffectController>();
            _audioEffectController = GetComponentInChildren<IBoundaryAudioEffectController>();
            _warningController = GetComponentInChildren<IBoundaryWarningController>();
            
            _gameObjectRegistry = ServiceLocator.Main.Get<GameObjectRegistry>();
            _gameObjectRegistry.OnRegistered += OnPlayerAdded;
            _gameObjectRegistry.OnDeRegistered += OnPlayerRemoved;
            _player = _gameObjectRegistry.Get<ShipController>();
            _playerExists = _player != null;
            Debug.Log($"[PlayerBoundaryHandler] Player exists: {_playerExists}");
        }
        
        private void OnEnable()
        {
           if (_gameObjectRegistry != null)
           {
               _gameObjectRegistry.OnRegistered -= OnPlayerAdded;
                _gameObjectRegistry.OnDeRegistered -= OnPlayerRemoved;
               _gameObjectRegistry.OnRegistered += OnPlayerAdded;
                _gameObjectRegistry.OnDeRegistered += OnPlayerRemoved;
               _player = _gameObjectRegistry.Get<ShipController>();
                _playerExists = _player != null;
           }
        }
        
        private void OnDisable()
        {
            if (_gameObjectRegistry != null)
            {
                _gameObjectRegistry.OnRegistered -= OnPlayerAdded;
                _gameObjectRegistry.OnDeRegistered -= OnPlayerRemoved;
            }
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
        
        private void OnPlayerAdded(Type type, MonoBehaviour monoBehaviour)
        {
            if (type != typeof(ShipController))
            {
                return;
            }
            _player = (ShipController)monoBehaviour;
            _playerExists = true;
            Debug.Log($"[PlayerBoundaryHandler] Player exists: {_playerExists}");
        }
        
        private void OnPlayerRemoved(Type type, MonoBehaviour monoBehaviour)
        {
            if (type != typeof(ShipController))
            {
                return;
            }
            _player = null;
            _playerExists = false;
            Debug.Log($"[PlayerBoundaryHandler] Player exists: {_playerExists}");
        }
        
        
    }
}