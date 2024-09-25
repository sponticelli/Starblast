using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace Starblast.Audio
{
    public class SoundManager : MonoBehaviour, ISoundManager
    {
        [Header("Settings")]
        [SerializeField] private int _maxSources = 10;
        [SerializeField] private AudioSource _audioSourcePrefab;
        [SerializeField] private AudioMixerGroup _sfxGroup;
        [SerializeField] private AudioMixerGroup _uiGroup;
        
        [Header("Sounds")]
        [SerializeField] private SoundInfoList _soundInfoList;
        
        private List<AudioSource> _audioSources;
        
        public bool IsInitialized { get; private set;}
        public void Initialize()
        {
            if (_audioSourcePrefab == null)
            {
                Debug.LogError("SoundManager needs an AudioSource prefab.");
                return;
            }
            
            CreateAudioSources();
            
            if (_soundInfoList == null)
            {
                Debug.LogError("SoundManager needs a SoundInfoList to Play by clipName.");
            }
            
            IsInitialized = true;
        }

        public void Play(SoundNameEnum soundEnum, Vector3 position = default, float volume = 1f, float pitch = 1f)
        {
            var soundInfo = _soundInfoList.GetSoundInfo(soundEnum);
            if (soundInfo == null)
            {
                Debug.LogWarning($"Sound '{soundEnum}' not found in the SoundInfoList.");
                return;
            }
            Play(soundInfo.Clip, soundInfo.Type, position, volume, pitch);
        }

        public void Play(string clipName, Vector3 position = default, float volume = 1f, float pitch = 1f)
        {
            var soundInfo = _soundInfoList.GetSoundInfo(clipName);
            if (soundInfo == null)
            {
                Debug.LogWarning($"Sound '{clipName}' not found in the SoundInfoList.");
                return;
            }
            Play(soundInfo.Clip, soundInfo.Type, position, volume, pitch);
        }

        public void Play(AudioClip clip, SoundType soundType = SoundType.SFX, Vector3 position = default, float volume = 1f, float pitch = 1f)
        {
            var audioSource = GetAvailableAudioSource();
            if (audioSource == null)
            {
                Debug.LogWarning("No available AudioSources to play the sound.");
                return;
            }
            audioSource.outputAudioMixerGroup = soundType == SoundType.SFX ? _sfxGroup : _uiGroup;
            audioSource.transform.position = position;
            audioSource.volume = volume;
            audioSource.pitch = pitch;
            audioSource.clip = clip;
            audioSource.loop = false;
            audioSource.Play();
        }

        public int PlayLoop(SoundNameEnum soundEnum, Vector3 position = default, float volume = 1, float pitch = 1)
        {
            var soundInfo = _soundInfoList.GetSoundInfo(soundEnum);
            if (soundInfo == null)
            {
                Debug.LogWarning($"Sound '{soundEnum}' not found in the SoundInfoList.");
                return 0;
            }
            return PlayLoop(soundInfo.Clip, soundInfo.Type, position, volume, pitch);
        }

        public int PlayLoop(string clipName, Vector3 position = default, float volume = 1, float pitch = 1)
        {
            var soundInfo = _soundInfoList.GetSoundInfo(clipName);
            if (soundInfo == null)
            {
                Debug.LogWarning($"Sound '{clipName}' not found in the SoundInfoList.");
                return 0;
            }
            return PlayLoop(soundInfo.Clip, soundInfo.Type, position, volume, pitch);
        }

        public int PlayLoop(AudioClip clip, SoundType soundType = SoundType.SFX, Vector3 position = default, float volume = 1,
            float pitch = 1)
        {
            var audioSource = GetAvailableAudioSource();
            if (audioSource == null)
            {
                Debug.LogWarning("No available AudioSources to play the sound.");
                return 0;
            }
            audioSource.outputAudioMixerGroup = soundType == SoundType.SFX ? _sfxGroup : _uiGroup;
            audioSource.transform.position = position;
            audioSource.volume = volume;
            audioSource.pitch = pitch;
            audioSource.clip = clip;
            audioSource.loop = true;
            audioSource.Play();
            return audioSource.GetInstanceID();
        }

        public void StopLoop(int id)
        {
            var audioSource = _audioSources.FirstOrDefault(source => source.GetInstanceID() == id);
            if (audioSource != null)
            {
                audioSource.Stop();
            }
        }
        
        public void Stop()
        {
            foreach (var source in _audioSources)
            {
                source.Stop();
            }
        }


        private AudioSource GetAvailableAudioSource()
        {
            return _audioSources.FirstOrDefault(audioSource => !audioSource.isPlaying);
        }

        private void CreateAudioSources()
        {
            _audioSources = new List<AudioSource>();
            
            for (int i = 0; i < _maxSources; i++)
            {
                var audioSource = Instantiate(_audioSourcePrefab, transform);
                audioSource.name = $"AudioSource_{i}";
                _audioSources.Add(audioSource);
            }
        }
    }
}