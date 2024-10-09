using Starblast.Services;
using UnityEngine;

namespace Starblast.Environments.Boundaries
{
    public class PlayerBoundaryHandler : MonoBehaviour
    {
        private IBoundaryVisualEffectController _visualEffectController;
        private IBoundaryAudioEffectController _audioEffectController;
        private IBoundaryWarningSystem _warningSystem;
        
        private IBoundaryManager _boundaryManager;
        
        private void Start()
        {
            _boundaryManager = ServiceLocator.Main.Get<IBoundaryManager>();
            _visualEffectController = GetComponentInChildren<IBoundaryVisualEffectController>();
            _audioEffectController = GetComponentInChildren<IBoundaryAudioEffectController>();
            _warningSystem = GetComponentInChildren<IBoundaryWarningSystem>();
        }
    }
}