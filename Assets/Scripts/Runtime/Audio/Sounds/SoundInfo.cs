using System;
using UnityEngine;

namespace Starblast.Audio
{
    [Serializable]
    public class SoundInfo
    {
        [SerializeField] private string _name;
        [SerializeField] private AudioClip _clip;
        [SerializeField] private SoundType _type;
        
        public string Name => _name;
        public AudioClip Clip => _clip;
        public SoundType Type => _type;
    }
}