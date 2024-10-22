using System;
using Starblast.Entities.Krakens;
using Starblast.Extensions;
using Starblast.Player;
using Starblast.Services;
using UnityEngine;
 
namespace Starblast.Environments.Boundaries
{
    public class BoundaryEnemyEffectController : MonoBehaviour, IBoundaryEnemyEffectController
    {
        [Serializable]
        public class KrakenSettings
        {
            public bool IsEnabled;
            public float RadiusOffset;
        }

        [Serializable]
        public class ZoneSettings
        {
            public ZoneType ZoneType;
            public KrakenSettings KrakenSettings;
        }
        
        [Header("Settings")] 
        [SerializeField] private ZoneSettings[] _zoneSettings;

        private ZoneSettings _currentZoneSettings = null;
        private KrakenController _kraken;
        private IBoundaryManager _boundaryManager;
        private PlayerController _player;
        
        private void Start()
        {
            _boundaryManager = ServiceLocator.Main.Get<IBoundaryManager>();
            _currentZoneSettings = null;
        }
        
        public void ResetEffects()
        {
            
        }

        public void OnEnterZone(ZoneType zone)
        {
            Debug.Log($"Enter: {zone}");
            _currentZoneSettings = GetZoneSettings(zone);
            EnableKraken();
        }

        public void OnExitZone(ZoneType zone)
        {

        }
        
        public void OnKrakenAdded(KrakenController kraken)
        {
            _kraken = kraken;
            _kraken.gameObject.SetActive(false);
            EnableKraken();
        }

        public void OnKrakenRemoved(KrakenController kraken)
        {
            _kraken = null;
        }
        
        public void OnPlayerRemoved(PlayerController player)
        {
            _player = null;
        }

        public void OnPlayerAdded(PlayerController player)
        {
            _player = player;
        }

        public void SetEffectIntensity(float intensity)
        {
            
        }
        
        private ZoneSettings GetZoneSettings(ZoneType zone)
        {
            foreach (var zoneSetting in _zoneSettings)
            {
                if (zoneSetting.ZoneType == zone)
                {
                    return zoneSetting;
                }
            }

            return null;
        }
        
        private void EnableKraken()
        {
            if (_kraken == null) return;
            
            if (_currentZoneSettings==null)
            {
                _currentZoneSettings = GetZoneSettings(ZoneType.SafeZone);
                _kraken.gameObject.SetActive(false);
                Debug.Log("Kraken disabled".Color(Color.green));
            }
            
            if (_currentZoneSettings.KrakenSettings.IsEnabled)
            {
                if (_kraken.gameObject.activeSelf == false)
                {
                    var direction = _player.transform.position.normalized * 
                                    (_currentZoneSettings.KrakenSettings.RadiusOffset + 
                                     _boundaryManager.GetZoneRadius(_currentZoneSettings.ZoneType));
                    _kraken.transform.position = direction;
                    _kraken.gameObject.SetActive(true);
                }
            }
                
            var isKrakenEnabled = _currentZoneSettings.KrakenSettings.IsEnabled;
            if (_boundaryManager == null) _boundaryManager = ServiceLocator.Main.Get<IBoundaryManager>();
            var zoneRadius = _boundaryManager.GetZoneRadius(_currentZoneSettings.ZoneType);
            
            Debug.Log($"Kraken: {isKrakenEnabled}, {_currentZoneSettings.ZoneType} {zoneRadius}".Color(Color.green));
            _kraken.SetBoundaryManagerAndZoneSettings(isKrakenEnabled, zoneRadius);
        }
    }
}