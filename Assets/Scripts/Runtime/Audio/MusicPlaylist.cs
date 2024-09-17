using System.Collections.Generic;
using UnityEngine;

namespace Starblast.Audio
{
    [CreateAssetMenu(fileName = "MusicPlaylist", menuName = "Starblast/Data/Audio/Music Playlist")]
    public class MusicPlaylist : ScriptableObject
    {
        [System.Serializable]
        public class TrackInfo
        {
            public string name;
            public AudioClip clip;
        }

        [field: SerializeField]
        public  List<TrackInfo> Tracks { get; private set; }
    }
}