using UnityEngine;

namespace Starblast.Audio
{
    public interface ISoundManager : IInitializable
    {
        void Play(SoundNameEnum soundEnum, Vector3 position = default, float volume = 1f, float pitch = 1f);
        void Play(string clipName, Vector3 position = default, float volume = 1f, float pitch = 1f);
        void Play(AudioClip clip, SoundType soundType = SoundType.SFX, Vector3 position = default, float volume = 1f, float pitch = 1f);
    }
}