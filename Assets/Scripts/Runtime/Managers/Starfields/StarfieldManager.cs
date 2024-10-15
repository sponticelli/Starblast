using Starblast.Services;
using UnityEngine;

namespace Starblast.Managers.Starfields
{
    public class StarfieldManager : MonoBehaviour, IInitializable
    {
        [Header("Settings")] 
        [SerializeField] private StarfieldSettings[] _settings;
        [SerializeField] private StarfieldController _starfieldPrefab;

        private StarfieldController[] _starfieldControllers;

        public void SetLuminosity(float luminosity)
        {
            foreach (var starfield in _starfieldControllers)
            {
                starfield.SetColor(starfield.Settings.Color * luminosity);
            }
        }
        
        
        private void Awake()
        {
            Initialize();
        }


        public bool IsInitialized { get; private set; }

        public void Initialize()
        {
            if (IsInitialized) return;
            CreateStarfields();
            IsInitialized = true;
        }
        
        private void CreateStarfields()
        {
            _starfieldControllers = new StarfieldController[_settings.Length];
            for (var i = 0; i < _settings.Length; i++)
            {
                var starfield = Instantiate(_starfieldPrefab, transform);
                starfield.Settings = _settings[i];
                _starfieldControllers[i] = starfield;
                var renderer = starfield.GetComponent<SpriteRenderer>();
                renderer.sortingOrder = i;
            }
            
        }
    }
}