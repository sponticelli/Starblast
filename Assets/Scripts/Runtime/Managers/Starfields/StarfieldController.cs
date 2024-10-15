using UnityEngine;

namespace Starblast.Managers.Starfields
{
    public class StarfieldController : MonoBehaviour
    {
        [Header("Settings")] 
        [SerializeField] private StarfieldSettings _settings;

        [Header("References")]
        [SerializeField] private Renderer _renderer;

        private Material _material;
        private static readonly int ColorKey = Shader.PropertyToID("_Color");
        private static readonly int SpeedKey = Shader.PropertyToID("_Speed");
        private static readonly int SizeKey = Shader.PropertyToID("_Size");
        private static readonly int DensityKey = Shader.PropertyToID("_Density");
        private static readonly int OffsetKey = Shader.PropertyToID("_Offset");

        public StarfieldSettings Settings
        {
            get => _settings;
            set
            {
                _settings.CopyFrom(value);
                
                ApplySettings();
            }
        }
        
        public void SetColor(Color color)
        {
            _material.SetColor(ColorKey, color);
        }

        private void Awake()
        {
            if (_renderer == null)
                _renderer = GetComponent<Renderer>();
            _material = _renderer.material;
        }

        private void Start()
        {
            ApplySettings();
        }

        private void ApplySettings()
        {
            _material.SetFloat(SpeedKey, _settings.Speed);
            _material.SetFloat(SizeKey, _settings.Size);
            _material.SetFloat(DensityKey, _settings.Density);
            _material.SetColor(ColorKey, _settings.Color);
            _material.SetFloat(OffsetKey, _settings.Offset);
        }
    }
}