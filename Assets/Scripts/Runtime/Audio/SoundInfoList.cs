using System.Collections.Generic;
using UnityEngine;

namespace Starblast.Audio
{
    [CreateAssetMenu(fileName = "SoundInfoList", menuName = "Starblast/Data/Audio/Sound Info List")]
    public class SoundInfoList : ScriptableObject
    {
        [SerializeField] private SoundInfo[] sounds;
        public SoundInfo[] Sounds => sounds;
        
        
        private Dictionary<string, SoundInfo> _soundDictionary = new Dictionary<string, SoundInfo>();
        
        private void OnEnable()
        {
            _soundDictionary.Clear();
            if (sounds == null) return;
            foreach (var sound in sounds)
            {
                if (sound != null && !string.IsNullOrEmpty(sound.Name))
                {
                    if (!_soundDictionary.TryAdd(sound.Name, sound))
                    {
                        Debug.LogWarning($"Duplicate sound name detected: {sound.Name}. This entry will be skipped.");
                    }
                }
                else
                {
                    Debug.LogWarning("Null or unnamed SoundInfo detected and skipped.");
                }
            }
        }
        
        public SoundInfo GetSoundInfo(string name)
        {
            return _soundDictionary.GetValueOrDefault(name);
        }
        
        public SoundInfo GetSoundInfo(SoundNameEnum value)
        {
            return GetSoundInfo(value.ToName());
        }
        
    }
}