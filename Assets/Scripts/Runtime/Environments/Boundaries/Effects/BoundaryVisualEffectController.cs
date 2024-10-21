using System;
using Starblast.Entities.Krakens;
using Starblast.Extensions;
using Starblast.Managers.Starfields;
using Starblast.Managers.Volumes;
using Starblast.Player;
using Starblast.Services;
using Starblast.Utils;
using UnityEngine;


namespace Starblast.Environments.Boundaries
{
    public class BoundaryVisualEffectController : MonoBehaviour, IBoundaryVisualEffectController
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
            public FloatRange StarLuminosity;
            public IntRange ColorSaturation;
            public FloatRange VignetteIntensity;
            public KrakenSettings KrakenSettings;
        }

        [Header("Settings")] [SerializeField] private ZoneSettings[] _zoneSettings;

        private StarfieldManager _starfieldManager;
        private VolumeController _volumeManager;
        private ZoneSettings _currentZoneSettings;
        private ZoneSettings _previousZoneSettings;
        private KrakenController _kraken;
        private IBoundaryManager _boundaryManager;
        private PlayerController _player;


        private void Start()
        {
            _starfieldManager = ServiceLocator.Main.Get<StarfieldManager>();
            _volumeManager = ServiceLocator.Main.Get<VolumeController>();
            _boundaryManager = ServiceLocator.Main.Get<IBoundaryManager>();
            _currentZoneSettings = GetZoneSettings(ZoneType.SafeZone);
        }

        public void ResetEffects()
        {
            ZoneSettings safeZoneSettings = GetZoneSettings(ZoneType.SafeZone);
            _starfieldManager.SetLuminosity(safeZoneSettings.StarLuminosity.Max);
            _volumeManager.SetSaturation(safeZoneSettings.ColorSaturation.Max);
            _volumeManager.SetVignetteIntensity(safeZoneSettings.VignetteIntensity.Max);
        }

        public void OnEnterZone(ZoneType zone)
        {
            Debug.Log($"Enter: {zone}");
            _currentZoneSettings = GetZoneSettings(zone);
            EnableKraken();
        }

        public void OnExitZone(ZoneType zone)
        {
            Debug.Log($"Exit: {zone}");
            _previousZoneSettings = _currentZoneSettings;
        }

        public void OnKrakenAdded(KrakenController kraken)
        {
            _kraken = kraken;
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
            if (_currentZoneSettings != null)
            {
                var luminosity = _currentZoneSettings.StarLuminosity.Min +
                                 (_currentZoneSettings.StarLuminosity.Max -
                                  _currentZoneSettings.StarLuminosity.Min) * intensity;
                _starfieldManager.SetLuminosity(luminosity);

                var saturation = _currentZoneSettings.ColorSaturation.Min +
                                 (_currentZoneSettings.ColorSaturation.Max -
                                  _currentZoneSettings.ColorSaturation.Min) * intensity;
                _volumeManager.SetSaturation(saturation);

                var vignetteIntensity = _currentZoneSettings.VignetteIntensity.Min +
                                        (_currentZoneSettings.VignetteIntensity.Max -
                                         _currentZoneSettings.VignetteIntensity.Min) * intensity;
                _volumeManager.SetVignetteIntensity(vignetteIntensity);
            }
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
            if (_kraken != null)
            {
                if (_currentZoneSettings==null)
                {
                    _currentZoneSettings = GetZoneSettings(ZoneType.SafeZone);
                    _kraken.gameObject.SetActive(false);
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
                _kraken.SetBoundaryManagerAndZoneSettings(_boundaryManager, _currentZoneSettings);
            }
        }

 
    }
}