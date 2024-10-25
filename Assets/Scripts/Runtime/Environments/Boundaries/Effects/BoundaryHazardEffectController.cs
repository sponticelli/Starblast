using System;
using Starblast.Entities.Krakens;
using Starblast.Entities.MeteorFalls;
using Starblast.Extensions;
using Starblast.Player;
using Starblast.Services;
using Starblast.Utils;
using UnityEngine;
 
namespace Starblast.Environments.Boundaries
{
    public class BoundaryHazardEffectController : MonoBehaviour, IBoundaryEnemyEffectController
    {
        [Serializable]
        public class KrakenSettings
        {
            public bool IsEnabled;
            public float RadiusOffset;
        }
        
        [Serializable]
        public class MeteorFallSettings
        {
            public bool IsEnabled;
            public float _distanceFromPlayer;
            public float _frequency;
            public FloatRange _speed;
            public FloatRange _lifeTime;
            public FloatRange _quantity;
        }

        [Serializable]
        public class ZoneSettings
        {
            public ZoneType ZoneType;
            public KrakenSettings KrakenSettings;
            public MeteorFallSettings MeteorFallSettings;
        }
        
        [Header("Settings")] 
        [SerializeField] private ZoneSettings[] _zoneSettings;

        private ZoneSettings _currentZoneSettings = null;
        private KrakenController _kraken;
        private MeteorFallManager _meteorFall;
        private IBoundaryManager _boundaryManager;
        private PlayerController _player;
        
        private float _meteorTimer;
        
        private void Start()
        {
            _boundaryManager = ServiceLocator.Main.Get<IBoundaryManager>();
            _meteorFall = ServiceLocator.Main.Get<MeteorFallManager>();
            _currentZoneSettings = null;
        }


        private void Update()
        {
            if (_currentZoneSettings == null) return;
            UpdateMeteorFall();
        }

        private void UpdateMeteorFall()
        {
            if (_currentZoneSettings.MeteorFallSettings.IsEnabled)
            {
                _meteorTimer += Time.deltaTime;
                if (_meteorTimer >= _currentZoneSettings.MeteorFallSettings._frequency)
                {
                    _meteorTimer = 0;
                    var playerPosition = _player.transform.position;
                    var position = playerPosition + playerPosition.normalized * _currentZoneSettings.MeteorFallSettings._distanceFromPlayer;
                    var direction = (playerPosition - position).normalized;
                    _meteorFall.Spawn(position, direction, 
                        _currentZoneSettings.MeteorFallSettings._speed, 
                        _currentZoneSettings.MeteorFallSettings._lifeTime, 
                        _currentZoneSettings.MeteorFallSettings._quantity);
                }
            }
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