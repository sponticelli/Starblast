using System;
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
        public class ZoneSettings
        {
            public ZoneType ZoneType;
            public FloatRange StarLuminosity;
            public IntRange ColorSaturation;
            public FloatRange VignetteIntensity;
        }

        [Header("Settings")] 
        [SerializeField] private ZoneSettings[] _zoneSettings;

        private StarfieldManager _starfieldManager;
        private VolumeController _volumeManager;
        private ZoneSettings _currentZoneSettings;


        private void Start()
        {
            _starfieldManager = ServiceLocator.Main.Get<StarfieldManager>();
            _volumeManager = ServiceLocator.Main.Get<VolumeController>();
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
            _currentZoneSettings = GetZoneSettings(zone);
        }

        public void OnExitZone(ZoneType zone)
        {

        }

        
        private ZoneSettings _previousZoneSettings;
        public void SetEffectIntensity(float intensity)
        {
            if (_currentZoneSettings == null) return;
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
            
            if (_currentZoneSettings != _previousZoneSettings)
            {
                Debug.Log($"Visual Enter: {_currentZoneSettings.ZoneType} Leave: {_previousZoneSettings?.ZoneType}");
                _previousZoneSettings = _currentZoneSettings;
                Debug.Log($"Luminosity: {luminosity}");
                Debug.Log($"Saturation: {saturation}");
                Debug.Log($"Vignette: {vignetteIntensity}");
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
    }
}