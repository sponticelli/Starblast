using Starblast.Services;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Starblast.Managers.Volumes
{
    [DefaultExecutionOrder(ExecutionOrder.Services)]
    public class VolumeController : MonoBehaviour, IInitializable
    {
        [SerializeField] private Volume _volume;
        public bool IsInitialized { get; private set; }

        private ColorAdjustments _colorAdjustments;
        private Vignette _vignette;
        
        public void Start()
        {
            Initialize();
        }
        
        public void Initialize()
        {
            IsInitialized = true;
            _volume.profile.TryGet<ColorAdjustments>(out _colorAdjustments);
            _volume.profile.TryGet<Vignette>(out _vignette);
        }
        
        public void SetSaturation(float value)
        {
            if (_colorAdjustments != null) _colorAdjustments.saturation.value = value;
        }

        public void SetVignetteIntensity(float value)
        {
            if (_vignette != null) _vignette.intensity.value = value;
        }
    }
}