using UnityEngine;

namespace Starblast.Audio
{
    public interface IMusicManager : IInitializable
    {
        void SetPlaylist(MusicPlaylist newPlaylist);
        void PlayNext();
        void PlayPrevious();
        void PlayTrack(int trackIndex);
        void PlayTrack(string trackName);
        void PlayTrack(AudioClip clip);
        void Stop();
        void Pause();
        void Resume();
        void SetVolume(float volume);
    }
}