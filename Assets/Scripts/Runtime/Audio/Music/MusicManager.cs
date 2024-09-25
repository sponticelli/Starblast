using System.Collections;
using UnityEngine;

namespace Starblast.Audio
{
    public class MusicManager : MonoBehaviour, IMusicManager
    {
        [SerializeField] private AudioSource[] audioSources;
        [SerializeField] private float crossfadeDuration = 2f;
        [SerializeField] private MusicPlaylist playlist;

        private int currentTrackIndex = -1;
        private int activeSourceIndex = 0;
        private Coroutine fadeCoroutine;
        
        public bool IsInitialized { get; private set; }

        public void Initialize()
        {
            if (audioSources.Length < 2)
            {
                Debug.LogError("MusicManager needs at least 2 AudioSources for crossfading.");
                return;
            }
            IsInitialized = true;
        }

        public void SetPlaylist(MusicPlaylist newPlaylist)
        {
            playlist = newPlaylist;
            currentTrackIndex = -1;
        }

        public void PlayNext()
        {
            if (playlist?.Tracks == null || playlist.Tracks.Count == 0) return;

            currentTrackIndex = (currentTrackIndex + 1) % playlist.Tracks.Count;
            PlayTrack(playlist.Tracks[currentTrackIndex].clip);
        }

        public void PlayPrevious()
        {
            if (playlist?.Tracks == null || playlist.Tracks.Count == 0) return;

            currentTrackIndex = (currentTrackIndex - 1 + playlist.Tracks.Count) % playlist.Tracks.Count;
            PlayTrack(playlist.Tracks[currentTrackIndex].clip);
        }
        
        public void PlayTrack(int trackIndex)
        {
            if (playlist?.Tracks == null) return;

            if (trackIndex >= 0 && trackIndex < playlist.Tracks.Count)
            {
                PlayTrack(playlist.Tracks[trackIndex].clip);
            }
            else
            {
                Debug.LogWarning($"Track index '{trackIndex}' out of range.");
            }
        }

        public void PlayTrack(string trackName)
        {
            if (playlist?.Tracks == null) return;

            var track = playlist.Tracks.Find(t => t.name == trackName);
            if (track != null)
            {
                PlayTrack(track.clip);
            }
            else
            {
                Debug.LogWarning($"Track '{trackName}' not found in the playlist.");
            }
        }

        public void PlayTrack(AudioClip clip)
        {
            if (!IsInitialized || audioSources == null || audioSources.Length < 2) return;

            int nextSourceIndex = (activeSourceIndex + 1) % audioSources.Length;
        
            audioSources[nextSourceIndex].clip = clip;
            audioSources[nextSourceIndex].Play();

            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            fadeCoroutine = StartCoroutine(CrossfadeAudioSources(activeSourceIndex, nextSourceIndex));
        
            activeSourceIndex = nextSourceIndex;
        }

        private IEnumerator CrossfadeAudioSources(int fromIndex, int toIndex)
        {
            float t = 0;
            while (t < crossfadeDuration)
            {
                t += Time.deltaTime;
                float normalizedTime = t / crossfadeDuration;

                audioSources[fromIndex].volume = 1 - normalizedTime;
                audioSources[toIndex].volume = normalizedTime;

                yield return null;
            }

            audioSources[fromIndex].Stop();
            fadeCoroutine = null;
        }

        public void Stop()
        {
            foreach (var source in audioSources)
            {
                source.Stop();
            }
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
                fadeCoroutine = null;
            }
        }

        public void Pause()
        {
            foreach (var source in audioSources)
            {
                source.Pause();
            }
        }

        public void Resume()
        {
            foreach (var source in audioSources)
            {
                source.UnPause();
            }
        }

        public void SetVolume(float volume)
        {
            foreach (var source in audioSources)
            {
                source.volume = volume;
            }
        }
    }
}