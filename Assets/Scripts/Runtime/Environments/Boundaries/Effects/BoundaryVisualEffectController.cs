using System;
using Starblast.Managers.Starfields;
using Starblast.Services;
using UnityEngine;

namespace Starblast.Environments.Boundaries
{
    public class BoundaryVisualEffectController : MonoBehaviour, IBoundaryVisualEffectController
    {
        [Serializable]
        public class ZoneSettings
        {
            public ZoneType ZoneType;
            public float  MinStarLuminosity = 1f;
            public float  MaxStarLuminosity = 1f;
        }

        [Header("Settings")]
        [SerializeField] private ZoneSettings[] _zoneSettings;
        
        private StarfieldManager _starfieldManager;
        private ZoneSettings _currentZoneSettings;
        
        
        private void Start()
        {
            _starfieldManager = ServiceLocator.Main.Get<StarfieldManager>();
            _currentZoneSettings = GetZoneSettings(ZoneType.SafeZone);
        }
        
        public void ResetEffects()
        {
            _starfieldManager.SetLuminosity(1f);
        }

        public void OnEnterZone(ZoneType zone)
        {
            _currentZoneSettings = GetZoneSettings(zone);
        }

        public void OnExitZone(ZoneType zone)
        {
            
        }

        public void SetEffectIntensity(float intensity)
        {
            if (_currentZoneSettings != null) 
            {
                var luminosity = _currentZoneSettings.MinStarLuminosity + 
                                 (_currentZoneSettings.MaxStarLuminosity - _currentZoneSettings.MinStarLuminosity) * intensity;
                _starfieldManager.SetLuminosity(luminosity);
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