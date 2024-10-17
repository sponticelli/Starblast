using Starblast.Services;
using UnityEngine;

namespace Starblast.Managers.Volumes
{
    public class VolumeControllerRegisterer : ATransientServiceRegisterer<VolumeController>
    {
        [SerializeField] private VolumeController _service;
        
        public override VolumeController Service => _service;
    }
}