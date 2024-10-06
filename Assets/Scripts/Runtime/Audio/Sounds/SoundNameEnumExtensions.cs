using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Starblast.Audio
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public static class SoundNameEnumExtensions
    {
        private static Dictionary<SoundNameEnum, string> _soundNameCache;

        static SoundNameEnumExtensions()
        {
            _soundNameCache = new Dictionary<SoundNameEnum, string>();

#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
        }

#if UNITY_EDITOR
        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            // Reset the cache when entering or exiting Play mode
            if (state == PlayModeStateChange.EnteredPlayMode || state == PlayModeStateChange.ExitingPlayMode)
            {
                _soundNameCache.Clear();
            }
        }
#endif

        public static string ToName(this SoundNameEnum soundName)
        {
            if (_soundNameCache == null)
            {
                _soundNameCache = new Dictionary<SoundNameEnum, string>();
            }

            if (_soundNameCache.TryGetValue(soundName, out var name))
            {
                return name;
            }

            name = soundName.ToString();
            _soundNameCache.Add(soundName, name);
            return name;
        }
    }
}